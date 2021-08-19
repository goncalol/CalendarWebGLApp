namespace PersonalApp.Models
{
    public class CalendarEventRequest : CalendarEventCommon
    {
        public bool StartAllDay { get; set; }
        public bool EndAllDay { get; set; }
        public int CategoryId { get; set; }
    }
}
