using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IPeopleNearByResponse : IBaseResponse
    {
        UserLocationModelView UserLocation { get; set; }
        GetUserLocationView GetUserLocation { get; set; }
    }
}
