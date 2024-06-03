using CalendarInviteICS.Dtos;
using CalendarInviteICS.Helpers;
using CalendarInviteICS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace CalendarInviteICS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarInviteController : ControllerBase
    {
        private readonly IEmailManager _emailSender;
        public CalendarInviteController(IEmailManager emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("Create")] 
        public async Task<IActionResult> CreateInvite(InviteDto inviteDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ResultDto { status = false, message = "The Parameters sent are not correct.", data = new { } });
            }
            var subject = "You are invited to join calendar invite";
            var icsCalander = CommonFunctions.CreateICSCalendar(inviteDto.FromTime, inviteDto.ToTime,
                subject,
                inviteDto.ToEmail, "India", "LevelUpLMS", "dev3@evolvous.com");
            var _emailMsg = $"<strong> Dear User,</strong><br/>I hope this email finds you well.<br/>" +
                $"I am excited to invite you to {inviteDto.MeetingType.ToString()}, which will be held on {inviteDto.InviteDate} at {inviteDto.FromTime}. The event will take place at {inviteDto.Location} and will feature {inviteDto.Agenda}<br/>" +
                $"Your presence would be greatly appreciated, and we believe you will find the {inviteDto.MeetingType.ToString()} both enjoyable and informative. We look forward to the opportunity to celebrate with you.<br/>" +
                $"<strong>Details of the event:</strong><br/><ul><li><b>Event:</b> {inviteDto.Title}</li><li><b>Date:</b> {inviteDto.InviteDate}</li><li><b>Time:</b> {inviteDto.FromTime}</li></ul><br/>" +
                $"Please RSVP by {inviteDto.ToTime} to confirm your attendance.We look forward to seeing you at the event.<br/> Best regards,<br/> Team LevelUp";
            var attachments = CommonFunctions.GetICSAttachment(icsCalander, CommonFunctions.Generate13UniqueDigits());
            await _emailSender.SendEmailAsync(email: inviteDto.ToEmail, subject: subject, 
                htmlMessage: _emailMsg, attachment: attachments);
            return Ok(new ResultDto { status = true, message = "Appointment Created Successfully.", data = new { } });
        }
    }
}
