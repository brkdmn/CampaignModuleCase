using System.Globalization;
using System.Net;

namespace CampaignModule.Core.Helper;

public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public AppException() : base() {}

    public AppException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public AppException(string message, HttpStatusCode statusCode, params object[] args) 
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
        StatusCode = statusCode;
    }
}