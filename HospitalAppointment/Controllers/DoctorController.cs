
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using HospitalAppointment.Models;
using HospitalAppointment.Extensions;

namespace HospitalAppointment.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IMemoryCache _cache;

        public DoctorsController(IMemoryCache cache)
        {
            _cache = cache;
        }

        private List<Doctor> GetDoctors()
        {
            return new List<Doctor>
            {
                new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. varsha Patel",
                    Specialization = "MD",
                    Experience = 10,
                    ConsultationFee = 500
                },
                new Doctor
                {
                    DoctorId = 2,
                    DoctorName = "Dr. Aanchal Panth",
                    Specialization = "Dermatologist",
                    Experience = 7,
                    ConsultationFee = 400
                },
                new Doctor
                {
                    DoctorId = 3,
                    DoctorName = "Dr. Mehul Kashaypwala",
                    Specialization = "Neurologist",
                    Experience = 12,
                    ConsultationFee = 700
                }
            };
        }

        // Doctor list with memory caching
        public IActionResult Index(string? specialization)
        {
            if (specialization != null)
            {
                HttpContext.Session.SetString(
                    "Specialization", specialization);
            }

            var doctors = _cache.GetOrCreate(
                "DoctorList",
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(3);

                    return GetDoctors();
                })!;

            string? selected = HttpContext.Session
                .GetString("Specialization");

            if (!string.IsNullOrEmpty(selected))
            {
                doctors = doctors
                    .Where(d => d.Specialization == selected)
                    .ToList();
            }

            ViewBag.PatientName =
                Request.Cookies["PatientName"];

            ViewBag.Specialization = selected;

            return View(doctors);
        }

        // Doctor details with response caching
        [ResponseCache(
            Duration = 60,
            Location = ResponseCacheLocation.Any,
            NoStore = false,
            VaryByQueryKeys = new[] { "doctorId" })]
        public IActionResult Details(int doctorId)
        {
            var doctors = _cache.GetOrCreate(
                "DoctorList",
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(3);

                    return GetDoctors();
                })!;

            var doctor = doctors.FirstOrDefault(
                d => d.DoctorId == doctorId);

            if (doctor == null)
                return NotFound();

            ViewBag.TotalCharge =
                doctor.CalculateTotalCharge();

            return View(doctor);
        }

        // Display appointment form
        [HttpGet]
        public IActionResult Appointment(int doctorId)
        {
            var doctors = GetDoctors();

            var doctor = doctors.FirstOrDefault(
                d => d.DoctorId == doctorId);

            if (doctor == null)
                return NotFound();

            ViewBag.Doctor = doctor;

            return View(new Appointment
            {
                DoctorId = doctorId,
                AppointmentDate = DateTime.Today.AddDays(1)
            });
        }

        // Submit appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Appointment(Appointment appointment)
        {
            var doctor = GetDoctors().FirstOrDefault(
                d => d.DoctorId == appointment.DoctorId);

            if (doctor == null)
                return NotFound();

            ViewBag.Doctor = doctor;

            if (appointment.AppointmentDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(appointment.AppointmentDate),
                    "Select today or a future date.");
            }

            if (!ModelState.IsValid)
                return View(appointment);

            Response.Cookies.Append(
                "PatientName",
                appointment.PatientName,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                });

            TempData["Message"] =
                $"Appointment request submitted for {doctor.DoctorName}.";

            return RedirectToAction("Index");
        }
    }
}
