using Microsoft.AspNetCore.Mvc;

namespace MtgCollectionMgrWs.Controllers
{
    public class CollectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
