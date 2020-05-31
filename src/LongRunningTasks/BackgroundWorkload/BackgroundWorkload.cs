using System;
using System.Threading;
using System.Threading.Tasks;
using Orleans;

namespace LongRunningTasks.BackgroundWorkload
{
    public abstract class BackgroundWorkload<TRequest, TResponse> : Grain, IBackgroundWorkload<TRequest, TResponse>
    {
        private IResult _result = new None();
        private Task? _task;

        public Task<bool> StartAsync(TRequest request)
        {
            return StartAsync(request, CancellationToken.None);
        }

        public Task<bool> StartAsync(TRequest request, GrainCancellationToken cancellationToken)
        {
            return StartAsync(request, cancellationToken.CancellationToken);
        }
        
        private Task<bool> StartAsync(TRequest request, CancellationToken cancellationToken)
        {
            if (_task != null)
            {
                return Task.FromResult(false);
            }

            _result = new Started();
            _task = CreateTask(request, cancellationToken, TaskScheduler.Current);
        
            return Task.FromResult(true);
        }

        private Task CreateTask(TRequest request, CancellationToken cancellationToken, TaskScheduler orleansTaskScheduler) =>
            Task.Run(async () =>
            {
                try
                {
                    var response = await ProcessAsync(request, cancellationToken);
                    await InvokeGrainAsync(orleansTaskScheduler, grain => grain.CompleteAsync(response));
                }
                catch (Exception exception)
                {
                    await InvokeGrainAsync(orleansTaskScheduler, grain => grain.FailedAsync(exception));
                }
            });

        private Task InvokeGrainAsync(TaskScheduler orleansTaskScheduler, Func<IBackgroundWorkload<TRequest, TResponse>, Task> action) =>
            Task.Factory.StartNew(async () =>
            {
                var grain = GrainFactory.GetGrain<IBackgroundWorkload<TRequest, TResponse>>(this.GetPrimaryKeyString());
                await action(grain);
            }, CancellationToken.None, TaskCreationOptions.None, orleansTaskScheduler);

        public Task CompleteAsync(TResponse response)
        {
            if (!(_result is Started))
            {
                return Task.CompletedTask;
            }
            
            _task = null;
            _result = new Completed<TResponse>(response);
            return Task.CompletedTask;
        }

        public Task FailedAsync(Exception exception)
        {
            if (!(_result is Started))
            {
                return Task.CompletedTask;
            }

            _task = null;
            _result = new Failed(exception);
            return Task.CompletedTask;
        }

        public Task<IResult> GetResultAsync()
        {
            return Task.FromResult(_result);
        }

        protected abstract Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken);
    }
}