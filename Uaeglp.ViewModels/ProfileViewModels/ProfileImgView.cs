using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.ViewModels.ProfileViewModels
{
	public class ProfileImgView
	{
        public ProfileImgView()
        {
            this.ProgrammeswithTypeForPopOver = new List<ProgrammeCatgView>();
        }

        public string ImgeIDEncrypted { get; set; }

        public string DefaultImageSrc { get; set; }

        public int ProfileID { get; set; }

        public bool HideAlumni { get; set; }

        public string ExtraCalasses { get; set; }

        public List<ProgrammeCatgView> ProgrammeswithTypeForPopOver { get; set; }
    }
}
