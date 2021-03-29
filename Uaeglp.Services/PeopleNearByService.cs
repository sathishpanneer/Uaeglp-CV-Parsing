using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.MongoModels;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using System.Linq;
using System.Net;
using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Uaeglp.ViewModels.UserLocationViewModels;

namespace Uaeglp.Services
{
    public class PeopleNearByService : IPeopleNearByService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IUserIPAddress _userIPAddress;

        public PeopleNearByService(AppDbContext appDbContext, IMapper mapper, IOptions<AppSettings> appSettings, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _userIPAddress = userIPAddress;
        }

        public async Task<IPeopleNearByResponse> AddUpdateUserLocation(UserLocationModel view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.ProfileID).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.ProfileID).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;

                var data = await _appDbContext.UserLocations.FirstOrDefaultAsync(x => x.ProfileID == view.ProfileID);

                if (data == null)
                {
                    data = new UserLocation()
                    {
                        ProfileID = view.ProfileID,
                        Latitude = view.Latitude,
                        Longitude = view.Longitude,
                        isHideLocation = true,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        CreatedBy = userName,
                        LastUpdatedBy = userName
                    };

                    await _appDbContext.UserLocations.AddAsync(data);
                    await _appDbContext.SaveChangesAsync();

                }
                else
                {
                    data.Latitude = view.Latitude;
                    data.Longitude = view.Longitude;
                    data.LastUpdated = DateTime.Now;
                    await _appDbContext.SaveChangesAsync();
                }

                var userLocation = _mapper.Map<UserLocationModelView>(data);
                return new PeopleNearByResponse(userLocation);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }

        public async Task<IPeopleNearByResponse> GetUserLocation(int userId, string latitude, string longitude)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  LocationAPIKey : {_appSettings.LocationAPIKey}");

                var radiusSetting = await _appDbContext.ApplicationSettings.ToListAsync();
                var nearByradius = radiusSetting.FirstOrDefault(k => k.Key == "peopleNearbyRadius")?.Value;
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  peopleNearbyRadiusConfig: {nearByradius}");

                var userData = await _appDbContext.UserLocations.FirstOrDefaultAsync(x => x.ProfileID == userId);

                if (userData == null) return new PeopleNearByResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                if (userData != null)
                {
                    userData.Latitude = latitude;
                    userData.Longitude = longitude;
                    userData.LastUpdated = DateTime.Now;
                    await _appDbContext.SaveChangesAsync();
                }


                var origin = await _appDbContext.UserLocations.Where(x => x.ProfileID == userId).FirstOrDefaultAsync();
                var destination = await _appDbContext.UserLocations.Where(x => x.ProfileID != userId).ToListAsync();


                List<NearByUsers> nearByUsers = new List<NearByUsers>();

                foreach (var item in destination)
                {
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == item.ProfileID);
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == item.ProfileID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var userDetails = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.ProfileID);

                    string url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins="
                                    + origin.Latitude + "," + origin.Longitude + "&destinations=" + item.Latitude + "," + item.Longitude
                                    + "&key=" + _appSettings.LocationAPIKey + "";

                    WebRequest request = WebRequest.Create(url);

                    WebResponse response = request.GetResponse();

                    Stream data = response.GetResponseStream();

                    StreamReader reader = new StreamReader(data);

                    // json-formatted string from maps api
                    string responseFromServer = reader.ReadToEnd();

                    var responseData = JsonConvert.DeserializeObject<UserLocationResponseView>(responseFromServer);

                    if (responseData.rows.Count > 0)
                    {
                        var radius = responseData.rows[0].elements[0]?.distance?.text.Split(new char[0]);
                        if (radius != null)
                        {
                            decimal distance = Convert.ToDecimal(radius[0]);
                            if (distance <= Convert.ToInt32(nearByradius) && item.isHideLocation == false)
                            {
                                var locationResponse = new NearByUsers()
                                {
                                    ProfileID = item.ProfileID,
                                    Latitude = item.Latitude,
                                    Longitude = item.Longitude,
                                    Distance = responseData.rows[0].elements[0]?.distance?.text,
                                    isHideLocation = item.isHideLocation,
                                    Created = item.Created,
                                    LastUpdated = item.LastUpdated,
                                    CreatedBy = item.CreatedBy,
                                    LastUpdatedBy = item.LastUpdatedBy,
                                    FirstNameAr = profile.FirstNameAr,
                                    FirstNameEn = profile.FirstNameEn,
                                    LastNameAr = profile.LastNameAr,
                                    LastNameEn = profile.LastNameEn,
                                    DesignationEn = workExperience?.Title?.TitleEn,
                                    DesignationAr = workExperience?.Title?.TitleAr,
                                    UserImageFileId = userDetails.OriginalImageFileId ?? 0,
                                };

                                nearByUsers.Add(locationResponse);
                            }
                        }
                    }
                    else
                    {
                        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  ErrorMessage : {responseData?.error_message}");
                    }

                    response.Close();
                }

                var hideLocationData = new HideLocationView()
                {
                    ProfileID = userData.ProfileID,
                    isHideLocation = userData.isHideLocation
                };

                var getLocationDetais = new GetUserLocationView()
                {
                    HideLocationDetails = hideLocationData,
                    NearByUsers = nearByUsers
                };

                return new PeopleNearByResponse(getLocationDetais);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }

        public async Task<IPeopleNearByResponse> HideUserLocation(int userId, bool isHideLocation)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserLocations.FirstOrDefaultAsync(x => x.ProfileID == userId);
                if (data == null) return new PeopleNearByResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                if (data != null)
                {
                    data.isHideLocation = isHideLocation;
                    await _appDbContext.SaveChangesAsync();

                }

                var userLocation = _mapper.Map<UserLocationModelView>(data);
                return new PeopleNearByResponse(userLocation);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        }
}
