using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using DatosOptiaqua;
using Models;

namespace webapi {

    /// <summary>
    /// login controller class for authenticate users
    /// </summary>
    [RoutePrefix("api/login")]    
    public class LoginController : ApiController {
        [AllowAnonymous]
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing() {
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser() {
            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }
     
        /// <summary>
        /// Cambiar password
        /// </summary>
        /// <param name="loginChange"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("changepassword")]
        public IHttpActionResult ChangePassword(LoginRequest loginChange) {
            try {
                var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;                
                int idReganteEncurso = Int32.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                var isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                string nifReganteEnCurso = identity.Claims.SingleOrDefault(c => c.Type == "NifRegante").Value;
                if (isAdmin == false && loginChange.NifRegante != nifReganteEnCurso) {
                    return BadRequest("No es posible cambiar el password de otro usuario sino es administrador");
                }
                if (DB.PasswordSave(loginChange))
                    return Ok("Contraseña cambiada satisfactoriamente");
                else
                    return BadRequest("No se pudo cambiar contraseña");
            }
            catch {
                return BadRequest();
            }
        }

        /// <summary>
        /// Identificar usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login) {
            try {
                if (login == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                bool isCredentialValid = DB.IsCorrectPassword(login, out Regante regante);
                if (isCredentialValid) {
                    var token = TokenGenerator.GenerateTokenJwt(regante );
                    return Ok(token);
                }
                else {
                    var retardo = TokenGenerator.CalculaRetardo(login.NifRegante);
                    if (retardo > 120 * 1000) // si el tiempo de retardo es muy alto responder error inmediatamente.( son unas 25 peticiones erroneas)
                        return BadRequest();
                    else {
                        System.Threading.Thread.Sleep(retardo);
                        string a1 = login.NifRegante + ":" + login.Password + ":";
                        string pass1 = DB.BuildPassword(login.NifRegante, login.Password);
                        //return Ok(a1+pass1);
                        return Unauthorized();
                    }
                }
            }
            catch {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("LoginAs/{IdRegante}")]
        public IHttpActionResult LoginAs(int idRegante) {
            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            var isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
            if (isAdmin) {
                var regante =  DB.Regante(idRegante) as Regante;
                var token = TokenGenerator.GenerateTokenJwt(regante);
                return Ok(token);
            } else
                return Unauthorized();
        }

    }
}
