using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalApp.Data;
using PersonalApp.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace PersonalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PersonalAppContext _context;

        public HomeController(PersonalAppContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string selectedDay)
        {
            ViewBag.SelectedDay = selectedDay;
            var events = _context.Todos.Include(e => e.Category).ToArray();
            var categories = _context.Categories.ToArray();
            return View(CalendarEventsWithCategories.Create(events,categories));//View("Index2");//
        }

         public string CreateTodo(CalendarEventRequest todo)
        {
            var invalidMsg = ValidateTodo(todo);
            if (invalidMsg != null) return invalidMsg;

            _context.Todos.AddRange(Todo.CreateMultiple(todo));
            _context.SaveChanges();

            return null;
        }

        public string RemoveTodo(int eventId)
        {
            //!!!!!! NÃO ESQUECER DA VALIDAÇÃO DE PERMISSAO QUANDO HOUVER USERS!!!!!!!

            var todo = _context.Todos.FirstOrDefault(e=>e.Id== eventId);
            if (todo != null) _context.Todos.Remove(todo);
            else return "Not allowed to remove the event with this Id";

            _context.SaveChanges();

            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string ValidateTodo(CalendarEventRequest todo)
        {
            if (todo == null || todo.Title == null || todo.Start == null)
            {
                return "Must send the right parameters";
            }
            else
            {
                DateTime startDate;
                if (!DateTime.TryParse(todo.Start, out startDate))
                    return "Start date has not the right format";
                if (todo.End != null)
                {
                    DateTime endDate;
                    if (!DateTime.TryParse(todo.End, out endDate))
                        return "End date has not the right format";

                    if (endDate <= startDate)
                        return "End date must be after Start date";
                }
            }

            return null;
        }
    }
}
