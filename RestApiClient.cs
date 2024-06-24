using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// Client for REST API calls
    /// </summary>
    public sealed class RestApiClient
    {

        #region Fluid Pattern Property Setters

        /// <summary>
        /// Set request timeout
        /// </summary>
        /// <param name="timeout">Request timeout in SECONDS</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithTimeout(int timeout)
        {
            RequestTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Configure a bearer token
        /// </summary>
        /// <param name="bearerToken">Bearer token</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithBearerToken(string bearerToken)
        {
            BearerToken = bearerToken;
            return this;
        }

        /// <summary>
        /// Configure the request URI
        /// </summary>
        /// <param name="uri">Must be an absolute URI</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithRequestUri(Uri uri)
        {
            RequestUri = uri;
            return this;
        }

        /// <summary>
        /// Configure the request URI
        /// </summary>
        /// <param name="uri">Must be an absolute URI</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithRequestUri(string uri)
        {
            RequestUri = new Uri(uri, UriKind.Absolute);
            return this;
        }

        /// <summary>
        /// Add a query string parameter. Replaces if parameter exists.
        /// </summary>
        /// <param name="name">Name of querystring parameter</param>
        /// <param name="value">Value of parameter. Will be Uri Escaped.</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Both name and value must be provided.");
            }

            _queryParameters[name] = Uri.EscapeDataString(value);
            return this;
        }

        /// <summary>
        /// Add query string parameters. Replaces if parameter exists.
        /// </summary>
        /// <param name="parameters">Dictionary containing parameters. Values will be Uri escaped</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithParameters(Dictionary<string, string> parameters)
        {
            foreach (string key in parameters.Keys)
            {
                _queryParameters[key] = Uri.EscapeDataString(parameters[key]);
            }

            return this;
        }

        /// <summary>
        /// Add a header. Replaces if header exists.
        /// </summary>
        /// <param name="name">Name of header parameter</param>
        /// <param name="value">Value of header</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithHeader(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Both name and value must be provided.");
            }

            _headers[name] = value;
            return this;
        }

        /// <summary>
        /// Add headers.. Replaces if header exists.
        /// </summary>
        /// <param name="headers">Dictionary containing headers</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithHeaders(Dictionary<string, string> headers)
        {
            foreach (string key in headers.Keys)
            {
                _headers[key] = headers[key];
            }

            return this;
        }


        /// <summary>
        /// Add a body
        /// </summary>
        /// <param name="content">Body content/data. Can be NULL if you just want to set content type.</param>
        /// <param name="contentType">Content type. Defaults to "application/json" if not provided</param>
        /// <returns>Current instance</returns>
        public RestApiClient WithBody(string? content, string contentType = "application/json")
        {
            _body = content;
            _contentType = contentType;

            return this;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Sends a request to the server and returns a response
        /// </summary>
        /// <param name="method">HTTP Method to use</param>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> SendAsync(HttpMethod method)
        {
            if (_requestUri == null)
            {
                throw new ArgumentException("Request Uri must be set. Call WithRequestUri().");
            }


            StringBuilder requestUriWithParameters = new();
            requestUriWithParameters.Append(_requestUri.ToString());
            if (_queryParameters.Count > 0)
            {
                requestUriWithParameters.Append('?');
                foreach (string key in _queryParameters.Keys)
                {
                    if (!key.Equals(Uri.EscapeDataString(key)))
                    {
                        throw new ArgumentException($"The query string parameter '{key}' contains illegal characters and cannot be used.");
                    }
                    requestUriWithParameters.Append($"{key}={_queryParameters[key]}&");
                }
            }

            HttpRequestMessage request = new(method, requestUriWithParameters.ToString().TrimEnd('&'));

            // add headers
            if (_headers.Count > 0)
            {
                foreach (string key in _headers.Keys)
                {
                    if (!request.Headers.Contains(key))
                    {
                        request.Headers.Add(key, _headers[key]);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(_body))
            {
                request.Content = new StringContent(_body);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_contentType ?? "application/json");
            }

            HttpClient _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(_requestTimeout)
            };
            _httpClient.DefaultRequestHeaders.ConnectionClose = true;

            // Add Bearer token if applicable
            if (! string.IsNullOrWhiteSpace(BearerToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Bearer", BearerToken);
            }

            try
            {
                Task<HttpResponseMessage> response = _httpClient.SendAsync(request);
                await response;

                if (response.IsCanceled)
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Remote server terminated the connection."),
                        ReasonPhrase = "Remote server terminated the connection."
                    };
                }

                if (response.IsFaulted)
                {
                    _httpClient.CancelPendingRequests();

                    if (response.Exception != null)
                    {
                        Exception exception = response.Exception.InnerException ?? response.Exception;

                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(exception.ToString()),
                            ReasonPhrase = exception.Message
                        };
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent("Unknown API error occurred."),
                            ReasonPhrase = "Unknown API error occurred."
                        };
                    }
                }

                return response.Result;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.ToString()),
                    ReasonPhrase = ex.Message
                };
            }
        }

        /// <summary>
        /// Perform a HTTP GET
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Get() => await SendAsync(HttpMethod.Get);

        /// <summary>
        /// Perform a HTTP PUT
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Put() => await SendAsync(HttpMethod.Put);

        /// <summary>
        /// Perform a HTTP PATCH
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Patch() => await SendAsync(HttpMethod.Patch);

        /// <summary>
        /// Perform a HTTP POST
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Post() => await SendAsync(HttpMethod.Post);

        /// <summary>
        /// Perform a HTTP DELETE
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Delete() => await SendAsync(HttpMethod.Delete);

        /// <summary>
        /// Perform a HTTP OPTIONS
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Options() => await SendAsync(HttpMethod.Options);

        /// <summary>
        /// Perform a HTTP HEAD
        /// </summary>
        /// <returns>The raw response from the call</returns>
        public async Task<HttpResponseMessage> Head() => await SendAsync(HttpMethod.Head);

        #endregion

        #region Properties

        /// <summary>
        /// Request timeout, in seconds
        /// </summary>
        public int RequestTimeout
        {
            get => _requestTimeout;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _requestTimeout = value;
            }
        }

        /// <summary>
        /// Authorization bearer token
        /// </summary>
        public string? BearerToken
        {
            get; set;
        }

        /// <summary>
        /// Request URI
        /// </summary>
        public Uri? RequestUri
        {
            get => _requestUri;
            set
            {
                if ((value != null) && (!value.IsAbsoluteUri))
                {
                    throw new ArgumentException("Uri must be an absolute URL.");
                }

                _requestUri = value;
            }
        }


        #endregion


        /// <summary>
        /// Initialize the client
        /// </summary>
        public RestApiClient()
        {
        }

        /// <summary>
        /// Kick off the Fluid-style building logic
        /// </summary>
        /// <returns>A new instance of RestApiClient</returns>
        public static RestApiClient CreateBuilder()
        {
            return new RestApiClient();
        }

        private Uri? _requestUri = null;
        private Dictionary<string, string> _queryParameters = new Dictionary<string, string>(), _headers = new Dictionary<string, string>();
        private string? _body = null, _contentType = null;
        private int _requestTimeout = 15;
    }
}
