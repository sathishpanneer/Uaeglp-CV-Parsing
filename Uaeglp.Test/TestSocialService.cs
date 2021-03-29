using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uaeglp.Contracts;
using Uaeglp.MongoModels;
using Uaeglp.Services;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.PostViewModels;

namespace Uaeglp.Tests
{
    [TestClass]
    public class TestSocialService
    {
        private readonly ISocialService _socialService;
        public TestSocialService()
        {
            var services = new ContainerResolver().Container;
            _socialService = (ISocialService)services.GetService(typeof(ISocialService));

        }

        //[TestMethod]
        //public void InsertActivityLog_ItshouldMakeEntryInLog_Success()
        //{
        //    var data = new UserActivityLog()
        //    {
        //        ActivityEn = "Your Liked This Post",
        //        ActivityType = ActivityType.Like,
        //        PostId = "5e46a906e6713a1ee49d57b7",
        //        ActivityDate = DateTime.UtcNow,
        //        ActivityAr = "",
        //        UserId = 15919
        //    };

        //    var mm = _socialService.AddActivityLogAsync(data).Result;

        //    Assert.IsNotNull(mm);
        //}




        //[TestMethod]
        //public void InsertActivityLogLiked_ItshouldMakeEntryInLog_Success()
        //{
        //    var data = new UserActivityLog()
        //    {
        //        ActivityEn = "Your Liked This Post",
        //        ActivityType = ActivityType.Like,
        //        PostId = "5e46a906e6713a1ee49d57b7",
        //        ActivityDate = DateTime.UtcNow,
        //        ActivityAr = "",
        //        UserId = 15919
        //    };

        //    var mm = _socialService.AddActivityLogAsync(data).Result;

        //    Assert.IsNotNull(mm);
        //}

        //[TestMethod]
        //public void InsertActivityLogCommented_ItshouldMakeEntryInLog_Success()
        //{
        //    var data = new UserActivityLog()
        //    {
        //        ActivityEn = "Your Commented This Post",
        //        ActivityType = ActivityType.Comment,
        //        PostId = "5e46a906e6713a1ee49d57b7",
        //        ActivityDate = DateTime.UtcNow,
        //        ActivityAr = "",
        //        UserId = 15919
        //    };

        //    var mm = _socialService.AddActivityLogAsync(data).Result;

        //    Assert.IsNotNull(mm);
        //}

        //[TestMethod]
        //public void InsertActivityLogShare_ItshouldMakeEntryInLog_Success()
        //{
        //    var data = new UserActivityLog()
        //    {
        //        ActivityEn = "Your Shared This Post",
        //        ActivityType = ActivityType.Share,
        //        ActivityDate = DateTime.UtcNow,
        //        ActivityAr = "",
        //        PostId = "5e46a906e6713a1ee49d57b7",
        //        UserId = 15919
        //    };

        //    var mm = _socialService.AddActivityLogAsync(data).Result;

        //    Assert.IsNotNull(mm);
        //}


        //[TestMethod]
        //public void InsertActivityLogFollowed_ItshouldMakeEntryInLog_Success()
        //{
        //    var data = new UserActivityLog()
        //    {
        //        ActivityEn = "Your Followed This Post",
        //        ActivityType = ActivityType.Follow,
        //        ActivityDate = DateTime.UtcNow,
        //        ActivityAr = "",
        //        PublicProfileId = 15922,
        //        UserId = 15919
        //    };

        //    var mm = _socialService.AddActivityLogAsync(data).Result;

        //    Assert.IsNotNull(mm);
        //}


        [TestMethod]
        public void GetActivityLog_ItshouldRetunrnLogs_Success()
        {
            var userId = 15919; 

            var mm = _socialService.GetActivityLogsAsync(userId,5,1).Result;

            Assert.IsNotNull(mm.ActivityLogs);
        }

        [TestMethod]
        public void GetPost_ItshouldRetunrnPost_Success()
        {
            var userId = 15919;

            var mm = _socialService.GetPostAsync("5e4a4d05f932be15844f5289",13600).Result;

            Assert.IsNotNull(mm.Post);
        }


        [TestMethod]
        public void GetAllPost_ItshouldRetunrnPost_Success()
        {
            var userId = 13600;

            var mm = _socialService.GetPostsAsync(new GetPostsViewModel()).Result;

            Assert.IsNotNull(mm.Posts);
        }

        [TestMethod]
        public void AddPost_ItshouldAddPost_Success()
        {
            var userId = new BasePostViewModel()
            {
                UserID = 13600,
                Text = " new Test...",
                TypeID = PostType.Text

            };

            //var mm = _socialService.AddPostAsync(userId).Result;

            //Assert.IsNotNull(mm.Post);
        }
    }
}
