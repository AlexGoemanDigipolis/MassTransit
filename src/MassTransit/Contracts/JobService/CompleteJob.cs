namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;
    using Topology;


    [ConfigureConsumeTopology(false)]
    public interface CompleteJob
    {
        Guid JobId { get; }

        DateTime Timestamp { get; }

        TimeSpan Duration { get; }

        /// <summary>
        /// The job, as an object dictionary
        /// </summary>
        IDictionary<string, object> Job { get; }

        /// <summary>
        /// The result of the job
        /// </summary>
        IDictionary<string, object> Result { get; }
    }
}
