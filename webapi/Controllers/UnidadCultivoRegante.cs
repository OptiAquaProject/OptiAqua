namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;

    /// <summary>
    /// Proporciona las unidades de cultivo asociadas a un regante
    /// </summary>
    public class UnidadCultivoReganteController : ApiController {
        /// <summary>
        /// Unidades de cultivo asociadas a un regante en una temporada
        /// </summary>
        /// <param name="idRegante"></param>
        /// <param name="idTemporada"></param>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [Authorize]
        [Route("api/UnidadCultivoRegante/{idRegante}/{idTemporada}")]
        public IHttpActionResult Get(int idRegante, string idTemporada) {
            try {
                return Json(DB.UnidadesCultivoList(idRegante, idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Unidades de cultivo asociadas a un regante
        /// </summary>
        /// <param name="idRegante"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UnidadCultivoRegante/{idRegante}")]
        public IHttpActionResult Get(int idRegante) {
            try {
                return Json(DB.UnidadCultivoList(idRegante));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
