using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Uaeglp.ViewModels.KnowledgeHub
{
	public class KnowledgeHubView
	{
        public string idEncrypted;

        public int ID { get; set; }

        public string ArticleTitleEn { get; set; }

        public string ArticleTitleAr { get; set; }

        public string AuthorTitleEn { get; set; }

        public string AuthorTitleAr { get; set; }

        public string IDEncyrpted { get; set; }

        public string Category { get; set; }

        public Guid? ImageID { get; set; }

        public int CategoryID { get; set; }

        [AllowHtml]
        public string HTMLContent { get; set; }

        [AllowHtml]
        public string HTMLContentEN { get; set; }

        [AllowHtml]
        public string HTMLContentAr { get; set; }

        public string ArticleTitle { get; set; }

        public string AuthorTitle { get; set; }

        public FileViewModel Image { get; set; }

        public string KnowledgeHubTitle { get; set; }
    }
}
