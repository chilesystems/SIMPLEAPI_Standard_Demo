using SimpleAPI.Enum;
using SimpleAPI.Models.DTE;
using SimpleAPI.Models.Envios;
using SimpleAPI.Security;
using SIMPLEAPI_Demo.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static SimpleAPI.Enum.Ambiente;

namespace SIMPLEAPI_Demo
{
    public partial class Main : Form
    {
        Handler handler = new Handler();
        Configuracion configuracion = new Configuracion();

        public Main()
        {
            InitializeComponent();
        }

        #region Emision de Documentos

        private void botonIngresarTimbraje_Click(object sender, EventArgs e)
        {
            IngresarTimbraje formulario = new IngresarTimbraje();
            formulario.ShowDialog();
        }

        private void botonGenerarDocumento_Click(object sender, EventArgs e)
        {
            var dte = handler.GenerateDTE(TipoDTE.DTEType.FacturaElectronica, 150);
            handler.GenerateDetails(dte);
            var path = handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");

            //handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente en " + path);
            //GenerarDocumentoElectronico formulario = new GenerarDocumentoElectronico();
            //1 formulario.ShowDialog();

        }

        private void botonGenerarEnvio_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] pathFiles = openFileDialog1.FileNames;
                List<DTE> dtes = new List<DTE>();
                List<string> xmlDtes = new List<string>();
                foreach (string pathFile in pathFiles)
                {
                    string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                    var dte = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);

                    /*Generar envio para el SII
                    Un envío puede contener 1 o varios DTE. No es necesario que sean del mismo tipo,
                    es decir, en un envío pueden ir facturas electrónicas afectas, notas de crédito, guias de despacho,
                    etc.             
                     */
                    dtes.Add(dte);
                    xmlDtes.Add(xml);
                }
                var EnvioSII = handler.GenerarEnvioDTEToSII(dtes, xmlDtes);

                /*Generar envio para el cliente
                En esencia es lo mismo que para el SII */
                //var EnvioCliente = GenerarEnvioCliente(dte, xml);
                /*Puede ser el EnvioSII o EnvioCliente, pues es el mismo tipo de objeto*/
                long min = dtes.Min(x => x.Documento.Encabezado.IdentificacionDTE.Folio);
                long max = dtes.Max(x => x.Documento.Encabezado.IdentificacionDTE.Folio);
                var filePath = EnvioSII.Firmar(configuracion.Certificado.Nombre, customName: $"Envio_{DateTime.Now.ToShortDateString()}_{min}_{max}");
                handler.Validate(filePath, Firma.TipoXML.Envio, SimpleAPI.XML.Schemas.EnvioDTE);
                MessageBox.Show("Envío generado exitosamente en " + filePath);
            }
        }

        private async void botonEnviarSii_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string pathFile = openFileDialog1.FileName;
                (long, string) retorno = await handler.EnviarEnvioDTEToSIIAsync(pathFile, radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion);
                if (retorno.Item1 == 0) MessageBox.Show("Ocurrió un error: " + retorno.Item2);
                else MessageBox.Show("Sobre enviado correctamente. TrackID: " + retorno.Item1.ToString());
            }

        }

        #endregion

        #region Simulacion

        private async void botonSimulacion_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string messageOut = string.Empty;

            List<DTE> dtes = new List<DTE>();
            List<string> xmlDtes = new List<string>();
            /*Cada valor de i se asigna como folio. Debes tener ojo con no enviar documentos con folios ya utilizados y enviados.*/
            for (int i = 31; i <= 50; i++)
            {
                var dteAux = handler.GenerateRandomDTE(i, TipoDTE.DTEType.FacturaElectronica);
                string filePath = handler.TimbrarYFirmarXMLDTE(dteAux, "out\\temp\\", "out\\caf\\");
                string xml = File.ReadAllText(filePath, Encoding.GetEncoding("ISO-8859-1"));
                dtes.Add(dteAux);
                xmlDtes.Add(xml);
            }

            var dteAux2 = handler.GenerateRandomDTE(33, TipoDTE.DTEType.NotaCreditoElectronica);
            var filePath2 = handler.TimbrarYFirmarXMLDTE(dteAux2, "out\\temp\\", "out\\caf\\");
            var xml2 = File.ReadAllText(filePath2, Encoding.GetEncoding("ISO-8859-1"));
            dtes.Add(dteAux2);
            xmlDtes.Add(xml2);

            var dteAux3 = handler.GenerateRandomDTE(23, TipoDTE.DTEType.NotaDebitoElectronica);
            var filePath3 = handler.TimbrarYFirmarXMLDTE(dteAux3, "out\\temp\\", "out\\caf\\");
            var xml3 = File.ReadAllText(filePath3, Encoding.GetEncoding("ISO-8859-1"));
            dtes.Add(dteAux3);
            xmlDtes.Add(xml3);

            var dteAux4 = handler.GenerateRandomDTE(19, TipoDTE.DTEType.FacturaCompraElectronica);
            var filePath4 = handler.TimbrarYFirmarXMLDTE(dteAux4, "out\\temp\\", "out\\caf\\");
            var xml4 = File.ReadAllText(filePath4, Encoding.GetEncoding("ISO-8859-1"));
            dtes.Add(dteAux4);
            xmlDtes.Add(xml4);

            var EnvioSII = handler.GenerarEnvioDTEToSII(dtes, xmlDtes);
            string filePathEnvio = EnvioSII.Firmar(configuracion.Certificado.Nombre);
            MessageBox.Show("Envío generado exitosamente en " + filePathEnvio);
        }



        private async void botonEnviarSimulacionSII_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string pathFile = openFileDialog1.FileName;
                var retorno = await handler.EnviarEnvioDTEToSIIAsync(pathFile, radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion);
                if (retorno.Item1 == 0) MessageBox.Show("Ocurrió un error: " + retorno.Item2);
                else MessageBox.Show($"Sobre enviado correctamente. TrackId: {retorno.Item1}");
            }

        }

        #endregion

        #region Boletas Electronicas

        private void botonGenerarBoleta_Click(object sender, EventArgs e)
        {
            GenerarDocumentoElectronico formulario = new GenerarDocumentoElectronico();
            formulario.ShowDialog();
        }

        private void botonGenerarRCOF_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] pathFiles = openFileDialog1.FileNames;
                List<DTE> dtes = new List<DTE>();
                foreach (string pathFile in pathFiles)
                {
                    string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                    var dte = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);
                    dtes.Add(dte);
                }
                var rcof = handler.GenerarRCOF(dtes);
                rcof.DocumentoConsumoFolios.Id = "RCOF_" + DateTime.Now.Ticks.ToString();
                /*Firmar retorna además a través de un out, el XML formado*/
                string xmlString = string.Empty;
                var filePathArchivo = rcof.Firmar(configuracion.Certificado.Nombre, out xmlString);
                MessageBox.Show("RCOF Generado correctamente en " + filePathArchivo);
            }
        }

        private async void botonAnularDocumento_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.ShowDialog();
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string pathFile = openFileDialog1.FileName;
                string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));

                var dteBoleta = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);

                var dteNC = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, 8);
                /*En el caso de las anulaciones, los detalles y totales son los mismos que el documento de origen*/
                dteNC.Documento.Detalles = dteBoleta.Documento.Detalles;
                dteNC.Documento.Encabezado.Totales = dteBoleta.Documento.Encabezado.Totales;
                dteNC.Documento.Referencias = new List<Referencia>();
                dteNC.Documento.Referencias.Add(new Referencia()
                {
                    CodigoReferencia = TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia,
                    FechaDocumentoReferencia = DateTime.Now,
                    //Folio de Referencia = Debe ir el folio del documento que estás referenciando
                    FolioReferencia = dteBoleta.Documento.Encabezado.IdentificacionDTE.Folio.ToString(),
                    IndicadorGlobal = 0,
                    Numero = 1,
                    RazonReferencia = "ANULA BOLETA ELECTRÓNICA",
                    TipoDocumento = "39"
                });

                var path = handler.TimbrarYFirmarXMLDTE(dteNC, "out\\temp\\", "out\\caf\\");
                handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);

                MessageBox.Show("Nota de crédito generada exitosamente en " + path);
            }

        }

        private async void botonRebajaDocumento_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string pathFile = openFileDialog1.FileName;
                string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                var dteBoleta = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);

                var dteNC = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, 11);
                /*En el caso de las anulaciones, los detalles y totales son los mismos que el documento de origen*/
                dteNC.Documento.Detalles = dteBoleta.Documento.Detalles;
                dteNC.Documento.Encabezado.Totales = dteBoleta.Documento.Encabezado.Totales;
                dteNC.Documento.Referencias = new List<Referencia>();
                dteNC.Documento.Referencias.Add(new Referencia()
                {
                    CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeMontos,
                    FechaDocumentoReferencia = DateTime.Now,
                    //Folio de Referencia = Debe ir el folio del documento que estás refenciando
                    FolioReferencia = dteBoleta.Documento.Encabezado.IdentificacionDTE.Folio.ToString(),
                    IndicadorGlobal = 0,
                    Numero = 1,
                    RazonReferencia = "CORRIGE BOLETA ELECTRÓNICA",
                    TipoDocumento = "39"
                });

                /*Calculo para el caso de una rebaja de un 40%*/
                double porc_descuento = 0.4;
                var neto = dteNC.Documento.Encabezado.Totales.MontoNeto - (dteNC.Documento.Encabezado.Totales.MontoNeto * porc_descuento);
                int netoInt = (int)Math.Round(neto, 0);
                int iva = (int)Math.Round(neto * 0.19, 0);
                int total = netoInt + iva;
                dteNC.Documento.Encabezado.Totales.MontoNeto = netoInt;
                dteNC.Documento.Encabezado.Totales.IVA = iva;
                dteNC.Documento.Encabezado.Totales.MontoTotal = total;

                dteNC.Documento.DescuentosRecargos = new List<DescuentosRecargos>();
                dteNC.Documento.DescuentosRecargos.Add(new DescuentosRecargos()
                {
                    Descripcion = "DESCUENTO COMERCIAL",
                    Numero = 1,
                    TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Descuento,
                    TipoValor = ExpresionDinero.ExpresionDineroEnum.Porcentaje,
                    Valor = porc_descuento * 100,
                });

                var path = handler.TimbrarYFirmarXMLDTE(dteNC, "out\\temp\\", "out\\caf\\");
                handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);

                MessageBox.Show("Nota de crédito generada exitosamente en " + path);
            }

        }

        private void botonGenerarEnvioBoleta_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] pathFiles = openFileDialog1.FileNames;
                List<DTE> dtes = new List<DTE>();
                List<string> xmlDtes = new List<string>();
                foreach (string pathFile in pathFiles)
                {
                    string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                    var dte = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);
                    dtes.Add(dte);
                    xmlDtes.Add(xml);
                }
                var EnvioSII = handler.GenerarEnvioBoletaDTEToSII(dtes, xmlDtes);

                long min = dtes.Min(x => x.Documento.Encabezado.IdentificacionDTE.Folio);
                long max = dtes.Max(x => x.Documento.Encabezado.IdentificacionDTE.Folio);

                var filePath = EnvioSII.Firmar(configuracion.Certificado.Nombre, customName: $"EnvioBoleta_{DateTime.Now.ToShortDateString()}_{min}_{max}");
                try
                {
                    handler.Validate(filePath, Firma.TipoXML.EnvioBoleta, SimpleAPI.XML.Schemas.EnvioBoleta);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                MessageBox.Show("Envío generado exitosamente en " + filePath);
            }

        }
        #endregion

        #region Utilitarios

        private async void botonAceptacion_Click(object sender, EventArgs e)
        {
            /*
                 * ACD: Acepta Contenido del Documento
                 * RCD: Reclamo al Contenido del Documento
                 * ERM: Otorga Recibo de Mercaderías o Servicios
                 * RFP: Reclamo por Falta Parcial de Mercaderías
                 * RFT: Reclamo por Falta Total de Mercaderías
                 */
            int tipoDocumento = 33;
            int folio = 3591;
            string rutProveedor = "76810888-9";
            var respuesta = await handler.EnviarAceptacionReclamo(tipoDocumento, folio, TipoAceptacion.ACD, rutProveedor, radioCertificacion.Checked ? AmbienteEnum.Certificacion : AmbienteEnum.Produccion);
            MessageBox.Show(respuesta);
        }
        private void botonConsultarEstadoDTE_Click(object sender, EventArgs e)
        {
            ConsultaEstadoDTE formulario = new ConsultaEstadoDTE();
            formulario.ShowDialog();
        }
        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            if (!configuracion.VerificarCarpetasIniciales())
            {
                //Los ejemplos de este proyecto se basan en estas dos carpetas. Se pueden modificar a gusto pero son necesarias al inicio.
                //Para más información: https://www.simple-api.cl/Tutoriales/Instalacion (Estructura de carpetas)
                MessageBox.Show("Se deben agregar las carpetas iniciales out\\temp, out\\caf y XML", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            configuracion.LeerArchivo();
            handler.configuracion = configuracion;
        }

        private void botonValidador_Click(object sender, EventArgs e)
        {
            Validador formulario = new Validador();
            formulario.ShowDialog();
        }

        private void botonTimbre_Click(object sender, EventArgs e)
        {
            MuestraTimbre formulario = new MuestraTimbre();
            formulario.ShowDialog();
        }

        private void botonMuestraImpresa_Click(object sender, EventArgs e)
        {
            MuestraImpresa formulario = new MuestraImpresa();
            formulario.ShowDialog();
        }

        private void botonConfiguracion_Click(object sender, EventArgs e)
        {
            ConfiguracionSistema formulario = new ConfiguracionSistema();
            formulario.ShowDialog();
            handler.configuracion.LeerArchivo();
        }

        private async void botonAgregarRef_Click(object sender, EventArgs e)
        {
            var dte = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, 105);
            handler.GenerateDetails(dte);

            //Esta referencia indica que se está corrigiendo el monto de la factura electrónica N° 50
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.CorrigeMontos, 33, DateTime.Now, 50);

            //Esta referencia indica que se está anulando la factura electrónica N° 50
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia, 33, DateTime.Now, 50);

            //Esta referencia indica que se trata de un set de pruebas
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, 0, null, null, "CASO X");

            var path = handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");

            string contenido = dte.ToString();

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente en " + path);
        }

        private void botonConsultarEstadoEnvio_Click(object sender, EventArgs e)
        {
            ConsultaEstadoEnvioDTE formulario = new ConsultaEstadoEnvioDTE();
            formulario.ShowDialog();
        }

        private void botonObtenerToken_Click(object sender, EventArgs e)
        {

        }

        private async void botonEnviarAlSIIBoletas_Click(object sender, EventArgs e)
        {
            /*Este botón no sirve si estás certificando un RUT, para ello, se debe usar el evento click del botón "botonEnviarSii". 
             * Puedes usar este botón para probar la API REST del SII para enviar tus boletas antes de pasar a producción, no sirve para certificar.*/
            openFileDialog1.Multiselect = false;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string pathFile = openFileDialog1.FileName;
                var retorno = await handler.EnviarEnvioDTEToSIIAsync(pathFile, radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion, true);
                if (retorno.Item1 == 0) MessageBox.Show("Ocurrió un error: " + retorno.Item2);
                else MessageBox.Show($"Sobre enviado correctamente. TrackID: {retorno.Item1}");
            }
        }

        private void botonEnviarAlSIIBoletas_Certificacion_Click(object sender, EventArgs e)
        {
            botonEnviarSii_Click(null, null);
        }

        private void botonGenerarRCOFVacio_Click(object sender, EventArgs e)
        {
            var rcof = handler.GenerarRCOFVacio(DateTime.Now);
            rcof.DocumentoConsumoFolios.Id = "RCOF_" + DateTime.Now.Ticks.ToString();
            /*Firmar retorna además a través de un out, el XML formado*/
            string xmlString = string.Empty;
            var filePathArchivo = rcof.Firmar(configuracion.Certificado.Nombre, out xmlString);
            MessageBox.Show("RCOF Generado correctamente en " + filePathArchivo);
        }
    }
}
