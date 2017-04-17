using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using BlindApp.Models;
using BlindApp.WebApi.Utilities;

namespace BlindApp.WebApi.Controllers
{
    public class ArticleController : ApiController
    {
        private List<string> _exposeTags = new List<string>{
            "<?xml:namespace prefix = o ns = \"urn:schemas-microsoft-com:office:office\" /><o:p></o:p>",
            "<o:p></o:p>"
        };
        /// <summary>
        /// 读取最新的文章(页面加载时使用)
        /// </summary>
        /// <returns></returns>
        public List<Article> GetArticles()
        {
            var articles = new List<Article>();
            try
            {
                using (var db = new BAContext())
                {
                    articles = db.Database.SqlQuery<Article>("select top 20 ID,Title, AuthorName, Intro, Hits, cast(IsGood as int) as IsGood, null as Content, IndexPicUrl, updatetime from dbo.LZ8_Article t where t.IncludePic = 1 and t.deleted=0 and charindex('UploadFiles', t.IndexPicUrl) > 0 order by t.updatetime desc").ToList();
                }
            }
            catch (Exception ex)
            {
                // 待写日志。
                throw ex;
            }
            return articles;
        }

        /// <summary>
        /// 向前读取的文章(上拉时使用)
        /// </summary>
        /// <param name="maxId"></param>
        /// <returns></returns>
        public List<Article> GetPreArticles(int maxId)
        {
            var articles = new List<Article>();
            try
            {
                using (var db = new BAContext())
                {
                    var sqlText = string.Format("select top 20 ID,Title, AuthorName, Intro, Hits, cast(IsGood as int) as IsGood, null as Content, IndexPicUrl, updatetime from dbo.LZ8_Article t where t.IncludePic = 1 and t.deleted=0 and charindex('UploadFiles', t.IndexPicUrl) > 0 and ID > {0} order by t.updatetime desc", maxId);
                    articles = db.Database.SqlQuery<Article>(sqlText).ToList();
                }
            }
            catch (Exception ex)
            {
                // 待写日志。
                throw ex;
            }
            return articles;
        }
        /// <summary>
        /// 向后读取文章(下拉时使用)
        /// </summary>
        /// <param name="minId"></param>
        /// <returns></returns>
        public List<Article> GetNextArticles(int minId)
        {
            var articles = new List<Article>();
            try
            {
                using (var db = new BAContext())
                {
                    var sqlText = string.Format("select top 20 ID,Title, AuthorName, Intro, Hits, cast(IsGood as int) as IsGood, null as Content, IndexPicUrl, updatetime from dbo.LZ8_Article t where t.IncludePic = 1 and t.deleted=0 and charindex('UploadFiles', t.IndexPicUrl) > 0 and ID < {0} order by t.updatetime desc", minId);
                    articles = db.Database.SqlQuery<Article>(sqlText).ToList();
                }
            }
            catch (Exception ex)
            {
                // 待写日志。
                throw ex;
            }
            return articles;
        }

        public Article GetArticle(int id)
        {
            try
            {
                Article article = null;
                using (var db = new BAContext())
                {
                    try
                    {
                        // 更新阅读数量
                        db.Database.ExecuteSqlCommand(string.Format("update LZ8_Article  set Hits = Hits + 1 where ID = {0} ", id));
                    }
                    finally
                    {
                        // 读取文章内容。
                        var sqlText = string.Format("select ID,Title, AuthorName, Intro, Hits, cast(IsGood as int) as IsGood, Content, IndexPicUrl, updatetime from dbo.LZ8_Article t where t.ID = {0} ", id);
                        article = db.Database.SqlQuery<Article>(sqlText).FirstOrDefault();
                        var parser = new HtmlParser(article.Content);
                        parser.KeepTag(new[] { "p", "img" });
                        var result = parser.Text();
                        // 清除内嵌样式。
                        var content = Regex.Replace(result, "style=\"(.*?)\"", "style=\"TEXT-INDENT: 2em;\"");
                        article.Content = content;
                    }
                    return article;
                }
            }
            catch (Exception ex)
            {
                // 待写日志。
                throw ex;
            }
        }

        [HttpGet] // 此方法名称没用以Get开头，需要加上[HttpGet]属性。
        public void DoIsGood(int articleId, int userId)
        {
            using (var db = new BAContext())
            {
                try
                {
                    // 更新阅读数量
                    db.Database.ExecuteSqlCommand(string.Format("update LZ8_Article  set IsGood = IsGood + 1 where ID = {0} ", articleId));
                    // 记录点赞人
                    //...
                }
                catch (Exception ex)
                {
                    // 待写日志。
                    throw ex;
                }
            }
        }
    }
}