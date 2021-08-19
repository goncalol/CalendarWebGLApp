using PersonalApp.Data;

namespace PersonalApp.Models
{
    public class CalendarEventsWithCategories
    {
        public CalendarEventViewModel[] CalendarEvents { get; set; }
        public CategoryViewModel[] Categories { get; set; }

        public static CalendarEventsWithCategories Create(Todo[] events, Category[] categories)
        => new CalendarEventsWithCategories
        {
            Categories = CategoryViewModel.Create(categories),
            CalendarEvents = CalendarEventViewModel.Create(events)
        };
    }
}
