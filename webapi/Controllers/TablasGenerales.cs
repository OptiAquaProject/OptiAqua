namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;

    /// <summary>
    /// Proporciona información sobre las temporadas
    /// </summary>
    public class TablasMaestrasController : ApiController {
        /// <summary>
        ///  Parajes
        /// </summary>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [HttpGet]
        [Authorize]
        [Route("api/Parajes/")]
        public IHttpActionResult Parajes() {
            try {
                return CacheDatosHidricos.Cache(Request.RequestUri.AbsolutePath, () => {
                    return Json(DB.ParajesList());
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Municipios
        /// </summary>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [HttpGet]
        [Authorize]
        [Route("api/Municipios/")]
        public IHttpActionResult Municipios() {
            try {
                return CacheDatosHidricos.Cache(Request.RequestUri.AbsolutePath, () => {
                    return Json(DB.MunicipiosList());
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Provincias
        /// </summary>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [HttpGet]
        [Authorize]
        [Route("api/Provincias/")]
        public IHttpActionResult Provincias() {
            try {
                return CacheDatosHidricos.Cache(Request.RequestUri.AbsolutePath, () => {
                    return Json(DB.ProvinciaList());
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Cultivos
        /// </summary>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [HttpGet]
        [Authorize]
        [Route("api/Cultivos/")]
        public IHttpActionResult Cultivos() {
            try {
                return CacheDatosHidricos.Cache(Request.RequestUri.AbsolutePath, () => {
                    return Json(DB.CultivosList());
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
