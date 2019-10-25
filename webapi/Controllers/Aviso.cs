namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;
    using webapi.Utiles;

    /// <summary>
    /// Sistema de avisos.
    /// </summary>
    public class AvisosController : ApiController {
        /// <summary>
        /// Lista de Avisos
        /// </summary>
        /// <param name="idAviso"></param>
        /// <param name="idAvisoTipo"></param>
        /// <param name="fInicio"></param>
        /// <param name="fFin"></param>
        /// <param name="de"></param>
        /// <param name="para"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Avisos/{IdAviso}/{IdAvisoTipo}/{FInicio}/{FFin}/{De}/{Para}/{Search}")]
        public IHttpActionResult Get(string idAviso, int? idAvisoTipo, string fInicio, string fFin, string de, string para, string search) {
            try {
                DateTime? ini = null;
                if (fInicio != "''") {
                    ini = DateTime.Parse(fInicio.Unquoted());
                }
                DateTime? fin = null;
                if (fFin != "''") {
                    fin = DateTime.Parse(fFin.Unquoted());
                }
                return Json(DB.AvisosList(idAviso, idAvisoTipo, ini, fin, de, para, search));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
