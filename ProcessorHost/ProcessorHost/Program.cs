using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace ProcessorHost
{
    class Program
    {
        const string EventHubConnectionString = "<namespace level connection string>";
        const string EventHubName = "<event hub name>";
        const string StorageConnectionString = "<azure storage connection string>";
        const string StorageContainerName = "<lease container name>";

        public static async Task Main(string[] args)
        {
            string hostName = "host-" + Guid.NewGuid().ToString().Substring(0, 5);

            Console.WriteLine($"Registering EventProcessor {hostName}");

            var eventProcessorHost = new EventProcessorHost(
                hostName,
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(new EventProcessorOptions()
            {
                InitialOffsetProvider = (partitionId) => EventPosition.FromEnd()
            });

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
