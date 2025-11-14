using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly PostgresContext _context;

        public PatientController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm)
        {
            var patients = _context.Patients
                .Where(p => string.IsNullOrEmpty(searchTerm) ||
                            p.Firstname.Contains(searchTerm) ||
                            p.Lastname.Contains(searchTerm) ||
                            p.Oib.Contains(searchTerm))
                .Include(p => p.Appointments)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Medicine)
                .Include(p => p.Medicalrecords)
                    .ThenInclude(mr => mr.Disease)
                .Select(p => new PatientVM
                {
                    PatientId = p.Patientid,
                    FirstName = p.Firstname,
                    LastName = p.Lastname,
                    Oib = p.Oib,
                    DateOfBirth = DateOnly.FromDateTime(p.Dateofbirth),
                    Gender = p.Gender.ToString(),
                    Appointments = p.Appointments.Select(a => new AppointmentVM
                    {
                        AppointmentId = a.Appointmentid,
                        AppointmentDate = a.Appointmentdate
                    }).ToList(),
                    Prescriptions = p.Prescriptions.Select(pr => new PrescriptionVM
                    {
                        PrescriptionId = pr.Prescriptionid,
                        MedicineNames = new List<string> { pr.Medicine.Medicinename }
                    }).ToList(),
                    MedicalRecords = p.Medicalrecords.Select(mr => new MedicalRecordVM
                    {
                        RecordId = mr.Recordid,
                        DiseaseNames = new List<string> { mr.Disease.Diseasename },
                        StartDate = mr.Startdate,
                        EndDate = mr.Enddate
                    }).ToList()
                })
                .ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(patients);
        }

        public IActionResult Details(int id)
        {
            var patient = _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Appointmenttype)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Medicine)
                .Include(p => p.Medicalrecords)
                    .ThenInclude(mr => mr.Disease)
                .FirstOrDefault(p => p.Patientid == id);

            if (patient == null)
                return NotFound();

            var patientVM = new PatientVM
            {
                PatientId = patient.Patientid,
                FirstName = patient.Firstname,
                LastName = patient.Lastname,
                Oib = patient.Oib,
                DateOfBirth = DateOnly.FromDateTime(patient.Dateofbirth),
                Gender = patient.Gender.ToString(),
                Appointments = patient.Appointments.Select(a => new AppointmentVM
                {
                    AppointmentId = a.Appointmentid,
                    AppointmentDate = a.Appointmentdate,
                    AppointmentTypeName = a.Appointmenttype.Appointmenttypename
                }).ToList(),
                Prescriptions = patient.Prescriptions.Select(pr => new PrescriptionVM
                {
                    PrescriptionId = pr.Prescriptionid,
                    MedicineNames = new List<string> { pr.Medicine.Medicinename }
                }).ToList(),
                MedicalRecords = patient.Medicalrecords.Select(mr => new MedicalRecordVM
                {
                    RecordId = mr.Recordid,
                    DiseaseNames = new List<string> { mr.Disease.Diseasename },
                    StartDate = mr.Startdate,
                    EndDate = mr.Enddate
                }).ToList()
            };

            return View(patientVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PatientVM patientVM)
        {
            if (!ModelState.IsValid)
                return View(patientVM);

            if (_context.Patients.Any(p => p.Oib == patientVM.Oib))
            {
                ModelState.AddModelError("Oib", "A patient with the same OIB already exists.");
                return View(patientVM);
            }

            var patient = new Patient
            {
                Firstname = patientVM.FirstName,
                Lastname = patientVM.LastName,
                Oib = patientVM.Oib,
                Dateofbirth = patientVM.DateOfBirth.ToDateTime(TimeOnly.MinValue).ToUniversalTime(),
                Gender = char.Parse(patientVM.Gender)
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Patientid == id);
            if (patient == null)
                return NotFound();

            var patientVM = new PatientVM
            {
                PatientId = patient.Patientid,
                FirstName = patient.Firstname,
                LastName = patient.Lastname,
                Oib = patient.Oib,
                DateOfBirth = DateOnly.FromDateTime(patient.Dateofbirth),
                Gender = patient.Gender.ToString()
            };

            return View(patientVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PatientVM patientVM)
        {
            if (!ModelState.IsValid)
                return View(patientVM);

            var patient = _context.Patients.FirstOrDefault(p => p.Patientid == id);
            if (patient == null)
                return NotFound();

            patient.Firstname = patientVM.FirstName;
            patient.Lastname = patientVM.LastName;
            patient.Oib = patientVM.Oib;
            patient.Dateofbirth = patientVM.DateOfBirth.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
            patient.Gender = char.Parse(patientVM.Gender);

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Patientid == id);
            if (patient == null)
                return NotFound();

            var patientVM = new PatientVM
            {
                PatientId = patient.Patientid,
                FirstName = patient.Firstname,
                LastName = patient.Lastname,
                Oib = patient.Oib,
                DateOfBirth = DateOnly.FromDateTime(patient.Dateofbirth),
                Gender = patient.Gender.ToString()
            };

            return View(patientVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Patientid == id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ExportToCsv()
        {
            var patients = _context.Patients
                .Select(p => new
                {
                    p.Patientid,
                    p.Firstname,
                    p.Lastname,
                    p.Oib,
                    DateOfBirth = p.Dateofbirth.ToString("yyyy-MM-dd"),
                    p.Gender
                })
                .ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("PatientId,FirstName,LastName,OIB,DateOfBirth,Gender");

            foreach (var patient in patients)
            {
                csvBuilder.AppendLine($"{patient.Patientid},{patient.Firstname},{patient.Lastname},{patient.Oib},{patient.DateOfBirth},{patient.Gender}");
            }

            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(bytes, "text/csv", "patients.csv");
        }
    }
}
