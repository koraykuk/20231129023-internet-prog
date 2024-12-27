using _20231129023.Models;

namespace _20231129023.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public DateTime Date { get; set; }
    }
}
