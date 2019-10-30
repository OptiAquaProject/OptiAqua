namespace WebApi {
    using DatosOptiaqua;
    using System;
    using System.Web.Http;
    using webapi.Utiles;

    public class ImportacionController : ApiController {    
        [HttpPost]
        public IHttpActionResult PostImporta([FromBody]ImportaPost param) {
            try {

                //if (!DB.IsCorrectPassword(param.NifRegante, param.PassRegante)) {
                //    return Json("Nif y contraseña no son válidos");
                //}
                // payaso
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
            public bool ModoPruebas { set; get; }
        }
    }
}
