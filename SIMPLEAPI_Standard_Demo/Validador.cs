using SimpleAPI.Models.DTE;
using SimpleAPI.Models.Envios;
using SimpleAPI.Security;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SIMPLEAPI_Demo
{
    public partial class Validador : Form
    {
        public Validador()
        {
            InitializeComponent();
        }

        private void botonBuscar_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName))
            {
                try
                {
                    string xml = File.ReadAllText(openFileDialog1.FileName, Encoding.GetEncoding("ISO-8859-1"));

                    string tipoSchema = string.Empty;
                    Firma.TipoXML tipoFirma = Firma.TipoXML.NotSet;
                    string tipo = comboTipo.SelectedItem.ToString();
                    if (tipo == "DTE")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.DTE;
                        tipoFirma = Firma.TipoXML.DTE;

                        var dte = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);
                        textDocumento.Text = "FOLIO: " + dte.Documento.Encabezado.IdentificacionDTE.Folio + ". EMISOR: " + dte.Documento.Encabezado.Emisor.RazonSocial;

                    }
                    else if (tipo == "SOBREENVIO")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.EnvioDTE;
                        tipoFirma = Firma.TipoXML.Envio;

                        var envio = SimpleAPI.XML.XmlHandler.TryDeserializeFromString<EnvioDTE>(xml);
                        // textDocumento.Text = "DOCUMENTOS: " + String.Join(",", envio.SetDTE.DTEs);

                    }
                    else if (tipo == "ENVIOBOLETA")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.EnvioBoleta;
                        tipoFirma = Firma.TipoXML.EnvioBoleta;
                    }
                    else if (tipo == "IECV")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.LCV_LIBRO;
                        tipoFirma = Firma.TipoXML.LCV;
                    }
                    else if (tipo == "CONSUMOFOLIOS")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.ConsumoFolios;
                        tipoFirma = Firma.TipoXML.RCOF;
                    }
                    else if (tipo == "LIBROBOLETA")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.LibroBoletas;
                        tipoFirma = Firma.TipoXML.LibroBoletas;
                    }
                    else if (tipo == "AEC")
                    {
                        tipoSchema = SimpleAPI.XML.Schemas.AEC;
                        tipoFirma = Firma.TipoXML.AEC;
                    }
                    string messageResultSchema = string.Empty;
                    string messageResultFirma = string.Empty;
                    if (SimpleAPI.XML.XmlHandler.ValidateWithSchema(openFileDialog1.FileName, out messageResultSchema, tipoSchema))
                    {
                        if (Firma.VerificarFirma(openFileDialog1.FileName, tipoFirma, out messageResultFirma))
                        {
                            textResultado.Text = "SCHEMA CORRECTO." + Environment.NewLine + " FIRMA CORRECTA.";
                        }
                        else
                            textResultado.Text = "NO SE PUDO VERIFICAR LA FIRMA: " + messageResultFirma;
                    }
                    else
                    {
                        textResultado.Text = "NO SE PUDO VERIFICAR EL SCHEMA: \n" + messageResultSchema;
                    }
                }
                catch (Exception ex)
                {
                    textResultado.Text = "ERROR: " + ex.ToString();
                }

            }
        }

        private void Validador_Load(object sender, EventArgs e)
        {
            comboTipo.SelectedIndex = 0;
        }

        private void ComboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
