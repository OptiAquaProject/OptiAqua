namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;

    /// <summary>
    /// Permite guardar y obtener la información relativa a la "materia orgánica tipo"
    /// </summary>
    public class MateriaOrganicaTipoController : ApiController {
        /// <summary>
        /// Proporcina de los valores de la tabla "materia orgánica tipo" para el resgistro referenciado por idMateriaOrganicatipo
        /// </summary>
        /// <param name="idMateriaOrganicatipo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/MateriaOrganicaTipo/{idMateriaOrganicatipo}")]
        public IHttpActionResult Get(string idMateriaOrganicatipo) {
            try {
                return Json(DB.MateriaOrganicaTipo(idMateriaOrganicatipo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Proporcina la lista de las materias orgánicas tipo.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/MateriaOrganicaTipo/")]
        public IHttpActionResult GetListSuelos() {
            try {
                return Json(DB.MateriaOrganicaTipo());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
