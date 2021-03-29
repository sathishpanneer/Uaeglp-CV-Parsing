using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using Uaeglp.Models;
using Uaeglp.Models.ProfileModels;
using Uaeglp.Models.Reports;
using Uaeglp.Utilities;

namespace Uaeglp.Repositories
{
    public partial class AppDbContext : DbContext
    {
        private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly string _appSettings;
        public AppDbContext(string appSettings)
        {
            _appSettings = appSettings;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Adconnection> Adconnections { get; set; }
        public virtual DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public virtual DbSet<Admapping> Admappings { get; set; }
        public virtual DbSet<Agendum> Agenda { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<ApplicationAchievement> ApplicationAchievements { get; set; }
        public virtual DbSet<ApplicationCycleLog> ApplicationCycleLogs { get; set; }
        public virtual DbSet<ApplicationProgress> ApplicationProgresses { get; set; }
        public virtual DbSet<ApplicationTraining> ApplicationTrainings { get; set; }
        public virtual DbSet<AssessmentBlock> AssessmentBlocks { get; set; }
        public virtual DbSet<AssessmentGroup> AssessmentGroups { get; set; }
        public virtual DbSet<AssessmentGroupMember> AssessmentGroupMembers { get; set; }
        public virtual DbSet<AssessmentNarrativeReport> AssessmentNarrativeReports { get; set; }
        public virtual DbSet<AssessmentNumber> AssessmentNumbers { get; set; }
        public virtual DbSet<AssessmentReportFeedback> AssessmentReportFeedbacks { get; set; }
        public virtual DbSet<AssessmentTool> AssessmentTools { get; set; }
        public virtual DbSet<AssessmentToolMatrix> AssessmentToolMatrices { get; set; }
        public virtual DbSet<AssignedAssessment> AssignedAssessments { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<AssignmentAnswer> AssignmentAnswers { get; set; }
        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<BadgeRequest> BadgeRequests { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<BatchAssessmentTool> BatchAssessmentTools { get; set; }
        public virtual DbSet<BatchInitiative> BatchInitiatives { get; set; }
        public virtual DbSet<BtimeZone> BtimeZones { get; set; }
        public virtual DbSet<Challenge> Challenges { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ChatMessageAttachment> ChatMessageAttachments { get; set; }
        public virtual DbSet<ChatMessageSeenBy> ChatMessageSeenBies { get; set; }
        public virtual DbSet<ChatRoom> ChatRooms { get; set; }
        public virtual DbSet<ChatRoomUser> ChatRoomUsers { get; set; }
        public virtual DbSet<ChatUnreadMessage> ChatUnreadMessages { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientClaim> ClientClaims { get; set; }
        public virtual DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }
        public virtual DbSet<ClientCustomGrantType> ClientCustomGrantTypes { get; set; }
        public virtual DbSet<ClientIdPrestriction> ClientIdPrestrictions { get; set; }
        public virtual DbSet<ClientMachine> ClientMachines { get; set; }
        public virtual DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }
        public virtual DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }
        public virtual DbSet<ClientScope> ClientScopes { get; set; }
        public virtual DbSet<ClientSecret> ClientSecrets { get; set; }
        public virtual DbSet<Competency> Competencies { get; set; }
        public virtual DbSet<CompetencyAssessmentTool> CompetencyAssessmentTools { get; set; }
        public virtual DbSet<CompetencySubscale> CompetencySubscales { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<Consent> Consents { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CriteriaClaim> CriteriaClaims { get; set; }
        public virtual DbSet<Criteria> Criteria { get; set; }
        public virtual DbSet<Criterion1> Criteria1 { get; set; }
        public virtual DbSet<DirectorySearchHistory> DirectorySearchHistories { get; set; }
        public virtual DbSet<EmailHeaderAndFooterTemplate> EmailHeaderAndFooterTemplates { get; set; }
        public virtual DbSet<Eresource> Eresources { get; set; }
        public virtual DbSet<EresourceLink> EresourceLinks { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventDay> EventDays { get; set; }
        public virtual DbSet<Factor> Factors { get; set; }
        public virtual DbSet<Faq> Faqs { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<FolderBatch> FolderBatches { get; set; }
        public virtual DbSet<ParticipationReference> ParticipationReferences { get; set; }
        public virtual DbSet<FormsHubConnection> FormsHubConnections { get; set; }
        public virtual DbSet<GlpOrganization> GlpOrganizations { get; set; }
        public virtual DbSet<GlpOrganizationItemType> GlpOrganizationItemTypes { get; set; }
        public virtual DbSet<Glpmodel> Glpmodels { get; set; }
        public virtual DbSet<Glppermission> Glppermissions { get; set; }
        public virtual DbSet<GlppermissionClientUser> GlppermissionClientUsers { get; set; }
        public virtual DbSet<GovernmentEntity> GovernmentEntities { get; set; }
        public virtual DbSet<GovernmentEntityCoordinator> GovernmentEntityCoordinators { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<ImpersonationRule> ImpersonationRules { get; set; }
        public virtual DbSet<Industry> Industries { get; set; }
        public virtual DbSet<Initiative> Initiatives { get; set; }
        public virtual DbSet<InitiativeCategory> InitiativeCategories { get; set; }
        public virtual DbSet<InitiativeProfile> InitiativeProfiles { get; set; }
        public virtual DbSet<KnowledgeHub> KnowledgeHubs { get; set; }
        public virtual DbSet<KnowledgeHubCategory> KnowledgeHubCategories { get; set; }
        public virtual DbSet<KnowledgeHubCourse> KnowledgeHubCourses { get; set; }
        public virtual DbSet<LibraryFolder> LibraryFolders { get; set; }
        public virtual DbSet<Localization> Localizations { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<LookupGroup> LookupGroups { get; set; }
        public virtual DbSet<LookupItem> LookupItems { get; set; }
        public virtual DbSet<MatrixTool> MatricesTools { get; set; }
        public virtual DbSet<Meetup> Meetups { get; set; }
        public virtual DbSet<NetworkGroup> NetworkGroups { get; set; }
        public virtual DbSet<NetworkGroupProfile> NetworkGroupProfiles { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionSet> PermissionSets { get; set; }
        public virtual DbSet<PermissionSetPermission> PermissionSetPermissions { get; set; }
        public virtual DbSet<PermissionSetUser> PermissionSetUsers { get; set; }
        public virtual DbSet<Pillar> Pillars { get; set; }
        public virtual DbSet<PillarAssessmentTool> PillarAssessmentTools { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<ProfileAchievement> ProfileAchievements { get; set; }
        public virtual DbSet<ProfileAdminComment> ProfileAdminComments { get; set; }
        public virtual DbSet<ProfileAssessmentToolScore> ProfileAssessmentToolScores { get; set; }
        public virtual DbSet<ProfileAssignedAssessment> ProfileAssignedAssessments { get; set; }
        public virtual DbSet<ProfileBatchAssessment> ProfileBatchAssessments { get; set; }
        public virtual DbSet<ProfileCompetencyScore> ProfileCompetencyScores { get; set; }
        public virtual DbSet<ProfileEducation> ProfileEducations { get; set; }
        public virtual DbSet<ProfileEducationFieldOfStudy> ProfileEducationFieldOfStudys { get; set; }
        public virtual DbSet<ProfileEvent> ProfileEvents { get; set; }
        public virtual DbSet<ProfileKnowledgeHubCourse> ProfileKnowledgeHubCourses { get; set; }
        public virtual DbSet<ProfileFactorScore> ProfileFactorScores { get; set; }
        public virtual DbSet<ProfileGroup> ProfileGroups { get; set; }
        public virtual DbSet<ProfileGroupAssessment> ProfileGroupAssessments { get; set; }
        public virtual DbSet<ProfileInterest> ProfileInterests { get; set; }
        public virtual DbSet<ProfileLearningPreference> ProfileLearningPreferences { get; set; }
        public virtual DbSet<ProfileMeetup> ProfileMeetups { get; set; }
        public virtual DbSet<ProfileMembership> ProfileMemberships { get; set; }
        public virtual DbSet<ProfileMenuItem> ProfileMenuItems { get; set; }
        public virtual DbSet<ProfilePillarScore> ProfilePillarScores { get; set; }
        public virtual DbSet<ProfileQuestionItemScore> ProfileQuestionItemScores { get; set; }
        public virtual DbSet<ProfileScaleScore> ProfileScaleScores { get; set; }
        public virtual DbSet<ProfileSkill> ProfileSkills { get; set; }
        public virtual DbSet<ProfileSkillProfile> ProfileSkillProfiles { get; set; }
        public virtual DbSet<ProfileSkillEndorsement> ProfileSkillEndorsements { get; set; }
        public virtual DbSet<ProfileSubAssessmentToolScore> ProfileSubAssessmentToolScores { get; set; }
        public virtual DbSet<ProfileSubScaleScore> ProfileSubScaleScores { get; set; }
        public virtual DbSet<ProfileTag> ProfileTags { get; set; }
        public virtual DbSet<ProfileTraining> ProfileTrainings { get; set; }
        public virtual DbSet<ProfileVideoAssessmentAnswerScore> ProfileVideoAssessmentAnswerScores { get; set; }
        public virtual DbSet<ProfileVideoAssessmentCriteriaScore> ProfileVideoAssessmentCriteriaScores { get; set; }
        public virtual DbSet<ProfileVideoAssessmentScore> ProfileVideoAssessmentScores { get; set; }
        public virtual DbSet<ProfileWorkExperience> ProfileWorkExperiences { get; set; }
        public virtual DbSet<Programme> Programmes { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<ProviderAttribute> ProviderAttributes { get; set; }
        public virtual DbSet<PublicHoliday> PublicHolidays { get; set; }
        public virtual DbSet<Qanswer> Qanswers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public virtual DbSet<QuestionAnswerOption> QuestionAnswerOptions { get; set; }
        public virtual DbSet<QuestionGroup> QuestionGroups { get; set; }
        public virtual DbSet<QuestionGroupsQuestion> QuestionGroupsQuestions { get; set; }
        public virtual DbSet<QuestionHead> QuestionHeads { get; set; }
        public virtual DbSet<QuestionItem> QuestionItems { get; set; }
        public virtual DbSet<Scale> Scales { get; set; }
        public virtual DbSet<Scope> Scopes { get; set; }
        public virtual DbSet<ScopeClaim> ScopeClaims { get; set; }
        public virtual DbSet<ScopeSecret> ScopeSecrets { get; set; }
        public virtual DbSet<ScoringApplication> ScoringApplications { get; set; }
        public virtual DbSet<ScoringProfile> ScoringProfiles { get; set; }
        public virtual DbSet<SubAssessmentTool> SubAssessmentTools { get; set; }
        public virtual DbSet<SubScale> SubScales { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveyProfileInfo> SurveyProfileInfos { get; set; }
        public virtual DbSet<SurveyProfileQuestionAnswer> SurveyProfileQuestionAnswers { get; set; }
        public virtual DbSet<SurveyProfileQuestionAnswerSurveyQuestionField> SurveyProfileQuestionAnswerSurveyQuestionFields { get; set; }
        public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public virtual DbSet<SurveyQuestionField> SurveyQuestionFields { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateInfo> TemplateInfos { get; set; }
        public virtual DbSet<TemplateUnsubscribe> TemplateUnsubscribes { get; set; }
        public virtual DbSet<Tmetask> Tmetasks { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<TopicGroup> TopicGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserConnection> UserConnections { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<UserDeviceInfo> UserDeviceInfos { get; set; }
        public virtual DbSet<VidAssesCriterion> VidAssesCriteria { get; set; }
        public virtual DbSet<VideoAssessment> VideoAssessments { get; set; }
        public virtual DbSet<VideoAssessmentQuestion> VideoAssessmentQuestions { get; set; }
        public virtual DbSet<WorkField> WorkFields { get; set; }
        public virtual DbSet<Workflow> Workflows { get; set; }
        public virtual DbSet<WorkflowActivity> WorkflowActivities { get; set; }
        public virtual DbSet<WorkflowActivityOutcome> WorkflowActivityOutcomes { get; set; }
        public virtual DbSet<WorkflowActivityVariable> WorkflowActivityVariables { get; set; }
        public virtual DbSet<WorkflowError> WorkflowErrors { get; set; }
        public virtual DbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public virtual DbSet<WorkflowInstanceComment> WorkflowInstanceComments { get; set; }
        public virtual DbSet<WorkflowInstanceTask> WorkflowInstanceTasks { get; set; }
        public virtual DbSet<WorkflowInstanceVariable> WorkflowInstanceVariables { get; set; }
        public virtual DbSet<WorkflowInstanceVariableVariableLookup> WorkflowInstanceVariableVariableLookups { get; set; }
        public virtual DbSet<WorkflowLog> WorkflowLogs { get; set; }
        public virtual DbSet<WorkflowLogVariable> WorkflowLogVariables { get; set; }
        public virtual DbSet<WorkflowVariable> WorkflowVariables { get; set; }
        public virtual DbSet<WorkflowVariableLookup> WorkflowVariableLookups { get; set; }
        public virtual DbSet<WorkflowVersion> WorkflowVersions { get; set; }
        public virtual DbSet<ProfileLanguage> ProfileLanguage { get; set; }
        public virtual DbSet<ProfileWorkExperienceJobTitle> ProfileWorkExperienceJobTitle { get; set; }
        public virtual DbSet<ReportAssessmentCognitiveSub> ReportAssessmentCognitiveSubs { get; set; }
        public virtual DbSet<ReportAssessmentCompetency> ReportAssessmentCompetencies { get; set; }
        public virtual DbSet<ReportAssessmentCount> ReportAssessmentCounts { get; set; }
        public virtual DbSet<ReportAssessmentOverview> ReportAssessmentOverviews { get; set; }
        public virtual DbSet<ReportAssessmentPercentageOverview> ReportAssessmentPercentageOverviews { get; set; }
        public virtual DbSet<ReportAssessmentPercentageRankingPerBatchMatrix> ReportAssessmentPercentageRankingPerBatchMatrices { get; set; }
        public virtual DbSet<ReportAssessmentPillar> ReportAssessmentPillars { get; set; }
        public virtual DbSet<ReportAssessmentProfilesWithStatus> ReportAssessmentProfilesWithStatuses { get; set; }
        public virtual DbSet<ReportAssessmentRound> ReportAssessmentRounds { get; set; }
        public virtual DbSet<ReportAssessmentScale> ReportAssessmentScales { get; set; }
        public virtual DbSet<ReportAssessmentScoreRangeDistribution> ReportAssessmentScoreRangeDistributions { get; set; }
        public virtual DbSet<ReportBatchAcceptance> ReportBatchAcceptances { get; set; }
        public virtual DbSet<ReportBatchSectorPipeline> ReportBatchSectorPipelines { get; set; }
        public virtual DbSet<ReportGlpprogramme> ReportGlpprogrammes { get; set; }
        public virtual DbSet<ReportGovernmentEntitiesCount> ReportGovernmentEntitiesCounts { get; set; }
        public virtual DbSet<ReportInitiativesCount> ReportInitiativesCounts { get; set; }
        public virtual DbSet<ReportInterest> ReportInterests { get; set; }
        public virtual DbSet<ReportLearningPreference> ReportLearningPreferences { get; set; }
        public virtual DbSet<ReportMeetingHubFollower> ReportMeetingHubFollowers { get; set; }
        public virtual DbSet<ReportNetworkEngagement> ReportNetworkEngagements { get; set; }
        public virtual DbSet<ReportOrgnizationIndustry> ReportOrgnizationIndustries { get; set; }
        public virtual DbSet<ReportParticipantSector> ReportParticipantSectors { get; set; }
        public virtual DbSet<ReportProfile> ReportProfiles { get; set; }
        public virtual DbSet<ReportProfileAssessmentRankingTrial> ReportProfileAssessmentRankingTrials { get; set; }
        public virtual DbSet<ReportSectorRegistrationActivation> ReportSectorRegistrationActivations { get; set; }
        public virtual DbSet<ReportSectorsEmirate> ReportSectorsEmirates { get; set; }
        public virtual DbSet<ReportSectorsProgramme> ReportSectorsProgrammes { get; set; }
        public virtual DbSet<ReportSkill> ReportSkills { get; set; }
        public virtual DbSet<ReportTalentSector> ReportTalentSectors { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<RecommendLeadr> RecommandLeaders { get; set; }
        public virtual DbSet<RecommendationFitDetails> RecommendationFitDetails { get; set; }
        public virtual DbSet<RecommendationCallback> RecommendationCallback { get; set; }
        public virtual DbSet<RecommandationOther> RecommandationOthers { get; set; }
        public virtual DbSet<UserRecommendation> UserRecommendations { get; set; }
        public virtual DbSet<ReportProblem> ReportProblems { get; set; }
        public virtual DbSet<UserLocation> UserLocations { get; set; }
        public virtual DbSet<CustomNotification> CustomNotifications { get; set; }
        public virtual DbSet<ProfileBatchSelectedAlumni> ProfileBatchSelectedAlumni { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  DBConnectionString : {_appSettings}");
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer("Data Source=10.200.2.116;Initial Catalog=UAEGLPPlatform_MobileR2;Persist Security Info=True;User ID=sa;Password=Sw0rd@2020;");
                optionsBuilder.UseSqlServer(_appSettings);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adconnection>(entity =>
            {
                entity.HasIndex(e => e.AdconnectionTypeItemId)
                    .HasName("IX_ADConnectionTypeItemID");

                entity.HasIndex(e => e.PermissionSetId)
                    .HasName("IX_PermissionSetID");

                entity.HasIndex(e => e.WelcomeTemplateId)
                    .HasName("IX_WelcomeTemplateID");

                entity.Property(e => e.LangKey).HasDefaultValueSql("('')");

                entity.HasOne(d => d.AdconnectionTypeItem)
                    .WithMany(p => p.Adconnections)
                    .HasForeignKey(d => d.AdconnectionTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ADConnection_bbsf.LookupItem_ADConnectionTypeItemID");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.Adconnections)
                    .HasForeignKey(d => d.PermissionSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ADConnection_bbsf.PermissionSet_PermissionSetID");

                entity.HasOne(d => d.WelcomeTemplate)
                    .WithMany(p => p.Adconnections)
                    .HasForeignKey(d => d.WelcomeTemplateId)
                    .HasConstraintName("FK_bbsf.ADConnection_bbsf.Template_WelcomeTemplateID");
            });

            modelBuilder.Entity<Admapping>(entity =>
            {
                entity.HasIndex(e => e.AdconnectionId)
                    .HasName("IX_ADConnectionID");

                entity.HasIndex(e => e.AdmappingTypeItemId)
                    .HasName("IX_ADMappingTypeItemID");

                entity.HasOne(d => d.Adconnection)
                    .WithMany(p => p.Admappings)
                    .HasForeignKey(d => d.AdconnectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ADMapping_bbsf.ADConnection_ADConnectionID");

                entity.HasOne(d => d.AdmappingTypeItem)
                    .WithMany(p => p.Admappings)
                    .HasForeignKey(d => d.AdmappingTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ADMapping_bbsf.LookupItem_ADMappingTypeItemID");
            });

            modelBuilder.Entity<Agendum>(entity =>
            {
                entity.HasOne(d => d.Meetup)
                    .WithMany(p => p.Agenda)
                    .HasForeignKey(d => d.MeetupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Agenda_dbo.Meetup_MeetupID");
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Announcements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Announcement_bbsf.User_UserID");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasIndex(e => e.VideoAssessmentStatusId)
                    .HasName("IX_VideoAssessmentStatusID");

                entity.HasOne(d => d.AssessmentItem)
                    .WithMany(p => p.ApplicationAssessmentItems)
                    .HasForeignKey(d => d.AssessmentItemId)
                    .HasConstraintName("FK_dbo.Application_bbsf.LookupItem_AssessmentItemID");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Application_dbo.Batch_BatchID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Application_dbo.Profile_ProfileID");

                entity.HasOne(d => d.ReviewStatusItem)
                    .WithMany(p => p.ApplicationReviewStatusItems)
                    .HasForeignKey(d => d.ReviewStatusItemId)
                    .HasConstraintName("FK_dbo.Application_bbsf.LookupItem_ReviewStatusItemID");

                entity.HasOne(d => d.SecurityItem)
                    .WithMany(p => p.ApplicationSecurityItems)
                    .HasForeignKey(d => d.SecurityItemId)
                    .HasConstraintName("FK_dbo.Application_bbsf.LookupItem_SecurityItemID");

                entity.HasOne(d => d.StatusItem)
                    .WithMany(p => p.ApplicationStatusItems)
                    .HasForeignKey(d => d.StatusItemId)
                    .HasConstraintName("FK_dbo.Application_bbsf.LookupItem_StatusItemID");

                entity.HasOne(d => d.VideoAssessmentStatus)
                    .WithMany(p => p.ApplicationVideoAssessmentStatuses)
                    .HasForeignKey(d => d.VideoAssessmentStatusId)
                    .HasConstraintName("FK_dbo.Application_bbsf.LookupItem_VideoAssessmentStatusID");
            });

            modelBuilder.Entity<ApplicationAchievement>(entity =>
            {
                entity.HasIndex(e => e.AwardItemId)
                    .HasName("IX_AwardItemID");

                entity.HasIndex(e => e.MedalItemId)
                    .HasName("IX_MedalItemID");

                entity.HasIndex(e => e.OrgnizationId)
                    .HasName("IX_OrgnizationID");

                entity.HasIndex(e => e.ReachedItemId)
                    .HasName("IX_ReachedItemID");

                entity.HasOne(d => d.Achievement)
                    .WithMany(p => p.ApplicationAchievements)
                    .HasForeignKey(d => d.AchievementId)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_dbo.ProfileAchievement_AchievementID");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationAchievements)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_dbo.Application_ApplicationID");

                entity.HasOne(d => d.AwardItem)
                    .WithMany(p => p.ApplicationAchievementAwardItems)
                    .HasForeignKey(d => d.AwardItemId)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_bbsf.LookupItem_AwardItemID");

                entity.HasOne(d => d.MedalItem)
                    .WithMany(p => p.ApplicationAchievementMedalItems)
                    .HasForeignKey(d => d.MedalItemId)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_bbsf.LookupItem_MedalItemID");

                entity.HasOne(d => d.Orgnization)
                    .WithMany(p => p.ApplicationAchievements)
                    .HasForeignKey(d => d.OrgnizationId)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_dbo.GlpOrganization_OrgnizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ApplicationAchievements)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_dbo.Profile_ProfileID");

                entity.HasOne(d => d.ReachedItem)
                    .WithMany(p => p.ApplicationAchievementReachedItems)
                    .HasForeignKey(d => d.ReachedItemId)
                    .HasConstraintName("FK_dbo.ApplicationAchievement_bbsf.LookupItem_ReachedItemID");
            });

            modelBuilder.Entity<ApplicationCycleLog>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasName("IX_ApplicationID");

                entity.HasIndex(e => e.ApplicationStatusItemId)
                    .HasName("IX_ApplicationStatusItemID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationCycleLogs)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationCycleLog_dbo.Application_ApplicationID");

                entity.HasOne(d => d.ApplicationStatusItem)
                    .WithMany(p => p.ApplicationCycleLogs)
                    .HasForeignKey(d => d.ApplicationStatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationCycleLog_bbsf.LookupItem_ApplicationStatusItemID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ApplicationCycleLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationCycleLog_bbsf.User_UserID");
            });

            modelBuilder.Entity<ApplicationProgress>(entity =>
            {
                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationProgresses)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationProgress_dbo.Application_ApplicationID");

                entity.HasOne(d => d.ApplicationSectionItem)
                    .WithMany(p => p.ApplicationProgressApplicationSectionItems)
                    .HasForeignKey(d => d.ApplicationSectionItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationProgress_bbsf.LookupItem_ApplicationSectionItemID");

                entity.HasOne(d => d.ApplicationSectionStatusItem)
                    .WithMany(p => p.ApplicationProgressApplicationSectionStatusItems)
                    .HasForeignKey(d => d.ApplicationSectionStatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationProgress_bbsf.LookupItem_ApplicationSectionStatusItemID");
            });

            modelBuilder.Entity<ApplicationTraining>(entity =>
            {
                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationTrainings)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationTraining_dbo.Application_ApplicationID");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ApplicationTrainings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationTraining_dbo.GlpOrganization_OrganizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ApplicationTrainings)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationTraining_dbo.Profile_ProfileID");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.ApplicationTrainings)
                    .HasForeignKey(d => d.TrainingId)
                    .HasConstraintName("FK_dbo.ApplicationTraining_dbo.ProfileTraining_TrainingID");
            });

            modelBuilder.Entity<AssessmentBlock>(entity =>
            {
                entity.HasIndex(e => e.AssessmenttoolId)
                    .HasName("IX_AssessmenttoolID");

                entity.HasIndex(e => e.LevelId)
                    .HasName("IX_LevelID");

                entity.HasIndex(e => e.SubAssessmenttoolId)
                    .HasName("IX_SubAssessmenttoolID");

                entity.HasOne(d => d.Assessmenttool)
                    .WithMany(p => p.AssessmentBlocks)
                    .HasForeignKey(d => d.AssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentBlock_dbo.AssessmentTool_AssessmenttoolID");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.AssessmentBlocks)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK_dbo.AssessmentBlock_bbsf.LookupItem_LevelID");

                entity.HasOne(d => d.SubAssessmenttool)
                    .WithMany(p => p.AssessmentBlocks)
                    .HasForeignKey(d => d.SubAssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentBlock_dbo.SubAssessmentTool_SubAssessmenttoolID");
            });

            modelBuilder.Entity<AssessmentGroup>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolMatrixId)
                    .HasName("IX_AssessmentToolMatrixID");

                entity.HasOne(d => d.AssessmentToolMatrix)
                    .WithMany(p => p.AssessmentGroups)
                    .HasForeignKey(d => d.AssessmentToolMatrixId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentGroup_dbo.AssessmentToolMatrix_AssessmentToolMatrixID");
            });

            modelBuilder.Entity<AssessmentGroupMember>(entity =>
            {
                entity.HasIndex(e => e.AssessmentGroupId)
                    .HasName("IX_AssessmentGroupID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.AssessmentGroup)
                    .WithMany(p => p.AssessmentGroupMembers)
                    .HasForeignKey(d => d.AssessmentGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentGroupMember_dbo.AssessmentGroup_AssessmentGroupID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.AssessmentGroupMembers)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentGroupMember_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<AssessmentNarrativeReport>(entity =>
            {
                entity.HasIndex(e => e.AssessmentCategoryId)
                    .HasName("IX_AssessmentCategoryID");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.AssessmentCategory)
                    .WithMany(p => p.AssessmentNarrativeReports)
                    .HasForeignKey(d => d.AssessmentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentNarrativeReport_bbsf.LookupItem_AssessmentCategoryID");
            });

            modelBuilder.Entity<AssessmentNumber>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<AssessmentReportFeedback>(entity =>
            {
                entity.HasIndex(e => e.CreatedUserId)
                    .HasName("IX_CreatedUserID");

                entity.HasIndex(e => e.ProfileAssesmentToolScoreId)
                    .HasName("IX_ProfileAssesmentToolScoreID");

                entity.HasOne(d => d.CreatedUser)
                    .WithMany(p => p.AssessmentReportFeedbacks)
                    .HasForeignKey(d => d.CreatedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentReportFeedback_bbsf.User_CreatedUserID");

                entity.HasOne(d => d.ProfileAssesmentToolScore)
                    .WithMany(p => p.AssessmentReportFeedbacks)
                    .HasForeignKey(d => d.ProfileAssesmentToolScoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentReportFeedback_dbo.ProfileAssessmentToolScore_ProfileAssesmentToolScoreID");
            });

            modelBuilder.Entity<AssessmentTool>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolCategory)
                    .HasName("IX_AssessmentToolCategory");

                entity.Property(e => e.DisplayOrder).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPublished).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.AssessmentToolCategoryNavigation)
                    .WithMany(p => p.AssessmentToolAssessmentToolCategoryNavigations)
                    .HasForeignKey(d => d.AssessmentToolCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssessmentTool_bbsf.LookupItem_AssessmentToolCategory");

                entity.HasOne(d => d.AssessmentToolTypeNavigation)
                    .WithMany(p => p.AssessmentToolAssessmentToolTypeNavigations)
                    .HasForeignKey(d => d.AssessmentToolType)
                    .HasConstraintName("FK_dbo.AssessmentTool_bbsf.LookupItem_AssessmentToolType");
            });

            modelBuilder.Entity<AssignedAssessment>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolMatrixId)
                    .HasName("IX_AssessmentToolMatrixID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.AssessmentToolMatrix)
                    .WithMany(p => p.AssignedAssessments)
                    .HasForeignKey(d => d.AssessmentToolMatrixId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssignedAssessment_dbo.AssessmentToolMatrix_AssessmentToolMatrixID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.AssignedAssessments)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssignedAssessment_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Assignment_dbo.Batch_BatchID");
            });

            modelBuilder.Entity<AssignmentAnswer>(entity =>
            {
                entity.HasOne(d => d.Assighment)
                    .WithMany(p => p.AssignmentAnswers)
                    .HasForeignKey(d => d.AssighmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssignmentAnswer_dbo.Assignment_AssighmentID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.AssignmentAnswers)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssignmentAnswer_dbo.Profile_ProfileID");

                entity.HasOne(d => d.StatusItem)
                    .WithMany(p => p.AssignmentAnswers)
                    .HasForeignKey(d => d.StatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.AssignmentAnswer_bbsf.LookupItem_StatusItemID");
            });

            modelBuilder.Entity<BadgeRequest>(entity =>
            {
                entity.HasIndex(e => e.BadgeId)
                    .HasName("IX_BadgeID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.StatusId)
                    .HasName("IX_StatusID");

                entity.HasOne(d => d.Badge)
                    .WithMany(p => p.BadgeRequests)
                    .HasForeignKey(d => d.BadgeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.BadgeRequest_dbo.Badge_BadgeID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.BadgeRequests)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.BadgeRequest_bbsf.User_ProfileID");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BadgeRequests)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.BadgeRequest_bbsf.LookupItem_LookupItemID");
            });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.HasIndex(e => e.AssessmentMatrixId)
                    .HasName("IX_AssessmentMatrixID");

                entity.HasIndex(e => e.VideoAssessmentId)
                    .HasName("IX_VideoAssessmentID");

                entity.HasOne(d => d.AssessmentMatrix)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.AssessmentMatrixId)
                    .HasConstraintName("FK_dbo.Batch_dbo.AssessmentToolMatrix_AssessmentMatrixID");

                entity.HasOne(d => d.Programme)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.ProgrammeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Batch_dbo.Programme_ProgrammeID");

                entity.HasOne(d => d.QuestionGroup)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.QuestionGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Batch_dbo.QuestionGroup_QuestionGroupID");

                entity.HasOne(d => d.VideoAssessment)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.VideoAssessmentId)
                    .HasConstraintName("FK_dbo.Batch_dbo.VideoAssessment_VideoAssessmentID");
            });

            modelBuilder.Entity<BatchAssessmentTool>(entity =>
            {
                entity.HasKey(e => new { e.BatchId, e.AssessmentToolId })
                    .HasName("PK_dbo.Batch_AssessmentTools");

                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentTool_ID");

                entity.HasIndex(e => e.BatchId)
                    .HasName("IX_Batch_ID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.BatchAssessmentTools)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Batch_AssessmentTools_dbo.AssessmentTool_AssessmentTool_ID");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.BatchAssessmentTools)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Batch_AssessmentTools_dbo.Batch_Batch_ID");
            });

            modelBuilder.Entity<BatchInitiative>(entity =>
            {
                entity.HasKey(e => new { e.BatchId, e.InitiativeId })
                    .HasName("PK_dbo.BatchInitiative");

                entity.HasIndex(e => e.BatchId)
                    .HasName("IX_Batch_ID");

                entity.HasIndex(e => e.InitiativeId)
                    .HasName("IX_Initiative_ID");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.BatchInitiatives)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.BatchInitiative_dbo.Batch_Batch_ID");

                entity.HasOne(d => d.Initiative)
                    .WithMany(p => p.BatchInitiatives)
                    .HasForeignKey(d => d.InitiativeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.BatchInitiative_dbo.Initiative_Initiative_ID");
            });

            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.HasIndex(e => e.GovernmentEntityId)
                    .HasName("IX_GovernmentEntityID");

                entity.HasOne(d => d.GovernmentEntity)
                    .WithMany(p => p.Challenges)
                    .HasForeignKey(d => d.GovernmentEntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Challenge_dbo.GovernmentEntity_GovernmentEntityID");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasIndex(e => e.MessageTypeItemId)
                    .HasName("IX_MessageTypeItemID");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("IX_OwnerID");

                entity.HasIndex(e => e.RoomId)
                    .HasName("IX_RoomID");

                entity.HasOne(d => d.MessageTypeItem)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.MessageTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatMessage_bbsf.LookupItem_MessageTypeItemID");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatMessage_bbsf.User_OwnerID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatMessage_bbsf.ChatRoom_RoomID");
            });

            modelBuilder.Entity<ChatMessageAttachment>(entity =>
            {
                entity.HasIndex(e => e.AttachmentTypeItemId)
                    .HasName("IX_AttachmentTypeItemID");

                entity.HasIndex(e => e.ChatMessageId)
                    .HasName("IX_ChatMessage_ID");

                entity.HasOne(d => d.AttachmentTypeItem)
                    .WithMany(p => p.ChatMessageAttachments)
                    .HasForeignKey(d => d.AttachmentTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatMessageAttachment_bbsf.LookupItem_AttachmentTypeItemID");

                entity.HasOne(d => d.ChatMessage)
                    .WithMany(p => p.ChatMessageAttachments)
                    .HasForeignKey(d => d.ChatMessageId)
                    .HasConstraintName("FK_bbsf.ChatMessageAttachment_bbsf.ChatMessage_ChatMessage_ID");
            });

            modelBuilder.Entity<ChatMessageSeenBy>(entity =>
            {
                entity.HasIndex(e => e.MessageId)
                    .HasName("IX_MessageID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.ChatMessageSeenBies)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatMessageSeenBy_bbsf.ChatMessage_MessageID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatMessageSeenBies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatMessageSeenBy_bbsf.User_UserID");
            });

            modelBuilder.Entity<ChatRoom>(entity =>
            {
                entity.HasIndex(e => e.RoomTypeItemId)
                    .HasName("IX_RoomTypeItemID");

                entity.HasOne(d => d.RoomTypeItem)
                    .WithMany(p => p.ChatRooms)
                    .HasForeignKey(d => d.RoomTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatRoom_bbsf.LookupItem_RoomTypeItemID");
            });

            modelBuilder.Entity<ChatRoomUser>(entity =>
            {
                entity.HasKey(e => new { e.ChatRoomId, e.UserId })
                    .HasName("PK_bbsf.ChatRoom_User");

                entity.HasIndex(e => e.ChatRoomId)
                    .HasName("IX_ChatRoomID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.ChatRoom)
                    .WithMany(p => p.ChatRoomUsers)
                    .HasForeignKey(d => d.ChatRoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatRoom_User_bbsf.ChatRoom_ChatRoomID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatRoomUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatRoom_User_bbsf.User_UserID");
            });

            modelBuilder.Entity<ChatUnreadMessage>(entity =>
            {
                entity.HasIndex(e => e.RoomId)
                    .HasName("IX_RoomID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.ChatUnreadMessages)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatUnreadMessage_bbsf.ChatRoom_RoomID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatUnreadMessages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ChatUnreadMessage_bbsf.User_UserID");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_ClientId")
                    .IsUnique();
            });

            modelBuilder.Entity<ClientClaim>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientClaims)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientClaims_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientCorsOrigin>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientCorsOrigins)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientCorsOrigins_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientCustomGrantType>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientCustomGrantTypes)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientCustomGrantTypes_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientIdPrestriction>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientIdPrestrictions)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientIdPRestrictions_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUri>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientPostLogoutRedirectUris)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientPostLogoutRedirectUris_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientRedirectUri>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientRedirectUris)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientRedirectUris_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientScope>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientScopes)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientScopes_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<ClientSecret>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasName("IX_Client_Id");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientSecrets)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.ClientSecrets_dbo.Clients_Client_Id");
            });

            modelBuilder.Entity<Competency>(entity =>
            {
                entity.HasOne(d => d.Pillar)
                    .WithMany(p => p.Competencies)
                    .HasForeignKey(d => d.PillarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Competency_dbo.Pillar_PillarID");
            });

            modelBuilder.Entity<CompetencyAssessmentTool>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.CompetencyId)
                    .HasName("IX_CompetencyID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.CompetencyAssessmentTools)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CompetencyAssessmentTool_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Competency)
                    .WithMany(p => p.CompetencyAssessmentTools)
                    .HasForeignKey(d => d.CompetencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CompetencyAssessmentTool_dbo.Competency_CompetencyID");
            });

            modelBuilder.Entity<CompetencySubscale>(entity =>
            {
                entity.HasIndex(e => e.CompetencyId)
                    .HasName("IX_CompetencyID");

                entity.HasIndex(e => e.SubscaleId)
                    .HasName("IX_SubscaleID");

                entity.HasOne(d => d.Competency)
                    .WithMany(p => p.CompetencySubscales)
                    .HasForeignKey(d => d.CompetencyId)
                    .HasConstraintName("FK_dbo.CompetencySubscale_dbo.Competency_CompetencyID");

                entity.HasOne(d => d.Subscale)
                    .WithMany(p => p.CompetencySubscales)
                    .HasForeignKey(d => d.SubscaleId)
                    .HasConstraintName("FK_dbo.CompetencySubscale_dbo.SubScale_SubscaleID");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.HasIndex(e => e.PermissionSetId)
                    .HasName("IX_PermissionSetID");

                entity.HasIndex(e => e.ScopeItemId)
                    .HasName("IX_ScopeItemID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.PermissionSetId)
                    .HasConstraintName("FK_bbsf.Configurations_bbsf.PermissionSet_PermissionSetID");

                entity.HasOne(d => d.ScopeItem)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.ScopeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.Configurations_bbsf.LookupItem_ScopeItemID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_bbsf.Configurations_bbsf.User_UserID");
            });

            modelBuilder.Entity<Consent>(entity =>
            {
                entity.HasKey(e => new { e.Subject, e.ClientId })
                    .HasName("PK_dbo.Consents");
            });

            modelBuilder.Entity<CriteriaClaim>(entity =>
            {
                entity.HasIndex(e => e.CriteriaId)
                    .HasName("IX_CriteriaID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.StatusId)
                    .HasName("IX_StatusID");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.CriteriaClaims)
                    .HasForeignKey(d => d.CriteriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CriteriaClaim_dbo.Criteria_CriteriaID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.CriteriaClaims)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CriteriaClaim_dbo.Profile_ProfileID");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CriteriaClaims)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CriteriaClaim_bbsf.LookupItem_StatusID");
            });

            modelBuilder.Entity<Criteria>(entity =>
            {
                entity.HasIndex(e => e.CriteriaCategoryId)
                    .HasName("IX_CriteriaCategoryID");

                entity.HasOne(d => d.CriteriaCategory)
                    .WithMany(p => p.Criteria)
                    .HasForeignKey(d => d.CriteriaCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Criteria_bbsf.LookupItem_CriteriaCategoryID");
            });

            modelBuilder.Entity<Criterion1>(entity =>
            {
                entity.HasIndex(e => e.CriteriaId)
                    .HasName("IX_CriteriaID");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.Criterion1s)
                    .HasForeignKey(d => d.CriteriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Criterion_dbo.Criteria_CriteriaID");
            });

            modelBuilder.Entity<DirectorySearchHistory>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DirectorySearchHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.DirectorySearchHistory_bbsf.User_UserID");
            });

            modelBuilder.Entity<EmailHeaderAndFooterTemplate>(entity =>
            {
                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            modelBuilder.Entity<Eresource>(entity =>
            {
                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.Eresources)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.EResource_dbo.Folder_FolderID");
            });

            modelBuilder.Entity<EresourceLink>(entity =>
            {
                entity.HasOne(d => d.Eresource)
                    .WithMany(p => p.EresourceLinks)
                    .HasForeignKey(d => d.EresourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.EResourceLink_dbo.EResource_EResourceID");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasIndex(e => e.BatchId)
                    .HasName("IX_BatchID");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("FK_dbo.Event_dbo.Batch_BatchID");
            });

            modelBuilder.Entity<EventDay>(entity =>
            {
                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventDays)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.EventDay_dbo.Event_EventID");
            });

            modelBuilder.Entity<Factor>(entity =>
            {
                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.Factors)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Factor_dbo.AssessmentTool_AssessmentToolID");
            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.HasIndex(e => e.ProgrammeId)
                    .HasName("IX_ProgrammeID");

                entity.HasOne(d => d.Programme)
                    .WithMany(p => p.Faqs)
                    .HasForeignKey(d => d.ProgrammeId)
                    .HasConstraintName("FK_dbo.FAQ_dbo.Programme_ProgrammeID");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasIndex(e => e.FolderId)
                    .HasName("IX_FolderID");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.FolderId)
                    .HasConstraintName("FK_bbsf.File_bbsf.LibraryFolder_FolderID");
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.HasOne(d => d.ParentFolder)
                    .WithMany(p => p.InverseParentFolder)
                    .HasForeignKey(d => d.ParentFolderId)
                    .HasConstraintName("FK_dbo.Folder_dbo.Folder_ParentFolderID");
            });

            modelBuilder.Entity<FolderBatch>(entity =>
            {
                entity.HasKey(e => new { e.FolderId, e.BatchId })
                    .HasName("PK_dbo.FolderBatch");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.FolderBatches)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.FolderBatch_dbo.Batch_Batch_ID");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.FolderBatches)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.FolderBatch_dbo.Folder_Folder_ID");
            });

            modelBuilder.Entity<FormsHubConnection>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FormsHubConnections)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.FormsHubConnection_bbsf.User_UserId");
            });

            modelBuilder.Entity<GlpOrganization>(entity =>
            {
                entity.HasIndex(e => e.OrganizationEmirateLookupItemId)
                    .HasName("IX_OrganizationEmirateLookupItemID");

                entity.HasIndex(e => e.OrganizationIndustryItemId)
                    .HasName("IX_OrganizationIndustryItemID");

                entity.HasOne(d => d.OrganizationEmirateLookupItem)
                    .WithMany(p => p.GlpOrganizationOrganizationEmirateLookupItems)
                    .HasForeignKey(d => d.OrganizationEmirateLookupItemId)
                    .HasConstraintName("FK_dbo.GlpOrganization_bbsf.LookupItem_OrganizationEmirateLookupItemID");

                entity.HasOne(d => d.OrganizationIndustryItem)
                    .WithMany(p => p.GlpOrganizationOrganizationIndustryItems)
                    .HasForeignKey(d => d.OrganizationIndustryItemId)
                    .HasConstraintName("FK_dbo.GlpOrganization_bbsf.LookupItem_OrganizationIndustryItemID");

                entity.HasOne(d => d.OrganizationScaleItem)
                    .WithMany(p => p.GlpOrganizationOrganizationScaleItems)
                    .HasForeignKey(d => d.OrganizationScaleItemId)
                    .HasConstraintName("FK_dbo.GlpOrganization_bbsf.LookupItem_OrganizationScaleItemID");

                entity.HasOne(d => d.OrganizationSectorTypeItem)
                    .WithMany(p => p.GlpOrganizationOrganizationSectorTypeItems)
                    .HasForeignKey(d => d.OrganizationSectorTypeItemId)
                    .HasConstraintName("FK_dbo.GlpOrganization_bbsf.LookupItem_OrganizationSectorTypeItemID");

                entity.HasOne(d => d.OrganizationTypeItem)
                    .WithMany(p => p.GlpOrganizationOrganizationTypeItems)
                    .HasForeignKey(d => d.OrganizationTypeItemId)
                    .HasConstraintName("FK_dbo.GlpOrganization_bbsf.LookupItem_OrganizationTypeItemID");
            });

            modelBuilder.Entity<GlpOrganizationItemType>(entity =>
            {
                entity.HasKey(e => new { e.GlpOrganizationId, e.ClientLookupItemId })
                    .HasName("PK_dbo.GlpOrganization_ItemTypes");

                entity.HasOne(d => d.ClientLookupItem)
                    .WithMany(p => p.GlpOrganizationItemTypes)
                    .HasForeignKey(d => d.ClientLookupItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GlpOrganization_ItemTypes_bbsf.LookupItem_ClientLookupItemID");

                entity.HasOne(d => d.GlpOrganization)
                    .WithMany(p => p.GlpOrganizationItemTypes)
                    .HasForeignKey(d => d.GlpOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GlpOrganization_ItemTypes_dbo.GlpOrganization_GlpOrganizationID");
            });

            modelBuilder.Entity<Glppermission>(entity =>
            {
                entity.HasIndex(e => e.BatchId)
                    .HasName("IX_BatchID");

                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Glppermissions)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("FK_dbo.GLPPermission_dbo.Batch_BatchID");
            });

            modelBuilder.Entity<GlppermissionClientUser>(entity =>
            {
                entity.HasKey(e => new { e.GlppermissionId, e.ClientUserId })
                    .HasName("PK_dbo.GLPPermissionClientUser");

                entity.HasIndex(e => e.ClientUserId)
                    .HasName("IX_ClientUser_ID");

                entity.HasIndex(e => e.GlppermissionId)
                    .HasName("IX_GLPPermission_ID");

                entity.HasOne(d => d.ClientUser)
                    .WithMany(p => p.GlppermissionClientUsers)
                    .HasForeignKey(d => d.ClientUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GLPPermissionClientUser_bbsf.User_ClientUser_ID");

                entity.HasOne(d => d.Glppermission)
                    .WithMany(p => p.GlppermissionClientUsers)
                    .HasForeignKey(d => d.GlppermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GLPPermissionClientUser_dbo.GLPPermission_GLPPermission_ID");
            });

            modelBuilder.Entity<GovernmentEntity>(entity =>
            {
                entity.HasIndex(e => e.EntityTypeItemId)
                    .HasName("IX_EntityTypeItemID");

                entity.HasOne(d => d.EntityTypeItem)
                    .WithMany(p => p.GovernmentEntities)
                    .HasForeignKey(d => d.EntityTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GovernmentEntity_bbsf.LookupItem_EntityTypeItemID");
            });

            modelBuilder.Entity<GovernmentEntityCoordinator>(entity =>
            {
                entity.HasKey(e => new { e.ClientUserId, e.GovernmentEntityId })
                    .HasName("PK_dbo.GovernmentEntity_coordinators");

                entity.HasIndex(e => e.ClientUserId)
                    .HasName("IX_ClientUser_ID");

                entity.HasIndex(e => e.GovernmentEntityId)
                    .HasName("IX_GovernmentEntity_ID");

                entity.HasOne(d => d.ClientUser)
                    .WithMany(p => p.GovernmentEntityCoordinators)
                    .HasForeignKey(d => d.ClientUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GovernmentEntity_coordinators_bbsf.User_ClientUser_ID");

                entity.HasOne(d => d.GovernmentEntity)
                    .WithMany(p => p.GovernmentEntityCoordinators)
                    .HasForeignKey(d => d.GovernmentEntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.GovernmentEntity_coordinators_dbo.GovernmentEntity_GovernmentEntity_ID");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.DescriptionAr).HasDefaultValueSql("('')");

                entity.Property(e => e.NameAr).HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<ImpersonationRule>(entity =>
            {
                entity.HasIndex(e => e.DestinationParticipantId)
                    .HasName("IX_DestinationParticipantID");

                entity.HasIndex(e => e.SourceParticipantId)
                    .HasName("IX_SourceParticipantID");

                entity.HasOne(d => d.DestinationParticipant)
                    .WithMany(p => p.ImpersonationRuleDestinationParticipants)
                    .HasForeignKey(d => d.DestinationParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ImpersonationRule_bbsf.Participant_DestinationParticipantID");

                entity.HasOne(d => d.SourceParticipant)
                    .WithMany(p => p.ImpersonationRuleSourceParticipants)
                    .HasForeignKey(d => d.SourceParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ImpersonationRule_bbsf.Participant_SourceParticipantID");
            });

            modelBuilder.Entity<Initiative>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("IX_CategoryID");

                entity.HasIndex(e => e.InitiativeTypeItemId)
                    .HasName("IX_InitiativeTypeItemID");

                entity.HasIndex(e => e.QuestionGroupId)
                    .HasName("IX_QuestionGroupID");

                entity.Property(e => e.InitiativeTypeItemId).HasDefaultValueSql("((71001))");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Initiatives)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_dbo.Initiative_dbo.EngagementActivityCategory_EngagementActivityCategoryID");

                entity.HasOne(d => d.InitiativeTypeItem)
                    .WithMany(p => p.Initiatives)
                    .HasForeignKey(d => d.InitiativeTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Initiative_bbsf.LookupItem_InitiativeTypeItemID");

                entity.HasOne(d => d.QuestionGroup)
                    .WithMany(p => p.Initiatives)
                    .HasForeignKey(d => d.QuestionGroupId)
                    .HasConstraintName("FK_dbo.Initiative_dbo.QuestionGroup_QuestionGroupID");
            });

            modelBuilder.Entity<InitiativeCategory>(entity =>
            {
                entity.Property(e => e.DescriptionAr).HasDefaultValueSql("('')");

                entity.Property(e => e.TitleAr).HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<InitiativeProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.InitiativeId)
                    .HasName("IX_InitiativeID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.StatusItemId)
                    .HasName("IX_StatusItemID");

                entity.HasOne(d => d.Initiative)
                    .WithMany(p => p.InitiativeProfiles)
                    .HasForeignKey(d => d.InitiativeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Initiative_Profile_dbo.Initiative_InitiativeID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.InitiativeProfiles)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Initiative_Profile_dbo.Profile_ProfileID");

                entity.HasOne(d => d.StatusItem)
                    .WithMany(p => p.InitiativeProfiles)
                    .HasForeignKey(d => d.StatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Initiative_Profile_bbsf.LookupItem_StatusItemID");

            });

                modelBuilder.Entity<KnowledgeHub>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("IX_CategoryID");

                entity.Property(e => e.HtmlcontentAr).HasDefaultValueSql("('')");

                entity.Property(e => e.HtmlcontentEn).HasDefaultValueSql("('')");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KnowledgeHubs)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.KnowledgeHub_bbsf.LookupItem_CategoryID");
            });

            modelBuilder.Entity<KnowledgeHubCategory>(entity =>
            {
                entity.HasIndex(e => e.LogoId)
                    .HasName("IX_LogoID");

                entity.HasOne(d => d.Logo)
                    .WithMany(p => p.KnowledgeHubCategories)
                    .HasForeignKey(d => d.LogoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.KnowledgeHubCategory_bbsf.File_LogoID");
            });

            modelBuilder.Entity<KnowledgeHubCourse>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("IX_CategoryID");

                entity.HasIndex(e => e.CourseTypeId)
                    .HasName("IX_CourseTypeID");

                entity.HasIndex(e => e.ProviderTypeId)
                    .HasName("IX_ProviderTypeID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KnowledgeHubCourses)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_dbo.KnowledgeHubCourse_dbo.KnowledgeHubCategory_CategoryID");

                entity.HasOne(d => d.CourseType)
                    .WithMany(p => p.KnowledgeHubCourseCourseTypes)
                    .HasForeignKey(d => d.CourseTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.KnowledgeHubCourse_bbsf.LookupItem_CourseTypeID");

                entity.HasOne(d => d.ProviderType)
                    .WithMany(p => p.KnowledgeHubCourseProviderTypes)
                    .HasForeignKey(d => d.ProviderTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.KnowledgeHubCourse_bbsf.LookupItem_ProviderTypeID");
            });

            modelBuilder.Entity<LibraryFolder>(entity =>
            {
                entity.HasIndex(e => e.ParentFolderId)
                    .HasName("IX_ParentFolderID");

                entity.HasOne(d => d.ParentFolder)
                    .WithMany(p => p.InverseParentFolder)
                    .HasForeignKey(d => d.ParentFolderId)
                    .HasConstraintName("FK_bbsf.LibraryFolder_bbsf.LibraryFolder_ParentFolderID");
            });

            modelBuilder.Entity<Localization>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_dbo.Localization");

                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("('admin@bnsights.com')");

                entity.Property(e => e.Modified).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasDefaultValueSql("('admin@bnsights.com')");

                entity.Property(e => e.Updated).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.LookupGroup)
                    .WithMany(p => p.Lookups)
                    .HasForeignKey(d => d.LookupGroupId)
                    .HasConstraintName("FK_bbsf.Lookup_bbsf.LookupGroup_LookupGroupID");
            });

            modelBuilder.Entity<LookupItem>(entity =>
            {
                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discriminator).HasDefaultValueSql("('ClientLookupItem')");

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Lookup)
                    .WithMany(p => p.LookupItems)
                    .HasForeignKey(d => d.LookupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.LookupItem_bbsf.Lookup_LookupID");
            });

            modelBuilder.Entity<MatrixTool>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.MatrixId)
                    .HasName("IX_MatrixID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.MatricesTool)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Matrix_Tools_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Matrix)
                    .WithMany(p => p.MatricesTool)
                    .HasForeignKey(d => d.MatrixId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Matrix_Tools_dbo.AssessmentToolMatrix_MatrixID");
            });

            modelBuilder.Entity<Meetup>(entity =>
            {
                entity.HasIndex(e => e.OwnerId)
                    .HasName("IX_OwnerID");

                entity.HasIndex(e => e.StatusItemId)
                    .HasName("IX_StatusItemID");

                entity.Property(e => e.StatusItemId).HasDefaultValueSql("((76001))");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Meetups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Meetup_dbo.Group_GroupID");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Meetups)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Meetup_bbsf.User_OwnerID");

                entity.HasOne(d => d.StatusItem)
                    .WithMany(p => p.Meetups)
                    .HasForeignKey(d => d.StatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Meetup_bbsf.LookupItem_StatusItemID");
            });

            modelBuilder.Entity<NetworkGroup>(entity =>
            {
                entity.HasIndex(e => e.LogoId)
                    .HasName("IX_LogoID");

                entity.HasOne(d => d.Logo)
                    .WithMany(p => p.NetworkGroups)
                    .HasForeignKey(d => d.LogoId)
                    .HasConstraintName("FK_dbo.NetworkGroup_bbsf.File_LogoID");
            });

            modelBuilder.Entity<NetworkGroupProfile>(entity =>
            {
                entity.HasKey(e => new { e.NetworkGroupId, e.ProfileId })
                    .HasName("PK_dbo.NetworkGroupProfile");

                entity.HasIndex(e => e.NetworkGroupId)
                    .HasName("IX_NetworkGroup_ID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_Profile_ID");

                entity.HasOne(d => d.NetworkGroup)
                    .WithMany(p => p.NetworkGroupProfiles)
                    .HasForeignKey(d => d.NetworkGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.NetworkGroupProfile_dbo.NetworkGroup_NetworkGroup_ID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.NetworkGroupProfiles)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.NetworkGroupProfile_dbo.Profile_Profile_ID");
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Options)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_dbo.Option_dbo.Question_QuestionID");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasIndex(e => e.ParticipantTypeItemId)
                    .HasName("IX_ParticipantTypeItemID");

                entity.HasIndex(e => e.PermissionSetId)
                    .HasName("IX_PermissionSetID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasIndex(e => e.WorkflowActivityId)
                    .HasName("IX_WorkflowActivityID");

                entity.HasIndex(e => e.WorkflowVariableId)
                    .HasName("IX_WorkflowVariableID");

                entity.HasOne(d => d.ParticipantTypeItem)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.ParticipantTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.Participant_bbsf.LookupItem_ParticipantTypeItemID");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.PermissionSetId)
                    .HasConstraintName("FK_bbsf.Participant_bbsf.PermissionSet_PermissionSetID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_bbsf.Participant_bbsf.User_UserID");

                entity.HasOne(d => d.WorkflowActivity)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.WorkflowActivityId)
                    .HasConstraintName("FK_bbsf.Participant_bbsf.WorkflowActivity_WorkflowActivityID");

                entity.HasOne(d => d.WorkflowVariable)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.WorkflowVariableId)
                    .HasConstraintName("FK_bbsf.Participant_bbsf.WorkflowVariable_WorkflowVariableID");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.Permission_bbsf.Organization_OrganizationID");
            });

            modelBuilder.Entity<PermissionSet>(entity =>
            {
                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.ActiveDirectoryGroup).IsUnicode(false);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.PermissionSets)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.PermissionSet_bbsf.Organization_OrganizationID");
            });

            modelBuilder.Entity<PermissionSetPermission>(entity =>
            {
                entity.HasKey(e => new { e.PermissionSetId, e.PermissionId })
                    .HasName("PK_bbsf.PermissionSet_Permission");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionSetPermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.PermissionSet_Permission_bbsf.Permission_PermissionID");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.PermissionSetPermissions)
                    .HasForeignKey(d => d.PermissionSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.PermissionSet_Permission_bbsf.PermissionSet_PermissionSetID");
            });

            modelBuilder.Entity<PermissionSetUser>(entity =>
            {
                entity.HasKey(e => new { e.PermissionSetId, e.UserId })
                    .HasName("PK_bbsf.PermissionSet_User");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.PermissionSetUsers)
                    .HasForeignKey(d => d.PermissionSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.PermissionSet_User_bbsf.PermissionSet_PermissionSetID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PermissionSetUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.PermissionSet_User_bbsf.User_UserID");
            });

            modelBuilder.Entity<Pillar>(entity =>
            {
                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Pillars)
                    .HasForeignKey(d => d.ModelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Pillar_dbo.GLPModel_ModelID");
            });

            modelBuilder.Entity<PillarAssessmentTool>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.PillarId)
                    .HasName("IX_PillarID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.PillarAssessmentTools)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.PillarAssessmentTool_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Pillar)
                    .WithMany(p => p.PillarAssessmentTools)
                    .HasForeignKey(d => d.PillarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.PillarAssessmentTool_dbo.Pillar_PillarID");
            });

            modelBuilder.Entity<ProfileSkillEndorsement>()
                .HasKey(c => new { c.ProfileSkillId, c.ProfileId, c.PublicProfileId });

            modelBuilder.Entity<ProfileSkillProfile>()
                .HasKey(c => new { c.Id, c.ProfileId });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasIndex(e => e.BadgeId)
                    .HasName("IX_BadgeID");

                entity.HasIndex(e => e.CitizenshipStatusId)
                    .HasName("IX_CitizenshipStatusID");

                entity.HasIndex(e => e.FamilyBookFileId)
                    .HasName("IX_FamilyBookFileID");

                entity.HasIndex(e => e.Id)
                    .HasName("IX_ID");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Age).HasComputedColumnSql("((CONVERT([int],CONVERT([char](8),getdate(),(112)))-CONVERT([char](8),[BirthDate],(112)))/(10000))");

                entity.Property(e => e.ProfileLastModified).HasDefaultValueSql("('1900-01-01 00:00:00')");

                entity.Property(e => e.TotalYearsOfExperinceReadOnly).HasComputedColumnSql("([dbo].[GetTotalYearsOfExperince]([ID]))");

                entity.HasOne(d => d.Badge)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.BadgeId)
                    .HasConstraintName("FK_dbo.Profile_dbo.Badge_BadgeID");

                entity.HasOne(d => d.CitizenshipStatus)
                    .WithMany(p => p.ProfileCitizenshipStatuses)
                    .HasForeignKey(d => d.CitizenshipStatusId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.LookupItem_CitizenshipStatusID");

                entity.HasOne(d => d.Cvfile)
                    .WithMany(p => p.ProfileCvfiles)
                    .HasForeignKey(d => d.CvfileId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.File_CVFileID");

                entity.HasOne(d => d.EmirateItem)
                    .WithMany(p => p.ProfileEmirateItems)
                    .HasForeignKey(d => d.EmirateItemId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.LookupItem_EmirateItemID");

                entity.HasOne(d => d.FamilyBookFile)
                    .WithMany(p => p.ProfileFamilyBookFiles)
                    .HasForeignKey(d => d.FamilyBookFileId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.File_FamilyBookFileID");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Profile)
                    .HasForeignKey<Profile>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_bbsf.User_ID");

                entity.HasOne(d => d.LastEducationCertificateFile)
                    .WithMany(p => p.ProfileLastEducationCertificateFiles)
                    .HasForeignKey(d => d.LastEducationCertificateFileId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.File_LastEducationCertificateFileID");

                entity.HasOne(d => d.PassportFile)
                    .WithMany(p => p.ProfilePassportFiles)
                    .HasForeignKey(d => d.PassportFileId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.File_PassportFileID");

                entity.HasOne(d => d.PassportIssueEmirateItem)
                    .WithMany(p => p.ProfilePassportIssueEmirateItems)
                    .HasForeignKey(d => d.PassportIssueEmirateItemId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.LookupItem_PassportIssueEmirateItemID");

                entity.HasOne(d => d.ResidenceCountry)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.ResidenceCountryId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.Country_ResidenceCountryID");

                entity.HasOne(d => d.Uaeidfile)
                    .WithMany(p => p.ProfileUaeidfiles)
                    .HasForeignKey(d => d.UaeidfileId)
                    .HasConstraintName("FK_dbo.Profile_bbsf.File_UAEIDFileID");
            });

            modelBuilder.Entity<ProfileAchievement>(entity =>
            {
                entity.HasIndex(e => e.AwardItemId)
                    .HasName("IX_AwardItemID");

                entity.HasIndex(e => e.ImpactItemId)
                    .HasName("IX_ImpactItemID");

                entity.HasIndex(e => e.MedalItemId)
                    .HasName("IX_MedalItemID");

                entity.HasIndex(e => e.OrgnizationId)
                    .HasName("IX_OrgnizationID");

                entity.HasIndex(e => e.ReachedItemId)
                    .HasName("IX_ReachedItemID");

                entity.HasIndex(e => e.VerbItemId)
                    .HasName("IX_VerbItemID");

                entity.Property(e => e.VerbItemId).HasDefaultValueSql("((77001))");

                entity.HasOne(d => d.AwardItem)
                    .WithMany(p => p.ProfileAchievementAwardItems)
                    .HasForeignKey(d => d.AwardItemId)
                    .HasConstraintName("FK_dbo.ProfileAchievement_bbsf.LookupItem_AwardItemID");

                entity.HasOne(d => d.ImpactItem)
                    .WithMany(p => p.ProfileAchievementImpactItems)
                    .HasForeignKey(d => d.ImpactItemId)
                    .HasConstraintName("FK_dbo.ProfileAchievement_bbsf.LookupItem_ImpactItemID");

                entity.HasOne(d => d.MedalItem)
                    .WithMany(p => p.ProfileAchievementMedalItems)
                    .HasForeignKey(d => d.MedalItemId)
                    .HasConstraintName("FK_dbo.ProfileAchievement_bbsf.LookupItem_MedalItemID");

                entity.HasOne(d => d.Orgnization)
                    .WithMany(p => p.ProfileAchievements)
                    .HasForeignKey(d => d.OrgnizationId)
                    .HasConstraintName("FK_dbo.ProfileAchievement_dbo.GlpOrganization_OrgnizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileAchievements)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAchievement_dbo.Profile_ProfileID");

                entity.HasOne(d => d.ReachedItem)
                    .WithMany(p => p.ProfileAchievementReachedItems)
                    .HasForeignKey(d => d.ReachedItemId)
                    .HasConstraintName("FK_dbo.ProfileAchievement_bbsf.LookupItem_ReachedItemID");

                entity.HasOne(d => d.VerbItem)
                    .WithMany(p => p.ProfileAchievementVerbItems)
                    .HasForeignKey(d => d.VerbItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAchievement_bbsf.LookupItem_VerbItemID");
            });

            modelBuilder.Entity<ProfileAdminComment>(entity =>
            {
                entity.HasIndex(e => e.AdminId)
                    .HasName("IX_AdminID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.ProfileAdminComments)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAdminComment_bbsf.User_AdminID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileAdminComments)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAdminComment_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileAssessmentToolScore>(entity =>
            {
                entity.Property(e => e.IsCompleted).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.ProfileAssessmentToolScores)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAssessmentToolScore_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileAssessmentToolScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAssessmentToolScore_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileAssignedAssessment>(entity =>
            {
                entity.HasIndex(e => e.AssignedAssessmentId)
                    .HasName("IX_AssignedAssessmentID");

                entity.HasIndex(e => e.ProfileAssessmenttoolId)
                    .HasName("IX_ProfileAssessmenttoolID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.AssignedAssessment)
                    .WithMany(p => p.ProfileAssignedAssessments)
                    .HasForeignKey(d => d.AssignedAssessmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAssignedAssessment_dbo.AssignedAssessment_AssignedAssessmentID");

                entity.HasOne(d => d.ProfileAssessmenttool)
                    .WithMany(p => p.ProfileAssignedAssessments)
                    .HasForeignKey(d => d.ProfileAssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAssignedAssessment_dbo.ProfileAssessmentToolScore_ProfileAssessmenttoolID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileAssignedAssessments)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileAssignedAssessment_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileBatchAssessment>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasName("IX_ApplicationID");

                entity.HasIndex(e => e.BatchId)
                    .HasName("IX_BatchID");

                entity.HasIndex(e => e.ProfileAssessmenttoolId)
                    .HasName("IX_ProfileAssessmenttoolID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ProfileBatchAssessments)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileBatchAssessment_dbo.Application_ApplicationID");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.ProfileBatchAssessments)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileBatchAssessment_dbo.Batch_BatchID");

                entity.HasOne(d => d.ProfileAssessmenttool)
                    .WithMany(p => p.ProfileBatchAssessments)
                    .HasForeignKey(d => d.ProfileAssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileBatchAssessment_dbo.ProfileAssessmentToolScore_ProfileAssessmenttoolID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileBatchAssessments)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileBatchAssessment_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileCompetencyScore>(entity =>
            {
                entity.HasIndex(e => e.AssessmenttoolId)
                    .HasName("IX_AssessmenttoolID");

                entity.HasIndex(e => e.CompetencyId)
                    .HasName("IX_CompetencyID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Assessmenttool)
                    .WithMany(p => p.ProfileCompetencyScores)
                    .HasForeignKey(d => d.AssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileCompetencyScore_dbo.AssessmentTool_AssessmenttoolID");

                entity.HasOne(d => d.Competency)
                    .WithMany(p => p.ProfileCompetencyScores)
                    .HasForeignKey(d => d.CompetencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileCompetencyScore_dbo.Competency_CompetencyID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileCompetencyScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileCompetencyScore_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileEducation>(entity =>
            {
                entity.HasIndex(e => e.FieldOfStudyId)
                    .HasName("IX_FieldOfStudyID");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.ProfileEducations)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileEducation_bbsf.Country_CountryID");

                entity.HasOne(d => d.DegreeItem)
                    .WithMany(p => p.ProfileEducationDegreeItems)
                    .HasForeignKey(d => d.DegreeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileEducation_bbsf.LookupItem_DegreeItemID");

                entity.HasOne(d => d.EmirateItem)
                    .WithMany(p => p.ProfileEducationEmirateItems)
                    .HasForeignKey(d => d.EmirateItemId)
                    .HasConstraintName("FK_dbo.ProfileEducation_bbsf.LookupItem_EmirateItemID");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ProfileEducations)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileEducation_dbo.GlpOrganization_OrganizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileEducations)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileEducation_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileEvent>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventID");

                entity.HasIndex(e => e.EventStatusItemId)
                    .HasName("IX_EventStatusItemID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.ProfileEvents)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Events_dbo.Event_EventID");

                entity.HasOne(d => d.EventStatusItem)
                    .WithMany(p => p.ProfileEvents)
                    .HasForeignKey(d => d.EventStatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Events_bbsf.LookupItem_EventStatusItemID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileEvents)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Events_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileKnowledgeHubCourse>(entity =>
            {
                entity.HasIndex(e => e.CourseId)
                    .HasName("IX_CourseId");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.ProfileCourses)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_KnowledgeHubCourse_dbo.KnowledgeHubCourse_CourseID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileCourses)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_KnowledgeHubCourse_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileFactorScore>(entity =>
            {
                entity.Property(e => e.IsCompleted).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Factor)
                    .WithMany(p => p.ProfileFactorScores)
                    .HasForeignKey(d => d.FactorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileFactorScore_dbo.Factor_FactorID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileFactorScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileFactorScore_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileGroup>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.ProfileGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Groups_dbo.Group_GroupID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileGroups)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Groups_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileGroupAssessment>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupID");

                entity.HasIndex(e => e.ProfileAssessmenttoolId)
                    .HasName("IX_ProfileAssessmenttoolID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.ProfileGroupAssessments)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileGroupAssessment_dbo.AssessmentGroup_GroupID");

                entity.HasOne(d => d.ProfileAssessmenttool)
                    .WithMany(p => p.ProfileGroupAssessments)
                    .HasForeignKey(d => d.ProfileAssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileGroupAssessment_dbo.ProfileAssessmentToolScore_ProfileAssessmenttoolID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileGroupAssessments)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileGroupAssessment_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileInterest>(entity =>
            {
                entity.HasKey(e => new { e.ProfileId, e.Id })
                    .HasName("PK_dbo.Profile_Interest");

                entity.HasIndex(e => e.Id)
                    .HasName("IX_ID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.ProfileInterests)
                    .HasForeignKey(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileClientLookupItem_bbsf.LookupItem_ClientLookupItem_ID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileInterests)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileClientLookupItem_dbo.Profile_Profile_ID");
            });

            modelBuilder.Entity<ProfileLearningPreference>(entity =>
            {
                entity.HasIndex(e => e.LearningPreferenceItemId)
                    .HasName("IX_LearningPreferenceItemID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.LearningPreferenceItem)
                    .WithMany(p => p.ProfileLearningPreferences)
                    .HasForeignKey(d => d.LearningPreferenceItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_LearningPreference_bbsf.LookupItem_LearningPreferenceItemID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileLearningPreferences)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_LearningPreference_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileMeetup>(entity =>
            {
                entity.HasIndex(e => e.MeetupId)
                    .HasName("IX_MeetupID");

                entity.HasIndex(e => e.MeetupStatusItemId)
                    .HasName("IX_MeetupStatusItemID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Meetup)
                    .WithMany(p => p.ProfileMeetups)
                    .HasForeignKey(d => d.MeetupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Meetups_dbo.Meetup_MeetupID");

                entity.HasOne(d => d.MeetupStatusItem)
                    .WithMany(p => p.ProfileMeetups)
                    .HasForeignKey(d => d.MeetupStatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Meetups_bbsf.LookupItem_MeetupStatusItemID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileMeetups)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Profile_Meetups_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileMembership>(entity =>
            {
                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ProfileMemberships)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileMembership_dbo.GlpOrganization_OrganizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileMemberships)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileMembership_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfilePillarScore>(entity =>
            {
                entity.HasIndex(e => e.AssessmenttoolId)
                    .HasName("IX_AssessmenttoolID");

                entity.HasIndex(e => e.PillarId)
                    .HasName("IX_PillarID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.Assessmenttool)
                    .WithMany(p => p.ProfilePillarScores)
                    .HasForeignKey(d => d.AssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfilePillarScore_dbo.AssessmentTool_AssessmenttoolID");

                entity.HasOne(d => d.Pillar)
                    .WithMany(p => p.ProfilePillarScores)
                    .HasForeignKey(d => d.PillarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfilePillarScore_dbo.Pillar_PillarID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfilePillarScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfilePillarScore_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileQuestionItemScore>(entity =>
            {
                entity.HasIndex(e => e.AnswerId)
                    .HasName("IX_AnswerID");

                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.ProfileQuestionItemScores)
                    .HasForeignKey(d => d.AnswerId)
                    .HasConstraintName("FK_dbo.ProfileQuestionItemScore_dbo.QAnswer_AnswerID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.ProfileQuestionItemScores)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileQuestionItemScore_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileQuestionItemScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileQuestionItemScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.QuestionItem)
                    .WithMany(p => p.ProfileQuestionItemScores)
                    .HasForeignKey(d => d.QuestionItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileQuestionItemScore_dbo.QuestionItem_QuestionItemID");
            });

            modelBuilder.Entity<ProfileScaleScore>(entity =>
            {
                entity.Property(e => e.IsCompleted).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileScaleScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileScaleScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.Scale)
                    .WithMany(p => p.ProfileScaleScores)
                    .HasForeignKey(d => d.ScaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileScaleScore_dbo.Scale_ScaleID");
            });

            modelBuilder.Entity<ProfileSubAssessmentToolScore>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.SubAssessmenttoolId)
                    .HasName("IX_SubAssessmenttoolID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.ProfileSubAssessmentToolScores)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileSubAssessmentToolScore_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileSubAssessmentToolScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileSubAssessmentToolScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.SubAssessmenttool)
                    .WithMany(p => p.ProfileSubAssessmentToolScores)
                    .HasForeignKey(d => d.SubAssessmenttoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileSubAssessmentToolScore_dbo.SubAssessmentTool_SubAssessmenttoolID");
            });

            modelBuilder.Entity<ProfileSubScaleScore>(entity =>
            {
                entity.HasIndex(e => e.CompetencyId)
                    .HasName("IX_CompetencyID");

                entity.Property(e => e.IsCompleted).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Competency)
                    .WithMany(p => p.ProfileSubScaleScores)
                    .HasForeignKey(d => d.CompetencyId)
                    .HasConstraintName("FK_dbo.ProfileSubScaleScore_dbo.Competency_CompetencyID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileSubScaleScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileSubScaleScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.SubScale)
                    .WithMany(p => p.ProfileSubScaleScores)
                    .HasForeignKey(d => d.SubScaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileSubScaleScore_dbo.SubScale_SubScaleID");
            });

            modelBuilder.Entity<ProfileTag>(entity =>
            {
                entity.HasKey(e => new { e.ProfileId, e.Id })
                    .HasName("PK_dbo.Profile_Tags");

                entity.HasIndex(e => e.Id)
                    .HasName("IX_ID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.ProfileTags)
                    .HasForeignKey(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileClientLookupItem1_bbsf.LookupItem_ClientLookupItem_ID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileTags)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileClientLookupItem1_dbo.Profile_Profile_ID");
            });

            modelBuilder.Entity<ProfileTraining>(entity =>
            {
                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ProfileTrainings)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileTraining_dbo.GlpOrganization_OrganizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileTrainings)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileTraining_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<ProfileVideoAssessmentAnswerScore>(entity =>
            {
                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.ProfileVideoAssessmentScoreId)
                    .HasName("IX_ProfileVideoAssessmentScoreID");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("IX_QuestionID");

                entity.HasIndex(e => e.VideoAssessmentId)
                    .HasName("IX_VideoAssessmentID");

                entity.HasIndex(e => e.VideoId)
                    .HasName("IX_VideoID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileVideoAssessmentAnswerScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentAnswerScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.ProfileVideoAssessmentScore)
                    .WithMany(p => p.ProfileVideoAssessmentAnswerScores)
                    .HasForeignKey(d => d.ProfileVideoAssessmentScoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentAnswerScore_dbo.ProfileVideoAssessmentScore_ProfileVideoAssessmentScoreID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ProfileVideoAssessmentAnswerScores)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentAnswerScore_dbo.VideoAssessmentQuestion_QuestionID");

                entity.HasOne(d => d.VideoAssessment)
                    .WithMany(p => p.ProfileVideoAssessmentAnswerScores)
                    .HasForeignKey(d => d.VideoAssessmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentAnswerScore_dbo.VideoAssessment_VideoAssessmentID");

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.ProfileVideoAssessmentAnswerScores)
                    .HasForeignKey(d => d.VideoId)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentAnswerScore_bbsf.File_VideoID");
            });

            modelBuilder.Entity<ProfileVideoAssessmentCriteriaScore>(entity =>
            {
                entity.HasIndex(e => e.CriteriaId)
                    .HasName("IX_CriteriaID");

                entity.HasIndex(e => e.CriterionId)
                    .HasName("IX_CriterionID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.ProfileVideoAssessmentScoreId)
                    .HasName("IX_ProfileVideoAssessmentScoreID");

                entity.HasIndex(e => e.VideoAssessmentId)
                    .HasName("IX_VideoAssessmentID");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.ProfileVideoAssessmentCriteriaScores)
                    .HasForeignKey(d => d.CriteriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentCriteriaScore_dbo.Criteria_CriteriaID");

                entity.HasOne(d => d.Criterion)
                    .WithMany(p => p.ProfileVideoAssessmentCriteriaScores)
                    .HasForeignKey(d => d.CriterionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentCriteriaScore_dbo.Criterion_CriterionID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileVideoAssessmentCriteriaScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentCriteriaScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.ProfileVideoAssessmentScore)
                    .WithMany(p => p.ProfileVideoAssessmentCriteriaScores)
                    .HasForeignKey(d => d.ProfileVideoAssessmentScoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentCriteriaScore_dbo.ProfileVideoAssessmentScore_ProfileVideoAssessmentScoreID");

                entity.HasOne(d => d.VideoAssessment)
                    .WithMany(p => p.ProfileVideoAssessmentCriteriaScores)
                    .HasForeignKey(d => d.VideoAssessmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentCriteriaScore_dbo.VideoAssessment_VideoAssessmentID");
            });

            modelBuilder.Entity<ProfileVideoAssessmentScore>(entity =>
            {
                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.VideoAssessmentId)
                    .HasName("IX_VideoAssessmentID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileVideoAssessmentScores)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentScore_dbo.Profile_ProfileID");

                entity.HasOne(d => d.VideoAssessment)
                    .WithMany(p => p.ProfileVideoAssessmentScores)
                    .HasForeignKey(d => d.VideoAssessmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileVideoAssessmentScore_dbo.VideoAssessment_VideoAssessmentID");
            });

            modelBuilder.Entity<ProfileWorkExperience>(entity =>
            {
                entity.HasIndex(e => e.TitleId)
                    .HasName("IX_TitleID");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.ProfileWorkExperiences)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileWorkExperience_bbsf.Country_CountryID");

                entity.HasOne(d => d.EmirateItem)
                    .WithMany(p => p.ProfileWorkExperiences)
                    .HasForeignKey(d => d.EmirateItemId)
                    .HasConstraintName("FK_dbo.ProfileWorkExperience_bbsf.LookupItem_EmirateItemID");

                entity.HasOne(d => d.FieldOfwork)
                    .WithMany(p => p.ProfileWorkExperiences)
                    .HasForeignKey(d => d.FieldOfworkId)
                    .HasConstraintName("FK_dbo.ProfileWorkExperience_dbo.WorkField_FieldOFWorkID");

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.ProfileWorkExperiences)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileWorkExperience_dbo.Industry_IndustryID");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ProfileWorkExperiences)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileWorkExperience_dbo.GlpOrganization_OrganizationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileWorkExperiences)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProfileWorkExperience_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<Programme>(entity =>
            {
                entity.HasIndex(e => e.ProgrammeTypeItemId)
                    .HasName("IX_ProgrammeTypeItemID");

                entity.Property(e => e.Order).HasDefaultValueSql("((1))");

                entity.Property(e => e.ShortTitleAr).HasDefaultValueSql("('')");

                entity.Property(e => e.ShortTitleEn).HasDefaultValueSql("('')");

                entity.HasOne(d => d.ProgrammeTypeItem)
                    .WithMany(p => p.Programmes)
                    .HasForeignKey(d => d.ProgrammeTypeItemId)
                    .HasConstraintName("FK_dbo.Programme_bbsf.LookupItem_ProgrammeTypeItemID");
            });

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.Provider_bbsf.Organization_OrganizationID");

                entity.HasOne(d => d.ProviderTypeItem)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.ProviderTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.Provider_bbsf.LookupItem_ProviderTypeItemID");
            });

            modelBuilder.Entity<ProviderAttribute>(entity =>
            {
                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.ProviderAttributes)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.ProviderAttribute_bbsf.Provider_ProviderId");
            });

            modelBuilder.Entity<PublicHoliday>(entity =>
            {
                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryID");

                entity.HasIndex(e => e.ImageFileId)
                    .HasName("IX_ImageFileID");

                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.PublicHolidays)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.PublicHoliday_bbsf.Country_CountryID");

                entity.HasOne(d => d.ImageFile)
                    .WithMany(p => p.PublicHolidays)
                    .HasForeignKey(d => d.ImageFileId)
                    .HasConstraintName("FK_bbsf.PublicHoliday_bbsf.File_ImageFileID");
            });

            modelBuilder.Entity<Qanswer>(entity =>
            {
                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageID");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("IX_QuestionID");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Qanswers)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.QAnswer_bbsf.File_ImageID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Qanswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.QAnswer_dbo.QuestionItem_QuestionID");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(d => d.QuestionTypeItem)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QuestionTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Question_bbsf.LookupItem_QuestionTypeItemID");
            });
            modelBuilder.Entity<QuestionAnswerOption>(entity =>
            {
                entity.HasKey(e => new { e.optionID, e.QuestionanswerID })
                    .HasName("PK_QuestionAnswer_Option");

                entity.HasIndex(e => e.QuestionanswerID)
                    .HasName("IX_QuestionAnswer_Option_QuestionAnswerID");

                entity.HasOne(d => d.Questionanswer)
                    .WithMany(p => p.Questionansweroptions)
                    .HasForeignKey(d => d.QuestionanswerID)
                    .HasConstraintName("FK_QuestionAnswer_Option_QuestionAnswer_QuestionAnswerID");

                entity.HasOne(d => d.option)
                   .WithMany(p => p.Questionansweroptions)
                   .HasForeignKey(d => d.optionID)
                   .HasConstraintName("FK_QuestionAnswer_Option_Option_OptionID");
            });
            modelBuilder.Entity<QuestionAnswer>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasName("IX_ApplicationID");

                entity.HasIndex(e => e.AssessmentReportFeedbackId)
                    .HasName("IX_AssessmentReportFeedbackID");

                entity.HasIndex(e => e.InitiativeId)
                    .HasName("IX_InitiativeID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.SurveyProfileInfoId)
                    .HasName("IX_SurveyProfileInfoID");

                entity.HasOne(d => d.AnswerFile)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.AnswerFileId)
                    .HasConstraintName("FK_dbo.ApplicationQuestionAnswer_bbsf.File_AnswerFileID");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_dbo.ApplicationQuestionAnswer_dbo.Application_ApplicationID");

                entity.HasOne(d => d.AssessmentReportFeedback)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.AssessmentReportFeedbackId)
                    .HasConstraintName("FK_dbo.QuestionAnswer_dbo.AssessmentReportFeedback_AssessmentReportFeedbackID");

                entity.HasOne(d => d.Initiative)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.InitiativeId)
                    .HasConstraintName("FK_dbo.QuestionAnswer_dbo.Initiative_InitiativeID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK_dbo.ApplicationQuestionAnswer_dbo.Profile_ProfileID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ApplicationQuestionAnswer_dbo.Question_QuestionID");

                entity.HasOne(d => d.SelectedOption)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.SelectedOptionId)
                    .HasConstraintName("FK_dbo.ApplicationQuestionAnswer_dbo.Option_SelectedOptionID");

                entity.HasOne(d => d.SurveyProfileInfo)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.SurveyProfileInfoId)
                    .HasConstraintName("FK_dbo.QuestionAnswer_dbo.SurveyProfileInfo_SurveyProfileInfoID");
            });

            modelBuilder.Entity<QuestionGroup>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<QuestionGroupsQuestion>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupID");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("IX_QuestionID");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.QuestionGroupsQuestions)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.QuestionGroupsQuestions_dbo.QuestionGroup_GroupID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionGroupsQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.QuestionGroupsQuestions_dbo.Question_QuestionID");
            });

            modelBuilder.Entity<QuestionHead>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.AssessmentblockId)
                    .HasName("IX_AssessmentblockID");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageID");

                entity.Property(e => e.TitleAr).HasDefaultValueSql("('')");

                entity.Property(e => e.TitleEn).HasDefaultValueSql("('')");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.QuestionHeads)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.QuestionHead_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Assessmentblock)
                    .WithMany(p => p.QuestionHeads)
                    .HasForeignKey(d => d.AssessmentblockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.QuestionHead_dbo.AssessmentBlock_AssessmentblockID");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.QuestionHeads)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.QuestionHead_bbsf.File_ImageID");
            });

            modelBuilder.Entity<QuestionItem>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.CompetencyId)
                    .HasName("IX_CompetencyID");

                entity.HasIndex(e => e.EnglishTestTypeId)
                    .HasName("IX_EnglishTestTypeID");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageID");

                entity.HasIndex(e => e.LevelId)
                    .HasName("IX_LevelID");

                entity.HasIndex(e => e.QuestionheadId)
                    .HasName("IX_QuestionheadID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.QuestionItems)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .HasConstraintName("FK_dbo.QuestionItem_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.Competency)
                    .WithMany(p => p.QuestionItems)
                    .HasForeignKey(d => d.CompetencyId)
                    .HasConstraintName("FK_dbo.QuestionItem_dbo.Competency_CompetencyID");

                entity.HasOne(d => d.EnglishTestType)
                    .WithMany(p => p.QuestionItemEnglishTestTypes)
                    .HasForeignKey(d => d.EnglishTestTypeId)
                    .HasConstraintName("FK_dbo.QuestionItem_bbsf.LookupItem_EnglishTestTypeID");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.QuestionItems)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.QuestionItem_bbsf.File_ImageID");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.QuestionItemLevels)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK_dbo.QuestionItem_bbsf.LookupItem_LevelID");

                entity.HasOne(d => d.Questionhead)
                    .WithMany(p => p.QuestionItems)
                    .HasForeignKey(d => d.QuestionheadId)
                    .HasConstraintName("FK_dbo.QuestionItem_dbo.QuestionHead_QuestionheadID");

                entity.HasOne(d => d.Scale)
                    .WithMany(p => p.QuestionItems)
                    .HasForeignKey(d => d.ScaleId)
                    .HasConstraintName("FK_dbo.QuestionItem_dbo.Scale_ScaleID");

                entity.HasOne(d => d.SubScale)
                    .WithMany(p => p.QuestionItems)
                    .HasForeignKey(d => d.SubScaleId)
                    .HasConstraintName("FK_dbo.QuestionItem_dbo.SubScale_SubScaleID");
            });

            modelBuilder.Entity<Scale>(entity =>
            {
                entity.HasOne(d => d.Factor)
                    .WithMany(p => p.Scales)
                    .HasForeignKey(d => d.FactorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Scale_dbo.Factor_FactorID");
            });

            modelBuilder.Entity<ScopeClaim>(entity =>
            {
                entity.HasIndex(e => e.ScopeId)
                    .HasName("IX_Scope_Id");

                entity.HasOne(d => d.Scope)
                    .WithMany(p => p.ScopeClaims)
                    .HasForeignKey(d => d.ScopeId)
                    .HasConstraintName("FK_dbo.ScopeClaims_dbo.Scopes_Scope_Id");
            });

            modelBuilder.Entity<ScopeSecret>(entity =>
            {
                entity.HasIndex(e => e.ScopeId)
                    .HasName("IX_Scope_Id");

                entity.HasOne(d => d.Scope)
                    .WithMany(p => p.ScopeSecrets)
                    .HasForeignKey(d => d.ScopeId)
                    .HasConstraintName("FK_dbo.ScopeSecrets_dbo.Scopes_Scope_Id");
            });

            modelBuilder.Entity<ScoringApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId)
                    .HasName("PK_dbo.ScoringApplication");

                entity.HasIndex(e => e.ApplicationId)
                    .HasName("IX_ApplicationID");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.ReviewerId)
                    .HasName("IX_ReviewerID");

                entity.Property(e => e.ApplicationId).ValueGeneratedNever();

                entity.Property(e => e.ApplcationTotalScore).HasComputedColumnSql("((((((([SpecilaizedInRareJobsScore]+[SelfContributionScore])+[DirectToOrganizationScore])+[ContributedToAProjectOnUAELevelScore])+[ContributedToAProjectOnEmiratesLevelScore])+[ContributedToAProjectOnGlobalLevelScore])+[ProgramDetailsScore])+[ContributedToASocialActivitiesScore])");

                entity.HasOne(d => d.Application)
                    .WithOne(p => p.ScoringApplication)
                    .HasForeignKey<ScoringApplication>(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ScoringApplication_dbo.Application_ApplicationID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ScoringApplications)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ScoringApplication_dbo.Profile_ProfileID");

                entity.HasOne(d => d.Reviewer)
                    .WithMany(p => p.ScoringApplications)
                    .HasForeignKey(d => d.ReviewerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ScoringApplication_bbsf.User_ReviewerID");
            });

            modelBuilder.Entity<ScoringProfile>(entity =>
            {
                entity.HasKey(e => e.ProfileId)
                    .HasName("PK_dbo.ScoringProfile");

                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.Property(e => e.ProfileId).ValueGeneratedNever();

                entity.Property(e => e.AssessmentTotalScore).HasComputedColumnSql("(([AssessmentPersonalityScore]+[AssessmentEmotionalIntelligenceScore])+[AssessmentWelbeingScore])");

                entity.Property(e => e.EducationTotalScore).HasComputedColumnSql("((((((((([EducationPHDHolderScore]+[EducationPHDHolderFromTop200QSRankScore])+[EducationStudyingPHDScore])+[EducationMasterHolderScore])+[EducationMasterHolderFromTop200QSRankScore])+[EducationStudyingMasterScore])+[EducationBachelorScore])+[EducationBachelorFromTop200QSRankScore])+[EducationHigherDiplomaScore])+[EducationDiplomaScore])");

                entity.Property(e => e.Experience13yearsScore).HasComputedColumnSql("([dbo].[GetExperience1_3YearsScore]([ProfileID]))");

                entity.Property(e => e.Experience47yearsScore).HasComputedColumnSql("([dbo].[GetExperience4_7YearsScore]([ProfileID]))");

                entity.Property(e => e.ExperienceAbove7YearsScore).HasComputedColumnSql("([dbo].[GetExperienceAbove7YearsScore]([ProfileID]))");

                entity.Property(e => e.ProfileTotalScore).HasComputedColumnSql("(((((((((((((((((([EducationPHDHolderScore]+[EducationPHDHolderFromTop200QSRankScore])+[EducationStudyingPHDScore])+[EducationMasterHolderScore])+[EducationMasterHolderFromTop200QSRankScore])+[EducationStudyingMasterScore])+[EducationBachelorScore])+[EducationBachelorFromTop200QSRankScore])+[EducationHigherDiplomaScore])+[EducationDiplomaScore])+[dbo].[GetExperience1_3YearsScore]([ProfileID]))+[dbo].[GetExperience4_7YearsScore]([ProfileID]))+[dbo].[GetExperienceAbove7YearsScore]([ProfileID]))+[ExperienceInMulinationalOrganizationScore])+[ExperienceInBothPublicAndPrivateScore])+[ExperienceInUAEPrioritySectorScore])+[AssessmentPersonalityScore])+[AssessmentEmotionalIntelligenceScore])+[AssessmentWelbeingScore])");

                entity.Property(e => e.WorkExperienceTotalScore).HasComputedColumnSql("((((([dbo].[GetExperience1_3YearsScore]([ProfileID])+[dbo].[GetExperience4_7YearsScore]([ProfileID]))+[dbo].[GetExperienceAbove7YearsScore]([ProfileID]))+[ExperienceInMulinationalOrganizationScore])+[ExperienceInBothPublicAndPrivateScore])+[ExperienceInUAEPrioritySectorScore])");

                entity.HasOne(d => d.Profile)
                    .WithOne(p => p.ScoringProfile)
                    .HasForeignKey<ScoringProfile>(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ScoringProfile_dbo.Profile_ProfileID");
            });

            modelBuilder.Entity<SubAssessmentTool>(entity =>
            {
                entity.HasIndex(e => e.AssessmentToolId)
                    .HasName("IX_AssessmentToolID");

                entity.HasIndex(e => e.SubAssessmentToolTypeId)
                    .HasName("IX_SubAssessmentToolTypeID");

                entity.HasOne(d => d.AssessmentTool)
                    .WithMany(p => p.SubAssessmentTools)
                    .HasForeignKey(d => d.AssessmentToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SubAssessmentTool_dbo.AssessmentTool_AssessmentToolID");

                entity.HasOne(d => d.SubAssessmentToolType)
                    .WithMany(p => p.SubAssessmentTools)
                    .HasForeignKey(d => d.SubAssessmentToolTypeId)
                    .HasConstraintName("FK_dbo.SubAssessmentTool_bbsf.LookupItem_SubAssessmentToolTypeID");
            });

            modelBuilder.Entity<SubScale>(entity =>
            {
                entity.HasOne(d => d.Scale)
                    .WithMany(p => p.SubScales)
                    .HasForeignKey(d => d.ScaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SubScale_dbo.Scale_ScaleID");
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasIndex(e => e.QuestionGroupId)
                    .HasName("IX_QuestionGroupID");

                entity.HasOne(d => d.QuestionGroup)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.QuestionGroupId)
                    .HasConstraintName("FK_dbo.Survey_dbo.QuestionGroup_QuestionGroupID");
            });

            modelBuilder.Entity<SurveyProfileInfo>(entity =>
            {
                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.SurveyId)
                    .HasName("IX_SurveyID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.SurveyProfileInfos)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK_dbo.SurveyProfileInfo_dbo.Profile_ProfileID");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.SurveyProfileInfos)
                    .HasForeignKey(d => d.SurveyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyProfileInfo_dbo.Survey_SurveyID");
            });

            modelBuilder.Entity<SurveyProfileQuestionAnswer>(entity =>
            {
                entity.HasIndex(e => e.ProfileId)
                    .HasName("IX_ProfileID");

                entity.HasIndex(e => e.SurveryQuestionTypeItemId)
                    .HasName("IX_SurveryQuestionTypeItemID");

                entity.HasIndex(e => e.SurveryQusetionId)
                    .HasName("IX_SurveryQusetionID");

                entity.HasIndex(e => e.SurveyId)
                    .HasName("IX_SurveyID");

                entity.HasIndex(e => e.SurveyProfileInfoId)
                    .HasName("IX_SurveyProfileInfoID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.SurveyProfileQuestionAnswers)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK_dbo.SurveyProfileQuestionAnswer_dbo.Profile_ProfileID");

                entity.HasOne(d => d.SurveryQuestionTypeItem)
                    .WithMany(p => p.SurveyProfileQuestionAnswers)
                    .HasForeignKey(d => d.SurveryQuestionTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyProfileQuestionAnswer_bbsf.LookupItem_SurveryQuestionTypeItemID");

                entity.HasOne(d => d.SurveryQusetion)
                    .WithMany(p => p.SurveyProfileQuestionAnswers)
                    .HasForeignKey(d => d.SurveryQusetionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyProfileQuestionAnswer_dbo.SurveyQuestion_SurveryQusetionID");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.SurveyProfileQuestionAnswers)
                    .HasForeignKey(d => d.SurveyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyProfileQuestionAnswer_dbo.Survey_SurveyID");

                entity.HasOne(d => d.SurveyProfileInfo)
                    .WithMany(p => p.SurveyProfileQuestionAnswers)
                    .HasForeignKey(d => d.SurveyProfileInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyProfileQuestionAnswer_dbo.SurveyProfileInfo_SurveyProfileInfoID");
            });

            modelBuilder.Entity<SurveyProfileQuestionAnswerSurveyQuestionField>(entity =>
            {
                entity.HasKey(e => new { e.SurveyProfileQuestionAnswerId, e.SurveyQuestionFieldId })
                    .HasName("PK_dbo.SurveyProfileQuestionAnswer_SurveyQuestionField");

                entity.HasIndex(e => e.SurveyProfileQuestionAnswerId)
                    .HasName("IX_SurveyProfileQuestionAnswer_ID");

                entity.HasIndex(e => e.SurveyQuestionFieldId)
                    .HasName("IX_SurveyQuestionField_ID");

                entity.HasOne(d => d.SurveyProfileQuestionAnswer)
                    .WithMany(p => p.SurveyProfileQuestionAnswerSurveyQuestionFields)
                    .HasForeignKey(d => d.SurveyProfileQuestionAnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveryQuestionAnswer_SurveyQuestionField_dbo.SurveyProfileQuestionAnswer_SurveyProfileQuestionAnswer_ID");

                entity.HasOne(d => d.SurveyQuestionField)
                    .WithMany(p => p.SurveyProfileQuestionAnswerSurveyQuestionFields)
                    .HasForeignKey(d => d.SurveyQuestionFieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveryQuestionAnswer_SurveyQuestionField_dbo.SurveyQuestionField_SurveyQuestionField_ID");
            });

            modelBuilder.Entity<SurveyQuestion>(entity =>
            {
                entity.HasIndex(e => e.QuestionTypeItemId)
                    .HasName("IX_QuestionTypeItemID");

                entity.HasIndex(e => e.SurveyId)
                    .HasName("IX_SurveyID");

                entity.HasOne(d => d.QuestionTypeItem)
                    .WithMany(p => p.SurveyQuestions)
                    .HasForeignKey(d => d.QuestionTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyQuestion_bbsf.LookupItem_QuestionTypeItemID");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.SurveyQuestions)
                    .HasForeignKey(d => d.SurveyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyQuestion_dbo.Survey_SurveyID");
            });

            modelBuilder.Entity<SurveyQuestionField>(entity =>
            {
                entity.HasIndex(e => e.LinkedSurveyQuestionId)
                    .HasName("IX_LinkedSurveyQuestionID");

                entity.HasIndex(e => e.SurveyQuestionId)
                    .HasName("IX_SurveyQuestionID");

                entity.HasOne(d => d.LinkedSurveyQuestion)
                    .WithMany(p => p.SurveyQuestionFieldLinkedSurveyQuestions)
                    .HasForeignKey(d => d.LinkedSurveyQuestionId)
                    .HasConstraintName("FK_dbo.SurveyQuestionField_dbo.SurveyQuestion_LinkedSurveyQuestionID");

                entity.HasOne(d => d.SurveyQuestion)
                    .WithMany(p => p.SurveyQuestionFieldSurveyQuestions)
                    .HasForeignKey(d => d.SurveyQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SurveyQuestionField_dbo.SurveyQuestion_SurveyQuestionID");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.CategoryItem)
                    .WithMany(p => p.Templates)
                    .HasForeignKey(d => d.CategoryItemId)
                    .HasConstraintName("FK_bbsf.Template_bbsf.LookupItem_CategoryItemID");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Templates)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.Template_bbsf.Organization_OrganizationID");
            });

            modelBuilder.Entity<TemplateInfo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(d => d.EmailHeaderAndFooterTemplate)
                    .WithMany(p => p.TemplateInfos)
                    .HasForeignKey(d => d.EmailHeaderAndFooterTemplateId)
                    .HasConstraintName("FK_bbsf.TemplateInfo_bbsf.EmailHeaderAndFooterTemplate_EmailHeaderAndFooterTemplateID");

                entity.HasOne(d => d.LanguageModeItem)
                    .WithMany(p => p.TemplateInfos)
                    .HasForeignKey(d => d.LanguageModeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TemplateInfo_bbsf.LookupItem_LanguageModeItemID");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateInfos)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TemplateInfo_bbsf.Template_TemplateID");
            });

            modelBuilder.Entity<TemplateUnsubscribe>(entity =>
            {
                entity.HasIndex(e => e.TemplateId)
                    .HasName("IX_TemplateID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateUnsubscribes)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TemplateUnsubscribe_bbsf.Template_TemplateID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TemplateUnsubscribes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TemplateUnsubscribe_bbsf.User_UserID");
            });

            modelBuilder.Entity<Tmetask>(entity =>
            {
                entity.HasIndex(e => e.AssignedToUserId)
                    .HasName("IX_AssignedToUserID");

                entity.HasIndex(e => e.CorrelationTypeItemId)
                    .HasName("IX_CorrelationTypeItemID");

                entity.HasIndex(e => e.TaskOutcomeItemId)
                    .HasName("IX_TaskOutcomeItemID");

                entity.HasIndex(e => e.TaskStatusItemId)
                    .HasName("IX_TaskStatusItemID");

                entity.HasIndex(e => e.WfinstanceTaskId)
                    .HasName("IX_WFInstanceTaskID");

                entity.HasOne(d => d.AssignedToUser)
                    .WithMany(p => p.Tmetasks)
                    .HasForeignKey(d => d.AssignedToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TMETask_bbsf.User_AssignedToUserID");

                entity.HasOne(d => d.CorrelationTypeItem)
                    .WithMany(p => p.TmetaskCorrelationTypeItems)
                    .HasForeignKey(d => d.CorrelationTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TMETask_bbsf.LookupItem_CorrelationTypeItemID");

                entity.HasOne(d => d.TaskOutcomeItem)
                    .WithMany(p => p.Tmetasks)
                    .HasForeignKey(d => d.TaskOutcomeItemId)
                    .HasConstraintName("FK_bbsf.TMETask_bbsf.WorkflowActivityOutcome_TaskOutcomeItemID");

                entity.HasOne(d => d.TaskStatusItem)
                    .WithMany(p => p.TmetaskTaskStatusItems)
                    .HasForeignKey(d => d.TaskStatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.TMETask_bbsf.LookupItem_TaskStatusItemID");

                entity.HasOne(d => d.WfinstanceTask)
                    .WithMany(p => p.Tmetasks)
                    .HasForeignKey(d => d.WfinstanceTaskId)
                    .HasConstraintName("FK_bbsf.TMETask_bbsf.WorkflowInstanceTask_WFInstanceTaskID");
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.TokenType })
                    .HasName("PK_dbo.Tokens");
            });

            modelBuilder.Entity<TopicGroup>(entity =>
            {
                entity.HasKey(e => new { e.TopicId, e.GroupId })
                    .HasName("PK_dbo.TopicGroup");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.TopicGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.TopicGroup_dbo.Group_Group_ID");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.TopicGroups)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.TopicGroup_dbo.Topic_Topic_ID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_Group_ID");

                entity.HasIndex(e => e.ManagerUserId)
                    .HasName("IX_ManagerUserID");

                entity.Property(e => e.Discriminator).HasDefaultValueSql("('ClientUser')");

                entity.Property(e => e.LanguageKey).IsUnicode(false);

                entity.Property(e => e.PermissionSetSid).IsUnicode(false);

                entity.Property(e => e.PermissionSid).IsUnicode(false);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.SamAccount).IsUnicode(false);

                entity.Property(e => e.Username).IsUnicode(false);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_bbsf.User_dbo.Group_Group_ID");

                entity.HasOne(d => d.LargeImageFile)
                    .WithMany(p => p.UserLargeImageFiles)
                    .HasForeignKey(d => d.LargeImageFileId)
                    .HasConstraintName("FK_bbsf.User_bbsf.File_LargeImageFileID");

                entity.HasOne(d => d.ManagerUser)
                    .WithMany(p => p.InverseManagerUser)
                    .HasForeignKey(d => d.ManagerUserId)
                    .HasConstraintName("FK_bbsf.User_bbsf.User_ManagerUserID");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.User_bbsf.Organization_OrganizationID");

                entity.HasOne(d => d.OriginalImageFile)
                    .WithMany(p => p.UserOriginalImageFiles)
                    .HasForeignKey(d => d.OriginalImageFileId)
                    .HasConstraintName("FK_bbsf.User_bbsf.File_OriginalImageFileID");

                entity.HasOne(d => d.SmallImageFile)
                    .WithMany(p => p.UserSmallImageFiles)
                    .HasForeignKey(d => d.SmallImageFileId)
                    .HasConstraintName("FK_bbsf.User_bbsf.File_SmallImageFileID");
            });

            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("('')");

                entity.Property(e => e.ModifiedBy).HasDefaultValueSql("('')");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserConnections)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.UserConnection_bbsf.User_UserID");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Mobile).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserInfos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.UserInfo_bbsf.User_UserID");
            });




            modelBuilder.Entity<VidAssesCriterion>(entity =>
            {
                entity.HasIndex(e => e.CriteriaCalculationTypeId)
                    .HasName("IX_CriteriaCalculationTypeID");

                entity.HasOne(d => d.CriteriaCalculationType)
                    .WithMany(p => p.VidAssesCriteria)
                    .HasForeignKey(d => d.CriteriaCalculationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Criteria_bbsf.LookupItem_CriteriaCalculationTypeID");
            });

            modelBuilder.Entity<VideoAssessment>(entity =>
            {
                entity.HasIndex(e => e.CriteriaId)
                    .HasName("IX_CriteriaID");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.VideoAssessments)
                    .HasForeignKey(d => d.CriteriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VideoAssessment_dbo.Criteria_CriteriaID");
            });

            modelBuilder.Entity<VideoAssessmentQuestion>(entity =>
            {
                entity.HasIndex(e => e.VideoAssessmentId)
                    .HasName("IX_VideoAssessmentID");

                entity.HasOne(d => d.VideoAssessment)
                    .WithMany(p => p.VideoAssessmentQuestions)
                    .HasForeignKey(d => d.VideoAssessmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VideoAssessmentQuestion_dbo.VideoAssessment_VideoAssessmentID");
            });

            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName")
                    .IsUnique();

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<WorkflowActivity>(entity =>
            {
                entity.HasIndex(e => e.ActivityTypeItemId)
                    .HasName("IX_ActivityTypeItemID");

                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName");

                entity.HasIndex(e => e.TemplateId)
                    .HasName("IX_TemplateID");

                entity.HasIndex(e => e.WorkflowVersionId)
                    .HasName("IX_WorkflowVersionID");

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.ActivityTypeItem)
                    .WithMany(p => p.WorkflowActivities)
                    .HasForeignKey(d => d.ActivityTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowActivity_bbsf.LookupItem_ActivityTypeItemID");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.WorkflowActivities)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK_bbsf.WorkflowActivity_bbsf.Template_TemplateID");

                entity.HasOne(d => d.WorkflowVersion)
                    .WithMany(p => p.WorkflowActivities)
                    .HasForeignKey(d => d.WorkflowVersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowActivity_bbsf.WorkflowVersion_WorkflowVersionID");
            });

            modelBuilder.Entity<WorkflowActivityOutcome>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => e.NextActivityId)
                    .HasName("IX_NextActivityID");

                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName");

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.WorkflowActivityOutcomeActivities)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowActivityOutcome_bbsf.WorkflowActivity_ActivityID");

                entity.HasOne(d => d.NextActivity)
                    .WithMany(p => p.WorkflowActivityOutcomeNextActivities)
                    .HasForeignKey(d => d.NextActivityId)
                    .HasConstraintName("FK_bbsf.WorkflowActivityOutcome_bbsf.WorkflowActivity_NextActivityID");
            });

            modelBuilder.Entity<WorkflowActivityVariable>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => e.VariableId)
                    .HasName("IX_VariableID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.WorkflowActivityVariables)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowActivityVariable_bbsf.WorkflowActivity_ActivityID");

                entity.HasOne(d => d.Variable)
                    .WithMany(p => p.WorkflowActivityVariables)
                    .HasForeignKey(d => d.VariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowActivityVariable_bbsf.WorkflowVariable_VariableID");
            });

            modelBuilder.Entity<WorkflowError>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => e.InstanceId)
                    .HasName("IX_InstanceID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.WorkflowErrors)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowError_bbsf.WorkflowActivity_ActivityID");

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.WorkflowErrors)
                    .HasForeignKey(d => d.InstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowError_bbsf.WorkflowInstance_InstanceID");
            });

            modelBuilder.Entity<WorkflowInstance>(entity =>
            {
                entity.HasIndex(e => e.CurrentActivityId)
                    .HasName("IX_CurrentActivityID");

                entity.HasIndex(e => e.InstanceStatusItemId)
                    .HasName("IX_InstanceStatusItemID");

                entity.HasIndex(e => e.StartedByUserId)
                    .HasName("IX_StartedByUserID");

                entity.HasIndex(e => e.WorkflowVersionId)
                    .HasName("IX_WorkflowVersionID");

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.CurrentActivity)
                    .WithMany(p => p.WorkflowInstances)
                    .HasForeignKey(d => d.CurrentActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstance_bbsf.WorkflowActivity_CurrentActivityID");

                entity.HasOne(d => d.InstanceStatusItem)
                    .WithMany(p => p.WorkflowInstances)
                    .HasForeignKey(d => d.InstanceStatusItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstance_bbsf.LookupItem_InstanceStatusItemID");

                entity.HasOne(d => d.StartedByUser)
                    .WithMany(p => p.WorkflowInstances)
                    .HasForeignKey(d => d.StartedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstance_bbsf.User_StartedByUserID");

                entity.HasOne(d => d.WorkflowVersion)
                    .WithMany(p => p.WorkflowInstances)
                    .HasForeignKey(d => d.WorkflowVersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstance_bbsf.WorkflowVersion_WorkflowVersionID");
            });

            modelBuilder.Entity<WorkflowInstanceComment>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => e.InstanceId)
                    .HasName("IX_InstanceID");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.WorkflowInstanceComments)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceComment_bbsf.WorkflowActivity_ActivityID");

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.WorkflowInstanceComments)
                    .HasForeignKey(d => d.InstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceComment_bbsf.WorkflowInstance_InstanceID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WorkflowInstanceComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceComment_bbsf.User_UserID");
            });

            modelBuilder.Entity<WorkflowInstanceTask>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => e.WorkflowInstanceId)
                    .HasName("IX_WorkflowInstanceID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.WorkflowInstanceTasks)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceTask_bbsf.WorkflowActivity_ActivityID");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.WorkflowInstanceTasks)
                    .HasForeignKey(d => d.WorkflowInstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceTask_bbsf.WorkflowInstance_WorkflowInstanceID");
            });

            modelBuilder.Entity<WorkflowInstanceVariable>(entity =>
            {
                entity.HasIndex(e => e.InstanceId)
                    .HasName("IX_InstanceID");

                entity.HasIndex(e => e.VariableId)
                    .HasName("IX_VariableID");

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.WorkflowInstanceVariables)
                    .HasForeignKey(d => d.InstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceVariable_bbsf.WorkflowInstance_InstanceID");

                entity.HasOne(d => d.Variable)
                    .WithMany(p => p.WorkflowInstanceVariables)
                    .HasForeignKey(d => d.VariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceVariable_bbsf.WorkflowVariable_VariableID");
            });

            modelBuilder.Entity<WorkflowInstanceVariableVariableLookup>(entity =>
            {
                entity.HasKey(e => new { e.VariableLookupId, e.InstanceVariableId })
                    .HasName("PK_bbsf.WorkflowInstanceVariable_VariableLookup");

                entity.HasIndex(e => e.InstanceVariableId)
                    .HasName("IX_InstanceVariableID");

                entity.HasIndex(e => e.VariableLookupId)
                    .HasName("IX_VariableLookupID");

                entity.HasOne(d => d.InstanceVariable)
                    .WithMany(p => p.WorkflowInstanceVariableVariableLookups)
                    .HasForeignKey(d => d.InstanceVariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceVariable_VariableLookup_bbsf.WorkflowVariableLookup_InstanceVariableID");

                entity.HasOne(d => d.VariableLookup)
                    .WithMany(p => p.WorkflowInstanceVariableVariableLookups)
                    .HasForeignKey(d => d.VariableLookupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowInstanceVariable_VariableLookup_bbsf.WorkflowInstanceVariable_VariableLookupID");
            });

            modelBuilder.Entity<WorkflowLog>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => e.ActivityOutcomeId)
                    .HasName("IX_ActivityOutcomeID");

                entity.HasIndex(e => e.CompletedByUserId)
                    .HasName("IX_CompletedByUserID");

                entity.HasIndex(e => e.InstanceId)
                    .HasName("IX_InstanceID");

                entity.HasIndex(e => e.WorkflowVersionId)
                    .HasName("IX_WorkflowVersionID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.WorkflowLogs)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowLog_bbsf.WorkflowActivity_ActivityID");

                entity.HasOne(d => d.ActivityOutcome)
                    .WithMany(p => p.WorkflowLogs)
                    .HasForeignKey(d => d.ActivityOutcomeId)
                    .HasConstraintName("FK_bbsf.WorkflowLog_bbsf.WorkflowActivityOutcome_ActivityOutcomeID");

                entity.HasOne(d => d.CompletedByUser)
                    .WithMany(p => p.WorkflowLogs)
                    .HasForeignKey(d => d.CompletedByUserId)
                    .HasConstraintName("FK_bbsf.WorkflowLog_bbsf.User_CompletedByUserID");

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.WorkflowLogs)
                    .HasForeignKey(d => d.InstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowLog_bbsf.WorkflowInstance_InstanceID");

                entity.HasOne(d => d.WorkflowVersion)
                    .WithMany(p => p.WorkflowLogs)
                    .HasForeignKey(d => d.WorkflowVersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowLog_bbsf.WorkflowVersion_WorkflowVersionID");
            });

            modelBuilder.Entity<WorkflowLogVariable>(entity =>
            {
                entity.HasIndex(e => e.ActivityVariableId)
                    .HasName("IX_ActivityVariableID");

                entity.HasIndex(e => e.LogId)
                    .HasName("IX_LogID");

                entity.HasOne(d => d.ActivityVariable)
                    .WithMany(p => p.WorkflowLogVariables)
                    .HasForeignKey(d => d.ActivityVariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowLogVariable_bbsf.WorkflowActivityVariable_ActivityVariableID");

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.WorkflowLogVariables)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowLogVariable_bbsf.WorkflowLog_LogID");
            });

            modelBuilder.Entity<WorkflowVariable>(entity =>
            {
                entity.HasIndex(e => e.DataTypeItemId)
                    .HasName("IX_DataTypeItemID");

                entity.HasIndex(e => e.SysName)
                    .HasName("IX_SysName");

                entity.HasIndex(e => e.ValueIdentifier)
                    .HasName("IX_Value_Identifier");

                entity.HasIndex(e => e.VariableTypeItemId)
                    .HasName("IX_VariableTypeItemID");

                entity.HasIndex(e => e.WorkflowVersionId)
                    .HasName("IX_WorkflowVersionID");

                entity.Property(e => e.SysName).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.DataTypeItem)
                    .WithMany(p => p.WorkflowVariableDataTypeItems)
                    .HasForeignKey(d => d.DataTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowVariable_bbsf.LookupItem_DataTypeItemID");

                entity.HasOne(d => d.VariableTypeItem)
                    .WithMany(p => p.WorkflowVariableVariableTypeItems)
                    .HasForeignKey(d => d.VariableTypeItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowVariable_bbsf.LookupItem_VariableTypeItemID");

                entity.HasOne(d => d.WorkflowVersion)
                    .WithMany(p => p.WorkflowVariables)
                    .HasForeignKey(d => d.WorkflowVersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowVariable_bbsf.WorkflowVersion_WorkflowVersionID");
            });

            modelBuilder.Entity<WorkflowVariableLookup>(entity =>
            {
                entity.HasIndex(e => e.VariableId)
                    .HasName("IX_VariableID");

                entity.HasOne(d => d.Variable)
                    .WithMany(p => p.WorkflowVariableLookups)
                    .HasForeignKey(d => d.VariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowVariableLookup_bbsf.WorkflowVariable_VariableID");
            });

            modelBuilder.Entity<WorkflowVersion>(entity =>
            {
                entity.HasIndex(e => e.WorkflowId)
                    .HasName("IX_WorkflowID");

                entity.HasOne(d => d.Workflow)
                    .WithMany(p => p.WorkflowVersions)
                    .HasForeignKey(d => d.WorkflowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bbsf.WorkflowVersion_bbsf.Workflow_WorkflowID");
            });

            modelBuilder.Entity<ReportAssessmentOverview>(entity =>
            {
                entity.HasIndex(e => e.TabItemId)
                    .HasName("IX_TabItemID");
            });

            modelBuilder.Entity<ReportAssessmentPercentageOverview>(entity =>
            {
                entity.HasIndex(e => e.TabItemId)
                    .HasName("IX_TabItemID");
            });


            modelBuilder.Entity<RecommendationFitDetails>(entity =>
            {
                entity.HasOne(d => d.LookupRecommendFit)
                    .WithMany(p => p.RecommendationFitItem)
                    .HasForeignKey(d => d.RecommendationFitItemId)
                    .HasConstraintName("FK_RecommendationSubmission_RecommendationFitLookupItems_LookupItem_RecommendationFitItemId");

                entity.HasOne(d => d.RecommendLeadr)
                    .WithMany(p => p.RecommendationFitDetail)
                    .HasForeignKey(d => d.RecommendID)
                    .HasConstraintName("FK_RecommendationSubmission_RecommendationFitLookupItems_RecommendationSubmissions_RecommendID");
            });

            modelBuilder.Entity<RecommendationCallback>(entity =>
            {
                entity.HasOne(d => d.RecommendLeadr)
                    .WithMany(p => p.RecommendationCallbacks)
                    .HasForeignKey(d => d.RecommendID)
                    .HasConstraintName("FK_RecommendationCallbacks_RecommendationSubmissions_RecommendID");
            });

            modelBuilder.Entity<RecommendLeadr>(entity =>
            {
                entity.HasOne(d => d.LookupRecommendSourceFit)
                    .WithMany(p => p.RecommendationSourceItem)
                    .HasForeignKey(d => d.SourceItemID)
                    .HasConstraintName("FK_RecommendationSubmissions_LookupItem_SourceItemID");

                entity.HasOne(d => d.LookupRecommendStatusItem)
                    .WithMany(p => p.RecommendationStatusItem)
                    .HasForeignKey(d => d.StatusItemID)
                    .HasConstraintName("FK_RecommendationSubmissions_LookupItem_StatusItemID");

                entity.HasOne(d => d.LookupRecommendProfileRecommended)
                    .WithMany(p => p.RecommendationProfileRecommended)
                    .HasForeignKey(d => d.RecommendedProfileID)
                    .HasConstraintName("FK_RecommendationSubmissions_Profile_ProfileID_Recommended");

                entity.HasOne(d => d.LookupRecommendProfileRecommender)
                    .WithMany(p => p.RecommendationProfileRecommender)
                    .HasForeignKey(d => d.RecommenderProfileID)
                    .HasConstraintName("FK_RecommendationSubmissions_Profile_ProfileID_Recommender");
            });

            modelBuilder.Entity<ProfileBatchSelectedAlumni>()
                .HasKey(c => new { c.BatchID, c.ProfileID});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}