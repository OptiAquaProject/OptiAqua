namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Define <see cref="UnidadCultivoDatosHidricos" />
    /// UnidadCultivoDatosHidricos es una clase que almacena y proporciona la mayoria de los datos necesarios para el calculo del balance hídrico de una unidad de cultivo en una temporada.
    /// Encapsula los datos necesarios para los cálculos del balance hídrico.
    /// </summary>
    public class UnidadCultivoDatosHidricos {
        /// <summary>
        /// Defines the lDatosClimaticos.
        /// </summary>
        private List<DatoClimatico> lDatosClimaticos;

        /// <summary>
        /// Defines the lTiposEstres.
        /// </summary>
        private readonly Dictionary<string, TipoEstres> lTiposEstres;

        /// <summary>
        /// Defines the lTipoEstresUmbralList.
        /// </summary>
        private readonly Dictionary<string, List<TipoEstresUmbral>> lTipoEstresUmbralList;

        /// <summary>
        /// Defines the unidadCultivo.
        /// </summary>
        private UnidadCultivo unidadCultivo;

        /// <summary>
        /// Defines the pUnidadCultivoExtensionM2.
        /// </summary>
        private readonly double pUnidadCultivoExtensionM2;

        /// <summary>
        /// Defines the cultivo.
        /// </summary>
        private Cultivo cultivo;

        /// <summary>
        /// Defines the estacion.
        /// </summary>
        private Estacion estacion;

        /// <summary>
        /// Defines the temporada.
        /// </summary>
        private Temporada temporada;

        /// <summary>
        /// Defines the riegoTipo.
        /// </summary>
        private RiegoTipo riegoTipo;

        /// <summary>
        /// Defines the regante.
        /// </summary>
        private Regante regante;

        /// <summary>
        /// Define pUnidadCultivoCultivo.
        /// </summary>
        private UnidadCultivoCultivo unidadCultivoCultivo;

        /// <summary>
        /// Define pUnidadCultivoDatosExtas.
        /// </summary>
        private List<UnidadCultivoDatosExtra> lUnidadCultivoDatosExtas;

        /// <summary>
        /// Define pDatosRiego.
        /// </summary>
        private readonly List<Riego> lDatosRiego;

        /// <summary>
        /// Gets the EficienciaRiego.
        /// </summary>
        public double EficienciaRiego => riegoTipo.Eficiencia;

        /// <summary>
        /// Gets the Alias.
        /// </summary>
        public string Alias => unidadCultivo.Alias;

        /// <summary>
        /// Gets the IdCultivo.
        /// </summary>
        public int? IdCultivo => cultivo.IdCultivo;

        /// <summary>
        /// Gets the UnidadCultivoExtensionM2.
        /// </summary>
        public double? UnidadCultivoExtensionM2 => DB.UnidadCultivoExtensionM2(unidadCultivo.IdUnidadCultivo, temporada.IdTemporada);

        /// <summary>
        /// Gets the IdTemporada.
        /// </summary>
        public string IdTemporada => temporada.IdTemporada;

        /// <summary>
        /// Gets the IdTipoRiego.
        /// </summary>
        public int? IdTipoRiego => riegoTipo.IdTipoRiego;

        /// <summary>
        /// Gets the NParcelas.
        /// </summary>
        public int? NParcelas => DB.NParcelas(unidadCultivo.IdUnidadCultivo, temporada.IdTemporada);

        /// <summary>
        /// Gets the ReganteNombre.
        /// </summary>
        public string ReganteNombre => regante.Nombre;

        /// <summary>
        /// Gets the ReganteNif.
        /// </summary>
        public string ReganteNif => regante.NIF;

        /// <summary>
        /// Gets the ReganteTelefono.
        /// </summary>
        public string ReganteTelefono => regante.Telefono;

        /// <summary>
        /// Gets the ReganteTelefonoSMS.
        /// </summary>
        public string ReganteTelefonoSMS => regante.TelefonoSMS;

        /// <summary>
        /// Gets the Pluviometria.
        /// </summary>
        public double Pluviometria => unidadCultivoCultivo.Pluviometria;

        /// <summary>
        /// Gets the TipoRiego.
        /// </summary>
        public string TipoRiego => riegoTipo.TipoRiego;

        /// <summary>
        /// Gets the UnidadCultivoCultivoEtapasList.
        /// </summary>
        public List<UnidadCultivoCultivoEtapas> UnidadCultivoCultivoEtapasList { get; private set; }

        /// <summary>
        /// Retorna primera fecha de las etapas como fecha de siembra.
        /// En caso de que no existandatos retorna fecha actual.
        /// </summary>
        /// <returns>.</returns>
        public DateTime FechaSiembra() => (DateTime)unidadCultivoCultivo.FechaSiembra();

        /// <summary>
        /// Gets the CultivoNombre.
        /// </summary>
        public string CultivoNombre => cultivo.Nombre;

        /// <summary>
        /// Gets the IdUnidadCultivo.
        /// </summary>
        public string IdUnidadCultivo => unidadCultivo.IdUnidadCultivo;

        /// <summary>
        /// Gets the EstacionNombre.
        /// </summary>
        public string EstacionNombre => estacion.Nombre;

        /// <summary>
        /// Gets the IdRegante.
        /// </summary>
        public int? IdRegante => unidadCultivoCultivo.IdRegante;

        /// <summary>
        /// Gets the IdEstacion.
        /// </summary>
        public int IdEstacion => estacion.IdEstacion;

        /// <summary>
        /// Gets the TipoSueloDescripcion.
        /// </summary>
        public string TipoSueloDescripcion => unidadCultivo.TipoSueloDescripcion;

        /// <summary>
        /// Gets the nEtapas.
        /// </summary>
        public int nEtapas => UnidadCultivoCultivoEtapasList.Count;

        /// <summary>
        /// Gets the CultivoTBase.
        /// </summary>
        public double CultivoTBase => cultivo.TBase;

        /// <summary>
        /// Gets the CultivoIntegralEmergencia.
        /// </summary>
        public double CultivoIntegralEmergencia => cultivo.IntegralEmergencia;

        /// <summary>
        /// Gets the CultivoModCobCoefA.
        /// </summary>
        public double CultivoModCobCoefA => cultivo.ModCobCoefA;

        /// <summary>
        /// Gets the CultivoModCobCoefB.
        /// </summary>
        public double CultivoModCobCoefB => cultivo.ModCobCoefB;

        /// <summary>
        /// Gets the CultivoModCobCoefC.
        /// </summary>
        public double? CultivoModCobCoefC => cultivo.ModCobCoefC;

        /// <summary>
        /// Gets the CultivoAlturaFinal.
        /// </summary>
        public double? CultivoAlturaFinal => cultivo.AlturaFinal;

        /// <summary>
        /// Gets the CultivoAlturaInicial.
        /// </summary>
        public double? CultivoAlturaInicial => cultivo.AlturaInicial;

        /// <summary>
        /// Gets the CultivoModAltCoefA.
        /// </summary>
        public double CultivoModAltCoefA => cultivo.ModAltCoefA;

        /// <summary>
        /// Gets the CultivoModAltCoefB.
        /// </summary>
        public double CultivoModAltCoefB => cultivo.ModAltCoefB;

        /// <summary>
        /// Gets the CultivoModAltCoefC.
        /// </summary>
        public double? CultivoModAltCoefC => cultivo.ModAltCoefC;

        /// <summary>
        /// Gets the CultivoProfRaizInicial.
        /// </summary>
        public double CultivoProfRaizInicial => cultivo.ProfRaizInicial;

        /// <summary>
        /// Gets the CultivoProfRaizMax.
        /// </summary>
        public double CultivoProfRaizMax => cultivo.ProfRaizMax;

        /// <summary>
        /// Gets the CultivoModRaizCoefA.
        /// </summary>
        public double CultivoModRaizCoefA => cultivo.ModRaizCoefA;

        /// <summary>
        /// Gets the CultivoModRaizCoefB.
        /// </summary>
        public double CultivoModRaizCoefB => cultivo.ModRaizCoefB;

        /// <summary>
        /// Gets the CultivoModRaizCoefC.
        /// </summary>
        public double? CultivoModRaizCoefC => cultivo.ModRaizCoefC;

        /// <summary>
        /// Gets the ListaUcSuelo.
        /// </summary>
        public List<UnidadCultivoSuelo> ListaUcSuelo { get; private set; }

        /// <summary>
        /// Gets the EtapaInicioRiego.
        /// </summary>
        public int EtapaInicioRiego => cultivo.EtapaInicioRiego;

        // Devuelve los límites inferior y superior para la etapa indicada.
        // de la etapa se optiene el tipo de estres (con idUmbralInferior Riego y idUmbralSuperiorRiego)
        // Los valores de los límite inferior y superior se obtienen de la lista de umbrales. Buscando por identificador.
        /// <summary>
        /// The ClaseEstresUmbralInferiorYSuperior.
        /// </summary>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <param name="limiteInferior">The limiteInferior<see cref="double"/>.</param>
        /// <param name="limiteSuperior">The limiteSuperior<see cref="double"/>.</param>
        public void ClaseEstresUmbralInferiorYSuperior(int nEtapa, out double limiteInferior, out double limiteSuperior) {
            int nEtapaBase0 = nEtapa - 1 > 0 ? nEtapa - 1 : 0;
            string idTipoEstres = UnidadCultivoCultivoEtapasList[nEtapaBase0].IdTipoEstres;
            if (idTipoEstres == null)
                throw new Exception("No se ha definido tipo de estrés");
            TipoEstres estres = lTiposEstres[idTipoEstres];
            int? idInferior = estres.IdUmbralInferiorRiego;
            int? idSuperior = estres.IdUmbralSuperiorRiego;
            List<TipoEstresUmbral> lUmbrales = lTipoEstresUmbralList[idTipoEstres];
            lUmbrales = lTipoEstresUmbralList[idTipoEstres];

            if (idInferior == null)
                throw new Exception("No se ha definido IdUmbraInferior para el tipo de estrés: " + idTipoEstres);
            if (idSuperior == null)
                throw new Exception("No se ha definido IdUmbraSuperior para el tipo de estrés: " + idTipoEstres);
            double? valInferior = lUmbrales.Find(x => x.IdUmbral == idInferior)?.UmbralMaximo;
            double? valSuperior = lUmbrales.Find(x => x.IdUmbral == idSuperior)?.UmbralMaximo;
            if (valInferior == null || valSuperior == null) {
                throw new Exception("Umbrales para el tipo de estes mal definidos: " + idTipoEstres);
            }
            limiteInferior = (double)valInferior;
            limiteSuperior = (double)valSuperior;
        }

        /// <summary>
        /// Retorna a la tabla de umbrales la clase de estrés.
        /// Ordena la tabla por el valor umbral.
        /// Retonar la descripción del maxímo umbral que puede superar el indiceEstres.
        /// </summary>
        /// <param name="idTipoEstres">idTipoEstres<see cref="string"/>.</param>
        /// <param name="indiceEstres">ie<see cref="double"/>.</param>
        /// <returns><see cref="string"/>.</returns>
        public TipoEstresUmbral TipoEstresUmbral(string idTipoEstres, double indiceEstres) {
            TipoEstresUmbral ret = null;
            List<TipoEstresUmbral> ltu = lTipoEstresUmbralList[idTipoEstres];
            if (ltu?.Count == 0)
                return ret;
            ret = ltu[0];
            int i = 0;
            while (indiceEstres > ltu[i].UmbralMaximo && (i + 1 < ltu.Count)) {
                ret = ltu[++i];
            }
            return ret;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnidadCultivoDatosHidricos"/> class.
        /// </summary>
        /// <param name="idUnidadCultivo">The IdUnidadCultivo<see cref="string"/>.</param>
        /// <param name="fecha">The idTemporada<see cref="string"/>.</param>
        public UnidadCultivoDatosHidricos(string idUnidadCultivo, DateTime fecha) {

            string idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, fecha);
            if ((temporada = DB.Temporada(idTemporada)) == null)
                throw new Exception($"Imposible cargar datos de la temporada {idTemporada}.");

            if ((unidadCultivo = DB.UnidadCultivo(idUnidadCultivo)) == null)
                throw new Exception($"Imposible cargar datos del cultivo {idUnidadCultivo}.");

            pUnidadCultivoExtensionM2 = DB.UnidadCultivoExtensionM2(idUnidadCultivo, idTemporada);

            if ((UnidadCultivoCultivoEtapasList = DB.UnidadCultivoCultivoEtapasList(idUnidadCultivo, idTemporada)).Count == 0)
                throw new Exception($"Imposible cargar las etapas para la unidad de cultivo {idUnidadCultivo}.");

            if ((unidadCultivoCultivo = DB.UnidadCultivoCultivo(idUnidadCultivo, idTemporada)) == null)
                throw new Exception("Imposible datos del cultivo en la temporada indicada.");

            lTiposEstres = DB.ListaTipoEstres();
            lTipoEstresUmbralList = DB.ListaEstresUmbral();

            if ((lDatosClimaticos = DB.DatosClimaticosList(FechaSiembra(), FechaFinalDeEstudio(), unidadCultivo.IdEstacion)) == null)
                throw new Exception($"Imposible cargar datos climáticos para la estación {unidadCultivo.IdEstacion}  en el intervalo de fechas de {FechaSiembra()} a {FechaFinalDeEstudio()}");

            cultivo = DB.Cultivo(unidadCultivoCultivo.IdCultivo);
            estacion = DB.Estacion(unidadCultivo.IdEstacion);
            regante = (Regante)DB.Regante(unidadCultivoCultivo.IdRegante);

            if ((ListaUcSuelo = DB.UnidadCultivoSueloList(idTemporada, idUnidadCultivo)) == null)
                throw new Exception("No se ha definido suelo para la unidad de Cultivo:" + idUnidadCultivo);

            riegoTipo = DB.RiegoTipo(unidadCultivoCultivo.IdTipoRiego);

            DateTime fechaSiembra = FechaSiembra();
            DateTime fechaFinal = FechaFinalDeEstudio(); ;
            lDatosRiego = DB.RiegosList(idUnidadCultivo, fechaSiembra, fechaFinal);

            lUnidadCultivoDatosExtas = DB.ParcelasDatosExtrasList(idUnidadCultivo, fechaSiembra, fechaFinal);
        }

        /// <summary>
        /// Retorna los datos extra a una fecha.  0 Si no se dispone de datos en esa fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public UnidadCultivoDatosExtra DatoExtra(DateTime fecha) => lUnidadCultivoDatosExtas.Find(d => d.Fecha == fecha);

        /// <summary>
        /// ObtenerMunicicioParaje.
        /// </summary>
        /// <param name="provincias">.</param>
        /// <param name="municipios">The municipios<see cref="string"/>.</param>
        /// <param name="parajes">The parajes<see cref="string"/>.</param>
        public void ObtenerMunicicioParaje(out string provincias, out string municipios, out string parajes) => DB.ObtenerMunicicioParaje(temporada.IdTemporada, unidadCultivo.IdUnidadCultivo, out provincias, out municipios, out parajes);

        /// <summary>
        /// Retonar el cálculo de la fecha de fin de estudio.
        /// Si se excede de fecha del día retornar la fecha del día.
        /// Si no hay datos suficientes para el cálculo retorna fecha del día.
        /// </summary>
        /// <returns>Fecha din del estudio.</returns>
        public DateTime FechaFinalDeEstudio() {
            DateTime ret = temporada.FechaFinal;

            int duracionDias = 0;
            if (UnidadCultivoCultivoEtapasList != null && UnidadCultivoCultivoEtapasList.Count > 0)
                duracionDias = UnidadCultivoCultivoEtapasList[UnidadCultivoCultivoEtapasList.Count - 1].DuracionDiasEtapa;

            if (UnidadCultivoCultivoEtapasList == null)
                return ret;
            if (UnidadCultivoCultivoEtapasList.Count == 0)
                return ret;
            DateTime fechaInicioUltimaEtapa = UnidadCultivoCultivoEtapasList[UnidadCultivoCultivoEtapasList.Count - 1].FechaInicioEtapa;

            if (UnidadCultivoCultivoEtapasList[UnidadCultivoCultivoEtapasList.Count - 1].FechaInicioEtapaConfirmada != null)
                fechaInicioUltimaEtapa = (DateTime)UnidadCultivoCultivoEtapasList[UnidadCultivoCultivoEtapasList.Count - 1].FechaInicioEtapaConfirmada;
            fechaInicioUltimaEtapa = fechaInicioUltimaEtapa.AddDays(duracionDias);
            ret = fechaInicioUltimaEtapa;
            if (ret > DateTime.Today) {
                ret = DateTime.Today;
            }
            return ret;
        }

        /// <summary>
        /// Retorna los datos de riego a una fecha. 0 Si no se dispone de datos en esa fecha.
        /// Primero consulta en la tabla datos extra.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        private double RiegoM3(DateTime fecha) {
            UnidadCultivoDatosExtra extra = DatoExtra(fecha);
            if (extra?.RiegoM3 != null) {
                return extra.RiegoM3 ?? 0;
            } else
                return (lDatosRiego?.Find(x => x.Fecha == fecha)?.RiegoM3) ?? 0;
        }

        /// <summary>
        /// RiegoMm.
        /// </summary>
        /// <param name="fecha">The fecha<see cref="DateTime"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double RiegoMm(DateTime fecha) {
            double r3 = RiegoM3(fecha);
            return (r3 * 1000 / pUnidadCultivoExtensionM2);
        }

        /// <summary>
        /// Retorna velocidad del viento a una Fecha.  0 Si no se dispone de datos en esa fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double VelocidadViento(DateTime fecha) {
            var velocidad=(lDatosClimaticos.Find(d => d.Fecha == fecha)?.VelViento) ;// velocidad del viento
            if (velocidad == null) {
                var aux1 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-1))?.TempMedia;
                var aux2 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-2))?.TempMedia;
                var aux3 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-3))?.TempMedia;
                var suma = (aux1 ?? 0) + (aux2 ?? 0) + (aux3 ?? 0);
                var nItemSumados = (aux1 == null ? 0 : 1) + (aux2 == null ? 0 : 1) + (aux3 == null ? 0 : 1);
                if (nItemSumados == 0)
                    velocidad = 0;
                else
                    velocidad = suma / (double)nItemSumados;
            }
            return (double)velocidad;
        }

        /// <summary>
        /// Retorna Humedad media a una fecha. 0 Si no se dispone de datos en esa fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double HumedadMedia(DateTime fecha) {
            double? humedad = lDatosClimaticos.Find(d => d.Fecha == fecha)?.HumedadMedia;
            if (humedad == null) {
                var aux1 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-1))?.TempMedia;
                var aux2 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-2))?.TempMedia;
                var aux3 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-3))?.TempMedia;
                var suma = (aux1 ?? 0) + (aux2 ?? 0) + (aux3 ?? 0);
                var nItemSumados = (aux1 == null ? 0 : 1) + (aux2 == null ? 0 : 1) + (aux3 == null ? 0 : 1);
                if (nItemSumados == 0)
                    humedad = 0;
                else
                    humedad = suma / (double)nItemSumados;
            }
            return (double)humedad;
        }

        /// <summary>
        /// Retorna la temperatura a una fecha.  0 Si no se dispone de datos en esa fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double Temperatura(DateTime fecha) {
            double? temp = lDatosClimaticos.Find(d => d.Fecha == fecha)?.TempMedia;
            if (temp == null) {
                var temp1= lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-1))?.TempMedia;
                var temp2 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-2))?.TempMedia;
                var temp3 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-3))?.TempMedia;
                var suma = (temp1 ?? 0) + (temp2 ?? 0) + (temp3 ?? 0);
                var nItemSumados = (temp1 == null ? 0 : 1)+ (temp2 == null ? 0 : 1)+ (temp3 == null ? 0 : 1);
                if (nItemSumados == 0)
                    temp = 0;
                else
                    temp = suma / (double)nItemSumados;
            }
            return (double)temp;
        }

        /// <summary>
        /// Retorna los mm de lluvía a una fecha.  0 Si no se dispone de datos en esa fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double LluviaMm(DateTime fecha) {
            UnidadCultivoDatosExtra extra = DatoExtra(fecha);
            if (extra?.LluviaMm != null)
                return extra.LluviaMm ?? 0;
            else
                return (lDatosClimaticos.Find(d => d.Fecha == fecha)?.Precipitacion) ?? 0;
        }

        /// <summary>
        /// Retorna el valor de Evotranspieración a una fecha.  0 Si no se dispone de datos en esa fecha.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <returns>.</returns>
        public double Eto(DateTime fecha) {
            var eto= lDatosClimaticos.Find(d => d.Fecha == fecha)?.Eto ;
            if (eto == null) {
                var aux1 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-1))?.TempMedia;
                var aux2 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-2))?.TempMedia;
                var aux3 = lDatosClimaticos.Find(d => d.Fecha == fecha.AddDays(-3))?.TempMedia;
                var suma = (aux1 ?? 0) + (aux2 ?? 0) + (aux3 ?? 0);
                var nItemSumados = (aux1 == null ? 0 : 1) + (aux2 == null ? 0 : 1) + (aux3 == null ? 0 : 1);
                if (nItemSumados == 0)
                    eto = 0;
                else
                    eto = suma / (double)nItemSumados;
            }
            return (double)eto;

        }

        /// <summary>
        /// Retorna la definición de la clase de estrés.
        /// </summary>
        /// <param name="indiceEstres">The ie<see cref="double"/>.</param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public TipoEstresUmbral TipoEstresUmbral(double indiceEstres, int nEtapa) {
            int nEtapaBase0 = nEtapa - 1;
            return TipoEstresUmbral(UnidadCultivoCultivoEtapasList[nEtapaBase0].IdTipoEstres, indiceEstres);
        }

        /// <summary>
        /// The CultivoEtapasList_Ndias1y2.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int CultivoEtapasList_Ndias1y2() {
            if (UnidadCultivoCultivoEtapasList == null)
                return 0;
            int ret = UnidadCultivoCultivoEtapasList.Count > 1 ? UnidadCultivoCultivoEtapasList[0].DuracionDiasEtapa : 0;
            ret += UnidadCultivoCultivoEtapasList.Count > 2 ? UnidadCultivoCultivoEtapasList[1].DuracionDiasEtapa : 0;
            return ret;
        }
    }
}
