using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// The REST API client
    /// </summary>
    public sealed class RestApiClient
    {

        #region Constructors

        /// <summary>
        /// Instantiate the client
        /// </summary>
        /// <param name="bearerToken">Bearer token</param>
        public RestApiClient(string bearerToken)
        {
            BearerToken = bearerToken;

            rwLock.EnterUpgradeableReadLock();
            try
            {
                if (_httpClient == null)
                {
                    rwLock.EnterWriteLock();

                    _httpClient = new HttpClient();
                    if (!string.IsNullOrWhiteSpace(bearerToken))
                    {
                        _httpClient.DefaultRequestHeaders.Add("Bearer", bearerToken);
                    }

                    rwLock.ExitWriteLock();
                }
            }
            finally
            {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Instantiate the client
        /// </summary>
        /// <param name="bearerToken">Bearer token</param>
        /// <param name="requestUri">The request Uri. Must be an absolute address</param>
        public RestApiClient(string bearerToken, Uri requestUri)
            : this(bearerToken)
        {
            RequestUri = requestUri;
        }

        /// <summary>
        /// Instantiate the client
        /// </summary>
        /// <param name="bearerToken">Bearer token</param>
        /// <param name="requestUri">The request Uri. Must be an absolute address</param>
        /// <param name="parameters">Dictionary of parameters for the query string</param>
        public RestApiClient(string bearerToken, Uri requestUri, IDictionary<string, string> parameters)
            : this(bearerToken, requestUri)
        {
            if (parameters != null)
            {
                // we dont want to set = 
                foreach (string key in parameters.Keys)
                {
                    if (string.IsNullOrWhiteSpace(parameters[key]))
                    {
                        throw new ArgumentException($"Value for parameter '{key}' cannot be empty or NULL.");
                    }

                    QueryString.Add(key, parameters[key]);
                }
            }
        }

        /// <summary>
        /// Instantiate the client
        /// </summary>
        /// <param name="bearerToken">Bearer token</param>
        /// <param name="requestUri">The request Uri. Must be an absolute address</param>
        /// <param name="parameters">Dictionary of parameters for the query string</param>
        /// <param name="body">Content for the body of the request (if set, type is automatically set to "json")</param>
        public RestApiClient(string bearerToken, Uri requestUri, IDictionary<string, string> parameters = null, string body = null)
            : this(bearerToken, requestUri, parameters)
        {
            RequestBodyString = body;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Request headers to be added
        /// </summary>
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The query string parameters
        /// </summary>
        public Dictionary<string, string> QueryString { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Request body content as a raw string
        /// </summary>
        public string RequestBodyString { get; set; }

        /// <summary>
        /// The absolute URI to connect to. No parameters should be added to this! Add any parameters to 
        /// the QueryString dictionary
        /// </summary>
        public Uri RequestUri
        {
            get => _requestUri;
            set
            {
                if ((value == null) || (!value.IsAbsoluteUri) || ((value.Scheme != "http") && (value.Scheme != "https")))
                {
                    throw new ArgumentException($"{nameof(RequestUri)} must be an absolute url with HTTP or HTTPS scheme");
                }

                _requestUri = value;
            }
        }
        private Uri _requestUri = new Uri("http://localhost");

        /// <summary>
        /// The access "bearer" token for this Http client
        /// </summary>
        public string BearerToken { get; private set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// Calls the API method and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <param name="method">Http Method to use</param>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage CallApiMethod(HttpMethod method)
        {
            return _httpClient.SendAsync(CreateRequest(method)).Result;
        }


        /// <summary>
        /// Calls the API method with a HTTP GET  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage Get() => CallApiMethod(HttpMethod.Get);

        /// <summary>
        /// Calls the API method with a HTTP PUT  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage Put() => CallApiMethod(HttpMethod.Put);

        /// <summary>
        /// Calls the API method with a HTTP PATCH  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage Patch() => CallApiMethod(HttpMethod.Patch);

        /// <summary>
        /// Calls the API method with a HTTP POST  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage Post() => CallApiMethod(HttpMethod.Post);

        /// <summary>
        /// Calls the API method with a HTTP DELETE  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage Delete() => CallApiMethod(HttpMethod.Delete);

        /// <summary>
        /// Calls the API method with a HTTP OPTIONS  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage GetOptions() => CallApiMethod(HttpMethod.Options);

        /// <summary>
        /// Calls the API method with a HTTP HEAD  and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage GetHead() => CallApiMethod(HttpMethod.Head);

        /// <summary>
        /// Create the request message
        /// </summary>
        /// <param name="method">Http Method to create request</param>
        /// <returns>HttpRequestMessage</returns>
        private HttpRequestMessage CreateRequest(HttpMethod method)
        {
            if ((RequestUri == null) || (!RequestUri.IsAbsoluteUri) || ((RequestUri.Scheme != "http") && (RequestUri.Scheme != "https")))
            {
                throw new ArgumentException($"{nameof(RequestUri)} must be an absolute url with HTTP or HTTPS scheme");
            }

            StringBuilder requestUriWithParameters = new StringBuilder();
            requestUriWithParameters.Append(RequestUri.ToString());
            if ((QueryString != null) && (QueryString.Count > 0))
            {
                requestUriWithParameters.Append("?");
                int paramCount = 0;
                foreach (string key in QueryString.Keys)
                {
                    if (!key.Equals(Uri.EscapeDataString(key)))
                    {
                        throw new ArgumentException($"The query string parameter '{key}' contains illegal characters and cannot be used.");
                    }


                    if (paramCount > 0)
                    {
                        requestUriWithParameters.Append("&");
                    }
                    requestUriWithParameters.Append($"key={Uri.EscapeDataString(QueryString[key])}");

                    paramCount++;
                }
            }

            // we don't need to add the bearer header because it was added as a DefaultHeader.
            HttpRequestMessage request = new HttpRequestMessage(method, requestUriWithParameters.ToString());

            if (!string.IsNullOrWhiteSpace(RequestBodyString))
            {
                request.Content = new StringContent(RequestBodyString, System.Text.Encoding.UTF8, JsonType);
            }

            return request;
        }

        #endregion

        private const string JsonType = "application/json";
        private static readonly System.Threading.ReaderWriterLockSlim rwLock = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.NoRecursion);

        // WARNING! Do NOT dispose this in the destructor! 
        private static HttpClient _httpClient;

        // Destructor
        ~RestApiClient() => rwLock.Dispose();

    }
}
