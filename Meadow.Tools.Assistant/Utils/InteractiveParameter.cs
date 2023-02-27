using System;

namespace Meadow.Tools.Assistant.Utils
{
    public class InteractiveParameter<T>
    {
        public T[] Options { get; }

        private readonly Func<T, string> _toString;

        private readonly Action<string> _output;

        public InteractiveParameter(T[] options,Action<string> output, Func<T, string> toString)
        {
            _toString = toString;
            Options = options;
            _output = output;
        }

        public InteractiveParameter(T[] options,Action<string> output) : this(options,output, arg => arg?.ToString() ?? "--")
        {
        }

        public InteractiveParameter(T[] options) : this(options, Console.WriteLine)
        {
            
        }

        public T AskFor()
        {

            if (Options == null || Options.Length == 0)
            {
                return default;
            }
            
            if (Options.Length == 1)
            {
                return Options[0];
            }
            
            
            while (true)
            {
                int index = 1;

                _output("Please Enter the number associated with your choice:");

                foreach (var option in Options)
                {
                    var text = _toString(option);

                    _output($"[{index}]: {text}");

                    index++;
                }

                var input = Console.ReadLine();

                if (int.TryParse(input, out var choice))
                {
                    if (choice > 0 && choice <= Options.Length)
                    {
                        return Options[choice-1];
                    }
                }
            }
        }
    }
}