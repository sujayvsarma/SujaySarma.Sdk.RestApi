using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// A simple API result that consists of a key whose value is an array.
    /// 
    /// Typically this is a response such as:
    /// {
    ///   "id": [ 1, 2, 3, 4... ]
    /// }
    /// </summary>
    public class SimpleArrayResult<T> : ServiceResultBase
    {

        #region Properties

        /// <summary>
        /// The result as an IEnumerable
        /// </summary>
        public IEnumerable<T> Result { get; private set; }

        #endregion

        /// <summary>
        /// Generate a simple array result
        /// </summary>
        /// <param name="message">The HttpResponseMessage from the API</param>
        public SimpleArrayResult(HttpResponseMessage message)
            : base(message)
        {
            if (!string.IsNullOrWhiteSpace(ResponseBody))
            {
                // this will throw if it is not a valid dictionary. but we are okay with bubbling it up.
                Dictionary<string, List<T>> preResult = JsonSerializer.Deserialize<Dictionary<string, List<T>>>(ResponseBody);
                foreach(string key in preResult.Keys)
                {
                    Result = preResult[key];
                    break;
                }
            }
            else
            {
                Result = new List<T>();
            }
        }

    }
}
