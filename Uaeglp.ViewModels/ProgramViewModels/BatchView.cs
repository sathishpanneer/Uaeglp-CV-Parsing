using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class BatchView
    {

        public int Id { get; set; }
        public int ProgrammeId { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        public DateTime DateRegFrom { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateRegTo { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsClosed { get; set; }
        public int QuestionGroupId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int? AssessmentMatrixId { get; set; }
        public DateTime? AssessmentStartDate { get; set; }
        public DateTime? AssessmentEndDate { get; set; }
        public DateTime? VideoAssessmentStartDate { get; set; }
        public DateTime? VideoAssessmentEndDate { get; set; }
        public int? VideoAssessmentId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNameEN { get; set; }
        public string ContactNumber { get; set; }
        public string ContactTitle { get; set; }
        public decimal? BestAssessmentScore { get; set; }
        public decimal? AverageAssessmentScore { get; set; }
        public string CostAR { get; set; }
        public string CostEN { get; set; }
        public string LearningOutcomesAR { get; set; }
        public string LearningOutcomesEN { get; set; }
        public string SelectionCriteriaAR { get; set; }
        public string SelectionCriteriaEN { get; set; }
        public string ContactNameAR { get; set; }

        public string ReferenceNumber { get; set; }
        public string DescriptionAR { get; set; }
        public string DescriptionEN { get; set; }

        public int? MobImageID { get; set; }
        public int? ImageID { get; set; }

        public string MobileBackgroundImageUrl
        {
            get
            {
                if (MobImageID != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + MobImageID;
                }

                return null;
            }
        }
        //public virtual AssessmentToolMatrix AssessmentMatrix { get; set; }
        //public virtual Programme Programme { get; set; }
        //public virtual QuestionGroup QuestionGroup { get; set; }
        //public virtual VideoAssessment VideoAssessment { get; set; }
        //public virtual ICollection<Application> Applications { get; set; }
        //public virtual ICollection<Assignment> Assignments { get; set; }
        //public virtual ICollection<BatchAssessmentTool> BatchAssessmentTools { get; set; }
        //public virtual ICollection<BatchInitiative> BatchInitiatives { get; set; }
        //public virtual ICollection<Event> Events { get; set; }
        //public virtual ICollection<FolderBatch> FolderBatches { get; set; }
        //public virtual ICollection<Glppermission> Glppermissions { get; set; }
        //public virtual ICollection<ProfileBatchAssessment> ProfileBatchAssessments { get; set; }
    }
}
