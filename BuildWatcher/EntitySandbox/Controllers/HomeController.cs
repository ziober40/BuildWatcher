using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EntitySandbox.Context;
using EntitySandbox.Repositories;

namespace EntitySandbox.Controllers
{
    public class HomeController : Controller
    {
        private TfsApiRepository tfsApiRepository;

        public HomeController()
        {
            tfsApiRepository = new TfsApiRepository(System.Configuration.ConfigurationManager.AppSettings["TfsAddress"]);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DependencyWheel()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> PullLastBuildsAsync(int amount = 10)
        {
            var repos = new TfsRepository(new TfsContext());

            
            var list = await Task.Run(() =>tfsApiRepository.GetLastBuilds(amount));
            return Json(list);
        }

        [HttpPost]
        public async Task<JsonResult> PullBuildAsync(int amount, string team)
        {
            return Json(await Task.Run(() => tfsApiRepository.GetLastBuild(amount,team)));
        }
    }
}
