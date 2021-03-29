using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Logging;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MoreLinq;
using NLog;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.MongoModels;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.Event;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Services
{
	public class MeetupService : IMeetupService
	{
		
		private readonly AppDbContext _appDbContext;
		private readonly IMapper _mapper;
		private readonly IEncryptionManager _Encryptor;
		private readonly IFileService _fileService;
		private readonly MongoDbContext _mongoDbContext;
		private readonly IPushNotificationService _pushNotificationService;
		private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly IUserIPAddress _userIPAddress;


		public IDictionary<ProfileSectionPercentage, int> ProfileSectionPercentages = new Dictionary<ProfileSectionPercentage, int>()
	{
	  {
		ProfileSectionPercentage.profileImg,
		5
	  },
	  {
		ProfileSectionPercentage.BasicInfoPrcange,
		15
	  },
	  {
		ProfileSectionPercentage.EducationPrcange,
		10
	  },
	  {
		ProfileSectionPercentage.WorkExpeincePrcange,
		25
	  },
	  {
		ProfileSectionPercentage.AchievementPrcange,
		20
	  },
	  {
		ProfileSectionPercentage.TrainingPrcange,
		5
	  },
	  {
		ProfileSectionPercentage.PersonalityAssessment,
		7
	  },
	  {
		ProfileSectionPercentage.EQAssessment,
		6
	  },
	  {
		ProfileSectionPercentage.WellbeingAssessment,
		7
	  }
	};

		public MeetupService(MongoDbContext mongoDbContext, AppDbContext appContext, IEncryptionManager Encryption, IMapper mapper, IFileService fileService, IPushNotificationService pushNotificationService, IUserIPAddress userIPAddress)
		{
			_appDbContext = appContext;
			_mapper = mapper;
			_Encryptor = Encryption;
			_fileService = fileService;
			_mongoDbContext = mongoDbContext;
			_pushNotificationService = pushNotificationService;
			_userIPAddress = userIPAddress;
		}

		#region Manage Groups
		public async Task<IMeetupResponse> GetGroupsByFilters(PagingView pagingView, int userId, string SearchText = null)
		{
			try
			{
				var username = (await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)).Username;

				var GroupPagedView = new GroupPagedView();
				var groups = _appDbContext.Groups.OrderBy(e => e.Id).Select(e => new GroupView
				{
					ID = e.Id,
					Description = new MeetupLangView() { En = e.DescriptionEn, Ar= e.DescriptionAr },
					Name = new MeetupLangView() { En = e.NameEn, Ar = e.NameAr },
					CoverImgID = e.CoverImageFileId,
					IDEncrypted = _Encryptor.Encrypt(e.Id.ToString()),
					CanEdit = e.CreatedBy == username
				});

				foreach (var group in groups)
				{
					group.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == group.ID && m.IsFollowed == true);
					var profileGroup = _appDbContext.ProfileGroups.FirstOrDefault(m => m.GroupId == group.ID && m.ProfileId == userId);
					if (profileGroup != null)
						group.IsFollowed = profileGroup.IsFollowed;
					else
						group.IsFollowed = false;
				}

				if (!string.IsNullOrEmpty(SearchText))
				{
					SearchText = SearchText.ToLower();
					groups = groups.Where(e => e.Name.En.ToLower().Contains(SearchText) || e.Name.Ar.ToLower().Contains(SearchText) || e.Description.En.ToLower().Contains(SearchText) || e.Description.Ar.ToLower().Contains(SearchText));
				}
				pagingView.TotalCount = groups.Count();
				var collection = groups.Skip(pagingView.Skip).Take(pagingView.Take).AsQueryable();
				GroupPagedView.Groups = new List<GroupView>(collection);
				GroupPagedView.pagingView = pagingView;
				return new MeetupResponse(GroupPagedView);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> GetGroups(int userId,int Take)
		{
			try
			{
				List<MeetupPagedView> _views = new List<MeetupPagedView>();

				var username = (await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)).Username;

				var groups = _appDbContext.Groups.OrderBy(e => e.Id).ToList();
				foreach (var item in groups)
				{
					_views.Add(await GetMeetups(item.Id, userId, 0, Take));
				}
				return new MeetupResponse(_views);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}
		private async Task<MeetupPagedView> GetMeetups(int groupId, int userId,int Skip, int Take)
		{
			try
			{

				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
				int? usedPermissionSetId = user.LastUsedPermissionSetId;
				int num1 = 1;

				Expression<Func<Meetup, bool>> predicate;
				Expression<Func<Meetup, bool>> predicate2;
				//if (usedPermissionSetId.GetValueOrDefault() == num1 & usedPermissionSetId.HasValue)
				//{
					//predicate = (i => i.GroupId == groupId && i.Date.Month == DateTime.Now.Month);
					//predicate2 = (i => i.GroupId == groupId);

				//}

				//else
				//{
					predicate = (i => i.GroupId == groupId && i.Date.Month == DateTime.Now.Month && (i.StatusItemId == 76001 || i.OwnerId == user.Id));
					predicate2 = (i => i.GroupId == groupId && (i.StatusItemId == 76001 || i.OwnerId == user.Id));
				//}
					

				var list1 = _appDbContext.Meetups.Where(predicate).OrderBy(m => m.Date).ToList();
				var listCount = _appDbContext.Meetups.Where(predicate2).OrderBy(m => m.Date).ToList();
				var source1 = new List<MeetupView>();
				foreach (var data in list1)
				{
					var item = data;
					MeetupView meetupView = new MeetupView();
					meetupView.ID = item.Id;
					meetupView.Date = item.Date;
					meetupView.Descriotion = item.Description;
					meetupView.Title = item.Title;
					meetupView.IDEncrypted = _Encryptor.Encrypt(item.Id.ToString());
					meetupView.IsOwner = item.CreatedBy == user.Username;
					meetupView.StatusItemID = item.StatusItemId;
					meetupView.DisplayAdminDecision = item.StatusItemId == 76001 && item.CreatedBy == user.Username;
					meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(m => m.MeetupId == item.Id && m.ProfileId == userId)?.MeetupStatusItemId;
					if (item.StatusItemId != null && item.StatusItemId > 0)
					{
						var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 76 && k.Id == item.StatusItemId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
						meetupView.StatusItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
					}
					//meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(x => x.MeetupId == item.Id && x.ProfileId == userId)?.MeetupStatusItemId;
					if (meetupView.ProfileDesicionID != null && meetupView.ProfileDesicionID > 0)
					{
						var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 69 && k.Id == meetupView.ProfileDesicionID).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
						meetupView.ProfileDesicionItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
					}
					meetupView.CreatedBY = item.CreatedBy;
					meetupView.StartTime = item.StartTime;
					meetupView.EndTime = item.EndTime;
					meetupView.MeetupPictureId = item.MeetupPictureID;
					var source2 = _appDbContext.ProfileMeetups.Where(m => m.MeetupId == item.Id && m.MeetupStatusItemId == 69001);
					meetupView.NumberOfGoing = source2.ToList().Count();
					meetupView.MapAutocompleteView = new MapAutocompleteView()
					{
						Latitude = item.Latitude,
						Longitude = item.Longitude,
						Text = item.MapText
					};
					var list2 = source2.Select(p => p.ProfileId).ToList();
					var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == item.OwnerId);
					meetupView.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
					meetupView.UserName = new MeetupLangView() { En = clientUser.NameEn, Ar = clientUser.NameAr };
					var fileViewList = new List<ParticipantsView>();
					foreach (int num2 in list2)
					{
						int profileId = num2;
						var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == profileId);
						var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
							.Where(k => k.ProfileId == profileId).OrderByDescending(k => k.DateFrom)
							.FirstOrDefaultAsync();

						int? smallImageFileId = commentUser.SmallImageFileId;
						if (smallImageFileId.HasValue)
						{
							smallImageFileId = commentUser.SmallImageFileId;

						}


						ParticipantsView view = new ParticipantsView();
						view.NameEn = commentUser.NameEn;
						view.NameAr = commentUser.NameAr;
						view.TitleEn = commentUserJob?.Title?.TitleEn;
						view.TitleAr = commentUserJob?.Title?.TitleAr;
						view.Userid = profileId;
						view.ParticipantsImageId = smallImageFileId;
						fileViewList.Add(view);
					}
					meetupView.ParticipantsList = fileViewList;
					if (clientUser.SmallImageFileId.HasValue)
						meetupView.ProfileImageId = clientUser.SmallImageFileId.Value;
					meetupView.Comments = GetMeetupCommentsAsync(meetupView.ID, userId,0,10).Result.CommentView;
					source1.Add(meetupView);
				}
				var pagingView = new PagingView();
				pagingView.TotalCount = listCount.Count();
				pagingView.PageCount = Take;

				MeetupPagedView MeetupPagedView1 = new MeetupPagedView();

				//MeetupPagedView1.MeetupList = source1.Skip(pagingView.Skip).Take(pagingView.Take).ToList();
				MeetupPagedView1.MeetupList = source1.ToList();
				MeetupPagedView1.pagingView = pagingView;
				Group group = _appDbContext.Groups.FirstOrDefault(x => x.Id == groupId);
				MeetupPagedView1.GroupName = new MeetupLangView() { En = group.NameEn, Ar = group.NameAr };
				MeetupPagedView1.GroupDescription = new MeetupLangView() { En = group.DescriptionEn, Ar = group.DescriptionAr };
				MeetupPagedView1.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == groupId && m.IsFollowed == true);
				_appDbContext.ProfileGroups.Where(m => m.GroupId == groupId && m.ProfileId == userId);
				MeetupPagedView MeetupPagedView2 = MeetupPagedView1;
				var profileGroups = _appDbContext.ProfileGroups.FirstOrDefault(m => m.GroupId == groupId && m.ProfileId == userId);
				MeetupPagedView2.IsFollowedGroup = (profileGroups != null) ? profileGroups.IsFollowed : false;
				MeetupPagedView1.GroupID = groupId;
				MeetupPagedView1.GroupIDEncrypted = _Encryptor.Encrypt(groupId.ToString());
				var list3 = _appDbContext.ProfileGroups.Where(m => m.GroupId == groupId && m.IsFollowed == true).Select(m => m.ProfileId).Take(Take).ToList<int>();
				List<ProfileInfo> profileInfoList = new List<ProfileInfo>();
				foreach (int num2 in list3)
				{
					int profileId = num2;
					ProfileInfo profileInfo = new ProfileInfo();
					int? smallImageFileId = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId).SmallImageFileId;
					if (smallImageFileId.HasValue)
					{
						smallImageFileId = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId).SmallImageFileId;
						int fileId = smallImageFileId.Value;
						profileInfo.GroupProfileImageId = fileId;
					}
					var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId);
					profileInfo.profileId = clientUser.Id;
					profileInfo.ProfileName = new MeetupLangView() { En = clientUser.NameEn, Ar = clientUser.NameAr };
					profileInfo.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
					var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == profileId);
					if (profileDesignation != null)
					{
						profileInfo.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
					}

					profileInfoList.Add(profileInfo);
				}
				MeetupPagedView1.profileInfos = profileInfoList;
				MeetupPagedView1.GroupImageID = _appDbContext.Groups.FirstOrDefault(m => m.Id == groupId).MobCoverImageFileID;
				MeetupPagedView1.GroupIDEncrypted = _Encryptor.Encrypt(groupId.ToString());
				return MeetupPagedView1;
			}
			catch (Exception e)
			{
				logger.Error(e);
				return null;
			}
		}

		public async Task<IMeetupResponse> CreateGroup(int userId, GroupView groupView)
		{
			try
			{
				var Entity = new Group
				{
					NameEn = groupView.GroupName.En,
					NameAr = groupView.GroupName.Ar,
					DescriptionAr = groupView.GroupDescription.Ar,
					DescriptionEn = groupView.GroupDescription.En,
				};

				if (groupView.CoverImage != null)
				{
					Entity.CoverImageFileId = (await _fileService.SaveMeetupFileAsync(groupView.CoverImage, userId)).Id;
				}
				
				
				_appDbContext.Groups.Add(Entity);
				_appDbContext.SaveChanges();
				IMeetupResponse Gview = await GetGroup(Entity.Id, userId);
				return Gview;
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> UpdateGroup(int userId, GroupView groupView)
		{
			try
			{
				Group group = _appDbContext.Groups.FirstOrDefault(x => x.Id == groupView.ID);
				if (group == null)
					throw new ArgumentException("Invalid groupId: " + (object)groupView.ID);
				group.NameEn = groupView.GroupName.En;
				group.NameAr = groupView.GroupName.Ar;
				group.DescriptionAr = groupView.GroupDescription.Ar;
				group.DescriptionEn = groupView.GroupDescription.En;
				if (groupView.CoverImage != null)
				{
					_appDbContext.Files.Remove(_appDbContext.Files.FirstOrDefault(x => x.Id == groupView.CoverImgID));
					group.CoverImageFileId = (await _fileService.SaveMeetupFileAsync(groupView.CoverImage, userId)).Id;
				}

				await _appDbContext.SaveChangesAsync();
				IMeetupResponse Gview = await GetGroup(group.Id, userId);
				return Gview;
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> GetGroup(int Id, int userId)
		{
			try
			{
				var group = _appDbContext.Groups.FirstOrDefault(m => m.Id == Id);
				if (group == null)
					throw new ArgumentException("Invalid GroupId: " + (object)Id);
				GroupView groupView = new GroupView();
				groupView.ID = group.Id;
				groupView.GroupDescription = new MeetupLangView()
				{
					En = group.DescriptionEn,
					Ar = group.DescriptionAr
				};
				groupView.GroupName = new MeetupLangView()
				{
					En = group.NameEn,
					Ar = group.NameAr
				};
				groupView.Description = new MeetupLangView() { En = group.DescriptionEn, Ar = group.DescriptionAr };
				groupView.Name = new MeetupLangView() { En = group.NameEn, Ar = group.NameAr };

				groupView.CoverImgID = group.CoverImageFileId;
				var profileGroup = await _appDbContext.ProfileGroups.FirstOrDefaultAsync(m => m.GroupId == group.Id && m.ProfileId == userId);
				if (profileGroup != null)
					groupView.IsFollowed = profileGroup.IsFollowed;
				else
					groupView.IsFollowed = false;

				groupView.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == Id && m.IsFollowed == true);
				return new MeetupResponse(groupView);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> FollowGroup(int groupId, int userId)
		{
			try
			{
				FollwGroupView fgview = new FollwGroupView();

				if (_appDbContext.ProfileGroups.Any(m => m.GroupId == groupId && m.ProfileId == userId))
					_appDbContext.ProfileGroups.FirstOrDefault(m => m.GroupId == groupId && m.ProfileId == userId).IsFollowed = true;
				else
					_appDbContext.ProfileGroups.Add(new ProfileGroup
					{
						ProfileId = userId,
						GroupId = groupId,
						IsFollowed = true
					});
				await _appDbContext.SaveChangesAsync();

				fgview.ID = groupId;
				fgview.Name = new MeetupLangView()
				{
					En = _appDbContext.ProfileGroups.Include(m => m.Group).FirstOrDefault(m => m.GroupId == groupId).Group.NameEn,
					Ar = _appDbContext.ProfileGroups.Include(m => m.Group).FirstOrDefault(m => m.GroupId == groupId).Group.NameAr
				};
				
				fgview.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == groupId && m.IsFollowed == true); 
				fgview.IsFollowed = true;

				return new MeetupResponse(fgview);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> UnFollowGroup(int groupId, int userId)
		{
			try
			{
				FollwGroupView fgview = new FollwGroupView();
				if (_appDbContext.ProfileGroups.Any(m => m.GroupId == groupId && m.ProfileId == userId))
				{
					_appDbContext.ProfileGroups.FirstOrDefault(m => m.GroupId == groupId && m.ProfileId == userId).IsFollowed = false;
					await _appDbContext.SaveChangesAsync();


					fgview.ID = groupId;
					fgview.Name = new MeetupLangView()
					{
						En = _appDbContext.ProfileGroups.Include(m => m.Group).FirstOrDefault(m => m.GroupId == groupId).Group.NameEn,
						Ar = _appDbContext.ProfileGroups.Include(m => m.Group).FirstOrDefault(m => m.GroupId == groupId).Group.NameAr
					};					
					fgview.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == groupId && m.IsFollowed == true); 
					fgview.IsFollowed = false;
				}
				return new MeetupResponse(fgview);

			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public int GetTotalGroupCount()
		{
			return _appDbContext.Groups.Count();
		}
		#endregion

		#region Manage Meetup
		public async Task<IMeetupResponse>  GetMeetupsByGroupId(int groupId, int userId, int skip, int limit)
		{
			try
			{
				
				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
				int? usedPermissionSetId = user.LastUsedPermissionSetId;
				int num1 = 1;
								
				Expression<Func<Meetup, bool>> predicate;
				//if (usedPermissionSetId.GetValueOrDefault() == num1 & usedPermissionSetId.HasValue)
				//{
				//	predicate = (i => i.GroupId == groupId);
					
				//}
				//else
					predicate = (i => i.GroupId == groupId && (i.StatusItemId == 76001 || i.OwnerId == user.Id));

				var list1 =  _appDbContext.Meetups.Where(predicate).OrderBy(m => m.Date).Skip(skip).Take(limit).ToList();
				var listCount = _appDbContext.Meetups.Where(predicate).OrderBy(m => m.Date).ToList();
				var source1 = new List<MeetupView>();
				foreach (var data in list1)
				{
					var item = data;
					MeetupView meetupView = new MeetupView();
					meetupView.ID = item.Id;
					meetupView.Date = item.Date;
					meetupView.Descriotion = item.Description;
					meetupView.Title = item.Title;
					meetupView.IDEncrypted = _Encryptor.Encrypt(item.Id.ToString());
					meetupView.IsOwner = item.CreatedBy == user.Username;
					meetupView.StatusItemID = item.StatusItemId;
					meetupView.DisplayAdminDecision = item.StatusItemId == 76001 && item.CreatedBy == user.Username;
					meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(m => m.MeetupId == item.Id && m.ProfileId == userId)?.MeetupStatusItemId;
					if (item.StatusItemId != null && item.StatusItemId > 0)
					{
						var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 76 && k.Id == item.StatusItemId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
						meetupView.StatusItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
					}
					//meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(x => x.MeetupId == item.Id && x.ProfileId == userId)?.MeetupStatusItemId;
					if (meetupView.ProfileDesicionID != null && meetupView.ProfileDesicionID > 0)
					{
						var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 69 && k.Id == meetupView.ProfileDesicionID).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
						meetupView.ProfileDesicionItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
					}
					meetupView.CreatedBY = item.CreatedBy;
					meetupView.StartTime = item.StartTime;
					meetupView.EndTime = item.EndTime;
					meetupView.MeetupPictureId = item.MeetupPictureID;
					var source2 = _appDbContext.ProfileMeetups.Where(m => m.MeetupId == item.Id && m.MeetupStatusItemId == 69001);
					meetupView.NumberOfGoing = source2.ToList().Count();
					meetupView.MapAutocompleteView = new MapAutocompleteView()
					{
						Latitude = item.Latitude,
						Longitude = item.Longitude,
						Text = item.MapText
					};
					var list2 = source2.Select(p => p.ProfileId).ToList();
					var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == item.OwnerId);
					meetupView.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
					meetupView.UserName = new MeetupLangView() { En= clientUser.NameEn,Ar= clientUser.NameAr };
					var fileViewList = new List<ParticipantsView>();
					foreach (int num2 in list2)
					{
						int profileId = num2;
						var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == profileId);
						var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
							.Where(k => k.ProfileId == profileId).OrderByDescending(k => k.DateFrom)
							.FirstOrDefaultAsync();
						
						int? smallImageFileId = commentUser.SmallImageFileId;
						if (smallImageFileId.HasValue)
						{
							smallImageFileId = commentUser.SmallImageFileId;
							
						}

						
						ParticipantsView view=new  ParticipantsView();
						view.NameEn = commentUser.NameEn;
						view.NameAr = commentUser.NameAr;
						view.TitleEn = commentUserJob?.Title?.TitleEn;
						view.TitleAr = commentUserJob?.Title?.TitleAr;
						view.Userid = profileId;
						view.ParticipantsImageId = smallImageFileId;
						fileViewList.Add(view);
					}
					meetupView.ParticipantsList = fileViewList;
					if (clientUser.SmallImageFileId.HasValue)
						meetupView.ProfileImageId = clientUser.SmallImageFileId.Value;
					meetupView.Comments = GetMeetupCommentsAsync(meetupView.ID, userId,0,10).Result.CommentView;
					source1.Add(meetupView);
				}
				var pagingView = new PagingView();
				pagingView.TotalCount = listCount.Count();
				MeetupPagedView MeetupPagedView1 = new MeetupPagedView();
				//MeetupPagedView1.List = source1.Skip(pagingView.Skip).Take(pagingView.Take).ToList();
				MeetupPagedView1.MeetupList = source1.ToList();
				MeetupPagedView1.pagingView = pagingView;
				Group group = _appDbContext.Groups.FirstOrDefault(x => x.Id == groupId);
				MeetupPagedView1.GroupName = new MeetupLangView() { En = group.NameEn, Ar = group.NameAr }; 
				MeetupPagedView1.GroupDescription = new MeetupLangView() { En = group.DescriptionEn, Ar= group.DescriptionAr }; 
				MeetupPagedView1.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == groupId && m.IsFollowed == true);
				_appDbContext.ProfileGroups.Where(m => m.GroupId == groupId && m.ProfileId == userId);
				MeetupPagedView MeetupPagedView2 = MeetupPagedView1;
				var profileGroups = _appDbContext.ProfileGroups.FirstOrDefault(m => m.GroupId == groupId && m.ProfileId == userId);
				MeetupPagedView2.IsFollowedGroup = (profileGroups != null) ? profileGroups.IsFollowed : false;
				MeetupPagedView1.GroupID = groupId;
				MeetupPagedView1.GroupIDEncrypted = _Encryptor.Encrypt(groupId.ToString());
				//var list3 = _appDbContext.ProfileGroups.Where(m => m.GroupId == groupId && m.IsFollowed == true).Select(m => m.ProfileId).ToList<int>();
				//List<ProfileInfo> profileInfoList = new List<ProfileInfo>();
				//foreach (int num2 in list3)
				//{
				//	int profileId = num2;
				//	ProfileInfo profileInfo = new ProfileInfo();
				//	int? smallImageFileId = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId).SmallImageFileId;
				//	if (smallImageFileId.HasValue)
				//	{
				//		smallImageFileId = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId).SmallImageFileId;
				//		int fileId = smallImageFileId.Value;
				//		profileInfo.GroupProfileImageId = fileId;
				//	}
				//	var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId);
				//	profileInfo.profileId = clientUser.Id;
				//	profileInfo.ProfileName = new MeetupLangView() { En = clientUser.NameEn, Ar = clientUser.NameAr };
				//	profileInfo.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
				//	var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == profileId);
				//	if (profileDesignation != null)
				//	{
				//		profileInfo.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
				//	}
					
				//	profileInfoList.Add(profileInfo);
				//}
				//MeetupPagedView1.profileInfos = profileInfoList;
				MeetupPagedView1.GroupImageID = _appDbContext.Groups.FirstOrDefault(m => m.Id == groupId).MobCoverImageFileID;
				MeetupPagedView1.GroupIDEncrypted = _Encryptor.Encrypt(groupId.ToString());
				return new MeetupResponse(MeetupPagedView1);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> GetMeetupsProfileByGroupId(int groupId, int userId, int skip, int limit)
		{
			try
			{

				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
				//int? usedPermissionSetId = user.LastUsedPermissionSetId;
				//int num1 = 1;

				//Expression<Func<Meetup, bool>> predicate;
				//if (usedPermissionSetId.GetValueOrDefault() == num1 & usedPermissionSetId.HasValue)
				//{
				//	predicate = (i => i.GroupId == groupId);

				//}
				//else
				//	predicate = (i => i.GroupId == groupId && (i.StatusItemId == 76001 || i.OwnerId == user.Id));

				//var list1 = _appDbContext.Meetups.Where(predicate).OrderBy(m => m.Date).ToList();
				//var source1 = new List<MeetupView>();
				//foreach (var data in list1)
				//{
				//	var item = data;
				//	MeetupView meetupView = new MeetupView();
				//	meetupView.ID = item.Id;
				//	meetupView.Date = item.Date;
				//	meetupView.Descriotion = item.Description;
				//	meetupView.Title = item.Title;
				//	meetupView.IDEncrypted = _Encryptor.Encrypt(item.Id.ToString());
				//	meetupView.IsOwner = item.CreatedBy == user.Username;
				//	meetupView.StatusItemID = item.StatusItemId;
				//	meetupView.DisplayAdminDecision = item.StatusItemId == 76001 && item.CreatedBy == user.Username;
				//	meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(m => m.MeetupId == item.Id && m.ProfileId == userId)?.MeetupStatusItemId;
				//	if (item.StatusItemId != null && item.StatusItemId > 0)
				//	{
				//		var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 76 && k.Id == item.StatusItemId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
				//		meetupView.StatusItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
				//	}
				//	//meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(x => x.MeetupId == item.Id && x.ProfileId == userId)?.MeetupStatusItemId;
				//	if (meetupView.ProfileDesicionID != null && meetupView.ProfileDesicionID > 0)
				//	{
				//		var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 69 && k.Id == meetupView.ProfileDesicionID).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
				//		meetupView.ProfileDesicionItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
				//	}
				//	meetupView.CreatedBY = item.CreatedBy;
				//	meetupView.StartTime = item.StartTime;
				//	meetupView.EndTime = item.EndTime;
				//	meetupView.MeetupPictureId = item.MeetupPictureID;
				//	var source2 = _appDbContext.ProfileMeetups.Where(m => m.MeetupId == item.Id && m.MeetupStatusItemId == 69001);
				//	meetupView.NumberOfGoing = source2.ToList().Count();
				//	meetupView.MapAutocompleteView = new MapAutocompleteView()
				//	{
				//		Latitude = item.Latitude,
				//		Longitude = item.Longitude,
				//		Text = item.MapText
				//	};
				//	var list2 = source2.Select(p => p.ProfileId).ToList();
				//	var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == item.OwnerId);
				//	meetupView.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
				//	meetupView.UserName = new MeetupLangView() { En = clientUser.NameEn, Ar = clientUser.NameAr };
				//	var fileViewList = new List<ParticipantsView>();
				//	foreach (int num2 in list2)
				//	{
				//		int profileId = num2;
				//		var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == profileId);
				//		var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
				//			.Where(k => k.ProfileId == profileId).OrderByDescending(k => k.DateFrom)
				//			.FirstOrDefaultAsync();

				//		int? smallImageFileId = commentUser.SmallImageFileId;
				//		if (smallImageFileId.HasValue)
				//		{
				//			smallImageFileId = commentUser.SmallImageFileId;

				//		}


				//		ParticipantsView view = new ParticipantsView();
				//		view.NameEn = commentUser.NameEn;
				//		view.NameAr = commentUser.NameAr;
				//		view.TitleEn = commentUserJob?.Title?.TitleEn;
				//		view.TitleAr = commentUserJob?.Title?.TitleAr;
				//		view.Userid = profileId;
				//		view.ParticipantsImageId = smallImageFileId;
				//		fileViewList.Add(view);
				//	}
				//	meetupView.ParticipantsList = fileViewList;
				//	if (clientUser.SmallImageFileId.HasValue)
				//		meetupView.ProfileImageId = clientUser.SmallImageFileId.Value;
				//	meetupView.Comments = GetMeetupCommentsAsync(meetupView.ID, userId, 0, 10).Result.CommentView;
				//	source1.Add(meetupView);
				//}
				//var pagingView = new PagingView();
				//pagingView.TotalCount = source1.Count();
				//MeetupPagedView1.List = source1.Skip(pagingView.Skip).Take(pagingView.Take).ToList();
				//MeetupPagedView1.MeetupList = source1.ToList();
				//MeetupPagedView1.pagingView = pagingView;
				MeetupPagedView MeetupPagedView1 = new MeetupPagedView();
				Group group = _appDbContext.Groups.FirstOrDefault(x => x.Id == groupId);
				MeetupPagedView1.GroupName = new MeetupLangView() { En = group.NameEn, Ar = group.NameAr };
				MeetupPagedView1.GroupDescription = new MeetupLangView() { En = group.DescriptionEn, Ar = group.DescriptionAr };
				MeetupPagedView1.NumberOfFollowing = _appDbContext.ProfileGroups.Count(m => m.GroupId == groupId && m.IsFollowed == true);
				_appDbContext.ProfileGroups.Where(m => m.GroupId == groupId && m.ProfileId == userId);
				MeetupPagedView MeetupPagedView2 = MeetupPagedView1;
				var profileGroups = _appDbContext.ProfileGroups.FirstOrDefault(m => m.GroupId == groupId && m.ProfileId == userId);
				MeetupPagedView2.IsFollowedGroup = (profileGroups != null) ? profileGroups.IsFollowed : false;
				MeetupPagedView1.GroupID = groupId;
				MeetupPagedView1.GroupIDEncrypted = _Encryptor.Encrypt(groupId.ToString());
				var list3 = _appDbContext.ProfileGroups.Where(m => m.GroupId == groupId && m.IsFollowed == true).Select(m => m.ProfileId).Skip(skip).Take(limit).ToList<int>();
				List<ProfileInfo> profileInfoList = new List<ProfileInfo>();
				foreach (int num2 in list3)
				{
					int profileId = num2;
					ProfileInfo profileInfo = new ProfileInfo();
					int? smallImageFileId = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId).SmallImageFileId;
					if (smallImageFileId.HasValue)
					{
						smallImageFileId = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId).SmallImageFileId;
						int fileId = smallImageFileId.Value;
						profileInfo.GroupProfileImageId = fileId;
					}
					var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == profileId);
					profileInfo.profileId = clientUser.Id;
					profileInfo.ProfileName = new MeetupLangView() { En = clientUser.NameEn, Ar = clientUser.NameAr };
					profileInfo.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
					var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == profileId);
					if (profileDesignation != null)
					{
						profileInfo.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
					}

					profileInfoList.Add(profileInfo);
				}
				MeetupPagedView1.profileInfos = profileInfoList;
				MeetupPagedView1.GroupImageID = _appDbContext.Groups.FirstOrDefault(m => m.Id == groupId).MobCoverImageFileID;
				MeetupPagedView1.GroupIDEncrypted = _Encryptor.Encrypt(groupId.ToString());
				return new MeetupResponse(MeetupPagedView1);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> UpdateMeetup(MeetupAdd View)
		{
			try
			{
				Meetup meetup = await _appDbContext.Meetups.FirstOrDefaultAsync(x => x.Id == View.MeetupId);
				if (meetup == null)
					throw new ArgumentException("Invalid MeetupId: " + (object)View.MeetupId);
				meetup.Title = View.Title;
				meetup.Description = View.Descriotion;
				meetup.Date = View.Date;
				meetup.Latitude = View.MapAutocompleteView.Latitude;
				meetup.Longitude = View.MapAutocompleteView.Longitude;
				meetup.StartTime = DateTime.Parse(View.Date.ToShortDateString() + " " + View.StartTime);
				meetup.EndTime = DateTime.Parse(View.Date.ToShortDateString() + " " + View.EndTime);
				
				meetup.MapText = View.MapAutocompleteView.Text;

				if (View.MeetupPicture != null)
				{
					meetup.MeetupPictureID = (await _fileService.SaveMeetupFileAsync(View.MeetupPicture, View.userId)).Id;
				}


				await _appDbContext.SaveChangesAsync();

				IMeetupResponse meetupView = await GetMeetup(meetup.Id, View.userId);
				return meetupView;
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> CreateMeetup(MeetupAdd View)
		{
			try
			{
				logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
				//var notifyText = "A new meeting has been added";
				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == View.userId);
				Meetup meetup = new Meetup
				{
					Title = View.Title,
					Date = View.Date,
					StartTime = DateTime.Parse(View.Date.ToShortDateString() + " " + View.StartTime),
					EndTime = DateTime.Parse(View.Date.ToShortDateString() + " " + View.EndTime),
					GroupId = View.GroupID,
					Description = View.Descriotion,
					Latitude = View.MapAutocompleteView.Latitude,
					Longitude = View.MapAutocompleteView.Longitude,
					MapText = View.MapAutocompleteView.Text,
					IsPublished=false,
					Created = DateTime.Now,
					Modified = DateTime.Now,
					CreatedBy = user.Username,
					ModifiedBy = user.Username

				};
				if (View.MeetupPicture != null)
				{
					meetup.MeetupPictureID = (await _fileService.SaveMeetupFileAsync(View.MeetupPicture, View.userId)).Id;
				}

				int? usedPermissionSetId1 = user.LastUsedPermissionSetId;
				int num1 = 1;
				meetup.StatusItemId = usedPermissionSetId1.GetValueOrDefault() == num1 & usedPermissionSetId1.HasValue ? 76001 : 76003;
				meetup.OwnerId = View.userId;
				_appDbContext.Meetups.Add(meetup);
				int? usedPermissionSetId2 = user.LastUsedPermissionSetId;
				int num2 = 1;
				if (usedPermissionSetId2.GetValueOrDefault() == num2 & usedPermissionSetId2.HasValue)
					meetup.PublishedDate = new DateTime?(DateTime.Now);
				await _appDbContext.SaveChangesAsync();
				IMeetupResponse meetupView = await GetMeetup(meetup.Id, View.userId);
				var notifyText = "A new meeting" + " " + meetup.Title + " " + "has been added";
				var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == View.userId).FirstOrDefaultAsync()) ??
						new MongoModels.User() { };

				var userIds = new List<int>();
				userIds.AddRange(mongoUser.FollowersIDs);
				foreach (var follwingItem in userIds)
				{

					var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == follwingItem && x.CategoryID == (int)CategoryType.MeetingHub).FirstOrDefaultAsync();
					if (customNotificationData?.isEnabled == true || customNotificationData == null)
					{
						await AddNotificationAsync(follwingItem, ActionType.AddNewItem, meetup.Id, ParentType.Meetup, View.userId, notifyText);
					}

				}

					

				return meetupView;
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IUserRecommendationResponse> AddNotificationAsync(int userId, ActionType actionId, int meetupId, ParentType parentTypeId, int senderUserId, string notifyText)
		{
			try
			{
				logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

				//var profileId = userId != senderUserId ? senderUserId : userId;
				var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
												await AddNotificationObjectAsync(userId);

				var notificationObj = new Notification
				{
					ID = ObjectId.GenerateNewId(),
					ActionID = (int)actionId,
					IsRead = false,
					ParentID = meetupId.ToString(),
					ParentTypeID = (int)parentTypeId,
					SenderID = senderUserId
				};

				notificationGenericObject.Notifications.Add(notificationObj);

				if (userId != senderUserId)
				{
				notificationGenericObject.UnseenNotificationCounter += 1;
				var notificationView = _mapper.Map<NotificationView>(notificationObj);
				await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView });

					var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
					foreach (var deviceId in deviceIds)
					{
						await _pushNotificationService.SendRecommendPushNotificationAsync(notificationView, notifyText, deviceId);
					}
					logger.Info("Notification sent");
				}

				await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);



				var notificationGenericObjectView = new NotificationGenericObjectView
				{
					ID = notificationGenericObject.ID.ToString(),
					UserID = notificationGenericObject.UserID,
					UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
					NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
				};

				return new UserRecommendationResponse(notificationGenericObjectView);
			}
			catch (Exception e)
			{
				return new UserRecommendationResponse(e);
			}
		}

		//public async Task<bool> PushNotification(NotificationView notification, string notifyText, int userId)
		//{
		//	var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
		//	foreach (var deviceId in deviceIds)
		//	{
		//		await _pushNotificationService.SendRecommendPushNotificationAsync(notification, notifyText, deviceId);
		//	}
		//	return true;
		//}
		private async Task FillNotificationUserDetailsAsync(int userId, List<NotificationView> notificationsList)
		{
			foreach (var notification in notificationsList)
			{
				var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == notification.SenderID);
				notification.UserNameEn = user?.NameEn;
				notification.UserNameAr = user?.NameAr;
				notification.UserImageFileId = user?.OriginalImageFileId ?? 0;
				notification.RedirectUrlPath = notification.ParentTypeID == ParentType.User
					? $"/api/Profile/get-public-profile/{userId}/{notification.SenderID}"
					: notification.RedirectUrlPath;
			}
		}
		private async Task<NotificationGenericObject> AddNotificationObjectAsync(int userId)
		{
			try
			{
				var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync();

				if (notificationGenericObject != null) return notificationGenericObject;

				notificationGenericObject = new NotificationGenericObject
				{
					ID = ObjectId.GenerateNewId(),
					UserID = userId,
					UnseenNotificationCounter = 0,
					Notifications = new List<Notification>()
				};

				await _mongoDbContext.NotificationGenericObjects.InsertOneAsync(notificationGenericObject);
				return notificationGenericObject;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public async Task<IMeetupResponse> GenerateMeetupQRCodeAsync(int meetupid,int userId)
		{
			try
			{
				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
				var item = await _appDbContext.Meetups.FirstOrDefaultAsync(e => e.Id == meetupid);
				if (item == null)
					throw new ArgumentException("Invalid meetup Id: " + meetupid);
				//if (item.QRCode == null)
				//{
				//	item.QRCode = RandomNumber().ToString();
				//	item.Modified = DateTime.Now;
				//	await _appDbContext.SaveChangesAsync();
				//}

				var QRCodeData = "Meetup-" + item.Id.ToString();

				return new MeetupResponse(QRCodeData);


			}
			catch (Exception e)
			{
				logger.Error(e);

				return new MeetupResponse(e);
			}
			//return null;
		}
        public int RandomNumber()
		{
			Random random = new Random();
			return random.Next(100000000, 999999999);
		}
		public async Task<IMeetupResponse> AddMeetupCommendsAsync(MeetupCommendAddView view)
		{
			try
			{
				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == view.userId);
				var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
				var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
				var userName = firstName + " " + lastName;
				var Gobject = await _mongoDbContext.GenericObjects.Find(x => x.ID == view.MeetupId).FirstOrDefaultAsync() ?? 
					       await AddGenericObjectAsync(view, userName);
				List<string> imageList = new List<string>();
				List<string> documentList = new List<string>();
				var imageId = ObjectId.GenerateNewId();
				var documentId = ObjectId.GenerateNewId();

				int TypeID = 1;
				if (view.ImageData != null)
				{
					TypeID = 2;
					imageList.Add(imageId.ToString());
				}
				if (view.DocumentData != null)
				{
					TypeID = 3;
					documentList.Add(documentId.ToString());
				}
				// message.SeenByIDs.Add(userId);
				var commentId = ObjectId.GenerateNewId();
				var fileName = "";
				var bucket = new GridFSBucket(_mongoDbContext.Database);
				var comment = new MeetupComment()
				{
					Id = commentId,
					Text = view.CommandText,
					Created = DateTime.Now,
					UserID = view.userId,
					TypeID = TypeID,
					IsAdminCreated = false,
					IsDeleted = false,
					Modified = DateTime.Now,
					Reports=new List<Report>(),
					FilesIDs = TypeID == 3 ? documentList : new List<string>(),
					ImagesIDs = TypeID == 2 ? imageList : new List<string>()

				};
				Gobject.Modified = DateTime.Now;
				Gobject.ModifiedBy = userName;

				if (view.ImageData != null)
				{
					
					await PostImageUploadAsync(view, bucket, comment);
				}

				if (view.DocumentData != null)
				{
					
					await PostDocumentUploadAsync(view, bucket, comment);
				}

				Gobject.Comments.Add(_mapper.Map<MeetupComment>(comment));
				await _mongoDbContext.GenericObjects.ReplaceOneAsync(x => x.ID == view.MeetupId, Gobject);

				return await GetMeetupCommentsbyIdAsync(view.MeetupId, view.userId, comment.Id.ToString()); 
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}
		public async Task<IMeetupResponse> EditMeetupCommentAsync(MeetupCommendAddView view, string commentid)
		{
			try
			{
				//logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()}");
				var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
				var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
				var userName = firstName + " " + lastName;
				var genericObject = await _mongoDbContext.GenericObjects.Find(x => x.ID == view.MeetupId).FirstOrDefaultAsync();

				if (genericObject == null) return new MeetupResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

				var exstingComment = genericObject.Comments.FirstOrDefault(k => k.Id == new ObjectId(commentid));

		
				List<string> imageList = new List<string>();
				List<string> documentList = new List<string>();
				var imageId = ObjectId.GenerateNewId();
				var documentId = ObjectId.GenerateNewId();

				int TypeID = 1;
				if (view.ImageData != null)
				{
					TypeID = 2;
					imageList.Add(imageId.ToString());
				}
				if (view.DocumentData != null)
				{
					TypeID = 3;
					documentList.Add(documentId.ToString());
				}
				// message.SeenByIDs.Add(userId);
				var commentId = ObjectId.GenerateNewId();
				var fileName = "";
				var bucket = new GridFSBucket(_mongoDbContext.Database);
				var comment = new MeetupComment()
				{
					Id = exstingComment.Id,
					Text = view.CommandText,
					Created = exstingComment.Created,
					UserID = view.userId,
					TypeID = exstingComment.TypeID,
					IsAdminCreated = exstingComment.IsAdminCreated,
					IsDeleted = exstingComment.IsDeleted,
					Modified = DateTime.Now,
					Reports = exstingComment.Reports,
					FilesIDs = exstingComment.FilesIDs,
					ImagesIDs = exstingComment.ImagesIDs,
					FileName = exstingComment.FileName

				};


				if (view.ImageData != null)
				{

					await PostImageUploadAsync(view, bucket, comment);
				}

				if (view.DocumentData != null)
				{

					await PostDocumentUploadAsync(view, bucket, comment);
				}


				genericObject.Comments.Remove(exstingComment);
				genericObject.Comments.Add(_mapper.Map<MeetupComment>(comment));
				await _mongoDbContext.GenericObjects.ReplaceOneAsync(x => x.ID == view.MeetupId, genericObject);

				return await GetMeetupCommentsbyIdAsync(view.MeetupId, view.userId, commentid);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> DeleteMeetupCommentAsync(int meetupid, string commentId, int userId)
		{
			try
			{
				var Gobject = await _mongoDbContext.GenericObjects.Find(x => x.ID == meetupid).FirstOrDefaultAsync();
				if (Gobject == null) return new MeetupResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

				var comment = Gobject.Comments.FirstOrDefault(x => x.Id == new ObjectId(commentId));
				Gobject.Comments.Remove(comment);
				await _mongoDbContext.GenericObjects.ReplaceOneAsync(x => x.ID == meetupid, Gobject);

				List<CommentViewModel> comments = new List<CommentViewModel>();
				CommentViewModel commentView = new CommentViewModel();
				commentView.MeetupID = meetupid;
				commentView.ID = comment.Id.ToString();
				commentView.Text = comment.Text;
				commentView.TypeID = comment.TypeID;
				commentView.UserID = comment.UserID;
				commentView.Created = comment.Created;
				commentView.FilesIDs = (comment.FilesIDs != null) ? comment.FilesIDs : new List<string>();
				commentView.ImagesIDs = (comment.ImagesIDs != null) ? comment.ImagesIDs : new List<string>();
				commentView.IsAdminCreated = comment.IsAdminCreated;
				commentView.IsDeleted = true;
				commentView.Modified = comment.Modified;
				commentView.FileName = (comment.FileName != null) ? comment.FileName : "";

				var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == comment.UserID);
				var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
					.Where(k => k.ProfileId == comment.UserID).OrderByDescending(k => k.DateFrom)
					.FirstOrDefaultAsync();

				commentView.NameEn = commentUser.NameEn;
				commentView.NameAr = commentUser.NameAr;
				commentView.TitleEn = commentUserJob?.Title?.TitleEn;
				commentView.TitleAr = commentUserJob?.Title?.TitleAr;
				commentView.UserImageFileId = commentUser.OriginalImageFileId ?? 0;
				comments.Add(commentView);

				return new MeetupResponse(comments);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

	 public async Task<IMeetupResponse> GetMeetupCommentsbyIdAsync(int meetupId, int userId,string commentid)
			{
				try
				{
					List<CommentViewModel> comments = new List<CommentViewModel>();
					GenericObject meetupcomment = new GenericObject();
					
					meetupcomment = await _mongoDbContext.GenericObjects.Find(x => x.ID == meetupId).FirstOrDefaultAsync();
					if (meetupcomment != null && meetupcomment.Comments != null)
					{
						foreach (var item in meetupcomment.Comments.Where(x=>x.Id == new ObjectId(commentid)))
						{
							CommentViewModel commentView = new CommentViewModel();
							commentView.MeetupID = meetupId;
							commentView.ID = item.Id.ToString();
							commentView.Text = item.Text;
							commentView.TypeID = item.TypeID;
							commentView.UserID = item.UserID;
							commentView.Created = item.Created;
							commentView.FilesIDs = (item.FilesIDs != null) ? item.FilesIDs : new List<string>();
							commentView.ImagesIDs = (item.ImagesIDs != null) ? item.ImagesIDs : new List<string>();
							commentView.IsAdminCreated = item.IsAdminCreated;
							commentView.IsDeleted = item.IsDeleted;
							commentView.Modified = item.Modified;
							commentView.FileName = (item.FileName != null) ? item.FileName : "";

							var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == item.UserID);
							var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
								.Where(k => k.ProfileId == item.UserID).OrderByDescending(k => k.DateFrom)
								.FirstOrDefaultAsync();

							commentView.NameEn = commentUser.NameEn;
							commentView.NameAr = commentUser.NameAr;
							commentView.TitleEn = commentUserJob?.Title?.TitleEn;
							commentView.TitleAr = commentUserJob?.Title?.TitleAr;
							commentView.UserImageFileId = commentUser.OriginalImageFileId ?? 0;
							comments.Add(commentView);
						}
					}
					return new MeetupResponse(comments.OrderBy(k => k.Created).ToList());
				}
				catch (Exception e)
				{
				logger.Error(e);
				return new MeetupResponse(e);
				}
			}
		
		public async Task<IMeetupResponse> GetMeetupCommentsAsync(int meetupId, int userId, int Skip, int Take)
		{
			try
			{
				List<CommentViewModel> comments = new List<CommentViewModel>();
				GenericObject meetupcomment = new GenericObject();
				int limit = (Take == 0) ? 10 : Take;
				 meetupcomment = await _mongoDbContext.GenericObjects.Find(x => x.ID == meetupId).FirstOrDefaultAsync();
				if (meetupcomment != null && meetupcomment.Comments != null)
				{
					foreach (var item in meetupcomment.Comments.Skip(Skip).Take(limit))
					{
						CommentViewModel commentView = new CommentViewModel();
						commentView.MeetupID = meetupId;
						commentView.ID = item.Id.ToString();
						commentView.Text = item.Text;
						commentView.TypeID = item.TypeID;
						commentView.UserID = item.UserID;
						commentView.Created = item.Created;
						commentView.FilesIDs = (item.FilesIDs != null) ? item.FilesIDs : new List<string>();
						commentView.ImagesIDs = (item.ImagesIDs != null) ? item.ImagesIDs : new List<string>();
						commentView.IsAdminCreated = item.IsAdminCreated;
						commentView.IsDeleted = item.IsDeleted;
						commentView.Modified = item.Modified;
						commentView.FileName = (item.FileName != null) ? item.FileName : "";

						var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == item.UserID);
						var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
							.Where(k => k.ProfileId == item.UserID).OrderByDescending(k => k.DateFrom)
							.FirstOrDefaultAsync();

						commentView.NameEn = commentUser.NameEn;
						commentView.NameAr = commentUser.NameAr;
						commentView.TitleEn = commentUserJob?.Title?.TitleEn;
						commentView.TitleAr = commentUserJob?.Title?.TitleAr;
						commentView.UserImageFileId = commentUser.OriginalImageFileId ?? 0;
						comments.Add(commentView);
					}
				}
				return new MeetupResponse(comments.OrderBy(k => k.Created).ToList());
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}
		private static async Task PostImageUploadAsync(MeetupCommendAddView meetup, GridFSBucket bucket, MeetupComment comment)
		{
			GridFSUploadOptions options = new GridFSUploadOptions()
			{
				ChunkSizeBytes = (int)meetup.ImageData.Length,
				ContentType = meetup.ImageData.ContentType
			};
			var imgFileId =
				await bucket.UploadFromStreamAsync(meetup.ImageData.FileName, meetup.ImageData.OpenReadStream(), options);
			comment.ImagesIDs = new List<string>() { imgFileId.ToString() };
		}

		private static async Task PostDocumentUploadAsync(MeetupCommendAddView meetup, GridFSBucket bucket, MeetupComment comment)
		{
			var documentName = meetup.DocumentData.FileName + "." + meetup.DocumentData.ContentType.Split('/')[1];
			GridFSUploadOptions docOption = new GridFSUploadOptions()
			{
				ChunkSizeBytes = (int)meetup.DocumentData.Length,
				ContentType = meetup.DocumentData.ContentType
			};
			var fileId = await bucket.UploadFromStreamAsync(documentName, meetup.DocumentData.OpenReadStream(), docOption);
			comment.FilesIDs = new List<string>() { fileId.ToString() };
			comment.FileName = meetup.DocumentData.FileName;


		}
		private async Task<GenericObject> AddGenericObjectAsync(MeetupCommendAddView view,string username)
		{
			try
			{
				var genericObject = await _mongoDbContext.GenericObjects.Find(x => x.ID == view.MeetupId).FirstOrDefaultAsync();

				if (genericObject != null) return genericObject;

				genericObject = new GenericObject
				{
					ID = view.MeetupId,
					TypeID = 1,
					Created = DateTime.Now,
					Modified = DateTime.Now,
					CreatedBy = username,
					ModifiedBy = username,
					Comments = new List<MeetupComment>()
				};

				await _mongoDbContext.GenericObjects.InsertOneAsync(genericObject);
				return genericObject;
			}
			catch (Exception e)
			{
				throw e;
			}
		}
		public async Task<IMeetupResponse> GetMeetup(int id, int userId)
		{
			try
			{
				var item = await _appDbContext.Meetups.FirstOrDefaultAsync(e => e.Id == id);
				if (item == null)
					throw new ArgumentException("Invalid meetup Id: " + id);

				var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

				MeetupView meetupView = new MeetupView
				{
					ID = item.Id,
					Date = item.Date,
					GroupID = item.GroupId,
					IsPublished = item.IsPublished,
					Descriotion = item.Description,
					Title = item.Title,
					RejectionComment = item.RejectionComment,
					IDEncrypted = _Encryptor.Encrypt(item.Id.ToString()),
					IsOwner = item.CreatedBy == user.Username,
					StatusItemID = item.StatusItemId,
					DisplayAdminDecision = item.StatusItemId == 76001 && item.CreatedBy == user.Username,
					CreatedBY = item.CreatedBy,
					StartTime = item.StartTime,
					EndTime = item.EndTime,
					MeetupPictureId = item.MeetupPictureID
				};
				if (item.StatusItemId != null && item.StatusItemId > 0)
				{
					var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 76 && k.Id == item.StatusItemId).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
					meetupView.StatusItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
				}
				meetupView.ProfileDesicionID = _appDbContext.ProfileMeetups.FirstOrDefault(x => x.MeetupId == item.Id && x.ProfileId == userId)?.MeetupStatusItemId;
				if (meetupView.ProfileDesicionID != null && meetupView.ProfileDesicionID > 0)
				{
					var lookup = await _appDbContext.LookupItems.Where(k => k.LookupId == 69 && k.Id == meetupView.ProfileDesicionID).OrderBy(k => k.NameEn).FirstOrDefaultAsync();
					meetupView.ProfileDesicionItem = new MeetupLangView() { En = lookup.NameEn, Ar = lookup.NameAr };
				}
				var profileMeetups = _appDbContext.ProfileMeetups.Where(x => x.MeetupId == item.Id && x.MeetupStatusItemId == 69001);
				meetupView.NumberOfGoing = profileMeetups.ToList().Count();
				meetupView.IsAdmin = userId == item.OwnerId;
				meetupView.MapAutocompleteView = new MapAutocompleteView()
				{
					Latitude = item.Latitude,
					Longitude = item.Longitude,
					Text = item.MapText
				};
				List<int> list = profileMeetups.Select(p => p.ProfileId).ToList<int>();
				var clientUser = _appDbContext.Users.FirstOrDefault(u => u.Id == item.OwnerId);
				meetupView.ProfileIDEncrypted = _Encryptor.Encrypt(clientUser.Id.ToString());
				meetupView.UserName = new MeetupLangView() { En = clientUser.NameEn, Ar = clientUser.NameAr };
				var fileViewList = new List<ParticipantsView>();
				foreach (int num in list)
				{
					int profileId = num;
					var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == profileId);
					var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
						.Where(k => k.ProfileId == profileId).OrderByDescending(k => k.DateFrom)
						.FirstOrDefaultAsync();

					int? smallImageFileId = commentUser.SmallImageFileId;
					if (smallImageFileId.HasValue)
					{
						smallImageFileId = commentUser.SmallImageFileId;

					}


					ParticipantsView view = new ParticipantsView();
					view.NameEn = commentUser.NameEn;
					view.NameAr = commentUser.NameAr;
					view.TitleEn = commentUserJob?.Title?.TitleEn;
					view.TitleAr = commentUserJob?.Title?.TitleAr;
					view.Userid = profileId;
					view.ParticipantsImageId = smallImageFileId;
					fileViewList.Add(view);
				}
				
				if (fileViewList != null)
					meetupView.ParticipantsList = fileViewList;
				if (clientUser.SmallImageFileId.HasValue)
					meetupView.ProfileImageId = clientUser.SmallImageFileId.Value;
				meetupView.MapAutocompleteView = new MapAutocompleteView()
				{
					Latitude = item.Latitude,
					Longitude = item.Longitude,
					Text = item.MapText
				};

				meetupView.Comments =  GetMeetupCommentsAsync(id, userId,0,10).Result.CommentView;

				return new MeetupResponse(meetupView);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> DeleteMeetup(int id)
		{
			try
			{
				Meetup meetup = await _appDbContext.Meetups.FirstOrDefaultAsync(x => x.Id == id);
				if (meetup == null)
					throw new ArgumentNullException("cann't find Meetup with id" + id);
				try
				{
					var pMetup = await _appDbContext.ProfileMeetups.Where(x => x.MeetupId == id).ToListAsync();
					if (pMetup != null && pMetup.Count>0)
					{
						foreach (var item in pMetup)
						{
							_appDbContext.Remove(item);
							_appDbContext.SaveChanges();
						}
						
					}
					var AMetup = await _appDbContext.Agenda.Where(x => x.MeetupId == id).ToListAsync();
					if (AMetup != null && AMetup.Count > 0)
					{
						foreach (var item in AMetup)
						{
							_appDbContext.Remove(item);
							_appDbContext.SaveChanges();
						}
					}
					_appDbContext.Remove(meetup);
					_appDbContext.SaveChanges();
					return new MeetupResponse(true);
				}
				catch (Exception ex)
				{
					return new MeetupResponse(false);
				}
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
			}
		}

		public async Task<IMeetupResponse> SetMeetupDesicion(int decisionId, int meetupId, int userId)
		{
			try
			{
				if (_appDbContext.ProfileMeetups.Any(m => m.MeetupId == meetupId && m.ProfileId == userId))
					_appDbContext.ProfileMeetups.FirstOrDefault(m => m.MeetupId == meetupId && m.ProfileId == userId).MeetupStatusItemId = decisionId;
				else
					_appDbContext.ProfileMeetups.Add(new ProfileMeetup
					{
						ProfileId = userId,
						MeetupId = meetupId,
						MeetupStatusItemId = decisionId
					});
				_appDbContext.SaveChanges();
				return new MeetupResponse(decisionId);
			}
			catch (Exception e)
			{
				logger.Error(e);
				return new MeetupResponse(e);
				throw;
			}
		}

		#endregion
	}
}
