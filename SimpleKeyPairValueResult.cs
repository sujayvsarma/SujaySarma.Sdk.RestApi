using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// A simple API result that consists of a key-pair value.
    /// 
    /// Typically this is a response such as:
    /// {
    ///   "id": "blah",
    ///   "name" : "xyz"
    /// }
    /// </summary>
    public class SimpleKeyPairValueResult : ServiceResultBase
    {

        #region Properties

        /// <summary>
        /// The result as a dictionary object
        /// </summary>
        public Dictionary<string, string> Result { get; private set; }

        #endregion

        /// <summary>
        /// Generate a simple KVP result
        /// </summary>
        /// <param name="message">The HttpResponseMessage from the API</param>
        public SimpleKeyPairValueResult(HttpResponseMessage message)
            : base(message)
        {
            if (! string.IsNullOrWhiteSpace(ResponseBody))
            {
                // this will throw if it is not a valid dictionary. but we are okay with bubbling it up.
                Result = JsonSerializer.Deserialize<Dictionary<string, string>>(ResponseBody);
            }
            else
            {
                Result = new Dictionary<string, string>();
            }
        }

    }
}
