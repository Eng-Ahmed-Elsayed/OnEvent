using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Utility.Validatiors
{
    /// <summary>
    /// Check if there is an Event with eventId and also not deleted.
    /// </summary>
    public class ValidateEventExistsAttribute : TypeFilterAttribute
    {
        public ValidateEventExistsAttribute() : base(typeof(ValidateEventExistsFilterImpl))
        {
        }

        private class ValidateEventExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IUnitOfWork _unitOfWork;


            public ValidateEventExistsFilterImpl(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.ActionArguments.TryGetValue("eventId", out object eventIdObject) && eventIdObject is Guid eventId)
                {
                    var eventExists = await _unitOfWork.EventRepository.GetAsync(e => e.Id == eventId && !e.IsDeleted);
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
