namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;

    /// <summary>
    /// Proporciona los datos de las parcelas y las propiedades de su suelo.
    /// </summary>
    public class ParcelaController : ApiController {
        /// <summary>
        /// Datos de la parcela indicada
        /// </summary>
        /// <param name="idParcela"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Parcela/{idParcela}")]
        public IHttpActionResult Get(int idParcela) {
            try {                
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUsuario = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                var role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;                
                if (!DB.EstaAutorizado(idUsuario, role, idParcela))
                    return Unauthorized();
                return Json(DB.Parcela(idParcela));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista de parcelas de una unidad de cultivo en una temporada
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="idUnidadCultivo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/ParcelasDeUnidadDeCultivo/{IdUnidadCultivo}/{Fecha}")]
        public IHttpActionResult GetParcelasDeUnidadDeCultivo(string fecha, string idUnidadCultivo) {
            try {
                DateTime dFecha = DateTime.Parse(fecha);
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUsuario = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                var role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;
                var idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, dFecha);
                if (!DB.EstaAutorizado(idUsuario, role, idUnidadCultivo, idTemporada))
                    return Unauthorized();

                return Json(DB.ParcelasList(idUnidadCultivo, idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Listado de todas las parcelas
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/parcelas/")]
        public IHttpActionResult GetParcelas() {
            try {
                return Json(DB.ParcelasList());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista con datos ampliados de las parcelas con filtros.
        /// </summary>
        /// <param name="Fecha"></param>
        /// <param name="IdParcela"></param>
        /// <param name="IdRegante"></param>
        /// <param name="IdMunicipio"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/ParcelaList/{IdTemporada}/{IdParcela}/{IdRegante}/{IdMunicipio}/{Search}")]
        public IHttpActionResult GetParcelaList(string Fecha, string IdParcela, string IdRegante, string IdMunicipio, string Search) {
            try {                
                return Json(DB.ParcelaList(Fecha, IdParcela, IdRegante, IdMunicipio, Search));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
