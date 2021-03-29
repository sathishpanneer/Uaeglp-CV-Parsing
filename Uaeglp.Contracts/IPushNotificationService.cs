using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
	public interface IPushNotificationService
    {
        Task<bool> SendPushNotificationAsync(NotificationView notification,string deviceId);

        Task<bool> SendReminderPushNotificationAsync(string content, string deviceId, int daysLeft, DateTime? registrationEndDate, int applicationId);
        Task<bool> SendRecommendPushNotificationAsync(NotificationView notification, string content, string deviceId);
        Task<bool> SendAdminPushNotificationAsync(NotificationView notification, string content, string deviceId);
    }
}
