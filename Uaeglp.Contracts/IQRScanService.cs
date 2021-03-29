using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Contracts
{
    public interface IQRScanService
    {
        Task<IQRScanResponse> ScanQRCodeAsync(int userId,string _data);
        Task<IQRScanResponse> JoinEventByQRCodeAsync(int userId, int decisionId, string _data);
        Task<IQRScanResponse> JoinAdminEventByQRCodeAsync(int eventid, string qrCode);
        Task<IQRScanResponse> JoinAdminEventByShortCodeAsync(int eventid, string shortCode);
        Task<IQRScanResponse> GetUserDataByshortCodeAsync(string shortCode);
        Task<IQRScanResponse> GetUserDataByQrCodeAsync(string qrCode);
        Task<IQRScanResponse> GetEvents();
        Task<IQRScanResponse> GetEventparticipants(int eventid);
    }
}
