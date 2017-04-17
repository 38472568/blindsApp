using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlindApp.Models;

namespace BlindApp.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            using(var db = new BAContext())
            {
                var articles = db.Database.SqlQuery<Article>("select top 20 ID,Title, AuthorName, Intro, Hits, IsGood, null as Content, IndexPicUrl, updatetime from dbo.LZ8_Article t where t.IncludePic = 1 and t.deleted=0 order by t.updatetime desc").ToList();
            }

            return View();
        }
    }
}
