using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Meetup
{
	public class AgendaView
	{
		public int ID { get; set; }

		public DateTime Day { get; set; }

		public DateTime Time { get; set; }

		public string Subject { get; set; }
	}
}
