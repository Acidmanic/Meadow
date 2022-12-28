namespace Meadow.MySql.StateMachine
{
    public class StateMachine<TIn,TContext> where TContext : new()
    {


        private IState<TIn, TContext> _state;

        private TContext _context;


        public void Initialize(IState<TIn, TContext> state)
        {
            _state = state;

            _context = new TContext();
        }
        


        public void Pass(TIn input)
        {

            var rules = _state.GetRules();

            foreach (var rule in rules)
            {
                if (rule.Applies(input))
                {
                    _state = rule.Invoke(input, _context);

                    break;
                }
            }
        }


        public virtual TContext FinishOff()
        {

            return _context;
        }
    }
}