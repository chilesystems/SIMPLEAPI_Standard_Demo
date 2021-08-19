using SimpleAPI.Enum;
using SimpleAPI.Models.ReciboMercaderia;
using SimpleAPI.SII;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestUnitarios
{
    public class Compras
    {
        Handler handler = new Handler();
        private string pathCertificado = System.IO.Path.Combine("Files", "CertificadoGonzalo2021.pfx");
        private string pathToken = System.IO.Path.Combine("Files", "tkn.dat");

        [Fact]
        public void GenerarRespuestaEnvio()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");

            List<SimpleAPI.Models.RespuestaEnvio.RecepcionDTE> dtes = new List<SimpleAPI.Models.RespuestaEnvio.RecepcionDTE>();
            dtes.Add(new SimpleAPI.Models.RespuestaEnvio.RecepcionDTE() 
            {
                FechaEmision = DateTime.Now,
                Folio = 500,
                MontoTotal = 56002,
                TipoDTE = TipoDTE.DTEType.FacturaElectronica,
                RutEmisor = "76269769-6",
                RutReceptor = "17096073-4"
            });
            dtes.Add(new SimpleAPI.Models.RespuestaEnvio.RecepcionDTE()
            {
                FechaEmision = DateTime.Now,
                Folio = 8540,
                MontoTotal = 845000,
                TipoDTE = TipoDTE.DTEType.FacturaElectronica,
                RutEmisor = "76269769-6",
                RutReceptor = "17096073-4"
            });

            var respuestaEnvio = handler.GenerarRespuestaEnvio(dtes);
            var filepath = respuestaEnvio.Firmar(pathCertificado, string.Empty, "Pollito702");
            Assert.True(System.IO.File.Exists(filepath));
            System.IO.File.Delete(filepath);
        }

        [Fact]
        public void GenerarRespuestaDTE()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");

            List<SimpleAPI.Models.RespuestaEnvio.ResultadoDTE> dtes = new List<SimpleAPI.Models.RespuestaEnvio.ResultadoDTE>();
            dtes.Add(new SimpleAPI.Models.RespuestaEnvio.ResultadoDTE() 
            {
                CodigoEnvio = 1,
                CodigoRechazoODiscrepancia = -1,
                EstadoDTE = EstadoResultadoDTE.EstadoResultadoDTEEnum.Ok,
                GlosaEstadoDTE = "ACEPTADO OK",
                FechaEmision = DateTime.Now,
                Folio = 787,
                MontoTotal = 9000,
                RutEmisor = "76269769-6",
                RutReceptor = "66666666-6",
                TipoDTE = TipoDTE.DTEType.FacturaElectronica
            });

            dtes.Add(new SimpleAPI.Models.RespuestaEnvio.ResultadoDTE()
            {
                CodigoEnvio = 1,
                CodigoRechazoODiscrepancia = -1,
                EstadoDTE = EstadoResultadoDTE.EstadoResultadoDTEEnum.Rechazo,
                GlosaEstadoDTE = "RECHAZADO",
                FechaEmision = DateTime.Now,
                Folio = 46885,
                MontoTotal = 700470,
                RutEmisor = "76269769-6",
                RutReceptor = "66666666-6",
                TipoDTE = TipoDTE.DTEType.FacturaElectronica
            });

            var respuestaEnvio = handler.GenerarRespuestaDTE(dtes);
            var filepath = respuestaEnvio.Firmar(pathCertificado, string.Empty, "Pollito702");
            Assert.True(System.IO.File.Exists(filepath));
            System.IO.File.Delete(filepath);
        }

        [Fact]
        public void GenerarAcuseRecibo()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");

            var recibo = new Recibo()
            {
                DocumentoRecibo = new DocumentoRecibo()
                {
                    Id = "R01",
                    RutEmisor = "88888888-8",
                    RutReceptor = "76269769-6",
                    FechaEmision = DateTime.Now,
                    Folio = 787,
                    MontoTotal = 185000,
                    TipoDocumento = TipoDTE.DTEType.FacturaElectronica,
                    Recinto = "Recinto",
                    RutFirma = "17096073-4"
                }
            };

            recibo.Firmar(pathCertificado, "Pollito702");
            var respuestaEnvio = handler.AcuseReciboMercaderias(recibo);
            var filepath = respuestaEnvio.Firmar(pathCertificado, string.Empty, "Pollito702");
            Assert.True(System.IO.File.Exists(filepath));
            System.IO.File.Delete(filepath);
            System.IO.File.Delete(recibo.filePath);
        }
    }
}
