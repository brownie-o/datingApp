using System;
using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

// This class is meant to be used as an attribute on controller actions to log user activity. It implements the IAsyncActionFilter interface, which allows it to execute code before and after an action method is called. The OnActionExecutionAsync method will contain the logic for logging user activity.
public class LogUserActivity : IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    // anything that happens after next() is going to happen after the action is been executed in the API controller.
    var resultContext = await next();

    // check if the user is authenticated before trying to log their activity.
    if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

    var memberId = resultContext.HttpContext.User.GetMemberId();

    // update database, using service loacte pattern to get hold the dbcontext
    var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

    // update the LaseActive property of the member in the database to the current UTC time. 
    await dbContext.Members
      .Where(x => x.Id == memberId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastActive, DateTime.UtcNow));
  }
}
