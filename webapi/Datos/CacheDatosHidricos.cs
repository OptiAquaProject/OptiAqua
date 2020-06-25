namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class CacheDatosHidricos {
        static public DateTime lastUpdate { get; private set; }

        static Dictionary<string, Dictionary<string, BalanceHidrico>> lCacheBalances = new Dictionary<string, Dictionary<string, BalanceHidrico>>();
        static Dictionary<string, Dictionary<string, DatosEstadoHidrico>> lCacheEstadosHidricos = new Dictionary<string, Dictionary<string, DatosEstadoHidrico>>();
        static List<string> lDirtUnidadesCultivo;

        static public bool Refresh(DateTime fecha) {
            return true;
        }

        static public BalanceHidrico Balance(string idUC, DateTime fecha) {
            var idTemporada = DB.TemporadaDeFecha(idUC, fecha);
            if (idTemporada == null)
                return null;
            if (fecha <= lastUpdate) {
                if (lCacheBalances.Keys.Contains(idTemporada)) {
                    var cacheTemporada = lCacheBalances[idTemporada];
                    if (cacheTemporada.Keys.Contains(idUC))
                        return cacheTemporada[idUC];                    
                }
            }
            return null;
        }

        static public bool  recalculando=false;
        static public bool RecreateAll(DateTime fechaUpdate, bool forceUpdate=false) {
            fechaUpdate = fechaUpdate.Date;
            if (recalculando == true)
                return false;
            if ((forceUpdate == false) && (fechaUpdate == lastUpdate)) {                
                return false;
            }
            recalculando = true;
            lastUpdate = fechaUpdate;
            //return true;
            lCacheBalances.Clear();
            lCacheEstadosHidricos.Clear();
            DB.InsertaEvento("Inicia RecreateAll" + DateTime.Now.ToString());
            DB.DatosClimaticosSiarRefresh();
            var db = DB.ConexionOptiaqua;
            var lTemporadas = DB.TemporadasList();
            foreach (var temporada in lTemporadas) {
                var idTemporada = temporada.IdTemporada;
                var fechaCalculo = fechaUpdate;
                if (fechaCalculo >= temporada.FechaFinal)
                    fechaCalculo = temporada.FechaFinal;
                var cacheTemporada = new Dictionary<string, BalanceHidrico>();
                var cacheEstadosHidricosTemporada = new Dictionary<string, DatosEstadoHidrico>();
                lCacheBalances.Add(idTemporada, cacheTemporada);
                lCacheEstadosHidricos.Add(idTemporada, cacheEstadosHidricosTemporada);
                //Lista de unidades de cultivo que tienen un cultivo para la temporada.
                List<string> lIdUnidadCultivo = db.Fetch<string>($"SELECT DISTINCT IdUnidadCultivo from UnidadCultivoCultivo WHERE IdTemporada=@0", idTemporada);

                DatosEstadoHidrico eh = null;
                BalanceHidrico bh = null;
                UnidadCultivoDatosHidricos dh = null;
                List<GeoLocParcela> lGeoLocParcelas = null;
                foreach (string idUc in lIdUnidadCultivo) {
                    try {
                        lGeoLocParcelas = null;
                        lGeoLocParcelas = DB.GeoLocParcelasList(idUc, idTemporada);
                        bh = BalanceHidrico.Balance(idUc, fechaCalculo, true,false);
                        dh = bh.unidadCultivoDatosHidricos;
                        eh = bh.DatosEstadoHidrico(fechaCalculo);
                        eh.GeoLocJson = Newtonsoft.Json.JsonConvert.SerializeObject(lGeoLocParcelas);
                        cacheTemporada.Add(idUc, bh);
                        cacheEstadosHidricosTemporada.Add(idUc, eh);
                    } catch (Exception ex) {
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
                        lCacheEstadosHidricos.Add(idUc, cacheEstadosHidricosTemporada);
                    }
                }
            }
            DB.InsertaEvento("Finaliza RecreateAll" + DateTime.Now.ToString());
            recalculando = false;
            return true;
        }

        internal static void Add(BalanceHidrico bh,DateTime fecha) {
            if (bh == null)
                return;
            var idUC = bh.unidadCultivoDatosHidricos.IdUnidadCultivo;
            var idTemporada = DB.TemporadaDeFecha(idUC, fecha);
            if (fecha >= lastUpdate && bh != null) {
                if (lCacheBalances.Keys.Contains(idTemporada)) {
                    var cacheTemporada = lCacheBalances[idTemporada];
                    if (cacheTemporada == null)
                        return ;                    
                    cacheTemporada.Remove(idUC);
                    cacheTemporada.Add(idUC, bh);
                }
            }
        }
    }    
}
