using System.Collections.Generic;
using System.Net;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// Interface defining a general result returned by an API service
    /// </summary>
    public interface IServiceResult
    {
        /// <summary>
        /// The Http status code returned
        /// </summary>
        public HttpStatusCode Status { get; }

        /// <summary>
        /// Response body as a string
        /// </summary>
        public string ResponseBody { get; }

        /// <summary>
        /// Collection of response headers
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; }

    }
}
