using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Event
{
    public class EventViewNew
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsPublic { get; set; }
        public bool IsMyEvent { get; set; }
        public int cost { get; set; }
        public int ID { get; set; }

        public int? BatchID { get; set; }

        public EnglishArabicViewEvents Text { get; set; }

        public EnglishArabicViewEvents Description { get; set; }

        public EnglishArabicViewEvents Location { get; set; }

        public string JoinedDate { get; set; }

        public MapAutocompleteView MapAutocompleteView { get; set; }

        public int DecisionID { get; set; }

        public List<attendee> attendees { get; set; }

        public EventViewNew()
        {
            this.MapAutocompleteView = new MapAutocompleteView();
        }

        public bool isReminderSet { get; set; }
    }
}
