using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using OpenTracker.Core;
using OpenTracker.Models.Forum;

namespace OpenTracker.Controllers.Forum
{
    public class ForumController : Controller
    {
        //
        // GET: /Forum/
        //
        public ActionResult Index()
        {
            /*
            using (var context = new OpenTrackerDbContext())
            {
                var forum = (from c in context.forum_category
                             join f in context.forum on c.id equals f.categoryid
                             join t in context.forum_topic on f.id equals t.forumid
                             join tc in context.forum_topic on f.id equals tc.forumid into tcount
                             join p in context.forum_posts on t.id equals p.topicid
                             join pc in context.forum_posts on t.id equals pc.topicid into pcount
                             join u in context.users on p.userid equals u.id
                             orderby p.added descending
                             select new ForumIndexModel
                                        {
                                            CategoryId = c.id,
                                            CategoryTitle = c.title,

                                            ForumId = f.id,
                                            ForumTitle = f.title,
                                            ForumDescription = f.description,
                                            TopicId = t.id,
                                            TopicTitle = t.title,

                                            PostId = p.id,
                                            PostAdded = p.added,

                                            Username = u.username,

                                            TopicCount = tcount.Count(),
                                            PostCount = pcount.Count()
                                        }).ToList();




                // var ggqjler = forum.ToGeneratedSql();
                return View(forum);
            }
            */
            return View();
        }

        public ActionResult Topics()
        {

            return View();
        }

    }

    public static class Test
    {

        public static string ToGeneratedSql(this IQueryable query)
        {
            var _objectQuery = query as ObjectQuery;

            if (_objectQuery == null)
                return null;

            return _objectQuery.ToTraceString();
        }
    }

}
