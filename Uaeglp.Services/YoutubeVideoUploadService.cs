using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Services
{
    public class YoutubeVideoUploadService : IYoutubeVideoUploadService
    {
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IEncryptionManager _encryption;
        private readonly YoutubeClientSecret _youtubeClientSecret;
        public YoutubeVideoUploadService(AppDbContext appDbContext, IEncryptionManager encryption, IOptions<YoutubeClientSecret> settings, MongoDbContext mongoDbContext)
        {
            _appDbContext = appDbContext;
            _encryption = encryption;
            _mongoDbContext = mongoDbContext;
            _youtubeClientSecret = settings.Value;
        }



        public async Task<string> UploadPostVideoAsync(string description,  
            string postId, IFormFile file)
        {
            string[] tags1 = new string[1] { postId };
            var title = file.FileName;
            PrivacyStatus privacyStatus = PrivacyStatus.Unlisted;
           return await RunPostAsync(title, description, tags1, privacyStatus, file);
        }

        public void UploadProfileVideoTask(string description,
            string profileId, IFormFile file)
        {
            string[] tags1 = new string[1] { profileId };
            var title = file.FileName;
            PrivacyStatus privacyStatus = PrivacyStatus.Unlisted;
            Run(title, description, tags1, privacyStatus, file).Wait();
        }


        private async Task Run(string title, string description, string[] tags, PrivacyStatus privacyStatus, IFormFile file)
        {
            UserCredential userCredential;
            GoogleAuthorizationCodeFlow.Initializer initializer = new GoogleAuthorizationCodeFlow.Initializer();
            initializer.ClientSecrets = new ClientSecrets()
            {
                ClientId = _youtubeClientSecret.client_id,
                ClientSecret = _youtubeClientSecret.client_secret
            };
            initializer.Scopes = new string[1]
            {
                    YouTubeService.Scope.YoutubeUpload
            };

            //var token = new TokenResponse { RefreshToken = _youtubeClientSecret.token_uri };
            //userCredential = new UserCredential(new GoogleAuthorizationCodeFlow(
            //    new GoogleAuthorizationCodeFlow.Initializer
            //    {
            //        ClientSecrets = initializer.ClientSecrets
            //    }),
            //    "user",
            //    token);
            string token = await GenerateTokenAsync();
            if (token != "")
            {
                userCredential = new UserCredential(new GoogleAuthorizationCodeFlow(initializer), "me", new TokenResponse()
                {
                    RefreshToken = token,
                    TokenType = "Bearer",
                    ExpiresInSeconds = new long?(3599L),
                    AccessToken = token
                });

                YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                });
                Video video = new Video()
                {
                    Snippet = new VideoSnippet()
                };
                video.Snippet.Title = title;
                video.Snippet.Tags = tags;
                video.Snippet.Description = description;
                video.Snippet.CategoryId = "";
                video.Status = new VideoStatus { PrivacyStatus = privacyStatus.ToString() };

                await Task.Run(() =>
                {
                    VideosResource.InsertMediaUpload insertMediaUpload = youtubeService.Videos.Insert(video, "snippet,status", file.OpenReadStream(), "video/*");
                    insertMediaUpload.ProgressChanged += videosInsertRequest_ProgressChanged;
                    insertMediaUpload.ResponseReceived += VideosInsertRequest_ResponseReceived;
                    insertMediaUpload.UploadAsync().Wait();
                });
            }
            else
            {
                return;
            }
        }

        private void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            if (progress.Status != UploadStatus.Failed)

                return;

        }

        public void VideosInsertRequest_ResponseReceived(Video video)
        {
            try
            {
                string str = video.Snippet.Tags.FirstOrDefault();
                if (str == null)
                    return;
                int profileId = int.Parse(this._encryption.Decrypt(str));

                Profile profile = _appDbContext.Profiles.FirstOrDefault(o => o.Id == profileId);
                if (profile == null)
                    return;
                profile.ExpressYourselfUrl = video.Id;
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }


        private async Task<string> RunPostAsync(string title, string description, string[] tags, PrivacyStatus privacyStatus, IFormFile file)
        {
            try
            {
                var retryCount = 0;
                GoogleAuthorizationCodeFlow.Initializer initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets()
                    {
                        ClientId = _youtubeClientSecret.client_id,
                        ClientSecret = _youtubeClientSecret.client_secret
                    },
                    Scopes = new string[1] {YouTubeService.Scope.YoutubeUpload}
                };

                ReGenerateToken:
                if (retryCount > 3)
                {
                    return "";
                }

                var token = await GenerateTokenAsync();
                
                if (token == "")
                {
                     retryCount +=1;
                     goto ReGenerateToken;
                }

                var userCredential = new UserCredential(new GoogleAuthorizationCodeFlow(initializer), "me", new TokenResponse()
                {
                    RefreshToken = token,
                    TokenType = "Bearer",
                    ExpiresInSeconds = new long?(3599L),
                    AccessToken = token
                });

                    YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = userCredential,
                        ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                    });
                    Video video = new Video()
                    {
                        Snippet = new VideoSnippet()
                    };
                    video.Snippet.Title = title;
                    video.Snippet.Tags = tags;
                    video.Snippet.Description = description;
                    video.Snippet.CategoryId = "";
                    video.Status = new VideoStatus { PrivacyStatus = privacyStatus.ToString() };

                    VideosResource.InsertMediaUpload insertMediaUpload = youtubeService.Videos.Insert(video, "snippet,status", file.OpenReadStream(), "video/*");
                    // insertMediaUpload.ProgressChanged += PostVideosInsertRequest_ProgressChanged;
                    // insertMediaUpload.ResponseReceived += PostVideosInsertRequest_ResponseReceived;
                    await insertMediaUpload.UploadAsync();

                    return insertMediaUpload.ResponseBody?.Id;
                
            }
            catch (Exception e)
            {
                return "";
            }
            
        }

        private void PostVideosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            if (progress.Status != UploadStatus.Failed)

                return;

        }

        public void PostVideosInsertRequest_ResponseReceived(Video video)
        {
            try
            {
                string postId = video.Snippet.Tags.FirstOrDefault();
                if (postId == null)
                    return;
                var post = _mongoDbContext.Posts.Find(k=>k.ID == new ObjectId(postId)).FirstOrDefault();
                if (post == null)
                    return;
                post.YoutubeVideoID = video.Id;
                _mongoDbContext.Posts.FindOneAndReplace(k => k.ID == new ObjectId(postId), post);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<string> GenerateTokenAsync()
        {
            try
            {
                string url = _youtubeClientSecret.token_uri;

                var req = new TokenRequestValues
                {
                    refresh_token = _youtubeClientSecret.refresh_token,
                    client_id = _youtubeClientSecret.client_id,
                    client_secret = _youtubeClientSecret.client_secret,
                    grant_type = "refresh_token"
                };

                var json = JsonConvert.SerializeObject(req);

                var client = new HttpClient {BaseAddress = new Uri(url)};

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (!result.IsSuccessStatusCode) return "";

                var resultObjects = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TokenResponseValues>(resultObjects);
                return response.access_token;

            }
            catch (Exception e)
            {
                return "";
            }
        }

        //public void UploadVideoAssessment(string title, string description, PrivacyStatus privacyStatus, string clientSecretsJsonObject, byte[] videoBytes, string categoryId, string[] tags, string profileVideoAnswerID)
        //{
        //    string[] keys = new string[1] { profileVideoAnswerID };
        //    this.RunVideo(title, description, keys, categoryId, privacyStatus, clientSecretsJsonObject, videoBytes);
        //}

        //private async Task RunVideo( string title, string description, string[] keys, string categoryId, PrivacyStatus privacyStatus, string clientSecretsJsonObject, byte[] VideoByte)
        //{
        //    UserCredential userCredential;
        //    using (FileStream fileStream = new FileStream(clientSecretsJsonObject, FileMode.Open, FileAccess.Read))
        //    {
        //        GoogleClientSecrets googleClientSecrets = GoogleClientSecrets.Load((Stream)fileStream);
        //        GoogleAuthorizationCodeFlow.Initializer initializer = new GoogleAuthorizationCodeFlow.Initializer();
        //        initializer.ClientSecrets = new ClientSecrets()
        //        {
        //            ClientId = googleClientSecrets.Secrets.ClientId,
        //            ClientSecret = googleClientSecrets.Secrets.ClientSecret
        //        };
        //        initializer.Scopes = (IEnumerable<string>)new string[1]
        //        {
        //  YouTubeService.Scope.YoutubeUpload
        //        };
        //        userCredential = new UserCredential((IAuthorizationCodeFlow)new GoogleAuthorizationCodeFlow(initializer), "me", new TokenResponse()
        //        {
        //            RefreshToken = this._config.VideoToken,
        //            TokenType = "Bearer",
        //            ExpiresInSeconds = new long?(3600L)
        //        });
        //    }
        //    YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = (IConfigurableHttpClientInitializer)userCredential,
        //        ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
        //    });
        //    Video video = new Video()
        //    {
        //        Snippet = new VideoSnippet()
        //    };
        //    video.Snippet.Title = title;
        //    video.Snippet.Tags = (IList<string>)keys;
        //    video.Snippet.Description = description;
        //    video.Snippet.CategoryId = categoryId;
        //    video.Status = new VideoStatus();
        //    video.Status.PrivacyStatus = privacyStatus.ToString();
        //    Task.Run((Action)(() =>
        //    {
        //        VideosResource.InsertMediaUpload insertMediaUpload = youtubeService.Videos.Insert(video, "snippet,status", (Stream)new MemoryStream(VideoByte), "video/*");
        //        insertMediaUpload.ProgressChanged += new Action<IUploadProgress>(this.assessementvideosInsertRequest_ProgressChanged);
        //        insertMediaUpload.ResponseReceived += new Action<Video>(this.assessementvideosInsertRequest_ResponseReceived);
        //        insertMediaUpload.UploadAsync().Wait();
        //    }));
        //}

        //private void assessementvideosInsertRequest_ResponseReceived(Video video)
        //{
        //    try
        //    {
        //        string str = video.Snippet.Tags.FirstOrDefault();
        //        if (str == null)
        //            return;
        //        int videoProfileId = int.Parse(_encryption.Decrypt(str));
        //        ProfileVideoAssessmentAnswerScore assessmentAnswerScore = _appDbContext.ProfileVideoAssessmentAnswerScores.FirstOrDefault((o => o.Id == videoProfileId));
        //        if (assessmentAnswerScore == null)
        //            return;
        //        assessmentAnswerScore.VideoIdyoutubeUrl = video.Id;
        //        assessmentAnswerScore.VideoId = null;
        //        _appDbContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void assessementvideosInsertRequest_ProgressChanged(IUploadProgress progress)
        //{
        //    if (progress.Status != UploadStatus.Failed)
        //        return;
        //}
    }


}
