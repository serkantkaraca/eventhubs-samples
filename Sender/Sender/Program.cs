using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Sender
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

            var msgIndex = 0;
            while (true)
            {
                var messageBody = $"Message {msgIndex++}";
                var message = new EventData(ASCIIEncoding.ASCII.GetBytes(messageBody));
                await eventHubClient.SendAsync(message);

                Console.WriteLine($"{DateTime.Now} > Sent: {messageBody}");
                await Task.Delay(500);
            }
        }
    }
}
