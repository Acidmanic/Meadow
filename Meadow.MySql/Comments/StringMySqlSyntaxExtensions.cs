using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public static class StringMySqlSyntaxExtensions
    {



        public static string ClearMySqlComments(this string sql)
        {
            var machine = new StateMachine<char, UnCommentContext>();

            var chars = sql.ToCharArray();

            machine.Initialize(new InCodeState());
            
            foreach (var c in chars)
            {
                machine.Pass(c);
            }

            var context = machine.FinishOff();

            return context.Content.ToString();
        }
    }
}