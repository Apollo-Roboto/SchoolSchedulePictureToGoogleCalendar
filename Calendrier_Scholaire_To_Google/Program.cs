using CalendrierScholaireToGoogle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calendrier_Scholaire_To_Google
{
    class Program
    {
        static string workingDirectory = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {

            string outedEventFileName = "OutedEvent.json";
            
            string scheduleDirectoryPath = "ScheduleImages";
            DirectoryInfo dir = new DirectoryInfo(scheduleDirectoryPath);

            SchoolSchedule[] schoolSchedules = SchoolScheduleLoader.Load(dir.GetFiles());
            saveToJson(outedEventFileName, schoolSchedules);

            Console.Write($"This will add multiple events to your calendar.\nContinue?[Y/N]: ");
            if (Console.ReadLine().ToUpper().Equals("Y"))
            {
                int result = sendToGoogle(outedEventFileName);
                if (result == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Done!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Canceled");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        static int sendToGoogle(string eventFileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.WorkingDirectory = workingDirectory;
            processInfo.FileName = "json_to_Google_Calendar.py";
            processInfo.Arguments = eventFileName;
            processInfo.CreateNoWindow = true;

            Process process = Process.Start(processInfo);
            process.WaitForExit();
            return process.ExitCode;
        }

        static void saveToJson(string fileName, SchoolSchedule[] schoolSchedules)
        {
            List<Dictionary<string, object>> schoolEvents = new List<Dictionary<string, object>>();

            foreach (SchoolSchedule sc in schoolSchedules)
            {
                for (int i = 0; i < sc.Count; i++)
                {
                    schoolEvents.Add(sc[i].toGoogle());
                }
            }

            // load the event into a json file
            string json = JsonConvert.SerializeObject(schoolEvents);

            // delete old file if exist
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // create and save json file
            TextWriter tw = new StreamWriter(fileName, false);
            tw.Write(json);
            tw.Close();
        }

    }
}
