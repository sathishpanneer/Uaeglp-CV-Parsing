using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class FileView
    {

        public int Id { get; set; }

        public Guid IdGuid { get; set; }

        public Guid? CorrelationId { get; set; }
    
        public string Name { get; set; }

        public string ExtraParams { get; set; }

        public string MimeType { get; set; }

        public string ProviderName { get; set; }

        public decimal SizeMb { get; set; }

        public byte[] FileBytes { get; set; }

        public DocumentType DocumentType { get; set; }

        public string DocumentURL => ConstantUrlPath.DocumentDownloadPath + Id;

        public bool IsSuccess { get; set; }

        public string ResponseMessage { get; set; }
    }
}
