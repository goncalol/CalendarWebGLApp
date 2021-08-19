using PersonalAPI.Data;
using PersonalAPI.Extensions;
using System.Linq;

namespace PersonalAPI.Messages
{
    public class TodoResult : CalendarEventCommon
    {
        public int Id { get; set; }
        public string Color { get; set; }

        internal static TodoResult Create(Todo todo)
            => new TodoResult
            {
                Title = todo?.Name,
                Start = todo?.StartDate?.ToString(todo.AllDay ? DateTimeExtensions.DateFormat : DateTimeExtensions.DateTimeFormat),
                End = todo?.EndDate?.ToString(todo.AllDay ? DateTimeExtensions.DateFormat : DateTimeExtensions.DateTimeFormat),
                Id = todo?.Id ?? 0,
                Color = todo?.Category?.Color
            };

        internal static TodoResult[] Create(Todo[] todos)
            => todos?.Select(Create).ToArray() ?? new TodoResult[] { };
    }
}
