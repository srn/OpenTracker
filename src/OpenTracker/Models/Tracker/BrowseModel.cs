namespace OpenTracker.Models.Tracker
{
    public class BrowseModel
    {
        // public List<TorrentModel> Torrents { get; set; }

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

        public int Seeders { get; set; }
        public int Leechers { get; set; }
    }
}