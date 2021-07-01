using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using static SimpleAPI.Enum.Ambiente;

namespace SIMPLEAPI_Demo
{
    public partial class ConsultaEstadoEnvioDTE : Form
    {
        Handler handler = new Handler();
        public ConsultaEstadoEnvioDTE()
        {
            InitializeComponent();
        }

        private void ConsultaEstadoAvanzadoDTE_Load(object sender, EventArgs e)
        {
            handler.configuracion.LeerArchivo();
            textRUTEmpresa.Text = handler.configuracion.Empresa.RutCuerpo.ToString();
            textDVEmpresa.Text = handler.configuracion.Empresa.DV;
        }

        private async void botonConsultar_Click(object sender, EventArgs e)
        {
            long trackId = long.Parse(textTrackID.Text);
            try
            {
                if (radioEnvioDTE.Checked || checkIsBoletaCertificacion.Checked)
                {
                    var responseEstadoDTE = await handler.ConsultarEstadoEnvioDTEAsync(radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion, trackId);
                    textRespuesta.Text = responseEstadoDTE.ResponseXml;
                }
                else
                {
                    var responseEstadoDTE = await handler.ConsultarEstadoEnvioBoletaAsync(radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion, trackId);
                    textRespuesta.Text = JsonConvert.SerializeObject(responseEstadoDTE, Formatting.Indented);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error:" + ex);
            }
        }

        private void radioEnvioBoleta_CheckedChanged(object sender, EventArgs e)
        {
            checkIsBoletaCertificacion.Enabled = radioEnvioBoleta.Checked;
        }
    }
}
