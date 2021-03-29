using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Event;

namespace Uaeglp.Models
{
    public class EventNew
    {

        public int Id { get; set; }
        public int? BatchId { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string LocationEn { get; set; }
        public string LocationAr { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string MapText { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public List<attendee> attendees { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsPublic { get; set; }
        public bool IsMyEvent { get; set; }

    }
}
