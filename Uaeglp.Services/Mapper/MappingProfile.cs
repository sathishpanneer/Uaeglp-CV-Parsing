using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Models;
using Uaeglp.Models.ProfileModels;
using Uaeglp.MongoModels;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ActivityViewModels;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.ProgramViewModels;



namespace Uaeglp.Services.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {

            CreateMap<UserDeviceInfoViewModel, UserDeviceInfo>().ReverseMap();
            CreateMap<Country, CountryView>().ReverseMap();
            CreateMap<LookupItem, LookupItemView>().ReverseMap();
            CreateMap<ProfileEducationFieldOfStudy, ProfileEducationFieldOfStudyView>().ReverseMap();
            CreateMap<Country, CountryView>().ReverseMap();
            CreateMap<GlpOrganization, OrganizationView>().ReverseMap();
            CreateMap<ProfileSkill, ProfileSkillView>().ReverseMap();
            CreateMap<ProfileWorkExperience, ProfileWorkExperienceView>().ReverseMap();
            CreateMap<ProfileWorkExperienceJobTitle, ProfileWorkExperienceJobTitleView>().ReverseMap();
            CreateMap<WorkField, WorkFieldView>().ReverseMap();
            CreateMap<File, FileView>().ReverseMap();
            CreateMap<Profile, PublicProfileView>().ReverseMap();
            CreateMap<Profile, MyProfileView>().ReverseMap();
            CreateMap<PublicProfileView, MyProfileView>().ReverseMap();
            CreateMap<ProfileSkillEndorsement, EndorsementView>().ReverseMap();
            CreateMap<Industry, IndustryView>().ReverseMap();
            CreateMap<AssessmentTool, AssessmentToolView>().ReverseMap();
            CreateMap<Batch, BatchView>().ReverseMap();
            CreateMap<InitiativeProfile, InitiativeProfileViewModel>()
                .ReverseMap();
            CreateMap<QuestionAnswer, QuestionAnswerViewModel>()
                .ForMember(k => k.AnswerFile, m => m.MapFrom(c => c.AnswerFile))
                //.ForMember(k => k.Questionansweroptions, m => m.MapFrom(c => c.Questionansweroptions))
                .ReverseMap();

            CreateMap<ApplicationSetting, ApplicationSettingViewModel>().ReverseMap();
            CreateMap<Option, OptionViewModel>().ReverseMap();
            CreateMap<QuestionGroup, QuestionGroupView>().ReverseMap();

            CreateMap<Question, QuestionViewModel>()
                .ForMember(k => k.QuestionTypeItem, m => m.MapFrom(c => c.QuestionTypeItem))
                .ForMember(k => k.Options, m => m.MapFrom(c => c.Options))
                .ReverseMap();

            CreateMap<Programme, ProgramView>()
                .ForMember(k => k.ProgrammeTypeItem, m => m.MapFrom(c => c.ProgrammeTypeItem))
                .ForMember(k=>k.Batches, m=>m.MapFrom(c=>c.Batches))
                .ReverseMap();
            CreateMap<Badge, BadgeView>().ReverseMap();

            CreateMap<Application, ApplicationView>()
                .ForMember(k => k.AssessmentItem, m => m.MapFrom(c => c.AssessmentItem))
                .ForMember(k => k.Batch, m => m.MapFrom(c => c.Batch))
                .ForMember(k => k.ReviewStatusItem, m => m.MapFrom(c => c.ReviewStatusItem))
                .ForMember(k => k.SecurityItem, m => m.MapFrom(c => c.SecurityItem))
                .ForMember(k => k.StatusItem, m => m.MapFrom(c => c.StatusItem))
                .ForMember(k => k.VideoAssessmentStatus, m => m.MapFrom(c => c.VideoAssessmentStatus))
                .ReverseMap();

            CreateMap<CriteriaClaim, CriteriaClaimView>()
                .ForMember(k => k.Criteria, m => m.MapFrom(c => c.Criteria))
                .ForMember(k => k.StatusLookup, m => m.MapFrom(c => c.Status))
                .ReverseMap();
            CreateMap<Criteria, CriteriaView>()
                .ForMember(k => k.CriteriaCategory, m => m.MapFrom(c => c.CriteriaCategory))
                .ReverseMap();


            CreateMap<ProfileEducation, ProfileEducationView>()
                .ForMember(k => k.Organization, m => m.MapFrom(c => c.Organization))
                .ForMember(k => k.FieldOfStudy, m => m.MapFrom(c => c.FieldOfStudyNavigation))
                .ForMember(k => k.DegreeItem, m => m.MapFrom(c => c.DegreeItem))
                .ForMember(k => k.Country, m => m.MapFrom(c => c.Country))
                .ForMember(k => k.EmirateItem, m => m.MapFrom(c => c.EmirateItem))
                .ForMember(k => k.IsStudied, m => m.MapFrom(c => c.Finshed))
                .ReverseMap();

            CreateMap<ProfileWorkExperience, ProfileWorkExperienceView>()
                .ForMember(k => k.Organization, m => m.MapFrom(c => c.Organization))
                .ForMember(k => k.Country, m => m.MapFrom(c => c.Country))
                .ForMember(k => k.EmirateItem, m => m.MapFrom(c => c.EmirateItem))
                .ForMember(k => k.ExperienceJobTitleView, m => m.MapFrom(c => c.Title))
                .ForMember(k => k.LineManagerTitleView, m => m.MapFrom(c => c.LineManagerTitle))
                .ForMember(k => k.FieldOfWork, m => m.MapFrom(c => c.FieldOfwork))
                .ReverseMap();

            CreateMap<ProfileTraining, ProfileTrainingView>()
                .ForMember(k => k.Organization, m => m.MapFrom(c => c.Organization)).ReverseMap();
            CreateMap<ProfileLanguage, LanguageItemView>()
                .ForMember(k => k.LanguageItem, m => m.MapFrom(c => c.LookupLanguage))
                .ForMember(k => k.ProficiencyItem, m => m.MapFrom(c => c.LookupProficiency)).ReverseMap();
            CreateMap<ProfileMembership, ProfileMembershipView>()
                .ForMember(k => k.Organization, m => m.MapFrom(c => c.Organization)).ReverseMap();
            CreateMap<ProfileLearningPreference, ProfileLearningPreferenceView>()
                .ForMember(k => k.LearningPreference, y => y.MapFrom(m => m.LearningPreferenceItem)).ReverseMap();
            CreateMap<ProfileAchievement, ProfileAchievementView>()
                .ForMember(k => k.AwardItem, y => y.MapFrom(m => m.AwardItem))
                .ForMember(k => k.ImpactItem, y => y.MapFrom(m => m.ImpactItem))
                .ForMember(k => k.MedalItem, y => y.MapFrom(m => m.MedalItem))
                .ForMember(k => k.ReachedItem, y => y.MapFrom(m => m.ReachedItem))
                .ForMember(k => k.VerbItem, y => y.MapFrom(m => m.VerbItem))
                .ForMember(k => k.Organization, y => y.MapFrom(m => m.Orgnization)).ReverseMap();


            CreateMap<InitiativeCategory, InitiativeCategoryViewModel>()
                .ForMember(k => k.Activities, y => y.MapFrom(m => m.Initiatives))
                .ReverseMap();
            CreateMap<Initiative, InitiativeViewModel>()
                .ForMember(k => k.InitiativeTypeItem, y => y.MapFrom(m => m.InitiativeTypeItem))
                .ReverseMap();

            // Mongo Models Mapping Section


            CreateMap<Post, BaseProfileView>().ReverseMap();
            CreateMap<Post, Post>().ReverseMap();

            CreateMap<Post, PostView>()
                .ForMember(k => k.Poll, y => y.MapFrom(m => m.PollPost))
                .ForMember(k => k.Survey, y => y.MapFrom(m => m.SurveyPost))
                .ForMember(k => k.TypeID, y => y.MapFrom(m => m.TypeID))
                .ReverseMap();


            CreateMap<Post, AllPostView>()
                .ForMember(k => k.Poll, y => y.MapFrom(m => m.PollPost))
                .ForMember(k => k.Survey, y => y.MapFrom(m => m.SurveyPost))
                .ForMember(k => k.TypeID, y => y.MapFrom(m => m.TypeID))
                .ReverseMap();


            CreateMap<Notification, NotificationView>().ReverseMap();
            CreateMap<PollAnswer, PollAnswerView>().ReverseMap();
            CreateMap<PollPost, PollView>().ReverseMap();
            CreateMap<MongoModels.Survey, SurveyView>().ReverseMap();
            CreateMap<Comment, CommentView>().ReverseMap();
            CreateMap<Viewer, ViewerView>().ReverseMap();
            CreateMap<Report, ReportView>().ReverseMap();
            CreateMap<MongoModels.User, UserView>().ReverseMap();
            CreateMap<Message, MessageView>().ReverseMap();
            CreateMap<UnreadMessage, UnreadMessageView>().ReverseMap();
            CreateMap<Room, RoomView>().ReverseMap();
            CreateMap<NotificationGenericObject, NotificationGenericObjectView>()
                .ForMember(k => k.NotificationsList, y => y.MapFrom(m => m.Notifications))
                .ReverseMap();
            CreateMap<RecommendationFitDetails, RecommendationFitDetailsView>().ReverseMap();
            CreateMap<RecommendLeadr, RecommendLeaderView>().ReverseMap();
            CreateMap<RecommendLeadr, RecommendSubmissionView>().ReverseMap();
            CreateMap<RecommendationFitDetails, RecommendFitView>().ReverseMap();
            CreateMap<RecommandationOther, RecommendOthersView>().ReverseMap();
            CreateMap<RecommendationCallback, RecommendationCallbackView>().ReverseMap();
            CreateMap<UserRecommendation, UserRecommendationModelView>().ReverseMap();
            CreateMap<UserRecommendation, UserRecommendView>().ReverseMap();
            CreateMap<ReportProblem, ReportProblemModelView>().ReverseMap();
            CreateMap<UserLocation, UserLocationModelView>().ReverseMap();
            CreateMap<CustomNotification, UserCustomNotificationView>().ReverseMap();
            CreateMap<Reminder, ReminderViewModel>().ReverseMap();

        }
    }
}
