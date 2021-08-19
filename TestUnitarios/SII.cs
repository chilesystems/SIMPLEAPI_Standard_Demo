using SimpleAPI.Enum;
using SimpleAPI.SII;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestUnitarios
{
    public class SII
    {
        Handler handler = new Handler();
        private string pathCertificado = System.IO.Path.Combine("Files", "CertificadoGonzalo2021.pfx");
        private string pathEnvioDTE = System.IO.Path.Combine("Files", "ENVIO_DTE_REST.xml");
        private string pathToken = System.IO.Path.Combine("Files", "tkn.dat");

        [Fact]
        public async Task ObtenerTokenAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            string token = await SimpleAPI.WS.Autorizacion.Autenticar.GetTokenAsync(pathCertificado, Ambiente.AmbienteEnum.Produccion, pathToken, "Pollito702");
            System.IO.File.Delete(pathToken);

            Assert.True(!string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task ObtenerEstadoDTEAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new GetEstadoEntity("17096073-4", "76269769-6", "3671414-K", new DateTime(2021, 5, 4), 33, 85, 75225);
            var estadoDTE = await SimpleAPI.WS.Estado.EstadoDTE.GetEstadoDTEAsync(entity, pathCertificado, ambiente, pathToken, "Pollito702");
            Assert.True(estadoDTE.Ok);
        }

        [Fact]
        public async Task ObtenerEstadoEnvioAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new GetEstadoEnvioEntity("76269769-6", "4942604664");
            var estadoDTE = await SimpleAPI.WS.Estado.EstadoEnvio.GetEstadoEnvioAsync(entity, ambiente, pathToken, pathCertificado, "Pollito702");

            Assert.True(estadoDTE.Ok);
        }

        [Fact]
        public async Task EnviarAsync()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Certificacion;
            var envioDTE = await SimpleAPI.WS.Envio.EnvioDTE.EnviarAsync("17096073-4", "76269769-6", pathEnvioDTE, pathCertificado, ambiente, pathToken, "Pollito702");

            Assert.True(envioDTE.Ok);
        }

        [Fact]
        public async Task EnviarAceptacionSII()
        {
            if (!System.IO.File.Exists(pathCertificado)) throw new Exception("No existe certificado digital");
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new AceptacionReclamoEntity("76203747-5", 33, 9420, TipoAceptacion.ERM);
            var aceptacion = await SimpleAPI.WS.AceptacionReclamo.AceptacionReclamo.NotificarAceptacionReclamoAsync(entity, pathCertificado, ambiente, pathToken, "Pollito702");

            Assert.True(aceptacion.CodRespuesta != 4);
        }

       
    }
}
