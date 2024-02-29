using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.Null3rdLevelIdCase;



[OwnerName("Exams")]
[OneToMany(nameof(ExamQuestionStorage.ExamId))]
public class ExamStorage
{
    [UniqueMember]
    public string Id { get; set; }
    
    public int DurationMinutes { get; set; }
    
    public int Level { get; set; }
    
    public double MinimumAcceptancePoints { get; set; }
    
    public List<ExamQuestionStorage> Questions { get; set; }
}

