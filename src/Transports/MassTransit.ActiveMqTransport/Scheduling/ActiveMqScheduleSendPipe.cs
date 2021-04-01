using MassTransit.Initializers.TypeConverters;

namespace MassTransit.ActiveMqTransport.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Scheduling;


    /// <summary>
    /// Sets the message enqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActiveMqScheduleSendPipe<T> :
        ScheduleSendPipe<T>
        where T : class
    {
        readonly DateTime _scheduledTime;
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();

        public ActiveMqScheduleSendPipe(IPipe<SendContext<T>> pipe, DateTime scheduledTime)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public override Task Send(SendContext<T> context)
        {
            ScheduledMessageId = ScheduleTokenIdCache<T>.GetTokenId(context.Message, context.MessageId);

            //var delay = Math.Max(0, (long)(_scheduledTime.Kind == DateTimeKind.Local
            //    ? _scheduledTime - DateTime.Now
            //    : _scheduledTime - DateTime.UtcNow).TotalMilliseconds);

            //if (delay > 0)
            //    context.Headers.Set("AMQ_SCHEDULED_DELAY", delay);

            _dateTimeConverter.TryConvert(_scheduledTime.ToUniversalTime(), out long scheduledMillis);
            context.Headers.Set("_AMQ_SCHED_DELIVERY", scheduledMillis);

            return base.Send(context);
        }
    }
}
