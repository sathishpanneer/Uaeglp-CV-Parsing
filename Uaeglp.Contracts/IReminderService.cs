using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.ViewModels;
using Uaeglp.Models;
using Uaeglp.Contracts.Communication;

namespace Uaeglp.Contracts
{
    public interface IReminderService 
    {
        Task<IReminderResponse> SetReminder(ReminderView view);
        Task<IReminderResponse> RemoveReminder(int userId, int applicationId, int activityId);
        Task<List<Reminder>> SendReminder();
    }
}
