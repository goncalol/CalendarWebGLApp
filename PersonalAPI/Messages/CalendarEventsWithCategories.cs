using PersonalAPI.Data;

namespace PersonalAPI.Messages
{
    public class CalendarEventsWithCategories
    {
        public TodoResult[] CalendarEvents { get; set; }
        public CategoryResult[] Categories { get; set; }

        public static CalendarEventsWithCategories Create(Todo[] events, Category[] categories)
        => new CalendarEventsWithCategories
        {
            Categories = CategoryResult.Create(categories),
            CalendarEvents = TodoResult.Create(events)
        };
    }
}
