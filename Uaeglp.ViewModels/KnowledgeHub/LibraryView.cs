using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.KnowledgeHub
{
    public class LibraryView
    {
        public List<FolderView> Folders { get; set; }
        public List<EsourceView> esourceViews { get; set; }
    }
    public class FolderView
    {

        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
}
