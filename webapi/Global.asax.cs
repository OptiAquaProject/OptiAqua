using DatosOptiaqua;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace webapi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Evito las referencias circulares al trabajar con Entity FrameWork         
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

            //SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

            //var tt = DB.HidrantesListJson("2467_R1", "T2021");
            //DB.ImportarHidrantes();
            //DB.AlturaInicialYFInal();
            //DB.QuitarParametrosJson();
            //DB.PropagarJsonCultivo();
            //DB.PropagarEtapas2();
            //DB.CrearJson1();
            //DB.CrearJson2();
            //Elimino que el sistema devuelva en XML, sólo trabajaremos con JSON
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           //DB.PasswordSave(new LoginRequest { NifRegante = "ASAJA", Password = "Pass3745" });
           //DB.PasswordSave(new LoginRequest { NifRegante = "CIERZO", Password = "Pass1498" });
           //DB.PasswordSave(new LoginRequest { NifRegante = "AIMCRA", Password = "Pass6524" });
            var cronExp = "0 0 8 * * ?";// cada dia a las 8:00                                        
            ScheduledTasks.JobScheduler.Start(cronExp).GetAwaiter().GetResult();
            DB.InsertaEvento("Aplicación Start at " + DateTime.Now.ToString());            
#if DEBUG
#else
            CacheDatosHidricos.RecreateAll();
#endif
        }
    }
}
