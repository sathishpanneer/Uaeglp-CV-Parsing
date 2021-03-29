using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class CriteriaView
    {

        public int Id { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Modified { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public int Points { get; set; }
        public string Description { get; set; }
        public bool RequiresApproval { get; set; }
        [JsonIgnore]
        public bool IsVisible { get; set; }
        [JsonIgnore]
        public bool PointEdited { get; set; }
        [JsonIgnore]
        public DateTime PointEditDate { get; set; }
        [JsonIgnore]
        public int PointEditDifference { get; set; }
        public int CriteriaCategoryId { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public virtual LookupItemView CriteriaCategory { get; set; }
    }
}
