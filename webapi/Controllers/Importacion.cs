namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;
    using webapi.Utiles;

    public class ImportacionController : ApiController {    
        [HttpPost]
        public IHttpActionResult PostImporta([FromBody]ImportaPost param) {
            try {
                if (!DB.IsCorrectPassword(param.NifRegante, param.PassRegante)) {
                    return Json("<h4>Datos de acceso no válidos. Nif o contraseña incorrectos.</h4>");
                }
                if (string.IsNullOrWhiteSpace(param.IdTemporada) || !DB.TemporadaExists(param.IdTemporada)) {
                    return Json("<h4>La temporada indicada no es válida</h4>");
                }
                var lErrores = Importacion.Importar(param);
                var ret = Json(lErrores);
                return ret;
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }            
        }

        public class ImportaPost {
            public string NifRegante { set; get; }
            public string PassRegante { set; get; }
            public string CSV { set; get; }
            public string IdTemporada { set; get; }
            public string IdTemporadaAnterior { set; get; }            
        }
    }
}
