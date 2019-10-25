namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;

    /// <summary>
    /// Permite guardar y obtener la información relativa a los "suelos tipo"
    /// </summary>
    public class SuelosTipoController : ApiController {
        /// <summary>
        /// Proporcina los datos de un suelo tipo
        /// </summary>
        /// <param name="idSueloTipo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/SuelosTipo/{idSueloTipo}")]
        public IHttpActionResult Get(string idSueloTipo) {
            try {
                return Json(DB.SueloTipo(idSueloTipo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Proporcina la lista de los suelos tipo
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/SuelosTipo/")]
        public IHttpActionResult GetList() {
            try {
                return Json(DB.SuelosTipoList());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
