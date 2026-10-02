using System.Collections.Generic;

namespace WebApplication4.Models
{
    public class FeedbackViewModel
    {
        public List<Booking> CompletedBookings { get; set; } = new List<Booking>();
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public Feedback NewFeedback { get; set; } = new Feedback();
    }
}
