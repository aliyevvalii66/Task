using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProsysTask.Models;

namespace ProsysTask.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ExamDbContext _context;
        public StudentsController(ExamDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.ToListAsync();
            return View(students);
        }

        public IActionResult Create() => PartialView("_StudentFormPartial", new Student());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
                return PartialView("_StudentFormPartial", student);

            if (await _context.Students.AnyAsync(s => s.StudentNumber == student.StudentNumber))
            {
                ModelState.AddModelError("StudentNumber", "Bu nömrə artıq mövcuddur.");
                return PartialView("_StudentFormPartial", student);
            }

            try
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                var students = await _context.Students.ToListAsync();
                return PartialView("_StudentsListPartial", students);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                return PartialView("_StudentFormPartial", student);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            return PartialView("_StudentFormPartial", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentNumber) return NotFound();
            if (!ModelState.IsValid) return PartialView("_StudentFormPartial", student);

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                var students = await _context.Students.ToListAsync();
                return PartialView("_StudentsListPartial", students);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                return PartialView("_StudentFormPartial", student);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student != null)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();
                }
                var students = await _context.Students.ToListAsync();
                return PartialView("_StudentsListPartial", students);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Silmək mümkün olmadı: " + ex.Message;
                var students = await _context.Students.ToListAsync();
                return PartialView("_StudentsListPartial", students);
            }
        }
    }

}
