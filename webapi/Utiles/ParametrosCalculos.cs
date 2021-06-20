using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utiles {
    public class ParametrosEtapasCalculos: List<Dictionary<string, double>> {
        public double? Get(int nEtapa,string parametro) {
            if (nEtapa >= this.Count)
                return null;
            if ( this[nEtapa].TryGetValue(parametro, out var ret))
                return ret;
            return null;
        }
        public bool GetBool(int nEtapa, string parametro) {
            if (nEtapa >= this.Count)
                return false;
            if (this[nEtapa].TryGetValue(parametro, out var ret))
                return ret>0?true:false;
            return false;
        }

    }
    public class ParametrosCultivoCalculos : Dictionary<string, double> {
        public double? Get(string parametro) {
            if (this.TryGetValue(parametro, out var ret))
                return ret;
            return null;
        }
        public bool GetBool(string parametro) {
            if (this.TryGetValue(parametro, out var ret))
                return ret > 0 ? true : false;
            return false;
        }

    }
}