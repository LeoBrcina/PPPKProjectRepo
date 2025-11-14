using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AttachmentController : Controller
    {
        private readonly PostgresContext _context;
        private readonly IWebHostEnvironment _environment;

        public AttachmentController(PostgresContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index(int? appointmentId)
        {
            var attachments = _context.Attachments
                .Include(a => a.Appointment)
                .Where(a => !appointmentId.HasValue || a.Appointmentid == appointmentId)
                .Select(a => new AttachmentVM
                {
                    AttachmentId = a.Attachmentid,
                    AppointmentId = a.Appointmentid,
                    FileName = a.Filename,
                    FilePath = a.Filepath,
                    UploadedAt = a.Uploadedat
                })
                .ToList();

            ViewBag.Appointments = _context.Appointments
                .Select(a => new SelectListItem
                {
                    Value = a.Appointmentid.ToString(),
                    Text = $"{a.Appointmentdate:yyyy-MM-dd} - {a.Patient.Firstname} {a.Patient.Lastname}"
                })
                .ToList();

            return View(attachments);
        }

        public IActionResult Create()
        {
            ViewBag.Appointments = _context.Appointments
                .Select(a => new SelectListItem
                {
                    Value = a.Appointmentid.ToString(),
                    Text = $"{a.Appointmentdate:yyyy-MM-dd} - {a.Patient.Firstname} {a.Patient.Lastname}"
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int appointmentId, List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                ModelState.AddModelError("files", "Please select at least one file.");
                return RedirectToAction(nameof(Create));
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "attachments");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var attachment = new Attachment
                    {
                        Appointmentid = appointmentId,
                        Filename = file.FileName,
                        Filepath = $"/uploads/attachments/{uniqueFileName}",
                        Uploadedat = DateTime.Now
                    };

                    _context.Attachments.Add(attachment);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();

            string filePath = Path.Combine(_environment.WebRootPath, attachment.Filepath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedAttachments)
        {
            if (selectedAttachments == null || !selectedAttachments.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var attachmentsToDelete = _context.Attachments
                .Where(a => selectedAttachments.Contains(a.Attachmentid))
                .ToList();

            foreach (var attachment in attachmentsToDelete)
            {
                string filePath = Path.Combine(_environment.WebRootPath, attachment.Filepath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Attachments.RemoveRange(attachmentsToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
