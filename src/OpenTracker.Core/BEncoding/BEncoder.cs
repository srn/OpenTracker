using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MonoTorrent;

namespace OpenTracker.Core.BEncoding
{
    public static class BEncoder
    {
        /// <summary>
        /// BEncode an object. Will throw an exception if an object is not a valid object to BEncode.
        /// </summary>
        /// <param name="input">The object to BEncode</param>
        /// <exception cref="ArgumentException">Supported types are strings, integers (short, int, long), Lists of objects and Dictionaries with string keys and valid objects as values</exception>
        /// <returns>The BEncoded object as a UTF-8 String</returns>
        public static string BEncode(object input)
        {
            if (input is string)
                return BEncodeString((string)input);

            if (input is long || input is int || input is short)
                return BEncodeInteger(Convert.ToInt64(input));

            if (input is List<object>)
                return BEncodeList((List<object>)input);

            if (input is Dictionary<string, object>)
                return BEncodeDictionary((Dictionary<string, object>)input);

            throw new ArgumentException(string.Format("Invalid Type {0}", input.GetType().Name), "input");
        }

        /// <summary>
        /// BEncode a String
        /// </summary>
        /// <param name="input">The string to Encode</param>
        /// <returns>The BEncoded string as a UTF-8 String</returns>
        public static string BEncodeString(string input)
        {
            // The Specification is not clear on the encoding
            // Does it have to be ASCII or is UTF-8 ok as well?
            // Assuming the latter for now
            return string.Format("{0}:{1}", input.Length, input);
        }

        /// <summary>
        /// BEncode a long integer (Int64)
        /// </summary>
        /// <param name="input">The long to Encode</param>
        /// <returns>The BEncoded integer as an UTF-8 string</returns>
        public static string BEncodeInteger(long input)
        {
            return string.Format("i{0}e", input);
        }

        /// <summary>1
        /// BEncode a List of Objects. Warning: Any Invalid objects will throw an Exception!
        /// </summary>
        /// <param name="input">The List of objects to Encode</param>
        /// <returns>The BEncoded List as a UTF-8 String</returns>
        public static string BEncodeList(List<object> input)
        {
            var result = new StringBuilder();

            result.Append("l");
            foreach (object o in input)
            {
                result.Append(BEncode(o));
            }
            result.Append("e");

            return result.ToString();
        }

        /// <summary>
        /// BEncode a Dictionary
        /// </summary>
        /// <param name="input">The Dictionary to Encode</param>
        /// <returns>The BEncoded Dictionary as a UTF-8 String</returns>
        public static string BEncodeDictionary(Dictionary<string, object> input)
        {
            var result = new StringBuilder();

            result.Append("d");
            foreach (KeyValuePair<string, object> o in input)
            {
                result.Append(BEncodeString(o.Key));
                result.Append(BEncode(o.Value));
            }
            result.Append("e");

            return result.ToString();
        }

        /// <summary>
        /// Formats the BEncoded string the client reports
        /// </summary>
        /// <returns></returns>
        public static string FormatUrlInfoHash()
        {
            // a temporary solution for getting the proper info_hash
            // by requesting raw url queries
            // see: http://stackoverflow.com/questions/2219647/ [..]
            var infoHashQuery = HttpContext.Current.Request.Url.Query;
            var infoHashArray = infoHashQuery.Split(Convert.ToChar("&"));
            var EncodedInfoHash = InfoHash.UrlDecode(
                infoHashArray[0].Replace("?info_hash=", string.Empty)
            ).ToString();

            return EncodedInfoHash.Replace("-", string.Empty);
        }
    }
}
