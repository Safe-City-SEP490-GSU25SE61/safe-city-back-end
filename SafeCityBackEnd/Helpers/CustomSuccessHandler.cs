using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SafeCityBackEnd.Helpers;

public static class CustomSuccessHandler
{
    public static IActionResult ResponseBuilder(HttpStatusCode statusCode, string message, object responseObject)
    {
        var response = new Dictionary<string, object>
        {
            { "http_status", statusCode },
            { "time_stamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
            { "message", message },
            { "data", responseObject }
        };

        return new ObjectResult(response) { StatusCode  = (int)statusCode };
    }
}