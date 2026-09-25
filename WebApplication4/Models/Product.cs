namespace WebApplication4.Models
{
    public class Product
    {
        // Formdaki ProductName kutusu buraya dolacak
        public string ProductName { get; set; }

        // Formdaki Quantity kutusu buraya dolacak
        public int Quantity { get; set; }
        
        // Basit id alanı ürünü düzenlemek/silmek için
        public int Id { get; set; }

        // Ürün stili / kategorisi
        public string Category { get; set; }
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Canceled
    }

    public class Barber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}