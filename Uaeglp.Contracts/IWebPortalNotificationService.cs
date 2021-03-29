using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.ViewModels;
using Uaeglp.Models;

namespace Uaeglp.Contracts
{
    public interface IWebPortalNotificationService
    {
        Task<bool> PostLikeNotification();
        Task<bool> PostCommentNotification();
        Task<bool> PostShareNotification();
        Task<bool> ActivityAndChallengeNotification();
        Task<bool> ProgrammeNotification();
        Task<bool> BatchNotification();
        Task<bool> UserFollowingNotification();
        Task<bool> NetworkGroupNotification();
        Task<bool> AssessmentAssignedNotification();
        Task<bool> AssessmentMemberNotification();
        Task<bool> MeetupNotification();
        Task<bool> MessagingNotification();
        Task<bool> EventNotification();
        Task<bool> EngagementActiity();
        //Task<bool> WebNotitification2();
        //Task<bool> customNotification();
    }
}
