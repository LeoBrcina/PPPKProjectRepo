using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class MedicineController : Controller
    {
        private readonly PostgresContext _context;

        public MedicineController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var medicines = _context.Medicines
                .Select(m => new MedicineVM
                {
                    MedicineId = m.Medicineid,
                    MedicineName = m.Medicinename
                })
                .ToList();
            return View(medicines);
        }

        public IActionResult Details(int id)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.Medicineid == id);
            if (medicine == null)
                return NotFound();

            var medicineVM = new MedicineVM
            {
                MedicineId = medicine.Medicineid,
                MedicineName = medicine.Medicinename
            };

            return View(medicineVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicineVM medicineVM)
        {
            if (!ModelState.IsValid)
            {
                return View(medicineVM);
            }

            var medicine = new Medicine
            {
                Medicinename = medicineVM.MedicineName
            };

            _context.Medicines.Add(medicine);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.Medicineid == id);
            if (medicine == null)
                return NotFound();

            var medicineVM = new MedicineVM
            {
                MedicineId = medicine.Medicineid,
                MedicineName = medicine.Medicinename
            };

            return View(medicineVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicineVM medicineVM)
        {
            if (!ModelState.IsValid)
            {
                return View(medicineVM);
            }

            var medicine = _context.Medicines.FirstOrDefault(m => m.Medicineid == id);
            if (medicine == null)
                return NotFound();

            medicine.Medicinename = medicineVM.MedicineName;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.Medicineid == id);
            if (medicine == null)
                return NotFound();

            var medicineVM = new MedicineVM
            {
                MedicineId = medicine.Medicineid,
                MedicineName = medicine.Medicinename
            };
            return View(medicineVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.Medicineid == id);
            if (medicine == null)
                return NotFound();

            _context.Medicines.Remove(medicine);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
