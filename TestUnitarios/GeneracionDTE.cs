using SimpleAPI.Enum;
using System;
using Xunit;

namespace TestUnitarios
{
    public class GeneracionDTE
    {
        Handler handler = new Handler();
        private string pathCAF_Boletas = System.IO.Path.Combine("Files", "CAF_boletas_1_50.xml");
        private string pathCertificado = System.IO.Path.Combine("Files", "CertificadoGonzalo2021.pfx");

        [Fact]
        public void TimbreCorrecto()
        {
            var dte = handler.GenerateDTE(TipoDTE.DTEType.BoletaElectronica, 1);
            handler.GenerateDetails(dte);
            var resultado = dte.Documento.Timbrar(pathCAF_Boletas, out string messageOut);
            Assert.True(resultado);
        }

        [Fact]
        public void TimbreFueradeRango()
        {
            var dte = handler.GenerateDTE(TipoDTE.DTEType.BoletaElectronica, 51);
            handler.GenerateDetails(dte);
            var resultado = dte.Documento.Timbrar(pathCAF_Boletas, out string messageOut);
            Assert.False(resultado);
        }

        [Fact]
        public async void FirmaCorrecta()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var dte = handler.GenerateDTE(TipoDTE.DTEType.BoletaElectronica, 1);
            handler.GenerateDetails(dte);
            dte.Documento.Timbrar(pathCAF_Boletas, out string messageOut);
            var resultado = await dte.Firmar(pathCertificado, "1523-Z930-6403-4900-6397", "", $"DTE_{DateTime.Now.Ticks.ToString()}", "Pollito702");
            System.IO.File.Delete(resultado.Item1);
            Assert.Equal(string.Empty, resultado.Item2);
        }

        [Fact]
        public async void FirmaPasswordIncorrecta()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var dte = handler.GenerateDTE(TipoDTE.DTEType.BoletaElectronica, 1);
            handler.GenerateDetails(dte);
            dte.Documento.Timbrar(pathCAF_Boletas, out string messageOut);
            var resultado = await dte.Firmar(pathCertificado, "1523-Z930-6403-4900-6397", "", $"DTE_{DateTime.Now.Ticks.ToString()}", "PasswordCualquiera");
            System.IO.File.Delete(resultado.Item1);
            Assert.Equal(string.Empty, resultado.Item2);
        }
    }
}
