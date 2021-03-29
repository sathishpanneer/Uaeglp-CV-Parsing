using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Event;

namespace Uaeglp.ViewModels.Meetup
{
    public class MeetupAdd
    {
        
        public int? MeetupId { get; set; }
        public int userId { get; set; }
        public string Title { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public MapAutocompleteView MapAutocompleteView { get; set; }

        public string Descriotion { get; set; }

        public DateTime Date { get; set; }

        public int GroupID { get; set; }
        public IFormFile MeetupPicture { get; set; }
    }
}
