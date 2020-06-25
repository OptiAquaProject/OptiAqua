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
        public UnidadCultivoDatosHidricos unidadCultivoDatosHidricos;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceHidrico"/> class.
        /// </summary>
        /// <param name="unidadCultivoDatosHidricos">The unidadCultivoDatosHidricos<see cref="UnidadCultivoDatosHidricos"/></param>
        /// <param name="actualizaEtapas">The actualizaEtapas<see cref="bool"/></param>
        /// <param name="fechaFinalEstudio"></param>
        public BalanceHidrico(UnidadCultivoDatosHidricos unidadCultivoDatosHidricos, bool actualizaEtapas, DateTime fechaFinalEstudio) {
            this.unidadCultivoDatosHidricos = unidadCultivoDatosHidricos;
            CalculaBalance(actualizaEtapas,fechaFinalEstudio);
        }

        /// <summary>
        /// Gets the LineasBalance
        /// LineasBalance. Almacena todas las líneas del balance.
        /// </summary>
        public List<LineaBalance> LineasBalance { get; } = new List<LineaBalance>();

        public static BalanceHidrico Balance(string idUC, DateTime fecha, bool actualizaFechasEtapas = true,bool usarCache=true) {
            BalanceHidrico bh = null;
            if (usarCache==true)
                bh=CacheDatosHidricos.Balance(idUC, fecha);
            if (bh == null) {
                UnidadCultivoDatosHidricos dh = new UnidadCultivoDatosHidricos(idUC, fecha);
                bh = new BalanceHidrico(dh, actualizaFechasEtapas,fecha);
                if (usarCache)
                    CacheDatosHidricos.Add(bh, fecha);
            }
            return bh;
        }

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
            ret = lin.ContenidoAguaSuelo - lin.CapacidadCampo;
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
                    sumaRiegoEfec += lin.RiegoEfectivo;
                    sumaDrenaje += lin.DrenajeProfundidad;
                }
            }
            return (sumaDrenaje + sumaRiego - sumaRiegoEfec);
        }

        /// <summary>
        /// SumaRiegosM3
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaRiegosMm(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Riego);
            return ret;
        }

        /// <summary>
        /// SumaDrenajeM3
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaDrenajeMm(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.DrenajeProfundidad);
            return ret;
        }

        /// <summary>
        /// SumaLluvias
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaLluvias(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Lluvia);
            return ret;
        }

        /// <summary>
        /// SumaLluviasEfectivas
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaLluviasEfectivas(DateTime fecha) {
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.LluviaEfectiva);
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
            return (lin.CapacidadCampo - lin.ContenidoAguaSuelo);
        }

        /// <summary>
        /// NDIasEstres
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int NDIasEstres(DateTime fecha) => LineasBalance.Count(x => (x.Fecha <= fecha) && (x.CoeficienteEstresHidrico < 1));

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
            etc1 = LineasBalance.Find(x => x.Fecha == fecha)?.EtcFinal;
            etc2 = LineasBalance.Find(x => x.Fecha == fecha.AddDays(-1))?.EtcFinal;
            etc3 = LineasBalance.Find(x => x.Fecha == fecha.AddDays(-2))?.EtcFinal;
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
            double ret = lin.CapacidadCampo - lin.PuntoMarchitez;
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
            if (lin.ContenidoAguaSuelo > lin.LimiteAgotamiento) {
                return ((lin.ContenidoAguaSuelo - lin.LimiteAgotamiento) / (lin.CapacidadCampo - lin.LimiteAgotamiento));
            } else {
                return (lin.CoeficienteEstresHidrico - 1);
            }
        }

        /// <summary>
        /// SumaRiegoEfectivo
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double SumaRiegoEfectivo(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.RiegoEfectivo);
            return ret;
        }

        /// <summary>
        /// CalculaBalance. Función de uso interno en la clase para calcular el balance. Se ejecuta una única vez para añadir las líneas de balance.
        /// </summary>
        /// <param name="actualizaEtapas">The actualizaEtapas<see cref="bool"/></param>
        /// <param name="fechaFinalEstudio"></param>
        private void CalculaBalance(bool actualizaEtapas,DateTime fechaFinalEstudio) {
            LineaBalance lbAnt = new LineaBalance();
            DateTime fecha = unidadCultivoDatosHidricos.FechaSiembra();
            //DateTime fechaFinalEstudio = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            int diasDesdeSiembra = 1;
            if (unidadCultivoDatosHidricos.nEtapas <= 0)
                throw new Exception("No se han definido etapas para la unidad de cultivo: " + unidadCultivoDatosHidricos.IdUnidadCultivo);
            while (fecha <= fechaFinalEstudio) {
                LineaBalance lineaBalance = CalculosHidricos.CalculaLineaBalance(unidadCultivoDatosHidricos, lbAnt, fecha);
                lineaBalance.DiasDesdeSiembra = diasDesdeSiembra++;
                LineasBalance.Add(lineaBalance);
                lbAnt = lineaBalance;
                fecha = fecha.AddDays(1);
            }
            if (actualizaEtapas)
                DB.FechasEtapasSave(unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList);
        }

        /// <summary>
        /// CosteAgua
        /// </summary>
        /// <param name="fechaCalculo">The fechaCalculo<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double CosteAgua(DateTime fechaCalculo) {
            double precioM3 = DB.UnidadCultivoTemporadaCosteM3Agua(unidadCultivoDatosHidricos.IdUnidadCultivo, unidadCultivoDatosHidricos.IdTemporada);
            double totalMm = SumaRiegosMm(fechaCalculo);
            double superficie = unidadCultivoDatosHidricos.UnidadCultivoExtensionM2 ?? 0;
            double ret = totalMm / 1000 * precioM3 * superficie;
            return ret;
        }

        /// <summary>
        /// CosteDrenaje
        /// </summary>
        /// <param name="fechaCalculo">The fechaCalculo<see cref="DateTime"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double CosteDrenaje(DateTime fechaCalculo) {
            double? precioM3 = DB.UnidadCultivoTemporadaCosteM3Agua(unidadCultivoDatosHidricos.IdUnidadCultivo, unidadCultivoDatosHidricos.IdTemporada);
            double totalMm = SumaDrenajeMm(fechaCalculo);
            double superficie = unidadCultivoDatosHidricos.UnidadCultivoExtensionM2 ?? 0;
            double? ret = totalMm / 1000 * precioM3 * superficie;
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
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.EtcFinal);
            return ret;
        }

        /// <summary>
        /// DatosEstadoHidrico
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/></param>
        /// <returns>The <see cref="DatosEstadoHidrico"/></returns>
        public DatosEstadoHidrico DatosEstadoHidrico(DateTime fecha) {
            if (fecha > DateTime.Today)
                fecha = DateTime.Today;
            if (fecha > unidadCultivoDatosHidricos.FechaFinalDeEstudio())
                fecha = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            LineaBalance linBalAFecha = LineasBalance.Find(x => x.Fecha == fecha);
            unidadCultivoDatosHidricos.ObtenerMunicicioParaje(out string provincias, out string municipios, out string parajes);
            DatosEstadoHidrico ret = new DatosEstadoHidrico {
                Fecha = fecha,
                Eficiencia = unidadCultivoDatosHidricos.EficienciaRiego,
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
                SumaRiego = SumaRiegosMm(fecha),
                AguaUtil = AguaUtil(fecha),
                RegarEnNDias = RegarEnNDias(fecha),
                AguaUtilTotal = AguaUtilTotal(fecha),
                CapacidadDeCampo = linBalAFecha.CapacidadCampo,
                PuntoDeMarchited = linBalAFecha.PuntoMarchitez,
                AguaUtilOptima = AguaUtilOptima(fecha),
                AguaPerdida = AguaPerdida(fecha),
                CosteAgua = CosteAgua(fecha),
                NDiasEstres = NDIasEstres(fecha),
                EstadoHidrico = IndiceEstadoHidrico(fecha),
                Textura = unidadCultivoDatosHidricos.TipoSueloDescripcion,
                IndiceEstres = linBalAFecha.IndiceEstres,
                DescripcionEstres = linBalAFecha.DescripcionEstres,
                ColorEstres = linBalAFecha.ColorEstres,
                MensajeEstres = linBalAFecha.MensajeEstres,
                Status = "OK",
            };
            return ret;
        }

        /// <summary>
        /// ResumenDiario
        /// </summary>
        /// <param name="fechaDeCalculo">Fecha en la que se desean presentar los datos<see cref="DateTime"/></param>
        /// <returns>The <see cref="ResumenDiario"/></returns>
        public ResumenDiario ResumenDiario(DateTime fechaDeCalculo) {
            if (fechaDeCalculo > unidadCultivoDatosHidricos.FechaFinalDeEstudio())
                fechaDeCalculo = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            if (fechaDeCalculo > DateTime.Today)
                fechaDeCalculo = DateTime.Today;
            if (fechaDeCalculo < unidadCultivoDatosHidricos.FechaSiembra())
                fechaDeCalculo = unidadCultivoDatosHidricos.FechaSiembra();
            ResumenDiario ret = new ResumenDiario();
            LineaBalance lb = LineaBalance(fechaDeCalculo);

            ret.FechaDeCalculo = fechaDeCalculo;
            ret.RiegoTotal = SumaRiegosMm(fechaDeCalculo);
            ret.RiegoEfectivoTotal = SumaRiegoEfectivo(fechaDeCalculo);
            ret.LluviaTotal = SumaLluvias(fechaDeCalculo);
            ret.LluviaEfectivaTotal = SumaLluviasEfectivas(fechaDeCalculo);
            ret.AguaPerdida = AguaPerdida(fechaDeCalculo);
            ret.ConsumoAguaCultivo = SumaConsumoAguaCultivo(fechaDeCalculo);
            ret.DiasEstres = NDIasEstres(fechaDeCalculo);
            ret.DeficitRiego = double.NaN; // Aún no definido
            ret.CosteDeficitRiego = double.NaN; // Aúno no definido.
            ret.CosteAguaRiego = CosteAgua(fechaDeCalculo);
            ret.CosteAguaDrenaje = CosteDrenaje(fechaDeCalculo);

            ret.CapacidadCampo = lb.CapacidadCampo;

            ret.LimiteAgotamiento = lb.LimiteAgotamiento;
            ret.PuntoMarchitez = lb.PuntoMarchitez;
            ret.ContenidoAguaSuelo = lb.ContenidoAguaSuelo;

            ret.CapacidadCampoPorcentaje = 1;
            try {
                ret.LimiteAgotamientoPorcentaje = (ret.LimiteAgotamiento - ret.PuntoMarchitez) / (ret.CapacidadCampo - ret.PuntoMarchitez);
            } catch {
                ret.LimiteAgotamientoPorcentaje = double.NaN;
            }

            ret.PuntoMarchitezPorcentaje = 0;
            try {
                ret.ContenidoAguaSueloPorcentaje = (ret.ContenidoAguaSuelo - ret.PuntoMarchitez) / (ret.CapacidadCampo - ret.PuntoMarchitez);
            } catch {
                ret.ContenidoAguaSueloPorcentaje = double.NaN;
            }

            ret.DrenajeProfundidad = lb.DrenajeProfundidad;
            ret.AvisoDrenaje = CalculosHidricos.AvisoDrenaje(lb.DrenajeProfundidad);

            ret.AguaHastaCapacidadCampo = ret.CapacidadCampo - ret.ContenidoAguaSuelo;
            ret.RecomendacionRiegoNeto = lb.RecomendacionRiegoNeto;
            ret.RecomendacionRiegoTiempo = lb.RecomendacionRiegoTiempo;

            ret.IndiceEstres = lb.IndiceEstres;
            ret.MensajeEstres = lb.MensajeEstres;
            ret.DescripcionEstres = lb.DescripcionEstres;
            ret.ColorEstres = lb.ColorEstres;

            ret.CapacidadCampoRefPM = lb.CapacidadCampoRefPM;
            ret.PuntoMarchitezRefPM = lb.PuntoMarchitezRefPM;
            ret.ContenidoAguaSueloRefPM = lb.ContenidoAguaSueloRefPM;
            ret.LimiteAgotamientoRefPM = lb.LimiteAgotamientoRefPM;
            ret.LimiteAgotamientoFijoRefPM = lb.LimiteAgotamientoFijoRefPM;

            return ret;
        }
    }
}
