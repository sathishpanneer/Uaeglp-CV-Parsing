using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IMessagingResponse: IBaseResponse
    {
        RoomViewModel RoomMessageView { get; set; }
        MessageViewModel MessageView { get; set; }
        List<RoomViewModel> SearchMessageViews { get; set; }
        List<RecipientUserView> RecipientUserView { get; set; }
        List<MessagesRecommendationView> UserMessagesRecommendationView { get; set; }
        
        RoomView Room { get; set; }
        IList<RoomView> Rooms { get; set; }
        int NotificationCount { get; set; }

    }
}
