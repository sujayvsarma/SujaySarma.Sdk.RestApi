using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SujaySarma.Sdk.RestApi
{
    public class ServiceResultBase : IServiceResult
    {

        #region Properties

        /// <summary>
        /// The Http status code returned
        /// </summary>
        public HttpStatusCode Status { get; private set; }

        /// <summary>
        /// Response body as a string
        /// </summary>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// Collection of response headers
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; private set; }
        

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a ServiceResult from the HttpResponseMessage
        /// </summary>
        /// <param name="httpResponse">Response message from the API</param>
        protected ServiceResultBase(HttpResponseMessage httpResponse)
        {
            ResponseHeaders = new Dictionary<string, string>();
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> _combinedHeaders = httpResponse.Headers.Concat(httpResponse.Content.Headers);
            foreach (KeyValuePair<string, IEnumerable<string>> header in _combinedHeaders)
            {
                ResponseHeaders.Add(
                        header.Key,
                        string.Join(" ", header.Value)
                    );
            }

            Status = httpResponse.StatusCode;
            ResponseBody = httpResponse.Content.ReadAsStringAsync().Result;
        }

        #endregion

    }
}
