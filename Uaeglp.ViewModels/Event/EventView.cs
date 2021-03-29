using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Event
{
    public class EventView
    {
        public int ID { get; set; }

        public int? BatchID { get; set; }

        public EnglishArabicView Text { get; set; }

        public EnglishArabicView Description { get; set; }

        public string DescriptionName { get; set; }

        public EnglishArabicView Location { get; set; }

        public int? StatusItemID { get; set; }

        public string LocationName { get; set; }

        public string TextName { get; set; }

        public string Longitude { get; set; }

        public string MapText { get; set; }

        public string Latitude { get; set; }

        public int DaysCount { get; set; }

        public List<EventDayView> EventDays { get; set; }

        public string JoinedDate { get; set; }

        public string EventDaysValidationErrors { get; set; }

        public MapAutocompleteView MapAutocompleteView { get; set; }

        public string EventDate { get; set; }

        public int DecisionID { get; set; }

        public EventView()
        {
            this.EventDays = new List<EventDayView>();
            this.MapAutocompleteView = new MapAutocompleteView();
        }
    }
    public class AdminRegisterEventView
    {
        public int ID { get; set; }

        

        public string TextEN { get; set; }
        public string TextAR { get; set; }

        public string DescriptionEN { get; set; }
        public string DescriptionAR { get; set; }
        

        public string LocationEN { get; set; }
        public string LocationAR { get; set; }

        

        public MapAutocompleteView MapAutocompleteView { get; set; }
        public AdminRegisterEventView()
        {
            this.MapAutocompleteView = new MapAutocompleteView();
        }
    }
}
