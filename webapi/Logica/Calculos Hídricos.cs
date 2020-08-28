namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase estática en la que se implementan las funciones hídricas.
    /// Todas las variables necesarias se pasan como parámetros de las funciones.
    /// No accede a Base de datos.
    /// </summary>
    public static class CalculosHidricos {
        /// <summary>
        /// Calculo del punto de marchitez
        /// Segun formula Saxton-Rawls (2006)
        /// Abreviaturas
        /// suelo.Arena/Arcilla/ElementosGruesos:     % arena, % arcilla, % Elementos Gruesos (%w)
        /// mo100:   materia Orgánica, (%w)
        /// O1500t:  humedad a 1500 kPa, primera solución (%v)
        /// O1500:   humedad a 1500 kPa, (%v)
        /// PAW1500: cantidad de agua disponible a 1500 kPa.
        /// </summary>
        /// <param name="suelo">The suelo<see cref="UnidadCultivoSuelo"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double PuntoDeMarchitez(this UnidadCultivoSuelo suelo) {
            double mo100 = suelo.MateriaOrganica * 100; //Nota: esta conversión es porque en BBDD no se apunta %M.O. como valor porcentual
            double o1500t = -0.024 * suelo.Arena + 0.487 * suelo.Arcilla + 0.006 * mo100 + 0.005 * (suelo.Arena * mo100) - 0.013 * (suelo.Arcilla * mo100) + 0.068 * (suelo.Arena * suelo.Arcilla) + 0.031;
            double o1500 = o1500t + (0.14 * o1500t - 0.02);
            double paw1500 = o1500 * (1 - suelo.ElementosGruesos);
            return paw1500;
        }

        /// <summary>
        /// Calcula de la capacidad de campo
        /// Abreviaturas
        /// suelo.Arena/Arcilla/ElementosGruesos:     % arena, % arcilla, % Elementos Gruesos (%w)
        /// mo100:   materia Orgánica, (%w)
        /// O33t:    humedad a 33 kPa, primera solución (%v)
        /// O33:     humedad 33 kPa, densidad normal (%v)
        /// PAW33:   cantidad de agua disponible a 33 kPa.
        /// </summary>
        /// <param name="suelo">The suelo<see cref="UnidadCultivoSuelo"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double CapacidadCampo(this UnidadCultivoSuelo suelo) {
            double mo100 = suelo.MateriaOrganica * 100; //Nota: esta conversión es porque en BBDD no se apunta %M.O. como valor porcentual
            double o33t = -0.251 * suelo.Arena + 0.195 * suelo.Arcilla + 0.011 * mo100 + 0.006 * (suelo.Arena * mo100) - 0.027 * (suelo.Arcilla * mo100) + 0.452 * (suelo.Arena * suelo.Arcilla) + 0.299;
            double o33 = o33t + (1.283 * (o33t * o33t) - 0.374 * (o33t) - 0.015);
            double paw33 = o33 * (1 - suelo.ElementosGruesos);
            return paw33;
        }

        /// <summary>
        /// The TasaCrecimientoCobertura.
        /// </summary>
        /// <param name="it">The it<see cref="double"/>.</param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <param name="itEmergencia">The itEmergencia<see cref="double"/>.</param>
        /// <param name="ModCobCoefA">The ModCobCoefA<see cref="double"/>.</param>
        /// <param name="ModCobCoefB">The ModCobCoefB<see cref="double"/>.</param>
        /// <param name="ModCobCoefC">The ModCobCoefC<see cref="double?"/>.</param>
        /// <param name="definicionEtapaPorDias">The definicionEtapaPorDias<see cref="bool"/>.</param>
        /// <param name="nDiasduracionEtapaDias">The nDiasduracionEtapaDias<see cref="int"/>.</param>
        /// <param name="coberturaInicial">The coberturaInicial<see cref="double?"/>.</param>
        /// <param name="coberturaFinal">The coberturaFinal<see cref="double?"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double TasaCrecimientoCobertura(double it, int nEtapa, double itEmergencia, double ModCobCoefA, double ModCobCoefB, double? ModCobCoefC, bool definicionEtapaPorDias, int nDiasduracionEtapaDias, double? coberturaInicial, double? coberturaFinal) {
            // !!! SIAR se añade un calculo alternativo del coeficiente cuando DefinicionPorDias = TRUE

            // ((CobCoefA) * CobCoefB * EXP(-CobCoefB * (C10 - CobCoefC)))/POTENCIA((1+EXP(-CobCoefB*(C10-CobCoefC)));2)
            //     1.- La función necesita conocer el número de Etapa y la it de emergencia de la BBDD
            //     2.- Si la it es menor que la de emergencia la planta no ha brotado y no hay cobertura
            //     3.- cuando la planta brota, se calcula la cobertura
            //     4.- se cambia la variable itomprueba si el coeficiente C existe y se aplica fórmula logarítmica o lineal
            double ret;
            if (nEtapa == 2)
                nEtapa = 2;
            if (definicionEtapaPorDias) {  // !!! propuestaSIAR && ModCobCoefB < 0) o && ModCobCoefB != -9999) { 
                if (coberturaFinal != null && coberturaInicial != null) {
                    ret = ((double)coberturaFinal - (double)coberturaInicial) / (double) nDiasduracionEtapaDias;
                } else {
                    ret = 0;
                }
            } else {
                if (nEtapa == 1 && it < itEmergencia) {
                    ret = 0;
                } else {
                    it = it - itEmergencia;
                    if (ModCobCoefC != null && ModCobCoefC != 0) {
                        ret = ModCobCoefA * ModCobCoefB * Math.Exp(-ModCobCoefB * (it - (double)ModCobCoefC)) / Math.Pow((1 + Math.Exp(-ModCobCoefB * (it - (double)ModCobCoefC))), 2);
                    } else {
                        ret = ModCobCoefB;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// The TasaCrecimientoAltura.
        /// </summary>
        /// <param name="it">The it<see cref="double"/>.</param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <param name="itEmergencia">The itEmergencia<see cref="double"/>.</param>
        /// <param name="ModAltCoefA">The ModAltCoefA<see cref="double"/>.</param>
        /// <param name="ModAltCoefB">The ModAltCoefB<see cref="double"/>.</param>
        /// <param name="ModAltCoefC">The ModAltCoefC<see cref="double?"/>.</param>
        /// <param name="definicionEtapaPorDias">The definicionEtapaPorDias<see cref="bool"/>.</param>
        /// <param name="NDiasEtapas1y2">The NDiasEtapas1y2<see cref="int"/>.</param>
        /// <param name="alturaInicial">The alturaInicial<see cref="double?"/>.</param>
        /// <param name="alturaFinal">The alturaFinal<see cref="double?"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double TasaCrecimientoAltura(double it, int nEtapa, double itEmergencia, double ModAltCoefA, double ModAltCoefB, double? ModAltCoefC,
            bool definicionEtapaPorDias, int NDiasEtapas1y2, double? alturaInicial, double? alturaFinal) {
            // ((CobCoefA) * CobCoefB * EXP(-CobCoefB * (C10 - CobCoefC)))/POTENCIA((1+EXP(-CobCoefB*(C10-CobCoefC)));2)
            //     1.- La función necesita conocer el número de Etapa y la it de emergencia de la BBDD
            //     2.- Si la it es menor que la de emergencia la planta no ha brotado y no hay cobertura
            //     3.- cuando la planta brota, se calcula la cobertura
            //     4.- se cambia la variable it por it (más correcto conceptualmente)
            //     5.- se añade la rutina que comprueba si el coeficiente C existe y se aplica fórmula logarítmica o lineal
            double ret;

            if (definicionEtapaPorDias) { // propuestaSIAR if(ModAltCoefB < 0)
                if (nEtapa < 3) { 
                    // Obtener de la tabla cultivoEtapas para la UC y la Etapa correspondiente:
                    // duracion de las etapas 1 y 2                    
                    // alturaInicial y alturaFinal
                    if (alturaInicial == null || alturaFinal == null)
                        ret = 0;
                    else
                        ret = ((double)alturaFinal - (double)alturaFinal) / NDiasEtapas1y2;
                } else {
                    ret = 0;
                }
            } else {
                if (nEtapa == 1 && it < itEmergencia) {
                    ret = 0;
                } else {
                    it = it - itEmergencia;
                    if (ModAltCoefC != null && ModAltCoefC != 0) {
                        ret = ModAltCoefA * ModAltCoefB * Math.Exp(-ModAltCoefB * (it - (double)ModAltCoefC)) / Math.Pow((1 + Math.Exp(-ModAltCoefB * (it - (double)ModAltCoefC))), 2);
                    } else {
                        ret = ModAltCoefB;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Retorna en Nº de Etapa en la que nos encontramos en base 1 (1,2,3,4....)
        /// Párametro nEtapa está en base 1 y la tabla de etapas en base 0.
        /// </summary>
        /// <param name="fecha">.</param>
        /// <param name="cobertura">.</param>
        /// <param name="nEtapaActual">.</param>
        /// <param name="unidadCultivoCultivosEtapas">.</param>
        /// <param name="pCultivoEtapas">.</param>
        /// <returns>.</returns>
        public static int NumeroEtapaDesarrollo(DateTime fecha, double cobertura, int nEtapaActual, List<UnidadCultivoCultivoEtapas> unidadCultivoCultivosEtapas, List<UnidadCultivoCultivoEtapas> pCultivoEtapas) {
            int nEtapaBase0 = nEtapaActual - 1 > 0 ? nEtapaActual - 1 : 0; // situación anómala
            int ret = nEtapaActual;
            if (unidadCultivoCultivosEtapas.Count < nEtapaActual)
                return unidadCultivoCultivosEtapas.Count; // situación anómala

            if (unidadCultivoCultivosEtapas[nEtapaBase0].FechaInicioEtapaConfirmada != null) {
                unidadCultivoCultivosEtapas[nEtapaBase0].FechaInicioEtapa = (DateTime)unidadCultivoCultivosEtapas[nEtapaBase0].FechaInicioEtapaConfirmada;
            }

            // !!! SIAR Ahora se calcula una cobertura siempre, usando fórmula o estimándola
            // a partir de la duración teórica de la fase
            // por lo tanto habría que tener en cuenta que en las dos primeras fases el paso a la siguiente
            // fase sea por cobertura, no por la duración del ciclo
            // Añado a la expresión de abajo la condición: && nEtapaActual >= 2
            // PERO SERÍA MÁS ELEGANTE QUE LA COMPROBACION FUERA QUE SI COBERTURA DE LAS FASES = NULL
            // ENTONCES SE HACE UN CAMBIO DE FASE POR DÍAS Y SI NO ES ASÍ SE HARÍA POR VALORES DE COBERTURA
            if (unidadCultivoCultivosEtapas[nEtapaBase0].DefinicionPorDias == true && nEtapaActual >= 2) {
                if (nEtapaBase0 + 1 < unidadCultivoCultivosEtapas.Count) {
                    DateTime fechaInicioSiguienteEtapa = unidadCultivoCultivosEtapas[nEtapaBase0].FechaInicioEtapa.AddDays(pCultivoEtapas[nEtapaBase0].DuracionDiasEtapa);
                    if (fecha >= fechaInicioSiguienteEtapa) {
                        //actualizar fecha inicio siguiente etapa
                        unidadCultivoCultivosEtapas[nEtapaBase0 + 1].FechaInicioEtapa = fechaInicioSiguienteEtapa;
                        ret++;
                    }
                }
            } else { // definido por integral termica !!! SIAR o bien estimada a partir de las coberturas teóricas
                if (unidadCultivoCultivosEtapas[nEtapaBase0].CobFinal < cobertura) {
                    if (unidadCultivoCultivosEtapas.Count > nEtapaBase0 + 1)
                        //actulizar siguiente fecha de inicio siguiente etapa
                        unidadCultivoCultivosEtapas[nEtapaBase0 + 1].FechaInicioEtapa = fecha;
                    ret++;
                }
            }
            return ret > unidadCultivoCultivosEtapas.Count ? unidadCultivoCultivosEtapas.Count : ret;
        }

        /// <summary>
        /// Calculo del Coeficiento de cultivo.
        /// </summary>
        /// <param name="nEtapa">.</param>
        /// <param name="fecha">.</param>
        /// <param name="cob">.</param>
        /// <param name="unidadCultivoCultivosEtapasList">.</param>
        /// <param name="cultivoEtapasList">.</param>
        /// <returns>.</returns>
        public static double Kc(int nEtapa, DateTime fecha, double cob, List<UnidadCultivoCultivoEtapas> unidadCultivoCultivosEtapasList, List<UnidadCultivoCultivoEtapas> cultivoEtapasList) {
            double ret = 0;
            int nEtapaIndex = nEtapa - 1 > 0 ? nEtapa - 1 : 0; // la etapa está en base 1
            if (unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial == unidadCultivoCultivosEtapasList[nEtapaIndex].KcFinal) {
                ret = unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial;
            } else {
                if (unidadCultivoCultivosEtapasList[nEtapaIndex].DefinicionPorDias == true) {
                    DateTime fechaInicioEtapaActual = unidadCultivoCultivosEtapasList[nEtapaIndex].FechaInicioEtapa;
                    if (unidadCultivoCultivosEtapasList[nEtapaIndex].FechaInicioEtapaConfirmada != null)
                        fechaInicioEtapaActual = (DateTime)unidadCultivoCultivosEtapasList[nEtapaIndex].FechaInicioEtapaConfirmada;
                    int nDias = (fecha - fechaInicioEtapaActual).Days;
                    if (nDias < 0)
                        nDias = 0;
                    int diasTeoricosFase = cultivoEtapasList[nEtapaIndex].DuracionDiasEtapa;
                    DateTime fechaFinEtapaActual = fechaInicioEtapaActual.AddDays(diasTeoricosFase);
                    if (nDias > diasTeoricosFase)
                        nDias = diasTeoricosFase;
                    double kcInicial = unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial;
                    double kcFinal = unidadCultivoCultivosEtapasList[nEtapaIndex].KcFinal;
                    ret = kcInicial + ((kcFinal - kcInicial) * (nDias / (double)diasTeoricosFase));
                } else { // por integral termica
                    double kcIni = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial);
                    double cobIni = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].CobInicial);
                    double kcFin = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].KcFinal);
                    double cobFin = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].CobFinal);
                    ret = kcIni + (cob - cobIni) * (kcFin - kcIni) / (cobFin - cobIni);
                }
            }
            return ret;
        }

        /// <summary>
        /// Calcula KcAdj.
        /// </summary>
        /// <param name="kc">kc<see cref="double"/>.</param>
        /// <param name="tcAlt">tcAlt<see cref="double"/>.</param>
        /// <param name="velocidadViento">velocidadViento<see cref="double"/>.</param>
        /// <param name="humedadMedia">humedadMedia<see cref="double"/>.</param>
        /// <returns><see cref="double"/>.</returns>
        public static double KcAdjClima(double kc, double tcAlt, double velocidadViento, double humedadMedia) {
            double ret;
            if (kc < 0.45) {
                ret = kc;
            } else {
                ret = kc + (0.04 * (velocidadViento - 2) - 0.004 * (humedadMedia - 45)) * Math.Pow(tcAlt / 3, 0.3);
            }
            return ret;
        }

        /// <summary>
        /// The RaizLongitud.
        /// </summary>
        /// <param name="lbAnt">The lbAnt<see cref="LineaBalance"/>.</param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <param name="it">The it<see cref="double"/>.</param>
        /// <param name="profRaizInicial">The profRaizInicial<see cref="double"/>.</param>
        /// <param name="modRaizCoefB">The modRaizCoefB<see cref="double"/>.</param>
        /// <param name="profRaizMax">The profRaizMax<see cref="double"/>.</param>
        /// <param name="definicionEtapaPorDias">The definicionEtapaPorDias<see cref="bool"/>.</param>
        /// <param name="nDiasEtapas1y2">The nDiasEtapas1y2<see cref="int"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double RaizLongitud(LineaBalance lbAnt, int nEtapa, double it, double profRaizInicial, double modRaizCoefB, double profRaizMax, bool definicionEtapaPorDias, int nDiasEtapas1y2) {
            double ret;

            if (lbAnt.LongitudRaiz == 0) {
                ret = profRaizInicial;
            } else if (definicionEtapaPorDias) { // propuestaSIAR if(modRaizCoefB < 0)
                if (nEtapa < 3) {
                    ret = lbAnt.LongitudRaiz + (profRaizMax - profRaizInicial) / (nDiasEtapas1y2 - 2);
                    if (ret > profRaizMax) {
                        ret = profRaizMax;
                    }
                } else {
                    ret = lbAnt.LongitudRaiz;
                }
            } else {
                ret = lbAnt.LongitudRaiz + modRaizCoefB * it;
                if (ret > profRaizMax) {
                    ret = profRaizMax;
                }
            }
            return ret;
        }

        /// <summary>
        /// Calcula Capacidad de campo.
        /// </summary>
        /// <param name="root">root<see cref="double"/>.</param>
        /// <param name="pUnidadCultivoSuelo">pUnidadCultivoSuelo<see cref="List{UnidadCultivoSuelo}"/>.</param>
        /// <returns><see cref="double"/>.</returns>
        public static double CapacidadCampo(double root, List<UnidadCultivoSuelo> pUnidadCultivoSuelo) {
            double ret = 0;
            double profRestante = root;
            int i = 0;
            while (i < pUnidadCultivoSuelo.Count && profRestante > 0) {
                double c = pUnidadCultivoSuelo[i].CapacidadCampo();

                if (profRestante > pUnidadCultivoSuelo[i].ProfundidadHorizonte) {
                    ret = ret + (pUnidadCultivoSuelo[i].ProfundidadHorizonte * 1000 * c);
                } else {
                    ret = ret + (profRestante * 1000 * c);
                }
                profRestante = profRestante - pUnidadCultivoSuelo[i].ProfundidadHorizonte;
                i++;
            }

            return ret;
        }

        /// <summary>
        /// Calculo de Depletion Factor.
        /// </summary>
        /// <param name="etc">.</param>
        /// <param name="nEtapa">.</param>
        /// <param name="unidadCultivoCultivosEtapasList">.</param>
        /// <returns>.</returns>
        public static double DepletionFactor(double etc, int nEtapa, List<UnidadCultivoCultivoEtapas> unidadCultivoCultivosEtapasList) {
            int nEtapaBase0 = nEtapa > 0 ? nEtapa - 1 : 0;
            double ret = unidadCultivoCultivosEtapasList[nEtapaBase0].FactorDeAgotamiento + 0.04 * (5 - etc);
            if (ret < 0.1)
                ret = 0.1;
            if (ret > 0.8)
                ret = 0.8;
            return ret;
        }

        /// <summary>
        /// Caculo de AguaFacilmenteExtraibleFija - RAW2.
        /// </summary>
        /// <param name="taw">.</param>
        /// <param name="nEtapa">.</param>
        /// <param name="unidadCultivoCultivosEtapasList">.</param>
        /// <returns>.</returns>
        public static double AguaFacilmenteExtraibleFija(double taw, int nEtapa, List<UnidadCultivoCultivoEtapas> unidadCultivoCultivosEtapasList) {
            int nEtapaBase0 = nEtapa > 0 ? nEtapa - 1 : 0;
            double ret = taw * Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaBase0].FactorDeAgotamiento);
            return ret;
        }

        /// <summary>
        /// Calcula punto de marchitez.
        /// </summary>
        /// <param name="root">.</param>
        /// <param name="pUnidadCultivoSuelo">.</param>
        /// <returns>.</returns>
        public static double PuntoMarchitez(double root, List<UnidadCultivoSuelo> pUnidadCultivoSuelo) {
            double profRestante = root;
            int i = 0;
            double ret = 0;
            while (i < pUnidadCultivoSuelo.Count && profRestante > 0) {
                double m = pUnidadCultivoSuelo[i].PuntoDeMarchitez();
                if (profRestante > pUnidadCultivoSuelo[i].ProfundidadHorizonte) {
                    ret = ret + (pUnidadCultivoSuelo[i].ProfundidadHorizonte * 1000 * m);
                } else {
                    ret = ret + (profRestante * 1000 * m);
                }
                profRestante = profRestante - pUnidadCultivoSuelo[i].ProfundidadHorizonte;
                i++;
            }
            return ret;
        }

        /// <summary>
        /// Calculo de la precipitación efectiva.
        /// </summary>
        /// <param name="precipitacion">.</param>
        /// <param name="eto">.</param>
        /// <returns>.</returns>
        public static double PrecipitacionEfectiva(double precipitacion, double eto) {
            double ret = precipitacion > 2 ? precipitacion - 0.2 * eto : 0;
            return ret;
        }

        /// <summary>
        /// Calculo de Coeficiente de estrés hídrico Ks.
        /// </summary>
        /// <param name="taw">.</param>
        /// <param name="raw">.</param>
        /// <param name="dr">.</param>
        /// <returns>.</returns>
        public static double CoeficienteEstresHidrico(double taw, double raw, double dr) {
            double ret = dr < raw ? 1 : (taw - dr) / (taw - raw);
            return ret;
        }

        /// <summary>
        /// The EtcAdj.
        /// </summary>
        /// <param name="et0">The et0<see cref="double"/>.</param>
        /// <param name="kcAdj">The kcAdj<see cref="double"/>.</param>
        /// <param name="ks">The ks<see cref="double"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double EtcFinal(double et0, double kcAdj, double ks) {
            //ETc ajustada por clima y estrés
            double ret = et0 * kcAdj * ks;
            if (ret == 0)
                ret = 0;
            return ret;
        }

        /// <summary>
        /// Cálculo del riego efectivo.
        /// </summary>
        /// <param name="riego">.</param>
        /// <param name="eficienciaRiego">.</param>
        /// <returns>.</returns>
        public static double RiegoEfectivo(double riego, double eficienciaRiego) => riego * eficienciaRiego;

        /// <summary>
        /// Calcula la Cobertura a una fecha dada. Tiene en cuenta si se han indicado datos extra.
        /// </summary>
        /// <param name="antCob">.</param>
        /// <param name="tcCob">.</param>
        /// <param name="incT">.</param>
        /// <param name="datoExtra">.</param>
        /// <returns>.</returns>
        public static double Cobertura(double antCob, double tcCob, double incT, UnidadCultivoDatosExtra datoExtra) {
            double ret;
            if (datoExtra?.Cobertura != null)
                ret = (double)datoExtra.Cobertura;
            else
                ret = antCob + tcCob * incT;
            return ret < 1 ? ret : 1;// !!! SIAR limitar cobertura máxima a 1                               
        }

        /// <summary>
        /// Calcula la Altura para una fecha dada. NO Tiene en cuenta si se han indicado datos extra.
        /// </summary>
        /// <param name="antAlt">.</param>
        /// <param name="tcAlt">.</param>
        /// <param name="incT">.</param>
        /// <param name="alturaFinal">.</param>
        /// <param name="datoExtra">.</param>
        /// <returns>.</returns>
        public static double Altura(double antAlt, double tcAlt, double incT, double? alturaFinal, UnidadCultivoDatosExtra datoExtra) {
            double ret = 0;
            if (datoExtra?.Altura != null)
                ret = datoExtra.Altura ?? 0;
            else
                ret = (antAlt + tcAlt * incT);
            if (alturaFinal != null && alturaFinal > 0 && ret > alturaFinal)
                ret = (double)alturaFinal;
            return ret;
        }

        /// <summary>
        /// CalculaAguaAportadaCrecRaiz.
        /// </summary>
        /// <param name="pSaturacion">pSaturacion<see cref="double"/>.</param>
        /// <param name="tawHoy">tawHoy<see cref="double"/>.</param>
        /// <param name="tawAyer">tawAyer<see cref="double"/>.</param>
        /// <returns><see cref="double"/>.</returns>
        public static double AguaAportadaCrecRaiz(double pSaturacion, double tawHoy, double tawAyer) {
            //  se usa el parametro pSaturacion que desde la funcion principal indica el porcentaje de agua que hay en el suelo
            //   que va explorando la raíz, cuando la raíz ha alcanzado su tamaño definitivo TAW es constante, es decir
            //   tawyHoy = tawAyer por lo que la aportación de agua es 0.

            double aguaAportadaCrecRaiz = pSaturacion * (tawHoy - tawAyer);
            return aguaAportadaCrecRaiz;
        }

        /// <summary>
        /// The AgotamientoFinalDia.
        /// </summary>
        /// <param name="taw">The taw<see cref="double"/>.</param>
        /// <param name="EtcAdj">The EtcAdj<see cref="double"/>.</param>
        /// <param name="rieEfec">The rieEfec<see cref="double"/>.</param>
        /// <param name="pef">The pef<see cref="double"/>.</param>
        /// <param name="aguaAportadaCrecRaiz">The aguaAportadaCrecRaiz<see cref="double"/>.</param>
        /// <param name="driStart">The driStart<see cref="double"/>.</param>
        /// <param name="dp">The dp<see cref="double"/>.</param>
        /// <param name="escorrentia">The escorrentia<see cref="double"/>.</param>
        /// <param name="lbAnt">The lbAnt<see cref="LineaBalance"/>.</param>
        /// <param name="datoExtra">The datoExtra<see cref="UnidadCultivoDatosExtra"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double AgotamientoFinalDia(double taw, double EtcAdj, double rieEfec, double pef, double aguaAportadaCrecRaiz, double driStart, double dp, double escorrentia, LineaBalance lbAnt, UnidadCultivoDatosExtra datoExtra) {
            double ret = 0;
            if (datoExtra?.DriEnd != null) {// si existen datos extra prevalecen sobre los calculados.
                return datoExtra.DriEnd ?? 0;
            }
            if (lbAnt.Fecha == null)
                driStart = taw; // el día 1 el "depósito" está vacío
            ret = driStart - rieEfec - pef - aguaAportadaCrecRaiz + EtcAdj + dp + escorrentia;
            return ret > 0 ? ret : 0; // !!! modificado SIAR para corregir errores debidos a la eliminación del drenaje (dp) inferiores al umbral Config.GetDouble("DrenajeUmbral")
        }

        /// <summary>
        /// The DrenajeEnProdundidad.
        /// </summary>
        /// <param name="lbAnt">The lbAnt<see cref="LineaBalance"/>.</param>
        /// <param name="taw">The taw<see cref="double"/>.</param>
        /// <param name="ETcAdj">The ETcAdj<see cref="double"/>.</param>
        /// <param name="rieEfec">The rieEfec<see cref="double"/>.</param>
        /// <param name="pef">The pef<see cref="double"/>.</param>
        /// <param name="aguaAportadaCrecRaiz">The aguaAportadaCrecRaiz<see cref="double"/>.</param>
        /// <param name="driStart">The driStart<see cref="double"/>.</param>
        /// <param name="escorrentia">The escorrentia<see cref="double"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double DrenajeEnProdundidad(LineaBalance lbAnt, double taw, double ETcAdj, double rieEfec, double pef, double aguaAportadaCrecRaiz, double driStart, double escorrentia) {
            if (lbAnt.Fecha == null)
                driStart = taw; // el día 1 el "depósito" está vacío
            double ret = rieEfec + pef + aguaAportadaCrecRaiz - (ETcAdj + driStart + escorrentia);
            if (ret < 0)
                ret = 0;
            if (ret > 0) {
                double drenajeUmbral = Config.GetDouble("DrenajeUmbral");
                if (ret < drenajeUmbral)
                    ret = 0;
            }
            return ret;
        }

        /// <summary>
        /// Calculo de la recomentación de riego en mm.
        /// </summary>
        /// <param name="raw">The raw<see cref="double"/>.</param>
        /// <param name="taw">The taw<see cref="double"/>.</param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <param name="driEnd">The driEnd<see cref="double"/>.</param>
        /// <param name="etapaInicioRiego">The etapaInicioRiego<see cref="int"/>.</param>
        /// <param name="ieUmbralRiego">The ieUmbralRiego<see cref="double"/>.</param>
        /// <param name="ieLimiteRiego">The ieLimiteRiego<see cref="double"/>.</param>
        /// <returns>.</returns>
        public static double RecomendacionRiegoMm(double raw, double taw, int nEtapa, double driEnd, int etapaInicioRiego, double ieUmbralRiego, double ieLimiteRiego) {
            double drUmbralRiego = 0;
            if (ieUmbralRiego < 0)
                drUmbralRiego = taw - (1 + ieUmbralRiego) * (taw - raw);
            else
                drUmbralRiego = raw * (1 - ieUmbralRiego);

            double drLimiteRiego = 0;
            if (ieLimiteRiego < 0)
                drLimiteRiego = taw - (1 + ieLimiteRiego) * (taw - raw);
            else
                drLimiteRiego = raw * (1 - ieLimiteRiego);

            double ret = 0;
            if (nEtapa >= etapaInicioRiego && driEnd > drUmbralRiego) {
                ret = driEnd - drLimiteRiego;
            }
            return ret;
        }

        /// <summary>
        /// Calcula la recomención del Riego en tiempo (Horas).
        /// </summary>
        /// <param name="raw">.</param>
        /// <param name="nEtapa">.</param>
        /// <param name="driEnd">.</param>
        /// <param name="v1">.</param>
        /// <param name="v2">.</param>
        /// <param name="v3">.</param>
        /// <returns>.</returns>
        public static double RecomendacionRiegoHr(double raw, int nEtapa, double driEnd, int v1, double v2, int v3) => double.NaN;

        /// <summary>
        /// Incremento de temperatura efectivo.
        /// </summary>
        /// <param name="temperatura">The temperatura<see cref="double"/>.</param>
        /// <param name="CultivoTBase">The CultivoTBase<see cref="double"/>.</param>
        /// <param name="definicionPorDias">The definicionPorDias<see cref="bool"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double IncrementoTemperatura(double temperatura, double CultivoTBase, bool definicionPorDias) {
            // ANTES=> temperatura > CultivoTBase ? temperatura - CultivoTBase : 0;
            double ret;
            if (definicionPorDias) {// propuestaSIAR if(CultivoTBase < 0) {PARECE COMPLICADO.... ESTO FUE UN APAÑO Y CON ESTA SOLUCION ESTÁ FALSEADO Y NO ES CORRECTO
                ret = 1;
            } else {
                if (temperatura > CultivoTBase) {
                    ret = temperatura - CultivoTBase;
                } else {
                    ret = 0;
                }
            }
            return (ret);
        }

        /// <summary>
        /// AvisoDrenaje.
        /// </summary>
        /// <param name="dp">dp<see cref="double"/>.</param>
        /// <returns><see cref="bool"/>.</returns>
        public static bool AvisoDrenaje(double dp) => dp > Config.GetDouble("DrenajeUmbral");

        /// <summary>
        /// Calculo del índice de Estres
        /// El valor estarán entre -1 y 1 + drenajeProduncidad 
        /// Si sobrepasa el valor de 1 indica que hay exceso de agua.
        /// </summary>
        /// <param name="contenidoAguaSuelo">.</param>
        /// <param name="limiteAgotamiento">.</param>
        /// <param name="coeficienteEstresFinalDelDia">.</param>
        /// <param name="capacidadDeCampo">.</param>
        /// <param name="drenajeProfundidad">.</param>
        /// <returns>.</returns>
        public static double IndiceEstres(double contenidoAguaSuelo, double limiteAgotamiento, double coeficienteEstresFinalDelDia, double capacidadDeCampo, double drenajeProfundidad) {
            double ret = 0;
            if (contenidoAguaSuelo > limiteAgotamiento) {
                double divisor = capacidadDeCampo - limiteAgotamiento;
                if (divisor == 0)
                    ret = double.PositiveInfinity;
                else
                    ret = ((contenidoAguaSuelo + drenajeProfundidad - limiteAgotamiento) / divisor);
            } else {
                ret = (coeficienteEstresFinalDelDia - 1);
            }
            return ret;
        }

        /// <summary>
        /// The LimiteOptimoRefClima.
        /// </summary>
        /// <param name="lo">The lo<see cref="double"/>.</param>
        /// <param name="pm">The pm<see cref="double"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double LimiteOptimoRefClima(double lo, double pm) => lo - pm;

        /// <summary>
        /// The LimiteOptimoFijoRefClima.
        /// </summary>
        /// <param name="loFijo">The loFijo<see cref="double"/>.</param>
        /// <param name="pm">The pm<see cref="double"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double LimiteOptimoFijoRefClima(double loFijo, double pm) => loFijo - pm;

        /// <summary>
        /// The ContenidoAguaSuelRefPuntoMarchitezMm.
        /// </summary>
        /// <param name="os">The os<see cref="double"/>.</param>
        /// <param name="pm">The pm<see cref="double"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double ContenidoAguaSuelRefPuntoMarchitezMm(double os, double pm) => os - pm;

        /// <summary>
        /// The PuntoMarchitezRefPuntoMarchitezMm.
        /// </summary>
        /// <returns>The <see cref="double"/>.</returns>
        public static double PuntoMarchitezRefPuntoMarchitezMm() => 0;

        /// <summary>
        /// The CapacidadCampoRefPuntoMarchitezMm.
        /// </summary>
        /// <param name="cc">The cc<see cref="double"/>.</param>
        /// <param name="pm">The pm<see cref="double"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double CapacidadCampoRefPuntoMarchitezMm(double cc, double pm) => cc - pm;

        /// <summary>
        /// CalculaLineaBalance.
        /// </summary>
        /// <param name="dh">dh<see cref="UnidadCultivoDatosHidricos"/>.</param>
        /// <param name="lbAnt">lbAnt<see cref="LineaBalance"/>.</param>
        /// <param name="fecha">fecha<see cref="DateTime"/>.</param>
        /// <returns><see cref="LineaBalance"/>.</returns>
        public static LineaBalance CalculaLineaBalance(UnidadCultivoDatosHidricos dh, LineaBalance lbAnt, DateTime fecha) {
            LineaBalance lb = new LineaBalance {
                Fecha = fecha
            };
            if (lbAnt == null)
                lbAnt = new LineaBalance();
            double temperatura = dh.Temperatura(fecha);

            bool definicionPorDias = dh.UnidadCultivoCultivoEtapasList[lb.NumeroEtapaDesarrollo - 1].DefinicionPorDias;
            double incT = IncrementoTemperatura(temperatura, dh.CultivoTBase, definicionPorDias);// !!! SIAR
            if (lbAnt?.Fecha == null) incT = 0; // el primero es 0

            UnidadCultivoDatosExtra datoExtra = dh.DatoExtra(fecha);

            lb.IntegralTermica = (lbAnt.IntegralTermica + incT);

            bool definicionEtapaPorDias = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo-1].DefinicionPorDias;
            int nDiasduracionEtapaDias = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo-1].DuracionDiasEtapa;
            double? coberturaInicial = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo-1].CobInicial;
            double? coberturaFinal = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo-1].CobFinal;
            int NDiasEtapas1y2 = dh.CultivoEtapasList_Ndias1y2();

            lb.TasaCrecimientoCobertura = TasaCrecimientoCobertura(lb.IntegralTermica, lbAnt.NumeroEtapaDesarrollo, dh.CultivoIntegralEmergencia, dh.CultivoModCobCoefA, dh.CultivoModCobCoefB, dh.CultivoModCobCoefC, definicionEtapaPorDias, nDiasduracionEtapaDias, coberturaInicial, coberturaFinal);
            lb.TasaCrecimientoAltura = TasaCrecimientoAltura(lb.IntegralTermica, lbAnt.NumeroEtapaDesarrollo, dh.CultivoIntegralEmergencia, dh.CultivoModAltCoefA, dh.CultivoModAltCoefB, dh.CultivoModAltCoefC, definicionEtapaPorDias, NDiasEtapas1y2, dh.CultivoAlturaInicial, dh.CultivoAlturaFinal);
            lb.Cobertura = Cobertura(lbAnt.Cobertura, lb.TasaCrecimientoCobertura, incT, datoExtra);

            lb.NumeroEtapaDesarrollo = NumeroEtapaDesarrollo(fecha, lb.Cobertura, lbAnt.NumeroEtapaDesarrollo, dh.UnidadCultivoCultivoEtapasList, dh.UnidadCultivoCultivoEtapasList);
            lb.AlturaCultivo = Altura(lbAnt.AlturaCultivo, lb.TasaCrecimientoAltura, incT, dh.CultivoAlturaFinal, datoExtra);
            lb.LongitudRaiz = RaizLongitud(lbAnt, lb.NumeroEtapaDesarrollo, incT, dh.CultivoProfRaizInicial, dh.CultivoModRaizCoefB, dh.CultivoProfRaizMax, definicionEtapaPorDias, NDiasEtapas1y2);

            lb.DiasMaduracion = lbAnt.DiasMaduracion > 0 ? lb.DiasMaduracion = lbAnt.DiasMaduracion + 1 : lb.Cobertura > 0.8 ? 1 : 0;
            lb.NombreEtapaDesarrollo = dh.UnidadCultivoCultivoEtapasList[lb.NumeroEtapaDesarrollo - 1].Etapa;

            // Parámetros de suelo
            lb.CapacidadCampo = CapacidadCampo(lb.LongitudRaiz, dh.ListaUcSuelo);
            lb.PuntoMarchitez = PuntoMarchitez(lb.LongitudRaiz, dh.ListaUcSuelo);
            lb.AguaDisponibleTotal = lb.CapacidadCampo - lb.PuntoMarchitez;

            // Parámetros de aporte de agua
            lb.Lluvia = dh.LluviaMm(fecha);
            lb.LluviaEfectiva = PrecipitacionEfectiva(lb.Lluvia, dh.Eto(fecha));
            lb.Riego = dh.RiegoMm(fecha);
            lb.RiegoEfectivo = RiegoEfectivo(lb.Riego, dh.EficienciaRiego);
            lb.AguaCrecRaiz = AguaAportadaCrecRaiz(0.8, lb.AguaDisponibleTotal, lbAnt.AguaDisponibleTotal);

            // Parámetros de cálculo del balance
            lb.AgotamientoInicioDia = lbAnt.AgotamientoFinalDia;
            lb.Kc = Kc(lb.NumeroEtapaDesarrollo, fecha, lb.Cobertura, dh.UnidadCultivoCultivoEtapasList, dh.UnidadCultivoCultivoEtapasList);
            lb.KcAjustadoClima = KcAdjClima(lb.Kc, lb.AlturaCultivo, dh.VelocidadViento(fecha), dh.HumedadMedia(fecha));

            // Parámetros de estrés en suelo
            lb.FraccionAgotamiento = DepletionFactor(lb.KcAjustadoClima * dh.Eto(fecha), lb.NumeroEtapaDesarrollo, dh.UnidadCultivoCultivoEtapasList);
            lb.AguaFacilmenteExtraible = lb.FraccionAgotamiento * lb.AguaDisponibleTotal; // depletion factor f(ETc)
            lb.AguaFacilmenteExtraibleFija = AguaFacilmenteExtraibleFija(lb.AguaDisponibleTotal, lb.NumeroEtapaDesarrollo, dh.UnidadCultivoCultivoEtapasList);
            lb.LimiteAgotamiento = (lb.CapacidadCampo - lb.AguaFacilmenteExtraible); // depletion factor f(ETc)
            lb.LimiteAgotamientoFijo = (lb.CapacidadCampo - lb.AguaFacilmenteExtraibleFija); // depletion factor fijo
            lb.CoeficienteEstresHidrico = CoeficienteEstresHidrico(lb.AguaDisponibleTotal, lb.AguaFacilmenteExtraible, lb.AgotamientoInicioDia); // K de estrés hídrico

            if (fecha == new DateTime(2020, 08, 26))
                fecha = fecha;
            lb.EtcFinal = EtcFinal(dh.Eto(fecha), lb.KcAjustadoClima, lb.CoeficienteEstresHidrico); //ETc ajustada por clima y estrés

            lb.DrenajeProfundidad = DrenajeEnProdundidad(lbAnt, lb.AguaDisponibleTotal, lb.EtcFinal, lb.RiegoEfectivo, lb.LluviaEfectiva, lb.AguaCrecRaiz, lb.AgotamientoInicioDia, 0);
            lb.AgotamientoFinalDia = AgotamientoFinalDia(lb.AguaDisponibleTotal, lb.EtcFinal, lb.RiegoEfectivo, lb.LluviaEfectiva, lb.AguaCrecRaiz, lb.AgotamientoInicioDia, lb.DrenajeProfundidad, 0, lbAnt, datoExtra);
            lb.ContenidoAguaSuelo = lb.CapacidadCampo - lb.AgotamientoFinalDia;

            double CoeficienteEstresHidricoFinalDelDia = CoeficienteEstresHidrico(lb.AguaDisponibleTotal, lb.AguaFacilmenteExtraible, lb.AgotamientoFinalDia);
            lb.IndiceEstres = IndiceEstres(lb.ContenidoAguaSuelo, lb.LimiteAgotamiento, CoeficienteEstresHidricoFinalDelDia, lb.CapacidadCampo, lb.DrenajeProfundidad);

            dh.ClaseEstresUmbralInferiorYSuperior(lb.NumeroEtapaDesarrollo, out double limiteInferior, out double limiteSuperior);

            TipoEstresUmbral tipoEstresUmbral = dh.TipoEstresUmbral(lb.IndiceEstres, lb.NumeroEtapaDesarrollo);
            lb.MensajeEstres = tipoEstresUmbral.Mensaje;
            lb.DescripcionEstres = tipoEstresUmbral.Descripcion;
            lb.ColorEstres = tipoEstresUmbral.Color;

            lb.RecomendacionRiegoNeto = RecomendacionRiegoMm(lb.AguaFacilmenteExtraible, lb.AguaDisponibleTotal, lb.NumeroEtapaDesarrollo, lb.AgotamientoFinalDia, dh.EtapaInicioRiego, limiteInferior, limiteSuperior);
            lb.RecomendacionRiegoBruto = lb.RecomendacionRiegoNeto / dh.EficienciaRiego;
            lb.RecomendacionRiegoTiempo = lb.RecomendacionRiegoBruto / dh.Pluviometria;

            lb.CapacidadCampoRefPM = CapacidadCampoRefPuntoMarchitezMm(lb.CapacidadCampo, lb.PuntoMarchitez);
            lb.PuntoMarchitezRefPM = PuntoMarchitezRefPuntoMarchitezMm();
            lb.ContenidoAguaSueloRefPM = ContenidoAguaSuelRefPuntoMarchitezMm(lb.ContenidoAguaSuelo, lb.PuntoMarchitez);
            lb.LimiteAgotamientoRefPM = LimiteOptimoRefClima(lb.LimiteAgotamiento, lb.PuntoMarchitez);
            lb.LimiteAgotamientoFijoRefPM = LimiteOptimoFijoRefClima(lb.LimiteAgotamientoFijo, lb.PuntoMarchitez);
            return lb;
        }
    }
}
