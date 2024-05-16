using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Models.Models;

namespace Utility.Validatiors
{
    /// <summary>
    /// A custom authorization filter that checks if the current user is authorized
    /// to access and edit the specified guest's information. It retrieves the guest
    /// based on the provided ID, verifies the guest's existence, checks the linkage
    /// to a user account, and ensures the authenticated user has the necessary
    /// permissions. If any of these checks fail, appropriate actions are taken.
    /// </summary>
    public class GuestAuthorizationAttribute : TypeFilterAttribute
    {
        public GuestAuthorizationAttribute() : base(typeof(ValidateEventExistsFilterImpl))
        {
        }

        private class ValidateEventExistsFilterImpl : IAsyncActionFilter
        {
            private readonly ILogger<GuestAuthorizationAttribute> _logger;
            private readonly UserManager<User> _userManager;
            private readonly IUnitOfWork _unitOfWork;


            public ValidateEventExistsFilterImpl(IUnitOfWork unitOfWork,
                UserManager<User> userManager,
                ILogger<GuestAuthorizationAttribute> logger
                )
            {
                _unitOfWork = unitOfWork;
                _userManager = userManager;
                _logger = logger;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    if (context.ActionArguments.TryGetValue("id", out var idValue) && idValue is Guid id)
                    {
                        var guest = await _unitOfWork.GuestRepository.GetAsync(x => x.Id == id, "Event,RSVP");
                        if (guest == null)
                        {
                            context.Result = new NotFoundResult();
                            return;
                        }
                        // Check if this guest is linked to an account or not
                        if (!guest.UserId.IsNullOrEmpty())
                        {
                            var user = await _userManager.FindByEmailAsync(context.HttpContext.User.Identity.Name ?? "");
                            // If not user redirect him to Login page
                            if (user == null)
                            {
                                context.Result = new RedirectToActionResult("Login", "Account", new
                                {
                                    area = "Identity",
                                    returnUrl = context.HttpContext.Request.Path.ToString()
                                });
                                return;
                            }
                            // If the user is the organizer redirect him to edit guest action in Guest Controller.
                            if (user.Id == guest.Event.OrganizerId)
                            {
                                context.Result = new RedirectToActionResult("EditGuest", "Guest", new
                                {
                                    area = "GuestManagement",
                                    eventId = guest.EventId,
                                    guestId = guest.Id
                                });
                                return;
                            }
                            // If user but he is not the guest
                            if (user.Id != guest.UserId)
                            {
                                context.Result = new UnauthorizedResult();
                                return;
                            }
                        }
                    }
                    // If guest without user or the guest oner.
                    await next();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    context.Result = new StatusCodeResult(500);
                }
            }
        }
    }
}
