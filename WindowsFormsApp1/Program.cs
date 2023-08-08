using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            // determine the package root, based on own location
            string result = Assembly.GetExecutingAssembly().Location;
            int index = result.LastIndexOf("\\");
            string rootPath = $"{result.Substring(0, index)}\\..\\";

            // process object to keep track of your child process
            Process newProcess = null;

            if (args.Length > 2)
            {
                // launch process based on parameter
                switch (args[2])
                {
                    case "/winforms":
                        //string parameters2 = ApplicationData.Current.LocalSettings.Values["parameters"] as string;
                        newProcess = Process.Start(rootPath + @"WindowsFormsApp1\WindowsFormsApp1.exe", "abcd");
                        break;
                }
            }
        }
    }
}
