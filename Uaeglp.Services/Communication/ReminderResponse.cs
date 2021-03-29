using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class ReminderResponse : BaseResponse, IReminderResponse
    {
        public ReminderViewModel SetReminder { get; set; }
        public bool RemoveReminder { get; set; }
        private ReminderResponse(bool success, string message, ReminderViewModel setReminder) : base(success, message)
        {
            SetReminder = setReminder;
        }

        private ReminderResponse(bool success, string message, bool removeReminder) : base(success, message)
        {
            RemoveReminder = removeReminder;
        }
        public ReminderResponse(bool removeReminder) : this(true, ClientMessageConstant.Success, removeReminder)
        { }
        public ReminderResponse(ReminderViewModel setReminder) : this(true, ClientMessageConstant.Success, setReminder)
        { }
        public ReminderResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }
        public ReminderResponse(Exception e) : base(e)
        { }

        public ReminderResponse() : base()
        { }
    }
}
