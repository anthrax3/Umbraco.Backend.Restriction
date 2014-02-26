using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Umbraco.Backend.Restriction.WebAppTest.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return Content("Test/Index");
        }

        //
        // GET: /Test/Details/5

        public ActionResult Details(int id)
        {
            return Content("/Test/Details/:id");
        }

        //
        // GET: /Test/Create

        public ActionResult Create()
        {
            return Content("/Test/Create/:id");
        }

        //
        // POST: /Test/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return Content("/Test/Create/:id - POST");
            }
            catch
            {
                return Content("/Test/Create/:id - POST");
            }
        }
    }
}
