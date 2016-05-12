﻿using System;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices;


namespace SendCloudToDevice
{
    /* Sends message from  console app directly to Rasp Pi via IoT Hub */
    class Program
    {
        private const string IOT_HUB_URI           = "dv-iot-labs.azure-devices.net";
        private const string DEVICE_TO_RECEIVE_MSG = "minwinpc";
        private const string NAME_OF_DEVICE        = "DeviceClient";
        private const string SHARED_ACCES_KEY      = "1q8HuqL0kPRZG9TAX62qBeCs88mOx1CAU7Jdsm5F9/E=";
        private const string IOT_HUB_CONN_STRING   = "HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";
              

        static void Main(string[] args)
        {
            Console.WriteLine("SIMULATED DEVICE\n");
            Console.WriteLine("I can send messages directly to the Raspberry Pi. Current target: " +
                              DEVICE_TO_RECEIVE_MSG);
            // Init client
            DeviceClient.Create(IOT_HUB_URI, new DeviceAuthenticationWithRegistrySymmetricKey(NAME_OF_DEVICE, SHARED_ACCES_KEY));
            ParseConsoleMsg();
        }


        /// <summary>
        /// Reads text from console app and sends messages to the IoT Hub, which the Rasp Pi reads.
        /// </summary>
        private static void ParseConsoleMsg()
        {
            Boolean quitNow = false;
            while (!quitNow)
            {
                var readLine = Console.ReadLine();
                if (readLine == null) {continue;}

                string msg  = readLine.ToLower();    

                switch (msg)
                {
                     case "help":
                        Console.WriteLine("POSSIBLE COMMANDS:       \n" +
                                          "quit                     \n" +
                                          "temp  (int val)          \n" +
                                          "light (int val)          \n" +
                                          "nav to (page name)       \n");
                        break;
                    case "quit":
                        quitNow = true;
                        break;
                }
                sendMessageToDevice(msg);
            }
        }


        /// <summary>
        /// Can send messages directly to Raspberry Pi. Requests delivery acknowledgement from device upoen receipt. 
        /// </summary>
        /// <param name="cmd">String to pass to the IoT device, which will turn into a function upon receipt.</param>
        private static async void sendMessageToDevice(string cmd)
        {
            try
            {
                var cloudToDeviceMessage    = cmd;
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

                var serviceMessage =
                    new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(cloudToDeviceMessage))
                    {
                        Ack       = DeliveryAcknowledgement.Full,
                        MessageId = Guid.NewGuid().ToString()
                    };

                await serviceClient.SendAsync(DEVICE_TO_RECEIVE_MSG, serviceMessage);
                Console.WriteLine(cloudToDeviceMessage + (" sent to Device ID: " + DEVICE_TO_RECEIVE_MSG + "\n"));
                await serviceClient.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION. Unable to sendMessageToDevice(). " + ex.ToString());
            }
        }



        /// <summary>
        /// Can send messages directly to Raspberry Pi. Requests delivery acknowledgement from device upoen receipt. 
        /// Also sends timestamp for debugging.
        /// </summary>
        /// <param name="cmd">String to pass to the IoT device, which will turn into a function upon receipt.</param>
        private static async void sendDetailedMessageToDevice(string cmd)
        {
            try
            {
                // TODO: String parses cannot parse out the date/time just yet. Don't use this.
                var cloudToDeviceMessage = DateTime.Now.ToLocalTime() + " - " + cmd;
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

                var serviceMessage =
                    new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(cloudToDeviceMessage))
                    {
                        Ack = DeliveryAcknowledgement.Full,
                        MessageId = Guid.NewGuid().ToString()
                    };

                await serviceClient.SendAsync(DEVICE_TO_RECEIVE_MSG, serviceMessage);
                Console.WriteLine(cloudToDeviceMessage += $" sent to Device ID: " + DEVICE_TO_RECEIVE_MSG + "\n");
                await serviceClient.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION. Unable to sendMessageToDevice(). " + ex.ToString());
            }
        }


    }
}
