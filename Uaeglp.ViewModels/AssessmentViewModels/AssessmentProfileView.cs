using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentProfileView
	{
        public AssessmentProfileView()
        {
            this.ProfileImgView = new ProfileImgView();
        }

        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public string ImageIDEncrypted { get; set; }

        public string AppIDEncrypted { get; set; }

        public bool AppIsShortlisted { get; set; }

        public int OrgnizationID { get; set; }

        public int? GenderID { get; set; }

        public string Name { get; set; }

        public string JobTitle { get; set; }

        public string OrgnizationName { get; set; }

        public string OrgnizationType { get; set; }

        public Decimal OverAllWeigthedScore { get; set; }

        public Decimal OverAllWeigthedScoreOutOf100 { get; set; }

        public int Rank { get; set; }

        public int? OrgEmirateID { get; set; }

        public string OrgEmirate { get; set; }

        public string HighesttDgree { get; set; }

        public string TotalYearsOfExpe { get; set; }

        public ProfileImgView ProfileImgView { get; set; }

        public int? OrgnizationTypeID { get; set; }

        public bool IsEnrolledInAnyOtherProgramme { get; set; }
    }
}
