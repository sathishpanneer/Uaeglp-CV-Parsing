// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("User", Schema = "bbsf")]
    public partial class User
    {
        public User()
        {
            Announcements = new HashSet<Announcement>();
            ApplicationCycleLogs = new HashSet<ApplicationCycleLog>();
            AssessmentReportFeedbacks = new HashSet<AssessmentReportFeedback>();
            BadgeRequests = new HashSet<BadgeRequest>();
            ChatMessageSeenBies = new HashSet<ChatMessageSeenBy>();
            ChatMessages = new HashSet<ChatMessage>();
            ChatRoomUsers = new HashSet<ChatRoomUser>();
            ChatUnreadMessages = new HashSet<ChatUnreadMessage>();
            Configurations = new HashSet<Configuration>();
            DirectorySearchHistories = new HashSet<DirectorySearchHistory>();
            FormsHubConnections = new HashSet<FormsHubConnection>();
            GlppermissionClientUsers = new HashSet<GlppermissionClientUser>();
            GovernmentEntityCoordinators = new HashSet<GovernmentEntityCoordinator>();
            InverseManagerUser = new HashSet<User>();
            Meetups = new HashSet<Meetup>();
            Participants = new HashSet<Participant>();
            PermissionSetUsers = new HashSet<PermissionSetUser>();
            ProfileAdminComments = new HashSet<ProfileAdminComment>();
            ScoringApplications = new HashSet<ScoringApplication>();
            TemplateUnsubscribes = new HashSet<TemplateUnsubscribe>();
            Tmetasks = new HashSet<Tmetask>();
            UserConnections = new HashSet<UserConnection>();
            UserInfos = new HashSet<UserInfo>();
            UserDeviceInfos = new HashSet<UserDeviceInfo>();
            WorkflowInstanceComments = new HashSet<WorkflowInstanceComment>();
            WorkflowInstances = new HashSet<WorkflowInstance>();
            WorkflowLogs = new HashSet<WorkflowLog>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("OrganizationID")]
        public int OrganizationId { get; set; }
        [Required]
        [StringLength(256)]
        public string Username { get; set; }
        [StringLength(256)]
        public string SamAccount { get; set; }
        [StringLength(2)]
        public string LanguageKey { get; set; }
        [Column("OriginalImageFileID")]
        public int? OriginalImageFileId { get; set; }
        [Column("LargeImageFileID")]
        public int? LargeImageFileId { get; set; }
        [Column("SmallImageFileID")]
        public int? SmallImageFileId { get; set; }
        [Column("PermissionSID")]
        [StringLength(2000)]
        public string PermissionSid { get; set; }
        [Column("PermissionSetSID")]
        [StringLength(2000)]
        public string PermissionSetSid { get; set; }
        [Column("NameEN")]
        [StringLength(100)]
        public string NameEn { get; set; }
        [Column("NameAR")]
        [StringLength(100)]
        public string NameAr { get; set; }
        [Column("TitleEN")]
        [StringLength(100)]
        public string TitleEn { get; set; }
        [Column("TitleAR")]
        [StringLength(100)]
        public string TitleAr { get; set; }
        [Column("GenderItemID")]
        public int? GenderItemId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }
        [Required]
        public byte[] RowVersion { get; set; }
        [Required]
        [StringLength(128)]
        public string Discriminator { get; set; }
        public byte[] License { get; set; }
        public bool IsOnline { get; set; }
        [Column("LastUsedPermissionSetID")]
        public int? LastUsedPermissionSetId { get; set; }
        [StringLength(4000)]
        public string Department { get; set; }
        [Column("ManagerUserID")]
        public int? ManagerUserId { get; set; }
        [Column("Group_ID")]
        public int? GroupId { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsTC_Accepted { get; set; }


        [ForeignKey(nameof(GroupId))]
        [InverseProperty("Users")]
        public virtual Group Group { get; set; }
        [ForeignKey(nameof(LargeImageFileId))]
        [InverseProperty(nameof(File.UserLargeImageFiles))]
        public virtual File LargeImageFile { get; set; }
        [ForeignKey(nameof(ManagerUserId))]
        [InverseProperty(nameof(User.InverseManagerUser))]
        public virtual User ManagerUser { get; set; }
        [ForeignKey(nameof(OrganizationId))]
        [InverseProperty("Users")]
        public virtual Organization Organization { get; set; }
        [ForeignKey(nameof(OriginalImageFileId))]
        [InverseProperty(nameof(File.UserOriginalImageFiles))]
        public virtual File OriginalImageFile { get; set; }
        [ForeignKey(nameof(SmallImageFileId))]
        [InverseProperty(nameof(File.UserSmallImageFiles))]
        public virtual File SmallImageFile { get; set; }
        [InverseProperty("IdNavigation")]
        public virtual Profile Profile { get; set; }
        [InverseProperty(nameof(Announcement.User))]
        public virtual ICollection<Announcement> Announcements { get; set; }
        [InverseProperty(nameof(ApplicationCycleLog.User))]
        public virtual ICollection<ApplicationCycleLog> ApplicationCycleLogs { get; set; }
        [InverseProperty(nameof(AssessmentReportFeedback.CreatedUser))]
        public virtual ICollection<AssessmentReportFeedback> AssessmentReportFeedbacks { get; set; }
        [InverseProperty(nameof(BadgeRequest.Profile))]
        public virtual ICollection<BadgeRequest> BadgeRequests { get; set; }
        [InverseProperty(nameof(ChatMessageSeenBy.User))]
        public virtual ICollection<ChatMessageSeenBy> ChatMessageSeenBies { get; set; }
        [InverseProperty(nameof(ChatMessage.Owner))]
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        [InverseProperty(nameof(ChatRoomUser.User))]
        public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; }
        [InverseProperty(nameof(ChatUnreadMessage.User))]
        public virtual ICollection<ChatUnreadMessage> ChatUnreadMessages { get; set; }
        [InverseProperty(nameof(Configuration.User))]
        public virtual ICollection<Configuration> Configurations { get; set; }
        [InverseProperty(nameof(DirectorySearchHistory.User))]
        public virtual ICollection<DirectorySearchHistory> DirectorySearchHistories { get; set; }
        [InverseProperty(nameof(FormsHubConnection.User))]
        public virtual ICollection<FormsHubConnection> FormsHubConnections { get; set; }
        [InverseProperty(nameof(GlppermissionClientUser.ClientUser))]
        public virtual ICollection<GlppermissionClientUser> GlppermissionClientUsers { get; set; }
        [InverseProperty(nameof(GovernmentEntityCoordinator.ClientUser))]
        public virtual ICollection<GovernmentEntityCoordinator> GovernmentEntityCoordinators { get; set; }
        [InverseProperty(nameof(User.ManagerUser))]
        public virtual ICollection<User> InverseManagerUser { get; set; }
        [InverseProperty(nameof(Meetup.Owner))]
        public virtual ICollection<Meetup> Meetups { get; set; }
        [InverseProperty(nameof(Participant.User))]
        public virtual ICollection<Participant> Participants { get; set; }
        [InverseProperty(nameof(PermissionSetUser.User))]
        public virtual ICollection<PermissionSetUser> PermissionSetUsers { get; set; }
        [InverseProperty(nameof(ProfileAdminComment.Admin))]
        public virtual ICollection<ProfileAdminComment> ProfileAdminComments { get; set; }
        [InverseProperty(nameof(ScoringApplication.Reviewer))]
        public virtual ICollection<ScoringApplication> ScoringApplications { get; set; }
        [InverseProperty(nameof(TemplateUnsubscribe.User))]
        public virtual ICollection<TemplateUnsubscribe> TemplateUnsubscribes { get; set; }
        [InverseProperty(nameof(Tmetask.AssignedToUser))]
        public virtual ICollection<Tmetask> Tmetasks { get; set; }
        [InverseProperty(nameof(UserConnection.User))]
        public virtual ICollection<UserConnection> UserConnections { get; set; }
        [InverseProperty(nameof(UserInfo.User))]
        public virtual ICollection<UserInfo> UserInfos { get; set; }

        [InverseProperty(nameof(UserDeviceInfo.User))]
        public virtual ICollection<UserDeviceInfo> UserDeviceInfos { get; set; }

        [InverseProperty(nameof(WorkflowInstanceComment.User))]
        public virtual ICollection<WorkflowInstanceComment> WorkflowInstanceComments { get; set; }
        [InverseProperty(nameof(WorkflowInstance.StartedByUser))]
        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }
        [InverseProperty(nameof(WorkflowLog.CompletedByUser))]
        public virtual ICollection<WorkflowLog> WorkflowLogs { get; set; }
    }
}