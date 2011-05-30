using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Core.BEncoding;
using OpenTracker.Core.Common;
using OpenTracker.Models.Tracker;

namespace OpenTracker.Controllers.Tracker
{
	public class AnnounceController : Controller
	{
		private readonly int ANNOUNCE_INTERVAL = TrackerSettings.ANNOUNCE_INTERVAL; 
		private readonly bool CHECK_CONNECTABLE = TrackerSettings.CHECK_CONNECTABLE;
		private readonly bool BLACKLIST_PORTS = TrackerSettings.BLACKLIST_PORTS;

		//
		// GET: /Announce/
		// 
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		private Boolean IsPortBlackListed(int port)
		{
			// direct connect
			if (port >= 411 && port <= 413)
				return true;

			// bittorrent
			if (port >= 6881 && port <= 6889)
				return true;

			// kazaa
			if (port == 1214)
				return true;

			// gnutella
			if (port >= 6346 && port <= 6347)
				return true;

			// emule
			if (port == 4662)
				return true;

			// winmx
			if (port == 6699)
				return true;

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		private Boolean IsConnectable(string ip, int port)
		{
			using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				try
				{
					sock.SendTimeout = 2;
#if DEBUG
					sock.Connect("127.0.0.1", port);
#endif
					sock.Connect(ip, port);
					return sock.Connected;
				}
				catch (SocketException ex)
				{
					return ex.ErrorCode == 10061 && sock.Connected;
				}

			}
		}


		//
		// GET: /Announce/Announce/
		// GET: /announce/123af0c917876f6d4711654b2293895f
		public ActionResult Announce(AnnounceModel announceModel)
		{
			if (!announceModel.IsValidRequest())
				return new BTErrorResult("Invalid request (see specification: http://bit.ly/bcYmSu)");

			var passkeyRegex = Regex.IsMatch(announceModel.Passkey, "[0-9a-fA-F]{32}");
			if (!passkeyRegex)
				return new BTErrorResult("Invalid passkey.");

			if (BLACKLIST_PORTS && IsPortBlackListed(Convert.ToInt32(announceModel.port)))
				return new BTErrorResult(string.Format("Port {0} is blacklisted", announceModel.port));

			try
			{
				using (var context = new OpenTrackerDbContext())
				{
					var crntUser = (from u in context.users
									 where u.passkey == announceModel.Passkey
										select u).Take(1).FirstOrDefault();
					if (crntUser == null)
						return new BTErrorResult(string.Format("Unknown passkey. Please re-download the torrent from {0}.",
							TrackerSettings.BASE_URL));

					if (crntUser.activated == 0)
						return new BTErrorResult("Permission denied, you\'re not activated.");

					var seeder = false;
					if (announceModel.left == 0)
						seeder = true;

					// Entity Framework does not support BINARY keys
					var EncodedPeerId = Convert.ToBase64String(Encoding.ASCII.GetBytes(announceModel.peer_id));
					var EncodedInfoHash = BEncoder.FormatUrlInfoHash();

					var torrentExist = (from t in context.torrents
										where t.info_hash == EncodedInfoHash
										select t)
										.Select( t => new
										{
											t.id, 
											t.info_hash,
											t.added
										}).Take(1).FirstOrDefault();
					if (torrentExist == null)
						return new BTErrorResult("Torrent not registered with this tracker.");

					var peerAlreadyExist = (from t in context.peers
											where t.torrentid == torrentExist.id
											&& t.peer_id == EncodedPeerId
											&& t.passkey == announceModel.Passkey
											select t).Take(1);
					var existingPeerCount = peerAlreadyExist.Count();
					peers p;
					if (existingPeerCount == 1)
					{
						p = peerAlreadyExist.First();
					}
					else
					{
						var connectionLimit = (from t in context.peers
											   where t.torrentid == torrentExist.id
											   && t.passkey == announceModel.Passkey
											   select t).Count();
						if (connectionLimit >= 1 && !seeder)
							return new BTErrorResult("Connection limit exceeded! " + 
								"You may only leech from one location at a time.");
						if (connectionLimit >= 3 && seeder)
							return new BTErrorResult("Connection limit exceeded.");


						if (announceModel.left > 0 && crntUser.@class < (decimal)AccountValidation.Class.Administrator)
						{
							var epoch = Unix.ConvertToUnixTimestamp(DateTime.UtcNow);
							var elapsed = Math.Floor((epoch - torrentExist.added) / 3600);

							var uploadedGigs = crntUser.uploaded/(1024*1024*1024);
							var ratio = ((crntUser.downloaded > 0) ? (crntUser.uploaded / crntUser.downloaded) : 1);

							int wait;
							if (ratio < (decimal)0.5 || uploadedGigs < 5)
								wait = 48;
							else if (ratio < (decimal)0.65 || uploadedGigs < (decimal)6.5)
								wait = 24;
							else if (ratio < (decimal)0.8 || uploadedGigs < 8)
								wait = 12;
							else if (ratio < (decimal)0.95 || uploadedGigs < (decimal)9.5)
								wait = 6;
							else
								wait = 0;

							if (elapsed < wait)
								return new BTErrorResult(string.Format("Not authorized (wait {0}h) - READ THE FAQ!",
																	   (wait - elapsed)));
						}

						p = new peers
						{
							torrentid = torrentExist.id,
							peer_id = EncodedPeerId,
							userid = crntUser.id,
							passkey = announceModel.Passkey,
							useragent = Request.UserAgent
						};
					}

					var remoteHost = Request.ServerVariables["REMOTE_HOST"];
					var ip = !string.IsNullOrEmpty(announceModel.ip) ? announceModel.ip : remoteHost;

					if (CHECK_CONNECTABLE) 
						p.connectable = IsConnectable(ip, Convert.ToInt32(announceModel.port)) ? 1 : 0;
					if (announceModel.left != null) 
						p.left = (decimal) announceModel.left;
					p.port = Convert.ToInt32(announceModel.port);
					p.ip = ip;

					p.seeding = seeder ? 1 : 0;

					if (existingPeerCount == 0)
						context.AddTopeers(p);
					else
					{
						if (crntUser.@class < (decimal)AccountValidation.Class.Administrator)
						{
							var nonUpdatedPeer = peerAlreadyExist.First();
							var thisUploaded = (announceModel.uploaded - nonUpdatedPeer.uploaded);
							var thisDownloaded = (announceModel.downloaded - nonUpdatedPeer.downloaded);

							p.uploaded += (decimal)thisUploaded;
							p.downloaded += (decimal)thisDownloaded;

							if (thisUploaded > 0)
								crntUser.uploaded = (crntUser.uploaded + Convert.ToInt64(thisUploaded));
							if (thisDownloaded > 0)
								crntUser.downloaded = (crntUser.downloaded + Convert.ToInt64(thisDownloaded));
						}
					}
					context.SaveChanges();

					if (announceModel.Event == "stopped")
					{
						var removePeer = (from pr in context.peers
										  where pr.torrentid == torrentExist.id
										  && pr.peer_id == EncodedPeerId
										  select pr).Take(1).FirstOrDefault();
						context.peers.DeleteObject(removePeer);
						context.SaveChanges();

						var announceResultStop = new AnnounceResult
						{
							Interval = ANNOUNCE_INTERVAL
						};
						return announceResultStop;
					}

					var announceResult = new AnnounceResult
											 {
												 Interval = ANNOUNCE_INTERVAL
											 };

					var existingPeers = (from t in context.peers
											where t.torrentid == torrentExist.id
											select t).ToList(); 

					foreach (var peer in existingPeers)
						announceResult.AddPeer(peer.peer_id, peer.ip, peer.port);

					return announceResult;
				}
			}
			catch (Exception)
			{
				return new BTErrorResult("Database unavailable");
			}
		}


	}
}
