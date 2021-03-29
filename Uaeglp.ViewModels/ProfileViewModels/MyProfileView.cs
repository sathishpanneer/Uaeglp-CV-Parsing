using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uaeglp.Utilities;


namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class MyProfileView
    {

        public int Id { get; set; }
        public string FirstNameAR { get; set; }
        public string SecondNameAR { get; set; }
        public string ThirdNameAR { get; set; }
        public string LastNameAR { get; set; }

        public string FirstNameEN { get; set; }
        public string SecondNameEN { get; set; }
        public string ThirdNameEN { get; set; }
        public string LastNameEN { get; set; }

        public string FullNameEN => FirstNameEN?.Trim() + " " + LastNameEN?.Trim();
        public string FullNameAR => FirstNameAR?.Trim() + " " + LastNameAR?.Trim();

        public int UserImageFileId { get; set; }
        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;
        public bool IsInfluencer { get; set; }
        public bool IsPublicFigure { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsAlumni { get; set; }
        public bool IsAmFollowing { get; set; }
        public bool IsFavoritePerson { get; set; }

        public string Designation { get; set; }
            //ProfileWorkExperience.OrderByDescending(k => k.DateFrom).FirstOrDefault()?.ExperienceJobTitleView?.TitleEn;

        public string DesignationAr { get; set; }
        //ProfileWorkExperience.OrderByDescending(k => k.DateFrom).FirstOrDefault()?.ExperienceJobTitleView?.TitleAr;
        public int PostCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }

        public int LPSPoint { get; set; }
        public string TotalYearsOfExperience { get; set; }
        public BioView Bio { get; set; } = new BioView();
        public List<ProfileEducationView> ProfileEducation { get; set; }
        public List<ProfileWorkExperienceView> ProfileWorkExperience { get; set; } = new List<ProfileWorkExperienceView>();
        public SkillAndInterestView SkillAndInterest { get; set; } = new SkillAndInterestView();
        public List<ProfileLearningPreferenceView> ProfileLearningPreference { get; set; } = new List<ProfileLearningPreferenceView>();
        public List<ProfileTrainingView> ProfileTraining { get; set; } = new List<ProfileTrainingView>();
        public List<ProfileAchievementView> ProfileAchievement { get; set; } = new List<ProfileAchievementView>();
        public List<ProfileMembershipView> ProfileMembership { get; set; } = new List<ProfileMembershipView>();

        public UserRecommendationModelView Recommendation { get; set; }

        public decimal CompletePercentage { get; set; }
        public int[] Batch { get; set; }

        public List<ProfileAlumniList> ProfileAlumni { get; set; }

        public int TotalCount { get; set; }
        public ProfileCompetencyView ProfileCompetency { get; set; }
    }
}
