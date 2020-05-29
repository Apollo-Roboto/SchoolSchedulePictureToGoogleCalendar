using CalendrierScholaireToGoogle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            FileInfo[] files = dir.GetFiles();

            List<Dictionary<string, object>> schoolEvents = new List<Dictionary<string, object>>();

            // for each file in the picture direcory
            foreach(FileInfo file in files)
            {
                SchoolSchedule sc = SchoolScheduleLoader.Load(file.FullName);
                if (sc != null)
                {
                    for (int i = 0; i < sc.Count; i++)
                    {
                        SchoolGoogleEvent schoolEvent = sc[i];

                        // add event to the list
                        schoolEvents.Add(schoolEvent.toGoogle());

                    }
                }
            }

            // save all the loaded events into a json file
            saveToJson(outedEventFileName, schoolEvents);

            Console.Write($"This will add {schoolEvents.Count} event to your calendar.\nContinue?[Y/N]: ");
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
        
        static void saveToJson(string fileName, List<Dictionary<string, object>> list)
        {
            // load the event into a json file
            string json = JsonConvert.SerializeObject(list);

            // delete old file
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
