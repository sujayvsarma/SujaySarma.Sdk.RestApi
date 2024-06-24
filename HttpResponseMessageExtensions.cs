using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SujaySarma.Sdk.RestApi
{
    /// <summary>
    /// Extension methods for the HttpResponseMessage class
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Read string content from the response
        /// </summary>
        /// <param name="message">HttpResponseMessage to read from</param>
        /// <returns>String content [body] of the response</returns>
        public static async Task<string> GetStringContentAsync(this HttpResponseMessage message)
        {
            return await message.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Read string content from the response
        /// </summary>
        /// <param name="message">HttpResponseMessage to read from</param>
        /// <returns>String content [body] of the response</returns>
        public static string GetStringContent(this HttpResponseMessage message)
        {
            return message.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// Read binary content from the response
        /// </summary>
        /// <param name="message">HttpResponseMessage to read from</param>
        /// <returns>Binary content [body] of the response</returns>
        public static async Task<byte[]> GetBinaryContentAsync(this HttpResponseMessage message)
        {
            return await message.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Read binary content from the response
        /// </summary>
        /// <param name="message">HttpResponseMessage to read from</param>
        /// <returns>Binary content [body] of the response</returns>
        public static byte[] GetBinaryContent(this HttpResponseMessage message)
        {
            return message.Content.ReadAsByteArrayAsync().Result;
        }



    }
}
