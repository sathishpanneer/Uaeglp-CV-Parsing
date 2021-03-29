using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Event;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Contracts.Communication
{
    public interface IQRScanResponse : IBaseResponse
    {
        QRScanView UserScanView { get; set; }
        int EventDesicion { get; set; }
         attendee UserView { get; set; }
         List<TaggedProfileView> TaggedProfiles { get; set; }
         List<AdminRegisterEventView> Events { get; set; }
    }
}
