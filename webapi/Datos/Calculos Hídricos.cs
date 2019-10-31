﻿namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Clase estática en la que se implementan las funciones hídricas
    /// </summary>
    public static class CalculosHidricos {
        /// <summary>
        /// CapacidadCampo. Función definida como extensi´no de UnidadCultivoSuelo
        /// </summary>
        /// <param name="suelo">The suelo<see cref="UnidadCultivoSuelo"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CapacidadCampo(this UnidadCultivoSuelo suelo) {
            double mo100 = suelo.MateriaOrganica * 100;
            double N = -0.251 * suelo.Arena + 0.195 * suelo.Arcilla + 0.011 * mo100 + 0.006 * (suelo.Arena * mo100) - 0.027 * (suelo.Arcilla * mo100) + 0.452 * (suelo.Arena * suelo.Arcilla) + 0.299;
            double P = 0.278 * suelo.Arena + 0.034 * suelo.Arcilla + 0.022 * mo100 - 0.018 * (suelo.Arena * mo100) - 0.027 * (suelo.Arcilla * mo100) - 0.584 * (suelo.Arena * suelo.Arcilla) + 0.078;
            double O = P + (0.636 * P - 0.107);
            double M = -0.024 * suelo.Arena + 0.487 * suelo.Arcilla + 0.006 * mo100 + 0.005 * (suelo.Arena * mo100) - 0.013 * (suelo.Arcilla * mo100) + 0.068 * (suelo.Arena * suelo.Arcilla) + 0.031;
            double H = M + (0.14 * M - 0.02);
            double G = N + (1.283 * (N * N) - 0.374 * (N) - 0.015);
            double I = G + O - 0.097 * suelo.Arena + 0.043;
            double J = (1 - I) * 2.65;
            double K = ((J / 2.65) * suelo.ElementosGruesos) / (1 - suelo.ElementosGruesos * (1 - (J / 2.65)));
            double L = J * (1 - K) + (K * 2.65);
            double r = G * (1 - suelo.ElementosGruesos);
            return r;
        }

        /// <summary>
        /// PuntoDeMarchitez. Función definida como extensi´no de UnidadCultivoSuelo
        /// </summary>
        /// <param name="suelo">The suelo<see cref="UnidadCultivoSuelo"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double PuntoDeMarchitez(this UnidadCultivoSuelo suelo) {
            double mo100 = suelo.MateriaOrganica * 100;
            double N = 0.251 * suelo.Arena + 0.195 * suelo.Arcilla + 0.011 * mo100 + 0.006 * (suelo.Arena * mo100) - 0.027 * (suelo.Arcilla * mo100) + 0.452 * (suelo.Arena * suelo.Arcilla) + 0.299;
            double P = 0.278 * suelo.Arena + 0.034 * suelo.Arcilla + 0.022 * mo100 - 0.018 * (suelo.Arena * mo100) - 0.027 * (suelo.Arcilla * mo100) - 0.584 * (suelo.Arena * suelo.Arcilla) + 0.078;
            double O = P + (0.636 * P - 0.107);
            double M = -0.024 * suelo.Arena + 0.487 * suelo.Arcilla + 0.006 * mo100 + 0.005 * (suelo.Arena * mo100) - 0.013 * (suelo.Arcilla * mo100) + 0.068 * (suelo.Arena * suelo.Arcilla) + 0.031;
            double H = M + (0.14 * M - 0.02);
            double G = N + (1.283 * (N * N) - 0.374 * (N) - 0.015);
            double I = G + O - 0.097 * suelo.Arena + 0.043;
            double J = (1 - I) * 2.65;
            double K = ((J / 2.65) * suelo.ElementosGruesos) / (1 - suelo.ElementosGruesos * (1 - (J / 2.65)));
            double L = J * (1 - K) + (K * 2.65);
            double r = H * (1 - suelo.ElementosGruesos);
            return r;
        }

        /// <summary>
        /// Calculo de la Cobertura por el método de la Tasa de Crecimiento
        /// </summary>
        /// <param name="it">Integral térmica</param>
        /// <param name="nEtapa"></param>
        /// <param name="itEmergencia"></param>
        /// <param name="ModCobCoefA"></param>
        /// <param name="ModCobCoefB"></param>
        /// <param name="ModCobCoefC"></param>
        /// <returns></returns>
        public static double TcCob(double it, int nEtapa, double itEmergencia, double ModCobCoefA, double ModCobCoefB, double? ModCobCoefC) {
            // ((CobCoefA) * CobCoefB * EXP(-CobCoefB * (C10 - CobCoefC)))/POTENCIA((1+EXP(-CobCoefB*(C10-CobCoefC)));2)
            //     1.- La función necesita conocer el número de Etapa y la it de emergencia de la BBDD
            //     2.- Si la it es menor que la de emergencia la planta no ha brotado y no hay cobertura
            //     3.- cuando la planta brota, se calcula la cobertura
            //     4.- se cambia la variable itomprueba si el coeficiente C existe y se aplica fórmula logarítmica o lineal
            double ret;

            if (nEtapa == 1 && it < itEmergencia) {
                ret = 0;
            } else {
                //it = it - itEmergencia; La IT de emergencia NO se debe usar para calcular la TcCob
                if (ModCobCoefC != null && ModCobCoefC != 0) {
                    ret = ((ModCobCoefA) * ModCobCoefB * Math.Exp(-ModCobCoefB * (it - (double)ModCobCoefC))) / Math.Pow((1 + Math.Exp(-ModCobCoefB * (it - (double)ModCobCoefC))), 2);
                } else {
                    ret = ModCobCoefB;
                }
            }
            return ret;
        }

        /// <summary>
        /// Calculo de la altura por el método de la Tasa de crecimiento
        /// </summary>
        /// <param name="it"></param>
        /// <param name="nEtapa"></param>
        /// <param name="itEmergencia"></param>
        /// <param name="ModAltCoefA"></param>
        /// <param name="ModAltCoefB"></param>
        /// <param name="ModAltCoefC"></param>
        /// <returns></returns>
        public static double TcAlt(double it, int nEtapa, double itEmergencia, double ModAltCoefA, double ModAltCoefB, double? ModAltCoefC) {
            // ((CobCoefA) * CobCoefB * EXP(-CobCoefB * (C10 - CobCoefC)))/POTENCIA((1+EXP(-CobCoefB*(C10-CobCoefC)));2)
            //     1.- La función necesita conocer el número de Etapa y la it de emergencia de la BBDD
            //     2.- Si la it es menor que la de emergencia la planta no ha brotado y no hay cobertura
            //     3.- cuando la planta brota, se calcula la cobertura
            //     4.- se cambia la variable it por it (más correcto conceptualmente)
            //     5.- se añade la rutina que comprueba si el coeficiente C existe y se aplica fórmula logarítmica o lineal
            double ret;

            if (nEtapa == 1 && it < itEmergencia) {
                ret = 0;
            } else {
                //it = it - itEmergencia; La IT de emergencia NO se debe usar para calcular la TcCob
                if (ModAltCoefC != null && ModAltCoefC != 0) {
                    ret = (ModAltCoefA * ModAltCoefB * Math.Exp(-ModAltCoefB * (it - (double)ModAltCoefC))) / Math.Pow((1 + Math.Exp(-ModAltCoefB * (it - (double)ModAltCoefC))), 2);
                } else {
                    ret = ModAltCoefB;
                }
            }
            return ret;
        }

        /// <summary>
        /// Retorna en Nº de Etapa en la que nos encontramos en base 1 (1,2,3,4....)
        /// Párametro nEtapa está en base 1 y la tabla de etapas en base 0
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="cobertura"></param>
        /// <param name="currentnEtapa"></param>
        /// <param name="pUnidadCultivoCultivosEtapas"></param>
        /// <param name="pCultivoEtapas"></param>
        /// <returns></returns>
        public static int NEtapaDesarrollo(DateTime fecha, double cobertura, int currentnEtapa, List<UnidadCultivoCultivoEtapas> pUnidadCultivoCultivosEtapas, List<CultivoEtapas> pCultivoEtapas) {
            int nEtapaBase0 = currentnEtapa - 1 > 0 ? currentnEtapa - 1 : 0; // situación anómala
            int ret = currentnEtapa;
            if (pUnidadCultivoCultivosEtapas.Count < currentnEtapa)
                return pUnidadCultivoCultivosEtapas.Count; // situación anómala

            if (pUnidadCultivoCultivosEtapas[nEtapaBase0].DefinicionPorDias == true) {
                if (pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaFinEtapaConfirmada != null) {
                    if (nEtapaBase0 + 1 < pUnidadCultivoCultivosEtapas.Count)
                        //actualizar fecha inicio siguiente etapa
                        pUnidadCultivoCultivosEtapas[nEtapaBase0 + 1].FechaInicioEtapa = ((DateTime)pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaFinEtapaConfirmada).AddDays(1);
                    if (fecha > pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaFinEtapaConfirmada)
                        ret++;
                } else {
                    if (nEtapaBase0 + 1 < pUnidadCultivoCultivosEtapas.Count) {
                        DateTime fechaInicioSiguienteEtapa = pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaInicioEtapa.AddDays(pCultivoEtapas[nEtapaBase0].DuracionDiasEtapa);
                        if (fecha >= fechaInicioSiguienteEtapa) {
                            //actualizar fecha inicio siguiente etapa
                            pUnidadCultivoCultivosEtapas[nEtapaBase0 + 1].FechaInicioEtapa = fechaInicioSiguienteEtapa;
                            ret++;
                        }
                    }
                }
            } else { // definido por integral termica
                if (pUnidadCultivoCultivosEtapas[nEtapaBase0].CobFinal < cobertura) {
                    if (pUnidadCultivoCultivosEtapas.Count > nEtapaBase0 + 1)
                        //actulizar siguiente fecha de inicio siguiente etapa
                        pUnidadCultivoCultivosEtapas[nEtapaBase0 + 1].FechaInicioEtapa = fecha;
                    ret++;
                } else if (pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaFinEtapaConfirmada != null) {
                    if (pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaFinEtapaConfirmada < fecha) {
                        if (pUnidadCultivoCultivosEtapas.Count > nEtapaBase0 + 1)
                            pUnidadCultivoCultivosEtapas[nEtapaBase0 + 1].FechaInicioEtapa = ((DateTime)pUnidadCultivoCultivosEtapas[nEtapaBase0].FechaFinEtapaConfirmada).AddDays(1);
                        ret++;
                    }
                }
            }
            return ret > pUnidadCultivoCultivosEtapas.Count ? pUnidadCultivoCultivosEtapas.Count : ret;
        }

        /// <summary>
        /// Calculo del Coeficiento de cultivo
        /// </summary>
        /// <param name="nEtapa"></param>
        /// <param name="fecha"></param>
        /// <param name="cob"></param>
        /// <param name="unidadCultivoCultivosEtapasList"></param>
        /// <returns></returns>
        public static double Kc(int nEtapa, DateTime fecha, double cob, List<UnidadCultivoCultivoEtapas> unidadCultivoCultivosEtapasList) {
            double ret = 0;
            int nEtapaIndex = nEtapa - 1 > 0 ? nEtapa - 1 : 0; // la etapa está en base 1
            if (unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial == unidadCultivoCultivosEtapasList[nEtapaIndex].KcFinal) {
                ret = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial);
            } else {
                if (unidadCultivoCultivosEtapasList[nEtapaIndex].DefinicionPorDias == true) {
                    double nDias = (fecha - unidadCultivoCultivosEtapasList[nEtapaIndex].FechaInicioEtapa).Days;
                    DateTime fechaSigEtapa = fecha;
                    if (nEtapaIndex + 1 < unidadCultivoCultivosEtapasList.Count)
                        fechaSigEtapa = unidadCultivoCultivosEtapasList[nEtapaIndex + 1].FechaInicioEtapa;

                    double nDiasEtapaActual = (fechaSigEtapa - unidadCultivoCultivosEtapasList[nEtapaIndex].FechaInicioEtapa).Days;
                    if (nDias > nDiasEtapaActual)
                        nDias = nDiasEtapaActual;
                    ret = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial + (unidadCultivoCultivosEtapasList[nEtapaIndex].KcFinal - unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial) * nDias / nDiasEtapaActual);
                } else {
                    double kcIni = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].KcInicial);
                    double cobIni = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].CobInicial);
                    double kcFin = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].KcFinal);
                    double cobFin = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaIndex].CobFinal);
                    // calcular cobertura usando formulas tabla cultivo 
                    // usando cobertura cultivo obtener kc
                    ret = kcIni + (cob - cobIni) * (kcFin - kcIni) / (cobFin - cobIni);
                    // si la cobertura actual > cobFin ==> cambio etapa
                }
            }
            return ret;
        }

        /// <summary>
        /// Calcula KcAdj
        /// </summary>
        /// <param name="kc">kc<see cref="double"/></param>
        /// <param name="tcAlt">tcAlt<see cref="double"/></param>
        /// <param name="velocidadViento">velocidadViento<see cref="double"/></param>
        /// <param name="humedadMedia">humedadMedia<see cref="double"/></param>
        /// <returns><see cref="double"/></returns>
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
        /// Calculo de la longitud de la raiz.
        /// </summary>
        /// <param name="lbAnt">lbAnt<see cref="LineaBalance"/></param>
        /// <param name="it">it<see cref="double"/></param>
        /// <param name="profRaizInicial">profRaizInicial<see cref="double"/></param>
        /// <param name="modRaizCoefB">modRaizCoefB<see cref="double"/></param>
        /// <param name="profRaizMax">profRaizMax<see cref="double"/></param>
        /// <returns><see cref="double"/></returns>
        public static double RaizLongitud(LineaBalance lbAnt, double it, double profRaizInicial, double modRaizCoefB, double profRaizMax) {
            double ret;
            if (lbAnt.LongitudRaiz == 0) {
                ret = profRaizInicial;
            } else {
                ret = lbAnt.LongitudRaiz + modRaizCoefB * it;
                if (ret > profRaizMax) {
                    ret = profRaizMax;
                }
            }
            return ret;
        }

        /// <summary>
        /// Calcula Capacidad de campo
        /// </summary>
        /// <param name="root">root<see cref="double"/></param>
        /// <param name="pUnidadCultivoSuelo">pUnidadCultivoSuelo<see cref="List{UnidadCultivoSuelo}"/></param>
        /// <returns><see cref="double"/></returns>
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
        /// Calculo de Depletion Factor
        /// </summary>
        /// <param name="etc"></param>
        /// <param name="nEtapa"></param>
        /// <param name="unidadCultivoCultivosEtapasList"></param>
        /// <returns></returns>
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
        /// Caculo de RAW2
        /// </summary>
        /// <param name="taw"></param>
        /// <param name="nEtapa"></param>
        /// <param name="unidadCultivoCultivosEtapasList"></param>
        /// <returns></returns>
        public static double RAW2(double taw, int nEtapa, List<UnidadCultivoCultivoEtapas> unidadCultivoCultivosEtapasList) {
            int nEtapaBase0 = nEtapa > 0 ? nEtapa - 1 : 0;
            double ret = taw * Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaBase0].FactorDeAgotamiento);
            return ret;
        }

        /// <summary>
        /// Calcula punto de marchitez
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pUnidadCultivoSuelo"></param>
        /// <returns></returns>
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
        /// Calculo de la precipitación efectiva
        /// </summary>
        /// <param name="precipitacion"></param>
        /// <param name="eto"></param>
        /// <returns></returns>
        public static double PrecipitacionEfectiva(double precipitacion, double eto) {
            double ret = precipitacion > 2 ? precipitacion - 0.2 * eto : 0;
            return ret;
        }

        /// <summary>
        /// Calculo de Ks
        /// </summary>
        /// <param name="taw"></param>
        /// <param name="raw"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static double Ks(double taw, double raw, double dr) {
            double ret = dr < raw ? 1 : (taw - dr) / (taw - raw);
            return ret;
        }

        /// <summary>
        /// The EtcAdj
        /// </summary>
        /// <param name="et0">The et0<see cref="double"/></param>
        /// <param name="kcAdj">The kcAdj<see cref="double"/></param>
        /// <param name="ks">The ks<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double EtcAdj(double et0, double kcAdj, double ks) =>
            //ETc ajustada por clima y estrés
            et0 * kcAdj * ks;

        /// <summary>
        /// Calcula el Coeficiente de Cultivo ajustado
        /// </summary>
        /// <param name="KcAdjClima"></param>
        /// <param name="ks"></param>
        /// <returns></returns>
        public static double CoeficienteCultivoAjustado(double KcAdjClima, double ks) {
            double ret = KcAdjClima * ks;
            return ret;
        }

        /// <summary>
        /// Calcula la evotranspiración ajustada
        /// </summary>
        /// <param name="kcAdj"></param>
        /// <param name="eto"></param>
        /// <returns></returns>
        public static double EvoTranspiracionAjustada(double kcAdj, double eto) {
            double ret = kcAdj * eto;
            return ret;
        }

        /// <summary>
        /// Cálculo del riego efectivo
        /// </summary>
        /// <param name="riego"></param>
        /// <param name="eficienciaRiego"></param>
        /// <returns></returns>
        public static double RiegoEfectivo(double riego,double eficienciaRiego) {
            return riego * eficienciaRiego;
        }

        /// <summary>
        /// Calcula la Cobertura a una fecha dada. Tiene en cuenta si se han indicado datos extra.
        /// </summary>
        /// <param name="antCob"></param>
        /// <param name="tcCob"></param>
        /// <param name="incT"></param>
        /// <param name="datoExtra"></param>
        /// <returns></returns>
        public static double Cobertura(double antCob, double tcCob, double incT, UnidadCultivoDatosExtra datoExtra) {
            if (datoExtra?.Cobertura != null)
                return datoExtra.Cobertura ?? 0;
            else
                return (antCob + tcCob * incT);
        }

        /// <summary>
        /// Calcula la Altura para una fecha dada. NO Tiene en cuenta si se han indicado datos extra.
        /// </summary>
        /// <param name="antAlt"></param>
        /// <param name="tcAlt"></param>
        /// <param name="incT"></param>
        /// <param name="modAltCoefC"></param>
        /// <param name="alturaFinal"></param>
        /// <param name="datoExtra"></param>
        /// <returns></returns>
        public static double Altura(double antAlt, double tcAlt, double incT, double? modAltCoefC, double? alturaFinal, UnidadCultivoDatosExtra datoExtra) {
            double ret = 0;
            if (datoExtra?.Altura != null)
                return datoExtra.Altura ?? 0;
            if (modAltCoefC == 0 || modAltCoefC == null) {
                ret = antAlt + tcAlt * incT;
            } else {
                ret = antAlt + tcAlt;
            }
            if (ret > alturaFinal)
                ret = alturaFinal ?? 0;
            return ret;
        }

        /// <summary>
        /// CalculaAguaAportadaCrecRaiz
        /// </summary>
        /// <param name="pSaturacion">pSaturacion<see cref="double"/></param>
        /// <param name="tawHoy">tawHoy<see cref="double"/></param>
        /// <param name="tawAyer">tawAyer<see cref="double"/></param>
        /// <returns><see cref="double"/></returns>
        public static double AguaAportadaCrecRaiz(double pSaturacion, double tawHoy, double tawAyer) {
            /*  se usa el parametro pSaturacion que desde la funcion principal indica el porcentaje de agua que hay en el suelo
                 que va explorando la raíz, cuando la raíz ha alcanzado su tamaño definitivo TAW es constante, es decir
                 tawyHoy = tawAyer por lo que la aportación de agua es 0.
            */
            double aguaAportadaCrecRaiz = pSaturacion * (tawHoy - tawAyer);
            return aguaAportadaCrecRaiz;
        }

        /// <summary>
        /// CalculaDriEnd
        /// </summary>
        /// <param name="taw">taw<see cref="double"/></param>
        /// <param name="EtcAdj">EtcAdj<see cref="double"/></param>
        /// <param name="rieEfec">rieEfec<see cref="double"/></param>
        /// <param name="pef">pef<see cref="double"/></param>
        /// <param name="driStart">driStart<see cref="double"/></param>
        /// <param name="dp">dp<see cref="double"/></param>
        /// <param name="escorrentia">escorrentia<see cref="double"/></param>
        /// <param name="pSaturacion">pSaturacion<see cref="double"/></param>
        /// <param name="lbAnt">lbAnt<see cref="LineaBalance"/></param>
        /// <param name="datoExtra">datoExtra<see cref="UnidadCultivoDatosExtra"/></param>
        /// <returns><see cref="double"/></returns>
        public static double DriEnd(double taw, double EtcAdj, double rieEfec, double pef, double driStart, double dp, double escorrentia, double pSaturacion, LineaBalance lbAnt, UnidadCultivoDatosExtra datoExtra) {
            double ret = 0;
            if (datoExtra?.DriEnd != null) {// si existen datos extra prevalecen sobre los calculados.
                return datoExtra.DriEnd ?? 0;
            }
            if (lbAnt.Fecha == null) {
                ret = (1 - pSaturacion) * taw;
            } else {
                double aguaAportadaCrecRaiz = pSaturacion * (taw - lbAnt.AguaDisponibleTotal);
                ret = driStart - rieEfec - pef - aguaAportadaCrecRaiz + EtcAdj + dp + escorrentia;
            }
            return ret;
        }

        /// <summary>
        /// Drenaje en profundidad
        /// </summary>
        /// <param name="ETcAdj"></param>
        /// <param name="rieEfec"></param>
        /// <param name="pef"></param>
        /// <param name="aguaAportadaCrecRaiz"></param>
        /// <param name="driStart"></param>
        /// <returns></returns>
        public static double DrenajeEnProdundidad(double ETcAdj, double rieEfec, double pef, double aguaAportadaCrecRaiz, double driStart) {
            double ret = rieEfec + pef + aguaAportadaCrecRaiz - ETcAdj - driStart;
            if (ret < 0) ret = 0;
            return ret;
        }

        /// <summary>
        /// calculo de la Cobertura por el método de la Tasa de Crecimiento
        /// </summary>
        /// <param name="it"></param>
        /// <param name="itEmergencia"></param>
        /// <param name="ModCobCoefA"></param>
        /// <param name="ModCobCoefB"></param>
        /// <param name="ModCobCoefC"></param>
        /// <returns></returns>
        public static double TcCob(double it, double itEmergencia, double ModCobCoefA, double ModCobCoefB, double? ModCobCoefC) {

            double ret;

            if (it < itEmergencia) {
                ret = 0;
            } else {
                if (ModCobCoefC != null && ModCobCoefC != 0) {
                    ret = ((ModCobCoefA) * ModCobCoefB * Math.Exp(-ModCobCoefB * (it - ModCobCoefC ?? 0))) / Math.Pow((1 + Math.Exp(-ModCobCoefB * (it - (double)ModCobCoefC))), 2);
                } else {
                    ret = ModCobCoefB;
                }
            }
            return ret;
        }

        /// <summary>
        /// Calculo de la recomentación de riego en mm.
        /// </summary>
        /// <param name="raw">The raw<see cref="double"/></param>
        /// <param name="taw">The taw<see cref="double"/></param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/></param>
        /// <param name="driEnd">The driEnd<see cref="double"/></param>
        /// <param name="etapaInicioRiego">The etapaInicioRiego<see cref="int"/></param>
        /// <param name="ieUmbralRiego">The ieUmbralRiego<see cref="double"/></param>
        /// <param name="ieLimiteRiego">The ieLimiteRiego<see cref="double"/></param>
        /// <returns></returns>
        public static double RecomendacionRiegoMm(double raw, double taw, int nEtapa, double driEnd, int etapaInicioRiego, double ieUmbralRiego, double ieLimiteRiego) {
            // Conversion de Indices de estrés a valores de agotamiento
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
        /// Calcula la recomención del Riego en tiempo (Horas)
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="nEtapa"></param>
        /// <param name="driEnd"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static double RecomendacionRiegoHr(double raw, int nEtapa, double driEnd, int v1, double v2, int v3) => double.NaN;

        /// <summary>
        /// Incremento de temperatura efectivo
        /// </summary>
        /// <param name="temperatura">The temperatura<see cref="double"/></param>
        /// <param name="CultivoTBase">The CultivoTBase<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double IncrementoTemperatura(double temperatura, double CultivoTBase) => temperatura > CultivoTBase ? temperatura - CultivoTBase : 0;

        /// <summary>
        /// AvisoDrenaje
        /// </summary>
        /// <param name="dp">dp<see cref="double"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool AvisoDrenaje(double dp) => dp > Config.GetDouble("DrenajeUmbral");

        /// <summary>
        /// Cálculo del indice de estrés.
        /// </summary>
        /// <param name="os"></param>
        /// <param name="lo"></param>
        /// <param name="ks"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static double IndiceEstres(double os, double lo, double ks, double cc) {
            if (os > lo)
                return (os - lo) / (cc - lo);
            else
                return ks - 1;
        }

        /// <summary>
        /// Retorna a la tabla de umbrales la clase de estres.
        /// Ordena la tabla por el valor umbral.
        /// Retonar la descripción del maxímo umbral que puede superar el indiceEstes
        /// </summary>
        /// <param name="idTipoEstres">idTipoEstres<see cref="string"/></param>
        /// <param name="indiceEstes">ie<see cref="double"/></param>
        /// <returns><see cref="string"/></returns>
        public static string ClaseEstres(string idTipoEstres, double indiceEstes) {
            string ret = "";
            List<TipoEstresUmbral> ltu = DB.TipoEstresUmbralOrderList(idTipoEstres);
            if (ltu?.Count == 0)
                return ret;
            ret = ltu[0].Descripcion;
            int i = 0;
            while (indiceEstes > ltu[i].IdUmbral) {
                ret = ret = ltu[++i].Descripcion;
            }
            return ret;
        }

        /// <summary>
        /// Devuelve el máximo valor de la tabla de umbrales para el tipo de estres indicado por idTipoEstres
        /// </summary>
        /// <param name="idTipoEstres"></param>
        /// <returns></returns>
        public static double MaxUmbralEstres(string idTipoEstres) {
            List<TipoEstresUmbral> ltu = DB.TipoEstresUmbralOrderList(idTipoEstres);
            return ltu.Max(x => x.Umbral);
        }


        public static double LimiteOptimoRefClima(double lo, double pm) => lo-pm;
        public static double LimiteOptimoFijoRefClima(double loFijo, double pm) => loFijo-pm;
        public static double ContenidoAguaSuelRefPuntoMarchitezMm(double os, double pm) => os-pm;
        public static double PuntoMarchitezRefPuntoMarchitezMm() => 0;
        public static double CapacidadCampoRefPuntoMarchitezMm(double cc, double pm) => cc-pm;


        /// <summary>
        /// CalculaLineaBalance
        /// </summary>
        /// <param name="dh">dh<see cref="UnidadCultivoDatosHidricos"/></param>
        /// <param name="lbAnt">lbAnt<see cref="LineaBalance"/></param>
        /// <param name="fecha">fecha<see cref="DateTime"/></param>
        /// <returns><see cref="LineaBalance"/></returns>
        public static LineaBalance CalculaLineaBalance(UnidadCultivoDatosHidricos dh, LineaBalance lbAnt, DateTime fecha) {
            LineaBalance lb = new LineaBalance {
                Fecha = fecha
            };
            if (lbAnt == null)
                lbAnt = new LineaBalance();
            double temperatura = dh.Temperatura(fecha);
            double incT = IncrementoTemperatura(temperatura, dh.CultivoTBase);
            if (lbAnt?.Fecha == null) incT = 0; // el primero es 0

            UnidadCultivoDatosExtra datoExtra = dh.DatoExtra(fecha);
            // Parámetros de desarrollo del cultivo
            lb.IntegralTermica = (lbAnt.IntegralTermica + incT);
            lb.TasaCrecimientoCobertura = TcCob(lb.IntegralTermica, dh.CultivoIntegralEmergencia, dh.CultivoModCobCoefA, dh.CultivoModCobCoefB, dh.CultivoModCobCoefC);
            lb.Cobertura = Cobertura(lbAnt.Cobertura, lb.TasaCrecimientoCobertura, incT, datoExtra);
            lb.NumeroEtapaDesarrollo = NEtapaDesarrollo(fecha, lb.Cobertura, lbAnt.NumeroEtapaDesarrollo, dh.UnidadCultivoCultivoEtapasList, dh.CultivoEtapasList);
            lb.TasaCrecimientoAltura = TcAlt(lb.IntegralTermica, lb.NumeroEtapaDesarrollo, dh.CultivoIntegralEmergencia, dh.CultivoModAltCoefA, dh.CultivoModAltCoefB, dh.CultivoModAltCoefC);
            lb.AlturaCultivo = Altura(lbAnt.AlturaCultivo, lb.TasaCrecimientoAltura, incT, dh.CultivoModAltCoefC, dh.CultivoAlturaFinal, datoExtra);
            lb.LongitudRaiz = RaizLongitud(lbAnt, incT, dh.CultivoProfRaizInicial, dh.CultivoModRaizCoefB, dh.CultivoProfRaizMax);

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
            lb.Kc = Kc(lb.NumeroEtapaDesarrollo, fecha, lb.Cobertura, dh.UnidadCultivoCultivoEtapasList);
            lb.KcAjustadoClima = KcAdjClima(lb.Kc, lb.AlturaCultivo, dh.VelocidadViento(fecha), dh.HumedadMedia(fecha));

            // Parámetros de estrés en suelo
            lb.FraccionAgotamiento = DepletionFactor(lb.KcAjustadoClima * dh.Eto(fecha), lb.NumeroEtapaDesarrollo, dh.UnidadCultivoCultivoEtapasList);
            lb.AguaFacilmenteExtraible = lb.FraccionAgotamiento * lb.AguaDisponibleTotal; // depletion factor f(ETc)
            lb.AguaFacilmenteExtraibleFija = RAW2(lb.AguaDisponibleTotal, lb.NumeroEtapaDesarrollo, dh.UnidadCultivoCultivoEtapasList); 
            lb.LimiteAgotamiento = (lb.CapacidadCampo - lb.AguaFacilmenteExtraible); // depletion factor f(ETc)
            lb.LimiteAgotamientoFijo = (lb.CapacidadCampo - lb.AguaFacilmenteExtraibleFija); // depletion factor fijo
            lb.CoeficienteEstresHidrico = Ks(lb.AguaDisponibleTotal, lb.AguaFacilmenteExtraible, lb.AgotamientoInicioDia); // K de estrés hídrico

            lb.EtcAjustadoClima = EtcAdj(dh.Eto(fecha), lb.KcAjustadoClima, lb.CoeficienteEstresHidrico); //ETc ajustada por clima y estrés

            lb.DrenajeProfundidad = DrenajeEnProdundidad(lb.EtcAjustadoClima, lb.RiegoEfectivo, lb.LluviaEfectiva, lb.AguaCrecRaiz, lb.AgotamientoInicioDia);
            lb.AgotamientoFinalDia = DriEnd(lb.AguaDisponibleTotal, lb.EtcAjustadoClima, lb.RiegoEfectivo, lb.LluviaEfectiva, lb.AgotamientoInicioDia, lb.DrenajeProfundidad, 0, 0.8, lbAnt, datoExtra);
            lb.ContenidoAguaSuelo = lb.CapacidadCampo - lb.AgotamientoFinalDia;

            double indiceEstres = IndiceEstres(lb.ContenidoAguaSuelo, lb.LimiteAgotamiento, lb.CoeficienteEstresHidrico, lb.CapacidadCampo);
            lb.RecRegMmEfectivos = RecomendacionRiegoMm(lb.AguaFacilmenteExtraible, lb.AguaDisponibleTotal, lb.NumeroEtapaDesarrollo, lb.AgotamientoFinalDia, dh.EtapaInicioRiego, dh.ClaseEstresUmbralInferior(lb.NumeroEtapaDesarrollo, indiceEstres), dh.ClaseEstresUmbralSuperior(lb.NumeroEtapaDesarrollo, indiceEstres));
            lb.RecRegMmReales = lb.RecRegMmEfectivos / dh.EficienciaRiego;
            lb.RecRegHr = lb.RecRegMmReales / dh.Pluviometria;

            lb.CapacidadCampoRefPM = CapacidadCampoRefPuntoMarchitezMm(lb.CapacidadCampo, lb.PuntoMarchitez);
            lb.PuntoMarchitezRefPM = PuntoMarchitezRefPuntoMarchitezMm();
            lb.ContenidoAguaSueloRefPM = ContenidoAguaSuelRefPuntoMarchitezMm(lb.ContenidoAguaSuelo , lb.PuntoMarchitez);
            lb.LimiteAgotamientoRefPM =     LimiteOptimoRefClima(lb.LimiteAgotamiento , lb.PuntoMarchitez);
            lb.LimiteAgotamientoFijoRefPM = LimiteOptimoFijoRefClima(lb.LimiteAgotamientoFijo , lb.PuntoMarchitez);
            return lb;
        }

    }
}
