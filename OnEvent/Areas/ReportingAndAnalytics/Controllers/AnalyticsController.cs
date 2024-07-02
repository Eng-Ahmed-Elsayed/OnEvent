using AutoMapper;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Utility.FileManager;

namespace OnEvent.Areas.ReportingAndAnalytics.Controllers
{
    [Area("ReportingAndAnalytics")]
    [Route("dashboard")]
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AnalyticsController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IFileManagerService _fileManagerService;

        public AnalyticsController(IUnitOfWork unitOfWork,
            ILogger<AnalyticsController> logger,
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
        public IActionResult Index()
        {
            return View();
        }
    }
}
