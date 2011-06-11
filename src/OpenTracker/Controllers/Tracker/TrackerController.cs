using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MonoTorrent;
using MonoTorrent.BEncoding;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Core.Common;

namespace OpenTracker.Controllers.Tracker
{
    public class TrackerController : Controller
    {

        //
        // GET: /Tracker/

        public ActionResult Index()
        {


            return View();
        }

        [AuthorizeUser]
        public void DownloadTorrent(int? torrentId)
        {
            using (var db = new OpenTrackerDbContext())
            {
                var torrentExist = (from t in db.torrents
                                    where t.id == torrentId
                                    select t).Take(1).FirstOrDefault();

                if (torrentExist == null)
                {
                    Response.Write("Torrent not found.");
                    return;
                }

                var file = string.Format("{0}.torrent", torrentExist.id);
                var finalTorrentPath = Path.Combine(TrackerSettings.TORRENT_DIRECTORY, file);

                var dictionary = (BEncodedDictionary)BEncodedValue.Decode(System.IO.File.ReadAllBytes(finalTorrentPath));

                var userInformation = (from u in db.users
                                       where u.username == User.Identity.Name
                                        select u).Take(1).FirstOrDefault();
                if (userInformation == null)
                {
                    Response.Write("This shouldn't happen.");
                    return;
                }
                var announceUrl = string.Format("{0}/announce/{1}", TrackerSettings.BASE_URL, userInformation.passkey);
                var editor = new TorrentEditor(dictionary)
                                 {
                                     Announce = announceUrl,
                                     Comment = "created by Open-Tracker.org"
                                 };
                var privateTorrent = editor.ToDictionary().Encode();

                var response = ControllerContext.HttpContext.Response;
                response.ClearHeaders();
                response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.torrent",
                                                                        TrackerSettings.TORRENT_NAME_PREFIX,
                                                                        Url.Encode(torrentExist.torrentname)));
                response.AddHeader("Content-Type", "application/x-bittorrent");
                response.BinaryWrite(privateTorrent);
                response.End();
            }
        }

    }
}
