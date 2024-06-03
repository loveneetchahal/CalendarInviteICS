namespace CalendarInviteICS.Helpers
{
    public static class GlobalEnumFunctions
    {
        public static readonly string DefaultDateFormat = "MM/dd/yyyy";
        public static readonly char stringSplitterPipe = '|';
        public static readonly string tempPassword = "Admin@123";
        public enum MeetingType
        {
            Event,
            Meeting,
            Occasion
        }
    }
}
