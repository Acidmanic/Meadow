using Meadow.Requests;

namespace Meadow.SQLite.Requests
{
    public sealed class CreateDatabaseRequest:MeadowRequest<MeadowVoid,MeadowVoid>
    {
        public CreateDatabaseRequest() : base(false)
        {
            Execution = RequestExecution.RequestTextIsExecutable;

            
        }

        public override string RequestText
        {
            get => "";
            protected set
            {
                
            }
        }
    }
}