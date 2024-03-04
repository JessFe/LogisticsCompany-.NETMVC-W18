using Spedizioni.Models;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View(cliente);
        }


    }
}