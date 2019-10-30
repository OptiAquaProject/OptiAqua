namespace WebApi {
    using System.Web.Mvc;

    /// <summary>
    /// Página de inicio
    /// </summary>
    public class HomeController : Controller {
        /// <summary>
        ///  Index - Página inicial del controlador
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        public ActionResult Index() {
            ViewBag.Title = "Página de Inicio";

            return View();
        }

        public ActionResult Importacion() {
            ViewBag.Title = "Importación para la creación masica de unidades de cultivo.";
            return View();
        }
        
        public ActionResult EspecificacionesImportacion() {            
            return View();
        }
    }
}
