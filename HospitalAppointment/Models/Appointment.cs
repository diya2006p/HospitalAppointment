
using System.ComponentModel.DataAnnotations;

namespace HospitalAppointment.Models
{
    public class Appointment
    {
        [Required]
        public string PatientName { get; set; } = "";

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
    }
}
