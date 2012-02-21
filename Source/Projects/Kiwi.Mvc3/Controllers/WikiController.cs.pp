using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    [HandleError]
    public class WikiController : Controller
    {
        [OutputCache(CacheProfile = "KiwiWikiCache")]
        public ActionResult Doc(string docId)
        {
            return View(KiwiMarkdownService.Instance.GetDocument(docId));
        }
    }
}
