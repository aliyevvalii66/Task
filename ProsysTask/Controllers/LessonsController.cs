using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProsysTask.Models;

namespace ProsysTask.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class LessonsController : Controller
    {
        private readonly ExamDbContext _context;

        public LessonsController(ExamDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lessons = await _context.Lessons.ToListAsync();
            return View(lessons); 
        }

        public IActionResult Create()
        {
            return PartialView("_LessonFormPartial", new Lesson());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lesson lesson)
        {
            if (!ModelState.IsValid)
                return PartialView("_LessonFormPartial", lesson);

            if (await _context.Lessons.AnyAsync(l => l.LessonCode == lesson.LessonCode))
            {
                ModelState.AddModelError("LessonCode", "Bu kod artıq mövcuddur.");
                return PartialView("_LessonFormPartial", lesson);
            }

            try
            {
                _context.Add(lesson);
                await _context.SaveChangesAsync();

                var lessons = await _context.Lessons.ToListAsync();
                return PartialView("_LessonsListPartial", lessons); 
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Verilənləri saxlamaq mümkün olmadı: " + ex.InnerException?.Message);
                return PartialView("_LessonFormPartial", lesson);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Xəta baş verdi: " + ex.Message);
                return PartialView("_LessonFormPartial", lesson);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();
            return PartialView("_LessonFormPartial", lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Lesson lesson)
        {
            //unique key change is not allowed
            if (id != lesson.LessonCode) return NotFound();

            if (!ModelState.IsValid)
                return PartialView("_LessonFormPartial", lesson);

            try
            {
                _context.Update(lesson);
                await _context.SaveChangesAsync();

                var lessons = await _context.Lessons.ToListAsync();
                return PartialView("_LessonsListPartial", lessons);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Verilənləri saxlamaq mümkün olmadı: " + ex.InnerException?.Message);
                return PartialView("_LessonFormPartial", lesson);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Xəta baş verdi: " + ex.Message);
                return PartialView("_LessonFormPartial", lesson);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            var examsExist = await _context.Exams.AnyAsync(e => e.LessonCode == id);
            if (examsExist)
            {
                TempData["ErrorMessage"] = "Bu dərsi silmək mümkün deyil, çünki ona bağlı imtahanlar mövcuddur.";
                var lessons = await _context.Lessons.ToListAsync();
                return PartialView("_LessonsListPartial", lessons);
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            var lessonsList = await _context.Lessons.ToListAsync();
            return PartialView("_LessonsListPartial", lessonsList);
        }

    }

}
