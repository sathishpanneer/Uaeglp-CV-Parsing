using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Event;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Services.Communication
{
    public class QRScanResponse : BaseResponse, IQRScanResponse
    {
        public QRScanView UserScanView { get; set; }
        public int EventDesicion { get; set; }
        public attendee UserView { get; set; }
        public List<TaggedProfileView> TaggedProfiles { get; set; }
        public List<AdminRegisterEventView> Events { get; set; }
        private QRScanResponse(bool success, string message, List<AdminRegisterEventView> view) : base(success, message)
        {
            Events = view;
        }
        private QRScanResponse(bool success, string message, QRScanView meetup) : base(success, message)
        {
            UserScanView = meetup;
        }
        private QRScanResponse(bool success, string message, int Desicion) : base(success, message)
        {
            EventDesicion = Desicion;
        }
        private QRScanResponse(bool success, string message, attendee view) : base(success, message)
        {
            UserView = view;
        }
        private QRScanResponse(bool success, string message, List<TaggedProfileView> view) : base(success, message)
        {
            TaggedProfiles = view;
        }
        
        public QRScanResponse(QRScanView meetup) : this(true, string.Empty, meetup)
        { }
        public QRScanResponse(int Desicion) : this(true, string.Empty, Desicion)
        { }
        public QRScanResponse(attendee userView) : this(true, string.Empty, userView)
        { }
        public QRScanResponse(List<TaggedProfileView> taggedProfiles) : this(true, string.Empty, taggedProfiles)
        { }
        public QRScanResponse(List<AdminRegisterEventView> eventview) : this(true, string.Empty, eventview)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// 
        public QRScanResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        public QRScanResponse(Exception e) : base(e)
        { }

        public QRScanResponse() : base()
        { }

    }
}
