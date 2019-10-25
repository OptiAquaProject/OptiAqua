using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using Models;
using Microsoft.IdentityModel.Tokens;
using NPoco;

namespace webapi {
    /// <summary>
    /// JWT Token generator class using "secret-key"
    /// more info: https://self-issued.info/docs/draft-ietf-oauth-json-web-token.html
    /// </summary>
    public static class TokenGenerator {
        public static string GenerateTokenJwt(Regante regante) {
            // appsetting for Token JWT
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity             
            //var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, regante.Nombre ), new Claim(ClaimTypes.Role, regante.Role )});
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("NifRegante", regante.NIF??"" ));
            claimsIdentity.AddClaim(new Claim("IdRegante", regante.IdRegante.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, regante.Role)) ;

            // create token to the user 
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }


        static private List<LoginAcceso> ListaAccesos = new List<LoginAcceso>();
        /// <summary>
        /// Añadir tiempos de retardo a las peticiones 
        /// </summary>
        /// <param name="nifRegante"></param>
        /// <returns></returns>
        static public int CalculaRetardo(string nifRegante) {
            // eliminar registro de accesos con último acceso con más de 10 minutos.
            var horaCorte = DateTime.Now.AddMinutes(-10);
            ListaAccesos.RemoveAll(x => x.horaUltimoIntento < horaCorte);
            if (ListaAccesos.Count > 1000) { // si tenemos más de 1000 intentos en la última hora -> retardo de 2 segundos para todos las peticiones
                return 2000;
            }
            var acceso = ListaAccesos.Find(x => x.nifRegante == nifRegante);
            if (acceso != null) {
                acceso.nIntentos++;
                acceso.horaUltimoIntento = DateTime.Now;
            }
            else {
                acceso = new LoginAcceso { nifRegante = nifRegante, horaUltimoIntento = DateTime.Now, nIntentos = 0 };
                ListaAccesos.Add(acceso);
            }
            return acceso.nIntentos * acceso.nIntentos * 200; // crece exponencialmente a partir del primer fallo. 0,2 - 0,8 - 1,8 - 3,2 - 5,0 - 7,2 - 9,8 - 12,8 segundos
        }
    }
}
