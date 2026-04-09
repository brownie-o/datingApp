using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// fallback route handler: if no other endpoints match, fallback to this controller and return the index.html file in wwwroot folder

// Controller: A base class for an MVC controller with view suppport. This is used when you want to return views from your controller actions.
public class FallbackController : Controller
{
  public ActionResult Index()
  {
    // Directory.GetCurrentDirectory(): get the app's root folder, which is the API project folder
    // Path.Combine(): and then combine the app's root folder with wwwroot/index.html to get the full path of the index.html file
    // PhysicalFile(..., "text/HTML"): returns a real file from disk as the HTTP response, and specify the content type as text/HTML
    return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
  }
}
