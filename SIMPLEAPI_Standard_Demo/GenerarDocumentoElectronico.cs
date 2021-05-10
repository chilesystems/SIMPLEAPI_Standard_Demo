using SimpleAPI.Enum;
using SimpleAPI.Security;
using SIMPLEAPI_Demo.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIMPLEAPI_Demo
{
    public partial class GenerarDocumentoElectronico : Form
    {
        Handler handler = new Handler();
        List<ItemBoleta> items;

        public GenerarDocumentoElectronico()
        {
            InitializeComponent();
            items = new List<ItemBoleta>();
        }

        private void GenerarBoletaElectronica_Load(object sender, EventArgs e)
        {
            gridResultados.AutoGenerateColumns = false;
            comboTipo.SelectedIndex = 0;
            
            handler.configuracion = new Configuracion();
            handler.configuracion.LeerArchivo();

            textRUTEmisor.Text = handler.configuracion.Empresa.RutEmpresa;
            textRazonSocialEmisor.Text = handler.configuracion.Empresa.RazonSocial;
            textDireccionEmisor.Text = handler.configuracion.Empresa.Direccion;
            textComunaEmisor.Text = handler.configuracion.Empresa.Comuna;
            textGiroEmisor.Text = handler.configuracion.Empresa.Giro;

            textRUTReceptor.Text = "66666666-6";
            textRazonSocialReceptor.Text = "Razón Social de Cliente";
            textDireccionReceptor.Text = "Dirección de Cliente";
            textComunaReceptor.Text = "Comuna de Cliente";
            textGiroReceptor.Text = "Giro de Cliente";
        }

        private void botonAgregarLinea_Click(object sender, EventArgs e)
        {
            ItemBoleta item = new ItemBoleta();
            item.Nombre = textNombre.Text;
            item.Cantidad = numericCantidad.Value;
            item.Afecto = checkAfecto.Checked;
            item.Precio = (int)numericPrecio.Value;
            item.UnidadMedida = checkUnidad.Checked ? "Kg." : string.Empty;
            items.Add(item);
            gridResultados.DataSource = null;
            gridResultados.DataSource = items;

            textNombre.Text = "";
            numericCantidad.Value = 1;
            checkAfecto.Checked = true;
        }

        private void gridResultados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (e.ColumnIndex == 6)
                {
                    var item = gridResultados.Rows[e.RowIndex].DataBoundItem as ItemBoleta;
                    items.Remove(item);
                    gridResultados.DataSource = null;
                    gridResultados.DataSource = items;
                }
            }
        }

        private async void botonGenerar_Click(object sender, EventArgs e)
        {
            var tipoDte = comboTipo.SelectedIndex == 0 ? TipoDTE.DTEType.BoletaElectronica : TipoDTE.DTEType.FacturaElectronica;

            var emisor = new SimpleAPI.Models.DTE.Emisor() 
            {
                Rut = textRUTEmisor.Text,
                DireccionOrigen = textDireccionEmisor.Text,
                ComunaOrigen = textComunaEmisor.Text
            };

            var receptor = new SimpleAPI.Models.DTE.Receptor() 
            {
                Rut = textRUTReceptor.Text,
                RazonSocial = textRazonSocialReceptor.Text,
                Direccion = textDireccionReceptor.Text,
                Comuna = textComunaReceptor.Text
            };

            if (tipoDte == TipoDTE.DTEType.BoletaElectronica)
            {
                emisor.RazonSocialBoleta = textRazonSocialEmisor.Text;
                emisor.GiroBoleta = textGiroEmisor.Text;
            }
            else
            {
                emisor.RazonSocial = textRazonSocialEmisor.Text;
                emisor.Giro = textGiroEmisor.Text;
                emisor.ActividadEconomica = handler.configuracion.Empresa.CodigosActividades.Select(x => x.Codigo).ToList();
                receptor.Ciudad = receptor.Comuna;
                receptor.Giro = textGiroReceptor.Text;
            }

            var dte = handler.GenerateDTE(tipoDte, (int)numericFolio.Value, emisor, receptor);
            handler.GenerateDetails(dte, items);

            if (checkSetPruebas.Checked)
            {
                string casoPrueba = "CASO-" + numericCasoPrueba.Value.ToString("N0");
                handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, tipoDte == TipoDTE.DTEType.BoletaElectronica ? TipoDTE.TipoReferencia.BoletaElectronica : TipoDTE.TipoReferencia.FacturaElectronica, null, 0, casoPrueba);
            }

            var path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente");
        }

        private void checkSetPruebas_CheckedChanged(object sender, EventArgs e)
        {
            labelCasoPrueba.Enabled = numericCasoPrueba.Enabled = checkSetPruebas.Checked;
        }
    }
}
