using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Event
{
    public class attendee
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string ImageURL { get; set; }
        public int ProfileId { get; set; }
    }
    public class TaggedProfileView
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string ImageURL { get; set; }
        public int ProfileId { get; set; }
        public int EventCount { get; set; }
    }
}
