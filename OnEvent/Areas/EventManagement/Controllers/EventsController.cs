using AutoMapper;
using DataAccess.UnitOfWork.Classes;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DataTransferObjects;
using Models.Models;

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


        public EventsController(IUnitOfWork unitOfWork,
            ILogger<EventsController> logger,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }
        // GET: EventsController
        public async Task<IActionResult> Index()
        {
            try
            {
                Parameters parameters = new Parameters()
                {
                    OrderBy = "Title",
                    PageNumber = 1,
                    PageSize = 20,
                };
                var events = await _unitOfWork.EventRepository.GetListAsync(x => !x.IsDeleted, parameters);
                return View(_mapper.Map<List<EventDto>>(events));
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
                var eventObj = await _unitOfWork.EventRepository.GetByIdAsync(id);

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
                return View(eventObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        // POST: EventsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventDto eventDto)
        {
            try
            {
                // If the model is not vaild
                if (!ModelState.IsValid || eventDto == null)
                {
                    return BadRequest(ModelState);
                }
                // Get the organizer
                var organizer = await _userManager.FindByEmailAsync(User.Identity.Name);
                // Create new event object
                Event eventObj = new()
                {
                    Id = Guid.NewGuid(),
                    Organizer = organizer,
                    CreatedAt = DateTime.UtcNow,
                    Title = eventDto.Title,
                    Description = eventDto.Description,
                    Date = eventDto.Date,
                    Location = eventDto.Location,
                    //Logistics = eventDto.Logistics,
                };
                // Add new event then save
                await _unitOfWork.EventRepository.InsertAsync(eventObj);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            }
        }

        // GET: EventsController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {

            try
            {
                Event eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == id);
                return eventObj != null ? View(_mapper.Map<EventDto>(eventObj)) : RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
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
                    return BadRequest(ModelState);
                }
                // Get the event if it is not null update the redirect, else return to the same view 
                // with the eventObj
                Event eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == eventDto.Id);
                if (eventObj != null)
                {
                    eventObj.Title = eventDto.Title;
                    eventObj.Description = eventDto.Description;
                    eventObj.Date = eventDto.Date;
                    eventObj.Location = eventDto.Location;
                    eventObj.Logistics = eventDto.Logistics;
                    eventObj.UpdateAt = DateTime.UtcNow;

                    await _unitOfWork.EventRepository.UpdateAsync(eventObj);
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View();
            };
        }


        // POST: EventsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EventDto eventDto)
        {
            try
            {
                if (ModelState.IsValid || eventDto == null)
                {
                    return BadRequest(ModelState);
                }
                var eventObj = await _unitOfWork.EventRepository.GetAsync(x => x.Id == eventDto.Id);
                if (eventObj != null)
                {
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
