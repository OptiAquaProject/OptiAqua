namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class CacheUnidadCultivo {
        public DateTime Fecha { set; get; }
        public BalanceHidrico Balance { set; get; }
    }

    public static class CacheDatosHidricos {
        //-------------------------------------------------------------------------------------------IdTemporada
        static Dictionary<string, Dictionary<string, CacheUnidadCultivo>> lCacheBalances = new Dictionary<string, Dictionary<string, CacheUnidadCultivo>>();

        static public BalanceHidrico Balance(string idUC, DateTime fecha) {
            var idTemporada = DB.TemporadaDeFecha(idUC, fecha);
            if (idTemporada == null)
                return null;
            if (!lCacheBalances.ContainsKey(idTemporada)) 
                return null;            
            var cacheTemporada = lCacheBalances[idTemporada];
            if (!cacheTemporada.ContainsKey(idUC))
                return null;
            var cacheUnidadCultivo = cacheTemporada[idUC];
            if (fecha > cacheUnidadCultivo.Fecha)
                return null;
            return cacheUnidadCultivo.Balance;
        }

        static public bool  recalculando=false;
        static public bool RecreateAll() {
            var dateUpdate = DateTime.Now.Date;
            var fechaCalculo = DateTime.Now.Date;
            if (recalculando == true)
                return false;
            recalculando = true;
            //return true;
            lCacheBalances.Clear();
            //lCacheEstadosHidricos.Clear();
            DB.InsertaEvento("Inicia RecreateAll" + DateTime.Now.ToString());
            DB.DatosClimaticosSiarRefresh();
            var db = DB.ConexionOptiaqua;
            var lTemporadas = DB.TemporadasList();
            foreach (var temporada in lTemporadas) {
                var idTemporada = temporada.IdTemporada;
                if (dateUpdate >= temporada.FechaFinal)
                    fechaCalculo = temporada.FechaFinal;
                else
                    fechaCalculo = dateUpdate;
                var cacheTemporada = new Dictionary<string, CacheUnidadCultivo>();
                //var cacheEstadosHidricosTemporada = new Dictionary<string, DatosEstadoHidrico>();
                lCacheBalances.Add(idTemporada, cacheTemporada);
                //lCacheEstadosHidricos.Add(idTemporada, cacheEstadosHidricosTemporada);
                //Lista de unidades de cultivo que tienen un cultivo para la temporada.
                List<string> lIdUnidadCultivo = db.Fetch<string>($"SELECT DISTINCT IdUnidadCultivo from UnidadCultivoCultivo WHERE IdTemporada=@0", idTemporada);

                //DatosEstadoHidrico eh = null;
                BalanceHidrico bh = null;
                //UnidadCultivoDatosHidricos dh = null;
                //List<GeoLocParcela> lGeoLocParcelas = null;
                foreach (string idUc in lIdUnidadCultivo) {
                    try {
                        //lGeoLocParcelas = null;
                        //lGeoLocParcelas = DB.GeoLocParcelasList(idUc, idTemporada);
                        bh = BalanceHidrico.Balance(idUc, fechaCalculo, true,false);
                        //dh = bh.unidadCultivoDatosHidricos;
                        //eh = bh.DatosEstadoHidrico(fechaCalculo);
                        //eh.GeoLocJson = Newtonsoft.Json.JsonConvert.SerializeObject(lGeoLocParcelas);
                        if (bh!=null)
                            cacheTemporada.Add(idUc, new CacheUnidadCultivo { Fecha = dateUpdate, Balance = bh });
                        //cacheEstadosHidricosTemporada.Add(idUc, eh);
                    } catch (Exception ex) {
                        /*
                        dh = bh.unidadCultivoDatosHidricos;
                        dh.ObtenerMunicicioParaje(out string provincias, out string municipios, out string parajes);
                        eh = new DatosEstadoHidrico {
                            Fecha = fechaCalculo,
                            Pluviometria = dh.Pluviometria,
                            TipoRiego = dh.TipoRiego,
                            FechaSiembra = dh.FechaSiembra(),
                            Cultivo = dh.CultivoNombre,
                            Estacion = dh.EstacionNombre,
                            IdEstacion = dh.IdEstacion,
                            IdRegante = dh.IdRegante,
                            IdUnidadCultivo = idUc,
                            Municipios = municipios,
                            Parajes = parajes,
                            Regante = dh.ReganteNombre,
                            Alias = dh.Alias,
                            Eficiencia = dh.EficienciaRiego,
                            IdCultivo = dh.IdCultivo,
                            IdTemporada = dh.IdTemporada,
                            IdTipoRiego = dh.IdTipoRiego,
                            NIF = dh.ReganteNif,
                            Telefono = dh.ReganteTelefono,
                            TelefonoSMS = dh.ReganteTelefonoSMS,
                            SuperficieM2 = dh.UnidadCultivoExtensionM2,
                            NParcelas = dh.NParcelas,
                            Textura = "",
                            GeoLocJson = Newtonsoft.Json.JsonConvert.SerializeObject(lGeoLocParcelas),
                            Status = "ERROR:" + ex.Message
                        };
                        //lCacheEstadosHidricos.Add(idUc, cacheEstadosHidricosTemporada);
                        */
                    }
                }
            }
            DB.InsertaEvento("Finaliza RecreateAll" + DateTime.Now.ToString());
            recalculando = false;
            return true;
        }


        internal static void SetDirty(string idUnidadCultivo) {
            foreach( var cacheTemporada in lCacheBalances.Values ) {
                cacheTemporada.Remove(idUnidadCultivo);
            }            
        }

        internal static void Add(BalanceHidrico bh,DateTime fecha) {
            if (bh == null)
                return;            
            var idUC = bh.unidadCultivoDatosHidricos.IdUnidadCultivo;            
            var idTemporada = DB.TemporadaDeFecha(idUC, fecha);
            if (!lCacheBalances.Keys.Contains(idTemporada))
                lCacheBalances.Add(idTemporada, new Dictionary<string, CacheUnidadCultivo>());
            var cacheTemporada = lCacheBalances[idTemporada];
            cacheTemporada.Remove(idUC);            
            cacheTemporada.Add(idUC, new CacheUnidadCultivo { Fecha = fecha,Balance=bh });
        }
    }    
}
