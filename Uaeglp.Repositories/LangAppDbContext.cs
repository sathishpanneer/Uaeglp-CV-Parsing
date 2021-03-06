// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Uaeglp.LangModel;
using Uaeglp.Utilities;
using Microsoft.Extensions.Logging;
using NLog;

namespace Uaeglp.Repositories
{
    public partial class LangAppDbContext : DbContext
    {
        private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly string _appSettings;
        public LangAppDbContext(string appSettings)
        {
            _appSettings = appSettings;
        }

        public LangAppDbContext(DbContextOptions<LangAppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<DevRegister> DevRegisters { get; set; }
        public virtual DbSet<DeviceDwell> DeviceDwells { get; set; }
        public virtual DbSet<Helpscreen> Helpscreens { get; set; }
        public virtual DbSet<LanguageQuestions> LanguageQuestions { get; set; }
        public virtual DbSet<Languages_Label> Languages_Label { get; set; }
        public virtual DbSet<LanguagesLabelBak> LanguagesLabelBaks { get; set; }
        public virtual DbSet<LogBugs> LogBugs { get; set; }
        public virtual DbSet<MasterAboutus> MasterAboutus { get; set; }
        public virtual DbSet<MasterCity> MasterCities { get; set; }
        public virtual DbSet<MasterEntity> MasterEntities { get; set; }
        public virtual DbSet<MasterFaq> MasterFaqs { get; set; }
        public virtual DbSet<MasterLanguages> MasterLanguages { get; set; }
        public virtual DbSet<MasterNotification> MasterNotifications { get; set; }
        public virtual DbSet<MasterServicecenter> MasterServicecenters { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<NotificationCenter> NotificationCenters { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Settings2> Settings2s { get; set; }
        public virtual DbSet<SettingsComments> SettingsComments { get; set; }
        public virtual DbSet<SettingsLabel> SettingsLabels { get; set; }
        public virtual DbSet<SurveyAnswer> SurveyAnswers { get; set; }
        public virtual DbSet<SurveyAnswerLanguages> SurveyAnswerLanguages { get; set; }
        public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public virtual DbSet<SysAccessMaster> SysAccessMasters { get; set; }
        public virtual DbSet<SysRoleMaster> SysRoleMasters { get; set; }
        public virtual DbSet<TempFeedback> TempFeedbacks { get; set; }
        public virtual DbSet<TempFeedbackstates> TempFeedbackstates { get; set; }
        public virtual DbSet<UserAnswer> UserAnswers { get; set; }
        public virtual DbSet<UserComments> UserComments { get; set; }
        public virtual DbSet<UserCurrentlocation> UserCurrentlocations { get; set; }
        public virtual DbSet<UserFeedback> UserFeedbacks { get; set; }
        public virtual DbSet<UserFeedbackscreens> UserFeedbackscreens { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<VwSurvey> VwSurveys { get; set; }
        public virtual DbSet<VwSurveyanswers> VwSurveyanswers { get; set; }
        public virtual DbSet<WebUsers> WebUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  DBConnectionString : {_appSettings}");
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer("Data Source=10.200.2.116;Initial Catalog=GLP_LOPA;Persist Security Info=True;User ID=Sa;Password=Sw0rd@2020");
                optionsBuilder.UseSqlServer(_appSettings);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DevRegister>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Notification).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DeviceDwell>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Helpscreen>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Language).IsFixedLength();
            });

            modelBuilder.Entity<LanguageQuestions>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsActive1).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Languages_Label>(entity =>
            {
                entity.Property(e => e.Is_Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Language_Code).IsFixedLength();
            });

            modelBuilder.Entity<LanguagesLabelBak>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LanguageCode).IsFixedLength();
            });

            modelBuilder.Entity<LogBugs>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasterAboutus>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Language).IsFixedLength();
            });

            modelBuilder.Entity<MasterEntity>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasterFaq>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Language).IsFixedLength();
            });

            modelBuilder.Entity<MasterLanguages>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Language).IsFixedLength();
            });

            modelBuilder.Entity<MasterNotification>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MasterServicecenter>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Latitude).HasDefaultValueSql("((0.0))");

                entity.Property(e => e.Longitude).HasDefaultValueSql("((0.0))");
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Deviceid).IsUnicode(false);

                entity.Property(e => e.Devicename).IsUnicode(false);

                entity.Property(e => e.Devicetoken).IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Ipaddress).IsUnicode(false);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<NotificationCenter>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Settings2>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Type).IsFixedLength();
            });

            modelBuilder.Entity<SettingsComments>(entity =>
            {
                entity.Property(e => e.LanguageCode).IsFixedLength();
            });

            modelBuilder.Entity<SettingsLabel>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SurveyAnswer>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Sequence).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SurveyAnswerLanguages>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SurveyQuestion>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SysAccessMaster>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysRoleMaster>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AccessIds).IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedBy).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TempFeedback>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TempFeedbackstates>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserAnswer>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserComments>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserCurrentlocation>(entity =>
            {
                entity.Property(e => e.CreatedBy).IsFixedLength();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedBy).IsFixedLength();
            });

            modelBuilder.Entity<UserFeedback>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<VwSurvey>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_survey");
            });

            modelBuilder.Entity<VwSurveyanswers>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_surveyanswers");
            });

            modelBuilder.Entity<WebUsers>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsSuperadmin).HasDefaultValueSql("((0))");

                entity.Property(e => e.RoleId).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}