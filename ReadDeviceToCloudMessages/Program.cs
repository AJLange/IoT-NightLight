﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ReadDeviceToCloudMessages
{
    /* Reads messages which are sent to IoT hub */
    class Program
    {
        static string connectionString  = "HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;


        static void Main(string[] args)
        {
            Console.WriteLine("I can receive messages from the IoT Hub and display them here: \n");

            eventHubClient    = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);
            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
            Console.ReadLine();
        }


        private async static Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            //  receive messages from all the IoT hub device-to-cloud receive partitions.  Only receives messages sent after it starts. 
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) { continue;}

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine($"Message received. Partition: {partition} Data: '{data}'");
            }
        }
    }
}
