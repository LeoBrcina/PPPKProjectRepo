using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly PostgresContext _context;

        public DiseaseController(PostgresContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var diseases = _context.Diseases
                .Select(d => new DiseaseVM
                {
                    DiseaseId = d.Diseaseid,
                    DiseaseName = d.Diseasename
                })
                .ToList();
            return View(diseases);
        }

        public IActionResult Details(int id)
        {
            var disease = _context.Diseases.FirstOrDefault(d => d.Diseaseid == id);
            if (disease == null)
                return NotFound();

            var diseaseVM = new DiseaseVM
            {
                DiseaseId = disease.Diseaseid,
                DiseaseName = disease.Diseasename
            };

            return View(diseaseVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DiseaseVM diseaseVM)
        {
            if (!ModelState.IsValid)
            {
                return View(diseaseVM);
            }

            var disease = new Disease
            {
                Diseasename = diseaseVM.DiseaseName
            };

            _context.Diseases.Add(disease);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var disease = _context.Diseases.FirstOrDefault(d => d.Diseaseid == id);
            if (disease == null)
                return NotFound();

            var diseaseVM = new DiseaseVM
            {
                DiseaseId = disease.Diseaseid,
                DiseaseName = disease.Diseasename
            };

            return View(diseaseVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DiseaseVM diseaseVM)
        {
            if (!ModelState.IsValid)
            {
                return View(diseaseVM);
            }

            var disease = _context.Diseases.FirstOrDefault(d => d.Diseaseid == id);
            if (disease == null)
                return NotFound();

            disease.Diseasename = diseaseVM.DiseaseName;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var disease = _context.Diseases.FirstOrDefault(d => d.Diseaseid == id);
            if (disease == null)
                return NotFound();

            var diseaseVM = new DiseaseVM
            {
                DiseaseId = disease.Diseaseid,
                DiseaseName = disease.Diseasename
            };
            return View(diseaseVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var disease = _context.Diseases.FirstOrDefault(d => d.Diseaseid == id);
            if (disease == null)
                return NotFound();

            _context.Diseases.Remove(disease);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
