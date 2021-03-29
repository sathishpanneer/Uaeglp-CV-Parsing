using System;
using System.Collections.Generic;

namespace Uaeglp.ViewModels
{
    public class ProfileScoreView
    {
        public ProfileScoreView()
        {
            this.Questions = new List<ScoringModel>();
            this.Subscales = new List<ScoringModel>();
            this.Scales = new List<ScoringModel>();
            this.Factors = new List<ScoringModel>();
            this.Tool = new ScoringModel();
        }

        public string AssessmentToolName { get; set; }

        public List<ScoringModel> Questions { get; set; }

        public List<ScoringModel> Subscales { get; set; }

        public List<ScoringModel> Scales { get; set; }

        public List<ScoringModel> Factors { get; set; }

        public ScoringModel Tool { get; set; }
    }

    public class ScoringModel
    {
        public Decimal Score;
        public string Name;
    }
}
