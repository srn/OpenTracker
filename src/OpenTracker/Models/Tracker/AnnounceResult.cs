using System;
using System.Collections.Generic;
using System.Web.Mvc;

using OpenTracker.Core;
using OpenTracker.Core.BEncoding;

namespace OpenTracker.Models.Tracker
{
    public class AnnounceResult : ActionResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly List<Object> Peers;

        /// <summary>
        /// 
        /// </summary>
        public AnnounceResult()
        {
            Peers = new List<Object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodedPeerId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void AddPeer(string encodedPeerId, string ip, int port)
        {
            var PeerId = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(encodedPeerId));

            var NewPeer = new Dictionary<string, Object>
                              {
                                    {
                                        "peer id", 
                                        PeerId
                                    },
                                    {
                                        "ip", 
                                        ip
                                    }, 
                                    {
                                        "port", 
                                        port
                                    }
                              };
            this.Peers.Add(NewPeer);
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
                                    "interval", 
                                    Interval
                                },
                                {
                                    "peers", 
                                    Peers
                                }
                          };
            var response = context.HttpContext.Response;
            response.Write(BEncoder.BEncodeDictionary(res));
            response.End();
        }
    }
}
