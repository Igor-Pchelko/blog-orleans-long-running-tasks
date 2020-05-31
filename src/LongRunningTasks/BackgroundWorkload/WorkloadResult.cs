using System;

namespace LongRunningTasks.BackgroundWorkload
{
    /// <summary>
    /// Workload execution result.
    /// </summary>
    public interface IResult { }
    
    /// <summary>
    /// Not started.
    /// </summary>
    public class None : IResult { }
    
    /// <summary>
    /// Workload started.
    /// </summary>
    public class Started : IResult { }

    /// <summary>
    /// Workload completed.
    /// </summary>
    /// <typeparam name="TResponse">Workload completion result.</typeparam>
    public class Completed<TResponse> : IResult
    {
        public TResponse Response { get; }

        public Completed(TResponse response)
        {
            Response = response;
        }
    }
    
    /// <summary>
    /// Workload failed result.
    /// </summary>
    public class Failed : IResult
    {
        public Exception Exception { get; }

        public Failed(Exception exception)
        {
            Exception = exception;
        }
    }
}