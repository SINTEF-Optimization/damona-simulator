namespace Damona.Simulator.Domain;

// [Flags]
// public enum SurgeryType
// {
//     OrtopediHofte = 1 << 0,
//     OrtopediKne = 1 << 1,
//     OrtopediPlattfot = 1 << 2,
//     UrologiBlerekreft = 1 << 3,
//     UrologiBlereSteinKnusing = 1 << 4,
//     GastrologiAppendicitt = 1 << 5,
//     GastrologiNavleblokk = 1 << 6,
//     GastrologiEndetarmsKreft = 1 << 7,
//     EntMyringoplastikk = 1 << 8,
//     EntDrenIOret = 1 << 9,
//     EntFess = 1 << 10,
//     EyeGraaStaer = 1 << 11,
//     EyeGodartetSvulst = 1 << 12,
//     OdontologiDentikaries = 1 << 13,
//     OdontologiVisdomstenner = 1 << 14,
//     MammoBrystbevarendeOperasjon = 1 << 15,
//     MammoFjerningAvBryst = 1 << 16,
//     KarkirurgiUtposingHovedpulsaare = 1 << 17,
//     KarkirurgiTea = 1 << 18,
//     KjevekirurgistGeneric = 1 << 19,
//     GynekologiskGeneric = 1 << 20,
//     GenerellKirurgiGeneric = 1 << 21,
//     Unknown = 1 << 22,
//     None = 0
// }
//
// public static class SurgeryTypeExtensions
// {
//     public static Specialty GetSpecialty(this SurgeryType surgeryType)
//     {
//         return surgeryType switch
//         {
//             SurgeryType.OrtopediHofte => Specialty.Ortopedi,
//             SurgeryType.OrtopediKne => Specialty.Ortopedi,
//             SurgeryType.OrtopediPlattfot => Specialty.Ortopedi,
//             SurgeryType.UrologiBlerekreft => Specialty.Urologi,
//             SurgeryType.UrologiBlereSteinKnusing => Specialty.Urologi,
//             SurgeryType.GastrologiAppendicitt => Specialty.Gastro,
//             SurgeryType.GastrologiNavleblokk => Specialty.Gastro,
//             SurgeryType.GastrologiEndetarmsKreft => Specialty.Gastro,
//             SurgeryType.EntMyringoplastikk => Specialty.EarNoseThroat,
//             SurgeryType.EntDrenIOret => Specialty.EarNoseThroat,
//             SurgeryType.EntFess => Specialty.EarNoseThroat,
//             SurgeryType.EyeGraaStaer => Specialty.Eye,
//             SurgeryType.EyeGodartetSvulst => Specialty.Eye,
//             SurgeryType.OdontologiDentikaries => Specialty.Odontologi,
//             SurgeryType.OdontologiVisdomstenner => Specialty.Odontologi,
//             SurgeryType.MammoBrystbevarendeOperasjon => Specialty.Mammo,
//             SurgeryType.MammoFjerningAvBryst => Specialty.Mammo,
//             SurgeryType.KarkirurgiUtposingHovedpulsaare => Specialty.Karkirurgi,
//             SurgeryType.KarkirurgiTea => Specialty.Karkirurgi,
//             SurgeryType.KjevekirurgistGeneric => Specialty.Kjevekirurgi,
//             SurgeryType.GynekologiskGeneric => Specialty.Gynekologi,
//             SurgeryType.GenerellKirurgiGeneric => Specialty.GeneralSurgery,
//             _ => Specialty.Emergency,
//         };
//     }
//
//     public static AnesthesiaType GetAnesthesia(this SurgeryType surgeryType)
//     {
//         return surgeryType switch
//         {
//             SurgeryType.OrtopediHofte => AnesthesiaType.Spinal,
//             SurgeryType.OrtopediKne => AnesthesiaType.Narcosis,
//             SurgeryType.OrtopediPlattfot => AnesthesiaType.Spinal,
//             SurgeryType.UrologiBlerekreft => AnesthesiaType.NarcosisAndSpinal,
//             SurgeryType.UrologiBlereSteinKnusing => AnesthesiaType.NarcosisAndSpinal,
//             SurgeryType.GastrologiAppendicitt => AnesthesiaType.Narcosis,
//             SurgeryType.GastrologiNavleblokk => AnesthesiaType.Narcosis,
//             SurgeryType.GastrologiEndetarmsKreft => AnesthesiaType.Narcosis,
//             SurgeryType.EntMyringoplastikk => AnesthesiaType.Narcosis,
//             SurgeryType.EntDrenIOret => AnesthesiaType.Narcosis,
//             SurgeryType.EntFess => AnesthesiaType.Narcosis,
//             SurgeryType.EyeGraaStaer => AnesthesiaType.Local,
//             SurgeryType.EyeGodartetSvulst => AnesthesiaType.Local,
//             SurgeryType.OdontologiDentikaries => AnesthesiaType.Narcosis,
//             SurgeryType.OdontologiVisdomstenner => AnesthesiaType.Local,
//             SurgeryType.MammoBrystbevarendeOperasjon => AnesthesiaType.Narcosis,
//             SurgeryType.MammoFjerningAvBryst => AnesthesiaType.Narcosis,
//             SurgeryType.KarkirurgiUtposingHovedpulsaare => AnesthesiaType.Narcosis,
//             SurgeryType.KarkirurgiTea => AnesthesiaType.Narcosis,
//             SurgeryType.KjevekirurgistGeneric => AnesthesiaType.None,
//             SurgeryType.GynekologiskGeneric => AnesthesiaType.None,
//             SurgeryType.GenerellKirurgiGeneric => AnesthesiaType.None,
//             _ => AnesthesiaType.None
//         };
//     }
//
//     public static TimeSpan GetDuration(this SurgeryType surgeryType)
//     {
//         var knifeTime = surgeryType switch
//         {
//             SurgeryType.OrtopediHofte => TimeSpan.FromMinutes(120),
//             SurgeryType.OrtopediKne => TimeSpan.FromMinutes(60),
//             SurgeryType.OrtopediPlattfot => TimeSpan.FromMinutes(250),
//             SurgeryType.UrologiBlerekreft => TimeSpan.FromMinutes(70),
//             SurgeryType.UrologiBlereSteinKnusing => TimeSpan.FromMinutes(40),
//             SurgeryType.GastrologiAppendicitt => TimeSpan.FromMinutes(70),
//             SurgeryType.GastrologiNavleblokk => TimeSpan.FromMinutes(60),
//             SurgeryType.GastrologiEndetarmsKreft => TimeSpan.FromMinutes(150),
//             SurgeryType.EntMyringoplastikk => TimeSpan.FromMinutes(150),
//             SurgeryType.EntDrenIOret => TimeSpan.FromMinutes(25),
//             SurgeryType.EntFess => TimeSpan.FromMinutes(90),
//             SurgeryType.EyeGraaStaer => TimeSpan.FromMinutes(25),
//             SurgeryType.EyeGodartetSvulst => TimeSpan.FromMinutes(50),
//             SurgeryType.OdontologiDentikaries => TimeSpan.FromMinutes(120),
//             SurgeryType.OdontologiVisdomstenner => TimeSpan.FromMinutes(30),
//             SurgeryType.MammoBrystbevarendeOperasjon => TimeSpan.FromMinutes(100),
//             SurgeryType.MammoFjerningAvBryst => TimeSpan.FromMinutes(120),
//             SurgeryType.KarkirurgiUtposingHovedpulsaare => TimeSpan.FromMinutes(150),
//             SurgeryType.KarkirurgiTea => TimeSpan.FromMinutes(180),
//             SurgeryType.KjevekirurgistGeneric => TimeSpan.FromMinutes(120),
//             SurgeryType.GynekologiskGeneric => TimeSpan.FromMinutes(110),
//             SurgeryType.GenerellKirurgiGeneric => TimeSpan.FromMinutes(130),
//             _ => TimeSpan.FromMinutes(60)
//         };
//
//         return knifeTime + surgeryType.GetAnesthesia().GetDuration();
//     }
// }