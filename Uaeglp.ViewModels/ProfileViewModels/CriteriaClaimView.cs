using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class CriteriaClaimView
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int CriteriaId { get; set; }
        public int StatusId { get; set; }
        public DateTime? RequestDate { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
        public DateTime? AdminResponseDate { get; set; }
        public string AdminComment { get; set; }
        public string Details { get; set; }
        public Guid? CorrelationId { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Modified { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public int Order { get; set; }

        public virtual CriteriaView Criteria { get; set; }
        public virtual LookupItemView StatusLookup { get; set; }
    }
}
