namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;

    /// <summary>
    /// Permite guardar y obtener la información relativa a los "Elementos gruesos tipo"
    /// </summary>
    public class ElementosGruesosTipoController : ApiController {
        /// <summary>
        /// Retorna los datos de la tabla "elementos gruesos tipo" para el elemento referenciado por IdElementosGruesosTipo
        /// </summary>
        /// <param name="IdElementosGruesosTipo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/ElementosGruesosTipo/{IdElementosGruesosTipo}")]
        public IHttpActionResult Get(string IdElementosGruesosTipo) {
            try {
                return Json(DB.ElementosGruesosTipo(IdElementosGruesosTipo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Proporcina la lista de los "elemento gruesos tipo"
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/ElementosGruesosTipo/")]
        public IHttpActionResult GetList() {
            try {
                return Json(DB.ElementosGruesosTipoList());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
