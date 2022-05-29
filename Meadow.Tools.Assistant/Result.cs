namespace Meadow.Tools.Assistant
{

    public class Result
    {
        public static Result<TResult> Failure<TResult>()
        {
            return new Result<TResult>
            {
                Success = false
            };
        }

        public static Result<TResult> Successful<TResult>(TResult value)
        {
            return new Result<TResult>
            {
                Success = true,
                Value = value
            };
        }
    }
    
    public class Result<T>:Result
    {
        public bool Success { get; set; }

        public T Value { get; set; }

        public Result(bool success, T value)
        {
            Success = success;
            value = value;
        }

        public Result()
        {
        }

        
    }
}