using System;

namespace Meadow.MySql.StateMachine
{
    public class Rule<TIn,TContext>
    {
        
        public Func<TIn,bool> Applies { get; set; }
        
        public Func<TIn,TContext,IState<TIn,TContext>> Invoke { get; set; } 
    }
}