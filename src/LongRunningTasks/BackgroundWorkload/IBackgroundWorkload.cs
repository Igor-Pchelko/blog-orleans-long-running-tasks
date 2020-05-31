using System;
using System.Threading.Tasks;
using Orleans;

namespace LongRunningTasks.BackgroundWorkload
{
    public interface IBackgroundWorkload<in TRequest, in TResponse> : IGrainWithStringKey
    {
        /// <summary>
        /// Starts long running workload.
        /// </summary>
        /// <param name="request">Workload request parameters.</param>
        /// <returns>True if new workload started, False when it's already started.</returns>
        Task<bool> StartAsync(TRequest request);

        /// <summary>
        /// Starts long running workload with cancellation token.
        /// </summary>
        /// <param name="request">Workload request parameters.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if new workload started, False when it's already started.</returns>
        Task<bool> StartAsync(TRequest request, GrainCancellationToken cancellationToken);

        /// <summary>
        /// Completes workload execution with specified response.
        /// This method is called internally and not intended to be called outside the grain.
        /// </summary>
        /// <param name="response">Workload response.</param>
        /// <returns>Task.</returns>
        Task CompleteAsync(TResponse response);
        
        /// <summary>
        /// Completes workload execution with failed exception response.
        /// </summary>
        /// <param name="exception">Exception details</param>
        /// <returns>Task.</returns>
        Task FailedAsync(Exception exception);
        
        /// <summary>
        /// Returns execution result.
        /// </summary>
        /// <returns>Workload execution result.</returns>
        Task<IResult> GetResultAsync();
    }
}