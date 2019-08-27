using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// Complex results returned by the API
    /// </summary>
    public class ComplexResult : ServiceResultBase
    {

        #region Properties

        /// <summary>
        /// The result as a dictionary object
        /// </summary>
        public Dictionary<string, object> Result { get; private set; }

        #endregion

        /// <summary>
        /// Generate a complex result
        /// </summary>
        /// <param name="message">The HttpResponseMessage from the API</param>
        public ComplexResult(HttpResponseMessage message)
            : base(message)
        {
            if (!string.IsNullOrWhiteSpace(ResponseBody))
            {
                // this will throw if it is not a valid dictionary. but we are okay with bubbling it up.
                Result = JsonSerializer.Deserialize<Dictionary<string, object>>(ResponseBody);
            }
            else
            {
                Result = new Dictionary<string, object>();
            }
        }
    }
}
