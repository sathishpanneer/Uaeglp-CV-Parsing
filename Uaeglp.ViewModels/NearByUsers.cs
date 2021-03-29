using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels
{
    public class NearByUsers
    {
        public int ProfileID { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Distance { get; set; }
        public bool isHideLocation { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string FirstNameAr { get; set; }
        public string LastNameAr { get; set; }

        public string FirstNameEn { get; set; }
        public string LastNameEn { get; set; }

        public string FullNameEN => LastNameEn == "" ? FirstNameEn?.Trim() : FirstNameEn?.Trim() + " " + LastNameEn?.Trim();
        public string FullNameAR => LastNameAr == "" ? FirstNameAr?.Trim() : FirstNameAr?.Trim() + " " + LastNameAr?.Trim();
        public int UserImageFileId { get; set; }
        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;
        public string DesignationEn { get; set; }
        public string DesignationAr { get; set; }
        //public string ErrorMessage { get; set; }
    }
}
