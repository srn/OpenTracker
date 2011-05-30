namespace OpenTracker.Models.Tracker
{
    public class AnnounceModel
    {
        /*
        /// <summary>
        /// The 20 byte sha1 hash of the bencoded form of the info value from the metainfo file
        /// </summary>
        /// <remarks>EF doesn't support BINARY</remarks>
        public string info_hash { get; set; }
        */
        /// <summary>
        /// A string of length 20 which the downloaders torrent client uses as its id, in bytes
        /// </summary>
        /// <remarks>EF doesn't support BINARY</remarks>
        public string peer_id { get; set; }

        /// <summary>
        /// The port number the peer is listening on
        /// </summary>
        public int? port { get; set; }

        /// <summary>
        /// The total amount uploaded so far
        /// </summary>
        public long? uploaded { get; set; }

        /// <summary>
        /// The total amount downloaded so far
        /// </summary>
        public long? downloaded { get; set; }

        /// <summary>
        /// What is left of bytes to download
        /// </summary>
        public long? left { get; set; }

        /// <summary>
        /// An optional parameter giving the IP (or dns name) from the peer
        /// </summary>
        public string ip { get; set; } // optional 

        /// <summary>
        /// This is an optional key which maps to started, completed, or stopped
        /// </summary>
        public string Event { get; set; } // optional => started, completed, stopped (or empty, which is the same as not being present)

        /// <summary>
        /// Unique user identification
        /// </summary>
        public string Passkey { get; set; } // optional, but required for private trackers

        /// <summary>
        /// Validates the required tracker GET requests
        /// </summary>
        /// <returns></returns>
        public bool IsValidRequest()
        {
            // !string.IsNullOrEmpty(info_hash.ToString()) && 
            return !string.IsNullOrEmpty(peer_id) && port.HasValue && uploaded.HasValue && downloaded.HasValue && left.HasValue;
        }
    }
}