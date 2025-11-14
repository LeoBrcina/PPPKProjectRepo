using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly PostgresContext _context;

        public AppointmentController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Appointmenttype)
                .Select(a => new AppointmentVM
                {
                    AppointmentId = a.Appointmentid,
                    AppointmentDate = a.Appointmentdate,
                    PatientId = a.Patientid,
                    AppointmentTypeName = a.Appointmenttype.Appointmenttypename ?? string.Empty,
                    PatientName = $"{a.Patient.Firstname} {a.Patient.Lastname}"
                })
                .ToList();

            return View(appointments);
        }

        public IActionResult Create()
        {
            ViewBag.AppointmentTypes = _context.Appointmenttypes
                .Select(at => new SelectListItem
                {
                    Value = at.Appointmenttypeid.ToString(),
                    Text = at.Appointmenttypename
                })
                .ToList();

            ViewBag.Patients = _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.Patientid.ToString(),
                    Text = $"{p.Firstname} {p.Lastname}"
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentVM appointmentVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AppointmentTypes = _context.Appointmenttypes
                    .Select(at => new SelectListItem
                    {
                        Value = at.Appointmenttypeid.ToString(),
                        Text = at.Appointmenttypename
                    })
                    .ToList();

                ViewBag.Patients = _context.Patients
                    .Select(p => new SelectListItem
                    {
                        Value = p.Patientid.ToString(),
                        Text = $"{p.Firstname} {p.Lastname}"
                    })
                    .ToList();

                return View(appointmentVM);
            }

            var appointment = new Appointment
            {
                Appointmentdate = appointmentVM.AppointmentDate,
                Patientid = appointmentVM.PatientId,
                Appointmenttypeid = appointmentVM.AppointmentId 
            };

            try
            {
                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public IActionResult Details(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Appointmenttype)
                .Include(a => a.Attachments) 
                .FirstOrDefault(a => a.Appointmentid == id);

            if (appointment == null)
                return NotFound();

            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointment.Appointmentid,
                AppointmentDate = appointment.Appointmentdate,
                PatientId = appointment.Patientid,
                PatientName = $"{appointment.Patient.Firstname} {appointment.Patient.Lastname}",
                AppointmentTypeName = appointment.Appointmenttype.Appointmenttypename ?? string.Empty
            };

            return View(appointmentVM);
        }

        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Appointmentid == id);

            if (appointment == null)
                return NotFound();

            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointment.Appointmentid,
                AppointmentDate = appointment.Appointmentdate,
                PatientId = appointment.Patientid,
                AppointmentTypeName = _context.Appointmenttypes
                    .FirstOrDefault(at => at.Appointmenttypeid == appointment.Appointmenttypeid)?.Appointmenttypename ?? string.Empty
            };

            ViewBag.AppointmentTypes = _context.Appointmenttypes
                .Select(at => new SelectListItem
                {
                    Value = at.Appointmenttypeid.ToString(),
                    Text = at.Appointmenttypename
                })
                .ToList();

            ViewBag.Patients = _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.Patientid.ToString(),
                    Text = $"{p.Firstname} {p.Lastname}"
                })
                .ToList();

            return View(appointmentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AppointmentVM appointmentVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AppointmentTypes = _context.Appointmenttypes
                    .Select(at => new SelectListItem
                    {
                        Value = at.Appointmenttypeid.ToString(),
                        Text = at.Appointmenttypename
                    })
                    .ToList();

                ViewBag.Patients = _context.Patients
                    .Select(p => new SelectListItem
                    {
                        Value = p.Patientid.ToString(),
                        Text = $"{p.Firstname} {p.Lastname}"
                    })
                    .ToList();

                return View(appointmentVM);
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.Appointmentid == id);

            if (appointment == null)
                return NotFound();

            appointment.Appointmentdate = appointmentVM.AppointmentDate;
            appointment.Patientid = appointmentVM.PatientId;
            appointment.Appointmenttypeid = appointmentVM.AppointmentId; 

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Appointmenttype)
                .FirstOrDefault(a => a.Appointmentid == id);

            if (appointment == null)
                return NotFound();

            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointment.Appointmentid,
                AppointmentDate = appointment.Appointmentdate,
                PatientName = $"{appointment.Patient.Firstname} {appointment.Patient.Lastname}",
                AppointmentTypeName = appointment.Appointmenttype.Appointmenttypename ?? string.Empty
            };

            return View(appointmentVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Appointmentid == id);

            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
