using AutoMapper;
using DataAccess.UnitOfWork.Classes;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DataTransferObjects;
using Models.Enums;
using Models.Models;
using Utility.Communication.Mail;
using Utility.Communication.MailTemplates;
using Utility.FileManager;
using Utility.Validatiors;

namespace OnEvent.Areas.InvitationManagement.Controllers
{
    [Area("InvitationManagement")]
    [Route("dashboard/events/{eventId:guid}/invitations")]
    [Authorize]
    [ValidateEventOrganizer]
    public class InvitationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvitationController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IFileManagerService _fileManagerService;
        private readonly IEmailService _emailService;
        private readonly IMailTemplate _mailTemplate;


        public InvitationController(IUnitOfWork unitOfWork,
            ILogger<InvitationController> logger,
            IMapper mapper,
            UserManager<User> userManager,
            IFileManagerService fileManagerService,
            IEmailService emailService,
            IMailTemplate mailTemplate)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _fileManagerService = fileManagerService;
            _emailService = emailService;
            _mailTemplate = mailTemplate;
        }

        /// <summary>
        /// Invitation List
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="parameters"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(Guid eventId,
            [FromQuery] Parameters? parameters,
            string? searchString)
        {
            try
            {
                if (parameters == null)
                {
                    parameters = new Parameters()
                    {
                        OrderBy = "Status",
                        PageNumber = 1,
                        PageSize = 3,
                    };
                }
                var x = await _unitOfWork.InvitationRepository.GetListAsync(x => !x.IsDeleted
                && x.EventId == eventId);
                var invitations = await _unitOfWork.InvitationRepository.GetListAsync(x => !x.IsDeleted
                && x.EventId == eventId
                && (searchString.IsNullOrEmpty() ? true
                : (x.GuestEmail.Contains(searchString)
                    || x.ResponseDate.ToString().Contains(searchString)))
                , parameters);
                // Add the parameters to the ViewBag
                ViewBag.searchString = searchString;
                ViewBag.orderBy = parameters.OrderBy.IsNullOrEmpty()
                    ? ""
                    : parameters.OrderBy.ToLower();
                // Toast messages if redirected
                ViewData["ToastHeader"] = TempData["ToastHeader"]?.ToString();
                ViewData["ToastMessage"] = TempData["ToastMessage"]?.ToString();
                return View(_mapper.Map<ViewPagedList<InvitationDto>>(invitations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        /// <summary>
        /// Get an invitation by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{invitationId:guid}")]
        public async Task<IActionResult> Details([FromRoute] Guid invitationId, Guid eventId)
        {
            try
            {
                // Get the event if it is not null return it, else redirect
                var invitationObj = await _unitOfWork.InvitationRepository.GetAsync(x => x.Id == invitationId
                    && !x.IsDeleted
                    && x.EventId == eventId,
                    "Event,Guest");

                return invitationObj != null ? View(_mapper.Map<InvitationDto>(invitationObj)) : RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        [Route("create")]
        public async Task<IActionResult> Create(Guid eventId)
        {
            try
            {
                InvitationDto invitationDto = new();
                return View("Upsert", invitationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create(InvitationDto invitationDto, Guid eventId)
        {
            try
            {
                // If the model is not vaild
                if (!ModelState.IsValid || invitationDto == null)
                {
                    return View("Upsert", invitationDto);
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Create new invitation object
                Invitation invitationObj = new()
                {
                    Id = Guid.NewGuid(),
                    GuestEmail = invitationDto.GuestEmail,
                    Status = InvitationStatus.Sent,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                };
                // Get the event
                var eventObj = await _unitOfWork.EventRepository.GetByIdAsync(eventId);
                // Create new email craft
                EmailCraft emailCraft = new()
                {
                    Id = Guid.NewGuid(),
                    User = organizer,
                    EventId = eventId,
                    EmailForReceiver = invitationDto.GuestEmail,
                    Timestamp = DateTime.UtcNow,
                    Subject = $"Invitation to {eventObj.Title}",
                    Message = await _mailTemplate.Invitation(eventObj, invitationObj.Id),
                };
                // Sent an email to this guest
                await _emailService.SendEmailAsync(emailCraft);
                // Add notification to organizer
                Notification notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    UserId = organizer.Id,
                    EventId = eventId,
                    Timestamp = DateTime.UtcNow,
                    Subject = $"New Invitation to event has been sent.",
                    Message = $"An Invitation to '{eventObj.Title}' event has been sent to {invitationDto.GuestEmail} successfully"
                };
                // Add and save
                await _unitOfWork.InvitationRepository.InsertAsync(invitationObj);
                await _unitOfWork.EmailCraftRepository.InsertAsync(emailCraft);
                await _unitOfWork.NotificationRepository.InsertAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("send-reminder-email/{invitationId:guid}")]
        public async Task<IActionResult> SendReminderEmail([FromRoute] Guid invitationId, Guid eventId)
        {
            try
            {
                // If the model is not vaild
                if (!ModelState.IsValid || invitationId == Guid.Empty || eventId == Guid.Empty)
                {
                    return View(nameof(Index), new { eventId });
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Get the event
                var eventObj = await _unitOfWork.EventRepository.GetByIdAsync(eventId);
                // Get the invetation
                var invitation = await _unitOfWork.InvitationRepository.GetByIdAsync(invitationId);
                // Check if null
                if (organizer == null || eventObj == null || invitation == null)
                {
                    return View(nameof(Index), new { eventId });
                }
                // Create new email craft
                EmailCraft emailCraft = new()
                {
                    Id = Guid.NewGuid(),
                    User = organizer,
                    EventId = eventId,
                    EmailForReceiver = invitation.GuestEmail,
                    Timestamp = DateTime.UtcNow,
                    Subject = $"Reminder Invitation to {eventObj.Title}",
                    Message = await _mailTemplate.Reminder(eventObj, invitation.Id),
                };
                // Sent an email to this guest
                await _emailService.SendEmailAsync(emailCraft);
                // Add notification to organizer
                Notification notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    UserId = organizer.Id,
                    EventId = eventId,
                    Timestamp = DateTime.UtcNow,
                    Subject = $"New Reminder Invitation to event has been sent.",
                    Message = $"A Reminder Invitation to '{eventObj.Title.ToUpper()}' event has been sent to {invitation.GuestEmail} successfully"
                };
                // Add and save
                await _unitOfWork.EmailCraftRepository.InsertAsync(emailCraft);
                await _unitOfWork.NotificationRepository.InsertAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("remind-all")]
        public async Task<IActionResult> RemindAll(Guid eventId)
        {
            try
            {
                // If the model is not vaild
                if (!ModelState.IsValid || eventId == Guid.Empty)
                {
                    return View(nameof(Index), new { eventId });
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Get the event
                var eventObj = await _unitOfWork.EventRepository.GetAsync(x =>
                x.Id == eventId,
                "Invitations");
                // Check if null
                if (organizer == null || eventObj == null)
                {
                    return View(nameof(Index), new { eventId });
                }
                if (eventObj.Invitations.Count() > 0)
                {
                    foreach (var invitation in eventObj.Invitations)
                    {

                        // Create new email craft
                        EmailCraft emailCraft = new()
                        {
                            Id = Guid.NewGuid(),
                            User = organizer,
                            EventId = eventId,
                            EmailForReceiver = invitation.GuestEmail,
                            Timestamp = DateTime.UtcNow,
                            Subject = $"Reminder Invitation to {eventObj.Title}",
                            Message = await _mailTemplate.Reminder(eventObj, invitation.Id),
                        };
                        // Sent an email to this guest
                        await _emailService.SendEmailAsync(emailCraft);
                        await _unitOfWork.EmailCraftRepository.InsertAsync(emailCraft);

                    }
                    // Add notification to organizer
                    Notification notification = new Notification()
                    {
                        Id = Guid.NewGuid(),
                        UserId = organizer.Id,
                        EventId = eventId,
                        Timestamp = DateTime.UtcNow,
                        Subject = $"New Reminder Invitation email for {eventObj.Title} was sent to all guests.",
                        Message = $"A Reminder Invitation email to '{eventObj.Title}' event has been sent to all gests successfully"
                    };
                    // Add and save
                    await _unitOfWork.NotificationRepository.InsertAsync(notification);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["ToastMessage"] = $"You have sent a reminder email to all guests!";
                }
                else
                {
                    TempData["ToastMessage"] = $"There are no guests to be reminded, please add guests first!";
                }
                return RedirectToAction(nameof(Index), new { eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }



    }
}

