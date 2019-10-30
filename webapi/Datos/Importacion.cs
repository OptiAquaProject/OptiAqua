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
        /// Defines the <see cref="ImportItem" />
        /// </summary>
        public class ImportItem {
            /// <summary>
            /// Gets or sets the IdUnidadCultivo
            /// </summary>
            public string IdUnidadCultivo { set; get; }

            /// <summary>
            /// Gets or sets the IdRegante
            /// </summary>
            public int IdRegante { set; get; }

            /// <summary>
            /// Gets or sets the IdEstacion
            /// </summary>
            public int IdEstacion { set; get; }

            /// <summary>
            /// Gets or sets the Alias
            /// </summary>
            public string Alias { set; get; }

            /// <summary>
            /// Gets or sets the IdSueloTipo
            /// </summary>
            public string IdSueloTipo { set; get; }

            /// <summary>
            /// Gets or sets the IdParcelaInt
            /// </summary>
            public List<int> IdParcelaIntList { set; get; }

            /// <summary>
            /// Gets or sets the IdCultivo
            /// </summary>
            public int IdCultivo { set; get; }

            /// <summary>
            /// Gets or sets the FechaSiembra
            /// </summary>
            public DateTime FechaSiembra { set; get; }

            public int IdTipoRiego { set; get; }

            /// <summary>
            /// Gets or sets the SuperficieM2
            /// </summary>
            public double SuperficieM2 { set; get; }
        }

        /// <summary>
        /// Defines the <see cref="ErrorItem" />
        /// </summary>
        public class ErrorItem {
            /// <summary>
            /// Gets or sets the NLinea
            /// </summary>
            public int NLinea { set; get; }

            /// <summary>
            /// Gets or sets the Descripcion
            /// </summary>
            public string Descripcion { set; get; }
        }

        /// <summary>
        /// The Importar
        /// </summary>
        /// <param name="param">The param<see cref="ImportaPost"/></param>
        /// <returns>The <see cref="List{ErrorItem}"/></returns>
        internal static List<ErrorItem> Importar(ImportaPost param) {
            DB.TypeModo saveMode = DB.Modo;
            if (param.ModoPruebas)
                DB.Modo = DB.TypeModo.Pruebas;
            else
                DB.Modo = DB.TypeModo.Real;
            string csv = param.CSV;
            int nLinea = 1;
            List<ErrorItem> lErrores = new List<ErrorItem>();
            string[] lineas = csv.Split('\n');
            foreach (string linea in lineas) {
                try {
                    if (linea == lineas.First())
                        continue;
                    string[] lItemsLinea = linea.Split(';');
                    ImportItem item = new ImportItem {
                        IdUnidadCultivo = lItemsLinea[0],
                        IdRegante = int.Parse(lItemsLinea[1]),
                        IdEstacion = int.Parse(lItemsLinea[2]),
                        Alias = lItemsLinea[3],
                        IdSueloTipo = lItemsLinea[4],                        
                        IdParcelaIntList = lItemsLinea[5].Split(',').Select(int.Parse).ToList(),
                        IdCultivo = int.Parse(lItemsLinea[6]),
                        FechaSiembra = DateTime.Parse(lItemsLinea[7]),
                        SuperficieM2 = double.Parse(lItemsLinea[8])
                    };
                    Importar(item,param.IdTemporada, param.IdTemporadaAnterior);
                } catch (Exception ex) {
                    lErrores.Add(new ErrorItem { NLinea = nLinea, Descripcion = ex.Message });
                }
                nLinea++;
            }
            DB.Modo = saveMode;
            return lErrores;
        }

        /// <summary>
        /// The Importar
        /// </summary>
        /// <param name="item">The item<see cref="ImportItem"/></param>
        /// <param name="idTemporada"></param>
        /// <param name="idTemporadaAnterior">The idTemporadaAnterior<see cref="string"/></param>
        private static void Importar(ImportItem item,string idTemporada, string idTemporadaAnterior) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            db.BeginTransaction();
            try {
                var uc = new Models.UnidadCultivo {
                    IdUnidadCultivo = item.IdUnidadCultivo,
                    Alias = item.Alias,
                    IdEstacion = item.IdEstacion,
                    IdRegante = item.IdRegante,
                    TipoSueloDescripcion = item.IdSueloTipo
                };
                db.Save(uc);
                DB.UnidadCultivoParcelaSave(item.IdUnidadCultivo, idTemporada, item.IdRegante, item.IdParcelaIntList);                

                // Sólo si se indicar un valor la superficie se almacena valor. En caso contrario se calculará porla parcelas indicadas
                if (item.SuperficieM2 != 0)
                    DB.UnidadCultivoSuperficieSave(item.IdUnidadCultivo, idTemporada, item.SuperficieM2);
                {
                    // Si se indica un tipo de suelo se replica el suelo tipo para la nueva temporada.
                    // Si no se indica tipo de suelo se duplica el de la temporada anterior para la nueva temporada.
                    if (string.IsNullOrWhiteSpace(item.IdSueloTipo))
                        if (!DB.DuplicarAnteriorSuelo(item.IdUnidadCultivo, idTemporada, idTemporadaAnterior))
                            throw new Exception("No se indicó IdSueloTipo y no se dispone de información del año anterior");
                        else
                            DB.CultivoSueloSave(item.IdUnidadCultivo, idTemporada, item.IdSueloTipo);
                }
                {
                    //Crear la fases de crecimiento según el cultivo indicado y al fecha de siembra
                    DB.UnidadCultivoCultivoTemporadaSave(item.IdUnidadCultivo, idTemporada, item.IdCultivo, item.IdRegante, item.IdTipoRiego, item.FechaSiembra.ToString());
                }
                db.CompleteTransaction();
            } catch (Exception ex) {
                db.AbortTransaction();
                throw new Exception(ex.Message);
            }
        }
    }
}
