using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.KnowledgeHub
{
    public class KnowledgeHubCategoryView
    {
        public int ID { get; set; }

        public string IDEncyrpted { get; set; }

        public string TitleEN { get; set; }
        public string TitleAR { get; set; }

        public int? LogoID { get; set; }

        //public FileViewModel Logo { get; set; }
        public string LogoURL
        {
            get
            {
                if (LogoID != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + LogoID;
                }

                return null;
            }
        }
    }
    public class KnowledgeHubCategoryCourseView
    {
        public int ID { get; set; }

        public string IDEncyrpted { get; set; }

        public string TitleEN { get; set; }
        public string TitleAR { get; set; }

        public int? LogoID { get; set; }

        //public FileViewModel Logo { get; set; }
        public string LogoURL
        {
            get
            {
                if (LogoID != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + LogoID;
                }

                return null;
            }
        }
        public List<KnowledgeHubCourseView> Courses { get; set; }
    }
}

