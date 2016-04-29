﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Azure.Devices;
using Message = Microsoft.Azure.Devices.Client.Message;


namespace SendCloudToDevice
{
    /* Sends message from  console app directly to Rasp Pi via IoT Hub */
    class Program
    {
        static DeviceClient deviceClient;

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
            deviceClient = DeviceClient.Create(IOT_HUB_URI, new DeviceAuthenticationWithRegistrySymmetricKey(NAME_OF_DEVICE, SHARED_ACCES_KEY));

            ParseConsoleMsg();
        }


        /// <summary>
        /// Possible caommands to send to IoT device. Will be turned into a function on the other end.
        /// </summary>
        private static void ParseConsoleMsg()
        {
            Boolean quitNow = false;
            while (!quitNow)
            {
                var readLine = Console.ReadLine();
                if (readLine == null) continue;

                string msg         = readLine.ToLower();


                // TODO: Need to pull string from here and find out if it includes things like "increase temp"
 
                int intInCommand  = GetIntVal(msg);
                string newMsg     = stringValFromMsg(msg);


                switch (newMsg)
                {
                    case "help":
                        Console.WriteLine("POSSIBLE COMMANDS:      \n" +
                                          "quit                    \n" +
                                          "temp (int val)          \n" +
                                          "increase light          \n" + 
                                          "decrease light          \n");
                        break;
                    case "quit":
                        quitNow = true;
                        break;
                    // -----------------------------------
                    case "temp 10":
                        break;
                    case "temp 30":
                        break;
                    case "temp 70":
                        break;
                    case "temp 100":
                        break;
                    // ------------------------------------
                    case "increase light":
                        //TODO: Create function to adjust GUI on client
                        break;
                    case "decrease light":
                        // TODO: Create a function on the client
                        break;
                    case "nav to log":
                        Console.WriteLine("nav to log");
                        break;
                }
                sendMessageToDevice(msg);
            }
        }

        private static string stringValFromMsg(string msg)
        {
            string[] separators = new string[] { ",", ".", "!", "\'", " ", "\'s" };
            string text         = msg;
            string newMsg       = "";

            foreach (string word in text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                Console.WriteLine(word);

                if (word.Contains("temp"))
                {
                    return newMsg;
                }
            }
            return newMsg;
        }


        /// <summary>
        /// Parses int from msg str which is sent to IoT device for increase / decrease temp, etc.
        /// </summary>
        /// <param name="msg">What do you want to the IoT device to do?</param>
        /// <returns>Integer used to change values in IoT device</returns>
        private static int GetIntVal(string msg)
        {
            int intInMsg = 0;
            string[] numbers = Regex.Split(msg, @"\D");
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    intInMsg = i;
                }
            }
            return intInMsg;
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
                var cloudToDeviceMessage    = DateTime.Now.ToLocalTime() + " - " + cmd;
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

                var serviceMessage =
                    new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(cloudToDeviceMessage))
                    {
                        Ack       = DeliveryAcknowledgement.Full,
                        MessageId = Guid.NewGuid().ToString()
                    };

                await serviceClient.SendAsync(DEVICE_TO_RECEIVE_MSG, serviceMessage);
                Console.WriteLine( cloudToDeviceMessage += $" sent to Device ID: " + DEVICE_TO_RECEIVE_MSG + "\n");
                await serviceClient.CloseAsync();
            }
            catch (Exception ex)
            {
               Console.WriteLine("EXCEPTION. Unable to sendMessageToDevice(). " + ex.ToString());
            }
        }

        private static async void sendMessageToDevice(string cmd)
        {
            try
            {
                var cloudToDeviceMessage    = cmd;
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



        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = "myFirstDevice",
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Thread.Sleep(1000);
            }
        }

    }
}
