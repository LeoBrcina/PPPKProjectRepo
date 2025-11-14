using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly PostgresContext _context;

        public MedicalRecordController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var medicalRecords = _context.Medicalrecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Disease)
                .Select(mr => new MedicalRecordVM
                {
                    RecordId = mr.Recordid,
                    PatientId = mr.Patientid,
                    Patient = new PatientVM
                    {
                        FirstName = mr.Patient.Firstname,
                        LastName = mr.Patient.Lastname
                    },
                    DiseaseNames = new List<string> { mr.Disease.Diseasename },
                    StartDate = mr.Startdate,
                    EndDate = mr.Enddate
                })
                .ToList();

            return View(medicalRecords);
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

            ViewBag.Diseases = _context.Diseases
                .Select(d => new SelectListItem
                {
                    Value = d.Diseaseid.ToString(),
                    Text = d.Diseasename
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalRecordVM medicalRecordVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = _context.Patients.Select(p => new SelectListItem
                {
                    Value = p.Patientid.ToString(),
                    Text = $"{p.Firstname} {p.Lastname}"
                }).ToList();

                ViewBag.Diseases = _context.Diseases.Select(d => new SelectListItem
                {
                    Value = d.Diseaseid.ToString(),
                    Text = d.Diseasename
                }).ToList();

                return View(medicalRecordVM);
            }

            foreach (var diseaseId in medicalRecordVM.DiseaseIds)
            {
                var medicalRecord = new Medicalrecord
                {
                    Patientid = medicalRecordVM.PatientId,
                    Diseaseid = diseaseId,
                    Startdate = medicalRecordVM.StartDate,
                    Enddate = medicalRecordVM.EndDate
                };

                _context.Medicalrecords.Add(medicalRecord);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var medicalRecords = _context.Medicalrecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Disease)
                .Where(mr => mr.Recordid == id)
                .ToList();

            if (!medicalRecords.Any())
                return NotFound();

            var medicalRecordVM = new MedicalRecordVM
            {
                RecordId = id,
                PatientId = medicalRecords.First().Patientid,
                Patient = new PatientVM
                {
                    FirstName = medicalRecords.First().Patient.Firstname,
                    LastName = medicalRecords.First().Patient.Lastname
                },
                DiseaseNames = medicalRecords.Select(mr => mr.Disease.Diseasename).ToList(),
                StartDate = medicalRecords.First().Startdate,
                EndDate = medicalRecords.First().Enddate
            };

            return View(medicalRecordVM);
        }

        public IActionResult Edit(int id)
        {
            var medicalRecords = _context.Medicalrecords.Where(mr => mr.Recordid == id).ToList();

            if (!medicalRecords.Any())
                return NotFound();

            var medicalRecordVM = new MedicalRecordVM
            {
                RecordId = id,
                PatientId = medicalRecords.First().Patientid,
                DiseaseIds = medicalRecords.Select(mr => mr.Diseaseid).ToList(),
                StartDate = medicalRecords.First().Startdate,
                EndDate = medicalRecords.First().Enddate
            };

            ViewBag.Patients = _context.Patients.Select(p => new SelectListItem
            {
                Value = p.Patientid.ToString(),
                Text = $"{p.Firstname} {p.Lastname}"
            }).ToList();

            ViewBag.Diseases = _context.Diseases.Select(d => new SelectListItem
            {
                Value = d.Diseaseid.ToString(),
                Text = d.Diseasename
            }).ToList();

            return View(medicalRecordVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicalRecordVM medicalRecordVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = _context.Patients.Select(p => new SelectListItem
                {
                    Value = p.Patientid.ToString(),
                    Text = $"{p.Firstname} {p.Lastname}"
                }).ToList();

                ViewBag.Diseases = _context.Diseases.Select(d => new SelectListItem
                {
                    Value = d.Diseaseid.ToString(),
                    Text = d.Diseasename
                }).ToList();

                return View(medicalRecordVM);
            }

            var existingMedicalRecords = _context.Medicalrecords.Where(mr => mr.Recordid == id);
            _context.Medicalrecords.RemoveRange(existingMedicalRecords);

            foreach (var diseaseId in medicalRecordVM.DiseaseIds)
            {
                var medicalRecord = new Medicalrecord
                {
                    Recordid = id,
                    Patientid = medicalRecordVM.PatientId,
                    Diseaseid = diseaseId,
                    Startdate = medicalRecordVM.StartDate,
                    Enddate = medicalRecordVM.EndDate
                };
                _context.Medicalrecords.Add(medicalRecord);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var medicalRecords = _context.Medicalrecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Disease)
                .Where(mr => mr.Recordid == id)
                .ToList();

            if (!medicalRecords.Any())
                return NotFound();

            var medicalRecordVM = new MedicalRecordVM
            {
                RecordId = id,
                PatientId = medicalRecords.First().Patientid,
                Patient = new PatientVM
                {
                    FirstName = medicalRecords.First().Patient.Firstname,
                    LastName = medicalRecords.First().Patient.Lastname
                },
                DiseaseNames = medicalRecords.Select(mr => mr.Disease.Diseasename).ToList()
            };

            return View(medicalRecordVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var medicalRecords = _context.Medicalrecords.Where(mr => mr.Recordid == id);
            _context.Medicalrecords.RemoveRange(medicalRecords);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
