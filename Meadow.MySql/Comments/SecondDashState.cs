using System.Collections.Generic;
using System.ComponentModel;
using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public class SecondDashState : IState<char, UnCommentContext>
    {
        public List<Rule<char, UnCommentContext>> GetRules()
        {
            return new List<Rule<char, UnCommentContext>>
            {
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c==13,
                    Invoke = (c,cx) => new InCodeState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c!=13 && (char.IsControl(c) ||  char.IsWhiteSpace(c)),
                    Invoke = (c,cx) => new InLineCommentState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => true,
                    Invoke = (c, cx) =>
                    {

                        cx.Content.Append("--").Append(c);
                        
                        return new InCodeState();
                    }
                }
            };
        }
    }
}