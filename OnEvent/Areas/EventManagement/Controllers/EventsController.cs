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

namespace OnEvent.Areas.EventManagement.Controllers
{
    [Area("EventManagement")]
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

        // GET: EventsController
        /// Ex for QueryString localhost:5001/api/events?pageNumber=2&pageSize=2
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
                        PageSize = 3,
                    };
                }
                parameters.PageSize = 3;
                var events = await _unitOfWork.EventRepository.GetListAsync(x => !x.IsDeleted
                && searchString.IsNullOrEmpty() ? true
                : (x.Title.Contains(searchString)
                    || x.Location.Contains(searchString)
                    || x.Date.ToString().Contains(searchString))
                , parameters);
                // Add the parameters to the ViewBag
                ViewBag.searchString = searchString;
                ViewBag.orderBy = parameters.OrderBy.IsNullOrEmpty()
                    ? ""
                    : parameters.OrderBy.ToLower();
                return View(_mapper.Map<ViewPagedList<EventDto>>(events));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        // GET: EventsController/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Get the event if it is not null return it, else redirect
                var eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == id && !x.IsDeleted,
                    "Invitations,Guests,Logistics");

                return eventObj != null ? View(_mapper.Map<EventDto>(eventObj)) : RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        // GET: EventsController/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                EventDto eventObj = new();
                return View("Upsert", eventObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        // POST: EventsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    Title = eventDto.Title.ToUpper(),
                    Description = eventDto.Description,
                    Date = eventDto.Date,
                    Location = eventDto.Location,
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
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View("Upsert");
            }
        }

        // GET: EventsController/Edit/5
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

        // POST: EventsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EventDto eventDto)
        {
            try
            {
                // If the model is not vaild
                if (!ModelState.IsValid || eventDto == null)
                {
                    return View("Upsert", eventDto);
                }
                // Get the event if it is not null update the redirect, else return to the same view 
                // with the eventObj
                Event eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == eventDto.Id,
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
                    eventObj.Title = eventDto.Title.ToUpper();
                    eventObj.Description = eventDto.Description;
                    eventObj.Date = eventDto.Date;
                    eventObj.Location = eventDto.Location;
                    eventObj.LocationType = eventDto.LocationType;
                    eventObj.Logistics.EquipmentNeeded = eventDto.Logistics.EquipmentNeeded;
                    eventObj.Logistics.CateringDetails = eventDto.Logistics.CateringDetails;
                    eventObj.Logistics.TransportationArrangements = eventDto.Logistics.TransportationArrangements;
                    eventObj.UpdateAt = DateTime.UtcNow;

                    await _unitOfWork.EventRepository.UpdateAsync(eventObj);
                    await _unitOfWork.SaveChangesAsync();
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


        // POST: EventsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == null || id == Guid.Empty)
                {
                    return BadRequest();
                }
                var eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == id);
                if (eventObj != null)
                {
                    if (eventObj.ImgPath != null)
                    {
                        _fileManagerService.DeleteFile(eventObj.ImgPath);
                    }
                    await _unitOfWork.EventRepository.DeleteAsync(eventObj.Id);
                    await _unitOfWork.SaveChangesAsync();
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
