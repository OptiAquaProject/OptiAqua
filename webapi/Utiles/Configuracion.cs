namespace DatosOptiaqua {
    /// <summary>
    /// Clase dedicada a optener y guardar parámetros de la aplicación almacenados en la tabla configuracion
    /// </summary>
    public static class Config {
        /// <summary>
        /// GetString - Retorna el parametro almacenado como un string
        /// </summary>
        /// <param name="parametro">The parametro<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetString(string parametro) => DB.ConfigLoad(parametro);

        /// <summary>
        /// GetDouble - Retorna el parametro almacenado como un double
        /// </summary>
        /// <param name="parametro">The parametro<see cref="string"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double GetDouble(string parametro) => double.Parse(DB.ConfigLoad(parametro));

        /// <summary>
        /// SetString - Almacenar valor para el parámetro indicado
        /// </summary>
        /// <param name="parametro">The parametro<see cref="string"/></param>
        /// <param name="valor">The valor<see cref="string"/></param>
        public static void SetString(string parametro, string valor) => DB.ConfigSave(parametro, valor);

        /// <summary>
        /// SetDouble Almacenar valor para el parámetro indicado
        /// </summary>
        /// <param name="parametro">The parametro<see cref="string"/></param>
        /// <param name="valor">The valor<see cref="double"/></param>
        public static void SetDouble(string parametro, double valor) => DB.ConfigSave(parametro, valor.ToString());

    }
}
