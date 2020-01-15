using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Receiver
{
    public class Program
    {
        const string EventHubConnectionString = "<namespace level connection string>";
        const string EventHubName = "<event hub name>";

        public static async Task Main(string[] args)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            var ehInfo = await eventHubClient.GetRuntimeInformationAsync();
            var receiveTasks = ehInfo.PartitionIds.Select(async partitionId =>
            {
                var receiver = eventHubClient.CreateReceiver(PartitionReceiver.DefaultConsumerGroupName, partitionId, EventPosition.FromEnd());

                Console.WriteLine($"{DateTime.Now} PARTITION {partitionId}: Starting to receive");

                while (true)
                {
                    var messages = await receiver.ReceiveAsync(10);
                    if (messages != null)
                    {
                        foreach (var message in messages)
                        {
                            var messageBody = ASCIIEncoding.UTF8.GetString(message.Body.Array);
                            Console.WriteLine($"{DateTime.Now} > PARTITION {partitionId}: {messageBody}");
                        }
                    }
                }
            });

            await Task.WhenAll(receiveTasks);
        }
    }
}
