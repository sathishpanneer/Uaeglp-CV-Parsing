using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Meetup
{
    public class MeetupCommendAddView
    {
        public int MeetupId { get; set; }
        public int userId { get; set; }
        public string CommandText { get; set; }
        public IFormFile DocumentData { get; set; }
        public IFormFile ImageData { get; set; }
    }
}
