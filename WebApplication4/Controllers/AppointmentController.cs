using Microsoft.AspNetCore.Mvc;
using WebApplication4.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    public class AppointmentController : Controller
    {
        // Basit in-memory kuaför/berber (stylist) listesi ve randevu deposu
        private static List<Stylist> _barbers = new List<Stylist>();

        static AppointmentController()
        {
            var slots = GenerateTimeSlots();
            _barbers = new List<Stylist>
            {
                new Stylist { Id = 1, Name = "Enes Bey", DisplayName = "Enes Bey", ImageUrl = "https://via.placeholder.com/400x200?text=Enes", AvailableTimes = new List<string>(slots) },
                new Stylist { Id = 2, Name = "Melih Bey", DisplayName = "Melih Bey", ImageUrl = "https://via.placeholder.com/400x200?text=Melih", AvailableTimes = new List<string>(slots) }
            };
        }

        private static List<string> GenerateTimeSlots()
        {
            var list = new List<string>();
            var start = DateTime.Today.AddHours(9);
            var end = DateTime.Today.AddHours(21);
            for (var t = start; t < end; t = t.AddMinutes(30))
            {
                list.Add(t.ToString("HH:mm"));
            }
            return list;
        }

        private static List<Booking> _appointments = PersistenceService.LoadBookings();

        private static int NextAppointmentId => _appointments.Any() ? _appointments.Max(a => a.Id) + 1 : 1;

        // Ana sayfa: berber listesini kartlarla göster
        public IActionResult Index()
        {
            return View(_barbers);
        }

        // Seçilen berberin müsait saatlerini ve randevu formunu gösterir
        public IActionResult Times(int barberId)
        {
            var barber = _barbers.FirstOrDefault(b => b.Id == barberId);
            if (barber == null) return NotFound();
            // Dolu saatleri sadece yöneticinin onayladýðý (Approved) veya zaten gelmiþ (Arrived) olarak iþaretle
            var taken = _appointments.Where(a => a.StylistId == barberId && (a.Status == BookingStatus.Approved || a.Status == BookingStatus.Arrived)).Select(a => a.TimeSlot).ToHashSet();
            // Gönderirken tüm slotlarý gönder, alýnanlarý ViewData içinde belirle
            ViewData["Barber"] = barber;
            ViewData["Taken"] = taken;
            return View(barber.AvailableTimes);
        }

        // Randevu oluþtur (POST)
        [HttpPost]
        public IActionResult CreateAppointment(Booking appointment)
        {
            // Basit kontrol: ayný telefon ile daha önce no-show olan varsa izin verme
            if (!string.IsNullOrEmpty(appointment.Phone))
            {
                // Eðer ayný telefondan yönetici tarafýndan onaylanmamýþ (Pending) veya onaylý bir randevu varsa yeni randevu verilmeyecek
                var exists = _appointments.Any(a => a.Phone == appointment.Phone && a.Status != BookingStatus.NoShow);
                if (exists)
                {
                    TempData["Error"] = "Zaten bir randevunuz bulunuyor veya bekleyen randevunuz var.";
                    return RedirectToAction("Times", new { barberId = appointment.StylistId });
                }
            }

            appointment.Id = NextAppointmentId;
            appointment.Status = BookingStatus.Pending; // yöneticinin onayýna býrak
            appointment.CreatedAt = DateTime.Now;
            // Berber adý kopyala
            var barber = _barbers.FirstOrDefault(b => b.Id == appointment.StylistId);
            appointment.StylistName = barber?.DisplayName ?? "";
            _appointments.Add(appointment);
            PersistenceService.SaveBookings(_appointments);
            // Bildirim göstermek için TempData kullanýyoruz ve telefon bilgisini panelin otomatik güncellenmesi için saklýyoruz
            TempData["Success"] = $"Randevu oluþturuldu ve yöneticinin onayýný bekliyor: {appointment.StylistName} - {appointment.TimeSlot}";
            TempData["PhoneLookup"] = appointment.Phone;
            return RedirectToAction("Times", new { barberId = appointment.StylistId });
        }

        private static List<Feedback> _feedbacks = new List<Feedback>();

        [HttpPost]
        public IActionResult Feedback([FromBody] Feedback feedback)
        {
            if (feedback == null) return Json(new { success = false });
            feedback.Id = _feedbacks.Any() ? _feedbacks.Max(f => f.Id) + 1 : 1;
            _feedbacks.Add(feedback);
            PersistenceService.SaveFeedbacks(_feedbacks);
            return Json(new { success = true });
        }

        // Basit API: verilen telefon numarasýyla eþleþen randevularý döndürür
        [HttpGet]
        public IActionResult MyAppointments(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return Json(new { success = false, message = "Telefon gerekli" });
            var items = _appointments.Where(a => a.Phone == phone)
                .Select(a => new { a.Id, a.StylistName, a.TimeSlot, Status = a.Status.ToString(), a.Request, CreatedAt = a.CreatedAt.ToString("g") })
                .OrderByDescending(a => a.Id)
                .ToList();
            return Json(new { success = true, items });
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var ap = _appointments.FirstOrDefault(a => a.Id == id);
            if (ap == null) return Json(new { success = false });
            ap.Status = BookingStatus.Canceled;
            PersistenceService.SaveBookings(_appointments);
            // Notify
            NotificationService.NotifyBySms(ap.Phone, $"Randevunuz iptal edildi: {ap.StylistName} {ap.TimeSlot}");
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult EditBooking([FromBody] Booking booking)
        {
            var ap = _appointments.FirstOrDefault(a => a.Id == booking.Id);
            if (ap == null) return Json(new { success = false });
            ap.Name = booking.Name;
            ap.Surname = booking.Surname;
            ap.Phone = booking.Phone;
            ap.Age = booking.Age;
            ap.Request = booking.Request;
            PersistenceService.SaveBookings(_appointments);
            return Json(new { success = true });
        }

        // Yönetici görünümü: gelen randevular
        public IActionResult Admin()
        {
            // Bu demo'da role kontrolü yok; ileride ekleyeceðiz
            return View(_appointments.OrderByDescending(a => a.Id));
        }

        // Kullanýcýlarýn kendi randevularýný görüp yönetebileceði sayfa
        public IActionResult Bookings()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var ap = _appointments.FirstOrDefault(a => a.Id == id);
            if (ap != null)
            {
                ap.Status = BookingStatus.Approved;
                PersistenceService.SaveBookings(_appointments);
                // Notify user
                NotificationService.NotifyBySms(ap.Phone, $"Randevunuz onaylandý: {ap.StylistName} {ap.TimeSlot}");
                NotificationService.NotifyByEmail(ap.Phone + "@example.com", "Randevu Onayý", $"Randevunuz {ap.StylistName} için {ap.TimeSlot} tarihinde onaylandý.");
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public IActionResult MarkArrived(int id)
        {
            var ap = _appointments.FirstOrDefault(a => a.Id == id);
            if (ap != null)
            {
                ap.Status = BookingStatus.Arrived;
                PersistenceService.SaveBookings(_appointments);
                NotificationService.NotifyBySms(ap.Phone, $"Randevunuza gelindi olarak iþaretlendi: {ap.StylistName} {ap.TimeSlot}");
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public IActionResult MarkNoShow(int id)
        {
            var ap = _appointments.FirstOrDefault(a => a.Id == id);
            if (ap != null)
            {
                ap.Status = BookingStatus.NoShow;
                PersistenceService.SaveBookings(_appointments);
                NotificationService.NotifyBySms(ap.Phone, $"Randevunuza gelinmedi olarak iþaretlendi: {ap.StylistName} {ap.TimeSlot}");
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public IActionResult NotifyNoShow(int id)
        {
            var ap = _appointments.FirstOrDefault(a => a.Id == id);
            if (ap != null)
            {
                ap.Notified = true;
                PersistenceService.SaveBookings(_appointments);
                NotificationService.NotifyBySms(ap.Phone, $"Yöneticiye bildirim: {ap.StylistName} randevu durumu güncellendi.");
            }
            return RedirectToAction("Admin");
        }
    }
}