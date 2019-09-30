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
        public RestApiClient()
        {
            BearerToken = null;
            RequestUri = null;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Instantiate the client
        /// </summary>
        /// <param name="bearerToken">Bearer token</param>
        public RestApiClient(string bearerToken)
        {
            BearerToken = bearerToken;

            _httpClient = new HttpClient();
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Bearer", bearerToken);
            }
        }

        /// <summary>
        /// Instantiate the client
        /// </summary>
        /// <param name="requestUri">The request Uri. Must be an absolute address</param>
        public RestApiClient(Uri requestUri)
        {
            BearerToken = null;
            RequestUri = requestUri;
            _httpClient = new HttpClient();
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

        /// <summary>
        /// Request timeout in SECONDS. Defaults to 15 seconds
        /// </summary>
        public int RequestTimeout { get; set; } = 15;

        #endregion

        #region Methods

        /// <summary>
        /// Calls the API method and returns the HttpResponseMessage. Caller should instantiate an appropriate 
        /// <see cref="IServiceResult"/> implementation with the result of this method.
        /// </summary>
        /// <param name="method">Http Method to use</param>
        /// <param name="contentType">The content type</param>
        /// <returns>The HttpResponseMessage</returns>
        public HttpResponseMessage CallApiMethod(HttpMethod method, string contentType = JsonType)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(RequestTimeout);
            return _httpClient.SendAsync(CreateRequest(method, contentType)).Result;
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
        /// <param name="contentType">The content type</param>
        /// <returns>HttpRequestMessage</returns>
        private HttpRequestMessage CreateRequest(HttpMethod method, string contentType = JsonType)
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

            HttpRequestMessage request = new HttpRequestMessage(method, requestUriWithParameters.ToString());

            // add headers
            // we don't need to add the "bearer" header because it was added as a DefaultHeader.
            if ((RequestHeaders != null) && (RequestHeaders.Count > 0))
            {
                foreach (string key in RequestHeaders.Keys)
                {
                    request.Headers.Add(key, RequestHeaders[key]);
                }
            }

            if (!string.IsNullOrWhiteSpace(RequestBodyString))
            {
                request.Content = new StringContent(RequestBodyString);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }

            return request;
        }

        #endregion

        private const string JsonType = "application/json";

        // WARNING! Do NOT dispose this in the destructor! 
        private HttpClient _httpClient;

    }
}
