using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Models;

namespace Utility.Validatiors
{
    public class ValidateEventOrganizerAttribute : TypeFilterAttribute
    {
        public ValidateEventOrganizerAttribute() : base(typeof(ValidateEventOrganizeFilterImpl))
        {
        }

        private class ValidateEventOrganizeFilterImpl : IAsyncActionFilter
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<User> _userManager;


            public ValidateEventOrganizeFilterImpl(IUnitOfWork unitOfWork, UserManager<User> userManager)
            {
                _unitOfWork = unitOfWork;
                _userManager = userManager;
            }

            /// <summary>
            /// Validate if the event organizer is the user 
            /// and if there is an event for this user with this id.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="next"></param>
            /// <returns></returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.ActionArguments.TryGetValue("eventId", out object eventIdObject) && eventIdObject is Guid eventId)
                {
                    // Get the organizer
                    var organizer = await _userManager.FindByEmailAsync(context.HttpContext.User.Identity.Name);
                    if (organizer == null)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    var eventExists = await _unitOfWork.EventRepository.GetAsync(e => e.Id == eventId
                                        && !e.IsDeleted
                                        && e.OrganizerId == organizer.Id);
                    if (eventExists == null)
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }
                }
                else
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                await next();
            }
        }
    }
}
