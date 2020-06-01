using System.Collections.Generic;

namespace SchoolScheduleLibrary
{
    public class SchoolSchedule
    {
        private List<SchoolGoogleEvent> schoolGoogleEvents = new List<SchoolGoogleEvent>();

        public int Count { get => schoolGoogleEvents.Count; }

        public SchoolSchedule()
        {

        }

        public SchoolSchedule(List<SchoolGoogleEvent> schoolGoogleEvents)
        {
            this.schoolGoogleEvents = schoolGoogleEvents;
        }

        public void AddEvent(SchoolGoogleEvent e)
        {
            schoolGoogleEvents.Add(e);
        }

        public SchoolGoogleEvent this[int i]
        {
            get => schoolGoogleEvents[i];
            set => schoolGoogleEvents[i] = value;
        }

    }
}
