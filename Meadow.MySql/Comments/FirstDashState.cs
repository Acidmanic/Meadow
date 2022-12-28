using System.Collections.Generic;
using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public class FirstDashState:IState<char,UnCommentContext>
    {
        public List<Rule<char, UnCommentContext>> GetRules()
        {
            return new List<Rule<char, UnCommentContext>>
            {
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c =='-',
                    Invoke = (c,cx) => new SecondDashState()
                },
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c!='-',
                    Invoke = (c,cx) => new InCodeState()
                }
            };
        }
    }
}