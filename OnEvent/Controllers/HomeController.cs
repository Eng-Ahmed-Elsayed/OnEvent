using System.Diagnostics;
using AutoMapper;
using DataAccess.UnitOfWork.Classes;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DataTransferObjects;
using Models.Models;
using OnEvent.Models;

namespace OnEvent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public HomeController(ILogger<HomeController> logger,
            IMapper mapper,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork
            )
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> IndexAsync()
        {
            //ViewData["ToastMessage"] = "Welcome to my ASP.NET Core MVC application!";
            Parameters parameters = new Parameters()
            {
                OrderBy = "title",
                PageNumber = 1,
                PageSize = 3,
            };
            var events = await _unitOfWork.EventRepository.GetListAsync(x =>
            !x.IsDeleted, parameters);

            return View(_mapper.Map<ViewPagedList<EventDto>>(events));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
