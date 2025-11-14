using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly PostgresContext _context;

        public PrescriptionController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Medicine)
                .Select(p => new PrescriptionVM
                {
                    PrescriptionId = p.Prescriptionid,
                    PatientId = p.Patientid,
                    PatientName = p.Patient.Firstname + " " + p.Patient.Lastname, 
                    MedicineNames = new List<string> { p.Medicine.Medicinename }
                })
                .ToList();

            return View(prescriptions);
        }

        public IActionResult Create()
        {
            ViewBag.Patients = _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.Patientid.ToString(),
                    Text = $"{p.Firstname} {p.Lastname}"
                })
                .ToList();

            ViewBag.Medicines = _context.Medicines
                .Select(m => new SelectListItem
                {
                    Value = m.Medicineid.ToString(),
                    Text = m.Medicinename
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PrescriptionVM prescriptionVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = _context.Patients
                    .Select(p => new SelectListItem
                    {
                        Value = p.Patientid.ToString(),
                        Text = $"{p.Firstname} {p.Lastname}"
                    })
                    .ToList();

                ViewBag.Medicines = _context.Medicines
                    .Select(m => new SelectListItem
                    {
                        Value = m.Medicineid.ToString(),
                        Text = m.Medicinename
                    })
                    .ToList();

                return View(prescriptionVM);
            }

            foreach (var medicineId in prescriptionVM.MedicineIds)
            {
                var prescription = new Prescription
                {
                    Patientid = prescriptionVM.PatientId,
                    Medicineid = medicineId
                };

                _context.Prescriptions.Add(prescription);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Medicine)
                .Where(p => p.Prescriptionid == id)
                .ToList();

            if (prescriptions.Count == 0)
                return NotFound();

            var prescriptionVM = new PrescriptionVM
            {
                PrescriptionId = id,
                PatientId = prescriptions.First().Patientid,
                MedicineNames = prescriptions.Select(p => p.Medicine.Medicinename).ToList()
            };

            return View(prescriptionVM);
        }

        public IActionResult Edit(int id)
        {
            var prescriptions = _context.Prescriptions
                .Where(p => p.Prescriptionid == id)
                .ToList();

            if (prescriptions.Count == 0)
                return NotFound();

            var prescriptionVM = new PrescriptionVM
            {
                PrescriptionId = id,
                PatientId = prescriptions.First().Patientid,
                MedicineIds = prescriptions.Select(p => p.Medicineid).ToList()
            };

            ViewBag.Patients = _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.Patientid.ToString(),
                    Text = $"{p.Firstname} {p.Lastname}"
                })
                .ToList();

            ViewBag.Medicines = _context.Medicines
                .Select(m => new SelectListItem
                {
                    Value = m.Medicineid.ToString(),
                    Text = m.Medicinename
                })
                .ToList();

            return View(prescriptionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PrescriptionVM prescriptionVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = _context.Patients
                    .Select(p => new SelectListItem
                    {
                        Value = p.Patientid.ToString(),
                        Text = $"{p.Firstname} {p.Lastname}"
                    })
                    .ToList();

                ViewBag.Medicines = _context.Medicines
                    .Select(m => new SelectListItem
                    {
                        Value = m.Medicineid.ToString(),
                        Text = m.Medicinename
                    })
                    .ToList();

                return View(prescriptionVM);
            }

            var existingPrescriptions = _context.Prescriptions.Where(p => p.Prescriptionid == id);
            _context.Prescriptions.RemoveRange(existingPrescriptions);

            foreach (var medicineId in prescriptionVM.MedicineIds)
            {
                var prescription = new Prescription
                {
                    Prescriptionid = id,
                    Patientid = prescriptionVM.PatientId,
                    Medicineid = medicineId
                };
                _context.Prescriptions.Add(prescription);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Medicine)
                .Where(p => p.Prescriptionid == id)
                .ToList();

            if (prescriptions.Count == 0)
                return NotFound();

            var prescriptionVM = new PrescriptionVM
            {
                PrescriptionId = id,
                PatientId = prescriptions.First().Patientid,
                MedicineNames = prescriptions.Select(p => p.Medicine.Medicinename).ToList()
            };

            return View(prescriptionVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var prescriptions = _context.Prescriptions.Where(p => p.Prescriptionid == id);
            _context.Prescriptions.RemoveRange(prescriptions);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
