using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using System.Threading;

namespace ApiDemoEventHubListener
{
    public class ApiManagementEventProcessor : IEventProcessor
    {
        private int totalMessages = 0;
        private Stopwatch checkpointStopWatch;

        public ApiManagementEventProcessor()
        {
            this.LastMessageOffset = "-1";
        }

        public event EventHandler ProcessorClosed;

        public bool IsInitialized { get; private set; }

        public bool IsClosed { get; private set; }

        public bool IsReceivedMessageAfterClose { get; set; }

        public int TotalMessages
        {
            get
            {
                return this.totalMessages;
            }
        }

        public CloseReason CloseReason { get; private set; }

        public PartitionContext Context { get; private set; }

        public string LastMessageOffset { get; private set; }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            // where you close the processor.
            Console.WriteLine("{0} > Close called for processor with PartitionId '{1}' and Owner: {2} with reason '{3}'.", DateTime.Now.ToString(), context.Lease.PartitionId, context.Lease.Owner ?? string.Empty, reason);
            this.IsClosed = true;
            this.checkpointStopWatch.Stop();
            this.CloseReason = reason;
            this.OnProcessorClosed();
            return context.CheckpointAsync();
        }

        public Task OpenAsync(PartitionContext context)
        {
            // here is the place for you to initialize your processor, e.g. db connection or other stuff. please ensure you have
            // retry or fault handling properly
            Console.WriteLine("{0} > Processor Initializing for PartitionId '{1}' and Owner: {2}.", DateTime.Now.ToString(), context.Lease.PartitionId, context.Lease.Owner ?? string.Empty);
            this.Context = context;
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            this.IsInitialized = true;

            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            try
            {
                foreach (EventData message in messages)
                {
                    Console.WriteLine("{0} > received message: {1} at partition {2}, owner: {3}, offset: {4}", DateTime.Now.ToString(), Encoding.UTF8.GetString(message.GetBytes()), context.Lease.PartitionId, context.Lease.Owner, message.Offset);

                    // increase the totally amount.
                    Interlocked.Increment(ref this.totalMessages);
                    this.LastMessageOffset = message.Offset;
                }

                if (this.IsClosed)
                {
                    this.IsReceivedMessageAfterClose = true;
                }

                if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
                {
                    lock (this)
                    {
                        this.checkpointStopWatch.Reset();
                        return context.CheckpointAsync();
                    }
                }
            }
            catch (Exception)
            {
                // processor exception handling here.
            }

            return Task.FromResult<object>(null);
        }
        protected virtual void OnProcessorClosed()
        {
            EventHandler handler = this.ProcessorClosed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
