using System;
using System.Collections.Generic;

namespace Meadow.MySql.StateMachine
{
    public interface IState<TIn,TContext>
    {

        public List<Rule<TIn, TContext>> GetRules();
    }
}