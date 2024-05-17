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
using OnEvent.Areas.InvitationManagement.Controllers;
using Utility.Communication.Mail;
using Utility.Communication.MailTemplates;
using Utility.FileManager;
using Utility.Validatiors;

namespace OnEvent.Areas.GuestManagement.Controllers
{
    [Area("GuestManagement")]
    [Route("guests")]
    public class GuestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvitationController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IFileManagerService _fileManagerService;
        private readonly IEmailService _emailService;
        private readonly IMailTemplate _mailTemplate;

        public GuestController(IUnitOfWork unitOfWork,
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


        #region Guest Actions

        [Authorize]
        public async Task<IActionResult> Index([FromQuery] Parameters? parameters,
            string? searchString)
        {

            try
            {
                // Add default parameters
                if (parameters == null)
                {
                    parameters = new Parameters()
                    {
                        OrderBy = "name",
                        PageNumber = 1,
                        PageSize = 5,
                    };
                }
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Search for the guests
                var guests = await _unitOfWork.GuestRepository.GetListAsync(x =>
                x.UserId == user.Id
                && !x.IsDeleted
                && (searchString.IsNullOrEmpty() ? true
                : (x.Name.Contains(searchString)
                    || x.Email.Contains(searchString)
                    || x.Event.Title.Contains(searchString))),
                parameters,
                "RSVP,Event");
                // Add the parameters to the ViewBag
                ViewBag.searchString = searchString;
                ViewBag.orderBy = parameters.OrderBy.IsNullOrEmpty()
                    ? ""
                    : parameters.OrderBy.ToLower();
                // Toast messages if redirected
                ViewData["ToastHeader"] = TempData["ToastHeader"]?.ToString();
                ViewData["ToastMessage"] = TempData["ToastMessage"]?.ToString();
                return View("GuestsList", _mapper.Map<ViewPagedList<GuestDto>>(guests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [Route("registration/{invitationId:guid}")]
        public async Task<IActionResult> GuestRegistration([FromRoute] Guid invitationId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name ?? "");
                var invitation = await _unitOfWork.InvitationRepository.GetAsync(
                    x => x.Id == invitationId,
                    "Event");
                // If no invitation
                if (invitation == null)
                {
                    TempData["ToastHeader"] = "Invaild Invitation";
                    TempData["ToastMessage"] = "You have an invaild invitation code!";
                    return NotFound();
                }

                // If the user is the organizer redirect him to AddGuest action.
                if (user?.Id == invitation.Event.OrganizerId)
                {
                    return RedirectToAction("AddGuest", new
                    {
                        eventId = invitation.EventId
                    });
                }
                Guest guest = await _unitOfWork.GuestRepository.GetAsync(x => x.InvitationId == invitationId);
                if (guest != null)
                {
                    TempData["ToastHeader"] = "Invaild Invitation";
                    TempData["ToastMessage"] = "There is a guest with this invitation!";
                    return BadRequest("There is a guest with this invitation");
                }
                GuestDto guestDto = new()
                {
                    Email = invitation.GuestEmail,
                    InvitationId = invitationId,
                    Event = _mapper.Map<EventDto>(invitation.Event),
                    UserId = user?.Id,
                };
                ViewBag.isOrganizer = false;
                return View("Upsert", guestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [Route("registration/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuestRegistration([FromRoute] Guid id, GuestDto guestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name ?? "");
                var invitation = await _unitOfWork.InvitationRepository.GetAsync(
                    x => x.Id == id,
                    "Event");
                // If the user is the organizer redirect him to AddGuest action.
                if (user?.Id == invitation.Event.OrganizerId)
                {
                    return RedirectToAction("AddGuest", new
                    {
                        eventId = invitation.EventId
                    });
                }
                // If no invitation
                if (invitation != null)
                {
                    TempData["ToastHeader"] = "Invaild Invitation";
                    TempData["ToastMessage"] = "You have an invaild invitation code!";
                    return NotFound();
                }
                // If invaild model
                if (!ModelState.IsValid)
                {
                    TempData["ToastHeader"] = "Invaild Data";
                    TempData["ToastMessage"] = "Please enter a vaild data!";
                    return View("Upsert", guestDto);
                }
                Guest guest = await _unitOfWork.GuestRepository.GetAsync(x => x.InvitationId == id);
                // If there is a guest with this invitation
                if (guest != null)
                {
                    TempData["ToastHeader"] = "Invaild Invitation";
                    TempData["ToastMessage"] = "There is a guest with this invitation!";
                    return BadRequest("There is a guest with this invitation");
                }
                // Create new guest
                guest = _mapper.Map<Guest>(guestDto);
                guest.Id = Guid.NewGuid();
                guest.InvitationId = id;
                guest.Event = invitation.Event;
                guest.CreatedAt = DateTime.Now;
                // If user
                if (user != null)
                {
                    guest.UserId = user.Id;
                    guest.Email = user.Email;
                }
                // Create new RSVP
                RSVP _RSVP = new()
                {
                    EventId = invitation.EventId,
                    GuestId = guest.Id,
                    RSVPStatus = guestDto.RSVP.RSVPStatus
                };
                // Update guest RSVP
                guest.RSVP = _RSVP;
                // Update the invitation
                invitation.Status = _RSVP.RSVPStatus == RSVPStatus.NotAttending
                    ? InvitationStatus.Declined
                    : InvitationStatus.Accepted;
                invitation.ResponseDate = DateTime.Now;
                // Create Confirmation email
                EmailCraft emailCraft = new()
                {
                    Id = Guid.NewGuid(),
                    User = guest.Event.Organizer,
                    EventId = guest.EventId,
                    EmailForReceiver = guest.Email,
                    Timestamp = DateTime.UtcNow,
                    Subject = $"Confirmation of Registration for {guest.Event.Title} - {guest.Event.Date} at {guest.Event.Time}",
                    Message = await _mailTemplate.Confirmation(guest),
                };
                // Save changes
                await _unitOfWork.GuestRepository.InsertAsync(guest);
                await _unitOfWork.RSVPRepository.InsertAsync(_RSVP);
                await _unitOfWork.EmailCraftRepository.InsertAsync(emailCraft);
                await _unitOfWork.SaveChangesAsync();
                // Sent an email to this guest
                await _emailService.SendEmailAsync(emailCraft);
                // Add toast message
                TempData["ToastHeader"] = "Register Successfully!";
                TempData["ToastMessage"] = $"You have registered to {invitation.Event.Title} " +
                    $"event successfully!";
                // Redirect to Info page.
                return RedirectToAction("Info",
                    new { guestId = guest.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [Route("{id:guid}/edit")]
        [HttpGet]
        [GuestAuthorization]
        public async Task<IActionResult> EditInfo([FromRoute] Guid id)
        {
            try
            {
                var guest = await _unitOfWork.GuestRepository.GetAsync(x => x.Id == id, "Event,RSVP");
                ViewBag.isOrganizer = false;
                return View("Upsert", _mapper.Map<GuestDto>(guest));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [Route("{id:guid}/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [GuestAuthorization]
        public async Task<IActionResult> EditInfo([FromRoute] Guid id, GuestDto guestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ToastHeader"] = "Invaild Data";
                    TempData["ToastMessage"] = "Please enter a vaild data!";
                    return View("Upsert", guestDto);
                }
                // Get the guest
                Guest guest = await _unitOfWork.GuestRepository.GetAsync(x =>
                x.Id == id,
                "Event,RSVP");
                if (guest == null)
                {
                    return NotFound();
                }
                var user = await _userManager.FindByEmailAsync(User.Identity.Name ?? "");
                // If logged in and the guest does not have a user link it to this user
                if (user != null && guest.UserId.IsNullOrEmpty())
                {
                    guest.UserId = user.Id;
                }
                // Update fields
                guest.Name = guestDto.Name;
                guest.Email = guestDto.Email;
                guest.RSVP.RSVPStatus = guestDto.RSVP.RSVPStatus;
                guest.MealPreference = guestDto.MealPreference;
                guest.UpdateAt = DateTime.Now;
                // Save changes
                await _unitOfWork.SaveChangesAsync();
                // Add toast message
                TempData["ToastHeader"] = "Updated Successfully!";
                TempData["ToastMessage"] = $"You have updated your info successfully!";
                // Redirect to info
                return RedirectToAction("Info",
                    new { guestId = guest.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [Route("{guestId:guid}/info")]
        [HttpGet]
        public async Task<IActionResult> Info([FromRoute] Guid guestId)
        {
            try
            {
                Guest guest = await _unitOfWork.GuestRepository.GetAsync(x =>
                x.Id == guestId,
                "Event,RSVP,Invitation");
                if (guest == null)
                {
                    return NotFound();
                }
                ViewData["ToastHeader"] = TempData["ToastHeader"]?.ToString();
                ViewData["ToastMessage"] = TempData["ToastMessage"]?.ToString();
                return View(_mapper.Map<GuestDto>(guest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }
        #endregion

        #region Organizer Manage Guests Actions
        // Organizer add guest manually
        [Route("~/dashboard/events/{eventId:guid}/guests/add")]
        [Authorize]
        [ValidateEventOrganizer]
        public async Task<IActionResult> AddGuest([FromRoute] Guid eventId)
        {
            try
            {
                var eventObj = await _unitOfWork.EventRepository.GetByIdAsync(eventId);
                GuestDto guestDto = new()
                {
                    EventId = eventId,
                    Event = _mapper.Map<EventDto>(eventObj)
                };
                ViewBag.isOrganizer = true;
                return View("Upsert", guestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/dashboard/events/{eventId:guid}/guests/add")]
        [Authorize]
        [ValidateEventOrganizer]
        public async Task<IActionResult> AddGuest([FromRoute] Guid eventId, GuestDto guestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ToastHeader"] = "Invaild Data";
                    TempData["ToastMessage"] = "Please enter a vaild data!";
                    return View("Upsert", guestDto);
                }
                // Create new invitation object
                Invitation invitationObj = new()
                {
                    Id = Guid.NewGuid(),
                    GuestEmail = guestDto.Email,
                    Status = InvitationStatus.Sent,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                };
                // Get the event
                var eventObj = await _unitOfWork.EventRepository.GetByIdAsync(eventId);

                // Create new guest
                Guest guest = _mapper.Map<Guest>(guestDto);
                guest.Id = Guid.NewGuid();
                guest.EventId = eventId;
                guest.Event = eventObj;
                guest.Invitation = invitationObj;
                // Create new RSVP
                RSVP _RSVP = new()
                {
                    EventId = eventId,
                    GuestId = guest.Id,
                    RSVPStatus = RSVPStatus.Undecided
                };
                // Add guest RSVP
                guest.RSVP = _RSVP;
                guest.CreatedAt = DateTime.Now;
                // Create new email craft
                EmailCraft emailCraft = new()
                {
                    Id = Guid.NewGuid(),
                    User = eventObj.Organizer,
                    EventId = eventId,
                    EmailForReceiver = guestDto.Email,
                    Timestamp = DateTime.UtcNow,
                    Subject = $"Invitation to {eventObj.Title}",
                    Message = await _mailTemplate.Confirmation(guest),
                };
                // Add and Save
                await _unitOfWork.InvitationRepository.InsertAsync(invitationObj);
                await _unitOfWork.EmailCraftRepository.InsertAsync(emailCraft);
                await _unitOfWork.RSVPRepository.InsertAsync(_RSVP);
                await _unitOfWork.GuestRepository.InsertAsync(guest);
                await _unitOfWork.SaveChangesAsync();
                // Sent an email to this guest
                await _emailService.SendEmailAsync(emailCraft);
                // Add toast message
                TempData["ToastHeader"] = "Register new guest Successfully!";
                TempData["ToastMessage"] = $"You have registered new guest to {eventObj.Title} successfully!";
                // Redirect to guests list
                return RedirectToAction("GuestsList", new { eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        // Guests list for specific event
        [Route("~/dashboard/events/{eventId:guid}/guests")]
        [Authorize]
        [ValidateEventOrganizer]
        public async Task<IActionResult> GuestsList(Guid eventId,
            [FromQuery] Parameters? parameters,
            string? searchString)
        {
            try
            {
                // Add default parameters
                if (parameters == null)
                {
                    parameters = new Parameters()
                    {
                        OrderBy = "name",
                        PageNumber = 1,
                        PageSize = 5,
                    };
                }
                // Search for guests
                var guests = await _unitOfWork.GuestRepository.GetListAsync(x =>
                x.EventId == eventId
                && !x.IsDeleted
                && (searchString.IsNullOrEmpty() ? true
                : (x.Name.Contains(searchString)
                    || x.Email.Contains(searchString))),
                parameters,
                "RSVP");
                // Add the parameters to the ViewBag
                ViewBag.searchString = searchString;
                ViewBag.orderBy = parameters.OrderBy.IsNullOrEmpty()
                    ? ""
                    : parameters.OrderBy.ToLower();
                // Toast messages if redirected
                ViewData["ToastHeader"] = TempData["ToastHeader"]?.ToString();
                ViewData["ToastMessage"] = TempData["ToastMessage"]?.ToString();
                // Event Name
                var eventObj = await _unitOfWork.EventRepository.GetByIdAsync(eventId);
                ViewData["EventName"] = eventObj?.Title;

                return View(_mapper.Map<ViewPagedList<GuestDto>>(guests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        // Delete Guest
        [Route("~/dashboard/events/{eventId:guid}/guests/{guestId:guid}/delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateEventOrganizer]
        [HttpPost]
        public async Task<IActionResult> DeleteGuest(Guid eventId, Guid guestId)
        {
            try
            {
                if (guestId == Guid.Empty || eventId == Guid.Empty)
                {
                    return BadRequest();
                }
                await _unitOfWork.GuestRepository.DeleteAsync(guestId);
                await _unitOfWork.SaveChangesAsync();
                TempData["ToastMessage"] = "You have deleted this guest successfully!";
                return RedirectToAction("GuestsList", new { eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [Route("~/dashboard/events/{eventId:guid}/guests/{guestId:guid}/edit")]
        [Authorize]
        [ValidateEventOrganizer]
        public async Task<IActionResult> EditGuest([FromRoute] Guid eventId, Guid guestId)
        {
            try
            {
                Guest guest = await _unitOfWork.GuestRepository.GetAsync(x =>
                x.Id == guestId,
                "Event,RSVP");
                if (guest == null)
                {
                    return NotFound();
                }
                ViewBag.isOrganizer = true;
                return View("Upsert", _mapper.Map<GuestDto>(guest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/dashboard/events/{eventId:guid}/guests/{guestId:guid}/edit")]
        [Authorize]
        [ValidateEventOrganizer]
        public async Task<IActionResult> EditGuest([FromRoute] Guid eventId,
            Guid guestId,
            GuestDto guestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ToastHeader"] = "Invaild Data";
                    TempData["ToastMessage"] = "Please enter a vaild data!";
                    return View("Upsert", guestDto);
                }

                // Create new guest
                Guest guest = await _unitOfWork.GuestRepository.GetAsync(x =>
                x.Id == guestId,
                "RSVP");
                // Add guest RSVP
                guest.Name = guestDto.Name;
                guest.Email = guestDto.Email;
                guest.MealPreference = guestDto.MealPreference;
                guest.UpdateAt = DateTime.Now;
                // Save
                await _unitOfWork.SaveChangesAsync();
                // Add toast message
                TempData["ToastMessage"] = $"You have updated guest {guest.Email} successfully!";
                // Redirect to guests list
                return RedirectToAction("GuestsList", new { eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500,
                    $"The server encountered an unexpected condition. Please try again later.");
            }
        }
        #endregion
    }
}
