using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using System;
using Uaeglp.MongoModels;

namespace Uaeglp.Contracts
{
    public interface IMessagingService
    {
        Task<IMessagingResponse> GetRoomAsync(string roomId);
        Task<IMessagingResponse> GetRoomMessageAsync(string roomId);
        Task<IMessagingResponse> GetRoomListAsync(int userId);
        Task<IMessagingResponse> AddRoomAsync(AddRoomView view);
        Task<IMessagingResponse> DeleteRoomAsync(string roomId);
        Task<IMessagingResponse> AddRoomMemberAsync(string roomId, int userId);
        Task<IMessagingResponse> DeleteRoomMemberAsync(string roomId, int userId);
        Task<IMessagingResponse> AddRoomMessageAsync(MessageAddView message);
        Task<IMessagingResponse> DeleteRoomMessageAsync(string roomId, string messageId);
        Task<IMessagingResponse> GetSearchMessage(string searchText, int userId);
        Task<IMessagingResponse> GetRecepientName(string searchName);
        Task<IMessagingResponse> CreateRoomAndMessageAsync(RoomAndMessageCreateView view);
        Task<IMessagingResponse> SetReadMessageAsync(string roomId, string messageId,int userId, bool isread);
        Task<IMessagingResponse> GetNotificationCount(int userId);
        Task<IMessagingResponse> UpdateRoomAsync(UpdateRommView view);
    }
}
