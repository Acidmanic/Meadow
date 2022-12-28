using System.Collections.Generic;
using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public class InLineCommentState:IState<char,UnCommentContext>
    {
        public List<Rule<char, UnCommentContext>> GetRules()
        {
            return new List<Rule<char, UnCommentContext>>
            {
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c == 10 || c==13,
                    Invoke = (cr, cx) =>
                    {
                        cx.Content.Append(cr);
                        
                        return new InCodeState();
                    }
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => true,
                    Invoke = (cr, cx) => this
                }
            };
        }
    }
}