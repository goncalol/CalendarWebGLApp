using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalAPI.Data;
using PersonalAPI.Messages;
using System;
using System.Linq;

namespace PersonalAPI.Controllers
{
    //[ApiController]
    //[AllowAnonymous]
    //[Route("[controller]")]
    //public class TestController : ControllerBase
    //{
    //    private readonly ILogger<TestController> _logger;
    //    private readonly PersonalAppContext _context;

    //    public TestController(PersonalAppContext context, ILogger<TestController> logger)
    //    {
    //        _logger = logger;
    //        _context = context;
    //    }

    //    [HttpGet]
    //    public CalendarEventsWithCategories Get()
    //    {
    //        var ownerId = "07a57474-c460-42d0-969d-db12e194a314";
    //        var events = _context.Todos.Where(e => e.UserId == ownerId).Include(e => e.Category).ToArray();
    //        var categories = _context.Categories.ToArray();
    //        return CalendarEventsWithCategories.Create(events, categories);
    //    }

    //    [HttpPost]
    //    [Consumes("application/x-www-form-urlencoded")]
    //    public string CreateEvent([FromForm] CalendarEventCreate calendarEvent)
    //    {

    //        var ownerId = "07a57474-c460-42d0-969d-db12e194a314";
    //        //var bla = user.Identity.FindFirst(JwtRegisteredClaimNames.UniqueName).Value;
    //        var invalidMsg = ValidateTodo(calendarEvent);
    //        if (invalidMsg != null) return invalidMsg;

    //        var category = _context.Categories.FirstOrDefault(e => e.Name == calendarEvent.Category);
    //        if (category == null) return "Invalid Category";

    //        _context.Todos.AddRange(Todo.Create(calendarEvent, category, ownerId));
    //        _context.SaveChanges();

    //        return null;
    //    }

    //    [HttpDelete("{id:int}")]
    //    public string RemoveEvent(int id)
    //    {
    //        var ownerId = "07a57474-c460-42d0-969d-db12e194a314";

    //        var todo = _context.Todos.FirstOrDefault(e => e.Id == id && e.UserId == ownerId);
    //        if (todo != null) _context.Todos.Remove(todo);
    //        else return "Not allowed to remove the event with this Id";

    //        _context.SaveChanges();

    //        return null;
    //    }

    //    private string ValidateTodo(CalendarEventCreate todo)
    //    {
    //        if (todo == null || todo.Title == null || todo.Start == null)
    //        {
    //            return "Must send the right parameters";
    //        }
    //        else
    //        {
    //            DateTime startDate;
    //            if (!DateTime.TryParse(todo.Start, out startDate))
    //                return "Start date has not the right format";
    //            if (!string.IsNullOrWhiteSpace(todo.End))
    //            {
    //                DateTime endDate;
    //                if (!DateTime.TryParse(todo.End, out endDate))
    //                    return "End date has not the right format";

    //                if (endDate <= startDate)
    //                    return "End date must be after Start date";
    //            }
    //        }

    //        return null;
    //    }
    //}
}
