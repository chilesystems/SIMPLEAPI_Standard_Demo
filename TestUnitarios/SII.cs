using SimpleAPI.Enum;
using System;
using SimpleAPI.SII;
using Xunit;
using Xunit.Abstractions;

namespace TestUnitarios
{
    public class SII
    {
        private readonly ITestOutputHelper _testOutputHelper;
        Handler handler = new Handler();
        private static string root 
            = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory());
        private string pathCertificado = System.IO.Path.Combine(root, "Files","CertificadoDigital.pfx");
        private string pathEnvioDTE = System.IO.Path.Combine(root, "Files","ENVIO_DTE_REST.xml");
        private string pathToken = System.IO.Path.Combine(root, "Files", "tkn.dat");

        public SII(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async System.Threading.Tasks.Task ObtenerTokenAsync()
        {
            _testOutputHelper.WriteLine(pathCertificado);
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            string token = await SimpleAPI.WS.Autorizacion.Autenticar.GetTokenAsync(pathCertificado, Ambiente.AmbienteEnum.Produccion, pathToken, "Pollito702");
            System.IO.File.Delete(pathToken);

            Assert.True(!string.IsNullOrEmpty(token));
        }

        [Fact]
        public async System.Threading.Tasks.Task ObtenerEstadoDTEAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new SimpleAPI.SII.GetEstadoEntity("17096073-4", "76269769-6", "3671414-K", new DateTime(2021, 5, 4), 33, 85, 75225);
            var estadoDTE = await SimpleAPI.WS.Estado.EstadoDTE.GetEstadoDTEAsync(entity, pathCertificado, ambiente, pathToken, "Pollito702");
            Assert.True(estadoDTE.Ok);
        }

        [Fact]
        public async System.Threading.Tasks.Task ObtenerEstadoEnvioAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new SimpleAPI.SII.GetEstadoEnvioEntity("76269769-6", "4942604664");
            var estadoDTE = await SimpleAPI.WS.Estado.EstadoEnvio.GetEstadoEnvioAsync(entity, ambiente, pathToken, pathCertificado, "Pollito702");

            Assert.True(estadoDTE.Ok);
        }

        [Fact]
        public async System.Threading.Tasks.Task EnviarAsync()
        {
            _testOutputHelper.WriteLine(pathCertificado);
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Certificacion;
            var envioDTE = await SimpleAPI.WS.Envio.EnvioDTE.EnviarAsync("17096073-4", "76269769-6", pathEnvioDTE, pathCertificado, ambiente, pathToken, "4150-8390-6401-8301-6912","Pollito702");

            Assert.True(envioDTE.Ok);
        }
        
        [Fact]
        public async System.Threading.Tasks.Task NotificarAceptacionReclamoAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Certificacion;
            var rutEmpresa = "55555555-5";
            var rutCertificado = "17096073-4";
            var entity = new AceptacionReclamoEntity(rutEmpresa, 39, 12345);
            entity.Accion = "RCD";
            var aceptacionReclamoResult = await SimpleAPI.WS.AceptacionReclamo.AceptacionReclamo.NotificarAceptacionReclamoAsync(entity, pathCertificado, ambiente, pathToken, "Pollito702");
            _testOutputHelper.WriteLine(aceptacionReclamoResult.ToString());
        
            Assert.True(aceptacionReclamoResult.CodRespuesta > -1000);
        }
    }
}
