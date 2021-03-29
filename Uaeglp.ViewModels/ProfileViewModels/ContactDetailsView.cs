using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ContactDetailsView :BaseProfileView
    {
        public int UserId { get; set; }
        public UserProfessionalDetailsView UserProfessionalDetail { get; set; }
        public UserPassportView UserPassport { get; set; }
        public UserLocationView UserLocation { get; set; }
    }

    public class UserProfessionalDetailsView
    {
        public string BusinessEmail { get; set; }
        public string OfficeNumber { get; set; }
        public string LinkedInUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UserPassportView
    {
        public string EmiratesId { get; set; }
        public string UnifiedPassportNumber { get; set; }
        public string PassportNumber { get; set; }
        public LookupItemView PassportIssue { get; set; }
    }

    public class UserLocationView
    {
        public CountryView Country { get; set; }
        public LookupItemView Emirate { get; set; }
        public string Address { get; set; }
    }
}
