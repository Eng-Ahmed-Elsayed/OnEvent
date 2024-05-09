using AutoMapper;
using DataAccess.UnitOfWork.Classes;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DataTransferObjects;
using Models.Models;
using Utility.FileManager;
using Utility.Text;
using Utility.Validatiors;

namespace OnEvent.Areas.EventManagement.Controllers
{
    [Area("EventManagement")]
    [Route("dashboard/events")]
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventsController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IFileManagerService _fileManagerService;


        public EventsController(IUnitOfWork unitOfWork,
            ILogger<EventsController> logger,
            IMapper mapper,
            UserManager<User> userManager,
            IFileManagerService fileManagerService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _fileManagerService = fileManagerService;
        }

        // GET: dashboard/events
        public async Task<IActionResult> Index([FromQuery] Parameters? parameters, string? searchString)
        {
            try
            {
                if (parameters == null)
                {
                    parameters = new Parameters()
                    {
                        OrderBy = "title",
                        PageNumber = 1,
                        PageSize = 5,
                    };
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Get the events
                var events = await _unitOfWork.EventRepository.GetListAsync(x =>
                !x.IsDeleted
                && x.OrganizerId == organizer.Id
                && (searchString.IsNullOrEmpty() ? true
                : (x.Title.Contains(searchString)
                    || x.Location.Contains(searchString)
                    || x.Date.ToString().Contains(searchString)))
                , parameters);
                // Add the parameters to the ViewBag
                ViewBag.searchString = searchString;
                ViewBag.orderBy = parameters.OrderBy.IsNullOrEmpty()
                    ? ""
                    : parameters.OrderBy.ToLower();
                // Toast messages if redirected
                ViewData["ToastHeader"] = TempData["ToastHeader"]?.ToString();
                ViewData["ToastMessage"] = TempData["ToastMessage"]?.ToString();
                return View(_mapper.Map<ViewPagedList<EventDto>>(events));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        // GET: dashboard/events/Details/{id}
        [Route("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Get the event if it is not null return it, else redirect
                var eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == id
                                    && !x.IsDeleted
                                    && x.OrganizerId == organizer.Id,
                    "Invitations,Guests,Logistics");
                ViewData["ToastHeader"] = TempData["ToastHeader"]?.ToString();
                ViewData["ToastMessage"] = TempData["ToastMessage"]?.ToString();
                return eventObj != null ? View(_mapper.Map<EventDto>(eventObj)) : RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        // GET: dashboard/events/Create
        [Route("create")]
        public IActionResult Create()
        {
            try
            {
                EventDto eventDto = new();
                return View("Upsert", eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        // POST: dashboard/events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create(EventDto eventDto)
        {
            try
            {
                // If no image add model error
                if (eventDto.ImageFile == null)
                {
                    ModelState.AddModelError("ImageFile", "The ImageFile field is required.");
                }
                // If the model is not vaild
                if (!ModelState.IsValid || eventDto == null)
                {
                    return View("Upsert", eventDto);
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Save event image
                var dbPath = await _fileManagerService.UploadFile(eventDto.ImageFile, "Events");
                // Create new event object
                Event eventObj = new()
                {
                    Id = Guid.NewGuid(),
                    Organizer = organizer,
                    Title = StringGlobalization.ToTitleCase(eventDto.Title),
                    Brief = eventDto.Brief,
                    Agenda = eventDto.Agenda,
                    Description = eventDto.Description,
                    Date = eventDto.Date,
                    Time = eventDto.Time,
                    Location = StringGlobalization.ToTitleCase(eventDto.Location),
                    LocationType = eventDto.LocationType,
                    ImgPath = dbPath,
                    CreatedAt = DateTime.UtcNow,
                };
                // Add logistics
                Logistics logistics = eventDto.Logistics;
                logistics.Id = Guid.NewGuid();
                logistics.EventId = eventObj.Id;
                eventObj.Logistics = logistics;
                // Add new event then save
                await _unitOfWork.EventRepository.InsertAsync(eventObj);
                await _unitOfWork.SaveChangesAsync();
                TempData["ToastMessage"] = "You have created new event successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        // GET: dashboard/events/{id}/Edit
        [Route("{id:guid}/edit")]
        public async Task<IActionResult> Edit(Guid id)
        {

            try
            {
                Event eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == id,
                    "Logistics");
                return eventObj != null
                    ? View("Upsert", _mapper.Map<EventDto>(eventObj))
                    : RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            };
        }

        // POST: dashboard/events/{id}/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id:guid}/edit")]
        public async Task<IActionResult> Edit(Guid id, EventDto eventDto)
        {
            try
            {
                // If the model is not vaild
                if (!ModelState.IsValid || eventDto == null)
                {
                    return View("Upsert", eventDto);
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Get the event if it is not null update the redirect, else return to the same view 
                // with the eventObj
                Event eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == eventDto.Id
                    && x.OrganizerId == organizer.Id,
                    "Logistics");
                if (eventObj != null)
                {
                    // If new image delete old then Save.
                    if (eventDto.ImageFile != null)
                    {
                        _fileManagerService.DeleteFile(eventObj.ImgPath);
                        eventObj.ImgPath = await _fileManagerService.UploadFile(eventDto.ImageFile,
                                        "Events");
                    }
                    eventObj.Title = StringGlobalization.ToTitleCase(eventDto.Title);
                    eventObj.Description = eventDto.Description;
                    eventObj.Date = eventDto.Date;
                    eventObj.Time = eventDto.Time;
                    eventObj.Location = StringGlobalization.ToTitleCase(eventDto.Location);
                    eventObj.LocationType = eventDto.LocationType;
                    eventObj.Logistics.EquipmentNeeded = eventDto.Logistics.EquipmentNeeded;
                    eventObj.Logistics.CateringDetails = eventDto.Logistics.CateringDetails;
                    eventObj.Logistics.TransportationArrangements = eventDto.Logistics.TransportationArrangements;
                    eventObj.UpdateAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();
                    TempData["ToastMessage"] = $"You have updated {eventObj.Title} event successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View("Upsert", eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            };
        }


        // POST: dashboard/events/{id}/Delete/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateEventOrganizer]
        [Route("{id:guid}/delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                // Get the organizer
                var eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == id
                    && !x.IsDeleted);
                if (eventObj != null)
                {
                    if (eventObj.ImgPath != null)
                    {
                        _fileManagerService.DeleteFile(eventObj.ImgPath);
                    }
                    await _unitOfWork.EventRepository.DeleteAsync(eventObj.Id);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["ToastMessage"] = "You have deleted this event successfully!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
