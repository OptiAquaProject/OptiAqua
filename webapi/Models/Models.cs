using NPoco;
using System;
using System.Collections.Generic;

/// <summary>
/// Modelo de datos de la aplicación
/// </summary>
namespace Models {

    public class LoginRequest {
        public string NifRegante { get; set; }
        public string Password { get; set; }
    }

    public class LoginAcceso {
        public string nifRegante;
        public int nIntentos;
        public DateTime horaUltimoIntento;
    }

    [TableName("Configuracion")]
    [PrimaryKey("Parametro", AutoIncrement = false)]
    public class Configuracion {
        public string Parametro { get; set; }
        public string Valor { get; set; }
    }

    [TableName("UnidadCultivo")]
    [PrimaryKey("IdUnidadCultivo", AutoIncrement = false)]
    public class UnidadCultivo {
        public string IdUnidadCultivo { get; set; }
        public int? IdRegante { get; set; }
        public int IdEstacion { get; set; }
        public string Alias { get; set; }
        public string TipoSueloDescripcion { get; set; }
    }

    [TableName("UnidadCultivoSuperficie")]
    [PrimaryKey("IdUnidadCultivo,IdTemporada", AutoIncrement = false)]
    public class UnidadCultivoSuperficie {
        public string IdUnidadCultivo { get; set; }
        public string IdTemporada { get; set; }
        public double SuperficieM2 { get; set; }
    }

    [TableName("CultivoEtapas")]
    [PrimaryKey("IdCultivo,OrdenEtapa", AutoIncrement = false)]
    public class CultivoEtapas {
        public int IdCultivo { get; set; }
        public string IdTipoEstres { get; set; }
        public int OrdenEtapa { get; set; }
        public string Etapa { get; set; }
        public int DuracionDiasEtapa { get; set; }
        public double KcInicial { get; set; }
        public double KcFinal { get; set; }
        public bool DefinicionPorDias { get; set; }
        public double? CobInicial { get; set; }
        public double? CobFinal { get; set; }
        public double FactorAgotamiento { get; set; }
    }

    [TableName("Cultivo")]
    [PrimaryKey("IdCultivo", AutoIncrement = false)]
    public class Cultivo {
        public int IdCultivo { get; set; }
        public string Nombre { get; set; }
        public double TBase { get; set; }
        public double ProfRaizInicial { get; set; }
        public double ProfRaizMax { get; set; }
        public double ModCobCoefA { get; set; }
        public double ModCobCoefB { get; set; }
        public double? ModCobCoefC { get; set; }
        public double ModAltCoefA { get; set; }
        public double ModAltCoefB { get; set; }
        public double? ModAltCoefC { get; set; }
        public double ModRaizCoefA { get; set; }
        public double ModRaizCoefB { get; set; }
        public double? ModRaizCoefC { get; set; }
        public double? AlturaInicial { get; set; }
        public double? AlturaFinal { get; set; }
        public double IntegralEmergencia { get; set; }
        public int EtapaInicioRiego { get; set; }
    }

    [TableName("UnidadCultivoParcela")]
    [PrimaryKey("IdUnidadCultivo,IdTemporada,IdParcelaInt", AutoIncrement = false)]
    public class UnidadCultivoParcela {
        public string IdUnidadCultivo { get; set; }
        public string IdTemporada { get; set; }
        public int IdParcelaInt { get; set; }
        public int IdRegante { get; set; }
    }


    [TableName("DatoClimatico")]
    [PrimaryKey("Fecha,IdEstacion", AutoIncrement = false)]
    public class DatoClimatico {
        public DateTime Fecha { get; set; }
        public int IdEstacion { get; set; }
        public double TempMedia { get; set; }
        public double HumedadMedia { get; set; }
        public double VelViento { get; set; }
        public double Precipitacion { get; set; }
        public double Eto { get; set; }
    }

    [TableName("Estacion")]
    [PrimaryKey("IdEstacion", AutoIncrement = false)]
    public class Estacion {
        public int IdEstacion { get; set; }
        public string Nombre { get; set; }
        public int IdRed { get; set; }
        public byte IdProvincia { get; set; }
        public int IdTermino { get; set; }
        public int? Longitud { get; set; }
        public int? Latitud { get; set; }
        public int? XUTM { get; set; }
        public int? YUTM { get; set; }
        public int? Huso { get; set; }
        public double? Altitud { get; set; }
    }

    [TableName("UnidadCultivoSuelo")]
    [PrimaryKey("IdTemporada,IdUnidadCultivo,IdHorizonte", AutoIncrement = false)]
    public class UnidadCultivoSuelo {
        public string IdTemporada { get; set; }
        public string IdUnidadCultivo { get; set; }
        public int IdHorizonte { get; set; }
        public double ProfundidadHorizonte { get; set; }
        public double ElementosGruesos { get; set; }
        public double Limo { get; set; }
        public double Arcilla { get; set; }
        public double Arena { get; set; }
        public double MateriaOrganica { get; set; }

    }

    [TableName("UnidadCultivoDatosExtra")]
    [PrimaryKey("IdUnidadCultivo,Fecha", AutoIncrement = false)]
    public class UnidadCultivoDatosExtra {
        public string IdUnidadCultivo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public double? Cobertura { get; set; } = null;
        public double? Altura { get; set; } = null;
        public double? LluviaMm { get; set; } = null;
        public double? DriEnd { get; set; } = null;
        public double? RiegoM3 { get; set; } = null;
    }

    [TableName("Parcela")]
    [PrimaryKey("IdParcelaInt", AutoIncrement = false)]
    public class Parcela {
        public int IdParcelaInt { get; set; } // nvarchar(10), not null
        public int? IdRegante { get; set; } // int, null
        public int? IdProvincia { get; set; } // nchar(2), not null
        public int? IdMunicipio { get; set; } // nchar(3), not null
        public int? IdPoligono { get; set; } // nchar(3), not null        
        public string IdParcela { get; set; } // nchar(5), not null
        public int? IdParaje { get; set; }
        public string Descripcion { get; set; } // nvarchar(50), not null
        public int? Longitud { get; set; } // int, null
        public int? Latitud { get; set; } // int, null
        public int? XUTM { get; set; } // int, null
        public int? YUTM { get; set; } // int, null
        public int? Huso { get; set; } // int, null
        public double? Altitud { get; set; } // float, null
        public double SuperficieM2 { get; set; } // float, not null
        public string RefCatastral { get; set; }
        public int GId { get; set; }
        public object GEO { set; get; }
    }

    [TableName("Paraje")]
    [PrimaryKey("IdParaje", AutoIncrement = false)]
    public class ParajePoco {
        public int IdParaje { get; set; }
        public string IdProvincia { get; set; }
        public int? IdMunucipio { get; set; }
        public string Paraje { get; set; }
    }



    [TableName("ParcelaValvula")]
    [PrimaryKey("IdParcelaInt,IdValcula", AutoIncrement = false)]
    public class ParcelaValvula {
        public int IdParcelaInt { get; set; }
        public int IdValvula { get; set; }
    }

    [TableName("UnidadCultivoCultivo")]
    [PrimaryKey("IdUnidadCultivo,IdTemporada", AutoIncrement = false)]
    public class UnidadCultivoCultivo {
        public string IdUnidadCultivo { get; set; }
        public string IdTemporada { get; set; }
        public int IdCultivo { get; set; }
        public int IdRegante { get; set; }
        public int IdTipoRiego { get; set; }
        public DateTime? FechaSiembra { get; set; }
        public double Pluviometria { set; get; }
    }

    [TableName("UnidadCultivoCultivoEtapas")]
    [PrimaryKey("IdParcela,IdTemporada,IdEtapaCultivo", AutoIncrement = false)]
    public class UnidadCultivoCultivoEtapas {
        public string IdUnidadCultivo { get; set; }
        public string IdTemporada { get; set; }
        public int IdEtapaCultivo { get; set; }
        public string Etapa { get; set; }
        public DateTime FechaInicioEtapa { get; set; }
        public DateTime? FechaFinEtapaConfirmada { get; set; }
        public bool DefinicionPorDias { get; set; }
        public double KcInicial { get; set; }
        public double KcFinal { get; set; }
        public double? CobInicial { get; set; }
        public double? CobFinal { get; set; }
        public double FactorDeAgotamiento { get; set; }
        public string IdTipoEstres { get; set; }
    }

    [TableName("Regante")]
    [PrimaryKey("idRegante", AutoIncrement = false)]
    public class Regante {
        public int IdRegante { get; set; }
        public int? IdGadmin { get; set; }
        public string NIF { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Poblacion { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
        public string Contraseña { get; set; }
        public string Role { get; set; }
        public string TelefonoSMS { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool? WebActive { get; set; }
    }

    public class RegantePost {
        public int IdRegante { get; set; }
        public string NIF { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Poblacion { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
        public string TelefonoSMS { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

    public class Riego {
        public DateTime Fecha { get; set; }
        public string IdUnidadCultivo { get; set; }
        public double RiegoM3 { get; set; }
    }

    [TableName("SueloTipo")]
    [PrimaryKey("IdSueloTipo,IdHorizonte", AutoIncrement = false)]
    public class SueloTipo {
        public string IdSueloTipo { get; set; }
        public int IdHorizonte { get; set; }
        public double Profundidad { get; set; }
        public double Limo { get; set; }
        public double Arcilla { get; set; }
        public double Arena { get; set; }
        public double ElementosGruesos { get; set; }
        public double MateriaOrganica { get; set; }
    }

    [TableName("MateriaOrganicaTipo")]
    [PrimaryKey("IdMateriaOrganica", AutoIncrement = false)]
    public class MateriaOrganicaTipo {
        public string IdMateriaOrganica { get; set; }
        public double MatOrgVal { get; set; }
    }

    [TableName("ElementosGruesosTipo")]
    [PrimaryKey("IdElementosGruesos", AutoIncrement = false)]
    public class ElementosGruesosTipo {
        public string IdElementosGruesos { get; set; }
        public double EleGruVal { get; set; }
    }

    [TableName("RiegoTipo")]
    [PrimaryKey("IdTipoRiego", AutoIncrement = false)]
    public class RiegoTipo {
        public int IdTipoRiego { get; set; }
        public string TipoRiego { get; set; }
        public double Eficiencia { get; set; }
        public double PluviometriaTipica { get; set; }
    }

    public class LineaBalance {
        public DateTime? Fecha { get; set; } = null;
        public int DiasDesdeSiembra { get; set; }
        public double IntegralTermica { get; set; }
        public double TasaCrecimientoCobertura { get; set; }
        public double Cobertura { get; set; }
        public double TasaCrecimientoAltura { get; set; }
        public double AlturaCultivo { get; set; }
        public double DiasMaduracion { get; set; }
        public int NumeroEtapaDesarrollo { get; set; } = 1;
        public string NombreEtapaDesarrollo { get; set; }
        public double Kc { get; set; }
        public double KcAjustadoClima { get; set; }
        public double CoeficienteEstresHidrico { get; set; }
        public double EtcFinal { get; set; }
        public double LongitudRaiz { get; set; }
        public double AguaDisponibleTotal { get; set; }
        public double FraccionAgotamiento { get; set; }
        public double AguaFacilmenteExtraible { get; set; }
        public double AguaFacilmenteExtraibleFija { get; set; }
        public double AgotamientoInicioDia { get; set; }
        public double Lluvia { get; set; }
        public double LluviaEfectiva { get; set; }
        public double Riego { get; set; }
        public double RiegoEfectivo { get; set; }
        public double DrenajeProfundidad { get; set; }
        public double AgotamientoFinalDia { get; set; }

        public double CapacidadCampo { get; set; }
        public double PuntoMarchitez { get; set; }
        public double ContenidoAguaSuelo { get; set; }
        public double LimiteAgotamiento { get; set; }
        public double LimiteAgotamientoFijo { get; set; }

        public double CapacidadCampoRefPM { get; set; }
        public double PuntoMarchitezRefPM { get; set; }
        public double ContenidoAguaSueloRefPM { get; set; }
        public double LimiteAgotamientoRefPM { get; set; }
        public double LimiteAgotamientoFijoRefPM { get; set; }

        public double AguaCrecRaiz { set; get; }

        public double RecomendacionRiegoTiempo { get; set; }
        public double RecomendacionRiegoNeto { set; get; }
        public double RecomendacionRiegoBruto { set; get; }

        public double IndiceEstres { set; get; }
        public string MensajeEstres { set; get; }
        public string DescripcionEstres { set; get; }
        public string ColorEstres { set; get; }
    }

    [TableName("Temporada")]
    [PrimaryKey("IdTemporada", AutoIncrement = false)]
    public class Temporada {
        public string IdTemporada { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public double CosteM3Agua { get; set; }
        public bool? Activa { get; set; }
    }

    public class DatosEstadoHidrico {
        public DateTime Fecha { set; get; }
        public string IdUnidadCultivo { set; get; }
        public int? IdRegante { set; get; }
        public string Municipios { set; get; }
        public string Parajes { set; get; }
        public double? AguaUtil { set; get; }
        public int? RegarEnNDias { set; get; }
        public double? AguaUtilTotal { set; get; }
        public double? Pluviometria { set; get; }
        public double? CapacidadDeCampo { set; get; }
        public double? PuntoDeMarchited { set; get; }
        public double? AguaUtilOptima { set; get; }
        public double? AguaPerdida { set; get; }
        public double? CosteAgua { set; get; }
        public int? NDiasEstres { set; get; }
        public double? EstadoHidrico { set; get; }
        public string TipoRiego { set; get; }
        public DateTime? FechaSiembra { set; get; }
        public int? IdEstacion { set; get; }
        public string Estacion { set; get; }
        public string Cultivo { set; get; }
        public double? SumaRiego { set; get; }
        public double? SumaLluvia { set; get; }
        public string Status { set; get; }

        public string IdTemporada { set; get; }
        public string Regante { set; get; }
        public string NIF { set; get; }
        public string Telefono { set; get; }
        public string TelefonoSMS { set; get; }
        public int? IdCultivo { set; get; }
        public int? IdTipoRiego { set; get; }
        public double? Eficiencia { set; get; }
        public string Alias { set; get; }
        public int? NParcelas { set; get; }
        public double? SuperficieM2 { set; get; }
        public string Textura { set; get; }

        public double IndiceEstres { set; get; }
        public string MensajeEstres { set; get; }
        public string DescripcionEstres { set; get; }
        public string ColorEstres { set; get; }

        public string GeoLocJson { set; get; }  // List<GeoLocParcela> ->Json 
        public double Consumo { get; set; }
        public double AguaTotalPerdidaDrenaje { get; set; }
        public int NumDiasEstresPorDrenaje { set; get; }
        public int NumCambiosDeEtapaPendientesDeConfirmar { get; set; }
    }

    public class ResumenDiario {
        public DateTime FechaDeCalculo { set; get; }
        public double RiegoTotal { set; get; }
        public double RiegoEfectivoTotal { set; get; }
        public double LluviaTotal { set; get; }
        public double LluviaEfectivaTotal { set; get; }
        public double AguaPerdida { set; get; }
        public double ConsumoAguaCultivo { set; get; }
        public int DiasEstres { set; get; }
        public double DeficitRiego { set; get; }
        public double CosteDeficitRiego { set; get; }
        public double CosteAguaRiego { set; get; }
        public double CosteAguaDrenaje { set; get; }

        public double CapacidadCampo { set; get; }
        public double LimiteAgotamiento { set; get; }
        public double PuntoMarchitez { set; get; }
        public double ContenidoAguaSuelo { set; get; }

        public double CapacidadCampoPorcentaje { set; get; }
        public double LimiteAgotamientoPorcentaje { set; get; }
        public double PuntoMarchitezPorcentaje { set; get; }
        public double ContenidoAguaSueloPorcentaje { set; get; }

        public double DrenajeProfundidad { set; get; }
        public bool AvisoDrenaje { set; get; }

        public double AguaHastaCapacidadCampo { set; get; }
        public double RecomendacionRiegoNeto { set; get; }
        public double RecomendacionRiegoBruto { set; get; }
        public double RecomendacionRiegoTiempo { set; get; }

        public double IndiceEstres { set; get; }
        public string MensajeEstres { set; get; }
        public string DescripcionEstres { set; get; }
        public string ColorEstres { set; get; }

        public double CapacidadCampoRefPM { get; set; }
        public double PuntoMarchitezRefPM { get; set; }
        public double ContenidoAguaSueloRefPM { get; set; }
        public double LimiteAgotamientoRefPM { get; set; }
        public double LimiteAgotamientoFijoRefPM { get; set; }

        public double Altura { get;  set; }
        public double Cobertura { get;  set; }
        public string NombreEtapaDesarrollo { get;  set; }
        public int NumeroEtapaDesarrollo { get;  set; }
        public double LongitudRaiz { get;  set; }
        public double AlturaFinal { get;  set; }
        public double AlturaInicial { get;  set; }
        public double ProfRaizInicial { get;  set; }
        public double ProfRaizMaxima { get;  set; }
    }

    public class GeoLocParcela {
        public int IdParcelaInt { set; get; }
        public int IdMunicipio { set; get; }
        public string Municipio { set; get; }
        public int IdPoligono { set; get; }
        public int IdParcela { set; get; }
        public int GID { set; get; }
        public string GEO { set; get; }
    }


    public class DatosLLuviaORiego {
        public string IdTipoAportacion;//Lluvia  o Riego
        public DateTime Fecha { set; get; }
        public double Mm { set; get; }
        public double M3 { set; get; }
        public string Obtencion { set; get; } // puede se (A)portación o (S)istema metereológico.
        public string IdUnidadCultivo { set; get; }
        public string IdTemporada { set; get; }
        public string UnidadCultivo { set; get; }
        public string IdEstacion { set; get; }
        public string Estacion { set; get; }
    }

    public class DatosLLuvia {
        public DateTime Fecha { set; get; }
        public double Mm { set; get; }
        public string Obtencion { set; get; } // puede se (A)portación o (S)istema metereológico.
        public string IdUnidadCultivo { set; get; }
        public string IdTemporada { set; get; }
        public string UnidadCultivo { set; get; }
        public string IdEstacion { set; get; }
        public string Estacion { set; get; }
    }

    public class DatosRiego {
        public DateTime Fecha { set; get; }
        public double M3 { set; get; }
        public double Mm { set; get; }
        public string Obtencion; // puede se (A)portación o (S)istema de riego de la comunidad.
        public string IdTemporada { set; get; }
        public string IdUnidadCultivo { set; get; }
        public string UnidadCultivo { set; get; }
    }

    public class BalanceData {
        public string IdUnidadCultivo { get; set; }
        public string Alias { get; set; }
        public string TipoSueloDescripcion { get; set; }
        public string IdTemporada { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }
        public int IdRegante { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaSiembra { get; set; }
        public double? Pluviometria { get; set; }
        public string CultivoNombre { get; set; }
        public int IdCultivo { get; set; }
        public double TBase { get; set; }
        public double ProfRaizInicial { get; set; }
        public double ProfRaizMax { get; set; }
        public double ModCobCoefA { get; set; }
        public double ModCobCoefB { get; set; }
        public double? ModCobCoefC { get; set; }
        public double ModAltCoefA { get; set; }
        public double ModAltCoefB { get; set; }
        public double? ModAltCoefC { get; set; }
        public double ModRaizCoefA { get; set; }
        public double ModRaizCoefB { get; set; }
        public double? ModRaizCoefC { get; set; }
        public double? AlturaInicial { get; set; }
        public double? AlturaFinal { get; set; }
        public double? IntegralEmergencia { get; set; }
        public int IdEtapaCultivo { get; set; }
        public string Etapa { get; set; }
        public DateTime FechaInicioEtapa { get; set; }
        public DateTime? FechaFinEtapaConfirmada { get; set; }
        public bool DefinicionPorDias { get; set; }
        public double? KcInicial { get; set; }
        public double? KcFinal { get; set; }
        public double? CobInicial { get; set; }
        public double? FactorDeAgotamiento { get; set; }
        public double? CobFinal { get; set; }
        public int IdTipoRiego { get; set; }
        public string TipoRiego { get; set; }
        public double Eficiencia { get; set; }
        public double PluviometriaTipica { get; set; }
        public int IdHorizonte { get; set; }
        public double ProfundidadHorizonte { get; set; }
        public double Limo { get; set; }
        public double Arcilla { get; set; }
        public double Arena { get; set; }
        public double ElementosGruesos { get; set; }
        public double MateriaOrganica { get; set; }
        public int OrdenEtapa { get; set; }
        public string EtapaNombre { get; set; }
        public int DuracionDiasEtapa { get; set; }
        public double? CultivoEtapasKcInicial { get; set; }
        public double? CultivoEtapasKcFinal { get; set; }
        public bool CultivoEtapasDefinicionPorDias { get; set; }
        public double? CultivoEtapasCobInicial { get; set; }
        public double? CultivoEtapasFinal { get; set; }
        public double? FactorAgotamiento { get; set; }
    }

    [TableName("UnidadCultivoTemporadaCosteAgua")]
    [PrimaryKey("IdUnidadCultivo,IdTemporada", AutoIncrement = false)]
    public class ParamPostCosteM3Agua {
        public string IdUnidadCultivo { set; get; }
        public string IdTemporada { set; get; }
        public float? CosteM3Agua { set; get; }
    }

    [TableName("TipoEstres")]
    [PrimaryKey("IdTipoEstres", AutoIncrement = false)]
    public class TipoEstres {
        public string IdTipoEstres { get; set; }
        public string Estres { get; set; }
        public int? IdUmbralInferiorRiego { get; set; }
        public int? IdUmbralSuperiorRiego { get; set; }
    }

    [TableName("TipoEstresUmbral")]
    [PrimaryKey("IdTipoEstres,IdUmbral", AutoIncrement = false)]
    public class TipoEstresUmbral {
        public string IdTipoEstres { get; set; }
        public int IdUmbral { get; set; }
        public string Descripcion { get; set; }
        public string Mensaje { get; set; }
        public double UmbralMaximo { get; set; }
        public string Color { get; set; }
    }

    public class ParamPostUnidadCultivoSuelo {
        public string Fecha { set; get; }
        public string IdUnidadCultivo { set; get; }
        public string IdSueloTipo { set; get; }
    }

    //Establecer la pluviometria para una unidad de cultivo.
    public class ParamPostPluviometria {
        public string Fecha { set; get; }
        public string IdUnidadCultivo { set; get; }
        public double Valor { set; get; }
    }

    public class ParamPostUnidadCultivoCultivo {
        public string IdUnidadCultivo { set; get; }
        public string Fecha { set; get; }
        public int IdCultivo { set; get; }
        public int IdRegante { set; get; }
        public int IdTipoRiego { set; get; }
        public string FechaSiembra { set; get; }
    }

    public class UnidadCultivoConSuperficieYGeoLoc {
        public string IdUnidadCultivo { get; set; }
        public int? IdRegante { get; set; }
        public string Alias { get; set; }
        public double? SuperficieM2 { get; set; }
        public string GeoLocJson { set; get; }  // List<GeoLocParcela> ->Json 
        public string ParcelasValvulasJson { set; get; }  // List<UnidadCultivoParcelasValvulas> ->Json 
    }

    public class ProvinciaMunicipioParaje {
        public string Provincia { set; get; }
        public string Municipio { set; get; }
        public string Paraje { set; get; }
    }

    public class UnidadCultivoDatosAmpliados {
        public string IdUnidadCultivo { get; set; }
        public string IdTemporada { get; set; }
        public int IdRegante { get; set; }
        public int IdCultivo { get; set; }
        public int IdTipoRiego { get; set; }
        public DateTime? FechaSiembra { get; set; }
        public double? Pluviometria { get; set; }
        public string TemporadaDescripcion { get; set; }
        public string Nombre { get; set; }
        public string NombreEstacion { get; set; }
        public string TipoRiego { get; set; }
        public double EficienciaRiego { get; set; }
        public double PluviometriaTipicaTipoRiego { get; set; }
        public int IdEstacion { get; set; }
        public string Alias { get; set; }
        public string Provincia { get; set; }
        public string Municipio { get; set; }
        public string Paraje { get; set; }
        public string TipoSueloDescripcion { get; set; }
    }

    public class Valvula {
        public int IdValvula { set; get; }
    }

    public class UnidadDeCultivoParcelasValvulas {
        public string IdUnidadCultivo { get; set; }
        public string IdTemporada { get; set; }
        public int IdParcelaInt { get; set; }        
        public double SuperficieM2 { get; set; }
        public string RefCatastral { get; set; }
        public int IdProvincia { get; set; }
        public int IdPoligono { get; set; }
        public string IdParcela { get; set; }
        public string Paraje { get; set; }
        public string Municipio { get; set; }
        public List<Valvula> LIdValvula { get; set; }
    }

    [TableName("Multimedia")]
    [PrimaryKey("IdMultimedia", AutoIncrement = true)]
    public class Multimedia {
        public int IdMultimedia { get; set; }
        public int IdMultimediaTipo { get; set; }
        public string Autor { get; set; }
        public DateTime? Fecha { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Url { get; set; }
        public DateTime? Expira { get; set; }
    }

    public class MultimediaPost {
        public int IdMultimedia { get; set; }
        public int IdMultimediaTipo { get; set; }
        public string Autor { get; set; }
        public string Fecha { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Url { get; set; }
        public string Expira { get; set; }
    }


    [TableName("MultimediaTipo")]
    [PrimaryKey("IdMultimediaTipo", AutoIncrement = true)]
    public class Multimedia_Tipo {
        public int IdMultimediaTipo { get; set; }
        public string MultimediaTipo { get; set; }
        public string Icono { get; set; }
    }

    [TableName("Eventos")]
    [PrimaryKey("IdEvento", AutoIncrement = true)]
    public class EventosPoco {
        public int IdEvento { get; set; }
        public string Evento { get; set; }
    }
}

