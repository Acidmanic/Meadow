namespace Meadow.Test.Functional.Models.Bug2
{
    public class Task : Card
    {
        public long StepId { get; set; }

        public long ProjectId { get; set; }

        public long GoalId { get; set; }

        public long IterationId { get; set; }
    }
}