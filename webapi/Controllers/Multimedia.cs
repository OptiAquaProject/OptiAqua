namespace WebApi {
    using DatosOptiaqua;
    using Models;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;
    using webapi.Utiles;

    /// <summary>
    /// Sistema de avisos.
    /// </summary>
    public class MultimediaController : ApiController {
    
        [Authorize]        
        [Route("api/Multimedia/{IdMultimedia}/{IdMultimediaTipo}/{FInicio}/{FFin}/{Activa}/{Search}")]
        public IHttpActionResult GetMultimedia(int? IdMultimedia, int? IdMultimediaTipo, string FInicio, string FFin, int? Activa, string Search) {
            try {
                DateTime? ini = null;
                if (FInicio != "''") {
                    ini = DateTime.Parse(FInicio.Unquoted());
                }
                DateTime? fin = null;
                if (FFin != "''") {
                    fin = DateTime.Parse(FFin.Unquoted());
                }
                return CacheDatosHidricos.Cache(Request.RequestUri.AbsolutePath, () => {
                    return Json(DB.MultimediaList(IdMultimedia, IdMultimediaTipo, ini, fin, Activa, Search));
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/MultimediaTipo/{IdMultimediaTipo}/{Search}")]
        public IHttpActionResult GetMultimediaTipo(int? IdMultimediaTipo, string Search) {
            try {
                return CacheDatosHidricos.Cache(Request.RequestUri.AbsolutePath, () => {
                    return Json(DB.MultimediaTipoList(IdMultimediaTipo, Search));
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Multimedia/")]
        public IHttpActionResult PostMultimedia([FromBody] MultimediaPost multimedia) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false)
                    return Unauthorized();
                CacheDatosHidricos.SetDirtyContainsKey("/Multimedia");
                return Json(DB.MultimediaSave(multimedia));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/MultimediaEliminar/")]
        public IHttpActionResult MultimediaEliminar([FromBody] int idMultimedia) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false)
                    return Unauthorized();
                CacheDatosHidricos.SetDirtyContainsKey("/Multimedia");
                return Json(DB.MultimediaDelete(idMultimedia));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/MultimediaTipo/")]
        public IHttpActionResult PostMultimediaTipo([FromBody] Multimedia_Tipo multimediaTipo) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false)
                    return Unauthorized();
                CacheDatosHidricos.SetDirtyContainsKey("/Multimedia");
                return Json(DB.MultimediaTipoSave(multimediaTipo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/MultimediaTipoEliminar/")]
        public IHttpActionResult MultimediaTipoEliminar([FromBody] int idMultimediaTipo) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false)
                    return Unauthorized();
                CacheDatosHidricos.SetDirtyContainsKey("/Multimedia");
                return Json(DB.MultimediaTipoDelete(idMultimediaTipo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

    }
}
