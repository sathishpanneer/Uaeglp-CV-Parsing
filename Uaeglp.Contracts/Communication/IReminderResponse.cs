using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IReminderResponse : IBaseResponse
    {
        ReminderViewModel SetReminder { get; set; }
        bool RemoveReminder { get; set; }
    }
}
