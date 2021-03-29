using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class CriteriaClaimRequestView
    {
        public int ProfileId { get; set; }
        public int CriteriaId { get; set; }
        public string Label { get; set; }
        public Guid CorrelationId { get; set; }
        public List<IFormFile> AttachmentFile { get; set; }
    }
}
