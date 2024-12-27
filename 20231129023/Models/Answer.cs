namespace _20231129023.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string AnswerText { get; set; }
        public DateTime Date { get; set; }
    }
}
