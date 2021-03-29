using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class TrackApplication
    {
        
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public LookupItemView ApplicationStatus { get; set; }
        //public string StatusAr { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime? EndDate { get; set; }
        public int BatchNumber { get; set; }
        public int BatchID { get; set; }
        public int ActivityID { get; set; }
        public int? CategoryID { get; set; }
        public int ApplicationTypeID { get; set; }
        public int InitiativeTypeItemID { get; set; }
        public DateTime Created { get; set; }
    }
}
