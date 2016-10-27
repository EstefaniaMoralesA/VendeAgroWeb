using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VendeAgroWeb.Controllers.Home
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OfertasDelDia()
        {
            return View();
        }

        public ActionResult Anunciate()
        {
            return View();
        }

        public ActionResult Contacto()
        {
            return View();
        }

        public ActionResult CarritoDeCompra()
        {
            return View();
        }
    }
}