namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using webapi.Utiles;

    /// <summary>
    /// Defines the <see cref="BalanceHidrico" />
    /// Crear el balance hídrico y proporciona varias funciones resumen del balance.
    /// </summary>
    public class BalanceHidrico {
        /// <summary>
        /// unidadCultivoDatosHidricos referencia al objeto que proporciona todos los datos necesarios para crear el balance hídrico.
        /// </summary>
        public UnidadCultivoDatosHidricos unidadCultivoDatosHidricos;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceHidrico"/> class.
        /// </summary>
        /// <param name="unidadCultivoDatosHidricos">The unidadCultivoDatosHidricos<see cref="UnidadCultivoDatosHidricos"/>.</param>
        /// <param name="actualizaEtapas">The actualizaEtapas<see cref="bool"/>.</param>
        /// <param name="fechaFinalEstudio">.</param>
        public BalanceHidrico(UnidadCultivoDatosHidricos unidadCultivoDatosHidricos, bool actualizaEtapas, DateTime fechaFinalEstudio) {
            this.unidadCultivoDatosHidricos = unidadCultivoDatosHidricos;
            CalculaBalance(actualizaEtapas, fechaFinalEstudio);
        }

        /// <summary>
        /// Gets the LineasBalance
        /// LineasBalance. Almacena todas las líneas del balance..
        /// </summary>
        public List<LineaBalance> LineasBalance { get; } = new List<LineaBalance>();

        /// <summary>
        /// The Balance.
        /// </summary>
        /// <param name="idUC">The idUC<see cref="string"/>.</param>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <param name="actualizaFechasEtapas">The actualizaFechasEtapas<see cref="bool"/>.</param>
        /// <param name="usarCache">The usarCache<see cref="bool"/>.</param>
        /// <returns>The <see cref="BalanceHidrico"/>.</returns>
        public static BalanceHidrico Balance(string idUC, DateTime fecha, bool actualizaFechasEtapas = true, bool usarCache = true) {
#if DEBUG
            usarCache = false;
#endif
            BalanceHidrico bh = null;
            if (usarCache == true)
                bh = CacheDatosHidricos.Balance(idUC, fecha);
            if (bh == null) {
                UnidadCultivoDatosHidricos dh = new UnidadCultivoDatosHidricos(idUC, fecha);
                bh = new BalanceHidrico(dh, actualizaFechasEtapas, dh.FechaFinalDeEstudio());
                if (usarCache)
                    CacheDatosHidricos.Add(bh, fecha);
            }
            return bh;
        }

        /// <summary>
        /// AguaUtil.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double AguaUtil(DateTime fecha) { // !!!SIAR esta formula es errónea y se puede eliminar
            double ret = 0;
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                return 0;
            ret = lin.CapacidadCampo - lin.ContenidoAguaSuelo;
            return ret;
        }

        /// <summary>
        /// AguaPerdida.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double AguaPerdida(DateTime fecha) { // !!! SIAR, eliminar esta función sustituyendola por AguaPerdidaEficienciaRiego
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
        /// AguaPerdidaEficienciaRiego.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double AguaPerdidaEficienciaRiego(DateTime fecha) { // !!! SIAR, añadir al glosario
            double sumaRiego = 0;
            double sumaRiegoEfec = 0;
            foreach (LineaBalance lin in LineasBalance) {
                if (lin.Fecha <= fecha) {
                    sumaRiego += lin.Riego;
                    sumaRiegoEfec += lin.RiegoEfectivo;
                }
            }
            return (sumaRiego - sumaRiegoEfec);
        }
        /// <summary>
        /// Suma agua perdida por drenaje hasta fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double AguaTotalPerdidaDrenaje(DateTime fecha) { //!!! SIAR en el glosario se llama a este parámetro AguaPerdida, cambiar glosario, ver comentario funcion SumaDrenajeMm
            double ret = LineasBalance.Sum(x => x.Fecha > fecha ? 0 : x.DrenajeProfundidad);
            return ret;
        }

        /// <summary>
        /// SumaRiegosM3.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaRiegosMm(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Riego);
            return ret;
        }

        /// <summary>
        /// SumaDrenajeM3.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaDrenajeMm(DateTime fecha) { //!!! SIAR esta función y la función AguaTotalPerdidaDrenaje son iguales, eliminar una de ellas
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.DrenajeProfundidad);
            return ret;
        }

        /// <summary>
        /// SumaLluvias.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaLluvias(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.Lluvia);
            return ret;
        }

        /// <summary>
        /// SumaLluviasEfectivas.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaLluviasEfectivas(DateTime fecha) {
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.LluviaEfectiva);
            return ret;
        }

        /// <summary>
        /// AguaUtilOptima.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double AguaUtilOptima(DateTime fecha) { // !!! SIAR renombrar a AguaFacilmenteExtraible, también en la salida API si se usase
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                throw new Exception("No se encontraron datos del balance para esa fecha.");
            return (lin.CapacidadCampo - lin.LimiteAgotamiento);
            //return (lin.AguaFacilmenteExtraible); // !!! SIAR es más sencillo así
        }

        /// <summary>
        /// NDIasEstres.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int NDIasEstres(DateTime fecha) => LineasBalance.Count(x => (x.Fecha <= fecha) && (x.CoeficienteEstresHidrico < 1));

        /// <summary>
        /// ETcMedia3Dias.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
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
        /// AguaUtilTotal.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double AguaUtilTotal(DateTime fecha) { //!!! SIAR renombrar función a AguaDisponibleTotal; también en la salida de la API si se usase
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                throw new Exception("No se encontraron valores en el balance para la fecha indicada.");
            double ret = lin.CapacidadCampo - lin.PuntoMarchitez;
            // double ret = lin.AguaDisponibleTotal; // !!! SIAR, más sencillo así
            return ret;
        }

        /// <summary>
        /// AguaUtilTotal.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaEtc(DateTime fecha) {
            double ret = LineasBalance.Sum(x => x.Fecha > fecha ? 0 : x.EtcFinal);
            return ret;
        }

        /// <summary>
        /// RegarEnNDias.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
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
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double IndiceEstres(DateTime fecha) {
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin == null)
                throw new Exception("No se encontraron datos del balance para esa fecha.");
            return lin.IndiceEstres;
        }

        /// <summary>
        /// SumaRiegoEfectivo.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaRiegoEfectivo(DateTime fecha) {
            // Los datos de riego del balance hídrico ya han tenido en cuenta los datos Extra
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.RiegoEfectivo);
            return ret;
        }

        /// <summary>
        /// CalculaBalance. Función de uso interno en la clase para calcular el balance. Se ejecuta una única vez para añadir las líneas de balance.
        /// </summary>
        /// <param name="actualizaEtapas">The actualizaEtapas<see cref="bool"/>.</param>
        /// <param name="fechaFinalEstudio">.</param>
        private void CalculaBalance(bool actualizaEtapas, DateTime fechaFinalEstudio) {
            LineaBalance lbAnt = new LineaBalance();
            DateTime fecha = unidadCultivoDatosHidricos.FechaSiembra();
            //DateTime fechaFinalEstudio = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            int diasDesdeSiembra = 1;
            if (unidadCultivoDatosHidricos.nEtapas <= 0)
                throw new Exception("No se han definido etapas para la unidad de cultivo: " + unidadCultivoDatosHidricos.IdUnidadCultivo);
            while (fecha <= fechaFinalEstudio && fecha <= DateTime.Today) {
                LineaBalance lineaBalance = null;
                lineaBalance = CalculosHidricos.CalculaLineaBalance(unidadCultivoDatosHidricos, lbAnt, fecha);
                lineaBalance.DiasDesdeSiembra = diasDesdeSiembra++;
                LineasBalance.Add(lineaBalance);
                lbAnt = lineaBalance;
                fecha = fecha.AddDays(1);
                if (lineaBalance.NumeroEtapaDesarrollo < unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList.Count)
                    fechaFinalEstudio = fecha.AddDays(1);
                else {
                    int duracionEtapa = unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList[unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList.Count - 1].DuracionDiasEtapa;
                    fechaFinalEstudio = unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList[unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList.Count - 1].FechaInicioEtapa.AddDays(duracionEtapa);
                }
            }
            if (actualizaEtapas)
                DB.FechasEtapasSave(unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList);

            LineasBalance.RemoveAll(x => x.Fecha > DateTime.Today.AddDays(-1));

        }

        /// <summary>
        /// CosteAgua.
        /// </summary>
        /// <param name="fechaCalculo">The fechaCalculo<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double CosteAgua(DateTime fechaCalculo) { // !!! SIAR Cambiar nombre función a CosteAguaRiego, hemos introducido pequeños cambios estéticos a la función para entenderla mejor
            double precioM3 = DB.UnidadCultivoTemporadaCosteM3Agua(unidadCultivoDatosHidricos.IdUnidadCultivo, unidadCultivoDatosHidricos.IdTemporada);
            double totalMm = SumaRiegosMm(fechaCalculo);
            double superficieM2 = unidadCultivoDatosHidricos.UnidadCultivoExtensionM2 ?? 0;
            double ret = (totalMm / 1000) * precioM3 * superficieM2;
            return ret;
        }

        /// <summary>
        /// CosteDrenaje.
        /// </summary>
        /// <param name="fechaCalculo">The fechaCalculo<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double CosteDrenaje(DateTime fechaCalculo) {// !!! SIAR, renombrar a CosteAguaDrenaje; hemos introducido pequeños cambios estéticos a la función para entenderla mejor
            double? precioM3 = DB.UnidadCultivoTemporadaCosteM3Agua(unidadCultivoDatosHidricos.IdUnidadCultivo, unidadCultivoDatosHidricos.IdTemporada);
            double totalMm = SumaDrenajeMm(fechaCalculo); //!!! Ver comentario en esta función
            double superficieM2 = unidadCultivoDatosHidricos.UnidadCultivoExtensionM2 ?? 0;
            double? ret = (totalMm / 1000) * precioM3 * superficieM2;
            return ret ?? 0;
        }

        /// <summary>
        /// LineaBalance.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="LineaBalance"/>.</returns>
        private LineaBalance LineaBalance(DateTime fecha) => LineasBalance.Find(x => x.Fecha == fecha);

        /// <summary>
        /// SumaConsumoAguaCultivo.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double SumaConsumoAguaCultivo(DateTime fecha) {
            double ret = LineasBalance.Sum(x => (x.Fecha > fecha) ? 0d : x.EtcFinal);
            return ret;
        }

        /// <summary>
        /// DatosEstadoHidrico.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="DatosEstadoHidrico"/>.</returns>
        public DatosEstadoHidrico DatosEstadoHidrico(DateTime fecha) {
            if (fecha > DateTime.Today)
                fecha = DateTime.Today;
            if (fecha > unidadCultivoDatosHidricos.FechaFinalDeEstudio())
                fecha = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            LineaBalance linBalAFecha = LineasBalance.Find(x => x.Fecha == fecha);
            if (linBalAFecha == null) {
                linBalAFecha = LineasBalance.Last();
                fecha = (DateTime)linBalAFecha.Fecha;
            }
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
                Consumo = SumaEtc(fecha),
                CapacidadDeCampo = linBalAFecha.CapacidadCampo,
                PuntoDeMarchited = linBalAFecha.PuntoMarchitez,
                AguaUtilOptima = AguaUtilOptima(fecha),
                AguaPerdida = AguaPerdida(fecha),
                AguaTotalPerdidaDrenaje = AguaTotalPerdidaDrenaje(fecha),
                CosteAgua = CosteAgua(fecha),
                NDiasEstres = NDIasEstres(fecha),
                NumDiasEstresPorDrenaje = NumDiasEstresPorDrenaje(fecha),
                EstadoHidrico = IndiceEstres(fecha),
                Textura = unidadCultivoDatosHidricos.TipoSueloDescripcion,
                IndiceEstres = linBalAFecha.IndiceEstres,
                DescripcionEstres = linBalAFecha.DescripcionEstres,
                ColorEstres = linBalAFecha.ColorEstres,
                MensajeEstres = linBalAFecha.MensajeEstres,
                NumCambiosDeEtapaPendientesDeConfirmar = NumCambiosDeEtapaPendientesDeConfirmar(fecha),
                Status = "OK",
            };
            return ret;
        }

        /// <summary>
        /// The NumCambiosDeEtapaPendientesDeConfirmar.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        private int NumCambiosDeEtapaPendientesDeConfirmar(DateTime fecha) { // !!! SIAR, no entendemos muy bien la función pero creemos que lo calcula mal
            int ret = 0;
            LineaBalance lin = LineasBalance.Find(x => x.Fecha == fecha);
            if (lin != null) {
                List<UnidadCultivoCultivoEtapas> etapas = unidadCultivoDatosHidricos?.UnidadCultivoCultivoEtapasList;
                if (etapas != null) {
                    etapas.ForEach(e => {
                        if ((e.IdEtapaCultivo > lin.NumeroEtapaDesarrollo) && (e.FechaInicioEtapaConfirmada == null))
                            ret++;
                    });
                }
            }
            return ret;
        }

        /// <summary>
        /// Cuenta el número de días que se excede de la cantidad indicada en configuración como DrenajeDrespreciable.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        private int NumDiasEstresPorDrenaje(DateTime fecha) { // !!! SIAR renombrar a NDiasDrenaje 
            int ret = LineasBalance.Count(x => x.DrenajeProfundidad > 0);
            return ret;
        }

        /// <summary>
        /// ResumenDiario.
        /// </summary>
        /// <param name="fechaDeCalculo">Fecha en la que se desean presentar los datos<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="ResumenDiario"/>.</returns>
        public ResumenDiario ResumenDiario(DateTime fechaDeCalculo) {
            if (fechaDeCalculo > unidadCultivoDatosHidricos.FechaFinalDeEstudio())
                fechaDeCalculo = unidadCultivoDatosHidricos.FechaFinalDeEstudio();
            if (fechaDeCalculo > DateTime.Today)
                fechaDeCalculo = DateTime.Today;
            if (fechaDeCalculo < unidadCultivoDatosHidricos.FechaSiembra())
                fechaDeCalculo = unidadCultivoDatosHidricos.FechaSiembra();
            ResumenDiario ret = new ResumenDiario();
            LineaBalance lb = LineaBalance(fechaDeCalculo);

            ret.IdUnidadCultivo = unidadCultivoDatosHidricos.IdUnidadCultivo;
            ret.FechaDeCalculo = fechaDeCalculo;
            ret.RiegoTotal = SumaRiegosMm(fechaDeCalculo); // !!! SIAR ELIMINAR
            ret.RiegoEfectivoTotal = SumaRiegoEfectivo(fechaDeCalculo); // !!! SIAR ELIMINAR  y AÑADIR A LA SALIDA DatosHidricos
            ret.LluviaTotal = SumaLluvias(fechaDeCalculo); // !!! SIAR ELIMINAR
            ret.LluviaEfectivaTotal = SumaLluviasEfectivas(fechaDeCalculo); // !!! SIAR ELIMINAR  y AÑADIR A LA SALIDA DatosHidricos
            ret.AguaPerdida = AguaPerdida(fechaDeCalculo); // !!! SIAR ELIMINAR
            ret.ConsumoAguaCultivo = SumaConsumoAguaCultivo(fechaDeCalculo); // !!! SIAR ELIMINAR
            ret.DiasEstres = NDIasEstres(fechaDeCalculo); // !!! SIAR ELIMINAR
            ret.DeficitRiego = double.NaN; // Aún no definido // !!! SIAR ELIMINAR y AÑADIR A LA SALIDA DatosHidricos
            ret.CosteDeficitRiego = double.NaN; // Aúno no definido. // !!! SIAR ELIMINAR y AÑADIR A LA SALIDA DatosHidricos
            ret.CosteAguaRiego = CosteAgua(fechaDeCalculo); // !!! SIAR ELIMINAR
            ret.CosteAguaDrenaje = CosteDrenaje(fechaDeCalculo); // !!! SIAR ELIMINAR y AÑADIR A LA SALIDA DatosHidricos

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

            ret.AguaHastaCapacidadCampo = ret.CapacidadCampo - ret.ContenidoAguaSuelo; // esto no lo debería usar Daniel
            ret.RecomendacionRiegoNeto = lb.RecomendacionRiegoNeto;
            ret.RecomendacionRiegoBruto = lb.RecomendacionRiegoBruto; // añadido SIAR 
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

            ret.AlturaInicial = unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList[lb.NumeroEtapaDesarrollo - 1].AlturaInicial ?? 0;
            ret.AlturaFinal = unidadCultivoDatosHidricos.UnidadCultivoCultivoEtapasList[lb.NumeroEtapaDesarrollo - 1].AlturaFinal ?? 0;
            ret.Altura = lb.AlturaCultivo;
            ret.Cobertura = lb.Cobertura;

            ret.ProfRaizInicial = unidadCultivoDatosHidricos.CultivoProfRaizInicial;
            ret.ProfRaizMaxima = unidadCultivoDatosHidricos.CultivoProfRaizMax;
            ret.LongitudRaiz = lb.LongitudRaiz;

            ret.NumeroEtapaDesarrollo = lb.NumeroEtapaDesarrollo;
            ret.NombreEtapaDesarrollo = lb.NombreEtapaDesarrollo;
            return ret;
        }

        /// <summary>
        /// Retorna listado de datos hídricos filtrados por los parámetros indicados.
        /// Pasar '' como parametro en blanco si no se desea filtrar.
        /// </summary>
        /// <param name="idRegante">idCliente<see cref="int?"/>.</param>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/>.</param>
        /// <param name="idMunicipio">idMunicipio<see cref="int?"/>.</param>
        /// <param name="idCultivo">idCultivo<see cref="string"/>.</param>
        /// <param name="fechaStr">fecha.</param>
        /// <param name="roleUsuario">.</param>
        /// <param name="idUsuario">.</param>
        /// <returns><see cref="object"/>.</returns>
        public static object DatosHidricosList(int? idRegante, string idUnidadCultivo, int? idMunicipio, string idCultivo, string fechaStr, string roleUsuario, int idUsuario) {
            List<DatosEstadoHidrico> ret = new List<DatosEstadoHidrico>();
            List<string> lIdUnidadCultivo = null;
            idUnidadCultivo = idUnidadCultivo.Unquoted();
            if (idUnidadCultivo != "")
                lIdUnidadCultivo = new List<string> { idUnidadCultivo };
            else
                lIdUnidadCultivo = DB.ListaUnidadesCultivoQueCumplenFiltro(idMunicipio, idCultivo, idRegante);

            if (!DateTime.TryParse(fechaStr, out DateTime dFecha))
                dFecha = DateTime.Now.Date;

            // De todas las Unidades de Cultivo quitar las que el usuario no puede ver.
            List<string> lValidas = new List<string>();
            if (roleUsuario == "admin") {
                lValidas = lIdUnidadCultivo;
            } else if (roleUsuario == "asesor") {
                List<string> lAsesorUCList = DB.AsesorUnidadCultivoList(idUsuario);
                lValidas = lIdUnidadCultivo.Intersect(lAsesorUCList).ToList();
            } else {// usuario
                foreach (string uc in lIdUnidadCultivo) {
                    var idTemp= DB.TemporadaDeFecha(uc, dFecha);
                    if (DB.LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(uc, idUsuario, idTemp))
                        lValidas.Add(uc);
                }
            }

            DatosEstadoHidrico datosEstadoHidrico = null;
            UnidadCultivoDatosHidricos dh = null;
            BalanceHidrico bh = null;
            List<GeoLocParcela> lGeoLocParcelas = null;
            string idTemporada="";
            foreach (string idUc in lValidas) {
                try {
                    lGeoLocParcelas = null;
                    idTemporada = DB.TemporadaDeFecha(idUc, dFecha);
                    if (idTemporada != null) {
                        lGeoLocParcelas = DB.GeoLocParcelasList(idUc, idTemporada);
                        bh = BalanceHidrico.Balance(idUc, dFecha);
                        datosEstadoHidrico = bh.DatosEstadoHidrico(dFecha);
                        datosEstadoHidrico.GeoLocJson = Newtonsoft.Json.JsonConvert.SerializeObject(lGeoLocParcelas);
                        datosEstadoHidrico.HidranteTomaJson = DB.HidrantesListJson(idUc, idTemporada);
                        ret.Add(datosEstadoHidrico);
                    }
                } catch (Exception ex) {
                    dh = bh.unidadCultivoDatosHidricos;
                    dh.ObtenerMunicicioParaje(out string provincias, out string municipios, out string parajes);                    
                    datosEstadoHidrico = new DatosEstadoHidrico {
                        Fecha = dFecha,
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
                        HidranteTomaJson = DB.HidrantesListJson(idUc, idTemporada),
                        Status = "ERROR:" + ex.Message
                    };
                    ret.Add(datosEstadoHidrico);
                }
            }
            return ret;
        }

    }

}
