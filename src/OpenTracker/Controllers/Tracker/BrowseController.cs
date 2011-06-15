using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Core.Common;
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


        public ActionResult Details(int id)
        {
            using (var context = new OpenTrackerDbContext())
            {
                var _torrent = (from t in context.torrents
                                    join t2 in context.categories on t.categoryid equals t2.id
                                    join t3 in context.users on t.owner equals t3.id
                                    join t4 in context.peers on t.id equals t4.torrentid into peer
                                    where t.id == id
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

                                               CategoryName = t2.name,

                                               Seeders = peer.Count(count => count.left == 0),
                                               Leechers = peer.Count(count => count.left > 0),

                                               Uploader = t3.username
                                        }).Take(1).FirstOrDefault();

                var comments = (from c in context.comments
                                join u in context.users on c.userid equals u.id
                                where c.torrentid == id
                                select new CommentModel
                                    {
                                        CommentId = c.id,
                                        CommentAuthor = u.username,
                                        CommentContent = c.comment

                                    }).ToList();


                return View(new BrowseModel {TorrentModel = _torrent, CommentModel = comments });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="torrentid"></param>
        /// <returns></returns>
        public JsonResult RetrieveTorrentFiles(int torrentid)
        {
            using (var context = new OpenTrackerDbContext())
            {
                var _files = (context.torrents_files
                    .Where(f => f.torrentid == torrentid)
                    .OrderBy(f => f.filename))
                    .Select(file => new
                                        {
                                            file.filename, 
                                            file.filesize
                                        })
                    .ToList();
                return Json(new { files = _files }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="limit"></param>
        /// <param name="timespamp"></param>
        /// <returns></returns>
        public string AutoSuggest(string q, int? limit, long? timespamp)
        {
            using (var context = new OpenTrackerDbContext())
            {
                var _torrents = (context.torrents
                    .Where(f => f.torrentname.Contains(q))
                    .OrderBy(f => f.torrentname))
                    .Select(torrent => new
                    {
                        torrent.id,
                        torrent.torrentname
                    })
                    .ToList();
                // return Json(new { torrents = _torrents }, JsonRequestBehavior.AllowGet);
                var bewlder = new StringBuilder();
                foreach (var torrent in _torrents)
                    bewlder.Append(torrent.torrentname + Environment.NewLine);
                return bewlder.ToString();
            }
        }


    }
}
