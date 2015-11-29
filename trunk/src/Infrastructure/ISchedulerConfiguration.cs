using System;

namespace Consumentor.ShopGun
{
    public interface ISchedulerConfiguration
    {
        TimeSpan Interval { get; }
        TimeSpan RetryInterval { get; }
        int NumberOfRetries { get; }
    }
}