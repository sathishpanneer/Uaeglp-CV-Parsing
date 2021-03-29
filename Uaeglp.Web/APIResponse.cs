using System.Net;
using Uaeglp.ViewModels;

namespace Uaeglp.Web
{
    public static class APIResponse
    {
        public static APIResponseModel Success(object data, HttpStatusCode statusCode = HttpStatusCode.OK, string status = "ok")
        {
            return new APIResponseModel
            {
                Message = statusCode.ToString(),
                Status = status,
                StatusCode = (int)statusCode,
                Result = data
            };
        }

        public static APIResponseModel Error(object errorMsg = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, object data = null, string status = "InternalServerError")
        {
            return new APIResponseModel
            {
                Message = statusCode.ToString(),
                Status = status,
                StatusCode = (int)statusCode,
                Result = data,
                ErrorMessage = errorMsg
            };
        }

        public static APIResponseModel BadRequest(object errorMsg, HttpStatusCode statusCode = HttpStatusCode.BadRequest, object data = null, string status = "BadRequest")
        {
            return new APIResponseModel
            {
                Message = statusCode.ToString(),
                Status = status,
                StatusCode = (int)statusCode,
                Result = data,
                ErrorMessage = errorMsg
            };
        }

        public static APIResponseModel ExpectationFailed(object errorMsg, HttpStatusCode statusCode = HttpStatusCode.ExpectationFailed, object data = null, string status = "ExpectationFailed")
        {
            return new APIResponseModel
            {
                Message = statusCode.ToString(),
                Status = status,
                StatusCode = (int)statusCode,
                Result = data,
                ErrorMessage = errorMsg
            };
        }

        public static APIResponseModel NoContent(object errorMsg, HttpStatusCode statusCode = HttpStatusCode.NoContent, object data = null, string status = "NoContent")
        {
            return new APIResponseModel
            {
                Message = statusCode.ToString(),
                Status = status,
                StatusCode = (int)statusCode,
                Result = data,
                ErrorMessage = errorMsg
            };
        }

        public static APIResponseModel NotFound(object errorMsg, HttpStatusCode statusCode = HttpStatusCode.NotFound, object data = null, string status = "NotFound")
        {
            return new APIResponseModel
            {
                Message = statusCode.ToString(),
                Status = status,
                StatusCode = (int)statusCode,
                Result = data,
                ErrorMessage = errorMsg
            };
        }
    }
}