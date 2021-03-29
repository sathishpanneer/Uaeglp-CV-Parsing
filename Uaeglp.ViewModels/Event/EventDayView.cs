using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Event
{
    public class EventDayView
    {
        public EventDayView()
        {
            this.Event = new EventView();
        }

        public int ID { get; set; }

        public int EventID { get; set; }

        public string EventTitle { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsMeetingRequestCreated { get; set; }

        public string DateText { get; set; }

        public EventView Event { get; set; }
    }
}
