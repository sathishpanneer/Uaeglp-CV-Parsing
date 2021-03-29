using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Event;

namespace Uaeglp.ViewModels
{
    public class QRScanView
    {
        public int UserId { get; set; }
        public int? ProfileImageId { get; set; }
        public string UserNameEN { get; set; }
        public string UserNameAR { get; set; }
        public string DesignationEN { get; set; }
        public string DesignationAR { get; set; }
        public string EventNameEN { get; set; }
        public string EventNameAR { get; set; }
        public MapAutocompleteView Location { get; set; }
        public string LocationTextEN { get; set; }
        public string LocationTextAR { get; set; }
        public string ProfileImageURL
        {
            get
            {
                if (ProfileImageId != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + ProfileImageId;
                }

                return null;
            }
        }
        public int? MeetupId { get; set; }
        public int? EventId { get; set; }
    }
}
