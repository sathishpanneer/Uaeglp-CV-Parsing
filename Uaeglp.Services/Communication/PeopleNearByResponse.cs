using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class PeopleNearByResponse : BaseResponse, IPeopleNearByResponse
    {
        public UserLocationModelView UserLocation { get; set; }
        public GetUserLocationView GetUserLocation { get; set; }
        private PeopleNearByResponse(bool success, string message, UserLocationModelView userLocation) : base(success, message)
        {
            UserLocation = userLocation;
        }
        private PeopleNearByResponse(bool success, string message, GetUserLocationView nearByUsers) : base(success, message)
        {
            GetUserLocation = nearByUsers;
        }
        public PeopleNearByResponse(UserLocationModelView UserLocation) : this(true, ClientMessageConstant.Success, UserLocation)
        { }
        public PeopleNearByResponse(GetUserLocationView NearByUsers) : this(true, ClientMessageConstant.Success, NearByUsers)
        { }

        public PeopleNearByResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }
        public PeopleNearByResponse(Exception e) : base(e)
        { }

        public PeopleNearByResponse() : base()
        { }
    }
}
