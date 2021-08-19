using PersonalApp.Data;
using System.Linq;
using PersonalApp.Extensions;

namespace PersonalApp.Models
{
    public class CalendarEventViewModel : CalendarEventCommon
    {
        public int Id { get; set; }
        public string Color { get; set; }

        internal static CalendarEventViewModel Create(Todo todo)
            => new CalendarEventViewModel
            {
                Title = todo?.Name,
                Start = todo?.StartDate?.ToString(todo.AllDay ? DateTimeExtensions.DateFormat : DateTimeExtensions.DateTimeFormat),
                End = todo?.EndDate?.ToString(todo.AllDay ? DateTimeExtensions.DateFormat : DateTimeExtensions.DateTimeFormat),
                Id = todo?.Id ?? 0,
                Color = todo?.Category?.Color
            };

        internal static CalendarEventViewModel[] Create(Todo[] todos)
            => todos?.Select(Create).ToArray() ?? new CalendarEventViewModel[] { };
    }
}
