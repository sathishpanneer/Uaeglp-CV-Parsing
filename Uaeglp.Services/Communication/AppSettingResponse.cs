using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class AppSettingResponse : BaseResponse, IAppSettingResponse
    {

        public List<ApplicationSettingViewModel> ApplicationSettings { get; set; }

        public ApplicationSettingViewModel ApplicationSetting { get; set; }



        private AppSettingResponse(bool success, string message, List<ApplicationSettingViewModel> appSetting) : base(success, message)
        {
            ApplicationSettings = appSetting;
        }

        private AppSettingResponse(bool success, string message, ApplicationSettingViewModel appSetting) : base(success, message)
        {
            ApplicationSetting = appSetting;
        }

        public AppSettingResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }


        public AppSettingResponse(List<ApplicationSettingViewModel> appSetting) : this(true, string.Empty, appSetting)
        { }

        public AppSettingResponse(ApplicationSettingViewModel appSetting) : this(true, string.Empty, appSetting)
        { }

        public AppSettingResponse(Exception e) : base(e)
        { }
        public AppSettingResponse() : base()
        { }

    }
}
