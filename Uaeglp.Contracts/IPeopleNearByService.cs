using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts
{
    public interface IPeopleNearByService
    {
        Task<IPeopleNearByResponse> AddUpdateUserLocation(UserLocationModel view);
        Task<IPeopleNearByResponse> GetUserLocation(int userId, string latitude, string longitude);
        Task<IPeopleNearByResponse> HideUserLocation(int userId, bool isHideLocation);
    }
}
