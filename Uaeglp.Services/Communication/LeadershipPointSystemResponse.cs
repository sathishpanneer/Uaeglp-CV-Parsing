using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class LeadershipPointSystemResponse : BaseResponse, ILeadershipPointSystemResponse
    {


        public LeadershipPointSystemView LeadershipPointSystemView { get; set; }
        public CriteriaClaimedPointsView CriteriaClaimedPointsView { get; set; }
        public List<FileView> MoreDetailsView { get; set; }


        private LeadershipPointSystemResponse(bool success, string message, LeadershipPointSystemView leadershipPointSystemView) : base(success, message)
        {
            LeadershipPointSystemView = leadershipPointSystemView;
        }

        private LeadershipPointSystemResponse(bool success, string message, CriteriaClaimedPointsView claimedPointsView) : base(success, message)
        {
            CriteriaClaimedPointsView = claimedPointsView;
        }
        private LeadershipPointSystemResponse(bool success, string message, List<FileView> fileView) : base(success, message)
        {
            MoreDetailsView = fileView;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public LeadershipPointSystemResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public LeadershipPointSystemResponse(bool state, string message, HttpStatusCode status) : base(state, message ,status)
        { }


        public LeadershipPointSystemResponse(LeadershipPointSystemView leadershipPointSystemView) : this(true, string.Empty, leadershipPointSystemView)
        { }


        public LeadershipPointSystemResponse(CriteriaClaimedPointsView claimedPointsView) : this(true, string.Empty, claimedPointsView)
        { }

        public LeadershipPointSystemResponse(List<FileView> fileView) : this(true, string.Empty, fileView)
        { }



        public LeadershipPointSystemResponse():base()
        {

        }

        public LeadershipPointSystemResponse(Exception e) : base(e)
        {

        }
    }
}
