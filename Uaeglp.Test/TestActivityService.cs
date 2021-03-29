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
    public class TestActivityService
    {
        private readonly IActivityAndChallengesService _activityAndChallengesService;
        public TestActivityService()
        {
            var services = new ContainerResolver().Container;
            _activityAndChallengesService = (IActivityAndChallengesService)services.GetService(typeof(IActivityAndChallengesService));

        }

    


        [TestMethod]
        public void GetCategoryList_ItshouldRetunrn_Success()
        {
           
            var mm = _activityAndChallengesService.GetActivityCategoryAsync(13535).Result;

            Assert.IsNotNull(mm);
        }


        //[TestMethod]
        //public void GetQestionList_ItshouldRetunrnAllList_Success()
        //{

        //    var mm = _programService.GetBatchQuestionsAsync(13630, 28).Result;

        //    Assert.IsNotNull(mm);
        //}

    }
}
