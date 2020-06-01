using System.Collections.Generic;

namespace CalendrierScholaireToGoogle
{
    class SchoolGoogleEvent
    {
        private string summary = "";
        private string fullSummary = "";
        private string location = "";
        private string colorId = "10";
        private string startDateTime = "";
        private string startTimeZone = "";
        private string endDateTime = "";
        private string endTimeZone = "";
        private string code = "";
        private string teacher = "";
        private string group = "";

        public string Summary { get => summary; set => summary = value; }
        public string FullSummary { get => fullSummary; set => fullSummary = value; }
        public string Location { get => location; set => location = value; }
        public string ColorId { get => colorId; set => colorId = value; }
        public string StartDateTime { get => startDateTime; set => startDateTime = value; }
        public string StartTimeZone { get => startTimeZone; set => startTimeZone = value; }
        public string EndDateTime { get => endDateTime; set => endDateTime = value; }
        public string EndTimeZone { get => endTimeZone; set => endTimeZone = value; }
        public string Code { get => code; set => code = value; }
        public string Teacher { get => teacher; set => teacher = value; }
        public string Group { get => group; set => group = value; }

        public string Description
        { 
            get
            {
                string str = "";

                if (!string.IsNullOrEmpty(FullSummary))
                    str += FullSummary + "\n";
                if(!string.IsNullOrEmpty(Code))
                    str += Code + "\n";
                if (!string.IsNullOrEmpty(Teacher))
                    str += Teacher + "\n";
                if (!string.IsNullOrEmpty(Group))
                    str += Group + "\n";

                str += "Event created automatically.";
                return str;
            }
        }

        public SchoolGoogleEvent()
        {

        }

        public Dictionary<string, object> toGoogle()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("summary", this.Summary);
            data.Add("location", this.Location);
            data.Add("description", this.Description);
            data.Add("colorId", this.ColorId);

            Dictionary<string, string> start = new Dictionary<string, string>();
            start.Add("dateTime", this.StartDateTime);
            start.Add("timeZone", this.StartTimeZone);
            data.Add("start", start);

            Dictionary<string, string> end = new Dictionary<string, string>();
            end.Add("dateTime", this.endDateTime);
            end.Add("timeZone", this.EndTimeZone);
            data.Add("end", end);

            return data;
        }

        public override string ToString()
        {
            return
                $"summary: {Summary}, " +
                $"fullSummary: {FullSummary}, " +
                $"location: {Location}, " +
                $"colorId: {ColorId}, " +
                $"startDateTime: {StartDateTime}, " +
                $"startTimeZone: {StartTimeZone}, " +
                $"endDateTime: {EndDateTime}, " +
                $"endTimeZone: {EndTimeZone}, " +
                $"code: {Code}, " +
                $"teacher: {Teacher}, " +
                $"group: {Group}";
        }
    }
}
