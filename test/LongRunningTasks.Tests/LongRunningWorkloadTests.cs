using System;
using System.Threading.Tasks;
using FluentAssertions;
using LongRunningTasks.BackgroundWorkload;
using LongRunningTasks.BackgroundWorkloadSamples;
using Orleans;
using Xunit;

namespace LongRunningTasks.Tests
{
    [Collection(nameof(TestClusterCollection))]
    public class BackgroundWorkloadSampleTest
    {
        private readonly TestClusterFixture _fixture;

        public BackgroundWorkloadSampleTest(TestClusterFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task Should_complete_successfully()
        {
            // Arrange
            var grain = _fixture.Cluster!.Client.GetGrain<IBackgroundWorkload<int, string>>(nameof(Should_complete_successfully));

            // Act & Assert
            await CompleteSuccessfully(grain, 10);
            await CompleteSuccessfully(grain, 15);
        }

        private static async Task CompleteSuccessfully(IBackgroundWorkload<int, string> grain, int value)
        {
            // Act
            await grain.StartAsync(value);
            var result = await WaitForResultAsync(grain);

            // Assert
            result.Should().BeEquivalentTo(new Completed<string>($"Long running task with value {value} is completed"));
        }

        private static async Task<IResult> WaitForResultAsync(IBackgroundWorkload<int, string> grain)
        {
            IResult? result = null;

            while (result == null || result is Started)
            {
                result = await grain.GetResultAsync();
            }

            return result;
        }

        [Fact]
        public async Task Should_not_start_if_running()
        {
            // Arrange
            var grain = _fixture.Cluster!.Client.GetGrain<IBackgroundWorkload<int, string>>(nameof(Should_not_start_if_running));

            // Act
            var startResult1 = await grain.StartAsync(10);
            var startResult2 = await grain.StartAsync(15);
            var result = await WaitForResultAsync(grain);

            // Assert
            startResult1.Should().BeTrue();
            startResult2.Should().BeFalse();
            result.Should().BeEquivalentTo(new Completed<string>($"Long running task with value 10 is completed"));
        }
        
        [Fact]
        public async Task Should_fail_with_unexpected_value()
        {
            // Arrange
            var grain = _fixture.Cluster!.Client.GetGrain<IBackgroundWorkload<int, string>>(nameof(Should_fail_with_unexpected_value));

            // Act
            var startResult = await grain.StartAsync(LongRunningWorkload.InvalidValue);
            var result = await WaitForResultAsync(grain);

            // Assert
            startResult.Should().BeTrue();
            result.Should().BeOfType<Failed>()
                .Which.Exception.Should().BeOfType<ArgumentException>()
                .Which.Message.Should().Be("Unexpected value");
        }
        
        [Fact]
        public async Task Should_fail_with_task_canceled()
        {
            // Arrange
            var grain = _fixture.Cluster!.Client.GetGrain<IBackgroundWorkload<int, string>>(nameof(Should_fail_with_task_canceled));

            // Act
            var cancellationTokenSource = new GrainCancellationTokenSource();
            var startResult = await grain.StartAsync(10, cancellationTokenSource.Token);
            await Task.Delay(1000);
            await cancellationTokenSource.Cancel();
            var result = await WaitForResultAsync(grain);

            // Assert
            startResult.Should().BeTrue();
            result.Should().BeOfType<Failed>()
                .Which.Exception.Should().BeOfType<TaskCanceledException>();
        }
    }
}
