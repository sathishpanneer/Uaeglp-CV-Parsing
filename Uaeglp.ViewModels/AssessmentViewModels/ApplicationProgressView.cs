using System.Collections.Generic;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ApplicationProgressView
	{
        public ApplicationProgressView()
        {
            this.Links = new Dictionary<string, string>();
        }

        public int ID { get; set; }

        public int ApplicationID { get; set; }

        public int ApplicationSectionItemID { get; set; }

        public string ApplicationSection { get; set; }

        public int ApplicationSectionStatusItemID { get; set; }

        public string ApplicationSectionStatus { get; set; }

        public string ReadActionURL { get; set; }

        public string ApplicationName { get; set; }

        public Dictionary<string, string> Links { get; set; }
    }
}
