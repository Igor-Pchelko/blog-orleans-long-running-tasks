using System;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Configuration;
using Orleans.TestingHost;

namespace LongRunningTasks.Tests
{
    public class TestClusterFixture : IDisposable 
    {
        public TestCluster? Cluster { get; }

        public TestClusterFixture()
        {
            var builder = new TestClusterBuilder();
            builder.AddClientBuilderConfigurator<TestClientBuilderConfigurator>();
            Cluster = builder.Build();
            Cluster.Deploy();
        }

        public void Dispose()
        {
            Cluster?.StopAllSilos();
        }

        private class TestClientBuilderConfigurator : IClientBuilderConfigurator
        {
            public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
            {
                clientBuilder
                    .Configure<ClientMessagingOptions>(options =>
                    {
                        options.ResponseTimeout = TimeSpan.FromSeconds(5);
                        options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(5);
                    });
            }
        }
    }
}