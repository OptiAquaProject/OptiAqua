namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;

    /// <summary>
    /// Proporciona información de las etapas del cultivo para una unidad de cultivo
    /// </summary>
    public class EtapasController : ApiController {
        /// <summary>
        /// Etapas de una unidad de cultivo en una temporada.
        /// </summary>
        /// <param name="IdUnidadCultivo"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/etapas/{IdUnidadCultivo}/{fecha}")]
        public IHttpActionResult Get(string IdUnidadCultivo, string fecha) {
            try {
                var idTemporada = DB.TemporadaDeFecha(IdUnidadCultivo,DateTime.Parse(fecha));
                return Json(DB.Etapas(IdUnidadCultivo, idTemporada));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Defines the <see cref="EtapasPost" />
        /// </summary>
        public class EtapasPost {
            /// <summary>
            /// Gets or sets the IdUnidadCultivo
            /// </summary>
            public string IdUnidadCultivo { set; get; }

            /// <summary>
            /// Gets or sets the IdTtemporada
            /// </summary>
            public string IdTemporada { set; get; }

            /// <summary>
            /// Gets or sets the nEtapa
            /// </summary>
            public int nEtapa { set; get; }

            /// <summary>
            /// Gets or sets the FechaConfirmada
            /// </summary>
            public string FechaStrConfirmada { set; get; }

            /// <summary>
            /// Gets or sets the IdTipoEstres
            /// </summary>
            public string IdTipoEstres { set; get; }
        }

        /// <summary>
        /// Actualizar la fecha de cambio de etapa.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] EtapasPost param) {
            try {
                DB.FechaConfirmadaSave(param.IdUnidadCultivo, param.IdTemporada, param.nEtapa, DateTime.Parse(param.FechaStrConfirmada));
                return Ok();
            } catch {
                return BadRequest();
            }
        }
    }
}
