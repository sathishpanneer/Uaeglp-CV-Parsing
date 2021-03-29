using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Services.Communication
{
    public class MeetupResponse : BaseResponse, IMeetupResponse
    {
        public GroupPagedView groupPagedView { get; set ; }
        public List<GroupView> Groupviews { get ; set ; }
        public GroupView Groupview { get ; set ; }
        public FollwGroupView FollowGroup { get ; set ; }
        public MeetupPagedView MeetuppagedView { get ; set ; }
        public MeetupView Meetup { get ; set ; }
        public bool DeleteMeetup { get ; set ; }
        public int MeetupDesicion { get; set ; }
        public List<MeetupPagedView> MeetuppagedViews { get; set; }
        public List<CommentViewModel> CommentView { get; set; }
        public string QRCodeData { get; set; }


        private MeetupResponse(bool success, string message, GroupPagedView GpView) : base(success, message)
        {
            groupPagedView = GpView;
        }

        private MeetupResponse(bool success, string message, List<GroupView> listgroup) : base(success, message)
        {
            Groupviews = listgroup;
        }

        private MeetupResponse(bool success, string message, GroupView gview) : base(success, message)
        {
            Groupview = gview;
        }

        private MeetupResponse(bool success, string message, FollwGroupView FGView) : base(success, message)
        {
            FollowGroup = FGView;
        }
        private MeetupResponse(bool success, string message, MeetupPagedView MPView) : base(success, message)
        {
            MeetuppagedView = MPView;
        }
        private MeetupResponse(bool success, string message, MeetupView meetup) : base(success, message)
        {
            Meetup = meetup;
        }
        private MeetupResponse(bool success, string message, bool delete) : base(success, message)
        {
            DeleteMeetup = delete;
        }
        private MeetupResponse(bool success, string message, int Desicion) : base(success, message)
        {
            MeetupDesicion = Desicion;
        }
        private MeetupResponse(bool success, string message, string QrCode) : base(success, message)
        {
            QRCodeData = QrCode;
        }
        private MeetupResponse(bool success, string message, List<MeetupPagedView> MPViews) : base(success, message)
        {
            MeetuppagedViews = MPViews;
        }
        private MeetupResponse(bool success, string message, List<CommentViewModel> CView) : base(success, message)
        {
            CommentView = CView;
        }
        public MeetupResponse(List<CommentViewModel> CView) : this(true, string.Empty, CView)
        { }
        public MeetupResponse(GroupPagedView GpView) : this(true, string.Empty, GpView)
        { }

        public MeetupResponse(List<GroupView> listgroup) : this(true, string.Empty, listgroup)
        { }
        public MeetupResponse(GroupView gview) : this(true, string.Empty, gview)
        { }
        public MeetupResponse(FollwGroupView FGView) : this(true, string.Empty, FGView)
        { }
        public MeetupResponse(MeetupPagedView MPView) : this(true, string.Empty, MPView)
        { }

        public MeetupResponse(List<MeetupPagedView> MPViews) : this(true, string.Empty, MPViews)
        { }
        public MeetupResponse(MeetupView meetup) : this(true, string.Empty, meetup)
        { }
        public MeetupResponse(bool delete) : this(true, string.Empty, delete)
        { }

        public MeetupResponse(int Desicion) : this(true, string.Empty, Desicion)
        { }
        public MeetupResponse(string QrCode) : this(true, string.Empty, QrCode)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// 
        public MeetupResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        public MeetupResponse(Exception e) : base(e)
        { }

        public MeetupResponse() : base()
        { }

    }
}
