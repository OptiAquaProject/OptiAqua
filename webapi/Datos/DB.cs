namespace DatosOptiaqua {
    using Models;
    using NPoco;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using webapi;
    using webapi.Utiles;

    /// <summary>
    /// Capa de acceso a las base de datos OptiAqua y Nebula en SQl Server
    /// Para simplificar el acceso se hace uso de la librería NPoco - https://github.com/schotime/NPoco
    /// La cadena de conexión CadenaConexionOptiAqua se define como parámetro de la aplicación.
    /// La cadena de conexión Nebula se define como parámetro de la aplicación.
    /// </summary>
    public static class DB {
        /// <summary>
        /// Defines the TypeModo
        /// </summary>
        public enum TypeModo { /// <summary>
                               /// Defines the Real
                               /// </summary>
            Real,
            /// <summary>
            /// Defines the Pruebas
            /// </summary>
            Pruebas
        };

        /// <summary>
        /// Gets or sets the Modo
        /// </summary>
        public static TypeModo Modo { set; get; } = TypeModo.Real;

        /// <summary>
        /// Gets the CadenaConexionOptiAqua
        /// </summary>
        public static string CadenaConexionOptiAqua => Modo == TypeModo.Real ? "CadenaConexionOptiAqua" : "CadenaConexionOptiAquaPruebas";
        public static string CadenaConexionNebula => "CadenaConexionNebula";

        public static Database ConexionOptiaqua => new Database(CadenaConexionOptiAqua);

        /// <summary>
        /// The IsCorrectPassword
        /// </summary>
        /// <param name="login">The login<see cref="LoginRequest"/></param>
        /// <param name="regante">The regante<see cref="Regante"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsCorrectPassword(LoginRequest login, out Regante regante) {
            regante = null;
            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                regante = db.SingleOrDefault<Regante>("select * from regante where nif=@0", login.NifRegante);
                if (regante == null)
                    return false;
                string pass1 = BuildPassword(login.NifRegante, login.Password);
                return regante.Contraseña == pass1;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Retorna si el password es correcto para en nif indicado.
        /// </summary>
        /// <param name="nif">The nif<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsCorrectPassword(string nif, string password) {
            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                Regante regante = db.SingleOrDefault<Regante>("select * from regante where nif=@0", nif);
                if (regante == null)
                    return false;
                string pass1 = BuildPassword(nif, password);
                return regante.Contraseña == pass1;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Retorna lista de avisos con los filtros indicados según los parámetros. 
        /// Pasar parámetro con valor '' si no se desea filtrar por campo
        /// </summary>
        /// <param name="idAviso">idAviso<see cref="string"/></param>
        /// <param name="idAvisoTipo">idAvisoTipo<see cref="int?"/></param>
        /// <param name="fInicio">fInicio<see cref="DateTime?"/></param>
        /// <param name="fFin">fFin<see cref="DateTime?"/></param>
        /// <param name="de">de<see cref="string"/></param>
        /// <param name="para">para<see cref="string"/></param>
        /// <param name="search">search<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object AvisosList(string idAviso, int? idAvisoTipo, DateTime? fInicio, DateTime? fFin, string de, string para, string search) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            idAviso = idAviso.Quoted();
            string strFInicio = fInicio?.ToString().Quoted() ?? "''";
            string strFFin = fFin?.ToString().Quoted() ?? "''";
            string strIdAvisoTipo = idAvisoTipo?.ToString() ?? "''";
            de = de.Quoted();
            para = para.Quoted();
            search = search.Quoted();
            string sql = $"SELECT * FROM AvisosList({idAviso},{strIdAvisoTipo},{strFInicio},{strFFin},{de},{para},{search})";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// Crea y almacena contraseña por defecto para regante.
        /// </summary>
        public static void CrearPassw() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<Regante> lr = db.Fetch<Regante>("select * from regante");
            foreach (Regante r in lr) {
                if (r.Role != "admin") {
                    string pass1 = BuildPassword(r.NIF, "Pass" + r.IdRegante.ToString());
                    r.Contraseña = pass1;
                    r.Role = "dbo";
                    db.Save(r);
                }
            }
        }

        /// <summary>
        /// Crear contraseña encriptada a partir del nif del regante
        /// </summary>
        /// <param name="nifRegante">nifRegante<see cref="string"/></param>
        /// <param name="password">password<see cref="string"/></param>
        /// <returns><see cref="string"/></returns>
        public static string BuildPassword(string nifRegante, string password) {
            if (string.IsNullOrEmpty(nifRegante)) {
                nifRegante = "0000000000000";
            }
            string cpass = Encriptacion.XorIt(nifRegante, password) + "0000000000000";
            cpass = cpass.Substring(1, 12); // 12 como máximo            
            string ret = Encriptacion.Encripta(cpass);
            return ret;
        }

        /// <summary>
        /// Crear o actualizar datos de temporada
        /// </summary>
        /// <param name="temporada">param<see cref="Temporada"/></param>
        /// <returns><see cref="object"/></returns>
        public static object TemporadaSave(Temporada temporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            db.Save(temporada);
            return "OK";
        }

        /// <summary>
        /// GeoLocParcelasList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="List{GeoLocParcela}"/></returns>
        public static List<GeoLocParcela> GeoLocParcelasList(string idUnidadCultivo, string idTemporada) {
            List<GeoLocParcela> ret = new List<GeoLocParcela>();
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "SELECT dbo.Parcela.IdParcelaInt, dbo.Parcela.IdMunicipio, dbo.Parcela.IdPoligono, dbo.Parcela.IdParcela, Parcela.GEO.ToString() AS Geo, dbo.Municipio.Municipio, dbo.Parcela.GID ";
            sql += " FROM dbo.Parcela INNER JOIN dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt = dbo.UnidadCultivoParcela.IdParcelaInt INNER JOIN ";
            sql += " dbo.Municipio ON dbo.Parcela.IdMunicipio = dbo.Municipio.IdMunicipio ";
            sql += "WHERE dbo.UnidadCultivoParcela.IdUnidadCultivo=@0 AND dbo.UnidadCultivoParcela.IdTemporada=@1 ";
            ret = db.Fetch<GeoLocParcela>(sql, idUnidadCultivo, idTemporada);
            return ret;
        }

        /// <summary>
        /// ParcelasList
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public static object ParcelasList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select IdParcelaInt, IdRegante, Descripcion, SuperficieM2 from parcela";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// UnidadesDeCultivoList
        /// </summary>
        /// <param name="lTemporadas">idTemporada<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object UnidadesDeCultivoList(List<string> lTemporadas) {
            List<UnidadCultivoConSuperficieYGeoLoc> ret = new List<UnidadCultivoConSuperficieYGeoLoc>();
            Database db = new Database(DB.CadenaConexionOptiAqua);
            foreach (string idTemporada in lTemporadas) {
                string sql = "Select DISTINCT IdUnidadCultivo, IdParcelaInt, IdRegante from UnidadCultivoParcela where IdTemporada=@0 ";
                List<UnidadCultivoConSuperficieYGeoLoc> deUnaTemporada = db.Fetch<UnidadCultivoConSuperficieYGeoLoc>(sql, idTemporada);
                foreach (UnidadCultivoConSuperficieYGeoLoc item in deUnaTemporada) {
                    List<UnidadDeCultivoParcelasValvulas> lValvulas = DB.UnidadCultivoParcelasValculas(item.IdUnidadCultivo, idTemporada);
                    item.ParcelasValvulasJson = Newtonsoft.Json.JsonConvert.SerializeObject(lValvulas);
                    item.SuperficieM2 = UnidadCultivoExtensionM2(item.IdUnidadCultivo, idTemporada);
                    List<GeoLocParcela> lGeoLocParcelas = DB.GeoLocParcelasList(item.IdUnidadCultivo, idTemporada);
                    item.GeoLocJson = Newtonsoft.Json.JsonConvert.SerializeObject(lGeoLocParcelas);
                    ret.Add(item);
                }
            }
            return ret;
        }

        /// <summary>
        /// Devuele el registro TipoEstres inficado por su identificador.
        /// </summary>
        /// <param name="idTipoEstres">The idTipoEstres<see cref="string"/></param>
        /// <returns>The <see cref="TipoEstres"/></returns>
        internal static TipoEstres TipoEstres(string idTipoEstres) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            TipoEstres ret = db.SingleById<TipoEstres>(idTipoEstres);
            return ret;
        }

        /// <summary>
        /// UnidadCultivoTemporadaCosteM3Agua
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="double?"/></returns>
        public static double UnidadCultivoTemporadaCosteM3Agua(string idUnidadCultivo, string idTemporada) {
            if (string.IsNullOrEmpty(idTemporada))
                return 0;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select CosteM3Agua from UnidadCultivoTemporadaCosteAgua where idUnidadCultivo=@0  and IdTemporada=@1;";
            double? ret = db.SingleOrDefault<double?>(sql, idUnidadCultivo, idTemporada);
            if (ret == null) {
                sql = "Select CosteM3Agua from Temporada where IdTemporada=@0;";
                ret = db.SingleOrDefault<double?>(sql, idTemporada);
            }
            return ret ?? 0;
        }

        internal static Dictionary<string, TipoEstres> ListaTipoEstres() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<TipoEstres> ret = db.Fetch<TipoEstres>();
            return ret.ToDictionary(x => x.IdTipoEstres);
        }

        internal static Dictionary<string, List<TipoEstresUmbral>> ListaEstresUmbral() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<TipoEstresUmbral> lUmbrales = db.Fetch<TipoEstresUmbral>();
            Dictionary<string, List<TipoEstresUmbral>> ret = new Dictionary<string, List<TipoEstresUmbral>>();
            foreach (TipoEstresUmbral umbral in lUmbrales) {
                if (!ret.Keys.Contains(umbral.IdTipoEstres))
                    ret.Add(umbral.IdTipoEstres, new List<TipoEstresUmbral>());
                ret[umbral.IdTipoEstres].Add(umbral);
            }
            return ret;
        }


        /// <summary>
        /// UnidadCultivoTemporadaCosteM3AguaSave
        /// </summary>
        /// <param name="param">param<see cref="ParamPostCosteM3Agua"/></param>
        /// <returns><see cref="object"/></returns>
        public static object UnidadCultivoTemporadaCosteM3AguaSave(ParamPostCosteM3Agua param) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            ParamPostCosteM3Agua reg = db.SingleOrDefaultById<ParamPostCosteM3Agua>(param);
            if (param.CosteM3Agua == null)
                db.Delete(param);
            else
                db.Save(param);
            return "OK";
        }

        /// <summary>
        /// ParcelasList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object ParcelasList(string idUnidadCultivo, string idTemporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "SELECT DISTINCT dbo.Parcela.IdParcelaInt, dbo.Parcela.Descripcion, dbo.UnidadCultivoParcela.IdRegante, dbo.Parcela.SuperficieM2, dbo.UnidadCultivoParcela.IdUnidadCultivo ";
            sql += " FROM dbo.UnidadCultivoParcela INNER JOIN ";
            sql += " dbo.Parcela ON dbo.UnidadCultivoParcela.IdParcelaInt = dbo.Parcela.IdParcelaInt";
            sql += " WHERE dbo.UnidadCultivoParcela.IdUnidadCultivo = @0 AND dbo.UnidadCultivoParcela.IdTemporada=@1";
            return db.Fetch<object>(sql, idUnidadCultivo, idTemporada);
        }

        /// <summary>
        /// Duplicar para la temporada indicada los datos de suelo de la temporada anterior
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="idTemporada"></param>
        /// <param name="idTemporadaAnterior"></param>;
        /// <returns>The <see cref="bool"/></returns>
        public static bool DuplicarAnteriorSuelo(string idUnidadCultivo, string idTemporada, string idTemporadaAnterior) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<UnidadCultivoSuelo> lSt = db.Fetch<UnidadCultivoSuelo>("Select * from UnidadCultivoSuelo where idUnidadCultivo=@0 and idTemporada=@1 ", idUnidadCultivo, idTemporadaAnterior);
            if (lSt.Count == 0)
                return false;
            foreach (UnidadCultivoSuelo st in lSt) {
                st.IdTemporada = idTemporada;
                db.Save(st);
            }
            return true;
        }

        /// <summary>
        /// Guardar en la temporada actual los horizontes definicios en el suelo tipo.
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idSueloTipo">idSueloTipo<see cref="string"/></param>
        public static void CultivoSueloSave(string idUnidadCultivo, string idTemporada, string idSueloTipo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            try {
                db.BeginTransaction();
                List<SueloTipo> lSt = db.Fetch<SueloTipo>("Select * from suelotipo where idSueloTipo=@0", idSueloTipo);
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
                UnidadCultivo uc = DB.UnidadCultivo(idUnidadCultivo);
                uc.TipoSueloDescripcion = idSueloTipo;
                db.Save(uc);
                db.CompleteTransaction();
            } catch {
                db.AbortTransaction();
                throw new Exception("No se puedo crear suelo para la unidad de cultivo.");
            }
        }

        /// <summary>
        /// DatosRiegosList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object DatosRiegosList(string idUnidadCultivo, string idTemporada) {
            List<DatosRiego> redDatosRiegos = new List<DatosRiego>();
            Temporada t = Temporada(idTemporada);
            UnidadCultivo uc = UnidadCultivo(idUnidadCultivo);
            UnidadCultivoCultivo ucc = UnidadCultivoCultivo(idUnidadCultivo, idTemporada);

            DateTime desdeFecha = ucc?.FechaSiembra ?? t.FechaInicial;
            DateTime hastaFecha = t.FechaFinal < DateTime.Today ? DateTime.Today : t.FechaFinal;


            List<Riego> lRiegos = RiegosList(idUnidadCultivo, desdeFecha, hastaFecha);

            double superficie = DB.UnidadCultivoExtensionM2(idUnidadCultivo, idTemporada) / 1000;
            if (superficie == 0)
                superficie = -double.MaxValue;

            foreach (Riego r in lRiegos)
                redDatosRiegos.Add(new DatosRiego {
                    Fecha = r.Fecha,
                    M3 = r.RiegoM3,
                    Mm = (r.RiegoM3) / superficie,
                    Obtencion = "S",
                    IdTemporada = t.IdTemporada,
                    IdUnidadCultivo = idUnidadCultivo,
                    UnidadCultivo = uc?.Alias ?? ""
                });

            List<UnidadCultivoDatosExtra> lExtra = DatosExtraList(idUnidadCultivo);
            foreach (UnidadCultivoDatosExtra extra in lExtra)
                if (extra.Fecha >= desdeFecha && extra.Fecha <= hastaFecha && (extra.RiegoM3 ?? 0) > 0) {
                    DatosRiego find = redDatosRiegos.Find(f => f.Fecha == extra.Fecha);
                    if (find != null)
                        redDatosRiegos.Remove(find);
                    redDatosRiegos.Add(new DatosRiego {
                        Fecha = extra.Fecha,
                        M3 = extra.RiegoM3 ?? 0,
                        Mm = (extra.RiegoM3 ?? 0) / superficie,
                        Obtencion = "A",
                        IdTemporada = t.IdTemporada,
                        IdUnidadCultivo = idUnidadCultivo,
                        UnidadCultivo = uc.Alias
                    });
                }
            List<DatosLLuviaORiego> ret = new List<DatosLLuviaORiego>();
            foreach (DatosRiego rie in redDatosRiegos) {
                DatosLLuviaORiego dat = new DatosLLuviaORiego {
                    IdTipoAportacion = "Riego",
                    Fecha = rie.Fecha,
                    IdTemporada = rie.IdTemporada,
                    IdUnidadCultivo = rie.IdUnidadCultivo,
                    Mm = rie.Mm,
                    M3 = rie.M3,
                    Obtencion = rie.Obtencion,
                    UnidadCultivo = rie.IdUnidadCultivo
                };
                ret.Add(dat);
            }
            return ret;
        }

        /// <summary>
        /// TipoEstresUmbralList
        /// </summary>
        /// <param name="idTipoEstres">idTipoEstres<see cref="string"/></param>
        /// <returns><see cref="List{TipoEstresUmbral}"/></returns>
        internal static List<TipoEstresUmbral> TipoEstresUmbralOrderList(string idTipoEstres) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = $"SELECT * FROM TipoEstresUmbral Where IdTipoEstres='{idTipoEstres}' order by umbralMaximo";
            List<TipoEstresUmbral> ret = db.Fetch<TipoEstresUmbral>(sql);
            return ret;
        }

        /// <summary>
        /// RiegosNebulaList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="desdeFecha">desdeFecha<see cref="DateTime"/></param>
        /// <param name="hastaFecha">hastaFecha<see cref="DateTime"/></param>
        /// <returns><see cref="List{Riego}"/></returns>
        public static List<Riego> RiegosNebulaList(string idUnidadCultivo, DateTime desdeFecha, DateTime hastaFecha) {
            Database db = new Database(DB.CadenaConexionNebula);
            string sql = $"SELECT IdUnidadCultivo, RiegoM3, Fecha from riegos where idUnidadCultivo='{idUnidadCultivo}' AND  fecha >=@0 and fecha<=@1";
            List<Riego> ret = db.Fetch<Riego>(sql, desdeFecha, hastaFecha);
            return ret;
        }

        /// <summary>
        /// DatosLluviaList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object DatosLluviaList(string idUnidadCultivo, string idTemporada) {
            List<DatosLLuvia> retDatosLluvia = new List<DatosLLuvia>();
            Temporada t = Temporada(idTemporada);
            if (t == null) {
                t = Temporada(DB.TemporadaActiva());
            }

            UnidadCultivo uc = UnidadCultivo(idUnidadCultivo);
            UnidadCultivoCultivo ucc = UnidadCultivoCultivo(idUnidadCultivo, idTemporada);
            string estacion = Estacion(uc.IdEstacion).Nombre;

            DateTime desdeFecha = ucc.FechaSiembra ?? t.FechaInicial;
            DateTime hastaFecha = t.FechaFinal < DateTime.Today ? DateTime.Today : t.FechaFinal;

            List<DatoClimatico> lLluvia = DatosClimaticosList(desdeFecha, hastaFecha, uc.IdEstacion);

            foreach (DatoClimatico ll in lLluvia)
                retDatosLluvia.Add(new DatosLLuvia {
                    Fecha = ll.Fecha,
                    Mm = ll.Precipitacion,
                    Obtencion = "S",
                    IdEstacion = ll.IdEstacion.ToString(),
                    IdTemporada = t.IdTemporada,
                    IdUnidadCultivo = uc.IdUnidadCultivo,
                    UnidadCultivo = uc.Alias,
                    Estacion = estacion
                });

            List<UnidadCultivoDatosExtra> lExtra = DatosExtraList(idUnidadCultivo);
            foreach (UnidadCultivoDatosExtra extra in lExtra)
                if (extra.Fecha >= desdeFecha && extra.Fecha <= hastaFecha && (extra.LluviaMm ?? 0) > 0) {
                    DatosLLuvia find = retDatosLluvia.Find(f => f.Fecha == extra.Fecha);
                    if (find != null)
                        retDatosLluvia.Remove(find);
                    retDatosLluvia.Add(new DatosLLuvia {
                        Fecha = extra.Fecha,
                        Mm = extra.LluviaMm ?? 0,
                        Obtencion = "A",
                        IdEstacion = uc.IdEstacion.ToString(),
                        IdTemporada = t.IdTemporada,
                        IdUnidadCultivo = uc.IdUnidadCultivo,
                        UnidadCultivo = uc.Alias,
                        Estacion = estacion
                    });
                }
            List<DatosLLuviaORiego> ret = new List<DatosLLuviaORiego>();
            foreach (DatosLLuvia llu in retDatosLluvia) {
                DatosLLuviaORiego dat = new DatosLLuviaORiego {
                    IdTipoAportacion = "Lluvia",
                    Estacion = llu.Estacion,
                    Fecha = llu.Fecha,
                    IdEstacion = llu.IdEstacion,
                    IdTemporada = llu.IdTemporada,
                    IdUnidadCultivo = llu.IdUnidadCultivo,
                    Mm = llu.Mm,
                    Obtencion = llu.Obtencion,
                    UnidadCultivo = llu.IdUnidadCultivo
                };
                ret.Add(dat);
            }
            return ret;
        }

        /// <summary>
        /// ParcelaList
        /// </summary>
        /// <param name="IdTemporada">IdTemporada<see cref="string"/></param>
        /// <param name="IdParcela">IdParcela<see cref="string"/></param>
        /// <param name="IdRegante">IdRegante<see cref="string"/></param>
        /// <param name="IdMunicipio">IdMunicipio<see cref="string"/></param>
        /// <param name="Search">Search<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object ParcelaList(string IdTemporada, string IdParcela, string IdRegante, string IdMunicipio, string Search) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            IdTemporada = IdTemporada.Quoted();
            Search = Search.Quoted();
            string sql = $"SELECT * FROM ParcelaList({IdTemporada},{IdParcela},{IdRegante},{IdMunicipio},{Search})";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// ReganteList
        /// </summary>
        /// <param name="IdTemporada">IdTemporada<see cref="string"/></param>
        /// <param name="IdRegante">IdRegante<see cref="string"/></param>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="IdParcela">IdParcela<see cref="string"/></param>
        /// <param name="Search">Search<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object ReganteList(string IdTemporada, string IdRegante, string IdUnidadCultivo, string IdParcela, string Search) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            IdTemporada = IdTemporada.Quoted();
            IdUnidadCultivo = IdUnidadCultivo.Quoted();
            Search = Search.Quoted();
            string sql = $"SELECT * FROM ReganteList({IdTemporada},{IdRegante},{IdUnidadCultivo},{IdParcela},{Search})";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// PluviometriaSave
        /// </summary>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="pluviometria">pluviometria<see cref="double"/></param>
        public static void PluviometriaSave(string idTemporada, string idUnidadCultivo, double pluviometria) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            UnidadCultivoCultivo unidadCultivoCultivo = db.Single<UnidadCultivoCultivo>("SELECT * FROM UnidadCultivoCultivo WHERE IdTemporada=@0 and idUnidadCultivo=@1");
            unidadCultivoCultivo.Pluviometria = pluviometria;
            db.Save(unidadCultivoCultivo);
        }

        /// <summary>
        /// Temporada
        /// </summary>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="Temporada"/></returns>
        public static Temporada Temporada(string idTemporada) {
            if (string.IsNullOrWhiteSpace(idTemporada))
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleOrDefaultById<Temporada>(idTemporada);
        }

        /// <summary>
        /// UnidadCultivoList
        /// </summary>
        /// <param name="IdTemporada">IdTemporada<see cref="string"/></param>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="IdRegante">IdRegante<see cref="string"/></param>
        /// <param name="IdCultivo">IdCultivo<see cref="string"/></param>
        /// <param name="idMunicipio">idMunicipio<see cref="string"/></param>
        /// <param name="IdTipoRiego">IdTipoRiego<see cref="string"/></param>
        /// <param name="IdEstacion">IdEstacion<see cref="string"/></param>
        /// <param name="Search">Search<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static object UnidadCultivoList(string IdTemporada, string IdUnidadCultivo, string IdRegante, string IdCultivo, string idMunicipio, string IdTipoRiego, string IdEstacion, string Search) {
            IdTemporada = IdTemporada.Unquoted();
            Database db = new Database(DB.CadenaConexionOptiAqua);
            if (string.IsNullOrWhiteSpace(IdTemporada)) {
                IdTemporada = DB.TemporadaActiva() ?? "";
            }

            IdTemporada = IdTemporada.Quoted();
            IdUnidadCultivo = IdUnidadCultivo.Quoted();
            Search = Search.Quoted();
            string sql = $"SELECT * FROM UnidadcultivoList({IdTemporada.Quoted()},{IdUnidadCultivo},{IdRegante},{IdCultivo},{idMunicipio},{IdTipoRiego},{IdEstacion},{Search})";

            IdTemporada = IdTemporada.Unquoted();
            List<Dictionary<string, object>> lRet = db.Fetch<Dictionary<string, object>>(sql);
            foreach (Dictionary<string, object> dic in lRet) {
                string idUC = dic["IdUnidadCultivo"] as string;
                List<GeoLocParcela> lGeoLocParcelas = DB.GeoLocParcelasList(idUC, IdTemporada);
                string geo = Newtonsoft.Json.JsonConvert.SerializeObject(lGeoLocParcelas);
                dic.Add("GeoLocJson", geo);
            }
            return lRet;
        }

        /// <summary>
        /// TemporadaActiva (si no hay ninguna marcada como activa devuelve la que tiene la fecha final mas alta)
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public static string TemporadaActiva() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string ret = db.Fetch<string>("SELECT IdTemporada from temporada WHERE ACTIVA=1")[0];
            if (ret == null) {
                string sql = " SELECT IdTemporada FROM dbo.Temporada WHERE(FechaFinal = (SELECT TOP(1) MAX(FechaFinal) AS Expr1 FROM dbo.Temporada AS Temporada_1))";
                ret = db.Fetch<string>(sql)[0];
            }
            return ret;
        }

        /// <summary>
        /// UnidadCultivoDatosAmpliados
        /// </summary>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <returns><see cref="object"/></returns>
        public static List<UnidadCultivoDatosAmpliados> UnidadCultivoDatosAmpliados(string idTemporada, string idUnidadCultivo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string filtro = "";
            if (idTemporada != "''") {
                filtro += $" WHERE IDTEMPORADA='{idTemporada}'";
            }
            if (idUnidadCultivo != "''") {
                if (filtro == "")
                    filtro = " WHERE ";
                else
                    filtro += " AND ";
                filtro += $" IdUnidadCultivo='{idUnidadCultivo }'";
            }

            string sql = $"SELECT * FROM UnidadCultivoDatosAmpliados " + filtro;
            List<UnidadCultivoDatosAmpliados> ret = db.Fetch<UnidadCultivoDatosAmpliados>(sql);
            foreach (UnidadCultivoDatosAmpliados dat in ret) {
                ObtenerMunicicioParaje(idTemporada, dat.IdUnidadCultivo, out string provincias, out string municipios, out string parajes);
                dat.Provincia = provincias;
                dat.Municipio = municipios;
                dat.Paraje = parajes;
            }
            return ret;
        }

        /// <summary>
        /// Estacion
        /// </summary>
        /// <param name="idEstacion">idEstacion<see cref="int?"/></param>
        /// <returns><see cref="Estacion"/></returns>
        public static Estacion Estacion(int? idEstacion) {
            if (idEstacion == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleById<Estacion>(idEstacion);
        }

        /// <summary>
        /// Regante
        /// </summary>
        /// <param name="idRegante">idRegante<see cref="int?"/></param>
        /// <returns><see cref="object"/></returns>
        public static object Regante(int? idRegante) {
            if (idRegante == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleById<Regante>(idRegante);
        }

        /// <summary>
        /// ReganteUpdate
        /// </summary>
        /// <param name="rp">rp<see cref="RegantePost"/></param>
        public static void ReganteUpdate(RegantePost rp) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            Regante r = db.SingleOrDefaultById<Regante>(rp.IdRegante);
            if (r == null)
                throw new Exception($"El Regante {rp.IdRegante} no existe");
            r.NIF = rp.NIF;
            r.Nombre = rp.Nombre;
            r.Direccion = rp.Direccion;
            r.CodigoPostal = rp.CodigoPostal;
            r.Poblacion = rp.Poblacion;
            r.Provincia = rp.Provincia;
            r.Pais = rp.Pais;
            r.Telefono = rp.Telefono;
            r.TelefonoSMS = rp.TelefonoSMS;
            r.Email = rp.Email;
            db.Save(r);
        }

        /// <summary>
        /// RiegoTipo
        /// </summary>
        /// <param name="idTipoRiego">idTipoRiego<see cref="int?"/></param>
        /// <returns><see cref="RiegoTipo"/></returns>
        public static RiegoTipo RiegoTipo(int? idTipoRiego) {
            if (idTipoRiego == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleById<RiegoTipo>(idTipoRiego);
        }

        /// <summary>
        /// LaUnidadDeCultivoPerteneceAlRegante
        /// </summary>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="idRegante">idRegante<see cref="int"/></param>
        /// <param name="IdTemporada">IdTemporada<see cref="string"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool LaUnidadDeCultivoPerteneceAlRegante(string IdUnidadCultivo, int idRegante, string IdTemporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select IdRegante from UnidadCultivoParcela Where IdUnidadCultivo=@0 and IdTemporada=@1 and idRegante=@2";
            List<object> lu = db.Fetch<object>(sql, IdUnidadCultivo, IdTemporada, idRegante);
            return lu.Count != 0;
        }

        /// <summary>
        /// RegantesList
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public static object RegantesList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select IdRegante, Nombre, Telefono, TelefonoSMS, Email from Regante";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// LaUnidadDeCultivoPerteneceAlRegante
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idRegante">idRegante<see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool LaUnidadDeCultivoPerteneceAlRegante(string idUnidadCultivo, int idRegante) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            UnidadCultivo unidadCultivo = db.SingleOrDefaultById<UnidadCultivo>(idUnidadCultivo);
            if (unidadCultivo == null)
                return false;
            return unidadCultivo.IdRegante == idRegante;
        }

        /// <summary>
        /// PasswordSave
        /// </summary>
        /// <param name="login">login<see cref="LoginRequest"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool PasswordSave(LoginRequest login) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            Regante regante = db.SingleById<Regante>(login.NifRegante);
            regante.Contraseña = BuildPassword(login.NifRegante, login.Password);
            db.Save(regante);
            return true;
        }

        /// <summary>
        /// LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idRegante">idRegante<see cref="int"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool LaUnidadDeCultivoPerteneceAlReganteEnLaTemporada(string idUnidadCultivo, int idRegante, string idTemporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select IdUnidadCultivo from UnidadCultivoCultivo where IdUnidadCultivo=@0 and IdRegante=@1 and idTemporada=@2";
            string unidadCultivo = db.SingleOrDefault<string>(sql, idUnidadCultivo, idRegante, idTemporada);
            return unidadCultivo != null;
        }

        /// <summary>
        /// TemporadasList
        /// </summary>
        /// <returns><see cref="List{Temporada}"/></returns>
        public static List<Temporada> TemporadasList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from Temporada;";
            return db.Fetch<Temporada>(sql);
        }

        /// <summary>
        /// ParajesList
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public static object ParajesList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from ParajeAmpliado;";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// ProvinciaList
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public static object ProvinciaList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from Provincia;";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// CultivosList
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public static object CultivosList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from Cultivo;";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// MunicipiosList
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public static object MunicipiosList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "SELECT dbo.Municipio.IdMunicipio, dbo.Municipio.Municipio, dbo.Provincia.IdProvincia, dbo.Provincia.Provincia FROM dbo.Municipio INNER JOIN  dbo.Provincia ON dbo.Municipio.IdProvincia = dbo.Provincia.IdProvincia";
            return db.Fetch<object>(sql);
        }

        /// <summary>
        /// DatosExtraList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <returns><see cref="List{UnidadCultivoDatosExtra}"/></returns>
        public static List<UnidadCultivoDatosExtra> DatosExtraList(string idUnidadCultivo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from  UnidadCultivoDatosExtra where IdUnidadCultivo=@0";
            List<UnidadCultivoDatosExtra> ret = db.Fetch<UnidadCultivoDatosExtra>(sql, idUnidadCultivo);
            return ret;
        }

        /// <summary>
        /// DatosExtraList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="fecha">fecha<see cref="DateTime"/></param>
        /// <returns><see cref="List{UnidadCultivoDatosExtra}"/></returns>
        public static List<UnidadCultivoDatosExtra> DatosExtraList(string idUnidadCultivo, DateTime fecha) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from  UnidadCultivoDatosExtra where IdUnidadCultivo=@0 and fecha=@1";
            List<UnidadCultivoDatosExtra> ret = db.Fetch<UnidadCultivoDatosExtra>(sql, idUnidadCultivo, fecha);
            return ret;
        }

        /// <summary>
        /// FechaConfirmadaSave
        /// </summary>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="temporada">temporada<see cref="string"/></param>
        /// <param name="nEtapa">nEtapa<see cref="int"/></param>
        /// <param name="fechaConfirmada">fechaConfirmada<see cref="DateTime"/></param>
        public static void FechaConfirmadaSave(string IdUnidadCultivo, string temporada, int nEtapa, DateTime fechaConfirmada) {
            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                UnidadCultivoCultivoEtapas dat = new UnidadCultivoCultivoEtapas {
                    IdUnidadCultivo = IdUnidadCultivo,
                    IdTemporada = temporada,
                    IdEtapaCultivo = nEtapa
                };
                dat = db.SingleOrDefaultById<UnidadCultivoCultivoEtapas>(dat);
                if (dat != null) {
                    dat.FechaFinEtapaConfirmada = fechaConfirmada;
                    db.Save(dat);
                } else {
                    throw new Exception("Error accediendo a UnidadCultivoCultivoEtapas\n.");
                }
            } catch (Exception ex) {
                string msgErr = "Error cargando ParcelasCultivosEtapas.\n ";
                msgErr += ex.Message;
                throw new Exception(msgErr);
            }
        }

        /// <summary>
        /// CultivoEtapasList
        /// </summary>
        /// <param name="idCultivo">idCultivo<see cref="int?"/></param>
        /// <returns><see cref="List{CultivoEtapas}"/></returns>
        public static List<CultivoEtapas> CultivoEtapasList(int? idCultivo) {
            if (idCultivo == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<CultivoEtapas> listaCF = db.Fetch<CultivoEtapas>("Select * from CultivoEtapas Where IdCultivo=@0", idCultivo);
            return listaCF;
        }

        /// <summary>
        /// SueloTipo
        /// </summary>
        /// <param name="idSueloTipo">idSueloTipo<see cref="string"/></param>
        /// <returns><see cref="List{SueloTipo}"/></returns>
        public static List<SueloTipo> SueloTipo(string idSueloTipo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.Fetch<SueloTipo>();
        }

        /// <summary>
        /// SuelosTipoList
        /// </summary>
        /// <returns><see cref="List{SueloTipo}"/></returns>
        public static List<SueloTipo> SuelosTipoList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<SueloTipo> ret = db.Fetch<SueloTipo>("select * from SueloTipo");
            return ret;
        }

        /// <summary>
        /// The MateriaOrganicaTipo
        /// </summary>
        /// <param name="idMateriaOrganicaTipo">The idMateriaOrganicaTipo<see cref="string"/></param>
        /// <returns>The <see cref="MateriaOrganicaTipo"/></returns>
        public static MateriaOrganicaTipo MateriaOrganicaTipo(string idMateriaOrganicaTipo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            MateriaOrganicaTipo ret = db.SingleById<MateriaOrganicaTipo>(idMateriaOrganicaTipo);
            return ret;
        }

        /// <summary>
        /// MateriaOrganicaTipo
        /// </summary>
        /// <returns><see cref="List{MateriaOrganicaTipo}"/></returns>
        public static List<MateriaOrganicaTipo> MateriaOrganicaTipoList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<MateriaOrganicaTipo> ret = db.Fetch<MateriaOrganicaTipo>("select * from MateriaOrganicaTipo");
            return ret;
        }

        /// <summary>
        /// DatosExtraSave
        /// </summary>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="fecha">fecha<see cref="string"/></param>
        /// <param name="cobertura">cobertura<see cref="double?"/></param>
        /// <param name="altura">altura<see cref="double?"/></param>
        /// <param name="lluvia">lluvia<see cref="double?"/></param>
        /// <param name="driEnd">driEnd<see cref="double?"/></param>
        /// <param name="riegoM3">riego<see cref="double?"/></param>
        /// <param name="riegoMm">riego<see cref="double?"/></param>
        public static void DatosExtraSave(string IdUnidadCultivo, string fecha, double? cobertura, double? altura, double? lluvia, double? driEnd, double? riegoM3, double? riegoMm) {
            try {
                if (DateTime.TryParse(fecha, out DateTime fs) == false) {
                    throw new Exception("Error. El formato de la fecha no es correcto.\n");
                }
                Database db = new Database(DB.CadenaConexionOptiAqua);
                UnidadCultivoDatosExtra dat = new UnidadCultivoDatosExtra() { IdUnidadCultivo = IdUnidadCultivo, Fecha = fs };
                dat = db.SingleOrDefaultById<UnidadCultivoDatosExtra>(dat);
                if (dat == null)
                    dat = new UnidadCultivoDatosExtra();
                dat.IdUnidadCultivo = IdUnidadCultivo;
                dat.Fecha = fs;
                if (cobertura != -1)
                    dat.Cobertura = cobertura;
                if (lluvia != -1)
                    dat.LluviaMm = lluvia;
                if (altura != -1)
                    dat.Altura = altura;
                if (driEnd != -1)
                    dat.DriEnd = driEnd;
                if (riegoM3 != -1)
                    dat.RiegoM3 = riegoM3;
                if (riegoMm != -1)
                    dat.RiegoM3 = DB.ConversionMmAM3((double)riegoMm,IdUnidadCultivo,fs);
                db.Save(dat);
            } catch (Exception ex) {
                string msgErr = "Error al guardar datos extra.\n ";
                msgErr += ex.Message;
                throw new Exception(msgErr);
            }
        }

        private static double ConversionMmAM3(double riegoMm, string idUnidadCultivo,DateTime fecha) {
            string idTemporada = DB.TemporadaDeFecha(idUnidadCultivo, fecha);
            double supertificeM2 = UnidadCultivoExtensionM2(idUnidadCultivo, idTemporada);
            var cultivo = DB.UnidadCultivoCultivo(idUnidadCultivo, idTemporada);
            var tipoRiego = DB.RiegoTipo(cultivo.IdTipoRiego);
            double ret = riegoMm*supertificeM2/1000/(1-tipoRiego.Eficiencia);
            return ret;
        }

        /// <summary>
        /// The ElementosGruesosTipo
        /// </summary>
        /// <param name="IdElementosGruesos">The IdElementosGruesos<see cref="string"/></param>
        /// <returns>The <see cref="ElementosGruesosTipo"/></returns>
        public static ElementosGruesosTipo ElementosGruesosTipo(string IdElementosGruesos) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            ElementosGruesosTipo ret = db.SingleOrDefaultById<ElementosGruesosTipo>(IdElementosGruesos);
            if (ret == null)
                throw new Exception();
            return ret;
        }

        /// <summary>
        /// ElementosGruesosTipo
        /// </summary>
        /// <returns><see cref="List{ElementosGruesosTipo}"/></returns>
        public static List<ElementosGruesosTipo> ElementosGruesosTipoList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            List<ElementosGruesosTipo> ret = db.Fetch<ElementosGruesosTipo>("select * from ElementosGruesosTipo");
            return ret;
        }

        /// <summary>
        /// TemporadasUnidadCultivoList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <returns><see cref="List{string}"/></returns>
        public static List<string> TemporadasUnidadCultivoList(string idUnidadCultivo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = $"Select Distinct IdTemporada from UnidadCultivoCultivo where IdUnidadCultivo ='{idUnidadCultivo}'";
            List<string> ret = db.Fetch<string>(sql);
            return ret;
        }

        /// <summary>
        /// Retorna los datos del suelo para el horizonte indicado.
        /// Si horizonte=="ALL" retorna todos.
        /// </summary>
        /// <param name="idTemporada"></param>
        /// <param name="IdUnidadCultivo"></param>
        /// <param name="idHorizonte"></param>
        /// <returns></returns>
        public static List<UnidadCultivoSuelo> UnidadCultivoHorizonte(string idTemporada, string IdUnidadCultivo, string idHorizonte) {
            if (string.IsNullOrWhiteSpace(idTemporada))
                idTemporada = DB.TemporadaActiva();
            List<UnidadCultivoSuelo> ret = null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            if (idHorizonte == "ALL")
                ret = db.Fetch<UnidadCultivoSuelo>("Select * from UnidadCultivoSuelo where IdUnidadCultivo=@0 and IdTemporada=@1 ", IdUnidadCultivo, idTemporada);
            else
                ret = db.Fetch<UnidadCultivoSuelo>("Select * from UnidadCultivoSuelo where IdUnidadCultivo =@0 and idHorizonte=@1 and idtemporada=@2 ", IdUnidadCultivo, int.Parse(idHorizonte), idTemporada);
            return ret;
        }

        /// <summary>
        /// ParcelasDatosExtrasList
        /// </summary>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="desdeFecha">desdeFecha<see cref="DateTime?"/></param>
        /// <param name="hastaFecha">hastaFecha<see cref="DateTime?"/></param>
        /// <returns><see cref="List{UnidadCultivoDatosExtra}"/></returns>
        public static List<UnidadCultivoDatosExtra> ParcelasDatosExtrasList(string IdUnidadCultivo, DateTime? desdeFecha, DateTime? hastaFecha) {
            if (IdUnidadCultivo == null || desdeFecha == null || hastaFecha == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from UnidadCultivoDatosExtra where ";
            sql += "IdUnidadCultivo='" + IdUnidadCultivo + "' AND ";
            sql += "fecha BETWEEN Convert(date, '" + ((DateTime)desdeFecha).ToString("yyyyMMdd") + "') AND ";
            sql += "Convert(date, '" + ((DateTime)hastaFecha).ToString("yyyyMMdd") + "') ";
            return db.Fetch<UnidadCultivoDatosExtra>(sql);
        }

        /// <summary>
        /// Retorno lista de riegos
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="fechaSiembra"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public static List<Riego> RiegosList(string idUnidadCultivo, DateTime fechaSiembra, DateTime fechaFinal) {
            List<Riego> riegoHistorico = DB.RiegosHistoricosList(idUnidadCultivo, fechaSiembra, fechaFinal);
            List<Riego> riegoNebula = DB.RiegosNebulaList(idUnidadCultivo, fechaSiembra, fechaFinal);
            List<Riego> ret = riegoHistorico;
            ret.AddRange(riegoNebula);
            //ret=ret.OrderBy(x => x.Fecha).ToList();
            return ret;
        }

        /// <summary>
        /// Carga los Riegos de una Parcela en un intervalo de fechas
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="desdeFecha"></param>
        /// <param name="hastaFecha"></param>
        /// <returns></returns>
        public static List<Riego> RiegosHistoricosList(string idUnidadCultivo, DateTime? desdeFecha, DateTime? hastaFecha) {
            if (idUnidadCultivo == null || desdeFecha == null || hastaFecha == null)
                return null;

            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from Riego where ";
            sql += "idUnidadCultivo=@0 AND ";
            sql += "fecha BETWEEN Convert(date, '" + ((DateTime)desdeFecha).ToString("yyyyMMdd") + "') AND ";
            sql += "Convert(date, '" + ((DateTime)hastaFecha).ToString("yyyyMMdd") + "') ";
            return db.Fetch<Riego>(sql, idUnidadCultivo);
        }

        /// <summary>
        /// Retorna los datos de la tabla ParcelasCultivo.
        /// </summary>
        /// <param name="idUnidadCultivo"></param>
        /// <param name="idTemporada"></param>
        /// <returns></returns>
        public static UnidadCultivoCultivo UnidadCultivoCultivo(string idUnidadCultivo, string idTemporada) {
            if (idUnidadCultivo == null || idTemporada == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from UnidadCultivoCultivo where idUnidadCultivo=@0 AND IdTemporada=@1";
            return db.SingleOrDefault<UnidadCultivoCultivo>(sql, idUnidadCultivo, idTemporada);
        }

        /// <summary>
        /// UnidadCultivo
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <returns><see cref="UnidadCultivo"/></returns>
        public static UnidadCultivo UnidadCultivo(string idUnidadCultivo) {
            if (idUnidadCultivo == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleOrDefaultById<UnidadCultivo>(idUnidadCultivo);
        }

        /// <summary>
        /// UnidadCultivoList
        /// </summary>
        /// <returns><see cref="List{UnidadCultivo}"/></returns>
        public static List<UnidadCultivo> UnidadCultivoList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.Fetch<UnidadCultivo>();
        }

        /// <summary>
        /// Retorna una clase con todos los valores de la parcela IdParcela desde BD.
        /// </summary>
        /// <param name="idParcela"></param>
        /// <returns></returns>
        public static Parcela Parcela(int idParcela) {
            Database db = null;
            Parcela ret = null;
            try {
                db = new Database(DB.CadenaConexionOptiAqua);
                string sql = "SELECT IdParcelaInt, IdGadmin, IdRegante, IdProvincia, IdMunicipio, IdPoligono, IdParcela, IdParaje, Descripcion, Longitud, Latitud, XUTM, YUTM, Huso, Altitud, RefCatastral, GID, SuperficieM2 FROM dbo.Parcela";
                sql += " where idParcelaInt=" + idParcela.ToString();
                ret = db.Single<Parcela>(sql);
            } catch (Exception ex) {
                throw new Exception("No se pudo cargar parcela:" + idParcela.ToString() + " -" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// Carga los datos del cultivo referenciado
        /// </summary>
        /// <param name="IdCultivo"></param>
        /// <returns></returns>
        public static Cultivo Cultivo(int? IdCultivo) {
            if (IdCultivo == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleById<Cultivo>(IdCultivo);
        }

        /// <summary>
        /// UnidadCultivoCultivoEtapasList
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="List{UnidadCultivoCultivoEtapas}"/></returns>
        public static List<UnidadCultivoCultivoEtapas> UnidadCultivoCultivoEtapasList(string idUnidadCultivo, string idTemporada) {
            if (idUnidadCultivo == null || idTemporada == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from UnidadCultivoCultivoEtapas where IdUnidadCultivo =@0 AND IDTemporada=@1";
            return db.Fetch<UnidadCultivoCultivoEtapas>(sql, idUnidadCultivo, idTemporada);
        }

        /// <summary>
        /// ParcelasCultivo
        /// </summary>
        /// <param name="IdParcela">IdParcela<see cref="int"/></param>
        /// <param name="temporada">temporada<see cref="string"/></param>
        /// <returns><see cref="UnidadCultivoCultivo"/></returns>
        public static UnidadCultivoCultivo ParcelasCultivo(int IdParcela, string temporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from ParcelasCultivoEtapas where IdParcela =" + IdParcela + " AND IDTemporada='" + temporada + "' ";
            return db.SingleOrDefault<UnidadCultivoCultivo>(sql);
        }

        /// <summary>
        /// UnidadCultivoSueloList
        /// </summary>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <returns><see cref="List{UnidadCultivoSuelo}"/></returns>
        public static List<UnidadCultivoSuelo> UnidadCultivoSueloList(string idTemporada, string idUnidadCultivo) {
            if (idUnidadCultivo == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from UnidadCultivoSuelo where idUnidadCultivo =@0 and IdTemporada=@1";
            return db.Fetch<UnidadCultivoSuelo>(sql, idUnidadCultivo, idTemporada);
        }

        /// <summary>
        /// Actualiza los datos climáticos almacenados.
        /// Se conecta a el api del SIAR si hace al menos una hora que no lo ha hecho y actualiza los datos desde última acualización.
        /// Si se actualizó hace menos de 4 días actuliza los últimos 4 días.
        /// </summary>
        public static void DatosClimaticosSiarRefresh() {
            try {
                List<Estacion> lEstaciones = DB.EstacionesList();
                DateTime? ultimaFechaEnTabla = DB.UltimaFechaDeEstacion();
                if (ultimaFechaEnTabla == null)
                    ultimaFechaEnTabla = new DateTime(2000, 01, 01);

                DateTime desdeFecha = ((DateTime)ultimaFechaEnTabla).AddDays(-4); // Añado 4 días a la lista
                DateTime hastaFecha = DateTime.Today;
                foreach (Estacion e in lEstaciones) {
                    List<DatoClimatico> datClima = DatosClimaticosSiarList(desdeFecha, hastaFecha, e.IdEstacion);
                    DB.DatosClimaticosSave(datClima);
                }
            } catch {
                // continua sin datos del SIAR
            }
        }

        /// <summary>
        /// DatosClimaticosSave
        /// </summary>
        /// <param name="lDatClima">lDatClima<see cref="List{DatoClimatico}"/></param>
        private static void DatosClimaticosSave(List<DatoClimatico> lDatClima) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            foreach (DatoClimatico datCli in lDatClima) {
                db.Save<DatoClimatico>(datCli);
            }
        }

        /// <summary>
        /// UltimaFechaDeEstacion
        /// </summary>
        /// <returns><see cref="DateTime?"/></returns>
        private static DateTime? UltimaFechaDeEstacion() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "SELECT MIN(MaxFecha) AS MinFecha FROM dbo.DatoClimaticoMaxFecha";
            return db.Single<DateTime?>(sql);
        }

        /// <summary>
        /// EstacionesList
        /// </summary>
        /// <returns><see cref="List{Estacion}"/></returns>
        private static List<Estacion> EstacionesList() {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from Estacion";
            return db.Fetch<Estacion>(sql);
        }

        /// <summary>
        /// DatosClimaticosList
        /// </summary>
        /// <param name="desdeFecha">desdeFecha<see cref="DateTime?"/></param>
        /// <param name="hastaFecha">hastaFecha<see cref="DateTime?"/></param>
        /// <param name="idEstacion">idEstacion<see cref="int?"/></param>
        /// <returns><see cref="List{DatoClimatico}"/></returns>
        public static List<DatoClimatico> DatosClimaticosList(DateTime? desdeFecha, DateTime? hastaFecha, int? idEstacion) {
            if (desdeFecha == null || hastaFecha == null || idEstacion == null)
                return null;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from DatoClimatico where ";
            sql += "fecha BETWEEN Convert(date, '" + ((DateTime)desdeFecha).ToString("yyyyMMdd") + "') AND ";
            sql += "Convert(date, '" + ((DateTime)hastaFecha).ToString("yyyyMMdd") + "') AND IDESTACION=" + idEstacion;
            return db.Fetch<DatoClimatico>(sql);
        }

        /// <summary>
        /// DatosClimaticosList
        /// </summary>
        /// <param name="desdeFecha">desdeFecha<see cref="DateTime"/></param>
        /// <param name="hastaFecha">hastaFecha<see cref="DateTime"/></param>
        /// <returns><see cref="List{DatoClimatico}"/></returns>
        public static List<DatoClimatico> DatosClimaticosList(DateTime desdeFecha, DateTime hastaFecha) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql;
            sql = "Select * from DatoClimatico where ";
            sql += "fecha BETWEEN Convert(date, '" + desdeFecha.ToString("yyyyMMdd") + "') AND ";
            sql += "Convert(date, '" + hastaFecha.ToString("yyyyMMdd") + "')";
            return db.Fetch<DatoClimatico>(sql);
        }

        /// <summary>
        /// DatosClimaticosSiarList
        /// </summary>
        /// <param name="desdeFecha">desdeFecha<see cref="DateTime"/></param>
        /// <param name="hastaFecha">hastaFecha<see cref="DateTime"/></param>
        /// <param name="idEstacion">idEstacion<see cref="int"/></param>
        /// <returns><see cref="List{DatoClimatico}"/></returns>
        public static List<DatoClimatico> DatosClimaticosSiarList(DateTime desdeFecha, DateTime hastaFecha, int idEstacion) {
            try {
                string sURL;
                //sURL Ejemplo = "http://apisiar.larioja.org/v1/datos-calculo-riego?estacion=501&fechaInicio=2017-08-07&fechaFin=2017-08-10";
                sURL = "http://apisiar.larioja.org/v1/datos-calculo-riego?";
                sURL += "estacion=" + idEstacion.ToString();
                sURL += "&fechaInicio=" + desdeFecha.ToString("yyyy-MM-dd");
                sURL += "&fechaFin=" + hastaFecha.ToString("yyyy-MM-dd");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                WebClient wc = new System.Net.WebClient();
                string json = wc.DownloadString(sURL);
                stopWatch.Stop();
                stopWatch.Start();
                dynamic JsonDynamic = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                dynamic data = JsonDynamic.data;
                List<DatoClimatico> lista = new List<DatoClimatico>();
                foreach (dynamic dat in data) {
                    DatoClimatico dc = new DatoClimatico {
                        IdEstacion = dat.Estacion,
                        Fecha = Convert.ToDateTime(dat.Fecha),
                        Eto = dat.ET0 == "NA" ? 0 : Convert.ToSingle(dat.ET0),
                        TempMedia = dat.TAirMd == "NA" ? 0 : Convert.ToSingle(dat.TAirMd),
                        HumedadMedia = dat.HRMn == "NA" ? 0 : Convert.ToSingle(dat.HRMn),
                        VelViento = dat.VWindMd == "NA" ? 0 : Convert.ToSingle(dat.VWindMd),
                        Precipitacion = dat.PAcum == "NA" ? 0 : Convert.ToSingle(dat.PAcum)
                    };
                    lista.Add(dc);
                }
                stopWatch.Stop();
                return lista;
            } catch {
                string msgErr = "Error cargando datos climáticos para parametros.\n";
                msgErr += "Desde Fecha: " + desdeFecha.ToShortDateString() + "\n";
                msgErr += "Hasta Fecha: " + hastaFecha.ToShortDateString() + "\n";
                msgErr += "Estación: " + idEstacion.ToString() + "\n";
                throw new Exception(msgErr);
            }
        }

        /// <summary>
        /// Crea nuevo o actualiza el registro UnidadCultivoSuelo
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idHorizonte">idHorizonte<see cref="int"/></param>
        /// <param name="limo">limo<see cref="double"/></param>
        /// <param name="arcilla">arcilla<see cref="double"/></param>
        /// <param name="arena">arena<see cref="double"/></param>
        /// <param name="matOrg">matOrg<see cref="double"/></param>
        /// <param name="eleGru">eleGru<see cref="double"/></param>
        /// <param name="prof">prof<see cref="double"/></param>
        public static void UnidadCultivoSueloSave(string idUnidadCultivo, string idTemporada, int idHorizonte, double limo, double arcilla, double arena, double matOrg, double eleGru, double prof) {
            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                UnidadCultivoSuelo ps = new UnidadCultivoSuelo {
                    IdUnidadCultivo = idUnidadCultivo,
                    IdHorizonte = idHorizonte,
                    Limo = limo,
                    Arcilla = arcilla,
                    Arena = arena,
                    MateriaOrganica = matOrg,
                    ElementosGruesos = eleGru,
                    ProfundidadHorizonte = prof,
                    IdTemporada = idTemporada
                };
                db.Save(ps);
            } catch (Exception ex) {
                throw new Exception("Error dando de alta horizonte.\n" + ex.Message);
            }
        }

        /// <summary>
        /// UnidadCultivoCultivoTemporadaSave
        /// </summary>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idCultivo">idCultivo<see cref="int"/></param>
        /// <param name="idRegante">idRegante<see cref="int"/></param>
        /// <param name="idTipoRiego">idTipoRiego<see cref="int"/></param>
        /// <param name="fechaSiembra">fechaSiembra<see cref="string"/></param>
        public static void UnidadCultivoCultivoTemporadaSave(string IdUnidadCultivo, string idTemporada, int idCultivo, int idRegante, int idTipoRiego, string fechaSiembra) {
            Database db = null;
            try {
                db = new Database(DB.CadenaConexionOptiAqua);
                db.BeginTransaction();
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
                    FechaSiembra = fs,
                    Pluviometria = PluviometriaTipica(idTipoRiego)
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
                        Etapa = cf.Etapa,
                        FechaInicioEtapa = fechaEtapa
                    };
                    fechaEtapa = fechaEtapa.AddDays(cf.DuracionDiasEtapa);
                    pcf.FechaFinEtapaConfirmada = null;
                    pcf.DefinicionPorDias = cf.DefinicionPorDias;
                    pcf.KcInicial = cf.KcInicial;
                    pcf.KcFinal = cf.KcFinal;
                    pcf.CobInicial = cf.CobInicial;
                    pcf.CobFinal = cf.CobFinal;
                    pcf.FactorDeAgotamiento = cf.FactorAgotamiento;
                    db.Insert(pcf);
                }

                db.CompleteTransaction();
                return;
            } catch (Exception ex) {
                db.AbortTransaction();
                throw new Exception("Error. No existe una definición de las Etapas para el cultivo indicado." + ex.Message);
            }
        }

        /// <summary>
        /// NParcelas
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="int?"/></returns>
        public static int? NParcelas(string idUnidadCultivo, string idTemporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "SELECT COUNT(IdParcelaInt) AS NParcelas FROM dbo.UnidadCultivoParcela GROUP BY IdUnidadCultivo, IdTemporada ";
            sql += " HAVING IdUnidadCultivo=@0 AND IdTemporada=@1";
            return db.SingleOrDefault<int?>(sql, idUnidadCultivo, idTemporada);
        }

        /// <summary>
        /// ObtenerMunicicioParaje
        /// </summary>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="provincias"></param>
        /// <param name="municipios">municipios<see cref="string"/></param>
        /// <param name="parajes">parajes<see cref="string"/></param>
        public static void ObtenerMunicicioParaje(string idTemporada, string idUnidadCultivo, out string provincias, out string municipios, out string parajes) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select Provincia,Municipio, Paraje from UnidadCultivoParaje where idTemporada=@0 and IdUnidadCultivo=@1";
            List<ProvinciaMunicipioParaje> lMunicipioProvinciaParaje = db.Fetch<ProvinciaMunicipioParaje>(sql, idTemporada, idUnidadCultivo);
            IEnumerable<string> lmunicicipos = lMunicipioProvinciaParaje.Select(x => x.Municipio).Distinct();
            IEnumerable<string> lParajes = lMunicipioProvinciaParaje.Select(x => x.Paraje).Distinct();
            IEnumerable<string> lProvincias = lMunicipioProvinciaParaje.Select(x => x.Provincia).Distinct();
            municipios = string.Join(",", lmunicicipos);
            parajes = string.Join(",", lParajes);
            provincias = string.Join(",", lProvincias);
        }

        /// <summary>
        /// UnidadesCultivoList
        /// </summary>
        /// <param name="idRegante">idRegante<see cref="int"/></param>
        /// <param name="fecha">idTemporada<see cref="string"/></param>
        /// <returns><see cref="List{string}"/></returns>
        public static List<string> UnidadesCultivoList(int idRegante, DateTime fecha) {
            List<string> ret = new List<string>();
            try {
                if (fecha == null)
                    return new List<string>();
                List<string> lTemporadas = DB.TemporadasDeFecha(fecha);
                Database db = new Database(DB.CadenaConexionOptiAqua);
                string sql = "Select Distinct IdUnidadCultivo from UnidadCultivoCultivo where IdRegante=@0 AND IdTemporada =@1";
                foreach (string idTemporada in lTemporadas) {
                    List<string> deTemporada = db.Fetch<string>(sql, idRegante, idTemporada);
                    ret.AddRange(deTemporada);
                }

            } catch {
                string msgErr = "No se pudo cargar lista de parcelas para los parámetros:\n";
                msgErr += "IdRegante:" + idRegante.ToString() + "\n";
                msgErr += "fecha:" + fecha.ToString() + "\n";
                throw new Exception(msgErr);
            }
            return ret;
        }

        /// <summary>
        /// UnidadCultivoList
        /// </summary>
        /// <param name="idRegante">idRegante<see cref="int"/></param>
        /// <returns><see cref="List{string}"/></returns>
        public static List<string> UnidadCultivoList(int idRegante) {
            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                string sql;
                sql = "Select Distinct IdUnidadCultivo from UnidadcultivoCultivo where IdRegante=@0";
                return db.Fetch<string>(sql, idRegante);
            } catch {
                string msgErr = "No se pudo cargar lista de parcelas para los parámetros:\n";
                msgErr += "IdRegante:" + idRegante.ToString() + "\n";
                throw new Exception(msgErr);
            }
        }

        /// <summary>
        /// EtapasList
        /// </summary>
        /// <param name="IdUnidadCultivo">IdUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="List{UnidadCultivoCultivoEtapas}"/></returns>
        public static List<UnidadCultivoCultivoEtapas> Etapas(string IdUnidadCultivo, string idTemporada) {
            if (string.IsNullOrWhiteSpace(idTemporada))
                return new List<UnidadCultivoCultivoEtapas>();
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select * from UnidadCultivoCultivoEtapas where IdUnidadCultivo=@0  AND IdTemporada=@1";
            List<UnidadCultivoCultivoEtapas> ret = db.Fetch<UnidadCultivoCultivoEtapas>(sql, IdUnidadCultivo, idTemporada);
            return ret;
        }

        /// <summary>
        /// FechasEtapasSave
        /// </summary>
        /// <param name="lEtapas">lEtapas<see cref="List{UnidadCultivoCultivoEtapas}"/></param>
        public static void FechasEtapasSave(List<UnidadCultivoCultivoEtapas> lEtapas) {
            Database db = null;
            if (lEtapas == null || lEtapas.Count == 0) return;
            try {
                db = new Database(DB.CadenaConexionOptiAqua);
                db.BeginTransaction();
                //Eliminar las actuales
                db.Execute(" delete from UnidadCultivoCultivoEtapas where IdUnidadCultivo=@0 and IdTemporada=@1 ", lEtapas[0].IdUnidadCultivo, lEtapas[0].IdTemporada);
                db.InsertBulk<UnidadCultivoCultivoEtapas>(lEtapas);
                db.CompleteTransaction();
            } catch (Exception) {
                if (db != null)
                    db.AbortTransaction();
            }
        }

        /// <summary>
        /// IdReganteDeUnidadCultivoTemporada
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="int?"/></returns>
        public static int? IdReganteDeUnidadCultivoTemporada(string idUnidadCultivo, string idTemporada) {
            int? ret = null;
            if (string.IsNullOrEmpty(idUnidadCultivo))
                return null;
            if (string.IsNullOrEmpty(idTemporada))
                return null;

            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                string sql;
                sql = "Select Distinct IdRegante from UnidadCultivoParcela where IdUnidadCultivo=@0 and idTemporada=@1 ";
                List<int?> lRet = db.Fetch<int?>(sql, idUnidadCultivo, idTemporada);
                if (lRet != null && lRet.Count > 0)
                    ret = lRet[0];
                return ret;
            } catch (Exception) {

                return null;
            }
        }

        /// <summary>
        /// IdReganteDeUnidadCultivo
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <returns><see cref="int?"/></returns>
        public static int? IdReganteDeUnidadCultivo(string idUnidadCultivo) {
            int? ret = null;
            if (string.IsNullOrEmpty(idUnidadCultivo))
                return null;
            try {
                Database db = new Database(DB.CadenaConexionOptiAqua);
                string sql;
                sql = "Select Distinct IdRegante from UnidadCultivo where IdUnidadCultivo=@0 ";
                List<int?> lRet = db.Fetch<int?>(sql, idUnidadCultivo);
                if (lRet != null && lRet.Count > 0)
                    ret = lRet[0];
                return ret;
            } catch (Exception) {

                return null;
            }
        }

        /// <summary>
        /// Retorna la lista de códigos de parcelas de una unidad de cultivo para la temporada indicada
        /// </summary>
        /// <param name="IdUnidadCultivo"></param>
        /// <param name="idTemporada"></param>
        /// <returns></returns>
        public static List<int> ParcelasDeUnidadCultivo(string IdUnidadCultivo, string idTemporada) {

            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select IdParcelaInt From UnidadCultivoParcela Where IdUnidadCultivo=@0 and IdTemporada=@1";
            List<int> ret = db.Fetch<int>(sql, IdUnidadCultivo, idTemporada);
            return ret;
        }

        /// <summary>
        /// UnidadCultivoExtensionM2
        /// </summary>
        /// <param name="idUnidadCultivo">idUnidadCultivo<see cref="string"/></param>
        /// <param name="idTemporada">idTemporada<see cref="string"/></param>
        /// <returns><see cref="float"/></returns>
        public static double UnidadCultivoExtensionM2(string idUnidadCultivo, string idTemporada) {
            if (idUnidadCultivo == null || idTemporada == null)
                return 0;
            double? ret = 0;
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select SuperficieM2 From UnidadCultivoSuperficie Where IdUnidadCultivo=@0 and IdTemporada=@1";
            ret = db.SingleOrDefault<float?>(sql, idUnidadCultivo, idTemporada);
            if (ret != null)
                return (double)ret;
            sql = "SELECT TOP (1) dbo.UnidadCultivoSuperficie.SuperficieM2 FROM dbo.Temporada INNER JOIN dbo.UnidadCultivoSuperficie ";
            sql += " ON dbo.Temporada.IdTemporada = dbo.UnidadCultivoSuperficie.IdUnidadCultivo ";
            sql += " WHERE dbo.UnidadCultivoSuperficie.IdUnidadCultivo = @0 ";
            sql += " ORDER BY dbo.Temporada.FechaInicial DESC";
            ret = db.SingleOrDefault<float?>(sql, idUnidadCultivo);
            if (ret != null)
                return (double)ret;

            sql = "  SELECT SUM(dbo.Parcela.SuperficieM2) AS Suma ";
            sql += " FROM dbo.UnidadCultivoParcela INNER JOIN ";
            sql += " dbo.Parcela ON dbo.UnidadCultivoParcela.IdParcelaInt = dbo.Parcela.IdParcelaInt ";
            sql += " GROUP BY dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.UnidadCultivoParcela.IdTemporada ";
            sql += " HAVING(dbo.UnidadCultivoParcela.IdUnidadCultivo =@0) AND(dbo.UnidadCultivoParcela.IdTemporada =@1)";
            ret = db.SingleOrDefault<double?>(sql, idUnidadCultivo, idTemporada);

            return ret ?? 0;
        }

        /// <summary>
        /// TemporadaDeFecha
        /// </summary>
        /// <param name="idUC">IdUnidadCultivo</param>
        /// <param name="fecha">fecha<see cref="DateTime"/></param>
        /// <returns><see cref="string"/></returns>
        public static string TemporadaDeFecha(string idUC, DateTime? fecha) {
            if (fecha==null)
                return DB.TemporadaActiva();
            if (string.IsNullOrWhiteSpace(idUC))
                return DB.TemporadaActiva();
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = $"SELECT * FROM TemporadaDeFecha(@0,@1)";
            return db.Single<string>(sql, idUC, fecha);
        }

        public static List<string> TemporadasDeFecha(DateTime fecha) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string strFecha = fecha.ToString();
            string sql = $"SELECT idTemporada FROM Temporada where @0>=FechaInicial AND @0<=FechaFinal";
            List<string> ret = db.Fetch<string>(sql, fecha);
            return ret;
        }

        /// <summary>
        /// PluviometriaTipica
        /// </summary>
        /// <param name="idCultivo">idCultivo<see cref="int"/></param>
        /// <returns><see cref="double"/></returns>
        public static double PluviometriaTipica(int idCultivo) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = "Select PluviometriaTipica from RiegoTipo where IdTipoRiego=@0";
            return db.Single<double>(sql, idCultivo);
        }

        /// <summary>
        /// ConfigLoad
        /// </summary>
        /// <param name="parametro">parametro<see cref="string"/></param>
        /// <returns><see cref="string"/></returns>
        public static string ConfigLoad(string parametro) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            return db.SingleOrDefaultById<Configuracion>(parametro)?.Valor;
        }

        /// <summary>
        /// ConfigSave
        /// </summary>
        /// <param name="parametro">parametro<see cref="string"/></param>
        /// <param name="valor">valor<see cref="string"/></param>
        public static void ConfigSave(string parametro, string valor) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            Configuracion cfg = new Configuracion { Parametro = parametro, Valor = valor };
            db.Save(cfg);
        }

        /// <summary>
        /// The TemporadaExists
        /// </summary>
        /// <param name="idTemporada">The idTemporada<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        internal static bool TemporadaExists(string idTemporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            bool ret = db.Exists<Temporada>(idTemporada);
            return ret;
        }

        internal static List<UnidadDeCultivoParcelasValvulas> UnidadCultivoParcelasValculas(string idUnidadCultivo, string idTemporada) {
            Database db = new Database(DB.CadenaConexionOptiAqua);
            string sql = $"Select * from UnidadDeCultivoParcelasValvulas where IdUnidadCultivo='{idUnidadCultivo}' And idTemporada='{idTemporada}'";
            List<UnidadDeCultivoParcelasValvulas> ret = db.FetchOneToMany<UnidadDeCultivoParcelasValvulas>(x => x.LIdValvula, sql);
            return ret;
        }
    }
}
