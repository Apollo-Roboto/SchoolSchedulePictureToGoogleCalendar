using CalendrierScholaireToGoogle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendrier_Scholaire_To_Google
{
    class Program
    {
        static void Main(string[] args)
        {
            string outedEventFilePath = @"C:\Users\Alex\source\repos\CalendrierScholaireToGoogle\Calendrier_Scholaire_To_Google\OutedEvent.json";
            string scheduleDirectoryPath = @"C:\Users\Alex\source\repos\CalendrierScholaireToGoogle\Calendrier_Scholaire_To_Google\ScheduleImages";
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
                        Console.WriteLine(schoolEvent);

                        // add event to the list
                        schoolEvents.Add(schoolEvent.toGoogle());

                    }
                }
            }

            // save all the loaded events into a json file
            saveToJson(outedEventFilePath, schoolEvents);


            Console.ReadKey();
        }

        static void saveToJson(string path, List<Dictionary<string, object>> list)
        {
            // load the event into a json file
            string json = JsonConvert.SerializeObject(list);

            // delete old file
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // create and save json file
            TextWriter tw = new StreamWriter(path, false);
            tw.Write(json);
            tw.Close();
        }
    }
}
