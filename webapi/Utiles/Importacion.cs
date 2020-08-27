namespace DatosOptiaqua {
    using Models;
    using NPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static WebApi.ImportacionController;

    /// <summary>
    /// Capa de acceso a las base de datos OptiAqua y Nebula en SQl Server
    /// Para simplificar el acceso se hace uso de la librería NPoco - https://github.com/schotime/NPoco
    /// La cadena de conexión CadenaConexionOptiAqua se define como parámetro de la aplicación.
    /// La cadena de conexión Nebula se define como parámetro de la aplicación.
    /// </summary>
    public static class Importacion {
        /// <summary>
        /// Defines the <see cref="ImportItem" />.
        /// </summary>
        public class ImportItem {
            /// <summary>
            /// Gets or sets the IdUnidadCultivo.
            /// </summary>
            public string IdUnidadCultivo { set; get; }

            /// <summary>
            /// Gets or sets the IdRegante.
            /// </summary>
            public int IdRegante { set; get; }

            /// <summary>
            /// Gets or sets the IdEstacion.
            /// </summary>
            public int IdEstacion { set; get; }

            /// <summary>
            /// Gets or sets the Alias.
            /// </summary>
            public string Alias { set; get; }

            /// <summary>
            /// Gets or sets the IdSueloTipo.
            /// </summary>
            public string IdSueloTipo { set; get; }

            /// <summary>
            /// Gets or sets the IdTemporada.
            /// </summary>
            public string IdTemporada { set; get; }

            /// <summary>
            /// Gets or sets the IdParcelaIntList.
            /// </summary>
            public List<int> IdParcelaIntList { set; get; }

            /// <summary>
            /// Gets or sets the IdCultivo.
            /// </summary>
            public int IdCultivo { set; get; }

            /// <summary>
            /// Gets or sets the FechaSiembra.
            /// </summary>
            public DateTime FechaSiembra { set; get; }

            /// <summary>
            /// Gets or sets the IdTipoRiego.
            /// </summary>
            public int IdTipoRiego { set; get; }

            /// <summary>
            /// Gets or sets the SuperficieM2.
            /// </summary>
            public double? SuperficieM2 { set; get; }
        }

        /// <summary>
        /// Defines the <see cref="ErrorItem" />.
        /// </summary>
        public class ErrorItem {
            /// <summary>
            /// Gets or sets the NLinea.
            /// </summary>
            public int NLinea { set; get; }

            /// <summary>
            /// Gets or sets the Descripcion.
            /// </summary>
            public string Descripcion { set; get; }
        }

        /// <summary>
        /// The Importar.
        /// </summary>
        /// <param name="param">The param<see cref="ImportaPost"/>.</param>
        /// <returns>The <see cref="List{ErrorItem}"/>.</returns>
        internal static List<ErrorItem> Importar(ImportaPost param) {
            string csv = param.CSV;
            int nLinea = 1;
            List<ErrorItem> lErrores = new List<ErrorItem>();
            string[] lineas = csv.Split('\n');
            foreach (string linea in lineas) {
                try {
                    if (linea == lineas.First())
                        continue;
                    if (linea.Length < 10)
                        continue;
                    string[] lItemsLinea = linea.Split(';');
                    ImportItem item = new ImportItem {
                        IdUnidadCultivo = lItemsLinea[0],
                        IdRegante = int.Parse(lItemsLinea[1]),
                        IdEstacion = int.Parse(lItemsLinea[2]),
                        Alias = lItemsLinea[3],
                        IdSueloTipo = lItemsLinea[4],
                        IdTemporada = lItemsLinea[5],
                        IdParcelaIntList = lItemsLinea[6].Split(',').Select(int.Parse).ToList(),
                        IdCultivo = int.Parse(lItemsLinea[7]),
                        FechaSiembra = DateTime.Parse(lItemsLinea[8]),
                        IdTipoRiego = int.Parse(lItemsLinea[9]),
                        SuperficieM2 = double.Parse(lItemsLinea[10])
                    };
                    Importar(item, param.IdTemporada, param.IdTemporadaAnterior);
                } catch (Exception ex) {
                    lErrores.Add(new ErrorItem { NLinea = nLinea, Descripcion = "<small><i>" + linea + "</i></small><br>" + ex.Message });
                }
                nLinea++;
            }
            return lErrores;
        }

        /// <summary>
        /// The Importar.
        /// </summary>
        /// <param name="item">The item<see cref="ImportItem"/>.</param>
        /// <param name="idTemporada">.</param>
        /// <param name="idTemporadaAnterior">The idTemporadaAnterior<see cref="string"/>.</param>
        private static void Importar(ImportItem item, string idTemporada, string idTemporadaAnterior) {
            Database db = DB.ConexionOptiaqua;
            db.BeginTransaction();
            try {
                UnidadCultivo uc = new Models.UnidadCultivo {
                    IdUnidadCultivo = item.IdUnidadCultivo,
                    Alias = item.Alias,
                    IdEstacion = item.IdEstacion,
                    IdRegante = item.IdRegante,
                    TipoSueloDescripcion = item.IdSueloTipo
                };
                db.Save(uc);
                UnidadCultivoParcelaSave(db, item.IdUnidadCultivo, idTemporada, item.IdRegante, item.IdParcelaIntList);

                // Sólo si se indicar un valor la superficie se almacena valor. En caso contrario se calculará porla parcelas indicadas
                if (item.SuperficieM2 == null || item.SuperficieM2 != 0)
                    UnidadCultivoSuperficieSave(db, item.IdUnidadCultivo, idTemporada, (double)item.SuperficieM2);
                {
                    // Si se indica un tipo de suelo se replica el suelo tipo para la nueva temporada.
                    // Si no se indica tipo de suelo se duplica el de la temporada anterior para la nueva temporada.
                    if (string.IsNullOrWhiteSpace(item.IdSueloTipo)) {
                        if (!DB.TemporadaExists(idTemporadaAnterior))
                            throw new Exception("No se indicó IdSueloTipo y la temporada anterior indicada no existe");
                        if (!DB.DuplicarAnteriorSuelo(item.IdUnidadCultivo, idTemporada, idTemporadaAnterior))
                            throw new Exception("No se indicó IdSueloTipo y no se dispone de información del año anterior");
                    } else
                        CultivoSueloSave(db, item.IdUnidadCultivo, idTemporada, item.IdSueloTipo);
                }
                {
                    //Crear la Etapas de crecimiento según el cultivo indicado y al fecha de siembra
                    UnidadCultivoCultivoTemporadaSave(db, item.IdUnidadCultivo, idTemporada, item.IdCultivo, item.IdRegante, item.IdTipoRiego, item.FechaSiembra.ToString());
                }
                db.CompleteTransaction();
            } catch (Exception ex) {
                db.AbortTransaction();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// The UnidadCultivoParcelaSave.
        /// </summary>
        /// <param name="db">The db<see cref="Database"/>.</param>
        /// <param name="idUnidadCultivo">The idUnidadCultivo<see cref="string"/>.</param>
        /// <param name="idTemporada">The idTemporada<see cref="string"/>.</param>
        /// <param name="idRegante">The idRegante<see cref="int"/>.</param>
        /// <param name="lIdParcelaInt">The lIdParcelaInt<see cref="List{int}"/>.</param>
        private static void UnidadCultivoParcelaSave(Database db, string idUnidadCultivo, string idTemporada, int idRegante, List<int> lIdParcelaInt) {
            UnidadCultivoParcela ucp = new UnidadCultivoParcela {
                IdUnidadCultivo = idUnidadCultivo,
                IdTemporada = idTemporada,
                IdParcelaInt = 0,
                IdRegante = idRegante,
            };
            lIdParcelaInt.ForEach(x => {
                ucp.IdParcelaInt = x;
                db.Save(ucp);
            });
        }

        /// <summary>
        /// The UnidadCultivoSuperficieSave.
        /// </summary>
        /// <param name="db">The db<see cref="Database"/>.</param>
        /// <param name="idUnidadCultivo">The idUnidadCultivo<see cref="string"/>.</param>
        /// <param name="idTemporada">The idTemporada<see cref="string"/>.</param>
        /// <param name="superficieM2">The superficieM2<see cref="double"/>.</param>
        private static void UnidadCultivoSuperficieSave(Database db, string idUnidadCultivo, string idTemporada, double superficieM2) {
            UnidadCultivoSuperficie r = new UnidadCultivoSuperficie {
                IdTemporada = idTemporada,
                IdUnidadCultivo = idUnidadCultivo,
                SuperficieM2 = superficieM2
            };
            db.Save(r);
        }

        /// <summary>
        /// The UnidadCultivoCultivoTemporadaSave.
        /// </summary>
        /// <param name="db">The db<see cref="Database"/>.</param>
        /// <param name="IdUnidadCultivo">The IdUnidadCultivo<see cref="string"/>.</param>
        /// <param name="idTemporada">The idTemporada<see cref="string"/>.</param>
        /// <param name="idCultivo">The idCultivo<see cref="int"/>.</param>
        /// <param name="idRegante">The idRegante<see cref="int"/>.</param>
        /// <param name="idTipoRiego">The idTipoRiego<see cref="int"/>.</param>
        /// <param name="fechaSiembra">The fechaSiembra<see cref="string"/>.</param>
        private static void UnidadCultivoCultivoTemporadaSave(Database db, string IdUnidadCultivo, string idTemporada, int idCultivo, int idRegante, int idTipoRiego, string fechaSiembra) {
            try {
                if (DateTime.TryParse(fechaSiembra, out DateTime fs) == false) {
                    throw new Exception("Error. La fecha de siembra no es correcta. ");
                }

                // validar Unidad de cultivo                
                if (db.Exists<UnidadCultivo>(IdUnidadCultivo) == false) {
                    throw new Exception("Error. No existe la unida de cultivo indicada.");
                }

                // validar Cultivo
                if (db.Exists<Cultivo>(idCultivo) == false) {
                    throw new Exception("Error. No existe el cultivo indicado. ");
                }

                // validar Regante
                if (db.Exists<Regante>(idRegante) == false) {
                    throw new Exception("Error. No existe el Regante indicado. ");
                }

                // validar TipoRiego
                if (db.Exists<RiegoTipo>(idTipoRiego) == false) {
                    throw new Exception("Error. No existe el tipo de Riego indicado.");
                }

                //Si existe, se elimina
                db.Execute(" delete from UnidadCultivoCultivo where IdUnidadCultivo=@0 and IdTemporada=@1 and IdCultivo=@2 ", IdUnidadCultivo, idTemporada, idCultivo);

                // Crear Registro Parcelas Cultivo
                UnidadCultivoCultivo uniCulCul = new UnidadCultivoCultivo {
                    IdUnidadCultivo = IdUnidadCultivo,
                    IdCultivo = idCultivo,
                    IdRegante = idRegante,
                    IdTemporada = idTemporada,
                    IdTipoRiego = idTipoRiego,
                    Pluviometria = DB.PluviometriaTipica(idTipoRiego)
                };
                db.Insert(uniCulCul);

                // Leer Cultivo Etapas de IdCultivo
                List<CultivoEtapas> listaCF = db.Fetch<CultivoEtapas>("Select * from CultivoEtapas Where IdCultivo=@0", idCultivo);
                if (listaCF.Count == 0) {
                    throw new Exception("Error. No existe una definición de las Etapas para el cultivo indicado.");
                }

                DateTime fechaEtapa = fs;
                foreach (CultivoEtapas cf in listaCF) {
                    UnidadCultivoCultivoEtapas pcf = new UnidadCultivoCultivoEtapas {
                        IdUnidadCultivo = uniCulCul.IdUnidadCultivo,
                        IdTemporada = uniCulCul.IdTemporada,
                        IdEtapaCultivo = cf.OrdenEtapa,
                        IdTipoEstres = cf.IdTipoEstres,
                        DuracionDiasEtapa = cf.DuracionDiasEtapa,
                        Etapa = cf.Etapa,
                        FechaInicioEtapa = fechaEtapa
                    };
                    fechaEtapa = fechaEtapa.AddDays(cf.DuracionDiasEtapa);
                    pcf.FechaInicioEtapaConfirmada = null;
                    pcf.DefinicionPorDias = cf.DefinicionPorDias;
                    pcf.KcInicial = cf.KcInicial;
                    pcf.KcFinal = cf.KcFinal;
                    pcf.CobInicial = cf.CobInicial;
                    pcf.CobFinal = cf.CobFinal;
                    pcf.FactorDeAgotamiento = cf.FactorAgotamiento;
                    db.Insert(pcf);
                }
                return;
            } catch (Exception ex) {
                throw new Exception("Error. No existe una definición de las Etapas para el cultivo indicado." + ex.Message);
            }
        }

        /// <summary>
        /// The CultivoSueloSave.
        /// </summary>
        /// <param name="db">The db<see cref="Database"/>.</param>
        /// <param name="idUnidadCultivo">The idUnidadCultivo<see cref="string"/>.</param>
        /// <param name="idTemporada">The idTemporada<see cref="string"/>.</param>
        /// <param name="idSueloTipo">The idSueloTipo<see cref="string"/>.</param>
        private static void CultivoSueloSave(Database db, string idUnidadCultivo, string idTemporada, string idSueloTipo) {
            try {
                List<SueloTipo> lSt = db.Fetch<SueloTipo>("Select * from suelotipo where idSueloTipo=@0", idSueloTipo);
                if (lSt.Count == 0)
                    throw new Exception($"El suelo tipo indicado ({idSueloTipo}) no existe");
                foreach (SueloTipo st in lSt) {
                    UnidadCultivoSuelo ucs = new UnidadCultivoSuelo {
                        Arcilla = st.Arcilla,
                        Arena = st.Arena,
                        ElementosGruesos = st.ElementosGruesos,
                        IdHorizonte = st.IdHorizonte,
                        IdUnidadCultivo = idUnidadCultivo,
                        Limo = st.Limo,
                        MateriaOrganica = st.MateriaOrganica,
                        ProfundidadHorizonte = st.Profundidad,
                        IdTemporada = idTemporada
                    };
                    db.Save(ucs);
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
