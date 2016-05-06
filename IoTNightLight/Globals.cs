﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.Globalization.Collation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoTNightLight
{
    public static class Globals
    {
        private static Frame     rootFrame;
        private static MainPage  mainPage;
        
        static Globals()
        {
            rootFrame = Window.Current.Content as Frame;
            mainPage  = (MainPage)rootFrame.Content;     
        }


        /// <summary>
        /// TODO: have this return a different value based on the string. Just demo right now
        /// </summary>
        /// <param name="msg"></param>
        private static string stringValFromMsg(string msg)
        {
            string[] separators = new string[] { ",", ".", "!", "\'", " ", "\'s" };
            string text         = msg;
            string newMsg       = "";

            foreach (string word in text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                Debug.WriteLine(word);

                if (word.Contains("temp"))
                {
                    return newMsg;
                }
            }
            return newMsg;
        }


        /// <summary>
        /// Interpreets commands from console app and relays to the correct function
        /// </summary>
        /// <param name="msg">Command sent from console app</param>
        public static void parseMsg(string msg)
        {
            int    intInMsg        = GetIntVal(msg); //TODO: May not need this

            switch (msg)
            {
                case "tween":
                    //TODO: Insert tweening value
                    break;
                // ---------------------------------------- TEMPERATURE
                case "temp 10":
                    Debug.WriteLine("temp 10");
                    mainPage.Goto(10);
                    break;
                case "temp 30":
                    Debug.WriteLine("temp 30");
                    mainPage.Goto(30);
                    break;
                case "temp 70":
                    Debug.WriteLine("temp 70");
                    mainPage.Goto(70);
                    break;
                case "temp 100":
                    Debug.WriteLine("temp 100");
                    mainPage.Goto(100);
                    break;
                // ---------------------------------------- LIGHT
                case "light 10":
                    mainPage.Goto(10);
                    break;
                case "light 30":
                    mainPage.Goto(30);
                    break;
                case "light 70":
                    mainPage.Goto(70);
                    break;
                case "light 100":
                    mainPage.Goto(100);
                    break;
                // ---------------------------------------- Moisture
                case "moisture 10":
                    mainPage.Goto(10);
                    break;
                case "moisture 30":
                    mainPage.Goto(30);
                    break;
                case "moisture 70":
                    mainPage.Goto(70);
                    break;
                case "moisture 100":
                    mainPage.Goto(100);
                    break;
                // ---------------------------------------- NAVIGATION
                case "nav to log":
                    Debug.WriteLine("navigating to log page");
                    mainPage.ChangeTitleText("Log");
                    break;
                case "nav to moisture":
                    Debug.WriteLine("navigating to main page");
                    mainPage.ChangeTitleText("Moisture Page");
                    break;
                case "nav to temp":
                    Debug.WriteLine("navigating to temp page");
                    mainPage.ChangeTitleText("Temperature");
                    break;
                case "nav to light":
                    Debug.WriteLine("navigating to light page");
                    mainPage.ChangeTitleText("Light");
                    break;
            }
        }


        /// <summary>
        /// Parses int from msg str which is sent to IoT device for increase / decrease temp, etc.
        /// </summary>
        /// <param name="msg">What do you want to the IoT device to do?</param>
        /// <returns>Integer used to change values in IoT device</returns>
        private static int GetIntVal(string msg)
        {
            int intInMsg     = 0;
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


        /* NAVIGATION
         * ==========================================================*/
        public static void Nav_To_Temp(object sender, RoutedEventArgs e)
        {
            mainPage.ChangeTitleText("Temperature");
        }

        public static void Nav_To_Main(object sender, RoutedEventArgs e)
        {
            mainPage.ChangeTitleText("Moisture");
        }

        public static void Nav_To_Light(object sender, RoutedEventArgs e)
        {
            mainPage.ChangeTitleText("Light");
        }

        public static void Nav_To_Log(object sender, RoutedEventArgs e)
        {
            mainPage.ChangeTitleText("Log");
        }

    }
}
