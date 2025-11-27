using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProsysTask.Models
{
    public class Exam
    {
        public string LessonCode { get; set; }
        public int StudentNumber { get; set; } 
        public DateTime ExamDate { get; set; } 
        public int Score { get; set; } 
        public Lesson Lesson { get; set; }
        public Student Student { get; set; }
    }
}
