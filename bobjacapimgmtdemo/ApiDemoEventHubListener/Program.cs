using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ApiDemoEventHubListener
{
    class Program
    {
        static void Main(string[] args)
        {
            string storageAccountName = "bobjacapidemo";
            string storageAccountKey = "lvYpWSHkdHpCOnv91B4tGLClqeIIe0ouDW9mBAYAlm69NrHGHXhW/jrIJx5nTMgkxPWGzOoJ1jVMTCFjVvh9mg==";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageAccountName, storageAccountKey);

            string eventHubConnectionString = "Endpoint=sb://bobjacsb.servicebus.windows.net/;SharedAccessKeyName=Receiving;SharedAccessKey=RjMnP8d1rUzYF9knM7eB3lGrV231/WAtEOCI5dI8AtM=";
            string eventProcessorHostName = Guid.NewGuid().ToString();
            string eventHubName = "apimgtdemo";
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            eventProcessorHost.RegisterEventProcessorAsync<ApiManagementEventProcessor>().Wait();

            Console.WriteLine("Receiving.  Press enter key to stop worker.");
            Console.ReadLine();
        }
    }
}
