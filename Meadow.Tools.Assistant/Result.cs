using System;

namespace Meadow.Tools.Assistant
{

    public class Result
    {
        
        public bool Success { get; set; }
        
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

        public static bool operator ==(Result value, bool bValue)
        {
            return value?.Success == bValue;
        }

        public static bool operator !=(Result value, bool bValue)
        {
            return !(value == bValue);
        }
        
        public static implicit operator bool(Result r) => r.Success;
    }
    
    public class Result<T>:Result
    {
        
        public T Value { get; set; }

        public Result(bool success, T value)
        {
            Success = success;
            value = value;
        }

        public Result()
        {
        }

        public static implicit operator T(Result<T> r) => r.Value;
    }
}