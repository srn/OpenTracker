using System.Linq;
using System.Web.Mvc;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Models.Tracker;

namespace OpenTracker.Controllers.Tracker
{
    public class BrowseController : Controller
    {
        // 
        // GET: /Browse/
        // 
        [AuthorizeUser]
        public ActionResult Index()
        {
            using (var context = new OpenTrackerDbContext())
            {
                var _torrents = (from t in context.torrents
                                join t2 in context.categories on t.categoryid equals t2.id
                                join t3 in context.comments on t.id equals t3.torrentid into comment
                                join t4 in context.peers on t.id equals t4.torrentid into peer
                                join t5 in context.users on t.owner equals t5.id
                                orderby t.id descending
                                select new TorrentModel
                                           {
                                               TorrentId = t.id,
                                               InfoHash = t.info_hash,
                                               TorrentName = t.torrentname,
                                               Description = t.description,
                                               DescriptionSmall = t.description_small,
                                               Added = t.added,
                                               Size = (long) t.size,
                                               FileCount = t.numfiles,

                                               CategoryId = t2.id,
                                               CategoryImage = t2.image,

                                               CommentCount = comment.Count(),

                                               Seeders = peer.Count(count => count.left == 0),
                                               Leechers = peer.Count(count => count.left > 0),

                                               Uploader = t5.username
                                           }).ToList();
                return View(_torrents);
            }
        }



        public ActionResult RetrieveTorrentFiles(int torrentid)
        {
            using (var context = new OpenTrackerDbContext())
            {
                var _files = (context.torrents_files
                    .Where(f => f.torrentid == torrentid)
                    .OrderBy(f => f.filename))
                    .ToList()
                    .Select(file => new
                                        {
                                            file.filename, 
                                            file.filesize
                                        });
                return Json(new { files = _files }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
