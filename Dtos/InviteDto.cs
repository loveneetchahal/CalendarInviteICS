using System.ComponentModel.DataAnnotations;
using static CalendarInviteICS.Helpers.GlobalEnumFunctions;

namespace CalendarInviteICS.Dtos
{
    public class InviteDto
    {
        [Required]
        public string Title { get; set; } 
        [Required]
        public string Agenda { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string ToEmail { get; set; }
        [Required]
        public DateTime FromTime { get; set; }

        [Required]
        public DateTime ToTime { get; set; }
        [Required]
        public DateTime InviteDate { get; set; }
        [Required]
        public MeetingType MeetingType { get; set; } 
    }
}
