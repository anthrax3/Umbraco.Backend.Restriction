using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Umbraco.Backend.Restriction.WebAppTest.Controllers
{
    public class HomeSurfaceController : Controller
    {
        public ActionResult Index()
        {
            return Content("/HomeSurfaceController/Index");
        }

        public ActionResult Details(int id)
        {
            return Content("/HomeSurfaceController/Details/:Id");
        }

        public ActionResult Create()
        {
            return Content("/HomeSurfaceController/Create - GET");
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            return Content("/HomeSurfaceController/Create - POST");
        }

        public ActionResult Edit(int id)
        {
            return Content("/HomeSurfaceController/Edit");
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            return Content("/HomeSurfaceController/Edit/:id - POST");
        }
    }
}
