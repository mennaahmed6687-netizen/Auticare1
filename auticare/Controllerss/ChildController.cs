using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 using auticare.Data;
using auticare.core;
namespace auticare.Controllerss
{
    public class ChildController : Controller
    {
        private readonly IdataChild<Child> dataChild;

        public ChildController(IdataChild<Child> dataChild )
        {
            this.dataChild = dataChild;
        }
        public ActionResult Index()
        {
            
            return View(dataChild.GetData());
        }

        // GET: ChildController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChildController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChildController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ChildController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChildController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ChildController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChildController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

    public class IdataChild
    {
    }
}
