using System.Collections.Generic;
using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public class InCodeState : IState<char, UnCommentContext>
    {
        public List<Rule<char, UnCommentContext>> GetRules()
        {
            return new List<Rule<char, UnCommentContext>>
            {
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c == '#',
                    Invoke = (cr, cx) => new InLineCommentState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c == '-',
                    Invoke = (cr, cx) => new FirstDashState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c == '/',
                    Invoke = (cr, cx) => new FirstSlashState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => true,
                    Invoke = (cr, cx) =>
                    {
                        cx.Content.Append(cr);

                        return this;
                    }
                }
            };
        }
    }
}