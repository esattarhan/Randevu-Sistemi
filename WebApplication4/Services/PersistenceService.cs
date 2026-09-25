using System.Text.Json;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public static class PersistenceService
    {
        private static readonly string DataDir = Path.Combine(AppContext.BaseDirectory, "App_Data");

        static PersistenceService()
        {
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
        }

        public static List<Booking> LoadBookings()
        {
            var f = Path.Combine(DataDir, "bookings.json");
            if (!File.Exists(f)) return new List<Booking>();
            var txt = File.ReadAllText(f);
            return JsonSerializer.Deserialize<List<Booking>>(txt) ?? new List<Booking>();
        }

        public static void SaveBookings(List<Booking> items)
        {
            var f = Path.Combine(DataDir, "bookings.json");
            File.WriteAllText(f, JsonSerializer.Serialize(items));
        }

        public static List<Feedback> LoadFeedbacks()
        {
            var f = Path.Combine(DataDir, "feedbacks.json");
            if (!File.Exists(f)) return new List<Feedback>();
            var txt = File.ReadAllText(f);
            return JsonSerializer.Deserialize<List<Feedback>>(txt) ?? new List<Feedback>();
        }

        public static void SaveFeedbacks(List<Feedback> items)
        {
            var f = Path.Combine(DataDir, "feedbacks.json");
            File.WriteAllText(f, JsonSerializer.Serialize(items));
        }
    }
}
