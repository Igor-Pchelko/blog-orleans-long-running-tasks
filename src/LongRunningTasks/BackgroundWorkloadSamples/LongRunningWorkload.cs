using System;
using System.Threading;
using System.Threading.Tasks;
using LongRunningTasks.BackgroundWorkload;

namespace LongRunningTasks.BackgroundWorkloadSamples
{
    public class LongRunningWorkload : BackgroundWorkload<int, string>
    {
        public const int InvalidValue = 100;

        protected override async Task<string> ProcessAsync(int value, CancellationToken cancellationToken)
        {
            if (value == InvalidValue)
            {
                throw new ArgumentException("Unexpected value");
            }

            await Task.Delay(4000, cancellationToken);
            
            return $"Long running task with value {value} is completed";
        }
    }
}