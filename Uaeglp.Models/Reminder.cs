using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Uaeglp.Models
{
	[Table("Reminder", Schema = "app")]
	public class Reminder
	{
		[Key]
		public int ID { get; set; }
		public int UserID { get; set; }
		public int ActivityId { get; set; }
		public int ApplicationId { get; set; }
		public string Name { get; set; }
		[Column(TypeName = "datetime")]
		public DateTime? RegistrationEndDate { get; set; }
		public DateTime? ReminderSendDate { get; set; }

		public bool isReminderSent { get; set; }

	}
}
