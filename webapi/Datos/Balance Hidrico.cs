namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="BalanceHidrico" />
    /// Crear el balance hídrico y proporciona varias funciones resumen del balance
    /// </summary>
    public class BalanceHidrico {
        /// <summary>
        /// unidadCultivoDatosHidricos referencia al objeto que proporciona todos los datos necesarios para crear el balance hídrico
        /// </summary>
        private UnidadCultivoDatosHidricos unidadCultivoDatosHidricos;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceHidrico"/> class.
        /// </summary>
        /// <param name="unidadCultivoDatosHidricos">The unidadCultivoDatosHidricos<see cref="UnidadCultivoDatosHidricos"/></param>
        /// <param name="actualizaFases">The actualizaFases<see cref="bool"/></param>
        public BalanceHidrico(UnidadCultivoDatosHidricos unidadCultivoDatosHidricos, bool actualizaFases) {
            this.unidadCultivoDatosHidricos = unidadCultivoDatosHidricos;
            CalculaBalance(actualizaFases);
        }

        /// <summary>
        /// Gets the LineasBalance
        /// LineasBalance. Almacena todas las líneas del balance.
        /// </summary>
        public List<LineaBalance> LineasBalance { get; } = new List<LineaBalance>();

        /// <summary>
        /// AguaUtil
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double AguaUtil(DateTime fecha) {
            double ret = 0;
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                return 0;
            ret = lin.OS - lin.CC;
            return ret;
        }

        /// <summary>
        /// AguaPerdida
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double AguaPerdida(DateTime fecha) {
            double sumaRiego = 0;
            double sumaRiegoEfec = 0;
            double sumaDrenaje = 0;
            foreach (LineaBalance lin in LineasBalance) {
                if (lin.Fecha <= fecha) {
                    sumaRiego += lin.Riego;
                    sumaRiegoEfec += lin.RieEfec;
                    sumaDrenaje += lin.Dp;
                }
            }
            return (sumaDrenaje + sumaRiego - sumaRiegoEfec);
        }

        /// <summary>
        /// SumaRiegosM3
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaRiegosM3(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Riego);
            return ret;
        }

        /// <summary>
        /// SumaDrenajeM3
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaDrenajeM3(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Dp);
            return ret;
        }

        /// <summary>
        /// SumaLluvias
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaLluvias(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.lluvia);
            return ret;
        }

        /// <summary>
        /// SumaLluviasEfectivas
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaLluviasEfectivas(DateTime fecha) {
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Pef);
            return ret;
        }

        /// <summary>
        /// AguaUtilOptima
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double AguaUtilOptima(DateTime fecha) {
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                throw new Exception("No se encontraron datos del balance para esa fecha.");
            return (lin.CC - lin.OS);
        }

        /// <summary>
        /// NDIasEstres
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int NDIasEstres(DateTime fecha) => LineasBalance.Count(x => (x.Fecha <= fecha) && (x.Ks < 1));

        /// <summary>
        /// ETcMedia3Dias
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double ETcMedia3Dias(DateTime fecha) {
            double ret = 0;
            int nItem = 0;
            double suma = 0;
            double? etc1, etc2, etc3;
            int c = LineasBalance.Count;
            etc1 = LineasBalance.Find(x => x.Fecha == fecha)?.EtcAdj;
            etc2 = LineasBalance.Find(x => x.Fecha == fecha.AddDays(-1))?.EtcAdj;
            etc3 = LineasBalance.Find(x => x.Fecha == fecha.AddDays(-2))?.EtcAdj;
            if (etc1 != null) {
                nItem++;
                suma += (double)etc1;
            }
            if (etc2 != null) {
                nItem++;
                suma += (double)etc2;
            }
            if (etc3 != null) {
                nItem++;
                suma += (double)etc3;
            }
            if (nItem == 0)
                throw new Exception("No se encontraron valores etc para esas fechas");
            ret = suma / nItem;
            return ret;
        }

        /// <summary>
        /// AguaUtilTotal
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double AguaUtilTotal(DateTime fecha) {
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                throw new Exception("No se encontraron valores en el balance para la fecha indicada.");
            double ret = lin.CC - lin.PM;
            return ret;
        }

        /// <summary>
        /// RegarEnNDias
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int RegarEnNDias(DateTime fecha) {
            double etc = ETcMedia3Dias(fecha);
            double aguaUtil = AguaUtil(fecha);
            if (aguaUtil < 0) // hay deficit de agua
                return 0;
            if (etc == 0)
                throw new Exception("No se puede calcular RegarEnNDias con valores etc=0");
            double ret = Math.Round(aguaUtil / etc, 0);
            return (int)ret;
        }

        /// <summary>
        /// Devuelve un valor entre -1 y 1 indicando es estado hidrico a una fecha.
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public double IndiceEstadoHidrico(DateTime fecha) {
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                throw new Exception("No se encontraron datos del balance para esa fecha.");
            if (lin.OS > lin.LO) {
                return ((lin.OS - lin.LO) / (lin.CC - lin.LO));
            } else {
                return (lin.Ks - 1);
            }
        }

        /// <summary>
        /// SumaRiegoEfectivo
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaRiegoEfectivo(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.RieEfec);
            return ret;
        }

        /// <summary>
        /// CalculaBalance. Función de uso interno en la clase para calcular el balance. Se ejecuta una única vez para añadir las líneas de balance.
        /// </summary>
        /// <param name="actualizaFases">The actualizaFases<see cref="bool"/></param>
        private void CalculaBalance(bool actualizaFases) {
            LineaBalance lbAnt = new LineaBalance();
            DateTime fecha = unidadCultivoDatosHidricos.FechaSiembra();
            DateTime fechaFinalEstudio = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            int dda = 1;
            if (unidadCultivoDatosHidricos.NFases <= 0)
                throw new Exception("No se han definido fases para la unidad de cultivo: " + unidadCultivoDatosHidricos.IdUnidadCultivo);
            while (fecha <= fechaFinalEstudio) {
                LineaBalance lineaBalance = CalculosHidricos.CalculaLineaBalance(unidadCultivoDatosHidricos, lbAnt, fecha);
                lineaBalance.DDA = dda++;
                LineasBalance.Add(lineaBalance);
                lbAnt = lineaBalance;
                fecha = fecha.AddDays(1);
            }
            if (actualizaFases)
                DB.FechasFasesSave(unidadCultivoDatosHidricos.UnidadCultivoCultivoFasesList);
        }

        /// <summary>
        /// CosteAgua
        /// </summary>
        /// <param name="fechaCalculo">The fechaCalculo<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double CosteAgua(DateTime fechaCalculo) {
            double? precioM3 = DB.UnidadCultivoTemporadaCosteM3Agua(unidadCultivoDatosHidricos.IdUnidadCultivo, unidadCultivoDatosHidricos.IdTemporada);
            double? ret = SumaRiegosM3(fechaCalculo) * precioM3;
            return ret ?? 0;
        }

        /// <summary>
        /// CosteDrenaje
        /// </summary>
        /// <param name="fechaCalculo">The fechaCalculo<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double CosteDrenaje(DateTime fechaCalculo) {
            double? precioM3 = DB.UnidadCultivoTemporadaCosteM3Agua(unidadCultivoDatosHidricos.IdUnidadCultivo, unidadCultivoDatosHidricos.IdTemporada);
            double? ret = SumaDrenajeM3(fechaCalculo) * precioM3;
            return ret ?? 0;
        }

        /// <summary>
        /// LineaBalance
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="LineaBalance"/></returns>
        private LineaBalance LineaBalance(DateTime fecha) => LineasBalance.Find(x => x.Fecha == fecha);

        /// <summary>
        /// SumaConsumoAguaCultivo
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaConsumoAguaCultivo(DateTime fecha) {
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.EtcAdj);
            return ret;
        }

        /// <summary>
        /// DatosEstadoHidrico
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="DatosEstadoHidrico"/></returns>
        public DatosEstadoHidrico DatosEstadoHidrico(DateTime fecha) {
            //CalculaBalance(true);
            LineaBalance linBalAFecha = LineasBalance.Find(x => x.Fecha == fecha);
            unidadCultivoDatosHidricos.ObtenerMunicicioParaje(out string municipios, out string parajes);
            DatosEstadoHidrico ret = new DatosEstadoHidrico {
                Eficiencia = unidadCultivoDatosHidricos.Eficiencia,
                Alias = unidadCultivoDatosHidricos.Alias,
                IdCultivo = unidadCultivoDatosHidricos.IdCultivo,
                SuperficieM2 = unidadCultivoDatosHidricos.UnidadCultivoExtensionM2,
                IdTemporada = unidadCultivoDatosHidricos.IdTemporada,
                IdTipoRiego = unidadCultivoDatosHidricos.IdTipoRiego,
                NParcelas = unidadCultivoDatosHidricos.NParcelas,
                Regante = unidadCultivoDatosHidricos.ReganteNombre,
                NIF = unidadCultivoDatosHidricos.ReganteNif,
                Municipios = municipios,
                Parajes = parajes,
                Telefono = unidadCultivoDatosHidricos.ReganteTelefono,
                TelefonoSMS = unidadCultivoDatosHidricos.ReganteTelefonoSMS,
                Pluviometria = unidadCultivoDatosHidricos.Pluviometria,
                TipoRiego = unidadCultivoDatosHidricos.TipoRiego,
                FechaSiembra = unidadCultivoDatosHidricos.FechaSiembra(),
                Cultivo = unidadCultivoDatosHidricos.CultivoNombre,
                IdUnidadCultivo = unidadCultivoDatosHidricos.IdUnidadCultivo,
                Estacion = unidadCultivoDatosHidricos.EstacionNombre,
                IdRegante = unidadCultivoDatosHidricos.IdRegante,
                IdEstacion = unidadCultivoDatosHidricos.IdEstacion,
                SumaLluvia = SumaLluvias(fecha),
                SumaRiego = SumaRiegosM3(fecha),
                AguaUtil = AguaUtil(fecha),
                RegarEnNDias = RegarEnNDias(fecha),
                AguaUtilTotal = AguaUtilTotal(fecha),
                CapacidadDeCampo = linBalAFecha.CC,
                PuntoDeMarchited = linBalAFecha.PM,
                AguaUtilOptima = AguaUtilOptima(fecha),
                AguaPerdida = AguaPerdida(fecha),
                CosteAgua = CosteAgua(fecha),
                NDiasEstres = NDIasEstres(fecha),
                EstadoHidrico = IndiceEstadoHidrico(fecha),
                Textura = unidadCultivoDatosHidricos.TipoSueloDescripcion,
                Status = "OK",
            };
            return ret;
        }

        /// <summary>
        /// ResumenDiario
        /// </summary>
        /// <returns>The <see cref="ResumenDiario"/></returns>
        public ResumenDiario ResumenDiario() {
            DateTime fechaDeCalculo = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            ResumenDiario ret = new ResumenDiario();
            LineaBalance lb = LineaBalance(fechaDeCalculo);

            ret.RiegoTotal = SumaRiegosM3(fechaDeCalculo);
            ret.RiegoEfecTotal = SumaRiegoEfectivo(fechaDeCalculo);
            ret.LluviaTotal = SumaLluvias(fechaDeCalculo);
            ret.LluviaEfecTotal = SumaLluviasEfectivas(fechaDeCalculo);
            ret.AguaPerdida = AguaPerdida(fechaDeCalculo);
            ret.ConsumoAguaCultivo = SumaConsumoAguaCultivo(fechaDeCalculo);
            ret.DiasEstres = NDIasEstres(fechaDeCalculo);
            ret.DeficitRiego = double.NaN; // Aún no definido
            ret.CosteDeficitRiego = double.NaN; // Aúno no definido.
            ret.CosteAguaRiego = CosteAgua(fechaDeCalculo);
            ret.CosteAguaDrenaje = CosteDrenaje(fechaDeCalculo);

            ret.CC = lb.CC;

            ret.LO = lb.LO;
            ret.PM = lb.PM;
            ret.OS = lb.OS;

            ret.CC_porcent = ret.CC / 100;
            try {
                ret.LO_porcent = (ret.LO - ret.PM) / (ret.CC - ret.PM);
            } catch {
                ret.LO_porcent = double.NaN;
            }

            ret.PM_porcent = 0;
            try {
                ret.OS_porcent = (ret.OS - ret.PM) / (ret.CC - ret.PM);
            } catch {
                ret.OS_porcent = double.NaN;
            }


            ret.DP = lb.Dp;
            ret.AvisoDrenaje = CalculosHidricos.AvisoDrenaje(lb.Dp);

            ret.AguaHastaCC = ret.CC - ret.OS;
            ret.RecRegMm = lb.RecRegMm;
            ret.RecRegTpo = lb.RecRegTpo;
            ret.IndiceEstres = CalculosHidricos.IndiceEstres(lb.OS, lb.LO, lb.Ks, lb.CC);
            ret.ClaseEstres = unidadCultivoDatosHidricos.ClaseEstres(ret.IndiceEstres, lb.NFase);
            return ret;
        }
    }
}
