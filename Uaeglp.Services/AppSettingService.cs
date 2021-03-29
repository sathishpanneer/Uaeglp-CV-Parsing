using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services
{
    public class AppSettingService : IAppSettingService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public AppSettingService(AppDbContext appContext, IMapper mapper)
        {
            _appDbContext = appContext;
            _mapper = mapper;
        }

        public async Task<IAppSettingResponse> GetAppSettingAsync()
        {
            try
            {
                var appSettings = await _appDbContext.ApplicationSettings.ToListAsync();

                return new AppSettingResponse(_mapper.Map<List<ApplicationSettingViewModel>>(appSettings));
            }
            catch (Exception e)
            {
                return new AppSettingResponse(e);
            }
        }

        public async Task<IAppSettingResponse> InsertAppSettingAsync(List<ApplicationSettingViewModel> model)
        {
            try
            {
                var entries = _mapper.Map<List<ApplicationSetting>>(model);
                _appDbContext.ApplicationSettings.AddRange(entries);
                await _appDbContext.SaveChangesAsync();

                var appSettings = await _appDbContext.ApplicationSettings.ToListAsync();

                return new AppSettingResponse(_mapper.Map<List<ApplicationSettingViewModel>>(appSettings));
            }
            catch (Exception e)
            {
                return new AppSettingResponse(e);
            }
        }

        public async Task<IAppSettingResponse> UpdateAppSettingAsync(ApplicationSettingViewModel appSetting)
        {
            try
            {
                var data =await _appDbContext.ApplicationSettings.FirstOrDefaultAsync(k => k.Key == appSetting.Key);
                if (data == null)
                {
                    return new AppSettingResponse("Not Found", HttpStatusCode.NotFound);
                }

                data.Value = appSetting.Value;
                await _appDbContext.SaveChangesAsync();
                return new AppSettingResponse(_mapper.Map<ApplicationSettingViewModel>(data));
            }
            catch (Exception e)
            {
                return new AppSettingResponse(e);
            }
        }
    }
}
