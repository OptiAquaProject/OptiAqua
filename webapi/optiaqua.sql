USE [master]
GO
/****** Object:  Database [OPTIAQUA]    Script Date: 12/11/2019 11:51:24 ******/
CREATE DATABASE [OPTIAQUA]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'OPTIRED', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLOPTIAQUA\MSSQL\DATA\OPTIRED3.mdf' , SIZE = 36864KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'OPTIRED_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLOPTIAQUA\MSSQL\DATA\OPTIRED3_log.ldf' , SIZE = 24384KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [OPTIAQUA] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OPTIAQUA].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OPTIAQUA] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [OPTIAQUA] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [OPTIAQUA] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [OPTIAQUA] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [OPTIAQUA] SET ARITHABORT OFF 
GO
ALTER DATABASE [OPTIAQUA] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [OPTIAQUA] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [OPTIAQUA] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [OPTIAQUA] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [OPTIAQUA] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [OPTIAQUA] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [OPTIAQUA] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [OPTIAQUA] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [OPTIAQUA] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [OPTIAQUA] SET  DISABLE_BROKER 
GO
ALTER DATABASE [OPTIAQUA] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [OPTIAQUA] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [OPTIAQUA] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [OPTIAQUA] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [OPTIAQUA] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [OPTIAQUA] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [OPTIAQUA] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [OPTIAQUA] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [OPTIAQUA] SET  MULTI_USER 
GO
ALTER DATABASE [OPTIAQUA] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [OPTIAQUA] SET DB_CHAINING OFF 
GO
ALTER DATABASE [OPTIAQUA] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [OPTIAQUA] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [OPTIAQUA] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [OPTIAQUA] SET QUERY_STORE = OFF
GO
USE [OPTIAQUA]
GO
/****** Object:  Table [dbo].[Cultivo]    Script Date: 12/11/2019 11:51:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cultivo](
	[IdCultivo] [int] NOT NULL,
	[Nombre] [nvarchar](20) NOT NULL,
	[TBase] [float] NOT NULL,
	[ProfRaizInicial] [float] NOT NULL,
	[ProfRaizMax] [float] NOT NULL,
	[ModCobCoefA] [float] NOT NULL,
	[ModCobCoefB] [float] NOT NULL,
	[ModCobCoefC] [float] NULL,
	[ModAltCoefA] [float] NOT NULL,
	[ModAltCoefB] [float] NOT NULL,
	[ModAltCoefC] [float] NULL,
	[ModRaizCoefA] [float] NOT NULL,
	[ModRaizCoefB] [float] NOT NULL,
	[ModRaizCoefC] [float] NULL,
	[AlturaInicial] [float] NULL,
	[AlturaFinal] [float] NULL,
	[IntegralEmergencia] [float] NULL,
	[EtapaInicioRiego] [int] NOT NULL,
 CONSTRAINT [PK_CultivosFAO] PRIMARY KEY CLUSTERED 
(
	[IdCultivo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoSuelo]    Script Date: 12/11/2019 11:51:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoSuelo](
	[IdTemporada] [nvarchar](10) NOT NULL,
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdHorizonte] [int] NOT NULL,
	[ProfundidadHorizonte] [float] NOT NULL,
	[Limo] [float] NOT NULL,
	[Arcilla] [float] NOT NULL,
	[Arena] [float] NOT NULL,
	[ElementosGruesos] [float] NOT NULL,
	[MateriaOrganica] [float] NOT NULL,
 CONSTRAINT [PK_ParcelaCultivoSuelo] PRIMARY KEY CLUSTERED 
(
	[IdTemporada] ASC,
	[IdUnidadCultivo] ASC,
	[IdHorizonte] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoCultivoEtapas]    Script Date: 12/11/2019 11:51:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoCultivoEtapas](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdTemporada] [nvarchar](10) NOT NULL,
	[IdEtapaCultivo] [int] NOT NULL,
	[IdTipoEstres] [nvarchar](20) NULL,
	[Etapa] [nvarchar](50) NOT NULL,
	[FechaInicioEtapa] [date] NOT NULL,
	[FechaFinEtapaConfirmada] [date] NULL,
	[DefinicionPorDias] [bit] NOT NULL,
	[KcInicial] [float] NOT NULL,
	[KcFinal] [float] NOT NULL,
	[CobInicial] [float] NULL,
	[CobFinal] [float] NULL,
	[FactorDeAgotamiento] [float] NOT NULL,
 CONSTRAINT [PK_ParcelasCultivoFases] PRIMARY KEY CLUSTERED 
(
	[IdUnidadCultivo] ASC,
	[IdTemporada] ASC,
	[IdEtapaCultivo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RiegoTipo]    Script Date: 12/11/2019 11:51:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RiegoTipo](
	[IdTipoRiego] [int] NOT NULL,
	[TipoRiego] [nvarchar](250) NOT NULL,
	[Eficiencia] [float] NOT NULL,
	[PluviometriaTipica] [float] NOT NULL,
 CONSTRAINT [PK_TiposRiego] PRIMARY KEY CLUSTERED 
(
	[IdTipoRiego] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Regante]    Script Date: 12/11/2019 11:51:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regante](
	[IdRegante] [int] NOT NULL,
	[IdGadmin] [int] NULL,
	[NIF] [nvarchar](15) NULL,
	[Nombre] [nvarchar](90) NULL,
	[Direccion] [nvarchar](200) NULL,
	[CodigoPostal] [nchar](8) NULL,
	[Poblacion] [nvarchar](50) NULL,
	[Provincia] [nvarchar](30) NULL,
	[Pais] [nvarchar](30) NULL,
	[Contraseña] [nvarchar](50) NULL,
	[Role] [nvarchar](20) NULL,
	[TelefonoSMS] [nvarchar](100) NULL,
	[Telefono] [nvarchar](100) NULL,
	[Email] [nvarchar](255) NULL,
	[WebActive] [bit] NULL,
 CONSTRAINT [PK_Regante] PRIMARY KEY CLUSTERED 
(
	[IdRegante] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoCultivo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoCultivo](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdTemporada] [nvarchar](10) NOT NULL,
	[IdRegante] [int] NOT NULL,
	[IdCultivo] [int] NOT NULL,
	[IdTipoRiego] [int] NOT NULL,
	[FechaSiembra] [date] NULL,
	[Pluviometria] [float] NULL,
 CONSTRAINT [PK_ParcelasCultivo] PRIMARY KEY CLUSTERED 
(
	[IdUnidadCultivo] ASC,
	[IdTemporada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivo](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdRegante] [int] NULL,
	[IdEstacion] [int] NOT NULL,
	[Alias] [nvarchar](30) NULL,
	[TipoSueloDescripcion] [nvarchar](30) NULL,
 CONSTRAINT [PK_UnidadCultivo] PRIMARY KEY CLUSTERED 
(
	[IdUnidadCultivo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Temporada]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Temporada](
	[IdTemporada] [nvarchar](10) NOT NULL,
	[Descripcion] [nvarchar](50) NULL,
	[FechaInicial] [date] NULL,
	[FechaFinal] [date] NULL,
	[CosteM3Agua] [float] NULL,
	[Activa] [bit] NULL,
 CONSTRAINT [PK_Temporada] PRIMARY KEY CLUSTERED 
(
	[IdTemporada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CultivoEtapas]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CultivoEtapas](
	[IdCultivo] [int] NOT NULL,
	[OrdenEtapa] [int] NOT NULL,
	[IdTipoEstres] [nvarchar](20) NULL,
	[Etapa] [nvarchar](50) NOT NULL,
	[DuracionDiasEtapa] [int] NOT NULL,
	[KcInicial] [float] NOT NULL,
	[KcFinal] [float] NOT NULL,
	[DefinicionPorDias] [bit] NOT NULL,
	[CobInicial] [float] NULL,
	[CobFinal] [float] NULL,
	[FactorAgotamiento] [float] NOT NULL,
	[LimSupPeligro] [float] NULL,
	[LimSupPerdidas] [float] NULL,
	[LimSupAlerta] [float] NULL,
	[LimSupVigilar] [float] NULL,
 CONSTRAINT [PK_CultivoFases] PRIMARY KEY CLUSTERED 
(
	[IdCultivo] ASC,
	[OrdenEtapa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[BalanceData]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[BalanceData]
AS
SELECT        dbo.UnidadCultivo.IdUnidadCultivo, dbo.UnidadCultivo.Alias, dbo.UnidadCultivo.TipoSueloDescripcion, dbo.Temporada.IdTemporada, dbo.Temporada.Descripcion, dbo.Temporada.FechaInicial, dbo.Temporada.FechaFinal, 
                         dbo.Regante.IdRegante, dbo.Regante.Nombre, dbo.UnidadCultivoCultivo.FechaSiembra, dbo.UnidadCultivoCultivo.Pluviometria, dbo.Cultivo.Nombre AS CultivoNombre, dbo.Cultivo.IdCultivo, dbo.Cultivo.TBase, 
                         dbo.Cultivo.ProfRaizInicial, dbo.Cultivo.ProfRaizMax, dbo.Cultivo.ModCobCoefA, dbo.Cultivo.ModCobCoefB, dbo.Cultivo.ModCobCoefC, dbo.Cultivo.ModAltCoefA, dbo.Cultivo.ModAltCoefB, dbo.Cultivo.ModAltCoefC, 
                         dbo.Cultivo.ModRaizCoefA, dbo.Cultivo.ModRaizCoefB, dbo.Cultivo.ModRaizCoefC, dbo.Cultivo.AlturaInicial, dbo.Cultivo.AlturaFinal, dbo.Cultivo.IntegralEmergencia, dbo.UnidadCultivoCultivoEtapas.IdEtapaCultivo, 
                         dbo.UnidadCultivoCultivoEtapas.Etapa, dbo.UnidadCultivoCultivoEtapas.FechaInicioEtapa, dbo.UnidadCultivoCultivoEtapas.FechaFinEtapaConfirmada, dbo.UnidadCultivoCultivoEtapas.DefinicionPorDias, 
                         dbo.UnidadCultivoCultivoEtapas.KcInicial, dbo.UnidadCultivoCultivoEtapas.KcFinal, dbo.UnidadCultivoCultivoEtapas.CobInicial, dbo.UnidadCultivoCultivoEtapas.FactorDeAgotamiento, dbo.UnidadCultivoCultivoEtapas.CobFinal, 
                         dbo.RiegoTipo.IdTipoRiego, dbo.RiegoTipo.TipoRiego, dbo.RiegoTipo.Eficiencia, dbo.RiegoTipo.PluviometriaTipica, dbo.UnidadCultivoSuelo.IdHorizonte, dbo.UnidadCultivoSuelo.ProfundidadHorizonte, 
                         dbo.UnidadCultivoSuelo.Limo, dbo.UnidadCultivoSuelo.Arcilla, dbo.UnidadCultivoSuelo.Arena, dbo.UnidadCultivoSuelo.ElementosGruesos, dbo.UnidadCultivoSuelo.MateriaOrganica, dbo.CultivoEtapas.OrdenEtapa, 
                         dbo.CultivoEtapas.Etapa AS EtapaNombre, dbo.CultivoEtapas.DuracionDiasEtapa, dbo.CultivoEtapas.KcInicial AS CultivoEtapasKcInicial, dbo.CultivoEtapas.KcFinal AS CultivoEtapasKcFinal, 
                         dbo.CultivoEtapas.DefinicionPorDias AS CultivoEtapasDefinicionPorDias, dbo.CultivoEtapas.CobInicial AS CultivoEtapasCobInicial, dbo.CultivoEtapas.CobFinal AS CultivoEtapasFinal, dbo.CultivoEtapas.FactorAgotamiento
FROM            dbo.UnidadCultivo INNER JOIN
                         dbo.UnidadCultivoCultivo ON dbo.UnidadCultivo.IdUnidadCultivo = dbo.UnidadCultivoCultivo.IdUnidadCultivo INNER JOIN
                         dbo.UnidadCultivoCultivoEtapas ON dbo.UnidadCultivoCultivo.IdUnidadCultivo = dbo.UnidadCultivoCultivoEtapas.IdUnidadCultivo AND 
                         dbo.UnidadCultivoCultivo.IdTemporada = dbo.UnidadCultivoCultivoEtapas.IdTemporada INNER JOIN
                         dbo.Cultivo ON dbo.UnidadCultivoCultivo.IdCultivo = dbo.Cultivo.IdCultivo AND dbo.UnidadCultivoCultivo.IdCultivo = dbo.Cultivo.IdCultivo INNER JOIN
                         dbo.Temporada ON dbo.UnidadCultivoCultivo.IdTemporada = dbo.Temporada.IdTemporada INNER JOIN
                         dbo.Regante ON dbo.UnidadCultivoCultivo.IdRegante = dbo.Regante.IdRegante INNER JOIN
                         dbo.RiegoTipo ON dbo.UnidadCultivoCultivo.IdTipoRiego = dbo.RiegoTipo.IdTipoRiego INNER JOIN
                         dbo.UnidadCultivoSuelo ON dbo.UnidadCultivo.IdUnidadCultivo = dbo.UnidadCultivoSuelo.IdUnidadCultivo INNER JOIN
                         dbo.CultivoEtapas ON dbo.Cultivo.IdCultivo = dbo.CultivoEtapas.IdCultivo
GO
/****** Object:  Table [dbo].[Aviso]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Aviso](
	[IdAviso] [int] IDENTITY(1,1) NOT NULL,
	[Titulo] [nvarchar](500) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Expira] [datetime] NULL,
	[De] [nvarchar](500) NOT NULL,
	[Para] [nvarchar](500) NOT NULL,
	[Mensaje] [nvarchar](max) NOT NULL,
	[IdAvisoTipo] [int] NOT NULL,
	[Leido] [bit] NOT NULL,
 CONSTRAINT [PK_Aviso] PRIMARY KEY CLUSTERED 
(
	[IdAviso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AvisoTipo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AvisoTipo](
	[IdAvisoTipo] [int] IDENTITY(1,1) NOT NULL,
	[AvisoTipo] [nvarchar](50) NOT NULL,
	[Icono] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AvisoTipo] PRIMARY KEY CLUSTERED 
(
	[IdAvisoTipo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[AvisosList]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[AvisosList] 
(	
	@IdAviso NVARCHAR(50)
	, @IdAvisoTipo INT
	, @FInicio DATETIME
	, @FFin DATETIME
	, @De NVARCHAR(100)
	, @Para NVARCHAR(100)
	, @Search  NVARCHAR(100)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM
	(
		SELECT		A.idAviso
					, A.titulo
					, A.de
					, A.para
					, A.fecha
					, A.mensaje
					, A.idAvisoTipo
					, A.leido
					, AT.avisoTipo
					, AT.icono
		FROM		Aviso A
					INNER JOIN AvisoTipo AT ON A.IdAvisoTipo = AT.IdAvisoTipo
		WHERE		(@IdAviso = 0 OR A.IdAviso=@IdAviso)
					AND (@IdAvisoTipo = 0 OR A.IdAvisoTipo=@IdAvisoTipo)
					AND (@FInicio = '' OR A.Fecha>=@FInicio)
					AND (@FFin = '' OR A.Fecha<=@FFin)
					AND (@De = '' OR A.De=@De)
					AND (@Para = '' OR A.Para=@Para)
	) AS T
	WHERE	(@Search='' OR UPPER(CAST(CONCAT(Titulo, ' ', Mensaje) AS VARCHAR(MAX))) COLLATE SQL_Latin1_General_Cp1251_CS_AS Like UPPER(Cast('%' + @Search + '%' as varchar(max))) collate SQL_Latin1_General_Cp1251_CS_AS )

)
GO
/****** Object:  Table [dbo].[Paraje]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Paraje](
	[IdParaje] [int] NOT NULL,
	[IdProvincia] [nchar](2) NULL,
	[IdMunucipio] [int] NULL,
	[Paraje] [nvarchar](50) NULL,
 CONSTRAINT [PK_Paraje] PRIMARY KEY CLUSTERED 
(
	[IdParaje] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Municipio]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Municipio](
	[IdProvincia] [nchar](2) NOT NULL,
	[IdMunicipio] [int] NOT NULL,
	[Municipio] [nvarchar](50) NULL,
 CONSTRAINT [PK_Municipio] PRIMARY KEY CLUSTERED 
(
	[IdMunicipio] ASC,
	[IdProvincia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoParcela]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoParcela](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdTemporada] [nvarchar](10) NOT NULL,
	[IdParcelaInt] [int] NOT NULL,
	[IdRegante] [int] NOT NULL,
 CONSTRAINT [PK_UnidadCultivoParcela] PRIMARY KEY CLUSTERED 
(
	[IdUnidadCultivo] ASC,
	[IdTemporada] ASC,
	[IdParcelaInt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Estacion]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estacion](
	[IdEstacion] [int] NOT NULL,
	[Nombre] [nvarchar](150) NOT NULL,
	[IdRed] [int] NOT NULL,
	[IdProvincia] [tinyint] NOT NULL,
	[IdTermino] [int] NOT NULL,
	[Longitud] [int] NULL,
	[Latitud] [int] NULL,
	[XUTM] [int] NULL,
	[YUTM] [int] NULL,
	[Huso] [int] NULL,
	[Altitud] [float] NULL,
 CONSTRAINT [PK_Estaciones] PRIMARY KEY CLUSTERED 
(
	[IdEstacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Parcela]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parcela](
	[IdParcelaInt] [int] NOT NULL,
	[IdGadmin] [int] NULL,
	[IdRegante] [int] NULL,
	[IdProvincia] [int] NOT NULL,
	[IdMunicipio] [int] NULL,
	[IdPoligono] [int] NOT NULL,
	[IdParcela] [nchar](5) NOT NULL,
	[IdParaje] [int] NULL,
	[Descripcion] [nvarchar](50) NOT NULL,
	[Longitud] [real] NULL,
	[Latitud] [real] NULL,
	[XUTM] [real] NULL,
	[YUTM] [real] NULL,
	[Huso] [int] NULL,
	[Altitud] [float] NULL,
	[SuperficieM2] [float] NOT NULL,
	[RefCatastral] [nchar](40) NULL,
	[GEO] [geometry] NULL,
	[GID] [int] NOT NULL,
 CONSTRAINT [PK_Parcelas_1] PRIMARY KEY CLUSTERED 
(
	[IdParcelaInt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Provincia]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Provincia](
	[IdProvincia] [nchar](2) NOT NULL,
	[Provincia] [nvarchar](30) NULL,
 CONSTRAINT [PK_Provincia] PRIMARY KEY CLUSTERED 
(
	[IdProvincia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UnidadCultivoDatosAmpliados]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UnidadCultivoDatosAmpliados]
AS
SELECT DISTINCT 
                         dbo.UnidadCultivoCultivo.IdUnidadCultivo, dbo.UnidadCultivoCultivo.IdTemporada, dbo.UnidadCultivoCultivo.IdRegante, dbo.UnidadCultivoCultivo.IdCultivo, dbo.UnidadCultivoCultivo.IdTipoRiego, 
                         dbo.UnidadCultivoCultivo.FechaSiembra, dbo.UnidadCultivoCultivo.Pluviometria, dbo.Temporada.Descripcion AS TemporadaDescripcion, dbo.Regante.Nombre, dbo.Estacion.Nombre AS NombreEstacion, 
                         dbo.RiegoTipo.TipoRiego, dbo.RiegoTipo.Eficiencia AS EficienciaRiego, dbo.RiegoTipo.PluviometriaTipica AS PluviometriaTipicaTipoRiego, dbo.UnidadCultivo.IdEstacion, dbo.UnidadCultivo.Alias, dbo.Provincia.Provincia, 
                         dbo.Municipio.Municipio, dbo.Paraje.Paraje, dbo.UnidadCultivo.TipoSueloDescripcion
FROM            dbo.Parcela INNER JOIN
                         dbo.Provincia INNER JOIN
                         dbo.Paraje ON dbo.Provincia.IdProvincia = dbo.Paraje.IdProvincia ON dbo.Parcela.IdParaje = dbo.Paraje.IdParaje INNER JOIN
                         dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt = dbo.UnidadCultivoParcela.IdParcelaInt INNER JOIN
                         dbo.UnidadCultivo INNER JOIN
                         dbo.Estacion ON dbo.UnidadCultivo.IdEstacion = dbo.Estacion.IdEstacion AND dbo.UnidadCultivo.IdEstacion = dbo.Estacion.IdEstacion INNER JOIN
                         dbo.UnidadCultivoCultivo ON dbo.UnidadCultivo.IdUnidadCultivo = dbo.UnidadCultivoCultivo.IdUnidadCultivo INNER JOIN
                         dbo.RiegoTipo ON dbo.UnidadCultivoCultivo.IdTipoRiego = dbo.RiegoTipo.IdTipoRiego ON dbo.UnidadCultivoParcela.IdUnidadCultivo = dbo.UnidadCultivo.IdUnidadCultivo INNER JOIN
                         dbo.Temporada ON dbo.UnidadCultivoParcela.IdTemporada = dbo.Temporada.IdTemporada AND dbo.UnidadCultivoParcela.IdTemporada = dbo.Temporada.IdTemporada AND 
                         dbo.UnidadCultivoCultivo.IdTemporada = dbo.Temporada.IdTemporada INNER JOIN
                         dbo.Regante ON dbo.Parcela.IdRegante = dbo.Regante.IdRegante AND dbo.UnidadCultivo.IdRegante = dbo.Regante.IdRegante AND dbo.UnidadCultivoCultivo.IdRegante = dbo.Regante.IdRegante INNER JOIN
                         dbo.Municipio ON dbo.Paraje.IdMunucipio = dbo.Municipio.IdMunicipio
GO
/****** Object:  View [dbo].[MaxAñoUnidadCultivo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MaxAñoUnidadCultivo]
AS
SELECT        dbo.UnidadCultivoParcela.IdUnidadCultivo, MAX(dbo.Temporada.FechaFinal) AS MAXAÑO
FROM            dbo.UnidadCultivoParcela INNER JOIN
                         dbo.Temporada ON dbo.UnidadCultivoParcela.IdTemporada = dbo.Temporada.IdTemporada
GROUP BY dbo.UnidadCultivoParcela.IdUnidadCultivo
GO
/****** Object:  View [dbo].[FiltroParcelasDatosHidricos]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[FiltroParcelasDatosHidricos]
AS
SELECT DISTINCT dbo.UnidadCultivoCultivo.IdUnidadCultivo, dbo.UnidadCultivoCultivo.IdTemporada, dbo.UnidadCultivoCultivo.IdCultivo, dbo.Parcela.IdMunicipio, dbo.UnidadCultivoCultivo.IdRegante
FROM            dbo.UnidadCultivoCultivo INNER JOIN
                         dbo.UnidadCultivoParcela ON dbo.UnidadCultivoCultivo.IdUnidadCultivo = dbo.UnidadCultivoParcela.IdUnidadCultivo AND dbo.UnidadCultivoCultivo.IdTemporada = dbo.UnidadCultivoParcela.IdTemporada INNER JOIN
                         dbo.Parcela ON dbo.UnidadCultivoParcela.IdParcelaInt = dbo.Parcela.IdParcelaInt
GO
/****** Object:  UserDefinedFunction [dbo].[ReganteList]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[ReganteList] 
(	
	@IdTemporada NVARCHAR(50)
	, @IdRegante INT
	, @IdUnidadCultivo NVARCHAR(10)
	, @IdParcela INT
	, @Search  NVARCHAR(100)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT        Regante.IdRegante
				, Regante.IdGadmin
				, COALESCE(Regante.NIF,'') NIF
				, COALESCE(Regante.Nombre,'') Nombre
				, COALESCE(Regante.Direccion,'') Direccion
				, COALESCE(Regante.CodigoPostal,'') CodigoPostal
				, COALESCE(Regante.Poblacion,'') Poblacion
				, COALESCE(Regante.Provincia,'') Provincia
				, COALESCE(Regante.Pais,'') Pais
				, COALESCE(Regante.Contraseña,'') Contraseña
				, COALESCE(Regante.Role,'') Role
				, COALESCE(Regante.TelefonoSMS,'') TelefonoSMS
				, COALESCE(Regante.Telefono,'') Telefono
				, COALESCE(Regante.Email,'') Email
				, Regante.WebActive
				, COUNT(DISTINCT UnidadCultivoCultivo.IdUnidadCultivo) AS NUnidadesCultivo
				, COUNT(DISTINCT UnidadCultivoParcela.IdParcelaInt) AS NParcelas
				, COALESCE(UnidadCultivoCultivo.IdTemporada,'') IdTemporada
	FROM        Regante 
				LEFT JOIN UnidadCultivoCultivo ON Regante.IdRegante = UnidadCultivoCultivo.IdRegante 
				INNER JOIN UnidadCultivo ON UnidadCultivoCultivo.IdUnidadCultivo = UnidadCultivo.IdUnidadCultivo 
				INNER JOIN UnidadCultivoParcela ON UnidadCultivo.IdUnidadCultivo = UnidadCultivoParcela.IdUnidadCultivo
	WHERE		(@IdTemporada = '' OR dbo.UnidadCultivoParcela.IdTemporada=@IdTemporada)
				AND (@IdRegante = '' OR dbo.Regante.IdRegante=@IdRegante)
				AND (@IdUnidadCultivo = '' OR UnidadCultivoParcela.IdUnidadCultivo=@IdUnidadCultivo)
				AND (@IdParcela = '' OR UnidadCultivoParcela.IdParcelaInt=@IdParcela)
				AND (@Search='' OR UPPER(CAST(CONCAT(Nombre, ' ', NIF, ' ', TelefonoSMS, ' ', Telefono, ' ', Email,' ', UnidadCultivoCultivo.IdTemporada) AS VARCHAR(MAX))) COLLATE SQL_Latin1_General_Cp1251_CS_AS Like UPPER(Cast('%' + @Search + '%' as varchar(max))) collate SQL_Latin1_General_Cp1251_CS_AS )
	GROUP BY	Regante.IdRegante
				, Regante.IdGadmin
				, Regante.NIF
				, Regante.Nombre
				, Regante.Direccion
				, Regante.CodigoPostal
				, Regante.Poblacion
				, Regante.Provincia
				, Regante.Pais
				, Regante.Contraseña
				, Regante.Role
				, Regante.TelefonoSMS
				, Regante.Telefono
				, Regante.Email
				, Regante.WebActive
				, UnidadCultivoCultivo.IdTemporada	
)
GO
/****** Object:  UserDefinedFunction [dbo].[ParcelaList]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[ParcelaList] 
(	
	@IdTemporada NVARCHAR(50)
	, @IdParcela INT
	, @IdRegante INT
	, @IdMunicipio INT
	, @Search  NVARCHAR(100)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM
	(
		SELECT      dbo.Parcela.IdParcelaInt
					, dbo.Parcela.IdRegante
					, COALESCE(dbo.Regante.Nombre,'') Regante
					, dbo.Parcela.IdProvincia
					, COALESCE(dbo.Provincia.Provincia,'') Provincia
					, dbo.Parcela.IdMunicipio
					, COALESCE(dbo.Municipio.Municipio,'') Municipio
					, dbo.Parcela.IdPoligono
					, dbo.Parcela.IdParcela
					, dbo.Parcela.IdParaje
					, COALESCE(dbo.Paraje.Paraje,'') Paraje
					, COALESCE(dbo.Parcela.Descripcion,'') Descripcion
					, COALESCE(dbo.Parcela.Longitud,0) Longitud
					, COALESCE(dbo.Parcela.Latitud,0)Latitud 
					, COALESCE(dbo.Parcela.XUTM,0) XUTM
					, COALESCE(dbo.Parcela.YUTM,0) YUTM
					, COALESCE(dbo.Parcela.Huso,0) Huso
					, COALESCE(dbo.Parcela.Altitud,0) Altitud
					, COALESCE(dbo.Parcela.SuperficieM2,0) SuperficieM2
					, COALESCE(dbo.Parcela.RefCatastral,'') RefCatastral
		FROM        dbo.Parcela 
					INNER JOIN dbo.Municipio ON dbo.Parcela.IdMunicipio = dbo.Municipio.IdMunicipio 
					INNER JOIN dbo.Provincia ON dbo.Parcela.IdProvincia = dbo.Provincia.IdProvincia
					INNER JOIN dbo.Paraje ON dbo.Parcela.IdParaje = dbo.Paraje.IdParaje 
					INNER JOIN dbo.Regante ON dbo.Parcela.IdRegante = dbo.Regante.IdRegante
					LEFT JOIN dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt= dbo.UnidadCultivoParcela.IdParcelaInt
		WHERE		(@IdTemporada = '' OR dbo.UnidadCultivoParcela.IdTemporada=@IdTemporada)
					AND (@IdParcela = '' OR dbo.Parcela.IdParcelaInt=@IdParcela)
					AND (@IdRegante = '' OR dbo.Regante.IdRegante=@IdRegante)
					AND (@IdMunicipio = '' OR dbo.Municipio.IdMunicipio=@IdMunicipio)
	) AS T
	WHERE	(@Search='' OR UPPER(CAST(CONCAT(Regante, ' ', Paraje, ' ', Municipio, ' ', Provincia, ' ', Descripcion) AS VARCHAR(MAX))) COLLATE SQL_Latin1_General_Cp1251_CS_AS Like UPPER(Cast('%' + @Search + '%' as varchar(max))) collate SQL_Latin1_General_Cp1251_CS_AS )

)
GO
/****** Object:  View [dbo].[UnidadCultivoTemporadaMunicipios]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UnidadCultivoTemporadaMunicipios]
AS
SELECT        IdTemporada, IdUnidadCultivo, STRING_AGG(Municipio, ', ') AS Municipios
FROM            (SELECT DISTINCT TOP (100) PERCENT dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.Municipio.Municipio, dbo.UnidadCultivoParcela.IdTemporada
                          FROM            dbo.Municipio INNER JOIN
                                                    dbo.Paraje ON dbo.Municipio.IdMunicipio = dbo.Paraje.IdMunucipio AND dbo.Municipio.IdProvincia = dbo.Paraje.IdProvincia INNER JOIN
                                                    dbo.Parcela ON dbo.Paraje.IdParaje = dbo.Parcela.IdParaje INNER JOIN
                                                    dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt = dbo.UnidadCultivoParcela.IdParcelaInt
                          GROUP BY dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.Municipio.Municipio, dbo.UnidadCultivoParcela.IdTemporada
                          ORDER BY dbo.UnidadCultivoParcela.IdTemporada, dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.Municipio.Municipio) AS TEMP
GROUP BY IdUnidadCultivo, IdTemporada
GO
/****** Object:  View [dbo].[UnidadCultivoTemporadaParajes]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UnidadCultivoTemporadaParajes]
AS
SELECT        IdTemporada, IdUnidadCultivo, STRING_AGG(Paraje, ', ') AS Parajes
FROM            (SELECT        TOP (100) PERCENT dbo.UnidadCultivoParcela.IdTemporada, dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.Paraje.Paraje
                          FROM            dbo.Paraje INNER JOIN
                                                    dbo.Parcela ON dbo.Paraje.IdParaje = dbo.Parcela.IdParaje INNER JOIN
                                                    dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt = dbo.UnidadCultivoParcela.IdParcelaInt
                          GROUP BY dbo.Paraje.Paraje, dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.UnidadCultivoParcela.IdTemporada
                          ORDER BY dbo.UnidadCultivoParcela.IdTemporada, dbo.UnidadCultivoParcela.IdUnidadCultivo) AS TEMP
GROUP BY IdUnidadCultivo, IdTemporada
GO
/****** Object:  UserDefinedFunction [dbo].[UnidadCultivoList]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[UnidadCultivoList] 
(	
	@IdTemporada NVARCHAR(50)
	, @IdUnidadCultivo NVARCHAR(10)
	, @IdRegante INT
	, @IdCultivo INT
	, @IdMunicipio INT
	, @IdTipoRiego INT
	, @IdEstacion INT
	, @Search  NVARCHAR(100)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT			*
	FROM
	(
		SELECT			T.IdUnidadCultivo
						, T.IdTemporada
						, T.IdRegante
						, T.Regante
						, T.NIF
						, T.Telefono
						, T.TelefonoSMS
						, T.IdCultivo
						, T.Cultivo
						, T.IdTipoRiego
						, T.TipoRiego
						, T.Eficiencia
						, T.FechaSiembra
						, T.IdEstacion
						, T.Alias
						, T.Estacion
						, COUNT(dbo.Parcela.IdParcelaInt) NParcelas
						, SUM(dbo.Parcela.SuperficieM2) SuperficieM2
						, T.Municipios
						,T.Parajes
		FROM 
		(
SELECT        UCCU.IdUnidadCultivo, UCCU.IdTemporada, UCCU.IdRegante, UPPER(COALESCE (dbo.Regante.Nombre, '')) AS Regante, UPPER(COALESCE (dbo.Regante.NIF, '')) AS NIF, UPPER(COALESCE (dbo.Regante.Telefono, '')) 
                         AS Telefono, UPPER(COALESCE (dbo.Regante.TelefonoSMS, '')) AS TelefonoSMS, UCCU.IdCultivo, UPPER(COALESCE (dbo.Cultivo.Nombre, '')) AS Cultivo, UCCU.IdTipoRiego, UPPER(COALESCE (dbo.RiegoTipo.TipoRiego, '')) 
                         AS TipoRiego, dbo.RiegoTipo.Eficiencia, UCCU.FechaSiembra, UC.IdEstacion, UPPER(COALESCE (UC.Alias, '')) AS Alias, UPPER(COALESCE (dbo.Estacion.Nombre, '')) AS Estacion, 
                         dbo.UnidadCultivoTemporadaMunicipios.Municipios, dbo.UnidadCultivoTemporadaParajes.Parajes
FROM            dbo.UnidadCultivo AS UC INNER JOIN
                         dbo.UnidadCultivoCultivo AS UCCU ON UC.IdUnidadCultivo = UCCU.IdUnidadCultivo INNER JOIN
                         dbo.Regante ON UC.IdRegante = dbo.Regante.IdRegante INNER JOIN
                         dbo.Cultivo ON UCCU.IdCultivo = dbo.Cultivo.IdCultivo INNER JOIN
                         dbo.RiegoTipo ON UCCU.IdTipoRiego = dbo.RiegoTipo.IdTipoRiego INNER JOIN
                         dbo.Estacion ON UC.IdEstacion = dbo.Estacion.IdEstacion AND UC.IdEstacion = dbo.Estacion.IdEstacion INNER JOIN
                         dbo.UnidadCultivoTemporadaMunicipios ON UCCU.IdUnidadCultivo = dbo.UnidadCultivoTemporadaMunicipios.IdUnidadCultivo INNER JOIN
                         dbo.UnidadCultivoTemporadaParajes ON UCCU.IdUnidadCultivo = dbo.UnidadCultivoTemporadaParajes.IdUnidadCultivo INNER JOIN
                         dbo.UnidadCultivoParcela ON UC.IdUnidadCultivo = dbo.UnidadCultivoParcela.IdUnidadCultivo INNER JOIN
                         dbo.Parcela ON dbo.UnidadCultivoParcela.IdParcelaInt = dbo.Parcela.IdParcelaInt
			WHERE		(@IdTemporada = '' OR UCCU.IdTemporada=@IdTemporada)
						AND (@IdUnidadCultivo = '' OR UCCU.IdUnidadCultivo=@IdUnidadCultivo)
						AND (@IdRegante = '' OR UCCU.IdRegante=@IdRegante)
						AND (@IdCultivo = '' OR UCCU.IdCultivo=@IdCultivo)
						AND (@IdTipoRiego = '' OR UCCU.IdTipoRiego=@IdTipoRiego)
						AND (@IdEstacion = '' OR UC.IdEstacion=@IdEstacion)
						AND (@IdMunicipio='' OR Parcela.IdMunicipio=@IdMunicipio)
		) T 
		LEFT JOIN dbo.UnidadCultivoParcela ON T.IdUnidadCultivo= dbo.UnidadCultivoParcela.IdUnidadCultivo AND T.IdTemporada= dbo.UnidadCultivoParcela.IdTemporada
		LEFT JOIN dbo.Parcela ON dbo.UnidadCultivoParcela.IdParcelaInt= dbo.Parcela.IdParcelaInt 
		GROUP BY		T.IdUnidadCultivo
						, T.IdTemporada
						, T.IdRegante
						, T.Regante
						, T.NIF
						, T.Telefono
						, T.TelefonoSMS
						, T.IdCultivo
						, T.Cultivo
						, T.IdTipoRiego
						, T.TipoRiego
						, T.Eficiencia
						, T.FechaSiembra
						, T.IdEstacion
						, T.Alias
						, T.Estacion
						, T.Municipios
						, T.Parajes
	) AS T
	WHERE	(@Search='' OR UPPER(CAST(CONCAT(Regante, ' ', NIF, ' ', Telefono, ' ', TelefonoSMS, ' ', Alias, ' ', TipoRiego, ' ', Cultivo, ' ', IdTemporada,' ',Municipios,' ', Parajes) AS VARCHAR(MAX))) COLLATE SQL_Latin1_General_Cp1251_CS_AS Like UPPER(Cast('%' + @Search + '%' as varchar(max))) collate SQL_Latin1_General_Cp1251_CS_AS )

)
GO
/****** Object:  View [dbo].[UnidadCultivoParaje]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UnidadCultivoParaje]
AS
SELECT DISTINCT dbo.UnidadCultivoParcela.IdTemporada, dbo.Municipio.Municipio, dbo.Paraje.Paraje, dbo.UnidadCultivoParcela.IdUnidadCultivo
FROM            dbo.Paraje INNER JOIN
                         dbo.Parcela ON dbo.Paraje.IdParaje = dbo.Parcela.IdParaje INNER JOIN
                         dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt = dbo.UnidadCultivoParcela.IdParcelaInt INNER JOIN
                         dbo.Municipio ON dbo.Paraje.IdMunucipio = dbo.Municipio.IdMunicipio AND dbo.Paraje.IdProvincia = dbo.Municipio.IdProvincia
GO
/****** Object:  Table [dbo].[ParcelaValvula]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParcelaValvula](
	[IdParcelaInt] [int] NOT NULL,
	[IdValvula] [int] NOT NULL,
 CONSTRAINT [PK_ParcelaValvula] PRIMARY KEY CLUSTERED 
(
	[IdParcelaInt] ASC,
	[IdValvula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UnidadDeCultivoValvula]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UnidadDeCultivoValvula]
AS
SELECT        dbo.ParcelaValvula.IdValvula, dbo.UnidadCultivoParcela.IdUnidadCultivo, dbo.UnidadCultivoParcela.IdTemporada, dbo.Parcela.IdParcelaInt
FROM            dbo.ParcelaValvula INNER JOIN
                         dbo.Parcela ON dbo.ParcelaValvula.IdParcelaInt = dbo.Parcela.IdParcelaInt INNER JOIN
                         dbo.UnidadCultivoParcela ON dbo.Parcela.IdParcelaInt = dbo.UnidadCultivoParcela.IdParcelaInt
GO
/****** Object:  View [dbo].[ParajeAmpliado]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ParajeAmpliado]
AS
SELECT        dbo.Paraje.IdParaje, dbo.Paraje.Paraje, dbo.Municipio.IdMunicipio, dbo.Municipio.Municipio, dbo.Provincia.IdProvincia, dbo.Provincia.Provincia
FROM            dbo.Paraje INNER JOIN
                         dbo.Municipio ON dbo.Paraje.IdMunucipio = dbo.Municipio.IdMunicipio AND dbo.Paraje.IdProvincia = dbo.Municipio.IdProvincia INNER JOIN
                         dbo.Provincia ON dbo.Municipio.IdProvincia = dbo.Provincia.IdProvincia
GO
/****** Object:  Table [dbo].[DatoClimatico]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatoClimatico](
	[Fecha] [datetime] NOT NULL,
	[IdEstacion] [int] NOT NULL,
	[TempMedia] [float] NOT NULL,
	[HumedadMedia] [float] NOT NULL,
	[VelViento] [float] NOT NULL,
	[Precipitacion] [float] NOT NULL,
	[Eto] [float] NOT NULL,
 CONSTRAINT [PK_DatosClimáticos] PRIMARY KEY CLUSTERED 
(
	[Fecha] ASC,
	[IdEstacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[DatoClimaticoMaxFecha]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DatoClimaticoMaxFecha]
AS
SELECT        IdEstacion, MAX(Fecha) AS MaxFecha
FROM            dbo.DatoClimatico
GROUP BY IdEstacion
GO
/****** Object:  Table [dbo].[Configuracion]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuracion](
	[Parametro] [nvarchar](255) NOT NULL,
	[Valor] [nvarchar](500) NULL,
 CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED 
(
	[Parametro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElementosGruesosTipo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElementosGruesosTipo](
	[IdElementosGruesos] [nvarchar](20) NOT NULL,
	[EleGruVal] [float] NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[IdElementosGruesos] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Geo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Geo](
	[GID] [int] IDENTITY(1,1) NOT NULL,
	[OBJECTID] [bigint] NULL,
	[MAPA] [bigint] NULL,
	[DELEGACIO] [int] NULL,
	[IdMunicipio] [int] NULL,
	[IdPoligono] [nvarchar](255) NULL,
	[HOJA] [nvarchar](255) NULL,
	[TIPO] [nvarchar](255) NULL,
	[Parcela] [nvarchar](50) NULL,
	[COORX] [float] NULL,
	[COORY] [float] NULL,
	[VIA] [bigint] NULL,
	[NUMERO] [int] NULL,
	[NUMERODUP] [nvarchar](255) NULL,
	[NUMSYMBOL] [int] NULL,
	[AREA] [float] NULL,
	[FECHAALTA] [bigint] NULL,
	[FECHABAJA] [bigint] NULL,
	[NINTERNO] [float] NULL,
	[PCAT1] [nvarchar](255) NULL,
	[PCAT2] [nvarchar](255) NULL,
	[EJERCICIO] [int] NULL,
	[NUM_EXP] [bigint] NULL,
	[CONTROL] [int] NULL,
	[REFCAT] [nvarchar](255) NULL,
	[Shape_Leng] [float] NULL,
	[Shape_Area] [float] NULL,
	[GEO] [geometry] NULL,
 CONSTRAINT [PK__UnidadCu__C51F0F3E2ADE247C] PRIMARY KEY CLUSTERED 
(
	[GID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MateriaOrganicaTipo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MateriaOrganicaTipo](
	[IdMateriaOrganica] [nvarchar](20) NOT NULL,
	[MatOrgVal] [float] NULL,
 CONSTRAINT [PK_MateriaOrganicaTipo] PRIMARY KEY CLUSTERED 
(
	[IdMateriaOrganica] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Riego]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Riego](
	[Fecha] [date] NOT NULL,
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[RiegoM3] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SueloTipo]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SueloTipo](
	[IdSueloTipo] [nvarchar](30) NOT NULL,
	[IdHorizonte] [int] NOT NULL,
	[Profundidad] [float] NOT NULL,
	[Limo] [float] NOT NULL,
	[Arcilla] [float] NOT NULL,
	[Arena] [float] NOT NULL,
	[ElementosGruesos] [float] NOT NULL,
	[MateriaOrganica] [float] NOT NULL,
 CONSTRAINT [PK_SueloTipo] PRIMARY KEY CLUSTERED 
(
	[IdSueloTipo] ASC,
	[IdHorizonte] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoEstres]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoEstres](
	[IdTipoEstres] [nvarchar](20) NOT NULL,
	[Estres] [nvarchar](50) NOT NULL,
	[IdUmbralInferiorRiego] [int] NULL,
	[IdUmbralSuperiorRiego] [int] NULL,
 CONSTRAINT [PK_TipoEstres] PRIMARY KEY CLUSTERED 
(
	[IdTipoEstres] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoEstresUmbral]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoEstresUmbral](
	[IdTipoEstres] [nvarchar](20) NOT NULL,
	[IdUmbral] [int] NOT NULL,
	[Descripcion] [nvarchar](50) NOT NULL,
	[Mensaje] [nchar](512) NULL,
	[Umbral] [float] NOT NULL,
	[Color] [int] NOT NULL,
 CONSTRAINT [PK_TipoEstresUmbral] PRIMARY KEY CLUSTERED 
(
	[IdTipoEstres] ASC,
	[IdUmbral] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoDatosExtra]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoDatosExtra](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Cobertura] [float] NULL,
	[Altura] [float] NULL,
	[LluviaMm] [float] NULL,
	[DRiEnd] [float] NULL,
	[RiegoM3] [float] NULL,
 CONSTRAINT [PK_ParcelaDatosExtras] PRIMARY KEY CLUSTERED 
(
	[IdUnidadCultivo] ASC,
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoSuperficie]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoSuperficie](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdTemporada] [nvarchar](10) NOT NULL,
	[SuperficieM2] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadCultivoTemporadaCosteAgua]    Script Date: 12/11/2019 11:51:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadCultivoTemporadaCosteAgua](
	[IdUnidadCultivo] [nvarchar](10) NOT NULL,
	[IdTemporada] [nvarchar](10) NOT NULL,
	[CosteM3Agua] [float] NOT NULL,
 CONSTRAINT [PK_UnidadCultivoTemporadaCosteAgua] PRIMARY KEY CLUSTERED 
(
	[IdUnidadCultivo] ASC,
	[IdTemporada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_Titulo]  DEFAULT ('') FOR [Titulo]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_Fecha]  DEFAULT ('') FOR [Fecha]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_De]  DEFAULT ('') FOR [De]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_Para]  DEFAULT ('') FOR [Para]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_Mensaje]  DEFAULT ('') FOR [Mensaje]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_IdTipoMensaje]  DEFAULT ((1)) FOR [IdAvisoTipo]
GO
ALTER TABLE [dbo].[Aviso] ADD  CONSTRAINT [DF_Aviso_Leido]  DEFAULT ((0)) FOR [Leido]
GO
ALTER TABLE [dbo].[AvisoTipo] ADD  CONSTRAINT [DF_AvisoTipo_AvisoTipo]  DEFAULT ('') FOR [AvisoTipo]
GO
ALTER TABLE [dbo].[AvisoTipo] ADD  CONSTRAINT [DF_AvisoTipo_Icono]  DEFAULT ('') FOR [Icono]
GO
ALTER TABLE [dbo].[CultivoEtapas] ADD  CONSTRAINT [DF_CultivoFases_LimSupPeligro]  DEFAULT ((-0.4)) FOR [LimSupPeligro]
GO
ALTER TABLE [dbo].[CultivoEtapas] ADD  CONSTRAINT [DF_CultivoFases_LimSupPerdidas]  DEFAULT ((0)) FOR [LimSupPerdidas]
GO
ALTER TABLE [dbo].[CultivoEtapas] ADD  CONSTRAINT [DF_CultivoFases_LimSupAlerta]  DEFAULT ((0.2)) FOR [LimSupAlerta]
GO
ALTER TABLE [dbo].[CultivoEtapas] ADD  CONSTRAINT [DF_CultivoFases_LimSupVigilar]  DEFAULT ((0.7)) FOR [LimSupVigilar]
GO
ALTER TABLE [dbo].[Regante] ADD  CONSTRAINT [DF_Regante_role]  DEFAULT (user_name()) FOR [Role]
GO
ALTER TABLE [dbo].[CultivoEtapas]  WITH CHECK ADD  CONSTRAINT [FK_CultivoFases_Cultivos] FOREIGN KEY([IdCultivo])
REFERENCES [dbo].[Cultivo] ([IdCultivo])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CultivoEtapas] CHECK CONSTRAINT [FK_CultivoFases_Cultivos]
GO
ALTER TABLE [dbo].[CultivoEtapas]  WITH CHECK ADD  CONSTRAINT [FK_CultivoFases_TipoEstres] FOREIGN KEY([IdTipoEstres])
REFERENCES [dbo].[TipoEstres] ([IdTipoEstres])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[CultivoEtapas] CHECK CONSTRAINT [FK_CultivoFases_TipoEstres]
GO
ALTER TABLE [dbo].[DatoClimatico]  WITH NOCHECK ADD  CONSTRAINT [FK_DatosClimaticos_Estaciones] FOREIGN KEY([IdEstacion])
REFERENCES [dbo].[Estacion] ([IdEstacion])
GO
ALTER TABLE [dbo].[DatoClimatico] CHECK CONSTRAINT [FK_DatosClimaticos_Estaciones]
GO
ALTER TABLE [dbo].[Municipio]  WITH CHECK ADD  CONSTRAINT [FK_Municipio_Provincia] FOREIGN KEY([IdProvincia])
REFERENCES [dbo].[Provincia] ([IdProvincia])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Municipio] CHECK CONSTRAINT [FK_Municipio_Provincia]
GO
ALTER TABLE [dbo].[Paraje]  WITH CHECK ADD  CONSTRAINT [FK_Paraje_Municipio] FOREIGN KEY([IdMunucipio], [IdProvincia])
REFERENCES [dbo].[Municipio] ([IdMunicipio], [IdProvincia])
GO
ALTER TABLE [dbo].[Paraje] CHECK CONSTRAINT [FK_Paraje_Municipio]
GO
ALTER TABLE [dbo].[Parcela]  WITH CHECK ADD  CONSTRAINT [FK_Parcela_Paraje] FOREIGN KEY([IdParaje])
REFERENCES [dbo].[Paraje] ([IdParaje])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Parcela] CHECK CONSTRAINT [FK_Parcela_Paraje]
GO
ALTER TABLE [dbo].[Parcela]  WITH CHECK ADD  CONSTRAINT [FK_Parcela_Regante] FOREIGN KEY([IdRegante])
REFERENCES [dbo].[Regante] ([IdRegante])
GO
ALTER TABLE [dbo].[Parcela] CHECK CONSTRAINT [FK_Parcela_Regante]
GO
ALTER TABLE [dbo].[ParcelaValvula]  WITH CHECK ADD  CONSTRAINT [FK_ParcelaHidranteToma_Parcelas] FOREIGN KEY([IdParcelaInt])
REFERENCES [dbo].[Parcela] ([IdParcelaInt])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParcelaValvula] CHECK CONSTRAINT [FK_ParcelaHidranteToma_Parcelas]
GO
ALTER TABLE [dbo].[TipoEstresUmbral]  WITH CHECK ADD  CONSTRAINT [FK_TipoEstresUmbral_TipoEstres] FOREIGN KEY([IdTipoEstres])
REFERENCES [dbo].[TipoEstres] ([IdTipoEstres])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TipoEstresUmbral] CHECK CONSTRAINT [FK_TipoEstresUmbral_TipoEstres]
GO
ALTER TABLE [dbo].[UnidadCultivo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivo_Estacion] FOREIGN KEY([IdEstacion])
REFERENCES [dbo].[Estacion] ([IdEstacion])
GO
ALTER TABLE [dbo].[UnidadCultivo] CHECK CONSTRAINT [FK_UnidadCultivo_Estacion]
GO
ALTER TABLE [dbo].[UnidadCultivo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivo_Estacion1] FOREIGN KEY([IdEstacion])
REFERENCES [dbo].[Estacion] ([IdEstacion])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivo] CHECK CONSTRAINT [FK_UnidadCultivo_Estacion1]
GO
ALTER TABLE [dbo].[UnidadCultivo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivo_Regante] FOREIGN KEY([IdRegante])
REFERENCES [dbo].[Regante] ([IdRegante])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivo] CHECK CONSTRAINT [FK_UnidadCultivo_Regante]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo]  WITH CHECK ADD  CONSTRAINT [FK_ParcelasCultivo_Cultivos] FOREIGN KEY([IdCultivo])
REFERENCES [dbo].[Cultivo] ([IdCultivo])
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo] CHECK CONSTRAINT [FK_ParcelasCultivo_Cultivos]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo]  WITH CHECK ADD  CONSTRAINT [FK_ParcelasCultivo_Cultivos1] FOREIGN KEY([IdCultivo])
REFERENCES [dbo].[Cultivo] ([IdCultivo])
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo] CHECK CONSTRAINT [FK_ParcelasCultivo_Cultivos1]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo]  WITH CHECK ADD  CONSTRAINT [FK_ParcelasCultivo_Temporada] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo] CHECK CONSTRAINT [FK_ParcelasCultivo_Temporada]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo]  WITH CHECK ADD  CONSTRAINT [FK_ParcelasCultivo_TiposRiego] FOREIGN KEY([IdTipoRiego])
REFERENCES [dbo].[RiegoTipo] ([IdTipoRiego])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo] CHECK CONSTRAINT [FK_ParcelasCultivo_TiposRiego]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo]  WITH CHECK ADD  CONSTRAINT [FK_ParcelasCultivo_UnidadCultivo] FOREIGN KEY([IdUnidadCultivo])
REFERENCES [dbo].[UnidadCultivo] ([IdUnidadCultivo])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo] CHECK CONSTRAINT [FK_ParcelasCultivo_UnidadCultivo]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoCultivo_Regante] FOREIGN KEY([IdRegante])
REFERENCES [dbo].[Regante] ([IdRegante])
GO
ALTER TABLE [dbo].[UnidadCultivoCultivo] CHECK CONSTRAINT [FK_UnidadCultivoCultivo_Regante]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivoEtapas]  WITH NOCHECK ADD  CONSTRAINT [FK_ParcelasCultivoFases_ParcelasCultivo] FOREIGN KEY([IdUnidadCultivo], [IdTemporada])
REFERENCES [dbo].[UnidadCultivoCultivo] ([IdUnidadCultivo], [IdTemporada])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoCultivoEtapas] CHECK CONSTRAINT [FK_ParcelasCultivoFases_ParcelasCultivo]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivoEtapas]  WITH NOCHECK ADD  CONSTRAINT [FK_UnidadCultivoCultivoFases_Temporada] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoCultivoEtapas] CHECK CONSTRAINT [FK_UnidadCultivoCultivoFases_Temporada]
GO
ALTER TABLE [dbo].[UnidadCultivoCultivoEtapas]  WITH NOCHECK ADD  CONSTRAINT [FK_UnidadCultivoCultivoFases_TipoEstres] FOREIGN KEY([IdTipoEstres])
REFERENCES [dbo].[TipoEstres] ([IdTipoEstres])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[UnidadCultivoCultivoEtapas] CHECK CONSTRAINT [FK_UnidadCultivoCultivoFases_TipoEstres]
GO
ALTER TABLE [dbo].[UnidadCultivoDatosExtra]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoDatosExtra_UnidadCultivo] FOREIGN KEY([IdUnidadCultivo])
REFERENCES [dbo].[UnidadCultivo] ([IdUnidadCultivo])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoDatosExtra] CHECK CONSTRAINT [FK_UnidadCultivoDatosExtra_UnidadCultivo]
GO
ALTER TABLE [dbo].[UnidadCultivoParcela]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoParcela_Parcelas] FOREIGN KEY([IdParcelaInt])
REFERENCES [dbo].[Parcela] ([IdParcelaInt])
GO
ALTER TABLE [dbo].[UnidadCultivoParcela] CHECK CONSTRAINT [FK_UnidadCultivoParcela_Parcelas]
GO
ALTER TABLE [dbo].[UnidadCultivoParcela]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoParcela_Temporada] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
GO
ALTER TABLE [dbo].[UnidadCultivoParcela] CHECK CONSTRAINT [FK_UnidadCultivoParcela_Temporada]
GO
ALTER TABLE [dbo].[UnidadCultivoParcela]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoParcela_Temporada1] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoParcela] CHECK CONSTRAINT [FK_UnidadCultivoParcela_Temporada1]
GO
ALTER TABLE [dbo].[UnidadCultivoParcela]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoParcela_UnidadCultivo] FOREIGN KEY([IdUnidadCultivo])
REFERENCES [dbo].[UnidadCultivo] ([IdUnidadCultivo])
GO
ALTER TABLE [dbo].[UnidadCultivoParcela] CHECK CONSTRAINT [FK_UnidadCultivoParcela_UnidadCultivo]
GO
ALTER TABLE [dbo].[UnidadCultivoSuelo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoSuelo_Temporada] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
GO
ALTER TABLE [dbo].[UnidadCultivoSuelo] CHECK CONSTRAINT [FK_UnidadCultivoSuelo_Temporada]
GO
ALTER TABLE [dbo].[UnidadCultivoSuelo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoSuelo_UnidadCultivo] FOREIGN KEY([IdUnidadCultivo])
REFERENCES [dbo].[UnidadCultivo] ([IdUnidadCultivo])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoSuelo] CHECK CONSTRAINT [FK_UnidadCultivoSuelo_UnidadCultivo]
GO
ALTER TABLE [dbo].[UnidadCultivoSuperficie]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoSuperficie_Temporada] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoSuperficie] CHECK CONSTRAINT [FK_UnidadCultivoSuperficie_Temporada]
GO
ALTER TABLE [dbo].[UnidadCultivoSuperficie]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoSuperficie_UnidadCultivo] FOREIGN KEY([IdUnidadCultivo])
REFERENCES [dbo].[UnidadCultivo] ([IdUnidadCultivo])
GO
ALTER TABLE [dbo].[UnidadCultivoSuperficie] CHECK CONSTRAINT [FK_UnidadCultivoSuperficie_UnidadCultivo]
GO
ALTER TABLE [dbo].[UnidadCultivoTemporadaCosteAgua]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoTemporadaCosteAgua_Temporada] FOREIGN KEY([IdTemporada])
REFERENCES [dbo].[Temporada] ([IdTemporada])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoTemporadaCosteAgua] CHECK CONSTRAINT [FK_UnidadCultivoTemporadaCosteAgua_Temporada]
GO
ALTER TABLE [dbo].[UnidadCultivoTemporadaCosteAgua]  WITH CHECK ADD  CONSTRAINT [FK_UnidadCultivoTemporadaCosteAgua_UnidadCultivo] FOREIGN KEY([IdUnidadCultivo])
REFERENCES [dbo].[UnidadCultivo] ([IdUnidadCultivo])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UnidadCultivoTemporadaCosteAgua] CHECK CONSTRAINT [FK_UnidadCultivoTemporadaCosteAgua_UnidadCultivo]
GO
ALTER TABLE [dbo].[Geo]  WITH CHECK ADD  CONSTRAINT [enforce_srid_geometry_prueba3] CHECK  (([GEO].[STSrid]=(4326)))
GO
ALTER TABLE [dbo].[Geo] CHECK CONSTRAINT [enforce_srid_geometry_prueba3]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Indica la temporada en curso (sólo se marca una de la tabla)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Temporada', @level2type=N'COLUMN',@level2name=N'Activa'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[47] 4[15] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[42] 4[23] 3) )"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 1
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "UnidadCultivo"
            Begin Extent = 
               Top = 11
               Left = 37
               Bottom = 340
               Right = 241
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoCultivo"
            Begin Extent = 
               Top = 5
               Left = 332
               Bottom = 305
               Right = 550
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoCultivoEtapas"
            Begin Extent = 
               Top = 11
               Left = 1115
               Bottom = 251
               Right = 1373
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "Cultivo"
            Begin Extent = 
               Top = 319
               Left = 897
               Bottom = 571
               Right = 1088
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Temporada"
            Begin Extent = 
               Top = 362
               Left = 607
               Bottom = 515
               Right = 838
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "Regante"
            Begin Extent = 
               Top = 396
               Left = 134
               Bottom = 526
               Right = 304
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RiegoTipo"
            Begin Extent = 
               Top = 68
               Left = 874
               Bottom = 304
               Right = 1062
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BalanceData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoSuelo"
            Begin Extent = 
               Top = 345
               Left = 368
               Bottom = 569
               Right = 575
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CultivoEtapas"
            Begin Extent = 
               Top = 341
               Left = 1197
               Bottom = 590
               Right = 1389
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
      PaneHidden = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 60
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3750
         Alias = 2805
         Table = 2790
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BalanceData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BalanceData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "DatoClimatico"
            Begin Extent = 
               Top = 4
               Left = 196
               Bottom = 231
               Right = 497
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1905
         Width = 2325
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 1470
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DatoClimaticoMaxFecha'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DatoClimaticoMaxFecha'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[37] 4[24] 2[14] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "UnidadCultivoCultivo"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 291
               Right = 463
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoParcela"
            Begin Extent = 
               Top = 11
               Left = 603
               Bottom = 282
               Right = 790
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Parcela"
            Begin Extent = 
               Top = 10
               Left = 911
               Bottom = 267
               Right = 1236
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 2325
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2370
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 2175
         Or = 2550
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FiltroParcelasDatosHidricos'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FiltroParcelasDatosHidricos'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "UnidadCultivoParcela"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 212
               Right = 306
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Temporada"
            Begin Extent = 
               Top = 12
               Left = 473
               Bottom = 259
               Right = 942
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 2460
         Alias = 1485
         Table = 2100
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 2715
         Filter = 2880
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MaxAñoUnidadCultivo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MaxAñoUnidadCultivo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Paraje"
            Begin Extent = 
               Top = 12
               Left = 36
               Bottom = 269
               Right = 206
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Municipio"
            Begin Extent = 
               Top = 9
               Left = 281
               Bottom = 227
               Right = 556
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Provincia"
            Begin Extent = 
               Top = 105
               Left = 746
               Bottom = 201
               Right = 916
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ParajeAmpliado'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ParajeAmpliado'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[48] 4[25] 2[3] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = -28
      End
      Begin Tables = 
         Begin Table = "Parcela"
            Begin Extent = 
               Top = 26
               Left = 46
               Bottom = 192
               Right = 216
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Provincia"
            Begin Extent = 
               Top = 7
               Left = 1490
               Bottom = 103
               Right = 1660
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Paraje"
            Begin Extent = 
               Top = 16
               Left = 968
               Bottom = 224
               Right = 1138
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoParcela"
            Begin Extent = 
               Top = 246
               Left = 314
               Bottom = 376
               Right = 545
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivo"
            Begin Extent = 
               Top = 240
               Left = 717
               Bottom = 421
               Right = 921
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Estacion"
            Begin Extent = 
               Top = 246
               Left = 1000
               Bottom = 376
               Right = 1170
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoCultivo"
            Begin Extent = 
               Top = 27
               Left = 331
               Bottom = 221
               Right = 566
       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoDatosAmpliados'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RiegoTipo"
            Begin Extent = 
               Top = 28
               Left = 693
               Bottom = 207
               Right = 902
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Temporada"
            Begin Extent = 
               Top = 122
               Left = 1199
               Bottom = 326
               Right = 1439
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Regante"
            Begin Extent = 
               Top = 221
               Left = 12
               Bottom = 378
               Right = 182
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Municipio"
            Begin Extent = 
               Top = 217
               Left = 1686
               Bottom = 330
               Right = 1856
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 20
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1935
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1635
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4320
         Alias = 2970
         Table = 2475
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoDatosAmpliados'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoDatosAmpliados'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Paraje"
            Begin Extent = 
               Top = 125
               Left = 339
               Bottom = 270
               Right = 525
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Parcela"
            Begin Extent = 
               Top = 71
               Left = 600
               Bottom = 201
               Right = 770
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoParcela"
            Begin Extent = 
               Top = 25
               Left = 918
               Bottom = 230
               Right = 1144
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Municipio"
            Begin Extent = 
               Top = 84
               Left = 52
               Bottom = 299
               Right = 222
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 2055
         Width = 2055
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 13' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoParaje'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'50
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoParaje'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoParaje'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[32] 4[17] 2[22] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TEMP"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 119
               Right = 229
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1935
         Width = 2115
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 6705
         Alias = 2565
         Table = 2760
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoTemporadaMunicipios'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoTemporadaMunicipios'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TEMP"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 168
               Right = 344
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 5385
         Alias = 2310
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoTemporadaParajes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadCultivoTemporadaParajes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ParcelaValvula"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 143
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Parcela"
            Begin Extent = 
               Top = 30
               Left = 350
               Bottom = 318
               Right = 535
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UnidadCultivoParcela"
            Begin Extent = 
               Top = 54
               Left = 622
               Bottom = 310
               Right = 917
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2445
         Alias = 900
         Table = 2640
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadDeCultivoValvula'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UnidadDeCultivoValvula'
GO
USE [master]
GO
ALTER DATABASE [OPTIAQUA] SET  READ_WRITE 
GO
