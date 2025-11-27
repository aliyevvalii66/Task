using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProsysTask.Models;

namespace ProsysTask.Controllers
{
    public class ExamsController : Controller
    {
        private readonly ExamDbContext _context;
        public ExamsController(ExamDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                //.Include(e => e.Student)
                //.Include(e => e.Lesson)
                .ToListAsync();
            return View(exams);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Lessons = await _context.Lessons.ToListAsync();
            return PartialView("_ExamFormPartial", new Exam());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Lessons = await _context.Lessons.ToListAsync();
                return PartialView("_ExamFormPartial", exam);
            }

            if (await _context.Exams.AnyAsync(e => e.LessonCode == exam.LessonCode && e.StudentNumber == exam.StudentNumber))
            {
                ModelState.AddModelError("", "Bu şagird üçün bu dərsdə imtahan artıq mövcuddur.");
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Lessons = await _context.Lessons.ToListAsync();
                return PartialView("_ExamFormPartial", exam);
            }

            try
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                //var exams = await _context.Exams.Include(e => e.Student).Include(e => e.Lesson).ToListAsync();
                return PartialView("_ExamsListPartial"/*exams*/);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Lessons = await _context.Lessons.ToListAsync();
                return PartialView("_ExamFormPartial", exam);
            }
        }

        public async Task<IActionResult> Edit(string lessonCode, int studentNumber)
        {
            var exam = await _context.Exams.FindAsync(lessonCode, studentNumber);
            if (exam == null) return NotFound();
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Lessons = await _context.Lessons.ToListAsync();
            return PartialView("_ExamFormPartial", exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string lessonCode, int studentNumber, Exam exam)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Lessons = await _context.Lessons.ToListAsync();
                return PartialView("_ExamFormPartial", exam);
            }

            try
            {
                _context.Update(exam);
                await _context.SaveChangesAsync();
                //var exams = await _context.Exams.Include(e => e.Student).Include(e => e.Lesson).ToListAsync();
                return PartialView("_ExamsListPartial"/*, exams*/);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Lessons = await _context.Lessons.ToListAsync();
                return PartialView("_ExamFormPartial", exam);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string lessonCode, int studentNumber)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(lessonCode, studentNumber);
                if (exam != null)
                {
                    _context.Exams.Remove(exam);
                    await _context.SaveChangesAsync();
                }
                //var exams = await _context.Exams.Include(e => e.Student).Include(e => e.Lesson).ToListAsync();
                return PartialView("_ExamsListPartial"/*, exams*/);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Silmək mümkün olmadı: " + ex.Message;
                //var exams = await _context.Exams.Include(e => e.Student).Include(e => e.Lesson).ToListAsync();
                return PartialView("_ExamsListPartial"/*, exams*/);
            }
        }
    }

}
