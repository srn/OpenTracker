using System;
using System.Collections.Generic;
using System.Web.Mvc;

using OpenTracker.Core;
using OpenTracker.Core.BEncoding;

namespace OpenTracker.Models.Tracker
{
    public class BTErrorResult : ActionResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public BTErrorResult(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message is null or empty", "message");

            this.Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var res = new Dictionary<string, object>
                          {
                                {
                                    "failure reason", 
                                    Message
                                }
                          };

            var response = context.HttpContext.Response;
            response.Write(BEncoder.BEncodeDictionary(res));        
        }
    }
}
