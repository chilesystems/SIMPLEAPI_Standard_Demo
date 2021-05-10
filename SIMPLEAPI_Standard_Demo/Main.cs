using SimpleAPI.Enum;
using SimpleAPI.Models.Envios;
using SimpleAPI.Models.DTE;
using SimpleAPI.Security;
using SIMPLEAPI_Demo.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static SimpleAPI.Enum.Ambiente;
using System.Linq;

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
            GenerarDocumentoElectronico formulario = new GenerarDocumentoElectronico();
            formulario.ShowDialog();
      
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
                int min = dtes.Min(x => x.Documento.Encabezado.IdentificacionDTE.Folio);
                int max = dtes.Max(x => x.Documento.Encabezado.IdentificacionDTE.Folio);
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
                string filePath = await handler.TimbrarYFirmarXMLDTE(dteAux, "out\\temp\\", "out\\caf\\");
                string xml = File.ReadAllText(filePath, Encoding.GetEncoding("ISO-8859-1"));
                dtes.Add(dteAux);
                xmlDtes.Add(xml);
            }

            var dteAux2 = handler.GenerateRandomDTE(33, TipoDTE.DTEType.NotaCreditoElectronica);
            var filePath2 = await handler.TimbrarYFirmarXMLDTE(dteAux2, "out\\temp\\", "out\\caf\\");
            var xml2 = File.ReadAllText(filePath2, Encoding.GetEncoding("ISO-8859-1"));
            dtes.Add(dteAux2);
            xmlDtes.Add(xml2);

            var dteAux3 = handler.GenerateRandomDTE(23, TipoDTE.DTEType.NotaDebitoElectronica);
            var filePath3 = await handler.TimbrarYFirmarXMLDTE(dteAux3, "out\\temp\\", "out\\caf\\");
            var xml3 = File.ReadAllText(filePath3, Encoding.GetEncoding("ISO-8859-1"));
            dtes.Add(dteAux3);
            xmlDtes.Add(xml3);

            var dteAux4 = handler.GenerateRandomDTE(19, TipoDTE.DTEType.FacturaCompraElectronica);
            var filePath4 = await handler.TimbrarYFirmarXMLDTE(dteAux4, "out\\temp\\", "out\\caf\\");
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
                var retorno = await handler.EnviarEnvioDTEToSIIAsync(pathFile,  radioProduccion.Checked ? AmbienteEnum.Produccion : AmbienteEnum.Certificacion);
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
                    TipoDocumento = TipoDTE.TipoReferencia.BoletaElectronica
                });

                var path = await handler.TimbrarYFirmarXMLDTE(dteNC, "out\\temp\\", "out\\caf\\");
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
                    TipoDocumento = TipoDTE.TipoReferencia.BoletaElectronica
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

                var path = await handler.TimbrarYFirmarXMLDTE(dteNC, "out\\temp\\", "out\\caf\\");
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

                int min = dtes.Min(x => x.Documento.Encabezado.IdentificacionDTE.Folio);
                int max = dtes.Max(x => x.Documento.Encabezado.IdentificacionDTE.Folio);

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
            int folio = 17158136;
            string accion = "ACD";
            string rutProveedor = "88888888-8";
            var respuesta = await handler.EnviarAceptacionReclamo(tipoDocumento, folio, accion, rutProveedor, radioCertificacion.Checked ? AmbienteEnum.Certificacion : AmbienteEnum.Produccion);
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

        private async void botonSetPruebas_Click(object sender, EventArgs e)
        {
            List<string> pathFiles = new List<string>();
            List<int> folios = new List<int>();

            string nAtencion = "1092644";

            for (int i = 51; i<= 53; i++) //4 facturas
                folios.Add(i);
            for (int i = 14; i <= 16; i++) // 3 notas de credito
                folios.Add(i);            
            folios.Add(6); //1 nota de debito           

            #region DTEs
            /******************************/
            string casoPruebas = "CASO " + nAtencion + "-1";
            var dte = handler.GenerateDTE(TipoDTE.DTEType.FacturaElectronica, folios[0]);

            var detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 139,
                Nombre = "Cajón AFECTO",
                Precio = 1838,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 59,
                Nombre = "Relleno AFECTO",
                Precio = 3021,
                Afecto = true
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[0], casoPruebas);
            var path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            
            /********************************/

            /********************************/
            casoPruebas = "CASO " + nAtencion + "-2";
            dte = handler.GenerateDTE(TipoDTE.DTEType.FacturaElectronica, folios[1]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 422,
                Nombre = "Pañuelo AFECTO",
                Precio = 3334,
                Descuento = 6,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 354,
                Nombre = "ITEM 2 AFECTO",
                Precio = 2393,
                Descuento = 12,
                Afecto = true
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[1], casoPruebas);
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            /********************************/
            casoPruebas = "CASO " + nAtencion + "-3";
            dte = handler.GenerateDTE(TipoDTE.DTEType.FacturaElectronica, folios[2]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 32,
                Nombre = "Pintura B&W AFECTO",
                Precio = 3820,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 180,
                Nombre = "ITEM 2 AFECTO",
                Precio = 3261,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 1,
                Nombre = "ITEM 3 SERVICIO EXENTO",
                Precio = 34914,
                Afecto = false
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[2], casoPruebas);
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            /********************************/
            casoPruebas = "CASO " + nAtencion + "-4";
            dte = handler.GenerateDTE(TipoDTE.DTEType.FacturaElectronica, folios[3]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 200,
                Nombre = "ITEM 1 AFECTO",
                Precio = 3186,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 85,
                Nombre = "ITEM 2 AFECTO",
                Precio = 3473,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 2,
                Nombre = "ITEM 3 SERVICIO EXENTO",
                Precio = 6791,
                Afecto = false
            });

            dte.Documento.DescuentosRecargos = new List<DescuentosRecargos>();
            dte.Documento.DescuentosRecargos.Add(new DescuentosRecargos()
            {
                Descripcion = "DESCUENTO COMERCIAL",
                Numero = 1,
                TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Descuento,
                TipoValor = ExpresionDinero.ExpresionDineroEnum.Porcentaje,
                Valor = 12
            });

            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[3], casoPruebas);
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            /********************************/
            casoPruebas = "CASO " + nAtencion + "-5";
            dte = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, folios[4]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Nombre = "CORRIGE GIRO DEL RECEPTOR",
                Afecto = true
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[4], casoPruebas);
            dte.Documento.Referencias.Add(new Referencia()
            {
                CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeTextoDocumentoReferencia,
                FechaDocumentoReferencia = DateTime.Now,
                FolioReferencia = folios[0].ToString(),
                IndicadorGlobal = 0,
                Numero = 2,
                RazonReferencia = "CORRIGE GIRO DEL RECEPTOR",
                TipoDocumento = TipoDTE.TipoReferencia.FacturaElectronica
            });
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            /********************************/
            casoPruebas = "CASO " + nAtencion + "-6"; 
            dte = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, folios[5]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 155,
                Nombre = "Pañuelo AFECTO",
                Precio = 3334,
                Descuento = 6,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 240,
                Nombre = "ITEM 2 AFECTO",
                Precio = 2393,
                Descuento = 12,
                Afecto = true
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[5], casoPruebas);
            dte.Documento.Referencias.Add(new Referencia()
            {
                CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeMontos,
                FechaDocumentoReferencia = DateTime.Now,
                FolioReferencia = folios[1].ToString(),
                IndicadorGlobal = 0,
                Numero = 2,
                RazonReferencia = "DEVOLUCIÓN DE MERCADERÍAS",
                TipoDocumento = TipoDTE.TipoReferencia.FacturaElectronica
            });
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            /********************************/
            casoPruebas = "CASO " + nAtencion + "-7";
            dte = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, folios[6]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 32,
                Nombre = "Pintura B&W AFECTO",
                Precio = 3820,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 180,
                Nombre = "ITEM 2 AFECTO",
                Precio = 3261,
                Afecto = true
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 1,
                Nombre = "ITEM 3 SERVICIO EXENTO",
                Precio = 34914,
                Afecto = false
            });

            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[6], casoPruebas);
            dte.Documento.Referencias.Add(new Referencia()
            {
                CodigoReferencia = TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia,
                FechaDocumentoReferencia = DateTime.Now,
                FolioReferencia = folios[2].ToString(),
                IndicadorGlobal = 0,
                Numero = 2,
                RazonReferencia = "ANULA FACTURA",
                TipoDocumento = TipoDTE.TipoReferencia.FacturaElectronica
            });
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/


            /********************************/
            casoPruebas = "CASO " + nAtencion + "-8";
            dte = handler.GenerateDTE(TipoDTE.DTEType.NotaDebitoElectronica, folios[7]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Nombre = "CORRIGE GIRO DEL RECEPTOR",
                Afecto = true
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[7], casoPruebas);
            dte.Documento.Referencias.Add(new Referencia()
            {
                CodigoReferencia = TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia,
                FechaDocumentoReferencia = DateTime.Now,
                FolioReferencia = folios[4].ToString(),
                IndicadorGlobal = 0,
                Numero = 2,
                RazonReferencia = "ANULA NOTA DE CREDITO",
                TipoDocumento = TipoDTE.TipoReferencia.NotaCreditoElectronica
            });
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/
            #endregion

            #region Envio de Documentos

            List<DTE> dtes = new List<DTE>();
            List<string> xmlDtes = new List<string>();
            foreach (string pathFile in pathFiles)
            {
                string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                var dteAux = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);
                dtes.Add(dteAux);
                xmlDtes.Add(xml);
            }
            var EnvioSII = handler.GenerarEnvioDTEToSII(dtes, xmlDtes);
            path = EnvioSII.Firmar(configuracion.Certificado.Nombre);
            handler.Validate(path, Firma.TipoXML.Envio, SimpleAPI.XML.Schemas.EnvioDTE);
            MessageBox.Show("Envío generado exitosamente en " + path);

            #endregion

            #region Libro de VENTAS

            var libroVentas = handler.GenerateLibroVentas(EnvioSII);
            path = libroVentas.Firmar(configuracion.Certificado.Nombre, "out\\temp\\");

            MessageBox.Show("Libro ventas guardado en " + path);
            #endregion

            #region Libro de COMPRAS

            var libroCompras = handler.GenerateLibroCompras();
            path = libroCompras.Firmar(configuracion.Certificado.Nombre, "out\\temp\\");

            MessageBox.Show("Libro compras guardado en " + path);
            #endregion

            MessageBox.Show("Proceso de set de pruebas terminado", "Al fin :)", MessageBoxButtons.OK, MessageBoxIcon.Information);

            
        }

        private async void botonFacturaCompra_Click(object sender, EventArgs e)
        {
            List<string> pathFiles = new List<string>();
            List<int> folios = new List<int>();

            string nAtencion = "1092644";

            folios.Add(18); //factura de compra
            folios.Add(32); //NC
            folios.Add(22); //ND    
            nAtencion = "1092647";

            string casoPruebas = "CASO " + nAtencion + "-1";
            var dte = handler.GenerateDTE(TipoDTE.DTEType.FacturaElectronica, folios[0]);

            var detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 521,
                Nombre = "Producto 1",
                Precio = 4301,
                Afecto = true,
                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 27,
                Nombre = "Producto 2",
                Precio = 2279,
                Afecto = true,
                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[0], casoPruebas);
            var path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            /********************************/
            dte = handler.GenerateDTE(TipoDTE.DTEType.NotaCreditoElectronica, folios[1]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 174,
                Nombre = "Producto 1",
                Precio = 4301,
                Afecto = true,
                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 9,
                Nombre = "Producto 2",
                Precio = 2279,
                Afecto = true,
                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[1], casoPruebas);
            dte.Documento.Referencias.Add(new Referencia()
            {
                CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeMontos,
                FechaDocumentoReferencia = DateTime.Now,
                FolioReferencia = folios[0].ToString(),
                IndicadorGlobal = 0,
                Numero = 2,
                RazonReferencia = "DEVOLUCIÓN DE MERCADERÍA ITEMS 1 Y 2",
                TipoDocumento = TipoDTE.TipoReferencia.FacturaCompraElectronica
            });
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            dte = handler.GenerateDTE(TipoDTE.DTEType.NotaDebitoElectronica, folios[2]);
            detalles = new List<ItemBoleta>();
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 174,
                Nombre = "Producto 1",
                Precio = 4301,
                Afecto = true,
                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal

            });
            detalles.Add(new ItemBoleta()
            {
                Cantidad = 9,
                Nombre = "Producto 2",
                Precio = 2279,
                Afecto = true,
                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
            });
            handler.GenerateDetails(dte, detalles);
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, DateTime.Now, folios[2], casoPruebas);
            dte.Documento.Referencias.Add(new Referencia()
            {
                CodigoReferencia = TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia,
                FechaDocumentoReferencia = DateTime.Now,
                FolioReferencia = folios[1].ToString(),
                IndicadorGlobal = 0,
                Numero = 2,
                RazonReferencia = "ANULA NOTA DE CREDITO",
                TipoDocumento = TipoDTE.TipoReferencia.NotaCreditoElectronica
            });
            path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");
            pathFiles.Add(path);
            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            /********************************/

            var dtes = new List<DTE>();
            var xmlDtes = new List<string>();
            foreach (string pathFile in pathFiles)
            {
                string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                var dteAux = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);
                dtes.Add(dteAux);
                xmlDtes.Add(xml);
            }
            var EnvioSII = handler.GenerarEnvioDTEToSII(dtes, xmlDtes);
            path = EnvioSII.Firmar(configuracion.Certificado.Nombre);
            handler.Validate(path, Firma.TipoXML.Envio, SimpleAPI.XML.Schemas.EnvioDTE);
            MessageBox.Show("Envío generado exitosamente en " + path);

        }
        private void botonIntercambio_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.ShowDialog();
            string pathFile = openFileDialog1.FileName;
            string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
            var envio = SimpleAPI.XML.XmlHandler.TryDeserializeFromString<EnvioDTE>(xml);

            /*Respuesta de Intercambio*/
            var filePath = handler.GenerarRespuestaEnvio(envio.SetDTE.DTEs, "ACD");
            MessageBox.Show("Respuesta de Intercambio " + filePath);

            /*Recibo de mercaderías*/
            filePath = handler.AcuseReciboMercaderias(envio.SetDTE.DTEs[0]);
            MessageBox.Show("Acuse recibo " + filePath);

            /*Aprobación comercial de documento*/
            filePath = handler.ResponderDTE(0, envio.SetDTE.DTEs[0], "PRUEBA");
            MessageBox.Show("Aprobación comercial " + filePath);

           
        }

        private void botonCesion_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                /*La cesión se debe realizar a partir de un DTE existente
                 Para ello, se carga el correspondiente XML. Puede ser un <EnvioDTE> o <DTE>,
                 Sin embargo, al usar el primero, hay que hacer la respectiva modificación 
                 de Tipo en XmlHandler.DeserializeFromString<T>*/

                string pathFile = openFileDialog1.FileName;
                string xml = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));
                

                /*Creo el objeto AEC*/
                var AEC = new SimpleAPI.Models.Cesion.AEC();

                /*Creo el objeto DteCedido a partir del XML leído. En caso del XML haber sido del
                 tipo <EnvioDTE>, hay que rescatar sólo el XML desde el tag <DTE>, según el DTE que
                 se quiera ceder.

                 La variable xmlDteCedido me indica el Path donde está el DteCedido firmado.
                 */

                var dteCedido = new SimpleAPI.Models.Cesion.DTECedido(xml);
                var xmlDteCedido = dteCedido.Firmar(configuracion.Certificado.Nombre, out string message);


                /*Creo el objeto cesion a partir de DTE leído, se le indica el número de secuencia de
                  la cesión. Pueden existir varias cesiones.*/
                var dte = SimpleAPI.XML.XmlHandler.DeserializeFromString<DTE>(xml);
                var cesion = new SimpleAPI.Models.Cesion.Cesion(dte, 1);

                /*Datos del factoring*/
                var cesionario = new SimpleAPI.Models.Cesion.Cesionario()
                {
                    Direccion = "Dirección Cesionario",
                    eMail = "Email Cesionario",
                    RazonSocial = "Factoring LTDA",
                    RUT = "11111111-1"
                };

                var cedente = new SimpleAPI.Models.Cesion.Cedente()
                {
                    RUT = dte.Documento.Encabezado.Emisor.Rut,
                    RazonSocial = dte.Documento.Encabezado.Emisor.RazonSocial,
                    Direccion = dte.Documento.Encabezado.Emisor.DireccionOrigen +", " + dte.Documento.Encabezado.Emisor.ComunaOrigen,
                    eMail = dte.Documento.Encabezado.Emisor.CorreoElectronico,
                    RUTsAutorizados = new List<SimpleAPI.Models.Cesion.RUTAutorizado>()
                    {
                        new SimpleAPI.Models.Cesion.RUTAutorizado()
                        {
                            Nombre = "Nombre Autorizado",
                            RUT = "RUT Autorizado"
                        }
                    }
                };

                string declaracionJurada = string.Format(
                 @"Se declara bajo juramento que {0}, RUT {1} ha puesto a disposición del 
                    cesionario {2}, RUT {3}, el o los documentos donde constan los recibos 
                    de las mercaderías entregadas o servicios prestados, entregados por parte 
                    del deudor de la factura {4}, RUT {5}, de acuerdo a lo establecido en la 
                    Ley N° 19.983", 
                 cedente.RazonSocial, 
                 cedente.RUT, 
                 cesionario.RazonSocial,
                 cesionario.RUT,
                 dte.Documento.Encabezado.Receptor.RazonSocial,
                 dte.Documento.Encabezado.Receptor.Rut);

                cedente.DeclaracionJurada = declaracionJurada;

                cesion.DocumentoCesion.Cedente = cedente;
                cesion.DocumentoCesion.Cesionario = cesionario;

                /*la variable cesionXML contiene el path de la cesión firmada*/
                var cesionXML = cesion.Firmar(configuracion.Certificado.Nombre, out message);

                AEC.DocumentoAEC.Caratula = new SimpleAPI.Models.Cesion.Caratula()
                {
                    MailContacto = cedente.eMail,
                    NombreContacto = cedente.RUTsAutorizados[0].Nombre,
                    RutCedente = cedente.RUT,
                    RutCesionario = cesionario.RUT,
                    TmstFirmaEnvio = DateTime.Now
                };

                /*Las cesiones y el Dte cedido, deben agregarse al objeto AEC como strings*/
                AEC.signedXMLCedido = File.ReadAllText(xmlDteCedido, Encoding.GetEncoding("ISO-8859-1"));
                AEC.DocumentoAEC.Cesiones.DTECedido = dteCedido;
                AEC.signedXMLCesion.Add(File.ReadAllText(cesionXML, Encoding.GetEncoding("ISO-8859-1")));
                AEC.DocumentoAEC.ID = "ID_TEST";
                var filePathAEC = AEC.Firmar(configuracion.Certificado.Nombre, out message);
                File.Delete(xmlDteCedido);
                File.Delete(cesionXML);

                MessageBox.Show("AEC generado exitosamente en " + filePathAEC);

                //var path = handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");

            }

        }

        private void BotonSetExportacion_Click(object sender, EventArgs e)
        {
            var n_atencion = "1221632";
            int folioFactura = 51;
            int folioNC = 51;
            int folioND = 51;
            #region SET EXPORTACION 1

            #region FACTURA EXPORTACION
            var dte = handler.GenerateDTEExportacionBase(TipoDTE.DTEType.FacturaExportacionElectronica, folioFactura);

            dte.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion = CodigosAduana.FormaPagoExportacionEnum.ACRED;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoModalidadVenta = CodigosAduana.ModalidadVenta.A_FIRME;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoClausulaVenta = CodigosAduana.ClausulaCompraVenta.FOB;
            dte.Exportaciones.Encabezado.Transporte.Aduana.TotalClausulaVenta = 285.88;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoViaTransporte = CodigosAduana.ViasdeTransporte.AEREO;

            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPuertoEmbarque = CodigosAduana.Puertos.ARICA;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPuertoDesembarque = CodigosAduana.Puertos.BUENOS_AIRES;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoUnidadMedidaTara = CodigosAduana.UnidadMedida.U;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoUnidadPesoBruto = CodigosAduana.UnidadMedida.U;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoUnidadPesoNeto = CodigosAduana.UnidadMedida.U;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CantidadBultos = 15;
            dte.Exportaciones.Encabezado.Transporte.Aduana.TipoBultos = new List<TipoBulto>();
            dte.Exportaciones.Encabezado.Transporte.Aduana.TipoBultos.Add(new TipoBulto()
            {
                CantidadBultos = 15,
                CodigoTipoBulto = CodigosAduana.TipoBultoEnum.CONTENEDOR_REFRIGERADO,
                IdContainer = "erer787df",
                Sello = "SelloTest"
            });
            dte.Exportaciones.Encabezado.Transporte.Aduana.MontoFlete = 13.62;
            dte.Exportaciones.Encabezado.Transporte.Aduana.MontoSeguro = 0.65;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPaisDestino = CodigosAduana.Paises.ARGENTINA;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPaisReceptor = CodigosAduana.Paises.ARGENTINA;


            dte.Exportaciones.Detalles = new List<DetalleExportacion>();
            var detalle = new DetalleExportacion();
            detalle.NumeroLinea = 1;
            detalle.IndicadorExento = IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalle.Nombre = "CHATARRA DE ALUMINIO";
            detalle.Cantidad = 148;
            detalle.UnidadMedida = "U";
            detalle.Precio = 105;
            detalle.MontoItem = 148 * 105;
            dte.Exportaciones.Detalles.Add(detalle);

            dte.Exportaciones.DescuentosRecargos = new List<DescuentosRecargos>();

            var descuentoFlete = new DescuentosRecargos();
            descuentoFlete.Numero = 1;
            descuentoFlete.TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Recargo;
            descuentoFlete.Descripcion = "Recargo flete";
            descuentoFlete.TipoValor = ExpresionDinero.ExpresionDineroEnum.Pesos;
            descuentoFlete.IndicadorExento = IndicadorExento.IndicadorExentoEnum.Exento;
            descuentoFlete.Valor = dte.Exportaciones.Encabezado.Transporte.Aduana.MontoFlete;
            dte.Exportaciones.DescuentosRecargos.Add(descuentoFlete);

            var descuentoSeguro = new DescuentosRecargos();
            descuentoSeguro.Numero = 2;
            descuentoSeguro.TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Recargo;
            descuentoSeguro.Descripcion = "Recargo seguro";
            descuentoSeguro.TipoValor = ExpresionDinero.ExpresionDineroEnum.Pesos;
            descuentoSeguro.IndicadorExento = IndicadorExento.IndicadorExentoEnum.Exento;
            descuentoSeguro.Valor = dte.Exportaciones.Encabezado.Transporte.Aduana.MontoSeguro;
            dte.Exportaciones.DescuentosRecargos.Add(descuentoSeguro);

            dte.Exportaciones.Referencias = new List<Referencia>();
            var referenciaSetPruebas = new Referencia();
            referenciaSetPruebas.Numero = 1;
            referenciaSetPruebas.TipoDocumento = TipoDTE.TipoReferencia.SetPruebas;
            referenciaSetPruebas.FolioReferencia = dte.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaSetPruebas.FechaDocumentoReferencia = DateTime.Now;
            referenciaSetPruebas.RazonReferencia = "CASO " + n_atencion + "-1";
            dte.Exportaciones.Referencias.Add(referenciaSetPruebas);

            var referenciaManifiesto = new Referencia();
            referenciaManifiesto.Numero = 2;
            referenciaManifiesto.TipoDocumento = TipoDTE.TipoReferencia.MIC;
            referenciaManifiesto.FolioReferencia = "asdasd47df";
            referenciaManifiesto.FechaDocumentoReferencia = DateTime.Now;
            referenciaManifiesto.RazonReferencia = "MANIFIESTO INTERNACIONAL";
            dte.Exportaciones.Referencias.Add(referenciaManifiesto);

            dte.Exportaciones.Encabezado.Totales.TipoMoneda = CodigosAduana.Moneda.DOLAR_ESTADOUNIDENSE;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoMoneda = CodigosAduana.Moneda.PESO_CHILENO;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoCambio = 681.07;

            handler.CalculateTotalesExportacion(dte);
            var path = handler.TimbrarYFirmarXMLDTEExportacion(dte, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente en " + path);
            #endregion

            #region NOTA CREDITO EXPORTACION
            var dteNC = handler.GenerateDTEExportacionBase(TipoDTE.DTEType.NotaCreditoExportacionElectronica, folioNC);
            dteNC.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion = dte.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion;
            dteNC.Exportaciones.Encabezado.Transporte.Aduana = dte.Exportaciones.Encabezado.Transporte.Aduana;
            dteNC.Exportaciones.Encabezado.Transporte.Aduana.MontoFlete = 0;
            dteNC.Exportaciones.Encabezado.Transporte.Aduana.MontoSeguro = 0;
            dteNC.Exportaciones.Encabezado.Transporte.Aduana.CantidadBultos = 0;

            dteNC.Exportaciones.Detalles = new List<DetalleExportacion>();
            var detalleNC = new DetalleExportacion();
            detalleNC.NumeroLinea = 1;
            detalleNC.IndicadorExento = IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalleNC.Nombre = detalle.Nombre;
            detalleNC.Cantidad = 49;
            detalleNC.UnidadMedida = detalle.UnidadMedida;
            detalleNC.Precio = detalle.Precio;
            detalleNC.MontoItem = (int)Math.Round(detalleNC.Cantidad * detalleNC.Precio, 0);
            dteNC.Exportaciones.Detalles.Add(detalleNC);

            dteNC.Exportaciones.Referencias = new List<Referencia>();
            referenciaSetPruebas = new Referencia();
            referenciaSetPruebas.Numero = 1;
            referenciaSetPruebas.TipoDocumento = TipoDTE.TipoReferencia.SetPruebas;
            referenciaSetPruebas.FolioReferencia = dteNC.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaSetPruebas.FechaDocumentoReferencia = DateTime.Now;
            referenciaSetPruebas.RazonReferencia = "CASO " + n_atencion + "-2";
            dteNC.Exportaciones.Referencias.Add(referenciaSetPruebas);

            var referenciaNC = new Referencia();
            referenciaNC.Numero = 2;
            referenciaNC.CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeMontos;
            referenciaNC.TipoDocumento = TipoDTE.TipoReferencia.FacturaExportacionElectronica;
            referenciaNC.FolioReferencia = dte.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaNC.FechaDocumentoReferencia = DateTime.Now;
            referenciaNC.RazonReferencia = "ANULACION DE FACTURA DE EXPORTACIÓN";
            dteNC.Exportaciones.Referencias.Add(referenciaNC);

            dteNC.Exportaciones.Encabezado.Totales.TipoMoneda = CodigosAduana.Moneda.DOLAR_ESTADOUNIDENSE;
            dteNC.Exportaciones.Encabezado.OtraMoneda.TipoMoneda = CodigosAduana.Moneda.PESO_CHILENO;
            dteNC.Exportaciones.Encabezado.OtraMoneda.TipoCambio = 681.07;
            handler.CalculateTotalesExportacion(dteNC);
            var pathNC = handler.TimbrarYFirmarXMLDTEExportacion(dteNC, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("NC generada exitosamente en " + pathNC);
            #endregion

            #region NOTA DEBITO EXPORTACION
            var dteND = handler.GenerateDTEExportacionBase(TipoDTE.DTEType.NotaDebitoExportacionElectronica, folioND);
            dteND.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion = dteNC.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion;
            dteND.Exportaciones.Encabezado.Transporte.Aduana = dteNC.Exportaciones.Encabezado.Transporte.Aduana;     

            dteND.Exportaciones.Detalles = new List<DetalleExportacion>();
            var detalleND = detalleNC;
            dteND.Exportaciones.Detalles.Add(detalleND);

            dteND.Exportaciones.Referencias = new List<Referencia>();
            referenciaSetPruebas = new Referencia();
            referenciaSetPruebas.Numero = 1;
            referenciaSetPruebas.TipoDocumento = TipoDTE.TipoReferencia.SetPruebas;
            referenciaSetPruebas.FolioReferencia = dteNC.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaSetPruebas.FechaDocumentoReferencia = DateTime.Now;
            referenciaSetPruebas.RazonReferencia = "CASO " + n_atencion + "-3";
            dteND.Exportaciones.Referencias.Add(referenciaSetPruebas);

            var referenciaND = new Referencia();
            referenciaND.Numero = 2;
            referenciaND.CodigoReferencia = TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia;
            referenciaND.TipoDocumento = TipoDTE.TipoReferencia.NotaCreditoExportacionElectronica;
            referenciaND.FolioReferencia = dteNC.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaND.FechaDocumentoReferencia = DateTime.Now;
            referenciaNC.RazonReferencia = "ANULACION NOTA DE CREDITO DE EXPORTACION";
            dteND.Exportaciones.Referencias.Add(referenciaND);

            dteND.Exportaciones.Encabezado.Totales.TipoMoneda = CodigosAduana.Moneda.DOLAR_ESTADOUNIDENSE;
            dteND.Exportaciones.Encabezado.OtraMoneda.TipoMoneda = CodigosAduana.Moneda.PESO_CHILENO;
            dteND.Exportaciones.Encabezado.OtraMoneda.TipoCambio = 681.07;
            handler.CalculateTotalesExportacion(dteND);
            var pathND = handler.TimbrarYFirmarXMLDTEExportacion(dteND, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("NC generada exitosamente en " + pathND);
            #endregion
            #endregion

        }

        private void BotonSetExportacion2_Click(object sender, EventArgs e)
        {
            var n_atencion = "1221633";
            int folioFactura = 58;
            #region SET EXPORTACION 1

            #region FACTURA EXPORTACION 1
            var dte = handler.GenerateDTEExportacionBase(TipoDTE.DTEType.FacturaExportacionElectronica, folioFactura);
            dte.Exportaciones.Encabezado.IdentificacionDTE.IndicadorServicio = IndicadorServicio.IndicadorServicioEnum.FacturaServicios2;
            dte.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion = CodigosAduana.FormaPagoExportacionEnum.ACRED;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoClausulaVenta = CodigosAduana.ClausulaCompraVenta.FOB;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoViaTransporte = CodigosAduana.ViasdeTransporte.AEREO;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPuertoEmbarque = CodigosAduana.Puertos.ARICA;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPuertoDesembarque = CodigosAduana.Puertos.BUENOS_AIRES;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPaisDestino = CodigosAduana.Paises.ARGENTINA;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPaisReceptor = CodigosAduana.Paises.ARGENTINA;


            dte.Exportaciones.Detalles = new List<DetalleExportacion>();
            var detalle = new DetalleExportacion();
            detalle.NumeroLinea = 1;
            detalle.IndicadorExento = IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalle.Nombre = "ASESORIAS Y PROYECTOS PROFESIONALES";
            detalle.Cantidad = 1;
            detalle.Precio = 19;
            detalle.Recargo = 2;
            detalle.RecargoPorcentaje = 10;
            detalle.MontoItem = 19 + 2; //26*1 + 10%            
            dte.Exportaciones.Detalles.Add(detalle);

            dte.Exportaciones.Referencias = new List<Referencia>();
            var referenciaSetPruebas = new Referencia();
            referenciaSetPruebas.Numero = 1;
            referenciaSetPruebas.TipoDocumento = TipoDTE.TipoReferencia.SetPruebas;
            referenciaSetPruebas.FolioReferencia = dte.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaSetPruebas.FechaDocumentoReferencia = DateTime.Now;
            referenciaSetPruebas.RazonReferencia = "CASO " + n_atencion + "-1";
            dte.Exportaciones.Referencias.Add(referenciaSetPruebas);

            var referenciaManifiesto = new Referencia();
            referenciaManifiesto.Numero = 2;
            referenciaManifiesto.TipoDocumento = TipoDTE.TipoReferencia.ResolucionSNA;
            referenciaManifiesto.FolioReferencia = "erere4f7d54";
            referenciaManifiesto.FechaDocumentoReferencia = DateTime.Now;
            referenciaManifiesto.RazonReferencia = "RESOLUCION SNA";
            dte.Exportaciones.Referencias.Add(referenciaManifiesto);

            dte.Exportaciones.Encabezado.Totales.TipoMoneda = CodigosAduana.Moneda.DOLAR_ESTADOUNIDENSE;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoMoneda = CodigosAduana.Moneda.PESO_CHILENO;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoCambio = 700;

            handler.CalculateTotalesExportacion(dte);
            var path = handler.TimbrarYFirmarXMLDTEExportacion(dte, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente en " + path);
            #endregion

            folioFactura++;
            #region FACTURA EXPORTACION 2
            dte = handler.GenerateDTEExportacionBase(TipoDTE.DTEType.FacturaExportacionElectronica, folioFactura);
            dte.Exportaciones.Encabezado.IdentificacionDTE.FormaPagoExportacion = CodigosAduana.FormaPagoExportacionEnum.ACRED;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoModalidadVenta = CodigosAduana.ModalidadVenta.A_FIRME;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoClausulaVenta = CodigosAduana.ClausulaCompraVenta.FOB;
            dte.Exportaciones.Encabezado.Transporte.Aduana.TotalClausulaVenta = 1138.3;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoViaTransporte = CodigosAduana.ViasdeTransporte.AEREO;

            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPuertoEmbarque = CodigosAduana.Puertos.ARICA;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPuertoDesembarque = CodigosAduana.Puertos.BUENOS_AIRES;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoUnidadMedidaTara = CodigosAduana.UnidadMedida.U;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoUnidadPesoBruto = CodigosAduana.UnidadMedida.U;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoUnidadPesoNeto = CodigosAduana.UnidadMedida.KN;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CantidadBultos = 29;
            dte.Exportaciones.Encabezado.Transporte.Aduana.TipoBultos = new List<TipoBulto>();
            dte.Exportaciones.Encabezado.Transporte.Aduana.TipoBultos.Add(new TipoBulto()
            {
                CantidadBultos = 29,
                CodigoTipoBulto = CodigosAduana.TipoBultoEnum.CONTENEDOR_REFRIGERADO,
                Marcas = "MARCA CHANCHO",
                IdContainer = "erer787df1",   
                Sello = "SelloTest2"
            });
            dte.Exportaciones.Encabezado.Transporte.Aduana.MontoFlete = 215.95;
            dte.Exportaciones.Encabezado.Transporte.Aduana.MontoSeguro = 40.97;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPaisDestino = CodigosAduana.Paises.ARGENTINA;
            dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoPaisReceptor = CodigosAduana.Paises.ARGENTINA;


            dte.Exportaciones.Detalles = new List<DetalleExportacion>();
            detalle = new DetalleExportacion();
            detalle.NumeroLinea = 1;
            detalle.IndicadorExento = IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalle.Nombre = "CAJAS CIRUELAS TIERNIZADAS SIN CAROZO CALIBRE 60/70";
            detalle.Cantidad = 290;
            detalle.UnidadMedida = "KN";
            detalle.Precio = 119;
            detalle.DescuentoPorcentaje = 5;
            var descuentoReal = detalle.Cantidad * detalle.Precio * 0.05;
            detalle.Descuento = (int)Math.Round(descuentoReal, 0, MidpointRounding.AwayFromZero);
            detalle.MontoItem = (int)Math.Round((detalle.Cantidad * detalle.Precio) - descuentoReal, 0, MidpointRounding.AwayFromZero);
            dte.Exportaciones.Detalles.Add(detalle);

            detalle = new DetalleExportacion();
            detalle.NumeroLinea = 2;
            detalle.IndicadorExento = IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalle.Nombre = "CAJAS DE PASAS DE UVA FLAME MORENA SIN SEMILLA MEDIANAS";
            detalle.Cantidad = 169;
            detalle.UnidadMedida = "KN";
            detalle.Precio = 67;
            detalle.MontoItem = (int)Math.Round(detalle.Cantidad * detalle.Precio, 0);
            dte.Exportaciones.Detalles.Add(detalle);

            dte.Exportaciones.DescuentosRecargos = new List<DescuentosRecargos>();

            var comisionExtranjero = new DescuentosRecargos();
            comisionExtranjero.Numero = 1;
            comisionExtranjero.TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Recargo;
            comisionExtranjero.Descripcion = "COMISIONES EN EL EXTRANJERO";
            comisionExtranjero.TipoValor = ExpresionDinero.ExpresionDineroEnum.Pesos;
            comisionExtranjero.IndicadorExento = IndicadorExento.IndicadorExentoEnum.Exento;
            comisionExtranjero.Valor = 125.21; 
            dte.Exportaciones.DescuentosRecargos.Add(comisionExtranjero);

            var descuentoFlete = new DescuentosRecargos();
            descuentoFlete.Numero = 2;
            descuentoFlete.TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Recargo;
            descuentoFlete.Descripcion = "Recargo flete";
            descuentoFlete.TipoValor = ExpresionDinero.ExpresionDineroEnum.Pesos;
            descuentoFlete.IndicadorExento = IndicadorExento.IndicadorExentoEnum.Exento;
            descuentoFlete.Valor = dte.Exportaciones.Encabezado.Transporte.Aduana.MontoFlete;
            dte.Exportaciones.DescuentosRecargos.Add(descuentoFlete);

            var descuentoSeguro = new DescuentosRecargos();
            descuentoSeguro.Numero = 3;
            descuentoSeguro.TipoMovimiento = TipoMovimiento.TipoMovimientoEnum.Recargo;
            descuentoSeguro.Descripcion = "Recargo seguro";
            descuentoSeguro.TipoValor = ExpresionDinero.ExpresionDineroEnum.Pesos;
            descuentoSeguro.IndicadorExento = IndicadorExento.IndicadorExentoEnum.Exento;
            descuentoSeguro.Valor = dte.Exportaciones.Encabezado.Transporte.Aduana.MontoSeguro;
            dte.Exportaciones.DescuentosRecargos.Add(descuentoSeguro);

            dte.Exportaciones.Referencias = new List<Referencia>();
            referenciaSetPruebas = new Referencia();
            referenciaSetPruebas.Numero = 1;
            referenciaSetPruebas.TipoDocumento = TipoDTE.TipoReferencia.SetPruebas;
            referenciaSetPruebas.FolioReferencia = dte.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaSetPruebas.FechaDocumentoReferencia = DateTime.Now;
            referenciaSetPruebas.RazonReferencia = "CASO " + n_atencion + "-2";
            dte.Exportaciones.Referencias.Add(referenciaSetPruebas);

            var referenciaDUS = new Referencia();
            referenciaDUS.Numero = 2;
            referenciaDUS.TipoDocumento = TipoDTE.TipoReferencia.DUS;
            referenciaDUS.FolioReferencia = "343fdf47";
            referenciaDUS.FechaDocumentoReferencia = DateTime.Now;
            referenciaDUS.RazonReferencia = "DUS";
            dte.Exportaciones.Referencias.Add(referenciaDUS);

            var referenciaAWB = new Referencia();
            referenciaAWB.Numero = 3;
            referenciaAWB.TipoDocumento = TipoDTE.TipoReferencia.AWB;
            referenciaAWB.FolioReferencia = "FDD7741E";
            referenciaAWB.FechaDocumentoReferencia = DateTime.Now;
            referenciaAWB.RazonReferencia = "AWB";
            dte.Exportaciones.Referencias.Add(referenciaAWB);

            dte.Exportaciones.Encabezado.Totales.TipoMoneda = CodigosAduana.Moneda.DOLAR_ESTADOUNIDENSE;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoMoneda = CodigosAduana.Moneda.PESO_CHILENO;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoCambio = 700;

            handler.CalculateTotalesExportacion(dte, comisionExtranjero.Valor);

            path = handler.TimbrarYFirmarXMLDTEExportacion(dte, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente en " + path);
            #endregion

            folioFactura++;
            #region FACTURA EXPORTACION 3
            dte = handler.GenerateDTEExportacionBase(TipoDTE.DTEType.FacturaExportacionElectronica, folioFactura);
            dte.Exportaciones.Encabezado.Receptor.Extranjero = new Extranjero() {
                Nacionalidad = CodigosAduana.Paises.ARGENTINA
            };
            dte.Exportaciones.Encabezado.IdentificacionDTE.IndicadorServicio = IndicadorServicio.IndicadorServicioEnum.ServiciosHoteleria2;
            //dato obligatorio
            //dte.Exportaciones.Encabezado.Transporte.Aduana.CodigoModalidadVenta = CodigosAduana.ModalidadVenta.A_FIRME;

            dte.Exportaciones.Detalles = new List<DetalleExportacion>();
            detalle = new DetalleExportacion();
            detalle.NumeroLinea = 1;
            detalle.IndicadorExento = IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalle.Nombre = "ALOJAMIENTO HABITACIONES";
            detalle.Cantidad = 1;
            detalle.Precio = 57;
            detalle.MontoItem = 57;          
            dte.Exportaciones.Detalles.Add(detalle);

            dte.Exportaciones.Referencias = new List<Referencia>();
            referenciaSetPruebas = new Referencia();
            referenciaSetPruebas.Numero = 1;
            referenciaSetPruebas.TipoDocumento = TipoDTE.TipoReferencia.SetPruebas;
            referenciaSetPruebas.FolioReferencia = dte.Exportaciones.Encabezado.IdentificacionDTE.Folio.ToString();
            referenciaSetPruebas.FechaDocumentoReferencia = DateTime.Now;
            referenciaSetPruebas.RazonReferencia = "CASO " + n_atencion + "-3";
            dte.Exportaciones.Referencias.Add(referenciaSetPruebas);

            referenciaAWB = new Referencia();
            referenciaAWB.Numero = 2;
            referenciaAWB.TipoDocumento = TipoDTE.TipoReferencia.AWB;
            referenciaAWB.FolioReferencia = "eer774df";
            referenciaAWB.FechaDocumentoReferencia = DateTime.Now;
            referenciaAWB.RazonReferencia = "AWB";
            dte.Exportaciones.Referencias.Add(referenciaAWB);

            dte.Exportaciones.Encabezado.Totales.TipoMoneda = CodigosAduana.Moneda.DOLAR_ESTADOUNIDENSE;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoMoneda = CodigosAduana.Moneda.PESO_CHILENO;
            dte.Exportaciones.Encabezado.OtraMoneda.TipoCambio = 700;

            handler.CalculateTotalesExportacion(dte);
            path = handler.TimbrarYFirmarXMLDTEExportacion(dte, "out\\temp\\", "out\\caf\\");

            handler.Validate(path, Firma.TipoXML.DTE, SimpleAPI.XML.Schemas.DTE);
            MessageBox.Show("Documento generado exitosamente en " + path);
            #endregion
            #endregion
        }

        private void botonLibroGuias_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName))
            {
                string xml = File.ReadAllText(openFileDialog1.FileName, Encoding.GetEncoding("ISO-8859-1"));
                var envio = SimpleAPI.XML.XmlHandler.TryDeserializeFromString<EnvioDTE>(xml);
                var libroGuias = handler.GenerateLibroGuias(envio);
                var filePathArchivo = libroGuias.Firmar(configuracion.Certificado.Nombre, "out\\temp\\");
                MessageBox.Show("Libro de Guías Generado correctamente en " + filePathArchivo);
            }
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
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.CorrigeMontos, TipoDTE.TipoReferencia.FacturaElectronica, DateTime.Now, 50);

            //Esta referencia indica que se está anulando la factura electrónica N° 50
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia, TipoDTE.TipoReferencia.FacturaElectronica, DateTime.Now, 50);

            //Esta referencia indica que se trata de un set de pruebas
            handler.Referencias(dte, TipoReferencia.TipoReferenciaEnum.SetPruebas, TipoDTE.TipoReferencia.NotSet, null, null, "CASO X");

            var path = await handler.TimbrarYFirmarXMLDTE(dte, "out\\temp\\", "out\\caf\\");

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
