using _20231129023.Models;

namespace _20231129023.ViewModels
{
    public class QuestionDetails
    {
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public List<Answer> Answers;
        public DateTime QuestionDate { get; set; }

    }
}
