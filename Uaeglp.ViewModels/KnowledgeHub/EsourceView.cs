using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.KnowledgeHub
{
    public class EsourceView
    {
        public int Id { get; set; }
 
        public string NameEn { get; set; }
  
        public string NameAr { get; set; }
 
        public string DescriptionEn { get; set; }
    
        public string DescriptionAr { get; set; }

        public string FileName { get; set; }
    
        public Guid? CorrelationId { get; set; }
        public string CorrelationURL
        {
            get
            {
                if (CorrelationId != null)
                {
                    return ConstantUrlPath.CorrelationFilePath + CorrelationId;
                }

                return null;
            }
        }
        public string EsourceUrl { get; set; }
        
    }
}
