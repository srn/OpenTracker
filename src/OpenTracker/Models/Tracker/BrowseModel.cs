using System.Collections.Generic;
namespace OpenTracker.Models.Tracker
{
    public class BrowseModel
    {
        public TorrentModel TorrentModel { get; set; }
        public List<CommentModel> CommentModel { get; set; }
    }

    public class TorrentModel
    {
        public long TorrentId { get; set; }
        public string InfoHash { get; set; }
        public string TorrentName { get; set; }
        public string Description { get; set; }
        public string DescriptionSmall { get; set; }
        public long Added { get; set; }
        public long Size { get; set; }
        public long FileCount { get; set; }
        public int Snatches { get; set; }
        public string Uploader { get; set; }

        public int CommentCount { get; set; }

        public long CategoryId { get; set; }
        public string CategoryImage { get; set; }
        public string CategoryName { get; set; }

        public int Seeders { get; set; }
        public int Leechers { get; set; }
    }

    public class CommentModel
    {
        public long CommentId { get; set; }
        public string CommentAuthor { get; set; }
        public string CommentContent { get; set; }
    }
}