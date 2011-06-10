using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MonoTorrent.Common;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Core.Common;
using OpenTracker.Models.Tracker;

namespace OpenTracker.Controllers.Tracker
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/
        //
        [AuthorizeUploader]
        public ActionResult Index()
        {
            return View(new UploadModel());
        }

        public List<ModelError> GetModelStateErrorsAsList(ModelStateDictionary state)
        {
            return (from key in state.Keys
                    select state[key].Errors.FirstOrDefault()
                    into error where error != null 
                    select error)
                    .ToList();
        }

        [HttpPost]
        [AuthorizeUploader]
        public ActionResult Index(UploadModel uploadModel)
        {
            if (!ModelState.IsValid)
            {
                /*
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                */
                foreach (var _error in GetModelStateErrorsAsList(this.ModelState))
                    ModelState.AddModelError("", _error.ErrorMessage);
                return View(uploadModel);
            }

            Torrent t;
            try
            {
                t = Torrent.Load(uploadModel.TorrentFile.InputStream);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Invalid .torrent file.");
                return View(uploadModel);
            }
            if (!t.IsPrivate)
            {
                ModelState.AddModelError("", "The torrent file needs to be marked as \"Private\" when you create the torrent.");
                return View(uploadModel);
            }
                
            var TORRENT_DIR = TrackerSettings.TORRENT_DIRECTORY;
            var NFO_DIR = TrackerSettings.NFO_DIRECTORY;

            using (var db = new OpenTrackerDbContext())
            {
                // 
                var _torrentFilename = uploadModel.TorrentFile.FileName;
                if (!string.IsNullOrEmpty(uploadModel.TorrentName))
                    _torrentFilename = uploadModel.TorrentName;

                var cleanTorentFilename = Regex.Replace(_torrentFilename, "[^A-Za-z0-9]", string.Empty);
                var finalTorrentFilename = string.Format("TEMP-{0}-{1}", DateTime.Now.Ticks, cleanTorentFilename);

                var _torrentPath = Path.Combine(TORRENT_DIR, string.Format("{0}.torrent",finalTorrentFilename));
                var _nfoPath = Path.Combine(NFO_DIR, string.Format("{0}.nfo", finalTorrentFilename));
                uploadModel.NFO.SaveAs(_nfoPath);
                uploadModel.TorrentFile.SaveAs(_torrentPath);

                var infoHash = t.InfoHash.ToString().Replace("-", string.Empty);
                var torrentSize = t.Files.Sum(file => file.Length);
                var numfiles = t.Files.Count();
                var client = t.CreatedBy;

                var torrent = new torrents
                {
                    categoryid = uploadModel.CategoryId,
                    info_hash = infoHash,
                    torrentname = _torrentFilename.Replace(".torrent", string.Empty),
                    description = uploadModel.Description,
                    description_small = uploadModel.SmallDescription,
                    added = (int) Unix.ConvertToUnixTimestamp(DateTime.UtcNow),
                    numfiles = numfiles,
                    size = torrentSize,
                    client_created_by = client,
                    owner = new Core.Account.AccountInformation().UserId
                };
                db.AddTotorrents(torrent);
                db.SaveChanges();

                var _torrent = (from tor in db.torrents
                                where tor.info_hash == infoHash
                                select tor)
                                .Select( tor => new { tor.info_hash, tor.id } )
                                .Take(1)
                                .FirstOrDefault();
                if (_torrent == null)
                {
                    // TODO: error logging etc. here
                }
                else
                {
                    System.IO.File.Move(_torrentPath, Path.Combine(TORRENT_DIR, string.Format("{0}.torrent", _torrent.id)));
                    System.IO.File.Move(_nfoPath, Path.Combine(NFO_DIR, string.Format("{0}.nfo", _torrent.id)));

                    var files = t.Files;
                    foreach (var tFile in files.Select(torrentFile => new torrents_files
                        {
                            torrentid = torrent.id,
                            filename = torrentFile.FullPath,
                            filesize = torrentFile.Length
                        }).OrderBy(torrentFile => torrentFile.filename))
                    {
                        db.AddTotorrents_files(tFile);
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Browse");
            }
        }

        [HttpPost]
        [AuthorizeUploader]
        public string Imdb(string title)
        {
            using (var client = new WebClient().OpenRead(string.Format("http://www.imdbapi.com/?i=&t={0}", title)))
            {
                if (client == null)
                    return title;

                using (var reader = new StreamReader(client))
                    return reader.ReadToEnd();
            }
        }

    }
}