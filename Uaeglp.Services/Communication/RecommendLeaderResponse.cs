using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;


namespace Uaeglp.Services.Communication
{
    public class RecommendLeaderResponse : BaseResponse, IRecommendLeaderResponse
    {
        public RecommendLeaderSubmissionView RecommendLeaderSubmission { get; set; }
        public RecommendationFitDetailsView RecommendationFitDetails { get; set; }
        public RecommendationCallbackView RecommendationCallback { get; set; }


        private RecommendLeaderResponse(bool success, string message, RecommendLeaderSubmissionView recommendLeaderSubmission) : base(success, message)
        {
            RecommendLeaderSubmission = recommendLeaderSubmission;
        }

        private RecommendLeaderResponse(bool success, string message, RecommendationFitDetailsView recommendationFitDetails) : base(success, message)
        {
            RecommendationFitDetails = recommendationFitDetails;
        }

        private RecommendLeaderResponse(bool success, string message, RecommendationCallbackView recommendationCallback) : base(success, message)
        {
            RecommendationCallback = recommendationCallback;
        }
        public RecommendLeaderResponse(RecommendLeaderSubmissionView RecommendLeader) : this(true, ClientMessageConstant.Success, RecommendLeader)
        { }

        public RecommendLeaderResponse(RecommendationFitDetailsView RecommendationFitDetails) : this(true, ClientMessageConstant.Success, RecommendationFitDetails)
        { }

        public RecommendLeaderResponse(RecommendationCallbackView RecommendationCallback) : this(true, ClientMessageConstant.Success, RecommendationCallback)
        { }

        public RecommendLeaderResponse(Exception e) : base(e)
        { }

        public RecommendLeaderResponse() : base()
        { }

    }
}
