namespace WebApi {
    using DatosOptiaqua;
    using Models;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;

    /// <summary>
    /// Proporciona información sobre las temporadas
    /// </summary>
    public class TemporadasController : ApiController {
        /// <summary>
        /// Todas las temporadas definidas.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IHttpActionResult Get() {
            try {
                return Json(DB.TemporadasList());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retorna los datos de una temporada
        /// </summary>
        /// <param name="idTemporada"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/temporada/{idTemporada}")]
        public IHttpActionResult GetTemporada(string idTemporada) {
            try {
                return Json(DB.Temporada(idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Todas las temporadas de la unidad de cultivo indicada.
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/temporadas/{idUnidadCultivo}")]
        public IHttpActionResult Get(string idUnidadCultivo) {
            try {
                /*
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlRegante(idUnidadCultivo, idRegante) == false) {
                    return BadRequest("La Unidad de cultivo no pertenece al regante");
                }
                */
                return Json(DB.TemporadasUnidadCultivoList(idUnidadCultivo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  PostUnidadCultivoTemporadaCosteM3Agua
        /// </summary>
        /// <param name="temporada">The temporada<see cref="Temporada"/></param>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Temporada/")]
        public IHttpActionResult PostUnidadCultivoTemporadaCosteM3Agua([FromBody] Temporada temporada) {
            try {
                /*
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false)
                    return Unauthorized();
                */
                return Json(DB.TemporadaSave(temporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
