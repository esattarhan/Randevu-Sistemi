namespace WebApplication4.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int StylistId { get; set; }
        public string StylistName { get; set; }
        public string TimeSlot { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public string Request { get; set; }
        public BookingStatus Status { get; set; }
        public bool Notified { get; set; }
    }
}
