using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;
using Minio;
using System.IO;
using Minio.Exceptions;
using System.Linq;
using Uaeglp.ViewModels.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Uaeglp.ViewModels.ProfileViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Uaeglp.Utilities;
using System.Threading.Tasks;
using System.Web.Mvc;
using FileStreamResult = Microsoft.AspNetCore.Mvc.FileStreamResult;

namespace Uaeglp.Services.MinIoService
{
    public class MiniIoService
    {
        //private readonly MinIoConfig _minIoConfig;
        //public async Task<FileStreamResult> GetVideoContent(string filename)
        //{

        //    var appSetting = new MinIoConfig()
        //    {
        //        EndPoint = _minIoConfig.EndPoint,
        //        AccessKey = _minIoConfig.AccessKey,
        //        SecretKey = _minIoConfig.SecretKey,
        //        BucketName = _minIoConfig.BucketName,
        //        Location = _minIoConfig.Location
        //    };
        //    var objectName = filename;
        //    try
        //    {
        //        var endpoint = appSetting.EndPoint;
        //        var accessKey = appSetting.AccessKey;
        //        var secretKey = appSetting.SecretKey;

        //        var minio = new MinioClient(endpoint, accessKey, secretKey);

        //        Stream st = new System.IO.MemoryStream();
        //        await minio.GetObjectAsync(appSetting.BucketName, objectName, objectName.ToString() + ".mp4");
        //        var fileStreamVal = new FileStream(objectName.ToString() + ".mp4", FileMode.Open, FileAccess.Read);
        //        FileStreamResult result = File(
        //                                   fileStream: fileStreamVal,
        //                                   contentType: new MediaTypeHeaderValue("video/mp4").MediaType,
        //                                   enableRangeProcessing: true //<-- enable range requests processing
        //                               );
        //        return result;

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }


        //}
    }
}
