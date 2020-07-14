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
    /// Proporciona los datos de las unidades de cultivo y las propiedades de su suelo.
    /// </summary>
    public class UnidadCultivoController : ApiController {
        /// <summary>
        /// Datos de la unidad de cultivo
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UnidadCultivo/{idUnidadCultivo}")]
        public IHttpActionResult Get(string idUnidadCultivo) {
            try {
                return Json(DB.UnidadCultivo(idUnidadCultivo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista de las unidades de Cultivo para una temporada
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UnidadesDeCultivo/{fecha}")]
        public IHttpActionResult GetUnidadesDeCultivo(string fecha) {
            try {
                var lTemporadas = DB.TemporadasDeFecha(DateTime.Parse(fecha));
                return Json(DB.UnidadesDeCultivoList(lTemporadas));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Datos de los horizontes de una unidad de cultivo. Si idHorizonte="ALL" retorna todos los horizontes.
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="idHorizonte"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UnidadCultivo/{fecha}/{idUnidadCultivo}/{idHorizonte}")]
        public IHttpActionResult GetHorizontes(string fecha, string idUnidadCultivo, string idHorizonte) {
            try {
                var idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, DateTime.Parse(fecha));
                return Json(DB.UnidadCultivoHorizonte(idTemporada, idUnidadCultivo, idHorizonte));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Defines the <see cref="ParamPostHorizonte" />
        /// </summary>
        public class ParamPostHorizonte {
            /// <summary>
            /// Gets or sets the IdUnidadCultivo
            /// </summary>
            public string IdUnidadCultivo { set; get; }

            /// <summary>
            /// Gets or sets the IdTemporada
            /// </summary>
            public string Fecha { set; get; }

            /// <summary>
            /// Gets or sets the IdHorizonte
            /// </summary>
            public int IdHorizonte { set; get; }

            /// <summary>
            /// Gets or sets the Limo
            /// </summary>
            public float Limo { set; get; }

            /// <summary>
            /// Gets or sets the Arcilla
            /// </summary>
            public float Arcilla { set; get; }

            /// <summary>
            /// Gets or sets the Arena
            /// </summary>
            public float Arena { set; get; }

            /// <summary>
            /// Gets or sets the MatOrg
            /// </summary>
            public float MatOrg { set; get; }

            /// <summary>
            /// Gets or sets the EleGru
            /// </summary>
            public float EleGru { set; get; }

            /// <summary>
            /// Gets or sets the Prof
            /// </summary>
            public float Prof { set; get; }
        }

        /// <summary>
        /// Actulizar datos horizonte
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IHttpActionResult PostHorizonte([FromBody] ParamPostHorizonte param) {
            try {
                var idTemporada = DB.TemporadaDeFecha(param.IdUnidadCultivo,DateTime.Parse(param.Fecha));
                DB.UnidadCultivoSueloSave(param.IdUnidadCultivo, idTemporada, param.IdHorizonte, param.Limo, param.Arcilla, param.Arena, param.MatOrg, param.EleGru, param.Prof);
                return Ok();
            } catch (Exception) {
                return BadRequest();
            }
        }

        /// <summary>
        /// Da de alta o actualizar un cultivo para la unidad de cultivo indicada y la temporada indicada.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/UnidadCultivoCultivo/")]
        public IHttpActionResult UnidadCultivoCultivo([FromBody] ParamPostUnidadCultivoCultivo param) {
            try {
                var idTemporada = DB.TemporadaDeFecha(param.IdUnidadCultivo,DateTime.Parse(param.Fecha));
                DB.UnidadCultivoCultivoTemporadaSave(param.IdUnidadCultivo, idTemporada, param.IdCultivo, param.IdRegante, param.IdTipoRiego, param.FechaSiembra);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Asignar los valores de un suelo tipo (con sus horizontes) a una unidad de cultivo.
        /// Si ya existiera una definición para el suelo se reemplaza
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/UnidadCultivoSuelo/{fecha}/{IdUnidadCultivo}/{IdSueloTipo}")]
        public IHttpActionResult UnidadCultivoSuelo([FromBody] ParamPostUnidadCultivoSuelo param) {
            try {
                var idTemporada = DB.TemporadaDeFecha(param.IdUnidadCultivo,DateTime.Parse(param.Fecha));
                DB.CultivoSueloSave(param.IdUnidadCultivo, idTemporada, param.IdSueloTipo);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista datos ampliados de unidades de cultivos con filtros
        /// </summary>
        /// <param name="Fecha"></param>
        /// <param name="IdUnidadCultivo"></param>
        /// <param name="IdRegante"></param>
        /// <param name="IdCultivo"></param>
        /// <param name="IdMunicipio"></param>
        /// <param name="IdTipoRiego"></param>
        /// <param name="IdEstacion"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UnidadCultivoList/{Fecha}/{IdUnidadCultivo}/{IdRegante}/{IdCultivo}/{IdMunicipio}/{IdTipoRiego}/{IdEstacion}/{Search}")]
        public IHttpActionResult GetUnidadCultivoList(string Fecha, string IdUnidadCultivo, string IdRegante, string IdCultivo, string IdMunicipio, string IdTipoRiego, string IdEstacion, string Search) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idUsuario = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                var role = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value;

                var idTemporada = "";
                if (DateTime.TryParse(Fecha, out var dFecha))
                    idTemporada = DB.TemporadaDeFecha(IdUnidadCultivo.Unquoted(), dFecha);
                else
                    idTemporada = DB.TemporadaActiva();
                return Json(DB.UnidadCultivoList(idTemporada, IdUnidadCultivo, IdRegante, IdCultivo, IdMunicipio, IdTipoRiego, IdEstacion, Search,idUsuario,role));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  PostPluviometria
        /// </summary>
        /// <param name="param">The param<see cref="ParamPostPluviometria"/></param>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [Authorize]
        [HttpPost]
        [Route("api/pluviometria/")]
        public IHttpActionResult PostPluviometria([FromBody] ParamPostPluviometria param) {
            try {
                var idTemporada = DB.TemporadaDeFecha(param.IdUnidadCultivo,DateTime.Parse(param.Fecha));
                DB.PluviometriaSave(idTemporada, param.IdUnidadCultivo, param.Valor);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retornar datos ampliados de la unidad de cultivo.
        /// Fecha puede ser '' para presentar todos
        /// IdUnidadCultivo puede ser '' para presentar todos
        /// </summary>
        /// <param name="Fecha"></param>
        /// <param name="IdUnidadCultivo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/UnidadCultivoDatosAmpliados/{Fecha}/{IdUnidadCultivo}")]
        public IHttpActionResult GetUnidadCultivoDatosAmpliados(string Fecha, string IdUnidadCultivo) {
            try {
                string idTemporada;
                if (string.IsNullOrWhiteSpace(Fecha))
                    idTemporada = "";
                else
                    idTemporada = DB.TemporadaDeFecha(IdUnidadCultivo, DateTime.Parse(Fecha));
                return Json(DB.UnidadCultivoDatosAmpliados(idTemporada, IdUnidadCultivo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  UnidadCultivoTemporadaCosteM3Agua
        /// </summary>
        /// <param name="Fecha">Fecha<see cref="string"/></param>
        /// <param name="IdUnidadCultivo">The IdUnidadCultivo<see cref="string"/></param>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [Authorize]
        [HttpGet]
        [Route("api/UnidadCultivoTemporadaCosteM3Agua/{IdUnidadCultivo}/{Fecha}")]
        public IHttpActionResult UnidadCultivoTemporadaCosteM3Agua(string Fecha, string IdUnidadCultivo) {
            try {
                var idTemporada = DB.TemporadaDeFecha(IdUnidadCultivo,DateTime.Parse(Fecha));
                return Json(DB.UnidadCultivoTemporadaCosteM3Agua(IdUnidadCultivo, idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  PostUnidadCultivoTemporadaCosteM3Agua
        /// </summary>
        /// <param name="param">The param<see cref="ParamPostCosteM3Agua"/></param>
        /// <returns>The <see cref="IHttpActionResult"/></returns>
        [Authorize]
        [HttpPost]
        [Route("api/UnidadCultivoTemporadaCosteM3Agua/")]
        public IHttpActionResult PostUnidadCultivoTemporadaCosteM3Agua([FromBody] ParamPostCosteM3Agua param) {
            try {
                return Json(DB.UnidadCultivoTemporadaCosteM3AguaSave(param));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
