using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contract.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IUserRecommendationResponse : IBaseResponse
    {
        UserRecommendationModelView UserRecommendation { get; set; }
        List<UserRecommendationModelView> SendRecommendations { get; set; }
        UserRecommendationDetails ReceiveAllRecommendations { get; set; }
         bool DeleteRecommendation { get; set; }
        NotificationGenericObjectView NotificationGenericObject { get; set; }
    }
}
