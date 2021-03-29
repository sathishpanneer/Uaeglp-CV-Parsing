using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class MessagingResponse : BaseResponse, IMessagingResponse
    {

        public RoomViewModel RoomMessageView { get; set; }

        public MessageViewModel MessageView { get; set; }
        public List<RoomViewModel> SearchMessageViews { get; set; }
        public List<RecipientUserView> RecipientUserView { get; set; }

        public RoomView Room { get; set; }
        public IList<RoomView> Rooms { get; set; }
        public List<MessagesRecommendationView> UserMessagesRecommendationView { get; set; }
        public int NotificationCount { get; set; }

        private MessagingResponse(bool success, string message, RoomViewModel roomMessageView) : base(success, message)
        {
            RoomMessageView = roomMessageView;
        }
        private MessagingResponse(bool success, string message, MessageViewModel messageView) : base(success, message)
        {
            MessageView = messageView;
        }
        private MessagingResponse(bool success, string message, List<RoomViewModel> searchMessageViews) : base(success, message)
        {
            SearchMessageViews = searchMessageViews;
        }
        private MessagingResponse(bool success, string message, List<RecipientUserView> recipientUserView) : base(success, message)
        {
            RecipientUserView = recipientUserView;
        }

        private MessagingResponse(bool success, string message, RoomView room) : base(success, message)
        {
            Room = room;
        }
        private MessagingResponse(bool success, string message, IList<RoomView> rooms) : base(success, message)
        {
            Rooms = rooms;
        }
        private MessagingResponse(bool success, string message, List<MessagesRecommendationView> userMessagesRecommendationView) : base(success, message)
        {
            UserMessagesRecommendationView = userMessagesRecommendationView;
        }
        private MessagingResponse(bool success, string message, int notifyCount) : base(success, message)
        {
            NotificationCount = notifyCount;
        }
        public MessagingResponse(int notifyCount) : this(true, string.Empty, notifyCount)
        { }
        public MessagingResponse(IList<RoomView> rooms) : this(true, string.Empty, rooms)
        { }
        public MessagingResponse(List<MessagesRecommendationView> userMessagesRecommendationView) : this(true, string.Empty, userMessagesRecommendationView)
        { }
        public MessagingResponse(RoomView room) : this(true, string.Empty, room)
        { }
        public MessagingResponse(RoomViewModel roomMessageView) : this(true, ClientMessageConstant.Success, roomMessageView)
        { }

        public MessagingResponse(MessageViewModel messageView) : this(true, ClientMessageConstant.Success, messageView)
        { }
        public MessagingResponse(List<RoomViewModel> searchMessageViews) : this(true, ClientMessageConstant.Success, searchMessageViews)
        { }
        public MessagingResponse(List<RecipientUserView> recipientUserView) : this(true, ClientMessageConstant.Success, recipientUserView)
        { }
        public MessagingResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }
        public MessagingResponse(Exception e) : base(e)
        { }

        public MessagingResponse() : base()
        { }

    }
}
