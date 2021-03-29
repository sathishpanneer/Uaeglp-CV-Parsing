using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class ProgramView
    {
        public int Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public int Order { get; set; }
        public decimal? Fees { get; set; }
        public string Duration { get; set; }
        public bool IsHidden { get; set; }
        public string ShortTitleEn { get; set; }
        public string ShortTitleAr { get; set; }
        public int? ProgrammeTypeItemId { get; set; }
        public int? ImageId { get; set; }
        public string DescriptionHtmlEn { get; set; }
        public string DescriptionHtmlAr { get; set; }
        public string SubDescriptionHtmlEn { get; set; }
        public string SubDescriptionHtmlAr { get; set; }
        public LookupItemView ProgrammeTypeItem { get; set; }
        public bool isReminderSet { get; set; }

        public string BackgroundImageUrl
        {
            get
            {
                if (ImageId != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + ImageId;
                }

                return null;
            }
        }

        [JsonIgnore]
        public List<BatchView> Batches { get; set; } = new List<BatchView>();
        [JsonIgnore]
        public List<ApplicationView> Applications { get; set; } = new List<ApplicationView>();

        public List<ApplicationReference> Application_reference { get; set; } = new List<ApplicationReference>();
        public BatchView ActiveBatch => Batches.FirstOrDefault(k => k.ProgrammeId == Id && !k.IsClosed && k.DateRegFrom <= DateTime.Now && k.DateRegTo >= DateTime.Now);

        public int? BatchId => BatchView?.Id;

        public BatchView BatchView { get; set; }

        public List<PublicProfileView> CompletedUsersList { get; set; }

        public bool HasActiveBatches => Batches.Any(k => !k.IsClosed && k.DateRegFrom <= DateTime.Now.Date && k.DateRegTo >= DateTime.Now.Date && k.Id == BatchView.Id);

        public LookupItemView ApplicationStatus
        {
            get
            {
                var batch = BatchView.Id;
                var application = Applications.Any(k => k.BatchId == batch);


                if (application)
                {
                    var proStatus = Applications.FirstOrDefault(k => k.BatchId == batch);
                    return proStatus?.StatusItem;
                }

                if (!HasActiveBatches)
                {
                    return new LookupItemView()
                    {
                        Id = 59021,
                        NameAr = " مغلق",
                        NameEn = "Closed"
                    };

                }

                var batchId = ActiveBatch?.Id ?? 0;
                var pro = Applications.FirstOrDefault(k => k.BatchId == batch);
                return pro?.StatusItem ?? new LookupItemView()
                {
                    Id = 59020,
                    NameAr = " افتح",
                    NameEn = "Open"
                };
            }
        }

        public decimal ApplicationCompletedPercentage
        {
            get
            {
                //if (!Batches.Any(k => !k.IsClosed && k.DateRegFrom <= DateTime.Now && k.DateRegTo >= DateTime.Now)) return 0;
                var pro = Applications.FirstOrDefault(k => k.BatchId == BatchView.Id);
                return pro?.CompletionPercentage ?? 0;
            }
        }




    }
    public class ApplicationReference
    {
            public int ApplicationId { get; set; }
            public string ReferenceNumber { get; set; }
    }
}
