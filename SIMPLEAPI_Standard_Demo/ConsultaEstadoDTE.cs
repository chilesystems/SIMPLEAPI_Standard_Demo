using Newtonsoft.Json;
using SimpleAPI.Enum;
using System;
using System.Windows.Forms;
using static SimpleAPI.Enum.Ambiente;

namespace SIMPLEAPI_Demo
{
    public partial class ConsultaEstadoDTE : Form
    {
        Handler handler = new Handler();
        public ConsultaEstadoDTE()
        {
            InitializeComponent();
        }

        private async void botonConsultar_Click(object sender, EventArgs e)
        {
            int rutReceptor = int.Parse(textRUTReceptor.Text);
            string dvReceptor = textDVReceptor.Text;
            int folio = int.Parse(textFolio.Text);
            int total = int.Parse(textTotal.Text);
            Enum.TryParse(comboTipoDTE.SelectedItem.ToString(), out TipoDTE.DTEType tipoDTE);
            if (checkIsBoletaCertificacion.Checked || !(tipoDTE == TipoDTE.DTEType.BoletaElectronica || tipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta))
            {
                var responseEstadoDTE = await handler.ConsultarEstadoDTEAsync(radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion, $"{textRUTReceptor.Text}-{textDVReceptor.Text}", tipoDTE, folio, dateFechaEmision.Value.Date, total);
                textRespuesta.Text = responseEstadoDTE.Response;
            }
            else
            {
                var responseEstadoDTE = await handler.ConsultarEstadoBoletaAsync(radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion, $"{textRUTReceptor.Text}-{textDVReceptor.Text}", tipoDTE, folio, dateFechaEmision.Value.Date, total);
                textRespuesta.Text = JsonConvert.SerializeObject(responseEstadoDTE, Formatting.Indented);
            }
        }

        private void ConsultaEstadoDTE_Load(object sender, EventArgs e)
        {
            foreach (var tipo in Enum.GetValues(typeof(TipoDTE.DTEType)))
            {
                comboTipoDTE.Items.Add(tipo);
            }
            handler.configuracion.LeerArchivo();
            textRUTEmpresa.Text = handler.configuracion.Empresa.RutCuerpo.ToString();
            textDVEmpresa.Text = handler.configuracion.Empresa.DV;
            textRUTEnvio.Text = handler.configuracion.Certificado.RutCuerpo.ToString();
            textDVEnvio.Text = handler.configuracion.Certificado.DV;

            comboTipoDTE.SelectedIndex = 0;
        }

        private void comboTipoDTE_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboTipoDTE.SelectedItem.ToString(), out TipoDTE.DTEType tipoDTE);
            checkIsBoletaCertificacion.Enabled = tipoDTE == TipoDTE.DTEType.BoletaElectronica || tipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta;
        }
    }
}
