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
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                /*if (isAdmin == false && DB.LaParcelaPerteneceAlRegante(idParcela, idRegante) == false) {
                    return BadRequest("La parcela no pertenece al regante");
                }*/
                return Json(DB.Parcela(idParcela));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista de parcelas de una unidad de cultivo en una temporada
        /// </summary>
        /// <param name="idTemporada"></param>
        /// <param name="IdUnidadCultivo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/ParcelasDeUnidadDeCultivo/{IdUnidadCultivo}/{IdTemporada}")]
        public IHttpActionResult GetParcelasDeUnidadDeCultivo(string idTemporada, string IdUnidadCultivo) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(IdUnidadCultivo, idRegante, idTemporada) == false)
                    return BadRequest("La unidad de cultivo no pertenece al regante");
                return Json(DB.ParcelasList(IdUnidadCultivo, idTemporada));
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
        /// <param name="IdTemporada"></param>
        /// <param name="IdParcela"></param>
        /// <param name="IdRegante"></param>
        /// <param name="IdMunicipio"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/ParcelaList/{IdTemporada}/{IdParcela}/{IdRegante}/{IdMunicipio}/{Search}")]
        public IHttpActionResult GetParcelaList(string IdTemporada, string IdParcela, string IdRegante, string IdMunicipio, string Search) {
            try {
                return Json(DB.ParcelaList(IdTemporada, IdParcela, IdRegante, IdMunicipio, Search));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
