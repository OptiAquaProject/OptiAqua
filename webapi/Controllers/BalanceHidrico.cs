namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;

    /// <summary>
    /// Proporciona información del balance hídrico
    /// </summary>
    public class BalanceHidricoController : ApiController {
        /// <summary>
        /// Balance hídrico de una unidad de cultivo en una temporada.
        /// </summary>
        /// <param name="idUnidadCultivo">Identificador de la unidad de cultivo</param>
        /// <param name="idTemporada">Identificador de la temporada</param>
        /// <param name="actualizaFechasEtapas">Activar si se desea recalcular las fechas de las etapas para la parcela indicada</param>
        /// <returns></returns>
        [Route("api/balancehidrico/{idUnidadCultivo}/{idTemporada}/{actualizaFechasEtapas}")]
        public IHttpActionResult GetBalanceHidrico(string idUnidadCultivo, string idTemporada, bool actualizaFechasEtapas) {
            try {
                UnidadCultivoDatosHidricos dh = new UnidadCultivoDatosHidricos(idUnidadCultivo, idTemporada);
                BalanceHidrico bh = new BalanceHidrico(dh, actualizaFechasEtapas);
                return Json(bh.LineasBalance);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retorna resumen de los datos hídricos a una fecha.
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        //[Authorize]
        [Route("api/DatosHidricos/{idUnidadCultivo}/{fecha}")]
        public IHttpActionResult GetDatosHidricos(string idUnidadCultivo, string fecha) {
            try {
                /*
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(idUnidadCultivo, idRegante, idTemporada) == false) {
                    return Unauthorized();
                }
                */
                string idTemporada = DB.TemporadaDeFecha(DateTime.Parse(fecha));
                UnidadCultivoDatosHidricos dh = new UnidadCultivoDatosHidricos(idUnidadCultivo, idTemporada);
                BalanceHidrico bh = new BalanceHidrico(dh, true);
                return Json(bh.DatosEstadoHidrico(DateTime.Parse(fecha)));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Listado de los balances hídricos
        /// </summary>
        /// <param name="idRegante"></param>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="idMunicipio"></param>
        /// <param name="idCultivo"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        [Route("api/DatosHidricos/{idRegante}/{idUnidadCultivo}/{idMunicipio}/{idCultivo}/{fecha}")]
        public IHttpActionResult GetDatosHidricosList(int? idRegante, string idUnidadCultivo, int? idMunicipio, string idCultivo, string fecha) {
            try {
                /*
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idReganteClamis = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && idRegante != idReganteClamis)
                    return Unauthorized();
                if (isAdmin == false)
                    idRegante = idReganteClamis;
                string idTemporada = DB.TemporadaDeFecha(DateTime.Parse(fecha));
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(idUnidadCultivo, (int)idRegante, idTemporada) == false)
                    return Unauthorized();
                    */
                var isAdmin = true;
                object lDatosHidricos = CalculosHidricos.DatosHidricosList(idRegante, idUnidadCultivo, idMunicipio, idCultivo, DateTime.Parse(fecha), isAdmin);
                return Json(lDatosHidricos);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retornar los Riegos de una unidad de cultivo en una temporada
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="idTemporada"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Riegos/{idUnidadCultivo}/{idTemporada}")]
        public IHttpActionResult GetRiegos(string idUnidadCultivo, string idTemporada) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(idUnidadCultivo, idRegante, idTemporada) == false) {
                    return Unauthorized();
                }
                return Json(DB.DatosRiegosList(idUnidadCultivo, idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retornar las lluvias registradas para una unidad de cultivo en una temporada
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="idTemporada"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Lluvias/{idUnidadCultivo}/{idTemporada}")]
        public IHttpActionResult GetLluvias(string idUnidadCultivo, string idTemporada) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(idUnidadCultivo, idRegante, idTemporada) == false) {
                    return Unauthorized();
                }
                return Json(DB.DatosLluviaList(idUnidadCultivo, idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  ResumenDiario
        /// </summary>
        /// <param name="idUnidadCultivo">The idUnidadCultivo<see cref="string"/></param>
        /// <param name="fechaStr"></param>                
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [HttpGet]
        [Route("api/ResumenDiario/{idUnidadCultivo}/{fechaStr}")]
        public IHttpActionResult ResumenDiario(string idUnidadCultivo, string fechaStr) {
            try {
                //payaso
                /*
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(idUnidadCultivo, idRegante, idTemporada) == false) {
                    return Unauthorized();
                }
                */
                DateTime fecha = DateTime.Parse(fechaStr);
                var idTemporada = DB.TemporadaDeFecha(fecha);
                UnidadCultivoDatosHidricos dh = new UnidadCultivoDatosHidricos(idUnidadCultivo, idTemporada);
                BalanceHidrico bh = new BalanceHidrico(dh, true);
                return Json(bh.ResumenDiario(fecha));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
