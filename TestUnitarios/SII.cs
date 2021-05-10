using SimpleAPI.Enum;
using System;
using Xunit;

namespace TestUnitarios
{
    public class SII
    {
        Handler handler = new Handler();
        private string pathCertificadoValido = System.IO.Path.Combine("Files","CertificadoGonzalo2021.pfx");
        private string pathCertificadoVencido = System.IO.Path.Combine("Files","CertificadoTest.pfx");
        private string pathEnvioDTE = System.IO.Path.Combine("Files","ENVIO_DTE_REST.xml");
        private string pathToken = System.IO.Path.Combine("Files", "tkn.dat");

        [Fact]
        public async System.Threading.Tasks.Task ObtenerTokenAsync()
        {
            string token = await SimpleAPI.WS.Autorizacion.Autenticar.GetTokenAsync(pathCertificadoValido, Ambiente.AmbienteEnum.Produccion, pathToken, "Pollito702");
            System.IO.File.Delete(pathToken);

            Assert.True(!string.IsNullOrEmpty(token));
        }

        [Fact]
        public async System.Threading.Tasks.Task ObtenerEstadoDTEAsync()
        {
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new SimpleAPI.SII.GetEstadoEntity("17096073-4", "76269769-6", "3671414-K", new DateTime(2021, 5, 4), 33, 85, 75225);
            var estadoDTE = await SimpleAPI.WS.Estado.EstadoDTE.GetEstadoDTEAsync(entity, pathCertificadoValido, ambiente, pathToken, "Pollito702");
            Assert.True(estadoDTE.Ok);
        }

        [Fact]
        public async System.Threading.Tasks.Task ObtenerEstadoEnvioAsync()
        {
            var ambiente = Ambiente.AmbienteEnum.Produccion;
            var entity = new SimpleAPI.SII.GetEstadoEnvioEntity("76269769-6", "4942604664");
            var estadoDTE = await SimpleAPI.WS.Estado.EstadoEnvio.GetEstadoEnvioAsync(entity, ambiente, pathToken, pathCertificadoValido, "Pollito702");

            Assert.True(estadoDTE.Ok);
        }

        [Fact]
        public async System.Threading.Tasks.Task EnviarAsync()
        {
            var ambiente = Ambiente.AmbienteEnum.Certificacion;
            var envioDTE = await SimpleAPI.WS.Envio.EnvioDTE.EnviarAsync("17096073-4", "76269769-6", pathEnvioDTE, pathCertificadoValido, ambiente, pathToken, "Pollito702");

            Assert.True(envioDTE.Ok);
        }

    }
}
