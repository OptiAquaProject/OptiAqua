﻿namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;

    /// <summary>
    /// Acceso a los datos extra de las parcelas
    /// </summary>
    public class DatosExtraController : ApiController {
        /// <summary>
        /// Devuelve los datos extra de una unidad de cultivo
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/DatosExtra/{idUnidadCultivo}")]
        public IHttpActionResult Get(string idUnidadCultivo) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlRegante(idUnidadCultivo, idRegante) == false) {
                    return Unauthorized();
                }

                return Json(DB.DatosExtraList(idUnidadCultivo));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve los datos extra de una unidad de cultivo a una fecha
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/DatosExtra/{idUnidadCultivo}/{fecha}")]
        public IHttpActionResult Get(string idUnidadCultivo, string fecha) {
            try {
                ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                int idRegante = int.Parse(identity.Claims.SingleOrDefault(c => c.Type == "IdRegante").Value);
                bool isAdmin = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value == "admin";
                if (isAdmin == false && DB.LaUnidadDeCultivoPerteneceAlRegante(idUnidadCultivo, idRegante) == false) {
                    return Unauthorized();
                }
                DateTime f = DateTime.Parse(fecha);
                return Json(DB.DatosExtraList(idUnidadCultivo, f));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Defines the <see cref="PostDatosExtraParam" />
        /// </summary>
        public class PostDatosExtraParam {
            /// <summary>
            /// Gets or sets the IdUnidadCultivo
            /// </summary>
            public string IdUnidadCultivo { set; get; }

            /// <summary>
            /// Gets or sets the Fecha
            /// </summary>
            public string Fecha { set; get; }

            /// <summary>
            /// Gets or sets the Cobertura
            /// </summary>
            public double? Cobertura { set; get; }

            /// <summary>
            /// Gets or sets the Altura
            /// </summary>
            public double? Altura { set; get; }

            /// <summary>
            /// Gets or sets the Lluvia
            /// </summary>
            public double? Lluvia { set; get; }

            /// <summary>
            /// Gets or sets the DriEnd
            /// </summary>
            public double? DriEnd { set; get; }

            /// <summary>
            /// Gets or sets the Riego
            /// </summary>
            public double? Riego { set; get; }
        }

        /// <summary>
        /// Añadir/Actualizar un registro en la tabla datos extra.
        /// Si el valor de los campos cobertura, lluvia,driEnd o riego =-1 no se tiene en cuenta el valor
        /// Ejemplo DatosExtra/2/02-05-2015/-1/-1/-1/0.5  Actualiza únicamente el valor del riego (0.5)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IHttpActionResult Post([FromBody] PostDatosExtraParam param) {
            try {
                DB.DatosExtraSave(param.IdUnidadCultivo, param.Fecha, param.Cobertura, param.Altura, param.Lluvia, param.DriEnd, param.Riego);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
