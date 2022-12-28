using System.Collections.Generic;
using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public class FirstAstrxState:IState<char,UnCommentContext>
    {
        public List<Rule<char, UnCommentContext>> GetRules()
        {
            return new List<Rule<char, UnCommentContext>>
            {
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c == '/',
                    Invoke = (c, cx) => new InCodeState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c != '/',
                    Invoke = (c, cx) => new InContentCommentState()
                }
            };
        }
    }
}