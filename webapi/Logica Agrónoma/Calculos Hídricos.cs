namespace DatosOptiaqua {
    using Models;
    using System;
    using System.Collections.Generic;
    using Utiles;

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
        /// Segun formula Saxton-Rawls (2006)
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
        /// Calcula de la tasa de cobertura como la direrencia de coberturas entre dos días.
        /// </summary>
        /// <param name="dh"></param>
        /// <param name="lb"></param>
        /// <param name="lbAnt"></param>
        /// <returns></returns>
        public static double TasaCrecimientoCobertura(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) => (lb?.Cobertura ?? 0) - (lbAnt?.Cobertura ?? 0);

        public static int NumeroEtapaDesarrollo(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            int nEtapaBase0 = lbAnt.NumeroEtapaDesarrollo - 1;

            int ret = lbAnt.NumeroEtapaDesarrollo;

            if (dh.UnidadCultivoCultivoEtapasList.Count < lbAnt.NumeroEtapaDesarrollo)
                return dh.UnidadCultivoCultivoEtapasList.Count; // situación anómala

            if (dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].FechaInicioEtapaConfirmada != null) {
                dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].FechaInicioEtapa = (DateTime)dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].FechaInicioEtapaConfirmada;
            }

            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            //bool UsarCoberturaParaCambioFase = paramEtapas.GetBool(nEtapaBase0, "UsarCoberturaParaCambioFase");            
            bool UsarCoberturaParaCambioFase = !dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].DefinicionPorDias;

            if (UsarCoberturaParaCambioFase == true) {
                if (dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].CobFinal < lb.Cobertura) {
                    if (dh.UnidadCultivoCultivoEtapasList.Count > nEtapaBase0 + 1)
                        //actualizar fecha de inicio siguiente etapa
                        dh.UnidadCultivoCultivoEtapasList[nEtapaBase0 + 1].FechaInicioEtapa = (DateTime)lb.Fecha;
                    ret++;
                }
            } else {
                if (nEtapaBase0 + 1 < dh.UnidadCultivoCultivoEtapasList.Count) {
                    DateTime fechaInicioSiguienteEtapa = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].FechaInicioEtapa.AddDays(dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].DuracionDiasEtapa);
                    if (lb.Fecha >= fechaInicioSiguienteEtapa) {
                        //actualizar fecha inicio siguiente etapa
                        dh.UnidadCultivoCultivoEtapasList[nEtapaBase0 + 1].FechaInicioEtapa = fechaInicioSiguienteEtapa;
                        ret++;
                    }
                }
            }
            return ret > dh.UnidadCultivoCultivoEtapasList.Count ? dh.UnidadCultivoCultivoEtapasList.Count : ret;
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
            int nEtapaBase0 = nEtapa - 1; // la etapa está en base 1
            if (unidadCultivoCultivosEtapasList[nEtapaBase0].KcInicial == unidadCultivoCultivosEtapasList[nEtapaBase0].KcFinal) {
                ret = unidadCultivoCultivosEtapasList[nEtapaBase0].KcInicial;
            } else {
                if (unidadCultivoCultivosEtapasList[nEtapaBase0].DefinicionPorDias == true) {
                    DateTime fechaInicioEtapaActual = unidadCultivoCultivosEtapasList[nEtapaBase0].FechaInicioEtapa;
                    if (unidadCultivoCultivosEtapasList[nEtapaBase0].FechaInicioEtapaConfirmada != null)
                        fechaInicioEtapaActual = (DateTime)unidadCultivoCultivosEtapasList[nEtapaBase0].FechaInicioEtapaConfirmada;
                    int nDias = (fecha - fechaInicioEtapaActual).Days;
                    if (nDias < 0)
                        nDias = 0;
                    int diasTeoricosFase = cultivoEtapasList[nEtapaBase0].DuracionDiasEtapa;
                    DateTime fechaFinEtapaActual = fechaInicioEtapaActual.AddDays(diasTeoricosFase);
                    if (nDias > diasTeoricosFase)
                        nDias = diasTeoricosFase;
                    double kcInicial = unidadCultivoCultivosEtapasList[nEtapaBase0].KcInicial;
                    double kcFinal = unidadCultivoCultivosEtapasList[nEtapaBase0].KcFinal;
                    ret = kcInicial + ((kcFinal - kcInicial) * (nDias / (double)diasTeoricosFase));
                } else { // por integral termica
                    double kcIni = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaBase0].KcInicial);
                    double cobIni = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaBase0].CobInicial);
                    double kcFin = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaBase0].KcFinal);
                    double cobFin = Convert.ToDouble(unidadCultivoCultivosEtapasList[nEtapaBase0].CobFinal);
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
        public static double RaizLongitud(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            //ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;
            switch (dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].IdTipoCalculoLongitudRaiz) {
                case 1:// Sin definir formulas
                    return RaizLongitudDefPorDias(dh, lb, lbAnt);
                case 2:
                case 3:
                default:
                    return RaizLongitudDefPorFormulaLineal(dh, lb, lbAnt);
            }

        }

        /// The RaizLongitudDefPorDias.        
        public static double RaizLongitudDefPorDias(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            double antLongRaiz = lbAnt.LongitudRaiz;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            double profRaizInicial = dh.CultivoProfRaizInicial;
            double profRaizMax = dh.CultivoProfRaizMax;

            if (antLongRaiz == 0) {
                ret = profRaizInicial;
            } else {
                ret = antLongRaiz + (profRaizMax - profRaizInicial) / (lb.DiasDesdeSiembra);
            }

            if (ret > profRaizMax) {
                ret = profRaizMax;
            }

            return ret;
        }

        /// <summary>
        /// The RaizLongitudDefPorFormulaLineal
        /// </summary>
        /// <returns>The <see cref="double"/>.</returns>
        public static double RaizLongitudDefPorFormulaLineal(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            double antLongRaiz = lbAnt.LongitudRaiz;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            double profRaizInicial = dh.CultivoProfRaizInicial;
            double profRaizMax = dh.CultivoProfRaizMax;
            double modRaizCoefB = paramCultivo.Get("ModRaizCoefB") ?? double.MaxValue;

            if (antLongRaiz == 0) {
                ret = profRaizInicial;
            } else {
                ret = antLongRaiz + modRaizCoefB * incT;
            }

            if (ret > profRaizMax) {
                ret = profRaizMax;
            }

            return ret;
        }

        /// <summary>
        /// The RaizLongitudDefPorFormulaCuadratica
        /// </summary>
        /// <returns>The <see cref="double"/>.</returns>
        public static double RaizLongitudDefPorFormulaCuadratica(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            double it = lb.IntegralTermica;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            double antLongRaiz = lbAnt.LongitudRaiz;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            double profRaizInicial = dh.CultivoProfRaizInicial;
            double profRaizMax = dh.CultivoProfRaizMax;
            double modRaizCoefA = paramCultivo.Get("ModRaizCoefA") ?? double.MaxValue;
            double modRaizCoefB = paramCultivo.Get("ModRaizCoefB") ?? double.MaxValue;
            double modRaizCoefC = paramCultivo.Get("ModRaizCoefC") ?? double.MaxValue;

            if (antLongRaiz == 0) { //primer dia
                ret = profRaizInicial;
            } else {
                double tasaCrecRaiz = modRaizCoefA * modRaizCoefB * Math.Exp(-modRaizCoefB * (it - modRaizCoefC)) / Math.Pow((1 + Math.Exp(-modRaizCoefB * (it - modRaizCoefC))), 2);
                ret = antLongRaiz + incT * tasaCrecRaiz;
            }

            if (ret > profRaizMax) {
                ret = profRaizMax;
            }


            return ret;
        }


        public static double CoberturaDefPorDias(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            int nDiasduracionEtapaDias = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].DuracionDiasEtapa;
            double? coberturaInicial = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].CobInicial;
            double? coberturaFinal = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].CobFinal;
            double? coberturaMax = paramEtapas.Get(nEtapaBase0, "CoberturaMax");

            if (coberturaInicial != null && coberturaFinal != null)
                ret = lbAnt.Cobertura + ((double)coberturaFinal - (double)coberturaInicial) / nDiasduracionEtapaDias;
            else
                ret = lbAnt.Cobertura; // Este punto no se debería alcanzar !!

            if (coberturaMax != null && ret > coberturaMax)
                ret = (double)coberturaMax;

            return ret;
        }

        public static double CoberturaCrecimientoLineal(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret;

            double it = lb.IntegralTermica;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            //ParametrosEtapasCalculos paramEtapas = dh.UnidadCultivoCultivoEtapasParametrosList;
            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            double itEmergencia = dh.CultivoIntegralEmergencia;
            double? modCobCoefA = paramCultivo.Get("ModCobCoefA") ?? 0;
            double? ModCobCoefB = paramCultivo.Get("ModCobCoefB") ?? double.MaxValue;
            double? CoberturaMax = paramCultivo.Get("CoberturaMax") ?? double.MaxValue;

            it = it - itEmergencia;
            if (it < 0) {
                ret = 0;
            } else {
                if (lbAnt.Cobertura == 0) { // primer dia
                    ret = modCobCoefA + incT * ModCobCoefB ?? 0;
                } else {
                    ret = lbAnt.Cobertura + incT * ModCobCoefB ?? 0;
                }
            }


            if (CoberturaMax != null & ret > CoberturaMax)
                ret = (double)CoberturaMax;
            return ret;
        }

        public static double CoberturaDefPorFormulaCuadratica(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {

            double ret = 0;
            double it = lb.IntegralTermica;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            //ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            double itEmergencia = dh.CultivoIntegralEmergencia;
            double modCobCoefA = paramCultivo.Get("ModCobCoefA") ?? double.MaxValue;
            double modCobCoefB = paramCultivo.Get("ModCobCoefB") ?? double.MaxValue;
            double modCobCoefC = paramCultivo.Get("ModCobCoefC") ?? double.MaxValue;
            double? coberturaMax = paramCultivo.Get("CoberturaMax") ?? double.MaxValue;

            it = it - itEmergencia;
            if (it < 0)
                ret = 0;
            else {
                double tasaCrecCob = modCobCoefA * modCobCoefB * Math.Exp(-modCobCoefB * (it - modCobCoefC)) / Math.Pow((1 + Math.Exp(-modCobCoefB * (it - modCobCoefC))), 2);
                ret = lbAnt.Cobertura + incT * tasaCrecCob;
            }

            if (coberturaMax != null && ret > coberturaMax)
                ret = (double)coberturaMax;
            return ret;
        }


        public static double AlturaDefPorDias(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret;
            double antAlt = lbAnt.AlturaCultivo;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;
            int nDiasDuracionEtapa = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].DuracionDiasEtapa;

            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            double? alturaInicial =  dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].AlturaInicial;
            double? alturaFinal = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].AlturaFinal;

            if (alturaInicial != null && alturaFinal != null)
                ret = antAlt + ((double)alturaFinal - (double)alturaInicial) / nDiasDuracionEtapa;
            else
                ret = antAlt;


            if (alturaFinal != null && alturaFinal > 0 && ret > alturaFinal)
                ret = (double)alturaFinal;
            return ret;
        }

        public static double AlturaDefPorFormulaLineal(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            double it = lb.IntegralTermica;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            double antAlt = lbAnt.AlturaCultivo;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            double? alturaFinal = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].AlturaFinal;
            double itEmergencia = dh.CultivoIntegralEmergencia;
            double modAltCoefA = paramCultivo.Get("ModAltCoefA") ?? 0;
            double modAltCoefB = paramCultivo.Get("ModAltCoefB") ?? 0;

            it = it - itEmergencia;
            if (it < 0) {
                ret = 0;
            } else {
                if (antAlt == 0) { // primer dia
                    ret = modAltCoefA + incT * modAltCoefB; //  ?? 0; En el cálculo de cobertura funciona pero aquí no¿?
                } else {
                    ret = antAlt + incT * modAltCoefB; // ?? 0; En el cálculo de cobertura funciona pero aquí no¿?
                }
            }


            if (alturaFinal != null && alturaFinal > 0 && ret > alturaFinal)
                ret = (double)alturaFinal;

            return ret;
        }

        public static double AlturaDefPorFormulaCuadratica(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            // (double antAlt, int nEtapa, double it, double itEmergencia, double ModAltCoefA, double ModAltCoefB, double ModAltCoefC, double? alturaFinal, UnidadCultivoDatosExtra datoExtra)

            double ret = 0;
            double it = lb.IntegralTermica;
            double incT = lb.IntegralTermica - lbAnt.IntegralTermica;
            double antAlt = lbAnt.AlturaCultivo;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;

            ParametrosCultivoCalculos paramCultivo = dh.ParametrosCultivo;
            ParametrosEtapasCalculos paramEtapas = dh.ParametrosEtapas;
            double? alturaFinal = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].AlturaFinal;
            double itEmergencia = dh.CultivoIntegralEmergencia;
            double modAltCoefA = paramEtapas.Get(nEtapaBase0, "ModAltCoefA") ?? double.MaxValue;
            double modAltCoefB = paramEtapas.Get(nEtapaBase0, "ModAltCoefB") ?? double.MaxValue;
            double modAltCoefC = paramEtapas.Get(nEtapaBase0, "ModAltCoefC") ?? double.MaxValue;

            it = it - itEmergencia;
            if (it < 0) {
                ret = 0;
            } else {
                double tasaCrecAlt = modAltCoefA * modAltCoefB * Math.Exp(-modAltCoefB * (it - modAltCoefC)) / Math.Pow((1 + Math.Exp(-modAltCoefB * (it - modAltCoefC))), 2);
                ret = lbAnt.AlturaCultivo + incT * tasaCrecAlt;
            }
            //}
            if (alturaFinal != null && alturaFinal > 0 && ret > alturaFinal)
                ret = (double)alturaFinal;

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
            int nEtapaBase0 = nEtapa - 1;
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
            int nEtapaBase0 = nEtapa - 1;
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


        public static double Cobertura(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1; //!!!
            UnidadCultivoDatosExtra datoExtra = dh.DatoExtra((DateTime)lb.Fecha);
            if (datoExtra?.Cobertura != null) {
                ret = (double)datoExtra.Cobertura;
            } else {
                switch (dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].IdTipoCalculoCobertura) {
                    case 1:// Sin definir formulas
                        ret = CoberturaDefPorDias(dh, lb, lbAnt);
                        break;
                    case 2:// Lineal
                        ret = CoberturaCrecimientoLineal(dh, lb, lbAnt);
                        break;
                    case 3:// Cuadrática
                        ret = CoberturaDefPorFormulaCuadratica(dh, lb, lbAnt);
                        break;
                    default:
                        ret = double.MinValue; // Error, valores grandes ayudarán a depurar errores;
                        break;
                }
            }
            if (ret > 1)
                ret = 1;
            return ret;
        }


        public static double Altura(UnidadCultivoDatosHidricos dh, LineaBalance lb, LineaBalance lbAnt) {
            double ret = 0;
            int nEtapaBase0 = lb.NumeroEtapaDesarrollo - 1;
            UnidadCultivoDatosExtra datoExtra = dh.DatoExtra((DateTime)lb.Fecha);
            if (datoExtra?.Altura != null) {
                ret = (double)datoExtra.Altura;
            } else {
                switch (dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].IdTipoCalculoAltura) {
                    case 1:// Sin definir formulas
                        ret = AlturaDefPorDias(dh, lb, lbAnt);
                        break;
                    case 2:// Lineal
                        ret = AlturaDefPorFormulaLineal(dh, lb, lbAnt);
                        break;
                    case 3:// Cuadrática
                        ret = AlturaDefPorFormulaCuadratica(dh, lb, lbAnt);
                        break;
                    default:
                        ret = double.MinValue;
                        break;
                }
            }

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

        public static double ConvertirIndiceEstresEnContenidoAguaDr(double indiceEstres, double raw, double taw, double CC, double PM) {
            double drIndiceEstres = 0;
            if (indiceEstres < 0)
                drIndiceEstres = taw - (1 + indiceEstres) * (taw - raw);
            else
                drIndiceEstres = raw * (1 - indiceEstres);
            return (drIndiceEstres);

        }

        public static double ConvertirIndiceEstresEnContenidoAguaRefZero(double indiceEstres, double raw, double taw, double CC, double PM) {
            double drIndiceEstres = 0;
            if (indiceEstres < 0)
                drIndiceEstres = taw - (1 + indiceEstres) * (taw - raw);
            else
                drIndiceEstres = raw * (1 - indiceEstres);
            return (CC - drIndiceEstres);
        }

        public static double ConvertirIndiceEstresEnContenidoAguaRefPM(double indiceEstres, double raw, double taw, double CC, double PM) {
            double drIndiceEstres = 0;
            if (indiceEstres < 0)
                drIndiceEstres = taw - (1 + indiceEstres) * (taw - raw);
            else
                drIndiceEstres = raw * (1 - indiceEstres);
            return (CC - drIndiceEstres - PM);
        }

        /// <summary>
        /// Calculo de la recomentación de riego en mm.
        /// </summary>
        /// <param name="raw">The raw<see cref="double"/>.</param>
        /// <param name="taw">The taw<see cref="double"/>.</param>
        /// <param name="nEtapa">The nEtapa<see cref="int"/>.</param>
        /// <param name="driEnd">The driEnd<see cref="double"/>.</param>
        /// <param name="seAplicaRiegoEnEtapa">The etapaInicioRiego<see cref="int"/>.</param>
        /// <param name="ieUmbralRiego">The ieUmbralRiego<see cref="double"/>.</param>
        /// <param name="ieLimiteRiego">The ieLimiteRiego<see cref="double"/>.</param>
        /// <returns>.</returns>
        public static double RecomendacionRiegoMm(double raw, double taw, int nEtapa, double driEnd, bool seAplicaRiegoEnEtapa, double ieUmbralRiego, double ieLimiteRiego) {
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
            if (seAplicaRiegoEnEtapa == true && driEnd > drUmbralRiego) {
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
        public static double IncrementoTemperaturaVIEJA(double temperatura, double CultivoTBase, bool definicionPorDias) {
            // ANTES=> temperatura > CultivoTBase ? temperatura - CultivoTBase : 0;
            double ret;
            if (CultivoTBase < 0) { // old: if(definicionPorDias) {// propuestaSIAR PARECE COMPLICADO.... ESTO FUE UN APAÑO Y CON ESTA SOLUCION ESTÁ FALSEADO Y NO ES CORRECTO. Sería más conveniente cambiar las funciones de cálculo de cobertura y altura para que no use la integral térmica (IT) si no hay fórmula, en lugar de cálcular una IT falsa
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
        /// Incremento de temperatura efectivo.
        /// </summary>
        /// <param name="temperatura">The temperatura<see cref="double"/>.</param>
        /// <param name="cultivoTBase">The CultivoTBase<see cref="double?"/>.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double IncrementoTemperatura(double temperatura, double? cultivoTBase) {
            double? ret = 0;
            if (cultivoTBase != null || cultivoTBase >= 0)
                ret = temperatura > cultivoTBase ? temperatura - cultivoTBase : 0;
            return ret ?? 0;
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
            int nEtapaBase0 = lbAnt.NumeroEtapaDesarrollo - 1;
            LineaBalance lb = new LineaBalance {
                Fecha = fecha
            };
            if (lbAnt == null)
                lbAnt = new LineaBalance();
            lb.NumeroEtapaDesarrollo = lbAnt.NumeroEtapaDesarrollo;// por defecto el valor anterior
            double temperatura = dh.Temperatura(fecha);

            bool definicionPorDias = dh.UnidadCultivoCultivoEtapasList[nEtapaBase0].DefinicionPorDias;

            double incT = 0;
            if (lbAnt?.Fecha != null)
                incT = IncrementoTemperatura(temperatura, dh.CultivoTBase);

            UnidadCultivoDatosExtra datoExtra = dh.DatoExtra(fecha);

            lb.IntegralTermica = (lbAnt.IntegralTermica + incT);

            bool definicionEtapaPorDias = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo - 1].DefinicionPorDias;
            int nDiasduracionEtapaDias = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo - 1].DuracionDiasEtapa;
            double? coberturaInicial = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo - 1].CobInicial;
            double? coberturaFinal = dh.UnidadCultivoCultivoEtapasList[lbAnt.NumeroEtapaDesarrollo - 1].CobFinal;

            lb.Cobertura = Cobertura(dh, lb, lbAnt);

            lb.NumeroEtapaDesarrollo = NumeroEtapaDesarrollo(dh, lb, lbAnt);
            lb.AlturaCultivo = Altura(dh, lb, lbAnt); //Altura(lbAnt.AlturaCultivo, lb.TasaCrecimientoAltura, incT, dh.CultivoAlturaFinal, datoExtra);
            lb.LongitudRaiz = RaizLongitud(dh, lb, lbAnt);

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

            bool seAplicaRiego = dh.UnidadCultivoCultivoEtapasList[lb.NumeroEtapaDesarrollo - 1].SeAplicaRiego ?? false;
            lb.RecomendacionRiegoNeto = RecomendacionRiegoMm(lb.AguaFacilmenteExtraible, lb.AguaDisponibleTotal, lb.NumeroEtapaDesarrollo, lb.AgotamientoFinalDia, seAplicaRiego, limiteInferior, limiteSuperior);
            lb.RecomendacionRiegoBruto = lb.RecomendacionRiegoNeto / dh.EficienciaRiego;
            lb.RecomendacionRiegoTiempo = lb.RecomendacionRiegoBruto / dh.Pluviometria;

            lb.CapacidadCampoRefPM = CapacidadCampoRefPuntoMarchitezMm(lb.CapacidadCampo, lb.PuntoMarchitez);
            lb.PuntoMarchitezRefPM = PuntoMarchitezRefPuntoMarchitezMm();
            lb.ContenidoAguaSueloRefPM = ContenidoAguaSuelRefPuntoMarchitezMm(lb.ContenidoAguaSuelo, lb.PuntoMarchitez);
            lb.LimiteAgotamientoRefPM = LimiteOptimoRefClima(lb.LimiteAgotamiento, lb.PuntoMarchitez);
            lb.LimiteAgotamientoFijoRefPM = LimiteOptimoFijoRefClima(lb.LimiteAgotamientoFijo, lb.PuntoMarchitez);

            lb.TasaCrecimientoCobertura = TasaCrecimientoCobertura(dh, lb, lbAnt);
            lb.TasaCrecimientoAltura = lb.AlturaCultivo - lbAnt.AlturaCultivo;

            lb.UmbralSuperiorRiegoOptimoRefPM = ConvertirIndiceEstresEnContenidoAguaRefPM(dh.UmbralSuperiorRiego(lb.NumeroEtapaDesarrollo), lb.AguaFacilmenteExtraible, lb.AguaDisponibleTotal, lb.CapacidadCampo, lb.PuntoMarchitez);
            lb.UmbralInferiorRiegoOptimoRefPM = ConvertirIndiceEstresEnContenidoAguaRefPM(dh.UmbralOptimoRiego(lb.NumeroEtapaDesarrollo), lb.AguaFacilmenteExtraible, lb.AguaDisponibleTotal, lb.CapacidadCampo, lb.PuntoMarchitez);

            return lb;
        }


    }
}
