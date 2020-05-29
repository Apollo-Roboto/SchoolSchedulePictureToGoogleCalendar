using Calendrier_Scholaire_To_Google;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tesseract;

namespace CalendrierScholaireToGoogle
{
    static class SchoolScheduleLoader
    {

        static uint numOfDay = 5;
        static int lineWidth = 2;

        static string xmlFilePath = "SchoolClasses.xml";
        static SchoolClassesXmlReader dataFile;

        static Dictionary<int, string> timeByPixel;
        // months Array to figure out what month we are looking at, regex is used here because Tesseract mess with letters a lot
        static string[] monthsArr = { "janv", "f[eé]vr", "mars", "avr", "ma[li]", "juin", "jui[li]", "ao[ûu]t", "sept", "oct", "nov", "d[ée]c" };

        static SchoolScheduleLoader()
        {
            dataFile = new SchoolClassesXmlReader(xmlFilePath);

            // initialisation of the timeByPixel dictionary, this is used to
            // figure out the time an event start and end
            // exemple, the event start at pixel 648, then the time corresponding to 648 is 13:00

            timeByPixel = new Dictionary<int, string>();

            for (int h = 8, m = 0, i = 47; i <= 1726; i += 30)
            {
                string time = h + "h" + m;
                if (time.EndsWith("h0"))
                    time += "0";
                timeByPixel.Add(i, time);

                m += 15;
                if (m >= 60)
                {
                    m = 0;
                    h++;
                }
            }
        }



        public static SchoolSchedule Load(string path)
        {
            Bitmap img;
            SchoolSchedule sc = new SchoolSchedule();

            try
            {
                img = new Bitmap(path);
            }catch(FileNotFoundException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }

            

            // find vertical line seperating days
            Color[] colors = new Color[] { 
                Color.FromArgb(255, 211, 211, 211),
                Color.FromArgb(255, 200, 213, 229),
                Color.FromArgb(255, 203, 213, 223)};
            List<int> verticalLines = FindLineInRow(ref img, lineWidth, colors);
            
            // for each day
            for(int i = 0; i < verticalLines.Count; i++)
            {

                // information to get
                int year;
                int monthNum = -1;
                int dayOfMonth;
                string code;
                string location;
                string teacher;
                string name;
                string friendlyName;
                string colorId;
                string timeZone;
                string startTime = null;
                string endTime = null;
                string startDateTime;
                string endDateTime;
                string group;

                // get year from the xml file
                year = dataFile.getYear();
                group = dataFile.getGroup();
                timeZone = dataFile.getTimeZone();

                int col = verticalLines[i];
                // break if the col is out of image or all the days have been checked or if doesn't have a next col
                if (col >= img.Width || i > numOfDay - 1 || i + 1 >= verticalLines.Count)
                    break;
                int nextCol = verticalLines[i + 1];

                // read the date on top
                Rectangle dateZone = new Rectangle(col, lineWidth, (nextCol - col), 44);
                string dateOCR = doOCR(ref img, dateZone);

                // find the day of month
                dayOfMonth = int.Parse(new Regex("[0-9]+").Match(dateOCR).Value);

                // find the month number
                for(int j = 0; j < monthsArr.Length; j++)
                {
                    string month = monthsArr[j].ToLower();
                    Regex regex = new Regex(month);
                    if (regex.Match(dateOCR.ToLower()).Success)
                    {
                        monthNum = j;
                        break;
                    }
                }


                // find horizontal lines seperating events
                colors = new Color[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 1, 1, 1) };
                List<int> horizontalLines = FindLineInColumn(ref img, col, colors);

                // for each event on that day
                for(int j = 0; j < horizontalLines.Count; j += 2)
                {
                    int row = horizontalLines[j];
                    // break if row is out of image or doesn't have a next row
                    if (row >= img.Width || j + 1 >= horizontalLines.Count)
                        break;
                    int nextRow = horizontalLines[j + 1];
                     
                    // create the scan zone (where the event is on the image)
                    Rectangle zone = new Rectangle();
                    zone.X = col;
                    zone.Y = row;
                    zone.Width = nextCol - col - lineWidth;
                    zone.Height = nextRow - row - lineWidth;

                    // Read text in that zone
                    string text = doOCR(ref img, zone);

                    // find the damn code and other information from text and zone
                    code = FindCodeInText(text);

                    if (timeByPixel.ContainsKey(zone.Y - 1))
                        startTime = timeByPixel[zone.Y - 1];

                    startDateTime = generateDateTime(year, monthNum, dayOfMonth, startTime);

                    if (timeByPixel.ContainsKey(zone.Y + zone.Height + 1))
                        endTime = timeByPixel[zone.Y + zone.Height + 1];

                    endDateTime = generateDateTime(year, monthNum, dayOfMonth, endTime);


                    // get some information from the data file

                    // correction of code, is null if not found in data
                    code = dataFile.getCode(code);

                    // gathering of information
                    name = dataFile.getName(code);
                    friendlyName = dataFile.getFriendlyName(code);
                    teacher = dataFile.getTeacher(code);
                    colorId = dataFile.getColorId(code);

                    // since location is optional in xml and can be found in text
                    location = dataFile.getLocation(code);
                    if (location == null)
                        location = FindLocationInText(text);

                    // creation of the school event
                    SchoolGoogleEvent e = new SchoolGoogleEvent();
                    e.Location = location;
                    e.Code = code;
                    e.Summary = friendlyName;
                    e.FullSummary = name;
                    e.Teacher = teacher;
                    e.Group = group;
                    e.ColorId = colorId;
                    e.StartTimeZone = timeZone;
                    e.StartDateTime = startDateTime;
                    e.EndTimeZone = timeZone;
                    e.EndDateTime = endDateTime;

                    sc.AddEvent(e);

                }

            }

            return sc;
        }



        private static string generateDateTime(int year, int monthNum, int dayOfMonth, string time)
        {
            //2020-7-14T12:15:00
            string hour = time.Split('h')[0];
            string min = time.Split('h')[1];
            string date = $"{year}-{monthNum + 1}-{dayOfMonth}T{hour}:{min}:00";
            return date;
        }

        private static string FindCodeInText(string text)
        {
            Regex regex;
            Match match;

            // Search for the class code in the text
            // should match something like "420-B1S-R0"
            regex = new Regex("[0-9a-zA-Z]{3}[-_—][0-9a-zA-Z]{3}[-_—]R[O00]");
            match = regex.Match(text);
            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }

        private static string FindLocationInText(string text)
        {
            Regex regex;
            Match match;

            // Search for the local in the text
            // should match something like "B-125"
            regex = new Regex("[a-zA-Z][-_—][0-9]{3}");
            match = regex.Match(text);
            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }

        private static string doOCR(ref Bitmap img, Rectangle zone)
        {
            // check to make sure the zone isn't out of the image.
            if(zone.X + zone.Width > img.Width || zone.Y + zone.Height > img.Height)
            {
                throw new ArgumentOutOfRangeException($"Zone: {zone} was out of bound for image: {img.Width}x{img.Height}");
            }

            // create a new image wich is the one that will be scaned
            Bitmap imgToScan = img.Clone(zone, img.PixelFormat);

            // init ocr
            TesseractEngine engine = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default);

            // do some magic
            Page page = engine.Process(imgToScan, PageSegMode.Auto);

            // get the text
            string result = page.GetText();
            return result;
        }

        private static List<int> FindLineInColumn(ref Bitmap img, int col, Color color)
        {
            List<int> pixels = new List<int>();
            for (int i = 0; i < img.Height; i++)
            {
                var px = img.GetPixel(col, i);
                if (px.Equals(color))
                {
                    i += lineWidth;
                    pixels.Add(i);
                }
            }
            return pixels;
        }

        private static List<int> FindLineInColumn(ref Bitmap img, int col, Color[] colors)
        {
            List<int> pixels = new List<int>();
            for (int i = 0; i < img.Height; i++)
            {
                var px = img.GetPixel(col, i);
                foreach(Color color in colors)
                {
                    if (px.Equals(color))
                    {
                        i += lineWidth;
                        pixels.Add(i);
                    }
                }
            }
            return pixels;
        }

        private static List<int> FindLineInRow(ref Bitmap img, int row, Color color)
        {
            List<int> pixels = new List<int>();
            for (int i = 0; i < img.Width; i++)
            {
                var px = img.GetPixel(i, row);
                if (px.Equals(color))
                {
                    i += lineWidth;
                    pixels.Add(i);
                }
            }
            return pixels;
        }

        private static List<int> FindLineInRow(ref Bitmap img, int row, Color[] colors)
        {
            List<int> pixels = new List<int>();
            for (int i = 0; i < img.Width; i++)
            {
                var px = img.GetPixel(i, row);
                foreach(Color color in colors)
                {
                    if (px.Equals(color))
                    {
                        i += lineWidth;
                        pixels.Add(i);
                    }
                }
            }
            return pixels;
        }
    }
}
