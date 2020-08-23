﻿namespace DatosOptiaqua {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="CacheUnidadCultivo" />.
    /// </summary>
    public class CacheUnidadCultivo {
        /// <summary>
        /// Gets or sets the Fecha.
        /// </summary>
        public DateTime Fecha { set; get; }

        /// <summary>
        /// Gets or sets the Balance.
        /// </summary>
        public BalanceHidrico Balance { set; get; }
    }

    /// <summary>
    /// Defines the <see cref="CacheDatosHidricos" />.
    /// </summary>
    public static class CacheDatosHidricos {
        //-------------------------------------------------------------------------------------------IdTemporada
        /// <summary>
        /// Defines the lCacheBalances.
        /// </summary>
        internal static Dictionary<string, Dictionary<string, CacheUnidadCultivo>> lCacheBalances = new Dictionary<string, Dictionary<string, CacheUnidadCultivo>>();

        /// <summary>
        /// The Balance.
        /// </summary>
        /// <param name="idUC">The idUC<see cref="string"/>.</param>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="BalanceHidrico"/>.</returns>
        public static BalanceHidrico Balance(string idUC, DateTime fecha) {
            string idTemporada = DB.TemporadaDeFecha(idUC, fecha);
            if (idTemporada == null)
                return null;
            if (!lCacheBalances.ContainsKey(idTemporada))
                return null;
            Dictionary<string, CacheUnidadCultivo> cacheTemporada = lCacheBalances[idTemporada];
            if (!cacheTemporada.ContainsKey(idUC))
                return null;
            CacheUnidadCultivo cacheUnidadCultivo = cacheTemporada[idUC];
            if (fecha > cacheUnidadCultivo.Fecha)
                return null;
            return cacheUnidadCultivo.Balance;
        }

        /// <summary>
        /// Defines the recalculando.
        /// </summary>
        public static bool recalculando = false;

        /// <summary>
        /// The RecreateAll.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool RecreateAll() {
            DateTime dateUpdate = DateTime.Now.Date;
            DateTime fechaCalculo = DateTime.Now.Date;
            if (recalculando == true)
                return false;
            recalculando = true;
            lCacheBalances.Clear();
            DB.InsertaEvento("Inicia RecreateAll" + DateTime.Now.ToString());
            DB.DatosClimaticosSiarForceRefresh();
            NPoco.Database db = DB.ConexionOptiaqua;
            List<Models.Temporada> lTemporadas = DB.TemporadasList();
            foreach (Models.Temporada temporada in lTemporadas) {
                string idTemporada = temporada.IdTemporada;
                if (dateUpdate >= temporada.FechaFinal)
                    fechaCalculo = temporada.FechaFinal;
                else
                    fechaCalculo = dateUpdate;
                Dictionary<string, CacheUnidadCultivo> cacheTemporada = new Dictionary<string, CacheUnidadCultivo>();
                lCacheBalances.Add(idTemporada, cacheTemporada);
                List<string> lIdUnidadCultivo = db.Fetch<string>($"SELECT DISTINCT IdUnidadCultivo from UnidadCultivoCultivo WHERE IdTemporada=@0", idTemporada);

                BalanceHidrico bh = null;
                foreach (string idUc in lIdUnidadCultivo) {
                    try {
                        bh = BalanceHidrico.Balance(idUc, fechaCalculo, true, false);
                        if (bh != null)
                            cacheTemporada.Add(idUc, new CacheUnidadCultivo { Fecha = dateUpdate, Balance = bh });
                    } catch (Exception) {
                    }
                }
            }
            DB.InsertaEvento("Finaliza RecreateAll" + DateTime.Now.ToString());
            recalculando = false;
            return true;
        }

        /// <summary>
        /// The SetDirty.
        /// </summary>
        /// <param name="idUnidadCultivo">The idUnidadCultivo<see cref="string"/>.</param>
        internal static void SetDirty(string idUnidadCultivo) {
            foreach (Dictionary<string, CacheUnidadCultivo> cacheTemporada in lCacheBalances.Values) {
                cacheTemporada.Remove(idUnidadCultivo);
            }
        }

        /// <summary>
        /// The Add.
        /// </summary>
        /// <param name="bh">The bh<see cref="BalanceHidrico"/>.</param>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        internal static void Add(BalanceHidrico bh, DateTime fecha) {
            if (bh == null)
                return;
            string idUC = bh.unidadCultivoDatosHidricos.IdUnidadCultivo;
            string idTemporada = DB.TemporadaDeFecha(idUC, fecha);
            if (!lCacheBalances.Keys.Contains(idTemporada))
                lCacheBalances.Add(idTemporada, new Dictionary<string, CacheUnidadCultivo>());
            Dictionary<string, CacheUnidadCultivo> cacheTemporada = lCacheBalances[idTemporada];
            cacheTemporada.Remove(idUC);
            cacheTemporada.Add(idUC, new CacheUnidadCultivo { Fecha = fecha, Balance = bh });
        }
    }
}
