using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.Null3rdLevelIdCase;

[OwnerName("UserAnswers")]
public class UserAnswerStorage
{
    
    [UniqueMember]
    public string Id { get; set; }
    
    public string EvaluationId { get; set; }
    
    public int Choice { get; set; }
    
    public string QuestionId { get; set; }
    
    
    public string UserId { get; set; }
    
}