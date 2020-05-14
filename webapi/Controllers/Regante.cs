namespace WebApi {
    using DatosOptiaqua;
    using Models;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;

    /// <summary>
    /// Proporciona los datos de las parcelas y las propiedades de su suelo.
    /// </summary>
    public class ReganteController : ApiController {
        /// <summary>
        /// Datos del regante indicado
        /// </summary>
        /// <param name="idRegante"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Regante/{idRegante}")]
        public IHttpActionResult Get(int idRegante) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUser = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && idUser != idRegante) {
                    return BadRequest("La parcela no pertenece al regante");
                }
                return Json(DB.Regante(idRegante));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista los regantes
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/Regantes")]
        public IHttpActionResult Get() {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUser = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false) {
                    return BadRequest("La parcela no pertenece al regante");
                }
                return Json(DB.RegantesList());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista datos ampliados de regantes con filtros
        /// </summary>
        /// <param name="Fecha"></param>
        /// <param name="IdRegante"></param>
        /// <param name="IdUnidadCultivo"></param>
        /// <param name="IdParcela"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/ReganteList/{Fecha}/{IdRegante}/{IdUnidadCultivo}/{IdParcela}/{Search}")]
        public IHttpActionResult GetReganteList(string Fecha, string IdRegante, string IdUnidadCultivo, string IdParcela, string Search) {
            try {
                var idTemporada = DB.TemporadaDeFecha(IdUnidadCultivo,DateTime.Parse(Fecha));
                return Json(DB.ReganteList(idTemporada, IdRegante, IdUnidadCultivo, IdParcela, Search));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualización de los datos del Regante
        /// </summary>
        /// <param name="regante"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/ReganteUpdate")]
        public IHttpActionResult ReganteUpdate([FromBody] RegantePost regante) {
            try {
                DB.ReganteUpdate(regante);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
