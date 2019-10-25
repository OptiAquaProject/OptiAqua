using Datos.Entidades;
using Newtonsoft.Json;
using NPoco;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace webapi.Controllers
{

    public class AltaTemporadaController : ApiController
    {
        /// <summary>
        /// // GET api/altatemporada/5/T2017/1/05-01-2018
        /// </summary>
        /// <param name="idParcela"></param>
        /// <param name="temporada"></param>
        /// <param name="idCultivo"></param>
        /// <param name="fechaSiembra"></param>
        /// <returns></returns>
        [Route("api/altatemporada/{idParcela}/{temporada}/{idCultivo}/{fechasiembra}")]
        public HttpResponseMessage Get(int idParcela, string temporada, string idCultivo, string idRegante, string fechaSiembra)
        {
            Database db = null;
            try
            {
                db = new Database("CadenaConexionOptiAqua");
                db.BeginTransaction();                
                DateTime fs;
                if (DateTime.TryParse(fechaSiembra, out fs) == false)
                {
                    var ret = new HttpResponseMessage();
                    ret.Content = new StringContent("Error. La fecha de siembra no es correcta. ");
                    return ret;
                }

                if (db== null)
                {

                }
                
                // ¿Ha sido ya dada de alta?
                string sql = " select * from ParcelasCultivosFases where idParcela=@0 and temporada=@1 ";
                int nReg = db.Execute(sql, idParcela, temporada);
                if (nReg > 0)
                {// existe, lo elimino
                    db.Execute(" delete from ParcelasCultivosFases where idParcela=@0 and temporada=@1 ");
                }
                // Crear nueva temporada
                sql = " Select * from CultivosFases where idCultivo=@0";

                db.CompleteTransaction();
                var json = JsonConvert.SerializeObject(db.Fetch<Riegos>(sql));
                var resp = new HttpResponseMessage();
                resp.Content = new StringContent(json);
                resp.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return resp;
            }
            catch (Exception ex)
            {
                db.AbortTransaction();
                var ret = new HttpResponseMessage();
                ret.Content = new StringContent("Error. La fecha de siembra no es correcta. " + ex.Message);
                return ret;
            }

        }
    }


}

