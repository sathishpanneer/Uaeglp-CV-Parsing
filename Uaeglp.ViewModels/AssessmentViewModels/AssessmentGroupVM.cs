using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class AssessmentGroupVM
    {
        public AssessmentGroupView AssessmentGroupView { get; set; }

        public ListingView<List<ProfileView>> MembersPaging { get; set; }

        public string GroupIDEncrypyted { get; set; }

        public AssessmentGroupVM()
        {
            this.AssessmentGroupView = new AssessmentGroupView();
            this.MembersPaging = new ListingView<List<ProfileView>>();
        }
    }
}
