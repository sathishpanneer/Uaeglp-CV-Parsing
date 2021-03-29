using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class UserRecommendationResponse : BaseResponse, IUserRecommendationResponse
    {
        public UserRecommendationModelView UserRecommendation { get; set; }
        public List<UserRecommendationModelView> SendRecommendations { get; set; }
        public  UserRecommendationDetails ReceiveAllRecommendations { get; set; }
        public bool DeleteRecommendation { get; set; }
        public NotificationGenericObjectView NotificationGenericObject { get; set; }


        private UserRecommendationResponse(bool success, string message, UserRecommendationModelView userRecommendation) : base(success, message)
        {
            UserRecommendation = userRecommendation;
        }

        private UserRecommendationResponse(bool success, string message, List<UserRecommendationModelView> sendRecommendations) : base(success, message)
        {
            SendRecommendations = sendRecommendations;
        }
        private UserRecommendationResponse(bool success, string message, UserRecommendationDetails receiveAllRecommendations) : base(success, message)
        {
            ReceiveAllRecommendations = receiveAllRecommendations;
        }
        private UserRecommendationResponse(bool success, string message, bool deleteRecommendation) : base(success, message)
        {
            DeleteRecommendation = deleteRecommendation;
        }
        private UserRecommendationResponse(bool success, string message, NotificationGenericObjectView view) : base(success, message)
        {
            NotificationGenericObject = view;
        }
        public UserRecommendationResponse(NotificationGenericObjectView view) : this(true, string.Empty, view)
        { }
        public UserRecommendationResponse(UserRecommendationModelView UserRecommend) : this(true, ClientMessageConstant.Success, UserRecommend)
        { }

        public UserRecommendationResponse(List<UserRecommendationModelView> sendRecommendations) : this(true, ClientMessageConstant.Success, sendRecommendations)
        { }
        public UserRecommendationResponse(UserRecommendationDetails ReceiveAllRecommendations) : this(true, ClientMessageConstant.Success, ReceiveAllRecommendations)
        { }
        public UserRecommendationResponse(bool deleteRecommendation) : this(true, ClientMessageConstant.Success, deleteRecommendation)
        { }

        public UserRecommendationResponse(Exception e) : base(e)
        { }

        public UserRecommendationResponse() : base()
        { }

    }
}
