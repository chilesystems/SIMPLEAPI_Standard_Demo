using SimpleAPI.Models.DTE;
using SimpleAPI.Security;
using SimpleAPI.XML;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SIMPLEAPI_Demo
{
    public partial class MuestraTimbre : Form
    {
        public MuestraTimbre()
        {
            InitializeComponent();
        }

        private void botonCargarDTE_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string pathFile = openFileDialog1.FileName;
            string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));

            var dte = XmlHandler.DeserializeFromString<DTE>(xml);
            using (var ms = new MemoryStream(dte.Documento.TimbrePDF417(out string outMessage)))
            {
                pictureBoxTimbre.BackgroundImage = Image.FromStream(ms);
            }

        }

        private void botonValidar_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Seleccione XML de CAF";
            openFileDialog1.ShowDialog();
            string pathFileCaf = openFileDialog1.FileName;
            string xmlCAF = File.ReadAllText(pathFileCaf, Encoding.GetEncoding("ISO-8859-1"));

            openFileDialog1.Title = "Seleccione XML de DTE";
            openFileDialog1.ShowDialog();
            string pathFileDTE = openFileDialog1.FileName;
            string xmlDTE = File.ReadAllText(pathFileDTE, Encoding.GetEncoding("ISO-8859-1"));

            var objetoDte = XmlHandler.TryDeserializeFromString<DTE>(xmlDTE);
            string firmadelDD = objetoDte.Documento.TED.FirmaDigital.Firma;

            string privateKey = CAFHandler.GetPrivateKey(pathFileCaf);


            string firmaResultante = Timbre.Timbrar(objetoDte.Documento.TED.DatosBasicos.ToString(), privateKey);

            MessageBox.Show((firmaResultante == firmadelDD).ToString());
        }
    }
}
