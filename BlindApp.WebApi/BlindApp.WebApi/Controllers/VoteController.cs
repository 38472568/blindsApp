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
    public class VoteController : ApiController
    {
        public List<VoteMaste> GetVotes()
        {
            var result = new List<VoteMaste>();
            try
            {
                using (var db = new BAContext())
                {
                    result = db.Database.SqlQuery<VoteMaste>("select ID,Web_Title as Title,Web_Content as Content, Web_Hits as Hits, time_Start as StartTime, time_End as EndTime, (select count(*) from [Ss_list] l where l.Gid = t.ID) as UserCount from [Site] t order by t.ID desc").ToList();
                }
            }
            catch (Exception ex)
            {
                // 待写日志。
                throw ex;
            }
            return result;
        }

        public VoteMaste GetVoteMaste(int id)
        {
            try
            {
                VoteMaste voteMaste = null;
                using (var db = new BAContext())
                {
                    try
                    {
                        // 更新阅读数量
                        db.Database.ExecuteSqlCommand(string.Format("update Ss_list  set Web_Hits = Web_Hits + 1 where ID = {0} ", id));
                    }
                    finally
                    {
                        // 读取文章内容。
                        var sqlText = string.Format("select ID,Web_Title as Title,Web_Content as Content, Web_Hits as Hits, time_Start as StartTime, time_End as EndTime, (select count(*) from [Ss_list] l where l.Gid = t.ID) as UserCount from [Site] t where t.ID = {0} ", id);
                        voteMaste = db.Database.SqlQuery<VoteMaste>(sqlText).FirstOrDefault();
                        var parser = new HtmlParser(voteMaste.Content);
                        parser.KeepTag(new[] { "p", "img" });
                        var result = parser.Text();
                        // 清除内嵌样式。
                        var content = Regex.Replace(result, "style=\"(.*?)\"", "style=\"TEXT-INDENT: 2em;\"");
                        voteMaste.Content = content;
                    }
                    return voteMaste;
                }
            }
            catch (Exception ex)
            {
                // 待写日志。
                throw ex;
            }
        }
    }
}