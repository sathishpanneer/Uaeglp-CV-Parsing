using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class MessageAddView
    {
		public string RoomId { get; set; }
		public int userId { get; set; }
		public string MessageText { get; set; }
	
		public IFormFile DocumentData { get; set; }
		public IFormFile ImageData { get; set; }
	}
}
