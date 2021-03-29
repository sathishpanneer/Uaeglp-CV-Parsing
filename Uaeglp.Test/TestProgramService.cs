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
    public class TestProgramService
    {
        private readonly IProgramService _programService;
        public TestProgramService()
        {
            var services = new ContainerResolver().Container;
            _programService = (IProgramService)services.GetService(typeof(IProgramService));

        }

    


        [TestMethod]
        public void GetProgrammList_ItshouldRetunrnAllProgramList_Success()
        {
           
            var mm = _programService.GetAllProgramAsync(13630).Result;

            Assert.IsNotNull(mm);
        }


        [TestMethod]
        public void GetQestionList_ItshouldRetunrnAllList_Success()
        {

            var mm = _programService.GetBatchQuestionsAsync(13630, 28).Result;

            Assert.IsNotNull(mm);
        }

    }
}
