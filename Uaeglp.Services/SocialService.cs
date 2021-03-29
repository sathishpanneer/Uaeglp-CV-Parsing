using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NLog;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts;
using Uaeglp.MongoModels;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.PostViewModels;
using Uaeglp.ViewModels.SurveyViewModels;
using Uaeglp.Models;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.ProfileViewModels;
using File = Uaeglp.Models.File;
using User = Uaeglp.MongoModels.User;
using Minio;
using Microsoft.AspNetCore.Http;
using Minio.Exceptions;
using Microsoft.Extensions.Options;
using MoreLinq;
using System.Net.Sockets;
using System.Web;

namespace Uaeglp.Services
{
    public class SocialService : ISocialService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly MongoDbContext _mongoDbContext;
        private readonly AppDbContext _appDbContext;
        private readonly FileDbContext _fileDbContext;
        private readonly IMapper _mapper;
        private readonly IYoutubeVideoUploadService _youtubeVideoUploadService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly MinIoConfig _minIoConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserIPAddress _userIPAddress;
        public SocialService(MongoDbContext mongoDbContext, AppDbContext appDbContext, IMapper mapper, IYoutubeVideoUploadService youtubeVideoUploadService, FileDbContext fileDbContext, IPushNotificationService pushNotificationService, IOptions<MinIoConfig> minIoConfig, IHttpContextAccessor httpContextAccessor, IUserIPAddress userIPAddress)
        {
            _mongoDbContext = mongoDbContext;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _youtubeVideoUploadService = youtubeVideoUploadService;
            _fileDbContext = fileDbContext;
            _pushNotificationService = pushNotificationService;
            _minIoConfig = minIoConfig.Value;
            _httpContextAccessor = httpContextAccessor;
            _userIPAddress = userIPAddress;
        }

        #region Manage Post
        public async Task<ISocialResponse> AddPostAsync(BasePostViewModelVisibility model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString() } UserIP: {  _userIPAddress.GetUserIP().Result }");
                var user = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == model.UserID);
                if (user == null)
                {
                    return new SocialResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }
                List<PostView> _res = new List<PostView>();
                List<int?> Gids = new List<int?>();
                List<NetworkGroup> ngs = new List<NetworkGroup>();
                if (model.GroupID != null)
                {
                    char[] spearator = { ',' };
                    var Ids = model.GroupID.Split(spearator);

                    foreach (var item1 in Ids)
                    {
                        if (item1 != "" && item1 != "0")
                        {
                            ngs.Add(_appDbContext.NetworkGroups.FirstOrDefault(a => a.Id == Convert.ToInt32(item1)));
                        }
                        

                    }

                    foreach (var item in ngs.OrderByDescending(a=>a.NameEn).ToList())
                    {
                        
                            Gids.Add(item.Id);
                        
                        //int? gid = (item != "") ? Convert.ToInt32(item) : 0;
                    }
                }
                if (model.IsFollowers)
                {
                    Gids.Add(0);

                }
                else if (model.IsAdminCreated && model.IsPublic)
                {
                    Gids.Add(0);
                }

                
                foreach (var item in Gids)
                {
                    string documentName = "";
                    var bucket = new GridFSBucket(_mongoDbContext.Database);
                    var id = ObjectId.GenerateNewId();
                    //int? gid = (item != "") ? Convert.ToInt32(item) : 0;
                    var post = new Post
                    {
                        ID = id,
                        Text = model.Text,
                        UserID = model.UserID,
                        CreatedBy = user.Email,
                        ModifiedBy = user.Email,
                        TypeID = (int)model.TypeID,
                        IsAdminCreated = model.IsAdminCreated,
                        GroupID = item != 0 ? item : null,
                        IsPublic = model.IsPublic
                    };

                    switch (model.TypeID)
                    {
                        case PostType.Text:
                            post.Text = model.Text;
                            break;
                        case PostType.Image:

                            if (model.ImageData == null)
                            {
                                return new SocialResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                            }
                            await PostImageUploadAsync(model, bucket, post);

                            break;
                        case PostType.Video:
                            post.YoutubeVideoID = model.YoutubeUrl != null
                                ? ExtractVideoIdFromUri(new Uri(model.YoutubeUrl))
                                : "";

                            if (model.VideoData != null)
                            {
                                var videoGuid = Guid.NewGuid();
                                post.YoutubeVideoID = videoGuid.ToString();
                                minioAudioVideoUpload(model.VideoData, videoGuid, true);
                                //post.YoutubeVideoID = await _youtubeVideoUploadService.UploadPostVideoAsync("video", post.ID.ToString(), model.VideoData);
                            }


                            break;
                        case PostType.Poll:

                            if (model.Poll == null)
                            {
                                return new SocialResponse(ClientMessageConstant.PollNotFound, HttpStatusCode.NotFound);
                            }

                            var pollPost = new PollPost()
                            {
                                Id = ObjectId.GenerateNewId(),
                                Question = model.Poll.Question,
                                Answers = model.Poll.Answers.Select(k => new PollAnswer()
                                {
                                    Id = ObjectId.GenerateNewId(),
                                    Answer = k.Answer,
                                    Users = new List<int>()
                                }).ToList()
                            };
                            post.PollPost = pollPost;
                            break;
                        case PostType.File:

                            if (model.DocumentData == null)
                            {
                                return new SocialResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                            }

                            documentName = await PostDocumentUploadAsync(model, bucket, post);
                            break;
                    }

                    await _mongoDbContext.Posts.InsertOneAsync(post);

                    await AddNotificationAsync(post.UserID, post.TypeID, ActionType.AddNewItem, post.ID.ToString(), ParentType.Post, model.UserID);

                    await UpdatePostCountAsync(model.UserID);

                    var postView = _mapper.Map<PostView>(post);

                    postView.DocumentName = documentName;

                    await AddExtrasToPostView(postView, model.UserID);
                    _res.Add(postView);
                }

                return new SocialResponse(_res);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public bool minioAudioVideoUpload(IFormFile formFile, Guid guid, bool isAddNewVideo)
        {
            var appSetting = new MinIoConfig()
            {
                EndPoint = _minIoConfig.EndPoint,
                AccessKey = _minIoConfig.AccessKey,
                SecretKey = _minIoConfig.SecretKey,
                BucketName = _minIoConfig.BucketName,
                Location = _minIoConfig.Location,
                MinIoForDev = _minIoConfig.MinIoForDev,
                FilePath = _minIoConfig.FilePath
            };

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EndPoint : {appSetting.EndPoint}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  AccessKey : {appSetting.AccessKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SecretKey : {appSetting.SecretKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  BucketName : {appSetting.BucketName}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {appSetting.Location}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {appSetting.MinIoForDev}");
            try
            {
                if (appSetting.MinIoForDev != true)
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey).WithSSL();
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, guid, isAddNewVideo, appSetting.FilePath).Wait();
                    return true;
                }
                else
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey);
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, guid, isAddNewVideo, appSetting.FilePath).Wait();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
        private async static Task Run(MinioClient minio, IFormFile _request, string bucketName, string location, Guid guid, bool isAddNewVideo, string fileLocation)
        {
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }
            string FilePath = "";
            using (var fileStream = new FileStream(fileLocation + _request.FileName, FileMode.Create))
            {
                await _request.CopyToAsync(fileStream);
                FilePath = fileStream.Name;
            }

            //var objectName = _request.FileName;
            var objectName = guid.ToString();
            var filePath = FilePath;

            var contentType = "video/mp4";
            //if (fileType == "audio")
            //{
            //    contentType = "audio/mp3";
            //}
            //else if (fileType == "video")
            //{
            //    contentType = "video/mp4";
            //}

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minio.MakeBucketAsync(bucketName, location);
                }
                // Upload a file to bucket.
                if (isAddNewVideo)
                {
                    await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
                    System.IO.File.Delete(filePath);
                } else
                {
                    await minio.RemoveObjectAsync(bucketName, objectName);
                    await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
                    System.IO.File.Delete(filePath);
                }
                
                Console.WriteLine("Successfully uploaded " + objectName);

            }
            catch (MinioException e)
            {
                logger.Error(e);
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }

        }
        private static async Task<string> PostDocumentUploadAsync(BasePostViewModelVisibility model, GridFSBucket bucket, Post post)
        {
            var documentName = model.DocumentFileName + "." + model.DocumentData.ContentType.Split('/')[1];
            GridFSUploadOptions docOption = new GridFSUploadOptions()
            {
                ChunkSizeBytes = (int)model.DocumentData.Length,
                ContentType = model.DocumentData.ContentType
            };
            var fileId = await bucket.UploadFromStreamAsync(documentName, model.DocumentData.OpenReadStream(), docOption);
            post.FileIDs = new List<string>() { fileId.ToString() };
            return documentName;
        }
        private static async Task<string> PostDocumentUploadAsync(BasePostViewModel model, GridFSBucket bucket, Post post)
        {
            var documentName = model.DocumentFileName + "." + model.DocumentData.ContentType.Split('/')[1];
            GridFSUploadOptions docOption = new GridFSUploadOptions()
            {
                ChunkSizeBytes = (int)model.DocumentData.Length,
                ContentType = model.DocumentData.ContentType
            };
            var fileId = await bucket.UploadFromStreamAsync(documentName, model.DocumentData.OpenReadStream(), docOption);
            post.FileIDs = new List<string>() { fileId.ToString() };
            return documentName;
        }

        public async Task<ISocialResponse> EditPostAsync(BasePostViewModel model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIP: {  _userIPAddress.GetUserIP().Result }");
                string documentName = "";

                var post = await _mongoDbContext.Posts.Find(k => k.ID == new ObjectId(model.ID)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                var bucket = new GridFSBucket(_mongoDbContext.Database);

                post.GroupID = model.GroupID != 0 ? model.GroupID : null;
                post.Modified = DateTime.Now;
                post.Text = model.Text;

                switch (model.TypeID)
                {
                    case PostType.Text:
                        post.Text = model.Text;
                        break;
                    case PostType.Image:
                        if (model.ImageData != null)
                        {
                            await PostImageUploadAsync(model, bucket, post);
                        }
                        break;
                    case PostType.Video:

                        if (model.YoutubeUrl != null)
                        {
                            post.YoutubeVideoID = ExtractVideoIdFromUri(new Uri(model.YoutubeUrl));
                        }
                        if (model.VideoData != null)
                        {
                            //post.YoutubeVideoID = await _youtubeVideoUploadService.UploadPostVideoAsync("video", post.ID.ToString(), model.VideoData);
                            minioAudioVideoUpload(model.VideoData, new Guid(post.YoutubeVideoID), false);
                        }
                        break;
                    case PostType.Poll:
                        if (string.IsNullOrWhiteSpace(model.Poll.ID))
                        {
                            return new SocialResponse(ClientMessageConstant.PollNotFound, HttpStatusCode.NotFound);
                        }

                        var pollPost = new PollPost()
                        {
                            Id = new ObjectId(model.Poll.ID),
                            Question = model.Poll.Question,
                            Answers = model.Poll.Answers?.Select(k => new PollAnswer()
                            {
                                Id = string.IsNullOrWhiteSpace(k.ID) ? ObjectId.GenerateNewId() : new ObjectId(k.ID),
                                Answer = k.Answer,
                                Users = string.IsNullOrWhiteSpace(k.ID) ? new List<int>() : post.PollPost.Answers?.FirstOrDefault(o => o.Id == new ObjectId(k.ID))?.Users ?? new List<int>(),
                                Score = string.IsNullOrWhiteSpace(k.ID) ? 0 : post.PollPost.Answers?.FirstOrDefault(o => o.Id == new ObjectId(k.ID))?.Score ?? 0
                            }).ToList()
                        };
                        post.PollPost = pollPost;
                        break;
                    case PostType.File:
                        if (model.DocumentData != null)
                        {
                            documentName = await PostDocumentUploadAsync(model, bucket, post);
                        }

                        break;
                }


                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(model.ID), post);

                await UpdatePostCountAsync(model.UserID);

                var postView = _mapper.Map<PostView>(post);

                postView.DocumentName = documentName;

                await AddExtrasToPostView(postView, model.UserID);

                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        private async Task UpdatePostCountAsync(int userId)
        {
            var posts = await _mongoDbContext.Posts.Find(x => x.UserID == userId && x.IsAdminCreated != true && x.IsDeleted != true).ToListAsync();
            var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
            profile.PostsCount = posts.Count();
            await _appDbContext.SaveChangesAsync();
        }
        private static async Task PostImageUploadAsync(BasePostViewModelVisibility model, GridFSBucket bucket, Post post)
        {
            GridFSUploadOptions options = new GridFSUploadOptions()
            {
                ChunkSizeBytes = (int)model.ImageData.Length,
                ContentType = model.ImageData.ContentType
            };
            var imgFileId =
                await bucket.UploadFromStreamAsync(model.ImageData.FileName, model.ImageData.OpenReadStream(), options);
            post.ImageIDs = new List<string>() { imgFileId.ToString() };
        }
        private static async Task PostImageUploadAsync(BasePostViewModel model, GridFSBucket bucket, Post post)
        {
            GridFSUploadOptions options = new GridFSUploadOptions()
            {
                ChunkSizeBytes = (int)model.ImageData.Length,
                ContentType = model.ImageData.ContentType
            };
            var imgFileId =
                await bucket.UploadFromStreamAsync(model.ImageData.FileName, model.ImageData.OpenReadStream(), options);
            post.ImageIDs = new List<string>() { imgFileId.ToString() };
        }

        public async Task<ISocialResponse> DeletePostAsync(int userId, string postId)
        {
            try
            {
                var isAdmin =
                    await _appDbContext.PermissionSetUsers.AnyAsync(o => o.UserId == userId && o.PermissionSetId == 1);

                var post = await _mongoDbContext.Posts.Find(k => k.ID == new ObjectId(postId) && (k.UserID == userId || isAdmin)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                await _mongoDbContext.Posts.DeleteOneAsync(x => x.ID == new ObjectId(postId) && (x.UserID == userId || isAdmin));

                await _mongoDbContext.Posts.DeleteManyAsync(x => x.PostSharedID == new ObjectId(postId));

                await DeleteNotificationAsync(post.UserID, postId);

                await UpdatePostCountAsync(userId);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostsAsync(GetPostsViewModel postsViewModel)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {postsViewModel.ToJsonString()} UserIP: {  _userIPAddress.GetUserIP().Result }" );
                var posts = await GetAllPostsAsync(postsViewModel.UserId, postsViewModel.FilterType, postsViewModel.Skip, postsViewModel.Limit, postsViewModel.SortBy);

                var postViews = _mapper.Map<List<AllPostView>>(posts);

                foreach (var item in postViews)
                {
                    var com = _mongoDbContext.Posts
                            .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                            .FirstOrDefault();
                    // var post = _mapper.Map<PostView>(post).CommentCount;
                    item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                }

                await ReplaceSharedAllPost(postViews, postsViewModel.UserId);

                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, postsViewModel.UserId);
                }

                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        private async Task ReplaceSharedPost(List<PostView> postViews, int userId)
        {

            foreach (var post in postViews.Where(k => k.TypeID == PostType.Shared))
            {
                await ReplaceSharedPost(post, userId);
            }

        }
        private async Task ReplaceSharedAllPost(List<AllPostView> postViews, int userId)
        {

            foreach (var post in postViews.Where(k => k.TypeID == PostType.Shared))
            {
                await ReplaceSharedPost(post, userId);
            }

        }
        private async Task ReplaceSharedPost(AllPostView post, int userId)
        {

            if (post.TypeID == PostType.Shared)
            {
                var originalPost = await _mongoDbContext.Posts.Find(k => k.ID == new ObjectId(post.PostSharedID))
                    .FirstOrDefaultAsync();
                if (originalPost == null)
                {
                    return;
                }

                post.ImageIDs = originalPost.ImageIDs;
                post.FileIDs = originalPost.FileIDs;
                post.Poll = _mapper.Map<PollView>(originalPost.PollPost);
                post.YoutubeVideoID = originalPost.YoutubeVideoID;
                post.Text = originalPost.Text;
                post.TypeID = (PostType)originalPost.TypeID;
                post.IsAdminPostShared = originalPost.IsAdminCreated;
                post.Survey = _mapper.Map<SurveyView>(originalPost.SurveyPost);
                //post.IsAdminCreated =
                //    await _appDbContext.PermissionSetUsers.AnyAsync(o => o.UserId == originalPost.UserID && o.PermissionSetId == 1);

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == originalPost.UserID);
                if (profile == null)
                {
                    return;
                }

                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == originalPost.UserID);

                var mongoUser = await _mongoDbContext.Users.Find(m => m.Id == userId).FirstOrDefaultAsync();
                var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                    .Where(k => k.ProfileId == originalPost.UserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                post.SharedUserDetails = new PublicProfileView()
                {
                    Id = profile.Id,
                    FirstNameAr = post.IsAdminPostShared ? "مشرف" : profile.FirstNameAr,
                    FirstNameEn = post.IsAdminPostShared ? "Admin" : profile.FirstNameEn,
                    LastNameAr = post.IsAdminPostShared ? "" : profile.LastNameAr,
                    LastNameEn = post.IsAdminPostShared ? "" : profile.LastNameEn,
                    SecondNameAr = profile.SecondNameAr,
                    SecondNameEn = profile.SecondNameEn,
                    ThirdNameAr = profile.ThirdNameAr,
                    ThirdNameEn = profile.ThirdNameEn,

                    FollowersCount = profile.FollowersCount,
                    FollowingCount = profile.FollowingCount,
                    PostCount = profile.PostsCount,

                    LPSPoint = profile.Lpspoints,
                    CompletePercentage = profile.CompletenessPercentage,
                    IsAmFollowing = mongoUser?.FollowingIDs?.Contains(profile.Id) ?? false,
                    Designation = workExperience?.Title?.TitleEn,
                    DesignationAr = workExperience?.Title?.TitleAr,
                    UserImageFileId = user.OriginalImageFileId ?? 0

                };
            }

        }
        private async Task ReplaceSharedPost(PostView post, int userId)
        {

            if (post.TypeID == PostType.Shared)
            {
                var originalPost = await _mongoDbContext.Posts.Find(k => k.ID == new ObjectId(post.PostSharedID))
                    .FirstOrDefaultAsync();
                if (originalPost == null)
                {
                    return;
                }

                post.ImageIDs = originalPost.ImageIDs;
                post.FileIDs = originalPost.FileIDs;
                post.Poll = _mapper.Map<PollView>(originalPost.PollPost);
                post.YoutubeVideoID = originalPost.YoutubeVideoID;
                post.Text = originalPost.Text;
                post.TypeID = (PostType)originalPost.TypeID;
                post.IsAdminPostShared = originalPost.IsAdminCreated;
                post.Survey = _mapper.Map<SurveyView>(originalPost.SurveyPost);
                //post.IsAdminCreated =
                //    await _appDbContext.PermissionSetUsers.AnyAsync(o => o.UserId == originalPost.UserID && o.PermissionSetId == 1);

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == originalPost.UserID);
                if (profile == null)
                {
                    return;
                }

                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == originalPost.UserID);

                var mongoUser = await _mongoDbContext.Users.Find(m => m.Id == userId).FirstOrDefaultAsync();
                var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                    .Where(k => k.ProfileId == originalPost.UserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                post.SharedUserDetails = new PublicProfileView()
                {
                    Id = profile.Id,
                    FirstNameAr = post.IsAdminPostShared ? "مشرف" : profile.FirstNameAr,
                    FirstNameEn = post.IsAdminPostShared ? "Admin" : profile.FirstNameEn,
                    LastNameAr = post.IsAdminPostShared ? "" : profile.LastNameAr,
                    LastNameEn = post.IsAdminPostShared ? "" : profile.LastNameEn,
                    SecondNameAr = profile.SecondNameAr,
                    SecondNameEn = profile.SecondNameEn,
                    ThirdNameAr = profile.ThirdNameAr,
                    ThirdNameEn = profile.ThirdNameEn,

                    FollowersCount = profile.FollowersCount,
                    FollowingCount = profile.FollowingCount,
                    PostCount = profile.PostsCount,

                    LPSPoint = profile.Lpspoints,
                    CompletePercentage = profile.CompletenessPercentage,
                    IsAmFollowing = mongoUser?.FollowingIDs?.Contains(profile.Id) ?? false,
                    Designation = workExperience?.Title?.TitleEn,
                    DesignationAr = workExperience?.Title?.TitleAr,
                    UserImageFileId = user.OriginalImageFileId ?? 0

                };
            }

        }
        private async Task<List<Post>> GetAllPostsAsync(int userId, List<int> groupIds, int skip, int limit, SortType sortBy)
        {
            List<Post> posts = new List<Post>();
            var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync()) ??
                            new MongoModels.User() { };


            var userIds = new List<int>() { userId };
            userIds.AddRange(mongoUser.FollowingIDs);
            userIds.AddRange(mongoUser.MyFavouriteProfilesIDs);

            posts = await _mongoDbContext.Posts.Find(k => k.IsPinned).SortBy(m => m.PinOrder).ToListAsync();

            var Ngroup = await _appDbContext.NetworkGroupProfiles
                 .Join(_appDbContext.NetworkGroups,
                       ngp => ngp.NetworkGroupId, ng => ng.Id,
                       (ngp, ng) => new { ngp, ng })
                 .Where(z => z.ngp.ProfileId == userId)
                 .Select(z => z.ngp.NetworkGroupId).ToListAsync();

            if (sortBy == SortType.Desc)
            {

                if (groupIds.Contains(-1))
                {
                    posts.AddRange(await _mongoDbContext.Posts
                        .Find(_ => (_.IsPublic || _.IsAdminCreated || userIds.Contains(_.UserID))  && (_.IsDeleted != true && _.GroupID == null)).SortByDescending(x => x.Created)
                        .ToListAsync());

                    if (Ngroup != null && Ngroup.Count > 0)
                    {
                        posts.AddRange(await _mongoDbContext.Posts
                            .Find(_ => (_.GroupID != null && Ngroup.Contains(_.GroupID.Value) || (_.IsAdminCreated && Ngroup.Contains(_.GroupID.Value))) && (_.IsDeleted != true)
                            ).SortByDescending(x => x.Created)
                            .ToListAsync());
                    }

                    posts = posts.OrderByDescending(a => a.Created).Skip(skip).Take(limit).ToList(); 
                   

                }

                else if (groupIds.Any())
                {
                    if (groupIds.Count == 1 && groupIds.Contains(0))
                    {
                        return await _mongoDbContext.Posts
                            .Find(_ => _.IsAdminCreated && _.GroupID == null && _.IsDeleted != true).SortByDescending(x => x.Created)
                            .Skip(skip).Limit(limit).ToListAsync();
                    }
                    else if (groupIds.Count > 1 && groupIds.Contains(0))
                    {
                        posts.AddRange(await _mongoDbContext.Posts
                            .Find(_ => _.IsAdminCreated && _.GroupID == null && _.IsDeleted != true).SortByDescending(x => x.Created)
                            .ToListAsync());

                        groupIds.Remove(0);

                        posts.AddRange(await _mongoDbContext.Posts
                       .Find(_ => (_.GroupID != null && groupIds.Contains(_.GroupID.Value)) && (_.IsDeleted != true)
                       ).SortByDescending(x => x.Created)
                       .ToListAsync());

                        posts = posts.OrderByDescending(a => a.Created).Skip(skip).Take(limit).ToList();
                        return posts;

                    }
                    groupIds.Remove(-1);

                    posts.AddRange(await _mongoDbContext.Posts
                        .Find(_ => (_.GroupID != null && groupIds.Contains(_.GroupID.Value) || (_.IsAdminCreated && groupIds.Contains(_.GroupID.Value))) && ( _.IsDeleted != true)
                        ).SortByDescending(x => x.Created)
                        .Skip(skip).Limit(limit).ToListAsync());

                }

            }
            else
            {
                if (groupIds.Contains(-1))
                {
                    posts.AddRange(await _mongoDbContext.Posts
                         .Find(_ => (_.IsPublic || _.IsAdminCreated || userIds.Contains(_.UserID)) && (_.IsDeleted != true && _.GroupID == null)).SortBy(x => x.Created)
                         .ToListAsync());

                    if (Ngroup != null && Ngroup.Count > 0)
                    {
                        posts.AddRange(await _mongoDbContext.Posts
                            .Find(_ => (_.GroupID != null && Ngroup.Contains(_.GroupID.Value) || (_.IsAdminCreated && Ngroup.Contains(_.GroupID.Value))) && (_.IsDeleted != true)
                            ).SortBy(x => x.Created)
                            .ToListAsync());
                    }

                    posts = posts.OrderBy(a => a.Created).Skip(skip).Take(limit).ToList();

                }
                else if (groupIds.Any())
                {
                    if (groupIds.Count == 1 && groupIds.Contains(0))
                    {
                        return await _mongoDbContext.Posts
                            .Find(_ => _.IsAdminCreated && _.IsDeleted != true).SortBy(x => x.Created)
                            .Skip(skip).Limit(limit).ToListAsync();
                    }
                    else if (groupIds.Count > 1 && groupIds.Contains(0))
                    {
                        posts.AddRange(await _mongoDbContext.Posts
                            .Find(_ => _.IsAdminCreated && _.GroupID == null && _.IsDeleted != true).SortBy(x => x.Created)
                            .ToListAsync());

                        groupIds.Remove(0);

                        posts.AddRange(await _mongoDbContext.Posts
                       .Find(_ => (_.GroupID != null && groupIds.Contains(_.GroupID.Value)) && (_.IsDeleted != true)
                       ).SortBy(x => x.Created)
                       .ToListAsync());

                        posts = posts.OrderBy(a => a.Created).Skip(skip).Take(limit).ToList();
                        return posts;

                    }
                    groupIds.Remove(-1);
                    posts.AddRange(await _mongoDbContext.Posts
                        .Find(_ => (_.GroupID != null && groupIds.Contains(_.GroupID.Value) || (_.IsAdminCreated && groupIds.Contains(_.GroupID.Value))) &&  (_.IsDeleted != true)
                        ).SortBy(x => x.Created)
                        .Skip(skip).Limit(limit).ToListAsync());
                }

            }
            

            return posts;
        }

        public async Task<ISocialResponse> GetPostAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(k => k.ID == ObjectId.Parse(postId)).FirstOrDefaultAsync();
                if (post == null)
                {
                    return new SocialResponse(ClientMessageConstant.PollNotFound, HttpStatusCode.NotFound);
                }

                var postView = _mapper.Map<PostView>(post);
                await ReplaceSharedPost(postView, userId);
                await AddExtrasToPostView(postView, userId);

                
                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetFilterTypeAsync(int userId)
        {
            try
            {
                var filter = new List<FilterTypeViewModel>()
                {
                    new FilterTypeViewModel
                    {
                        Id = -1,
                        FilterType = "ALL POST",
                        NameEn = "ALL POST",
                        NameAr = "كل وظيفة"
                    },
                    new FilterTypeViewModel
                    {
                        Id = 0,
                        FilterType = "ADMIN POST",
                        NameEn = "ADMIN POST",
                        NameAr =  "كل وظيفة"
                    }
                };


                var person = await _appDbContext.NetworkGroupProfiles
                 .Join(_appDbContext.NetworkGroups,
                       ngp => ngp.NetworkGroupId, ng => ng.Id,
                       (ngp, ng) => new { ngp, ng })
                 .Where(z => z.ngp.ProfileId == userId)
                 .Select(z => new FilterTypeViewModel
                 {
                     Id = z.ngp.NetworkGroupId,
                     FilterType = z.ng.NameEn,
                     NameEn = z.ng.NameEn,
                     NameAr = z.ng.NameAr,
                     DescriptionAr = z.ng.DescriptionAr,
                     DescriptionEn = z.ng.DescriptionEn,
                     LogoId = z.ng.LogoId ?? 0
                 }).OrderBy(a=>a.NameEn).ToListAsync();

                filter.AddRange(person);

                return new SocialResponse(filter);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetMyPostsAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                var posts = await _mongoDbContext.Posts.Find(x => x.UserID == userId && x.IsAdminCreated != true && x.IsDeleted != true).ToListAsync();

                //var postViews = _mapper.Map<List<PostView>>(posts.OrderByDescending(x => x.Created).ToList());
                //await ReplaceSharedPost(postViews, userId);
                //foreach (var item in postViews)
                //{
                //    await AddExtrasToPostView(item, userId);
                //}


                var postViews = _mapper.Map<List<AllPostView>>(posts.OrderByDescending(x => x.Created).Skip(skip).Take(limit).ToList());

                foreach (var item in postViews)
                {
                    var com = _mongoDbContext.Posts
                            .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                            .FirstOrDefault();
                    // var post = _mapper.Map<PostView>(post).CommentCount;
                    item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                }

                await ReplaceSharedAllPost(postViews, userId);

                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }
                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostsByAdminCreatedAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                var posts = await _mongoDbContext.Posts.Find(x => x.IsAdminCreated)
                    .Skip(skip).Limit(limit).ToListAsync();

                var postViews = _mapper.Map<List<PostView>>(posts);
                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }
                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostsByGroupCreatedAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                var posts = await _mongoDbContext.Posts.Find(x => x.IsGroupCreated == true)
                    .Skip(skip).Limit(limit).ToListAsync();
                var postViews = _mapper.Map<List<PostView>>(posts);
                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }
                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostsSortedByDateAscendingAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                var posts = await _mongoDbContext.Posts.Find(_ => true).SortBy(x => x.Created).Skip(skip).Limit(limit).ToListAsync();
                var postViews = _mapper.Map<List<PostView>>(posts);
                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }
                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostsSortedByDateDescendingAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                var posts = await _mongoDbContext.Posts.Find(_ => true).SortByDescending(x => x.Created).Skip(skip).Limit(limit).ToListAsync();
                var postViews = _mapper.Map<List<PostView>>(posts);
                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }
                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostsAsync(int userId, string text = "", int skip = 0, int limit = 5)
        {
            try
            {

                var posts = await _mongoDbContext.Posts.Find(x => x.Text.Trim().ToLower().Contains(text.Trim().ToLower()))
                    .Skip(skip).Limit(limit).ToListAsync();

                var postViews = _mapper.Map<List<PostView>>(posts);
                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }
                return new SocialResponse(postViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        //public async Task<ISocialResponse> SearchPostsAsync(string text = "", int skip = 0, int limit = 5)
        //{
        //    try
        //    {
        //        var users = _appDbContext.Users.Where(x => x.NameAr.Trim().ToLower().Contains(text.Trim().ToLower()) ||
        //        x.NameEn.Trim().ToLower().Contains(text.Trim().ToLower())).ToList();

        //        List<PostView> postViewLists = new List<PostView>();
        //        if (users.Count > 0)
        //        {
        //            foreach (var user in users)
        //            {
        //                //var searchposts = await (await _mongoDbContext.Rooms.FindAsync(_ => true)).ToListAsync();
        //                //var userposts1 = await _mongoDbContext.Posts.Find(x => x.UserID == user.Id)
        //                //.Skip(skip).Limit(limit).ToListAsync();

        //                var userposts = await _mongoDbContext.Posts.Find(x => x.UserID == user.Id)
        //                .Skip(skip).Limit(limit).ToListAsync();
        //                var userpostViews = _mapper.Map<List<PostView>>(userposts);
        //                foreach (var item in userpostViews)
        //                {
        //                    await AddExtrasToPostView(item, user.Id);
        //                }
        //                postViewLists.AddRange(userpostViews);

        //                var postsWithMatchingUserComments = await _mongoDbContext.Posts.Find(x => x.Comments.Any(y => y.UserID == user.Id))
        //            .Skip(skip).Limit(limit).ToListAsync();

        //                var userCommandpostViews = _mapper.Map<List<PostView>>(postsWithMatchingUserComments);
        //                foreach (var item in userCommandpostViews)
        //                {
        //                    await AddExtrasToPostView(item, user.Id);
        //                }
        //                 postViewLists.AddRange(userCommandpostViews);
        //            }
        //        }

        //            var posts = await _mongoDbContext.Posts.Find(x => x.Text.ToLower().Contains(text.ToLower().Trim()))
        //                .Skip(skip).Limit(limit).ToListAsync();
        //            var postViews = _mapper.Map<List<PostView>>(posts);
        //            foreach (var item in postViews)
        //            {
        //                await AddExtrasToPostView(item, item.UserID);
        //            }
        //            postViewLists.AddRange(postViews);

        //            var pollposts = await _mongoDbContext.Posts.Find(x => x.PollPost.Question.ToLower().Contains(text.ToLower().Trim()))
        //            .Skip(skip).Limit(limit).ToListAsync();
        //            var pollpostViews = _mapper.Map<List<PostView>>(pollposts);
        //            foreach (var item in pollpostViews)
        //            {
        //                await AddExtrasToPostView(item, item.UserID);
        //            }
        //            postViewLists.AddRange(pollpostViews);

        //        var postsWithMatchingComments = await _mongoDbContext.Posts.Find(x => x.Comments.Any(y => y.Text.ToLower().Contains(text.ToLower().Trim())))
        //            .Skip(skip).Limit(limit).ToListAsync();

        //        var postViewsWithMatchingComments = _mapper.Map<List<PostView>>(postsWithMatchingComments);
        //        foreach (var item in postViewsWithMatchingComments)
        //        {
        //            await AddExtrasToPostView(item, item.UserID);
        //        }
        //        postViewLists.AddRange(postViewsWithMatchingComments);

        //        return new SocialResponse(postViewLists.Distinct().ToList());
        //    }
        //    catch (Exception e)
        //    {
        //        return new SocialResponse(e);
        //    }
        //}

        public async Task<ISocialResponse> SearchPostsAsync(int userId, string text = "", int skip = 0, int limit = 5)
        {
            try
            {
                //var users = _appDbContext.Users.Where(x => x.Id == userId);

                var userSearch = _appDbContext.Users.Where(x => x.Id == userId);
                if (userSearch != null)
                {


                    var mongoUser = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                    if (mongoUser == null)
                    {
                        mongoUser = new User() { Id = userId };
                        await _mongoDbContext.Users.InsertOneAsync(mongoUser);
                    }

                    var userViewModel = _mapper.Map<MongoModels.User>(mongoUser);

                    var users = await _appDbContext.Users.Where(k =>
                        (userViewModel.FollowingIDs.Contains(k.Id) || k.Id == userViewModel.Id) && (string.IsNullOrWhiteSpace(text)
                                                                      || (text.IsEnglishString() ? k.NameEn.Contains(text) : k.NameAr.Contains(text))
                                                                      )).OrderBy(k => k.NameEn)
                                                .ToListAsync();

                    var userIds = users.Select(k => k.Id).ToList();

                    List<AllPostView> postViewLists = new List<AllPostView>();
                    if (userIds.Count > 0)
                    {
                        foreach (var user in userIds)
                        {
                            //var searchposts = await (await _mongoDbContext.Rooms.FindAsync(_ => true)).ToListAsync();
                            //var userposts1 = await _mongoDbContext.Posts.Find(x => x.UserID == user.Id)
                            //.Skip(skip).Limit(limit).ToListAsync();
                            
                            var userposts = await _mongoDbContext.Posts.Find(x => x.UserID == user).ToListAsync();
                            //var userpostViews = _mapper.Map<List<PostView>>(userposts);
                            var userpostViews = _mapper.Map<List<AllPostView>>(userposts);

                            foreach (var item in userpostViews)
                            {
                                await AddExtrasToPostView(item, user);

                                var com = _mongoDbContext.Posts
                                        .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                        .FirstOrDefault();
                                // var post = _mapper.Map<PostView>(post).CommentCount;
                                item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                            }
                            postViewLists.AddRange(userpostViews);

                            var postsWithMatchingUserComments = await _mongoDbContext.Posts.Find(x => x.Comments.Any(y => y.UserID == user)).ToListAsync();
                            //var userCommandpostViews = _mapper.Map<List<PostView>>(postsWithMatchingUserComments);
                            //foreach (var item in userCommandpostViews)
                            //{
                            //    await AddExtrasToPostView(item, user);
                            //}
                            var userCommandpostViews = _mapper.Map<List<AllPostView>>(postsWithMatchingUserComments);

                            foreach (var item in userCommandpostViews)
                            {
                                await AddExtrasToPostView(item, user);
                                var com = _mongoDbContext.Posts
                                        .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                        .FirstOrDefault();
                                // var post = _mapper.Map<PostView>(post).CommentCount;
                                item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                            }
                            
                            postViewLists.AddRange(userCommandpostViews);
                        }
                    }

                    var users1 = await _appDbContext.Users.Where(k => userViewModel.FollowingIDs.Contains(k.Id)).OrderBy(k => k.NameEn).ToListAsync();

                    var userIds1 = users1.Select(k => k.Id).ToList();

                    var posts = await _mongoDbContext.Posts.Find(x => x.Text.ToLower().Contains(text.ToLower().Trim()) && (userIds1.Contains(x.UserID) || x.IsAdminCreated || x.UserID == userId)).ToListAsync();
                    var postViews = _mapper.Map<List<AllPostView>>(posts);

                    foreach (var item in postViews)
                    {
                        await AddExtrasToPostView(item, userId);
                        var com = _mongoDbContext.Posts
                                .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                .FirstOrDefault();
                        // var post = _mapper.Map<PostView>(post).CommentCount;
                        item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                    }

                    
                    postViewLists.AddRange(postViews);

                    var postShared = await _mongoDbContext.Posts.Find(x => x.Text.ToLower().Contains(text.ToLower().Trim()) && (userIds1.Contains(x.UserID) || x.IsAdminCreated || x.UserID == userId)).ToListAsync();

                    foreach(var sharedItem in postShared)
                    {
                        var sharedPost = await _mongoDbContext.Posts.Find(x => x.PostSharedID == sharedItem.ID).ToListAsync();
                        var postSharedViews = _mapper.Map<List<AllPostView>>(sharedPost);
                        foreach (var item in postSharedViews)
                        {
                            await ReplaceSharedPost(item, item.UserID);
                            await AddExtrasToPostView(item, userId);
                            var com = _mongoDbContext.Posts
                                    .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                    .FirstOrDefault();
                            // var post = _mapper.Map<PostView>(post).CommentCount;
                            item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                        }
                        postViewLists.AddRange(postSharedViews);
                    }
                    
                    var pollposts = await _mongoDbContext.Posts.Find(x => x.PollPost.Question.ToLower().Contains(text.ToLower().Trim()) && (userIds1.Contains(x.UserID) || x.IsAdminCreated || x.UserID == userId)).ToListAsync();
                   
                    var pollpostViews = _mapper.Map<List<AllPostView>>(pollposts);

                    foreach (var item in pollpostViews)
                    {
                        await AddExtrasToPostView(item, userId);
                        var com = _mongoDbContext.Posts
                                .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                .FirstOrDefault();
                        // var post = _mapper.Map<PostView>(post).CommentCount;
                        item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                    }
                    postViewLists.AddRange(pollpostViews);

                    var postsWithMatchingComments = await _mongoDbContext.Posts.Find(x => x.Comments.Any(y => y.Text.ToLower().Contains(text.ToLower().Trim())) && (userIds1.Contains(x.UserID) || x.IsAdminCreated || x.UserID == userId)).ToListAsync();

                   
                    var postViewsWithMatchingComments = _mapper.Map<List<AllPostView>>(postsWithMatchingComments);

                    foreach (var item in postViewsWithMatchingComments)
                    {
                        await AddExtrasToPostView(item, userId);
                        var com = _mongoDbContext.Posts
                                .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                .FirstOrDefault();
                        // var post = _mapper.Map<PostView>(post).CommentCount;
                        item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                    }
                    postViewLists.AddRange(postViewsWithMatchingComments);

                    var surveyposts = await _mongoDbContext.Posts.Find(x => (x.SurveyPost.Text.ToLower().Contains(text.ToLower().Trim()) || x.SurveyPost.LinkURLDescription.ToLower().Contains(text.ToLower().Trim())) && (userIds1.Contains(x.UserID) || x.IsAdminCreated || x.UserID == userId)).ToListAsync();

                    var surveyViews = _mapper.Map<List<AllPostView>>(surveyposts);

                    foreach (var item in surveyViews)
                    {
                        await AddExtrasToPostView(item, userId);
                        var com = _mongoDbContext.Posts
                                .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                                .FirstOrDefault();
                        // var post = _mapper.Map<PostView>(post).CommentCount;
                        item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                    }
                    postViewLists.AddRange(surveyViews);

                    var result = new SocialResponse(postViewLists.DistinctBy(x => x.ID).OrderByDescending(x => x.Created).Skip(skip).Take(limit).ToList())
                    {
                        SearchPostCout = postViewLists.DistinctBy(x => x.ID).Count()
                    };
                    return result;
                }
                else
                {
                    return new SocialResponse(new List<AllPostView>());
                }
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }


        public async Task<ISocialResponse> GetPostsByUserFavoriteAsync(int userId, int skip = 0, int limit = 5)
        {
            try
            {
                var posts = await _mongoDbContext.Posts.Find(x => x.Favorites != null && x.Favorites.Contains(userId))
                    .SortByDescending(x=> x.FavoritesTimestamp.First(x => x.UserID == userId).Datetime).Skip(skip).Limit(limit).ToListAsync();
                 posts.Reverse();
                var postCount = await _mongoDbContext.Posts.Find(x => x.Favorites != null && x.Favorites.Contains(userId)).ToListAsync();
                //var postViews = _mapper.Map<List<PostView>>(posts);
                //await ReplaceSharedPost(postViews, userId);
                //foreach (var item in postViews)
                //{
                //    await AddExtrasToPostView(item, userId);
                //}

                var postViews = _mapper.Map<List<AllPostView>>(posts);

                foreach (var item in postViews)
                {
                    var com = _mongoDbContext.Posts
                            .Find(_ => _.ID == new ObjectId(item.ID)).SortBy(x => x.Created)
                            .FirstOrDefault();
                    // var post = _mapper.Map<PostView>(post).CommentCount;
                    item.CommentCount = _mapper.Map<PostView>(com).CommentCount;
                }

                await ReplaceSharedAllPost(postViews, userId);

                foreach (var item in postViews)
                {
                    await AddExtrasToPostView(item, userId);
                }

                //return new SocialResponse(postViews);
                var result = new SocialResponse(postViews)
                {
                    TotalFavoriteCount = postCount.Count()
                };
                return result;
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddPostLikeAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                if (!post.Likes.Contains(userId))
                {

                    post.Likes.Add(userId);

                    post.Likes = post.Likes.Distinct().ToList();

                    post.LikesTimestamp.Add(new TimeStampModel
                    {
                        UserID = userId,
                        Datetime = DateTime.UtcNow
                    });
                }

                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == post.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                {
                    await AddNotificationAsync(post.UserID, post.TypeID, ActionType.Like, post.ID.ToString(), ParentType.Post, userId);
                }
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                var postView = _mapper.Map<PostView>(post);

                await ReplaceSharedPost(postView, userId);
                await AddExtrasToPostView(postView, userId);
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  call completed");
                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeletePostLikeAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                if (!post.Likes.Any())
                    return new SocialResponse();

                if (post.Likes.Contains(userId))
                {
                    post.Likes.Remove(userId);
                    post.LikesTimestamp.Remove(post.LikesTimestamp.First(x => x.UserID == userId));
                }

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                await DeleteNotificationAsync(post.UserID, postId, ActionType.Like);

                var data = _mapper.Map<PostView>(post);

                await AddExtrasToPostView(data, userId);



                return new SocialResponse(data);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddPostDislikeAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                if (!post.Dislikes.Contains(userId))
                    post.Dislikes.Add(userId);

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeletePostDislikeAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                if (!post.Dislikes.Any())
                    return new SocialResponse();
                if (post.Dislikes.Contains(userId))
                    post.Dislikes.Remove(userId);
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddPostFavoriteAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                if (!post.Favorites.Contains(userId))
                {
                    post.Favorites.Add(userId);
                    post.FavoritesTimestamp.Add(new TimeStampModel
                    {
                        UserID = userId,
                        Datetime = DateTime.UtcNow
                    });
                }

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeletePostFavoriteAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                if (!post.Favorites.Any())
                    return new SocialResponse();

                if (post.Favorites.Contains(userId))
                {
                    post.Favorites.Remove(userId);
                    post.FavoritesTimestamp.Remove(post.FavoritesTimestamp.First(x => x.UserID == userId));
                }

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddPostShareAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                if (post.TypeID == (int)PostType.Shared)
                {
                    post = await _mongoDbContext.Posts.Find(x => x.ID == post.PostSharedID).FirstOrDefaultAsync();
                    if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                }

                post.Shares.Add(userId);
                post.SharesTimestamp.Add(new TimeStampModel
                {
                    UserID = userId,
                    Datetime = DateTime.UtcNow
                });

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == post.ID, post);

                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == post.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                {
                    await AddNotificationAsync(post.UserID, post.TypeID, ActionType.Share, post.ID.ToString(), ParentType.Post, userId);
                }

                var sharePost = await SharePostAsync(userId, post);
                var postView = _mapper.Map<PostView>(sharePost);

                await UpdatePostCountAsync(userId);

                await ReplaceSharedPost(postView, userId);
                await AddExtrasToPostView(postView, userId);

                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        private async Task<Post> SharePostAsync(int userId, Post post)
        {

            var sharePost = new Post()
            {
                UserID = userId,
                ID = ObjectId.GenerateNewId(),
                TypeID = (int)PostType.Shared,
                PostSharedID = post.ID
            };
            await _mongoDbContext.Posts.InsertOneAsync(sharePost);
            return sharePost;
        }

        public async Task<ISocialResponse> GetSharedUsersDetailsAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(k => k.ID == ObjectId.Parse(postId)).FirstOrDefaultAsync();
                if (post == null)
                {
                    return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                }

                var mongoUser = await _mongoDbContext.Users.Find(m => m.Id == userId).FirstOrDefaultAsync();

                var profileList = new List<PublicProfileView>();
                // var sharedProfiles = await _appDbContext.Profiles.Where(k => post.Shares.Contains(k.Id)).ToListAsync();

                foreach (var id in post.Shares)
                {
                    await FillUserDetailsAsync(id, profileList, mongoUser);
                }



                return new SocialResponse(profileList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task FillUserDetailsAsync(int id, List<PublicProfileView> profileList, User mongoUser)
        {
            var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == id);
            if (profile == null)
            {
                return;
            }

            var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                .Where(k => k.ProfileId == id).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
            var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == id);
            profileList.Add(new PublicProfileView()
            {
                Id = profile.Id,
                FirstNameAr = profile.FirstNameAr,
                FirstNameEn = profile.FirstNameEn,
                LastNameAr = profile.LastNameAr,
                LastNameEn = profile.LastNameEn,
                SecondNameAr = profile.SecondNameAr,
                SecondNameEn = profile.SecondNameEn,
                ThirdNameAr = profile.ThirdNameAr,
                ThirdNameEn = profile.ThirdNameEn,

                FollowersCount = profile.FollowersCount,
                FollowingCount = profile.FollowingCount,
                PostCount = profile.PostsCount,

                LPSPoint = profile.Lpspoints,
                CompletePercentage = profile.CompletenessPercentage,
                IsAmFollowing = mongoUser.FollowingIDs?.Contains(id) ?? false,
                Designation = workExperience?.Title?.TitleEn,
                DesignationAr = workExperience?.Title?.TitleAr,
                UserImageFileId = user.OriginalImageFileId ?? 0
            });
        }


        public async Task<ISocialResponse> GetLikedUsersDetailsAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(k => k.ID == ObjectId.Parse(postId)).FirstOrDefaultAsync();
                if (post == null)
                {
                    return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                }

                var mongoUser = await _mongoDbContext.Users.Find(m => m.Id == userId).FirstOrDefaultAsync();

                var profileList = new List<PublicProfileView>();
                // var sharedProfiles = await _appDbContext.Profiles.Where(k => post.Shares.Contains(k.Id)).ToListAsync();

                foreach (var id in post.Likes)
                {
                    await FillUserDetailsAsync(id, profileList, mongoUser);
                }

                return new SocialResponse(profileList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ISocialResponse> DeletePostShareAsync(string postId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                if ((post.Shares == null) || (post.SharesTimestamp == null))
                    return new SocialResponse();
                if (post.Shares.Contains(userId))
                {
                    post.Shares.Remove(userId);
                    post.SharesTimestamp.Remove(post.SharesTimestamp.First(x => x.UserID == userId));
                }

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                await DeleteNotificationAsync(post.UserID, postId, ActionType.Share);

                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddPostCommentAsync(CommentView view, string postId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                var comment = _mapper.Map<Comment>(view);
                comment.Id = ObjectId.GenerateNewId();
                post.Comments.Add(comment);

                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == post.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                {
                    await AddNotificationAsync(post.UserID, post.TypeID, ActionType.Comment, post.ID.ToString(), ParentType.Post, view.UserID);
                }

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                var postView = _mapper.Map<PostView>(post);

                await ReplaceSharedPost(postView, view.UserID);
                await AddExtrasToPostView(postView, view.UserID);

                postView.Comments = postView.Comments.Where(k => k.Id == comment.Id.ToString()).ToList();

                return new SocialResponse(postView);

            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> EditPostCommentAsync(CommentView model, string postId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                var exstingComment = post.Comments.FirstOrDefault(k => k.Id == new ObjectId(model.Id));
               

                var comment = new Comment()
                {
                    Id = exstingComment.Id,
                    UserID = model.UserID,
                    Text = model.Text,
                    Created = exstingComment.Created,
                    Reports = exstingComment.Reports,
                    Modified = DateTime.Now,
                    IsAdminCreated = exstingComment.IsAdminCreated,
                    IsDeleted = exstingComment.IsDeleted
                };
                post.Comments.Remove(exstingComment);
                post.Comments.Add(comment);
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                var postView = _mapper.Map<PostView>(post);

                await ReplaceSharedPost(postView, model.UserID);
                await AddExtrasToPostView(postView, model.UserID);

                postView.Comments = postView.Comments.Where(k => k.Id == comment.Id.ToString()).ToList();
                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeletePostCommentAsync(string postId, string commentId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                var comment = post.Comments.FirstOrDefault(x => x.Id == new ObjectId(commentId));
                post.Comments.Remove(comment);
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);


                await DeleteNotificationAsync(post.UserID, postId, ActionType.Comment);

                var postView = _mapper.Map<PostView>(post);

                await ReplaceSharedPost(postView, userId);
                await AddExtrasToPostView(postView, userId);

                post.Comments = new List<Comment>();

                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> ReportPostCommentAsync(List<ReportView> models, string postId, string commentId, int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {models.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);


                var comment = post.Comments.FirstOrDefault(k => k.Id == new ObjectId(commentId));

                if (comment == null)
                {
                    return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                }

                var reports = _mapper.Map<List<Report>>(models);

                reports.ForEach(k => k.ID = ObjectId.GenerateNewId());

                post.Comments.Remove(comment);
                comment.Reports.AddRange(reports);
                post.Comments.Add(comment);

                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                var postView = _mapper.Map<PostView>(post);
                await ReplaceSharedPost(postView, userId);
                await AddExtrasToPostView(postView, userId);

                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetPostCommentsAsync(string postId, int skip, int limit)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {postId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                if (!post.Comments.Any())
                    return new SocialResponse();

                var postView = _mapper.Map<PostView>(post);

                var comments = await FillCommentsAsync(postView);
                var commentsView = comments.Skip(skip).Take(limit).ToList();

                var result = new SocialResponse(commentsView)
                {
                    TotalCommentCount = comments.Count()
                };
                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddPostReportAsync(List<ReportView> models, string postId, int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {models.ToJsonString()} postId:{postId} userId :{userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();

                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                var report = _mapper.Map<List<Report>>(models);

                report.ForEach(k => k.ID = ObjectId.GenerateNewId());
                post.Reports.AddRange(report);
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                var postView = _mapper.Map<PostView>(post);
                await AddExtrasToPostView(postView, userId);

                return new SocialResponse(postView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeletePostReportAsync(string reportId, string postId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                if (!post.Reports.Any())
                    return new SocialResponse();

                post.Reports.Remove(post.Reports.FirstOrDefault(x => x.ID == new BsonObjectId(new ObjectId(reportId))));
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }


        public async Task<ISocialResponse> UpdatePostPollScoreAsync(string postId, string answerId, int userId)
        {
            try
            {
                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);
                //if (post.TypeID == 5)
                //{
                //    var OriginalPost = await _mongoDbContext.Posts.Find(x => x.ID == post.PostSharedID).FirstOrDefaultAsync();
                //    if (OriginalPost.PollPost == null) return new SocialResponse(ClientMessageConstant.PollNotFound, HttpStatusCode.NotFound);

                //    foreach (var item in OriginalPost.PollPost.Answers)
                //    {
                //        item.Users ??= new List<int>();
                //        if (item.Users.Any() && item.Users.Contains(userId))
                //        {
                //            item.Users.Remove(userId);
                //            item.Score = item.Score - 1;
                //            break;
                //        }
                //    }

                //    var answer = OriginalPost.PollPost.Answers.FirstOrDefault(x => x.Id == new ObjectId(answerId));
                //    if (answer != null)
                //    {
                //        answer.Score = answer.Score + 1;
                //        answer.Users?.Add(userId);
                //    }

                //    await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == OriginalPost.ID, OriginalPost);

                //    var data = _mapper.Map<PostView>(post);

                //    await ReplaceSharedPost(data, userId);

                //    await AddExtrasToPostView(data, userId);

                //    return new SocialResponse(data);
                //}
                //else
                //{

                    if (post.PollPost == null) return new SocialResponse(ClientMessageConstant.PollNotFound, HttpStatusCode.NotFound);

                    foreach (var item in post.PollPost.Answers)
                    {
                        item.Users ??= new List<int>();
                        if (item.Users.Any() && item.Users.Contains(userId))
                        {
                            item.Users.Remove(userId);
                            item.Score = item.Score - 1;
                            break;
                        }
                    }

                    var answer = post.PollPost.Answers.FirstOrDefault(x => x.Id == new ObjectId(answerId));
                    if (answer != null)
                    {
                        answer.Score = answer.Score + 1;
                        answer.Users?.Add(userId);
                    }

                    await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                    var data = _mapper.Map<PostView>(post);

                    await AddExtrasToPostView(data, userId);

                    return new SocialResponse(data);
                //}
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> MakeItPublicAsync(string postId, int userId)
        {
            try
            {
                var user = await _appDbContext.PermissionSetUsers.FirstOrDefaultAsync(k => k.UserId == userId && k.PermissionSetId == 1);
                if (user == null)
                {
                    return new SocialResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var post = await _mongoDbContext.Posts.Find(x => x.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                if (post == null) return new SocialResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                post.IsPublic = true;
                await _mongoDbContext.Posts.ReplaceOneAsync(x => x.ID == new ObjectId(postId), post);

                var data = _mapper.Map<PostView>(post);

                await AddExtrasToPostView(data, userId);

                return new SocialResponse(data);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }

        }

        #endregion

        #region Manger User Profile
        public async Task<ISocialResponse> AddUserFollowerAsync(int userId, int followerId)
        {
            try
            {
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                user.FollowersIDs.Add(followerId);
                user.FollowersIDs = user.FollowersIDs.Distinct().ToList();
                var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);

                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteUserFollowerAsync(int userId, int followerId)
        {
            try
            {
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);
                if (!user.FollowersIDs.Any()) return new SocialResponse();

                user.FollowersIDs = user.FollowersIDs.Distinct().ToList();

                user.FollowersIDs.Remove(followerId);
                var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);

                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddUserFollowingAsync(int userId, int followingId)
        {
            try
            {
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                user.FollowingIDs.Add(followingId);

                user.FollowingIDs = user.FollowingIDs.Distinct().ToList();

                var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);

                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteUserFollowingAsync(int userId, int followingId)
        {
            try
            {
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);
                if (!user.FollowingIDs.Any()) return new SocialResponse();

                user.FollowingIDs = user.FollowingIDs.Distinct().ToList();

                user.FollowingIDs.Remove(followingId);

                var result = await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);

                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddUserFavoriteProfileAsync(int userId, int profileId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                user.MyFavouriteProfilesIDs.Add(profileId);

                user.MyFavouriteProfilesIDs = user.MyFavouriteProfilesIDs.Distinct().ToList();

                await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteUserFavoriteProfileAsync(int userId, int profileId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);
                if (!user.MyFavouriteProfilesIDs.Any()) return new SocialResponse();
                user.MyFavouriteProfilesIDs = user.MyFavouriteProfilesIDs.Distinct().ToList();
                user.MyFavouriteProfilesIDs.Remove(profileId);
                await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }
        public async Task<ISocialResponse> AddUserProfileViewerAsync(int userId, int viewerId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);
                if (user.Viewers == null) user.Viewers = new List<Viewer>();

                user.Viewers.Add(new Viewer
                {
                    ViewerID = viewerId,
                    Date = DateTime.UtcNow
                });

                await _mongoDbContext.Users.ReplaceOneAsync(x => x.Id == userId, user);
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetUserFollowersAndFollowingAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var user = await _mongoDbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) return new SocialResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                return new SocialResponse(_mapper.Map<UserView>(user));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }
        #endregion

        #region Manage Notifiations
        public async Task<ISocialResponse> GetNotificationObjectAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name} {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIP: {  _userIPAddress.GetUserIP().Result }");
                var notificationObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ?? await AddNotificationObjectAsync(userId);

                //notificationObject.UnseenNotificationCounter = 0;

                //await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(k => k.UserID == userId, notificationObject);

                notificationObject.Notifications =
                    notificationObject.Notifications.Where(k => k.SenderID != userId).ToList();

                var notificationObjectView = _mapper.Map<NotificationGenericObjectView>(notificationObject);

                await FillNotificationUserDetailsAsync(userId, notificationObjectView.NotificationsList);

                return new SocialResponse(notificationObjectView);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new SocialResponse(e);
            }
        }

        public string GetUserIP()
        {
            var hostname = _httpContextAccessor.HttpContext.Request.Host.Value;


            var ip = (_httpContextAccessor.HttpContext.Request.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") != null
            && _httpContextAccessor.HttpContext.Request.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") != "")
            ? _httpContextAccessor.HttpContext.Request.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR")
            : _httpContextAccessor.HttpContext.Request.HttpContext.GetServerVariable("REMOTE_ADDR");
            if (ip.Contains(","))
                ip = ip.Split(',').First().Trim();
            return ip;
        }
        //private static string GetUserIPs()
        //{
        //    //var request = HttpRequest.Request;
        //    string ipList = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(ipList))
        //    {
        //        return ipList.Split(',')[0];
        //    }

        //    return Request.ServerVariables["REMOTE_ADDR"];
        //}
        //public string GetIPAddress()
        //{
        //    System.Web.HttpContext context = System.Web.HttpContext.Current;
        //    string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(ipAddress))
        //    {
        //        string[] addresses = ipAddress.Split(',');
        //        if (addresses.Length != 0)
        //        {
        //            return addresses[0];
        //        }
        //    }

        //    return context.Request.ServerVariables["REMOTE_ADDR"];
        //}

        public  string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public async Task<ISocialResponse> GetNotificationUnseenCountObjectAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var notificationObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ?? await AddNotificationObjectAsync(userId);

                //var notificationObjectView = _mapper.Map<NotificationUnseenCountView>(notificationObject);

                return new SocialResponse(notificationObject.UnseenNotificationCounter);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }


        private async Task FillNotificationUserDetailsAsync(int userId, List<NotificationView> notificationsList)
        {
            try
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
                    if (notification.ParentTypeID == ParentType.Meetup)
                    {
                        var meetup = await _appDbContext.Meetups.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        notification.titleEn = meetup?.Title;
                    }

                    if (notification.ParentTypeID == ParentType.Challenge)
                    {
                        var activity = await _appDbContext.Initiatives.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        notification.titleEn = activity?.TitleEn;
                        notification.titleAr = activity?.TitleAr;
                        notification.categoryID = activity?.CategoryId != null ? activity?.CategoryId : activity?.InitiativeTypeItemId;
                    }

                    if (notification.ParentTypeID == ParentType.EngagementActivities)
                    {
                        var activity = await _appDbContext.Initiatives.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        notification.titleEn = activity?.TitleEn;
                        notification.titleAr = activity?.TitleAr;
                        notification.categoryID = activity?.CategoryId != null ? activity?.CategoryId : activity?.InitiativeTypeItemId;
                    }

                    if (notification.ParentTypeID == ParentType.Programme)
                    {
                        var programme = await _appDbContext.Programmes.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        notification.titleEn = programme?.TitleEn;
                        notification.titleAr = programme?.TitleAr;
                    }

                    if (notification.ParentTypeID == ParentType.Batch)
                    {
                        var batch = await _appDbContext.Batches.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        if (batch != null)
                        {
                            var titleEn = await _appDbContext.Programmes.Where(x => x.Id == batch.ProgrammeId).Select(x => x.TitleEn).FirstOrDefaultAsync();
                            var titleAr = await _appDbContext.Programmes.Where(x => x.Id == batch.ProgrammeId).Select(x => x.TitleAr).FirstOrDefaultAsync();
                            notification.titleEn = titleEn ?? "";
                            notification.titleAr = titleAr ?? "";
                        }
                    }

                    if (notification.ParentTypeID == ParentType.Group)
                    {
                        var networkGroup = await _appDbContext.NetworkGroups.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        var titleEn = networkGroup?.NameEn;
                        var titleAr = networkGroup?.NameAr;
                        notification.titleEn = titleEn;
                        notification.titleAr = titleAr;
                    }

                    if (notification.ParentTypeID == ParentType.AssessmentGroup)
                    {
                        var assesmentGroupMember = await _appDbContext.AssessmentGroupMembers.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        if (assesmentGroupMember != null)
                        {
                            var assessmentGroup = await _appDbContext.AssessmentGroups.Where(x => x.Id == assesmentGroupMember.AssessmentGroupId).FirstOrDefaultAsync();
                            notification.titleEn = assessmentGroup?.NameEn;
                            notification.titleAr = assessmentGroup?.NameAr;
                        }
                    }

                    if (notification.ParentTypeID == ParentType.AssignedAssessment)
                    {
                        var assessment = await _appDbContext.AssessmentTools.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        notification.titleEn = assessment?.NameEn;
                        notification.titleAr = assessment?.NameAr;
                    }

                    if (notification.ParentTypeID == ParentType.AssessmentCoordinator)
                    {
                        var assesmentGroupMember = await _appDbContext.AssessmentGroupMembers.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        if (assesmentGroupMember != null)
                        {
                            var assessmentGroup = await _appDbContext.AssessmentGroups.Where(x => x.Id == assesmentGroupMember.AssessmentGroupId).FirstOrDefaultAsync();
                            notification.titleEn = assessmentGroup?.NameEn;
                            notification.titleAr = assessmentGroup?.NameAr;
                        }
                    }
                    if (notification.ParentTypeID == ParentType.Messaging)
                    {
                        var room = await _mongoDbContext.Rooms.Find(x => x.Messages.Any(x => x.ID == new ObjectId(notification.ParentID))).FirstOrDefaultAsync();

                        if (room != null)
                        {
                            notification.ParentID = room.ID.ToString();
                        }
                    }
                    if (notification.ParentTypeID == ParentType.Event)
                    {
                        var events = await _appDbContext.Events.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                        if (events != null)
                        {
                            //var assessmentGroup = await _appDbContext.AssessmentGroups.Where(x => x.Id == assesmentGroupMember.AssessmentGroupId).FirstOrDefaultAsync();
                            notification.titleEn = events.TextEn;
                            notification.titleAr = events?.TextAr;
                            var date = await _appDbContext.EventDays.Where(x => x.EventId == events.Id).FirstOrDefaultAsync();
                            notification.UserNameEn = date?.StartTime.ToString("yyyy-MM-dd'T'HH:mm:ss");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        private async Task FillActivityUserDetailsAsync(int userId, List<NotificationView> notificationsList)
        {
            foreach (var notification in notificationsList)
            {
                if (notification.ParentTypeID != ParentType.User)
                {
                    continue;
                }

                int.TryParse(notification.ParentID, out int notifyUserId);

                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == notifyUserId);
                notification.UserNameEn = user?.NameEn;
                notification.UserNameAr = user?.NameAr;
                notification.UserImageFileId = user?.OriginalImageFileId ?? 0;
                notification.RedirectUrlPath = notification.ParentTypeID == ParentType.User
                    ? $"/api/Profile/get-public-profile/{userId}/{notifyUserId}"
                    : notification.RedirectUrlPath;
            }
        }


        public async Task<ISocialResponse> AddNotificationAsync(int userId, int typeId, ActionType actionId, string parentPostId, ParentType parentTypeId, int senderUserId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(userId);
                var notificationGenericObjectActivity = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == senderUserId).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(senderUserId);

                //var profileId = userId != senderUserId ? senderUserId : userId;

                //                                await AddNotificationObjectAsync(userId);

                var notificationObj = new Notification
                {
                    ID = ObjectId.GenerateNewId(),
                    ActionID = (int)actionId,
                    IsRead = false,
                    ParentID = parentPostId,
                    ParentTypeID = (int)parentTypeId,
                    SenderID = senderUserId,
                    IsPushed = true
                   // PostTypeID = typeId
                };

                notificationGenericObject.Notifications.Add(notificationObj);
                notificationGenericObjectActivity.Notifications.Add(notificationObj);

                if (userId != senderUserId)
                {
                    notificationGenericObject.UnseenNotificationCounter += 1;
                    var notificationView = _mapper.Map<NotificationView>(notificationObj);
                    await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView });

                    PushNotification(notificationView, userId);
                    logger.Info("Notification sent");
                    
                    await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == senderUserId, notificationGenericObjectActivity);
                }

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);

                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return new SocialResponse(notificationGenericObjectView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<bool> PushNotification(NotificationView notification, int userId)
        {
            var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
            foreach (var deviceId in deviceIds)
            {
                await _pushNotificationService.SendPushNotificationAsync(notification, deviceId);
            }
            return true;
        }

            public async Task<ISocialResponse> UpdateNotificationAsReadAsync(int userId, string notificationId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} notificationId:{notificationId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ?? await AddNotificationObjectAsync(userId);

                notificationGenericObject.Notifications.Where(x => x.ID == new ObjectId(notificationId)).ToList().ForEach(k => k.IsRead = true);

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);

                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return new SocialResponse(notificationGenericObjectView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> UpdateUnseenNotificationAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ?? await AddNotificationObjectAsync(userId);


                notificationGenericObject.UnseenNotificationCounter = 0;

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);

                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return new SocialResponse(notificationGenericObjectView);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
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

        private async Task DeleteNotificationAsync(int userId, string postId, ActionType action)
        {
            try
            {
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync();

                if (notificationGenericObject == null) return;

                var notification = notificationGenericObject.Notifications.Find(k => k.ParentID == postId && k.ActionID == (int)action);

                notificationGenericObject.Notifications.Remove(notification);

                notificationGenericObject.UnseenNotificationCounter = (notificationGenericObject.UnseenNotificationCounter - 1).ToUint();

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(k => k.UserID == userId, notificationGenericObject);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task DeleteNotificationAsync(int userId, string postId)
        {
            try
            {
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync();

                if (notificationGenericObject == null) return;

                var count = notificationGenericObject.Notifications.Count(k => k.ParentID == postId);

                notificationGenericObject.Notifications.RemoveAll(k => k.ParentID == postId);

                notificationGenericObject.UnseenNotificationCounter = (notificationGenericObject.UnseenNotificationCounter - count).ToUint();

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(k => k.UserID == userId, notificationGenericObject);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region file upload
        public async Task<ISocialResponse> UploadFile(string filename, Stream file)
        {
            try
            {
                var bucket = new GridFSBucket(_mongoDbContext.Database);
                var fileId = await bucket.UploadFromStreamAsync(filename, file);
                return new SocialResponse(new List<string> { fileId.ToString() });
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetFile(string fileId)
        {
            try
            {
                if (fileId == null) throw new ArgumentException("fileId is null");
                var bucket = new GridFSBucket(_mongoDbContext.Database);
                var file = await bucket.DownloadAsBytesAsync(new BsonObjectId(new ObjectId(fileId)).Value);
                return new SocialResponse(file);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }
        #endregion

        #region Manage Rooms
        public async Task<ISocialResponse> GetRoomAsync(int userId)
        {
            try
            {
                var rooms = await _mongoDbContext.Rooms.Find(_ => true).ToListAsync();
                return new SocialResponse(_mapper.Map<List<RoomView>>(rooms));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetRoomAsync(string roomId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddRoomAsync(RoomView view)
        {
            try
            {
                var room = _mapper.Map<Room>(view);
                await _mongoDbContext.Rooms.InsertOneAsync(room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteRoomAsync(string roomId)
        {
            try
            {
                await _mongoDbContext.Rooms.DeleteOneAsync(x => x.ID == new ObjectId(roomId));
                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddRoomMemberAsync(string roomId, int userId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                room.MembersIDs.Add(userId);
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteRoomMemberAsync(string roomId, int userId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                if (room.MembersIDs == null) return new SocialResponse(_mapper.Map<RoomView>(room));

                room.MembersIDs.Remove(userId);
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddRoomArchiveMemberAsync(string roomId, int memberId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                room.ArchivedMembersIDs.Add(memberId);
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteRoomArchiveMemberAsync(string roomId, int memberId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                if (!room.ArchivedMembersIDs.Any()) return new SocialResponse(_mapper.Map<RoomView>(room));

                room.ArchivedMembersIDs.Remove(memberId);
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddRoomMessageAsync(string roomId, MessageView message)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                room.Messages.Add(_mapper.Map<Message>(message));
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> DeleteRoomMessageAsync(string roomId, string messageId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                if (!room.Messages.Any()) return new SocialResponse(_mapper.Map<RoomView>(room));

                room.Messages.Remove(room.Messages.FirstOrDefault(x => x.ID == new ObjectId(messageId)));
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new SocialResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }
        #endregion

        public async Task<ISocialResponse> GetActivityLogsAsync(int userId, int take, int page)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var activityLog = await _mongoDbContext.NotificationGenericObjects.Find(k => k.UserID == userId).FirstOrDefaultAsync() ??
                                  await AddNotificationObjectAsync(userId);

                var activityViews = _mapper.Map<List<NotificationView>>(activityLog.Notifications.Where(k => k.SenderID == userId).OrderByDescending(y => y.Modified).Skip((page - 1) * take).Take(take).ToList());

                await FillActivityUserDetailsAsync(userId, activityViews);

                var result = new SocialResponse(activityViews)
                {
                    TotalActivityLogCount = activityLog.Notifications.Count(k => k.SenderID == userId)
                };

                return result;
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> GetAnnouncementsAsync()
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var announcements = await _appDbContext.Announcements.Where(k =>k.IsActive).OrderBy(x => x.Order).ToListAsync();

                List<AnnouncementView> announcementViews = new List<AnnouncementView>();

                foreach (var item in announcements)
                {
                    AnnouncementView view = new AnnouncementView();
                    view.Id = item.Id;
                    view.UserId = item.UserId;
                    view.ImageArId = item.ImageArId;
                    view.ImageEnId = item.ImageArId;
                    view.LinkAr = item.LinkAr;
                    view.LinkEn = item.LinkEn;
                    view.MobileLink = item.MobileNavigationField;
                    view.IsActive = item.IsActive;
                    view.Order = item.Order;
                    view.isHighlights = item.Highlights;
                    view.MobileLinkNameEN = item.MobileLinkNameEN;
                    view.MobileLinkNameAR = item.MobileLinkNameAR;
                    view.MobileLinkDescriptionEN = item.MobileLinkDescriptionEN;
                    view.MobileLinkDescriptionAR = item.MobileLinkDescriptionAR;
                    announcementViews.Add(view);
                }

                return new SocialResponse(announcementViews);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        #region Survey

        public async Task<ISocialResponse> GetSurveyQuestionsAsync(int surveyId, int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId}  surveyId : {surveyId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                if (await _appDbContext.SurveyProfileInfos.AnyAsync(
                    k => k.ProfileId == userId && k.SurveyId == surveyId))
                {
                    var survey = await _appDbContext.Surveys.FirstOrDefaultAsync(k => k.Id == surveyId);

                    return new SocialResponse(new SurveyQuestionViewModel()
                    {
                        IsSurveyAlreadySubmitted = true,
                        TitleEn = survey?.TitleEn,
                        TitleAr = survey?.TitleAr,
                        DescriptionAr = survey?.DescriptionAr,
                        DescriptionEn = survey?.DescriptionEn,
                        IsPublished = survey?.IsPublished ?? false
                    });
                }

                var data = await _appDbContext.Surveys.Include(k => k.SurveyQuestions).Include(k => k.QuestionGroup)
                    .Include(k => k.SurveyProfileInfos).Include(k => k.SurveyProfileQuestionAnswers).Where(k => k.Id == surveyId).Select(k =>
                          new SurveyQuestionViewModel()
                          {
                              SurveyId = k.Id,
                              DescriptionAr = k.DescriptionAr,
                              DescriptionEn = k.DescriptionEn,
                              IsPublished = k.IsPublished,
                              TitleAr = k.TitleAr,
                              TitleEn = k.TitleEn,
                              SurveyQuestions = k.SurveyQuestions.Where(m => m.IsVisible).Select(m => new SurveyQuestionView()
                              {
                                  Id = m.Id,
                                  AcceptedFiles = m.AcceptedFiles,
                                  DisplayOrder = m.DisplayOrder,
                                  IsRequired = m.IsRequired,
                                  IsVisible = m.IsVisible,
                                  PlaceHolderAr = m.PlaceHolderAr,
                                  PlaceHolderEn = m.PlaceHolderEn,
                                  QuestionTextAr = m.QuestionTextAr,
                                  QuestionTextEn = m.QuestionTextEn,
                                  QuestionTypeItemId = m.QuestionTypeItemId,
                                  MaxFileSize = m.MaxFileSize,
                                  MaxLength = m.MaxLength,
                                  SurveyID = m.SurveyId,
                                  SurveyQuestionFields = m.SurveyQuestionFieldSurveyQuestions.Select(y => new SurveyQuestionFieldView()
                                  {
                                      Id = y.Id,
                                      DisplayOrder = y.DisplayOrder,
                                      SurveyQuestionId = y.SurveyQuestionId,
                                      TextAr = y.TextAr,
                                      TextEn = y.TextEn,
                                      LinkedSurveyQuestionId = y.LinkedSurveyQuestionId
                                  }).ToList()

                              }).OrderBy(o => o.DisplayOrder).ToList()

                          }).FirstOrDefaultAsync();

                return new SocialResponse(data);
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        public async Task<ISocialResponse> AddUserSurveyAnswersAsync(SurveyAnswersView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == model.ProfileId);

                if (userInfo == null)
                {
                    return new SocialResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var surveyProfileInfo = new SurveyProfileInfo()
                {
                    ProfileId = model.ProfileId,
                    SurveyId = model.SurveyId,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email
                };

                await _appDbContext.SurveyProfileInfos.AddAsync(surveyProfileInfo);
                await _appDbContext.SaveChangesAsync();

                foreach (var answer in model.AnswerList)
                {
                    switch (answer.QuestionTypeItemId)
                    {
                        case SurveyQuestionType.StarRating:
                        case SurveyQuestionType.SingleTextBox:
                        case SurveyQuestionType.MultilineTextBox:
                        case SurveyQuestionType.DateTime:
                        case SurveyQuestionType.YesNo:
                            await SaveDirectAnswerAsync(model, answer, userInfo, surveyProfileInfo);
                            break;
                        case SurveyQuestionType.RadioButtons:
                        case SurveyQuestionType.Dropdown:
                            await SaveSingleSelectionAsync(model, answer, userInfo, surveyProfileInfo);
                            break;
                        case SurveyQuestionType.MultipleChoice:
                        case SurveyQuestionType.Checkboxes:
                            await SaveMultiSelectionAsync(model, answer, userInfo, surveyProfileInfo);
                            break;
                        case SurveyQuestionType.FileUpload:
                            await SaveFileSelectionAsync(model, answer, userInfo, surveyProfileInfo);
                            break;

                    }
                }

                return new SocialResponse();
            }
            catch (Exception e)
            {
                return new SocialResponse(e);
            }
        }

        private async Task SaveFileSelectionAsync(SurveyAnswersView model, SurveyAnswerViewModel answer, UserInfo userInfo,
            SurveyProfileInfo surveyProfileInfo)
        {
            var file = new File()
            {
                IdGuid = Guid.NewGuid(),
                SizeMb = answer.FileData.Length.ToFileMB(),
                Name = answer.FileData.FileName,
                ProviderName = "SqlProvider",
                ExtraParams = "",
                Created = DateTime.UtcNow,
                MimeType = answer.FileData.ContentType,
                Modified = DateTime.UtcNow,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email
            };

            await _appDbContext.Files.AddAsync(file);
            await _appDbContext.SaveChangesAsync();

            var fileDb = new FileDB()
            {
                Id = file.IdGuid,
                Bytes = answer.FileData.OpenReadStream().ToBytes()
            };

            await _fileDbContext.FileDB.AddAsync(fileDb);
            await _fileDbContext.SaveChangesAsync();


            var data = new SurveyProfileQuestionAnswer
            {
                ProfileId = model.ProfileId,
                SurveyId = model.SurveyId,
                SurveryQusetionId = answer.QuestionId,
                SurveryQuestionTypeItemId = (int)answer.QuestionTypeItemId,
                Value = file.Id.ToString(),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email,
                SurveyProfileInfoId = surveyProfileInfo.Id
            };

            await _appDbContext.SurveyProfileQuestionAnswers.AddAsync(data);
            await _appDbContext.SaveChangesAsync();
        }

        private async Task SaveMultiSelectionAsync(SurveyAnswersView model, SurveyAnswerViewModel answer, UserInfo userInfo,
            SurveyProfileInfo surveyProfileInfo)
        {
            var data = new SurveyProfileQuestionAnswer
            {
                ProfileId = model.ProfileId,
                SurveyId = model.SurveyId,
                SurveryQusetionId = answer.QuestionId,
                SurveryQuestionTypeItemId = (int)answer.QuestionTypeItemId,
                Value = string.Join(',', answer.MultiSelectionIds),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email,
                SurveyProfileInfoId = surveyProfileInfo.Id
            };

            await _appDbContext.SurveyProfileQuestionAnswers.AddAsync(data);
            await _appDbContext.SaveChangesAsync();

            foreach (var id in answer.MultiSelectionIds)
            {
                await _appDbContext.SurveyProfileQuestionAnswerSurveyQuestionFields.AddAsync(
                    new SurveyProfileQuestionAnswerSurveyQuestionField()
                    {
                        SurveyProfileQuestionAnswerId = data.Id,
                        SurveyQuestionFieldId = id
                    });
                await _appDbContext.SaveChangesAsync();
            }
        }

        private async Task SaveSingleSelectionAsync(SurveyAnswersView model, SurveyAnswerViewModel answer, UserInfo userInfo,
            SurveyProfileInfo surveyProfileInfo)
        {
            var data = new SurveyProfileQuestionAnswer
            {
                ProfileId = model.ProfileId,
                SurveyId = model.SurveyId,
                SurveryQusetionId = answer.QuestionId,
                SurveryQuestionTypeItemId = (int)answer.QuestionTypeItemId,
                Value = answer.SingleSelectionId.ToString(),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email,
                SurveyProfileInfoId = surveyProfileInfo.Id
            };

            await _appDbContext.SurveyProfileQuestionAnswers.AddAsync(data);
            await _appDbContext.SaveChangesAsync();


            var ansAndQues = new SurveyProfileQuestionAnswerSurveyQuestionField()
            {
                SurveyProfileQuestionAnswerId = data.Id,
                SurveyQuestionFieldId = answer.SingleSelectionId
            };

            await _appDbContext.SurveyProfileQuestionAnswerSurveyQuestionFields.AddAsync(ansAndQues);
            await _appDbContext.SaveChangesAsync();
        }

        private async Task SaveDirectAnswerAsync(SurveyAnswersView model, SurveyAnswerViewModel answer, UserInfo userInfo,
            SurveyProfileInfo surveyProfileInfo)
        {
            var data = new SurveyProfileQuestionAnswer
            {
                ProfileId = model.ProfileId,
                SurveyId = model.SurveyId,
                SurveryQusetionId = answer.QuestionId,
                SurveryQuestionTypeItemId = (int)answer.QuestionTypeItemId,
                Value = answer.Answer,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email,
                SurveyProfileInfoId = surveyProfileInfo.Id
            };

            await _appDbContext.SurveyProfileQuestionAnswers.AddAsync(data);
            await _appDbContext.SaveChangesAsync();
        }

        #endregion

        #region private methods
        private async Task AddExtrasToPostView(AllPostView post, int userId)
        {

            var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == post.UserID);
            var job = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title).Where(k => k.ProfileId == post.UserID).OrderByDescending(k => k.DateFrom)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                post.NameEn = user.NameEn;
                post.NameAr = user.NameAr;
                post.TitleEn = job?.Title?.TitleEn;
                post.TitleAr = job?.Title?.TitleAr;
                post.UserImageFileId = user.OriginalImageFileId ?? 0;
            }
            post.Liked = post.Likes?.Contains(userId) ?? false;
            post.IsAmCommented = post.Comments?.Select(y => y.UserID).Contains(userId) ?? false;
            post.Favorited = post.Favorites?.Contains(userId) ?? false;
            post.DocumentName = post.FileIDs.Any() ? await GetFilenameAsync(post.FileIDs[0]) : "";

            var mongoUser = await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync();

            post.IsAmFollowing = mongoUser?.FollowingIDs.Contains(post.UserID) ?? false;

            post.Comments = await FillCommentsAsync(post);

            if (post.GroupID != null && post.GroupID != 0)
            {
                var Groups = _appDbContext.NetworkGroups.FirstOrDefault(a => a.Id == post.GroupID);
                post.GroupNameEN = Groups.NameEn ?? "";
                post.GroupNameAR = Groups.NameAr ?? "";
            }

            if (post.TypeID == PostType.Poll)
            {
                post.IsAnsweredThisPoll = post.Poll.Answers?.Where(k => k.Users != null).SelectMany(k => k.Users).ToList()
                    .Contains(userId) ?? false;
            }
        }
        private async Task AddExtrasToPostView(PostView post, int userId)
        {

            var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == post.UserID);
            var job = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title).Where(k => k.ProfileId == post.UserID).OrderByDescending(k => k.DateFrom)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                post.NameEn = user.NameEn;
                post.NameAr = user.NameAr;
                post.TitleEn = job?.Title?.TitleEn;
                post.TitleAr = job?.Title?.TitleAr;
                post.UserImageFileId = user.OriginalImageFileId ?? 0;
            }
            post.Liked = post.Likes?.Contains(userId) ?? false;
            post.IsAmCommented = post.Comments?.Select(y=>y.UserID).Contains(userId) ?? false;
            post.Favorited = post.Favorites?.Contains(userId) ?? false;
            post.DocumentName = post.FileIDs.Any() ? await GetFilenameAsync(post.FileIDs[0]) : "";

            var mongoUser = await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync();

            post.IsAmFollowing = mongoUser?.FollowingIDs.Contains(post.UserID) ?? false;

            post.Comments = await FillCommentsAsync(post);

            if (post.GroupID != null)
            {
                var Groups = _appDbContext.NetworkGroups.FirstOrDefault(a => a.Id == post.GroupID);
                post.GroupNameEN = Groups.NameEn ?? "";
                post.GroupNameAR = Groups.NameAr ?? "";
            }

            if (post.TypeID == PostType.Poll)
            {
                post.IsAnsweredThisPoll = post.Poll.Answers?.Where(k => k.Users != null).SelectMany(k => k.Users).ToList()
                    .Contains(userId) ?? false;
            }
        }
        private async Task<List<CommentView>> FillCommentsAsync(AllPostView post)
        {
            List<CommentView> comments = new List<CommentView>();
            foreach (var item in post.Comments)
            {
                item.PostId = post.ID;
                var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == item.UserID);
                var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
                    .Where(k => k.ProfileId == item.UserID).OrderByDescending(k => k.DateFrom)
                    .FirstOrDefaultAsync();
                var commentView = _mapper.Map<CommentView>(item);
                if (commentUser != null)
                {
                    commentView.NameEn = commentUser.NameEn;
                    commentView.NameAr = commentUser.NameAr;
                    commentView.TitleEn = commentUserJob?.Title?.TitleEn;
                    commentView.TitleAr = commentUserJob?.Title?.TitleAr;
                    commentView.UserImageFileId = commentUser.OriginalImageFileId ?? 0;
                }

                comments.Add(commentView);
            }

            post.Comments = comments;
            return comments.OrderBy(k => k.Created).ToList();
        }
        private async Task<List<CommentView>> FillCommentsAsync(PostView post)
        {
            List<CommentView> comments = new List<CommentView>();
            foreach (var item in post.Comments)
            {
                item.PostId = post.ID;
                var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == item.UserID);
                var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
                    .Where(k => k.ProfileId == item.UserID).OrderByDescending(k => k.DateFrom)
                    .FirstOrDefaultAsync();
                var commentView = _mapper.Map<CommentView>(item);
                if (commentUser != null)
                {
                    commentView.NameEn = commentUser.NameEn;
                    commentView.NameAr = commentUser.NameAr;
                    commentView.TitleEn = commentUserJob?.Title?.TitleEn;
                    commentView.TitleAr = commentUserJob?.Title?.TitleAr;
                    commentView.UserImageFileId = commentUser.OriginalImageFileId ?? 0;
                }

                comments.Add(commentView);
            }

            post.Comments = comments;
            return comments.OrderBy(k => k.Created).ToList();
        }

        private async Task<string> GetFilenameAsync(string fileId)
        {

            try
            {

                var bucket = new GridFSBucket(_mongoDbContext.Database);
                using (var stream = await bucket.OpenDownloadStreamAsync(new ObjectId(fileId)))
                {
                    return stream?.FileInfo.Filename;
                }
            }
            catch (Exception e)
            {
                return "";
            }

        }

        public string ExtractVideoIdFromUri(Uri uri)
        {
            try
            {
                Regex regexExtractId = new Regex("(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+", RegexOptions.Compiled);
                string[] validAuthorities = new string[4]
                {
                    "youtube.com",
                    "www.youtube.com",
                    "youtu.be",
                    "www.youtu.be"
                };

                string lower = new UriBuilder(uri).Uri.Authority.ToLower();
                if (validAuthorities.Contains<string>(lower))
                {
                    Match match = regexExtractId.Match(uri.ToString());
                    if (match.Success)
                        return match.Groups[1].Value;
                }
            }
            catch
            {
            }
            return (string)null;
        }

        //private List<SurveyQuestionView> GetSurveyQuestionsAsync(int surveyId)
        //{
        //    try
        //    {
        //        var surveyQuestions = _appDbContext.SurveyQuestions.Where(sq => sq.SurveyId == surveyId).ToList().Select(sqv =>
        //        new SurveyQuestionView
        //        {
        //            Id = sqv.Id,
        //            QuestionTypeItemId = sqv.QuestionTypeItemId,
        //            QuestionTextEn = sqv.QuestionTextEn,
        //            QuestionTextAr = sqv.QuestionTextAr,
        //            PlaceHolderAr = sqv.PlaceHolderAr,
        //            PlaceHolderEn = sqv.PlaceHolderEn,
        //            DisplayOrder = sqv.DisplayOrder,
        //            IsRequired = sqv.IsRequired,
        //            IsVisible = sqv.IsVisible
        //        }).ToList();

        //        foreach (var surveyQuestion in surveyQuestions)
        //        {
        //            surveyQuestion.SurveyQuestionFields = GetSurveyQuestionFieldViews(surveyQuestion.Id);
        //        }
        //        return surveyQuestions;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        private List<SurveyQuestionFieldView> GetSurveyQuestionFieldViews(int surveyQuestionId)
        {
            try
            {
                var surveyQuestionFields = _appDbContext.SurveyQuestionFields.Where(sqf => sqf.SurveyQuestionId == surveyQuestionId).Select(sqf =>
                  new SurveyQuestionFieldView
                  {
                      Id = sqf.Id,
                      SurveyQuestionId = sqf.SurveyQuestionId,
                      TextEn = sqf.TextEn,
                      TextAr = sqf.TextAr,
                      DisplayOrder = sqf.DisplayOrder,
                      LinkedSurveyQuestionId = sqf.LinkedSurveyQuestionId
                  }).ToList();
                return surveyQuestionFields;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}
