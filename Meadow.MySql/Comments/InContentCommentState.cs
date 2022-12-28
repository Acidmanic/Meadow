using System.Collections.Generic;
using Meadow.MySql.StateMachine;

namespace Meadow.MySql.Comments
{
    public class InContentCommentState:IState<char,UnCommentContext>
    {
        public List<Rule<char, UnCommentContext>> GetRules()
        {
            return new List<Rule<char, UnCommentContext>>
            {
                new Rule<char, UnCommentContext>
                {
                    Applies = c => c == '*',
                    Invoke = (cr, cx) => new FirstAstrxState()
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