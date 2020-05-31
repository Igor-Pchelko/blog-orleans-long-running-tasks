using Xunit;

namespace LongRunningTasks.Tests
{
    [CollectionDefinition(nameof(TestClusterCollection))]
    public class TestClusterCollection : ICollectionFixture<TestClusterFixture>
    {
    }
}