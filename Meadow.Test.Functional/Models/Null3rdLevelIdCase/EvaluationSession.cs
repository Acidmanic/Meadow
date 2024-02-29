using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.Null3rdLevelIdCase;


[OwnerName("EvaluationSessions")]
[OneToMany(nameof(UserAnswerStorage.EvaluationId))]
public class EvaluationSessionStorage
{
    [UniqueMember] public string Id { get; set; }

    public bool IsStarted { get; set; }

    public bool IsSubmitted { get; set; }

    public long? StartDate { get; set; }
    public long? SubmitDate { get; set; }

    public long Duration { get; set; }
    
    public long? ExpiresAt { get; set; }

    public string UserId { get; set; }

    public string ExamId { get; set; }

    public double ResultPoints { get; set; }

    public bool ResultAccepted { get; set; }
    public int AcceptedLevel { get; set; }

    public UserStorage? User { get; set; }

    public ExamStorage? Exam { get; set; }
    
    public List<UserAnswerStorage> UserAnswers { get; set; }
}