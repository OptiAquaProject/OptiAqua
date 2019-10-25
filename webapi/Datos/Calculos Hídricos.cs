namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;

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
        /// <param name="it"></param>
        /// <param name="nFase"></param>
        /// <param name="itEmergencia"></param>
        /// <param name="ModCobCoefA"></param>
        /// <param name="ModCobCoefB"></param>
        /// <param name="ModCobCoefC"></param>
        /// <returns></returns>
        public static double CalculaTcCob(double it, int nFase, double itEmergencia, double ModCobCoefA, double ModCobCoefB, double? ModCobCoefC) {
            // ((CobCoefA) * CobCoefB * EXP(-CobCoefB * (C10 - CobCoefC)))/POTENCIA((1+EXP(-CobCoefB*(C10-CobCoefC)));2)
            //     1.- La función necesita conocer el número de Fase y la it de emergencia de la BBDD
            //     2.- Si la it es menor que la de emergencia la planta no ha brotado y no hay cobertura
            //     3.- cuando la planta brota, se calcula la cobertura
            //     4.- se cambia la variable itomprueba si el coeficiente C existe y se aplica fórmula logarítmica o lineal
            double ret;

            if (nFase == 1 && it < itEmergencia) {
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
        /// <param name="nFase"></param>
        /// <param name="itEmergencia"></param>
        /// <param name="ModAltCoefA"></param>
        /// <param name="ModAltCoefB"></param>
        /// <param name="ModAltCoefC"></param>
        /// <returns></returns>
        public static double CalculaTcAlt(double it, int nFase, double itEmergencia, double ModAltCoefA, double ModAltCoefB, double? ModAltCoefC) {
            // ((CobCoefA) * CobCoefB * EXP(-CobCoefB * (C10 - CobCoefC)))/POTENCIA((1+EXP(-CobCoefB*(C10-CobCoefC)));2)
            //     1.- La función necesita conocer el número de Fase y la it de emergencia de la BBDD
            //     2.- Si la it es menor que la de emergencia la planta no ha brotado y no hay cobertura
            //     3.- cuando la planta brota, se calcula la cobertura
            //     4.- se cambia la variable it por it (más correcto conceptualmente)
            //     5.- se añade la rutina que comprueba si el coeficiente C existe y se aplica fórmula logarítmica o lineal
            double ret;

            if (nFase == 1 && it < itEmergencia) {
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
        /// Retorna en Nº de fase en la que nos encontramos en base 1 (1,2,3,4....)
        /// Párametro nFase está en base 1 y la tabla de fases en base 0
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="cobertura"></param>
        /// <param name="currentNFase"></param>
        /// <param name="pUnidadCultivoCultivosFases"></param>
        /// <param name="pCultivoFases"></param>
        /// <returns></returns>
        public static int CalculaNFaseDesarrollo(DateTime fecha, double cobertura, int currentNFase, List<UnidadCultivoCultivoFases> pUnidadCultivoCultivosFases, List<CultivoFases> pCultivoFases) {
            int nFaseBase0 = currentNFase - 1 > 0 ? currentNFase - 1 : 0; // situación anómala
            int ret = currentNFase;
            if (pUnidadCultivoCultivosFases.Count < currentNFase)
                return pUnidadCultivoCultivosFases.Count; // situación anómala

            if (pUnidadCultivoCultivosFases[nFaseBase0].DefinicionPorDias == true) {
                if (pUnidadCultivoCultivosFases[nFaseBase0].FechaFinFaseConfirmada != null) {
                    if (nFaseBase0 + 1 < pUnidadCultivoCultivosFases.Count)
                        //actualizar fecha inicio siguiente fase
                        pUnidadCultivoCultivosFases[nFaseBase0 + 1].FechaInicioFase = ((DateTime)pUnidadCultivoCultivosFases[nFaseBase0].FechaFinFaseConfirmada).AddDays(1);
                    if (fecha > pUnidadCultivoCultivosFases[nFaseBase0].FechaFinFaseConfirmada)
                        ret++;
                } else {
                    if (nFaseBase0 + 1 < pUnidadCultivoCultivosFases.Count) {
                        DateTime fechaInicioSiguientefase = pUnidadCultivoCultivosFases[nFaseBase0].FechaInicioFase.AddDays(pCultivoFases[nFaseBase0].DuracionDiasFase);
                        if (fecha >= fechaInicioSiguientefase) {
                            //actualizar fecha inicio siguiente fase
                            pUnidadCultivoCultivosFases[nFaseBase0 + 1].FechaInicioFase = fechaInicioSiguientefase;
                            ret++;
                        }
                    }
                }
            } else { // definido por integral termica
                if (pUnidadCultivoCultivosFases[nFaseBase0].CobFinal < cobertura) {
                    if (pUnidadCultivoCultivosFases.Count > nFaseBase0 + 1)
                        //actulizar siguiente fecha de inicio siguiente fase
                        pUnidadCultivoCultivosFases[nFaseBase0 + 1].FechaInicioFase = fecha;
                    ret++;
                } else if (pUnidadCultivoCultivosFases[nFaseBase0].FechaFinFaseConfirmada != null) {
                    if (pUnidadCultivoCultivosFases[nFaseBase0].FechaFinFaseConfirmada < fecha) {
                        if (pUnidadCultivoCultivosFases.Count > nFaseBase0 + 1)
                            pUnidadCultivoCultivosFases[nFaseBase0 + 1].FechaInicioFase = ((DateTime)pUnidadCultivoCultivosFases[nFaseBase0].FechaFinFaseConfirmada).AddDays(1);
                        ret++;
                    }
                }
            }
            return ret > pUnidadCultivoCultivosFases.Count ? pUnidadCultivoCultivosFases.Count : ret;
        }

        /// <summary>
        /// Calculo del Coeficiento de cultivo
        /// </summary>
        /// <param name="nFase"></param>
        /// <param name="fecha"></param>
        /// <param name="cob"></param>
        /// <param name="pUnidadCultivoCultivosFases"></param>
        /// <returns></returns>
        public static double CalulaKc(int nFase, DateTime fecha, double cob, List<UnidadCultivoCultivoFases> pUnidadCultivoCultivosFases) {
            double ret = 0;
            int nFaseIndex = nFase - 1 > 0 ? nFase - 1 : 0; // la fase está en base 1
            if (pUnidadCultivoCultivosFases[nFaseIndex].KcInicial == pUnidadCultivoCultivosFases[nFaseIndex].KcFinal) {
                ret = Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseIndex].KcInicial);
            } else {
                if (pUnidadCultivoCultivosFases[nFaseIndex].DefinicionPorDias == true) {
                    double nDias = (fecha - pUnidadCultivoCultivosFases[nFaseIndex].FechaInicioFase).Days;
                    DateTime fechaSigFase = fecha;
                    if (nFaseIndex + 1 < pUnidadCultivoCultivosFases.Count)
                        fechaSigFase = pUnidadCultivoCultivosFases[nFaseIndex + 1].FechaInicioFase;

                    double nDiasFaseActual = (fechaSigFase - pUnidadCultivoCultivosFases[nFaseIndex].FechaInicioFase).Days;
                    if (nDias > nDiasFaseActual)
                        nDias = nDiasFaseActual;
                    ret = Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseIndex].KcInicial + (pUnidadCultivoCultivosFases[nFaseIndex].KcFinal - pUnidadCultivoCultivosFases[nFaseIndex].KcInicial) * nDias / nDiasFaseActual);
                } else {
                    double kcIni = Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseIndex].KcInicial);
                    double cobIni = Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseIndex].CobInicial);
                    double kcFin = Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseIndex].KcFinal);
                    double cobFin = Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseIndex].CobFinal);
                    // calcular cobertura usando formulas tabla cultivo 
                    // usando cobertura cultivo obtener kc
                    ret = kcIni + (cob - cobIni) * (kcFin - kcIni) / (cobFin - cobIni);
                    // si la cobertura actual > cobFin ==> cambio fase
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
        public static double CalculaKcAdjClima(double kc, double tcAlt, double velocidadViento, double humedadMedia) {
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
        public static double CalculaRoot(LineaBalance lbAnt, double it, double profRaizInicial, double modRaizCoefB, double profRaizMax) {
            double ret;
            if (lbAnt.Root == 0) {
                ret = profRaizInicial;
            } else {
                ret = lbAnt.Root + modRaizCoefB * it;
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
        public static double CalculaCC(double root, List<UnidadCultivoSuelo> pUnidadCultivoSuelo) {
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
        /// <param name="nFase"></param>
        /// <param name="pUnidadCultivoCultivosFases"></param>
        /// <returns></returns>
        public static double CalculaP(double etc, int nFase, List<UnidadCultivoCultivoFases> pUnidadCultivoCultivosFases) {
            int nFaseBase0 = nFase > 0 ? nFase - 1 : 0;
            double ret = pUnidadCultivoCultivosFases[nFaseBase0].FactorDeAgotamiento + 0.04 * (5 - etc);
            if (ret < 0.1)
                ret = 0.1;
            if (ret > 0.8)
                ret = 0.8;
            return ret;
        }

        /// <summary>
        /// Caculo de taw
        /// </summary>
        /// <param name="taw"></param>
        /// <param name="nFase"></param>
        /// <param name="pUnidadCultivoCultivosFases"></param>
        /// <returns></returns>
        public static double CalculaRAW2(double taw, int nFase, List<UnidadCultivoCultivoFases> pUnidadCultivoCultivosFases) {
            int nFaseBase0 = nFase > 0 ? nFase - 1 : 0;
            double ret = taw * Convert.ToDouble(pUnidadCultivoCultivosFases[nFaseBase0].FactorDeAgotamiento);
            return ret;
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
        /// Calcula punto de marchitez
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pUnidadCultivoSuelo"></param>
        /// <returns></returns>
        public static double CalculaPM(double root, List<UnidadCultivoSuelo> pUnidadCultivoSuelo) {
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
        public static double CalculaPrecipitacionEfectiva(double precipitacion, double eto) {
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
        public static double CalculaKs(double taw, double raw, double dr) {
            double ret = dr < raw ? 1 : (taw - dr) / (taw - raw);
            return ret;
        }

        /// <summary>
        /// Calcula el Coeficiente de Cultivo ajustado
        /// </summary>
        /// <param name="KcAdjClima"></param>
        /// <param name="ks"></param>
        /// <returns></returns>
        public static double CalculaKcAdj(double KcAdjClima, double ks) {
            double ret = KcAdjClima * ks;
            return ret;
        }

        /// <summary>
        /// Calcula la evotranspiración ajustada
        /// </summary>
        /// <param name="kcAdj"></param>
        /// <param name="eto"></param>
        /// <returns></returns>
        public static double CalculaEtcAdj(double kcAdj, double eto) {
            double ret = kcAdj * eto;
            return ret;
        }

        /// <summary>
        /// Cálculo del riego efectivo
        /// </summary>
        /// <param name="riego"></param>
        /// <param name="eto"></param>
        /// <returns></returns>
        public static double CalculaRiegoEfectivo(double riego, double eto) {
            double ret = riego > 2 ? riego - 0.2 * eto : 0;
            return ret;
        }

        /// <summary>
        /// Drenaje de profundidad
        /// </summary>
        /// <param name="ETcAdj"></param>
        /// <param name="rieEfec"></param>
        /// <param name="pef"></param>
        /// <param name="driStart"></param>
        /// <returns></returns>
        public static double CalculaDP(double ETcAdj, double rieEfec, double pef, double driStart) {
            double ret = rieEfec + pef - ETcAdj - driStart;
            if (ret < 0) ret = 0;
            return ret;
        }

        /// <summary>
        /// Calcula la Cobertura a una fecha dada. Tiene en cuenta si se han indicado datos extra.
        /// </summary>
        /// <param name="antCob"></param>
        /// <param name="tcCob"></param>
        /// <param name="incT"></param>
        /// <param name="datoExtra"></param>
        /// <returns></returns>
        public static double CalculaCobertura(double antCob, double tcCob, double incT, UnidadCultivoDatosExtra datoExtra) {
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
        public static double CalculaAltura(double antAlt, double tcAlt, double incT, double? modAltCoefC, double? alturaFinal, UnidadCultivoDatosExtra datoExtra) {
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
        public static double CalculaAguaAportadaCrecRaiz(double pSaturacion, double tawHoy, double tawAyer) {
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
        public static double CalculaDriEnd(double taw, double EtcAdj, double rieEfec, double pef, double driStart, double dp, double escorrentia, double pSaturacion, LineaBalance lbAnt, UnidadCultivoDatosExtra datoExtra) {
            double ret = 0;
            if (datoExtra?.DriEnd != null) {// si existen datos extra prevalecen sobre los calculados.
                return datoExtra.DriEnd ?? 0;
            }
            if (lbAnt.Fecha == null) {
                ret = (1 - pSaturacion) * taw;
            } else {
                double aguaAportadaCrecRaiz = pSaturacion * (taw - lbAnt.Taw);
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
        public static double CalculaDP(double ETcAdj, double rieEfec, double pef, double aguaAportadaCrecRaiz, double driStart) {
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
        public static double CalculaTcCob(double it, double itEmergencia, double ModCobCoefA, double ModCobCoefB, double? ModCobCoefC) {

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
        /// Calculo de la altura por el método de la Tasa de crecimiento
        /// </summary>
        /// <param name="it"></param>
        /// <param name="itEmergencia"></param>
        /// <param name="ModAltCoefA"></param>
        /// <param name="ModAltCoefB"></param>
        /// <param name="ModAltCoefC"></param>
        /// <returns></returns>
        public static double CalculaTcAlt(double it, double itEmergencia, double ModAltCoefA, double ModAltCoefB, double? ModAltCoefC) {
            double ret;
            if (it < itEmergencia) {
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
        /// CalculaRecomendacionRiegoMm
        /// </summary>
        /// <param name="raw">raw<see cref="double"/></param>
        /// <param name="nFase">nFase<see cref="int"/></param>
        /// <param name="driEnd">driEnd<see cref="double"/></param>
        /// <param name="faseInicioRiego">faseInicioRiego<see cref="int"/></param>
        /// <param name="pAguaMinima">pAguaMinima<see cref="double"/></param>
        /// <returns><see cref="double"/></returns>
        public static double CalculaRecomendacionRiegoMm(double raw, int nFase, double driEnd, int faseInicioRiego, double pAguaMinima) {
            /*
                1- Cambiar el funcionamiento de la función para hacerlo más claro
                2- Añadir las variables faseInicioRiego y pAguaMinima.
                      faseInicioRiego, en el codigo anterior por defecto se regaba siempre que nFase > 1, pensamos que tendría
                      más flexibilidad si se pudiera parametrizar esto, cada cultivo podría tener su propia fase de inicio
                      pAguaMinima, porcentaje de agua a partir del cual se hace recomendable regar, valores recomendados entre 0.1 y 0.8
          */
            double ret = 0;
            if ((nFase >= faseInicioRiego) && (driEnd > raw * pAguaMinima)) { /* comprobar que este condicional es correcto */
                ret = driEnd;
            }
            return ret;
        }

        /// <summary>
        /// Calcula la recomención del Riego en tiempo (Horas)
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="nFase"></param>
        /// <param name="driEnd"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static double CalculaRecomendacionRiegoTpo(double raw, int nFase, double driEnd, int v1, double v2, int v3) => double.NaN;

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
        /// AvisoDrenaje
        /// </summary>
        /// <param name="dp">dp<see cref="double"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool AvisoDrenaje(double dp) => dp > Config.GetDouble("DrenajeUmbral");

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
            double incT = temperatura > dh.CultivoTBase ? temperatura - dh.CultivoTBase : 0;
            if (lbAnt?.Fecha == null) incT = 0; // el primero es 0

            UnidadCultivoDatosExtra datoExtra = dh.DatoExtra(fecha);
            // Parámetros de desarrollo del cultivo
            lb.IT = (lbAnt.IT + incT);
            lb.TcCob = CalculaTcCob(lb.IT, dh.CultivoIntegralEmergencia, dh.CultivoModCobCoefA, dh.CultivoModCobCoefB, dh.CultivoModCobCoefC);
            lb.tcAlt = CalculaTcAlt(lb.IT, dh.CultivoIntegralEmergencia, dh.CultivoModAltCoefA, dh.CultivoModAltCoefB, dh.CultivoModAltCoefC);
            lb.Cob = CalculaCobertura(lbAnt.Cob, lb.TcCob, incT, datoExtra);
            lb.Alt = CalculaAltura(lbAnt.Alt, lb.tcAlt, incT, dh.CultivoModAltCoefC, dh.CultivoAlturaFinal, datoExtra);
            lb.Root = CalculaRoot(lbAnt, incT, dh.CultivoProfRaizInicial, dh.CultivoModRaizCoefB, dh.CultivoProfRaizMax);

            lb.NFase = CalculaNFaseDesarrollo(fecha, lb.Cob, lbAnt.NFase, dh.UnidadCultivoCultivoFasesList, dh.CultivoFasesList);
            lb.Mad = lbAnt.Mad > 0 ? lb.Mad = lbAnt.Mad + 1 : lb.Cob > 0.8 ? 1 : 0;
            lb.EtapaDes = dh.UnidadCultivoCultivoFasesList[lb.NFase - 1].Fase;

            // Parámetros de suelo
            lb.CC = CalculaCC(lb.Root, dh.ListaUcSuelo);
            lb.PM = CalculaPM(lb.Root, dh.ListaUcSuelo);
            lb.Taw = lb.CC - lb.PM;

            // Parámetros de aporte de agua
            lb.lluvia = dh.LluviaMm(fecha);
            lb.Pef = CalculaPrecipitacionEfectiva(lb.lluvia, dh.Eto(fecha));
            lb.Riego = dh.RiegoMm(fecha);
            lb.RieEfec = CalculaRiegoEfectivo(lb.Riego, dh.Eto(fecha));
            lb.AguaCrecRaiz = CalculaAguaAportadaCrecRaiz(0.8, lb.Taw, lbAnt.Taw);

            // Parámetros de cálculo del balance
            lb.DriStart = lbAnt.DriEnd;
            lb.Kc = CalulaKc(lb.NFase, fecha, lb.Cob, dh.UnidadCultivoCultivoFasesList);
            lb.KcAdjClima = CalculaKcAdjClima(lb.Kc, lb.Alt, dh.VelocidadViento(fecha), dh.HumedadMedia(fecha));

            // Parámetros de estrés en suelo
            lb.P = CalculaP(lb.KcAdjClima * dh.Eto(fecha), lb.NFase, dh.UnidadCultivoCultivoFasesList);
            lb.Raw = lb.P * lb.Taw; // depletion factor f(ETc)
            lb.Raw2 = CalculaRAW2(lb.Taw, lb.NFase, dh.UnidadCultivoCultivoFasesList); // depletion factor fijo
            lb.LO = (lb.CC - lb.Raw); // depletion factor f(ETc)
            lb.LOFijo = (lb.CC - lb.Raw2); // depletion factor fijo
            lb.Ks = CalculaKs(lb.Taw, lb.Raw, lb.DriStart); // K de estrés hídrico

            lb.EtcAdj = (dh.Eto(fecha) * lb.KcAdjClima * lb.Ks); //ETc ajustada por clima y estrés

            lb.Dp = CalculaDP(lb.EtcAdj, lb.RieEfec, lb.Pef, lb.AguaCrecRaiz, lb.DriStart);
            lb.DriEnd = CalculaDriEnd(lb.Taw, lb.EtcAdj, lb.RieEfec, lb.Pef, lb.DriStart, lb.Dp, 0, 0.8, lbAnt, datoExtra);
            lb.OS = lb.CC - lb.DriEnd;
            lb.RecRegMm = CalculaRecomendacionRiegoMm(lb.Raw, lb.NFase, lb.DriEnd, 2, 0.8); //!!! Esta función es incorrecta
            lb.RecRegTpo = CalculaRecomendacionRiegoTpo(lb.Raw, lb.NFase, lb.DriEnd, 2, 0.8, 0); //!!! POR DESARROLLAR

            return lb;
        }
    }
}
