using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.ActivityViewModels
{
    public class InitiativeViewModel
    {
        public int Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string RequirmentsEN { get; set; }
        public string RequirmentsAR { get; set; }
        public bool IsActive { get; set; }
        public Guid? FileId { get; set; }
        public string FileName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Created { get; set; }
        public Guid? ImageId { get; set; }
        public int? QuestionGroupId { get; set; }
        public decimal? Cost { get; set; }
        public string PaymentDetails { get; set; }
        public int SeatsCount { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int RemainingSeatsCount { get; set; }
        public int InitiativeTypeItemId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public string DisplayLocationTextEn { get; set; }
        public string DisplayLocationTextAr { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string MapText { get; set; }
        public string SubDescriptionEn { get; set; }
        public string SubDescriptionAr { get; set; }
        public bool IsRegistrationClosed => EndDate < DateTime.Now;
        public  LookupItemView InitiativeTypeItem { get; set; }

        public string ImageUrl => ConstantUrlPath.FileDownloadPath + MobImageID;
        public string FileUrl => ConstantUrlPath.FileDownloadPath + FileId;

       // public string FileName { get; set; }
        public LookupItemView InitiativeStatus { get; set; }

        public virtual QuestionGroupView QuestionGroup { get; set; }
        public string CostAR { get; set; }
        public string CostEN { get; set; }
        public string DidYouKnowEN { get; set; }
        public string DidYouKnowAR { get; set; }
        public string EventOrganizerNameEN { get; set; }
        public string EventOrganizerNameAR { get; set; }
        public string EventOrganizerEmail { get; set; }
        public string EventOrganizerPhoneNumber { get; set; }
        public string PaymentDetailsAR { get; set; }
        public string PaymentDetailsEN { get; set; }
        public string QRCode { get; set; }
        public int RegistrationTypeItemID { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? MobImageID { get; set; }
        public bool isReminderSet { get; set; }
    }
}
