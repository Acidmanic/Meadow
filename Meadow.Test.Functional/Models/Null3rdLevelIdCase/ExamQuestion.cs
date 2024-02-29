using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.Null3rdLevelIdCase;

[OwnerName("ExamQuestions")]
public class ExamQuestionStorage
{
    [UniqueMember]
    public string Id { get; set; }
    
    public int Points { get; set; }
    
    public string Question { get; set; }
    
    public string Answers { get; set; }
    
    public int CorrectAnswer { get; set; }
    
    public string ExamId { get; set; }
}
