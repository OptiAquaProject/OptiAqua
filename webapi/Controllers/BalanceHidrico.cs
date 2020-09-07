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
        /// <param name="fecha">Identificador de la temporada</param>
        /// <param name="actualizaFechasEtapas">Activar si se desea recalcular las fechas de las etapas para la parcela indicada</param>
        /// <returns></returns>
        [Route("api/balancehidrico/{idUnidadCultivo}/{fecha}/{actualizaFechasEtapas}")]
        public IHttpActionResult GetBalanceHidrico(string idUnidadCultivo, string fecha, bool actualizaFechasEtapas) {
            try {
#if DEBUG
#else
                string clave = "balancehidrico" + idUnidadCultivo + fecha + actualizaFechasEtapas.ToString();
                IHttpActionResult cache = CacheDatosHidricos.ActionResult(clave);
                if (cache != null)
                    return cache;
#endif

                BalanceHidrico bh = BalanceHidrico.Balance(idUnidadCultivo, DateTime.Parse(fecha), actualizaFechasEtapas);
                System.Web.Http.Results.JsonResult<System.Collections.Generic.List<Models.LineaBalance>> ret = Json(bh.LineasBalance);
#if DEBUG
#else
                CacheDatosHidricos.AddActionResult(clave, ret);
#endif
                return ret;
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
        [Authorize]
        [Route("api/DatosHidricos/{idUnidadCultivo}/{fecha}")]
        public IHttpActionResult GetDatosHidricos(string idUnidadCultivo, string fecha) {
            try {
                DateTime dFecha = DateTime.Parse(fecha);
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUsuario = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                string role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;
                string idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, dFecha);
                if (!DB.EstaAutorizado(idUsuario, role, idUnidadCultivo, idTemporada))
                    return Unauthorized();
                BalanceHidrico bh = BalanceHidrico.Balance(idUnidadCultivo, dFecha);
                return Json(bh.DatosEstadoHidrico(dFecha));
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
        [HttpGet]
        [Route("api/DatosHidricos/{idRegante}/{idUnidadCultivo}/{idMunicipio}/{idCultivo}/{fecha}")]
        public IHttpActionResult GetDatosHidricosList(int? idRegante, string idUnidadCultivo, int? idMunicipio, string idCultivo, string fecha) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idReganteClamis = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                string role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;
#if DEBUG
#else
                string clave = "DatosHidricos" + (idRegante.ToString() ?? "") + idUnidadCultivo + (idMunicipio.ToString() ?? "") + idCultivo + fecha.ToString();
                IHttpActionResult cache = CacheDatosHidricos.ActionResult(clave);
                if (cache != null)
                    return cache;
#endif
                object lDatosHidricos = BalanceHidrico.DatosHidricosList(idRegante, idUnidadCultivo, idMunicipio, idCultivo, fecha, role, idReganteClamis);
                System.Web.Http.Results.JsonResult<object> ret = Json(lDatosHidricos);
#if DEBUG
#else
                CacheDatosHidricos.AddActionResult(clave, ret);
#endif
                return ret;
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retornar los Riegos de una unidad de cultivo en una temporada
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Riegos/{idUnidadCultivo}/{fecha}")]
        public IHttpActionResult GetRiegos(string idUnidadCultivo, string fecha) {
            try {
                DateTime dFecha = DateTime.Parse(fecha);
                string idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, dFecha);
                if (idTemporada == null)
                    return BadRequest("La unidad de cultivo no está definida para la temporada");

                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUsuario = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                string role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;
                if (!DB.EstaAutorizado(idUsuario, role, idUnidadCultivo, idTemporada))
                    return Unauthorized();
#if DEBUG
#else
                string clave = "Riegos" + idUnidadCultivo + fecha;
                IHttpActionResult cache = CacheDatosHidricos.ActionResult(clave);
                if (cache != null)
                    return cache;
#endif

                System.Web.Http.Results.JsonResult<object> ret = Json(DB.DatosRiegosList(idUnidadCultivo, idTemporada));
#if DEBUG
#else
                CacheDatosHidricos.AddActionResult(clave, ret);
#endif
                return ret;
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retornar las lluvias registradas para una unidad de cultivo en una temporada
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/Lluvias/{idUnidadCultivo}/{fecha}")]
        public IHttpActionResult GetLluvias(string idUnidadCultivo, string fecha) {
            try {

                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUsuario = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                string role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;
                string idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, DateTime.Parse(fecha));
                if (!DB.EstaAutorizado(idUsuario, role, idUnidadCultivo, idTemporada))
                    return Unauthorized();

#if DEBUG
#else
                string clave = "Lluvias" + idUnidadCultivo + fecha;
                var cache = CacheDatosHidricos.ActionResult(clave);
                if (cache != null)
                    return cache;
#endif

                var ret= Json(DB.DatosLluviaList(idUnidadCultivo, idTemporada));
#if DEBUG
#else
                CacheDatosHidricos.AddActionResult(clave, ret);
#endif
                return ret;
                
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  ResumenDiario
        /// </summary>
        /// <param name="idUnidadCultivo">The idUnidadCultivo<see cref="string"/></param>
        /// <param name="fecha"></param>                
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [HttpGet]
        [Route("api/ResumenDiario/{idUnidadCultivo}/{fecha}")]
        public IHttpActionResult ResumenDiario(string idUnidadCultivo, string fecha) {
            try {
                DateTime dFecha = DateTime.Parse(fecha);

#if DEBUG
#else
                string clave = "ResumenDiario" + idUnidadCultivo + fecha;
                var cache = CacheDatosHidricos.ActionResult(clave);
                if (cache != null)
                    return cache;
#endif

                BalanceHidrico bh = BalanceHidrico.Balance(idUnidadCultivo, dFecha);
                var ret= Json(bh.ResumenDiario(dFecha));

#if DEBUG
#else
                CacheDatosHidricos.AddActionResult(clave, ret);
#endif
                return ret;
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("api/Recalcula/")]
        public IHttpActionResult Recalcula() {
            try {
                CacheDatosHidricos.RecreateAll();
                return Json("OK");
            } catch (Exception ex) {
                CacheDatosHidricos.recalculando = false;
                return BadRequest(ex.Message);
            }
        }
    }
}
