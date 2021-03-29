using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Contracts
{
    public interface IMeetupService
    {
        Task<IMeetupResponse> GetGroupsByFilters(PagingView pagingView, int userId, string SearchText = null);
        Task<IMeetupResponse> GetGroups(int userId,int Take);
        Task<IMeetupResponse> CreateGroup(int userId, GroupView groupView);
        Task<IMeetupResponse> UpdateGroup(int userId, GroupView groupView);
        Task<IMeetupResponse> GetGroup(int Id, int userId);
        Task<IMeetupResponse> FollowGroup(int groupId, int userId);
        Task<IMeetupResponse> UnFollowGroup(int groupId, int userId);
        Task<IMeetupResponse> GetMeetupsByGroupId(int groupId, int userId, int skip, int limit);
        Task<IMeetupResponse> GetMeetupsProfileByGroupId(int groupId, int userId, int skip, int limit);
        Task<IMeetupResponse> UpdateMeetup(MeetupAdd View);
        Task<IMeetupResponse> CreateMeetup(MeetupAdd View);
        Task<IMeetupResponse> GetMeetup(int id, int userId);
        Task<IMeetupResponse> DeleteMeetup(int id);
        Task<IMeetupResponse> SetMeetupDesicion(int decisionId, int meetupId, int userId);
        Task<IMeetupResponse> AddMeetupCommendsAsync(MeetupCommendAddView view);
        Task<IMeetupResponse> GetMeetupCommentsAsync(int meetupId, int userId, int Skip, int Take);
        Task<IMeetupResponse> DeleteMeetupCommentAsync(int meetupid, string commentId, int userId);
        Task<IMeetupResponse> EditMeetupCommentAsync(MeetupCommendAddView view, string commentid);
        Task<IMeetupResponse> GetMeetupCommentsbyIdAsync(int meetupId, int userId, string commentid);
        Task<IMeetupResponse> GenerateMeetupQRCodeAsync(int meetupid, int userId);
    }
}