using System.Collections.Generic;
using System.Linq;
using OpenTracker.Core;

namespace OpenTracker.Models.Forum
{
    public class ForumIndexModel
    {
        public long CategoryId { get; set; }
        public string CategoryTitle { get; set; }

        public long ForumId { get; set; }
        public string ForumTitle { get; set; }
        public string ForumDescription { get; set; }

        public long TopicId { get; set; }
        public string TopicTitle { get; set; }

        public long PostId { get; set; }
        public long PostAdded { get; set; }

        public string Username { get; set; }

        public int TopicCount { get; set; }
        public int PostCount { get; set; }

    }

    
    
    public class ForumTestModel
    {
        static OpenTrackerDbContext context = new OpenTrackerDbContext();

        public List<forum_category> Categories
        {
            get
            {
                var categoriez = (from c in context.forum_category 
                                  select c).ToList();
                return categoriez;
            }
        }

        public static List<forum> Forum(int categoryid)
        {
                /*
                             join f in context.forum on c.id equals f.categoryid
                             join t in context.forum_topic on f.id equals t.forumid
                             join tc in context.forum_topic on f.id equals tc.forumid into tcount
                             join p in context.forum_posts on t.id equals p.topicid
                             join pc in context.forum_posts on t.id equals pc.topicid into pcount
                             join u in context.users on p.userid equals u.id
                             orderby p.added descending

                */
            var _forumz = (from f in context.forum
                            join t in context.forum_topic on f.id equals t.forumid
                            join tc in context.forum_topic on f.id equals tc.forumid into tcount
                            join p in context.forum_posts on t.id equals p.topicid
                            join pc in context.forum_posts on t.id equals pc.topicid into pcount
                            join u in context.users on p.userid equals u.id
                            where f.id == categoryid
                            select new
                                       {
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

                                       });
            return _forumz as List<forum>;
        }


    }


}