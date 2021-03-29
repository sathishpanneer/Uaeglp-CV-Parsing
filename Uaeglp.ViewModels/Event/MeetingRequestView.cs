using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Event
{
	public class MeetingRequestView
	{
        public int EventID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        //public Guid MessageID { get; set; }

        //public List<string> To { get; set; }

        //public List<string> CC { get; set; }

        //public List<string> BCC { get; set; }

        //public string Subject { get; set; }

        //public string Body { get; set; }

        //public List<EmailAttachment> Attachments { get; set; }

        //public string Queue { get; set; }

        //public string Recipients
        //{
        //    get
        //    {
        //        StringBuilder stringBuilder = new StringBuilder();
        //        if (this.To.Count > 0)
        //            stringBuilder.AppendLine("To: " + string.Join(";", (IEnumerable<string>)this.To));
        //        if (this.CC.Count > 0)
        //            stringBuilder.AppendLine("CC: " + string.Join(";", (IEnumerable<string>)this.CC));
        //        if (this.BCC.Count > 0)
        //            stringBuilder.AppendLine("BCC: " + string.Join(";", (IEnumerable<string>)this.BCC));
        //        return stringBuilder.ToString();
        //    }
        //}

        //public DateTime Start { get; set; }

        //public DateTime End { get; set; }

        //public string Location { get; set; }

        //public string ExchangeId { get; set; }

        //public override string ToString()
        //{
        //    return string.Format("MessageID:{0}\r\nAttachmentsCount:{1}\r\nSubject:{2}\r\nStart:{3}\r\nEnd:{4}\r\nLocation:{5}\r\nExchangeId:{6}\r\n{7}\r\nBody:\r\n{8}\r\n", (object)this.MessageID, (object)this.Attachments.Count, (object)this.Subject, (object)this.Start.ToString("dd-MM-yyyy hh:mm tt"), (object)this.End.ToString("dd-MM-yyyy hh:mm tt"), (object)this.Location, (object)this.ExchangeId, (object)this.Recipients, (object)this.Body);
        //}

        //public MeetingRequestView()
        //{
        //    this.To = new List<string>();
        //    this.CC = new List<string>();
        //    this.BCC = new List<string>();
        //    this.Attachments = new List<EmailAttachment>();
        //}
    }

    public class EmailAttachment
    {
        public string Name { get; set; }

        public byte[] Bytes { get; set; }
    }

    public class RecipientsCollection
    {
        public HashSet<Recipient> Recipients;
        public bool IsEnglish;
        public bool IsArabic;

        public bool IsEnglishAndArabic()
        {
            return this.IsEnglish && this.IsArabic;
        }

        public RecipientsCollection()
        {
            this.Recipients = new HashSet<Recipient>();
            this.IsEnglish = this.IsArabic = false;
        }
    }

    public class Recipient : IEqualityComparer<Recipient>
    {
        public int ID { get; set; }

        public string NameEN { get; set; }

        public string NameAR { get; set; }

        public string LangKey { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public Dictionary<string, string> KeyMap { get; set; }

        public bool IsEmail { get; set; }

        public bool IsSms { get; set; }

        public Recipient()
        {
            this.KeyMap = new Dictionary<string, string>();
            this.IsEmail = this.IsSms = true;
        }

        public bool IsEnglish
        {
            get
            {
                return this.LangKey == "en";
            }
        }

        public void AddKeyToMap(string key, string value)
        {
            if (this.KeyMap.ContainsKey(key))
                this.KeyMap[key] = value;
            else
                this.KeyMap.Add(key, value);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && this.ID == (obj as Recipient).ID;
        }

        public bool Equals(Recipient x, Recipient y)
        {
            return x.Equals((object)y);
        }

        public int GetHashCode(Recipient obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
