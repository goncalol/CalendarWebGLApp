using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalAPI.Data;
using PersonalAPI.Messages;
using System;
using System.Globalization;
using System.Linq;

namespace PersonalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PersonalAppContext _context;

        public HomeController(PersonalAppContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public CalendarEventsWithCategories Get()
        {
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var events = _context.Todos.Where(e=>e.UserId==ownerId).Include(e => e.Category).ToArray();
            var categories = _context.Categories.ToArray();
            return CalendarEventsWithCategories.Create(events, categories);
        }

        [HttpPost]
        public string CreateEvent([FromBody] CalendarEventCreate calendarEvent)
        {

            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            //var bla = user.Identity.FindFirst(JwtRegisteredClaimNames.UniqueName).Value;
            var invalidMsg = ValidateTodo(calendarEvent);
            if (invalidMsg != null) return invalidMsg;

            var category = _context.Categories.FirstOrDefault(e=>e.Name==calendarEvent.Category);
            if (category == null) return "Invalid Category";

            _context.Todos.AddRange(Todo.Create(calendarEvent, category, ownerId));
            _context.SaveChanges();

            return null;
        }

        [HttpDelete("{id:int}")]
        public string RemoveEvent(int id)
        {
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var todo = _context.Todos.FirstOrDefault(e => e.Id == id && e.UserId==ownerId);
            if (todo != null) _context.Todos.Remove(todo);
            else return "Not allowed to remove the event with this Id";

            _context.SaveChanges();

            return null;
        }

        private string ValidateTodo(CalendarEventCreate todo)
        {
            if (todo == null || todo.Title == null || todo.Start == null)
            {
                return "Must send the right parameters";
            }
            else
            {
                DateTime startDate;
                if (!DateTime.TryParseExact(todo.Start, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    return "Start date has not the right format";
                if (!string.IsNullOrWhiteSpace(todo.End))
                {
                    DateTime endDate;
                    if (!DateTime.TryParseExact(todo.End,"dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                        return "End date has not the right format";

                    if (endDate <= startDate)
                        return "End date must be after Start date";
                }
            }

            return null;
        }
    }
}
