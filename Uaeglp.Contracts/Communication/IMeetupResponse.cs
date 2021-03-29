using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Contracts.Communication
{
	public interface IMeetupResponse : IBaseResponse
	{
        GroupPagedView groupPagedView { get; set; }
        List<GroupView> Groupviews { get; set; }
        GroupView Groupview { get; set; }

        FollwGroupView FollowGroup { get; set; }
    
       MeetupPagedView MeetuppagedView { get; set; }
        MeetupView Meetup { get; set; }
        bool DeleteMeetup { get; set; }
        int MeetupDesicion { get; set; }
        List<CommentViewModel> CommentView { get; set; }
        string QRCodeData { get; set; }
        //T Data { get; set; }
    }
}
