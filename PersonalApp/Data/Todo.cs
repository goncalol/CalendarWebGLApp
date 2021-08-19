using PersonalApp.Extensions;
using PersonalApp.Models;
using System;

#nullable disable

namespace PersonalApp.Data
{
    public partial class Todo
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
        public bool AllDay { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        internal static Todo[] CreateMultiple(CalendarEventRequest todo)
        {
            var end = todo.End == null ? DateTime.Parse(todo.Start).Date : DateTime.Parse(todo.End);//????fim do dia ou so 1h??????,
            var start = DateTime.Parse(todo.Start);
            var startNext = start.AddDays(1);

            if (end.Date > startNext.Date)//criar 3 eventos -> 2 da ponta not all day e 1 com all day(range)
            {
                var s = Create(start, start.AbsoluteEnd(), todo.Title, todo.StartAllDay,todo.CategoryId);
                var m = Create(startNext.Date, end.Date, todo.Title, true, todo.CategoryId);
                var e = Create(end.Date, end, todo.Title, todo.EndAllDay, todo.CategoryId);

                return new Todo[] { s, m, e };
            }

            if (end.Date > start.Date)
            {
                var s = Create(start, start.AbsoluteEnd(), todo.Title, todo.StartAllDay, todo.CategoryId);
                var e = Create(end.Date, end, todo.Title, todo.EndAllDay, todo.CategoryId);
                return new Todo[] { s, e };
            }

            return new Todo[] { Create(start, end, todo.Title, todo.StartAllDay, todo.CategoryId) };

        }

        private static Todo Create(DateTime start, DateTime end, string name, bool allDay, int categoryId)
        => new Todo
        {
            StartDate = start,
            Name = name,
            EndDate = end,
            AllDay = allDay,
            CategoryId = categoryId
        };

    }
}
