using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class ReminderViewModel
    {
		public int ID { get; set; }
		public int UserID { get; set; }
		public int ActivityId { get; set; }
		public int ApplicationId { get; set; }
		public DateTime? RegistrationEndDate { get; set; }
		public DateTime? ReminderSendDate { get; set; }
		public bool isReminderSent { get; set; }
	}
}
