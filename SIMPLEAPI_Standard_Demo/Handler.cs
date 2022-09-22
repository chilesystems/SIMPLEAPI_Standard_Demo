using SimpleAPI.Enum;
using SimpleAPI.Models.DTE;
using SimpleAPI.Models.Envios;
using SimpleAPI.Models.LCV;
using SimpleAPI.Models.RCOF;
using SimpleAPI.Models.ReciboMercaderia;
using SimpleAPI.Models.RespuestaEnvio;
using SimpleAPI.WS.Envio;
using SimpleAPI.WS.Estado;
using SIMPLEAPI_Demo.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SimpleAPI.Enum.Ambiente;

namespace SIMPLEAPI_Demo
{
    public class Handler
    {

        public Configuracion configuracion;

        public Handler()
        {
            this.configuracion = new Configuracion();
        }

        #region Generar Documento

        public DTE GenerateDTE(TipoDTE.DTEType tipoDTE, int folio, Emisor emisor, Receptor receptor)
        {
            // DOCUMENTO
            var dte = new DTE();
            dte.Documento = new Documento();
            //
            // DOCUMENTO - ENCABEZADO - CAMPO OBLIGATORIO               
            dte.Documento.Id = "DTE_" + DateTime.Now.Ticks.ToString();

            // DOCUMENTO - ENCABEZADO - IDENTIFICADOR DEL DOCUMENTO - CAMPOS OBLIGATORIOS
            dte.Documento.Encabezado.IdentificacionDTE.TipoDTE = tipoDTE;
            dte.Documento.Encabezado.IdentificacionDTE.FechaEmision = DateTime.Now;
            dte.Documento.Encabezado.IdentificacionDTE.Folio = folio;

            //DOCUMENTO - ENCABEZADO - EMISOR - CAMPOS OBLIGATORIOS          
            dte.Documento.Encabezado.Emisor = emisor;

            //Para boletas electrónicas
            if (tipoDTE == TipoDTE.DTEType.BoletaElectronica || tipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta)
            {
                dte.Documento.Encabezado.IdentificacionDTE.IndicadorServicio = IndicadorServicio.IndicadorServicioEnum.BoletaVentasYServicios;
            }
            //DOCUMENTO - ENCABEZADO - RECEPTOR - CAMPOS OBLIGATORIOS

            dte.Documento.Encabezado.Receptor = receptor;

            dte.Documento.Referencias = new List<Referencia>();

            return dte;
        }

        public DTE GenerateDTE(TipoDTE.DTEType tipoDTE, int folio, string idDTE = "")
        {
            // DOCUMENTO
            var dte = new DTE();
            dte.Documento = new Documento();
            //
            // DOCUMENTO - ENCABEZADO - CAMPO OBLIGATORIO
            //Id = puede ser compuesto según tus propios requerimientos pero debe ser único                  
            dte.Documento.Id = string.IsNullOrEmpty(idDTE) ? "DTE_" + DateTime.Now.Ticks.ToString() : idDTE;

            // DOCUMENTO - ENCABEZADO - IDENTIFICADOR DEL DOCUMENTO - CAMPOS OBLIGATORIOS
            dte.Documento.Encabezado.IdentificacionDTE.TipoDTE = tipoDTE;
            dte.Documento.Encabezado.IdentificacionDTE.FechaEmision = DateTime.Now;
            dte.Documento.Encabezado.IdentificacionDTE.Folio = folio;

            //DOCUMENTO - ENCABEZADO - EMISOR - CAMPOS OBLIGATORIOS          
            dte.Documento.Encabezado.Emisor.Rut = configuracion.Empresa.RutEmpresa;
            dte.Documento.Encabezado.Emisor.DireccionOrigen = configuracion.Empresa.Direccion;
            dte.Documento.Encabezado.Emisor.ComunaOrigen = configuracion.Empresa.Comuna;

            //Para boletas electrónicas
            if (tipoDTE == TipoDTE.DTEType.BoletaElectronica || tipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta)
            {
                dte.Documento.Encabezado.IdentificacionDTE.IndicadorServicio = IndicadorServicio.IndicadorServicioEnum.BoletaVentasYServicios;
                dte.Documento.Encabezado.Emisor.RazonSocialBoleta = configuracion.Empresa.RazonSocial;
                dte.Documento.Encabezado.Emisor.GiroBoleta = configuracion.Empresa.Giro;
            }
            else
            {
                dte.Documento.Encabezado.Emisor.ActividadEconomica = configuracion.Empresa.CodigosActividades.Select(x => x.Codigo).ToList();
                dte.Documento.Encabezado.Emisor.RazonSocial = configuracion.Empresa.RazonSocial;
                dte.Documento.Encabezado.Emisor.Giro = configuracion.Empresa.Giro;
            }

            if (tipoDTE == TipoDTE.DTEType.GuiaDespachoElectronica)
            {
                dte.Documento.Encabezado.IdentificacionDTE.TipoTraslado = TipoTraslado.TipoTrasladoEnum.OperacionConstituyeVenta;
                dte.Documento.Encabezado.IdentificacionDTE.TipoDespacho = TipoDespacho.TipoDespachoEnum.EmisorACliente;
            }
            //DOCUMENTO - ENCABEZADO - RECEPTOR - CAMPOS OBLIGATORIOS

            dte.Documento.Encabezado.Receptor.Rut = "66666666-6";
            dte.Documento.Encabezado.Receptor.RazonSocial = "Razon Social de Cliente";
            dte.Documento.Encabezado.Receptor.Direccion = "Dirección de cliente";
            dte.Documento.Encabezado.Receptor.Comuna = "Comuna de cliente";
            if (tipoDTE != TipoDTE.DTEType.BoletaElectronica && tipoDTE != TipoDTE.DTEType.BoletaElectronicaExenta)
            {
                dte.Documento.Encabezado.Receptor.Ciudad = "Ciudad de cliente";
                dte.Documento.Encabezado.Receptor.Giro = "Giro de cliente";
            }

            dte.Documento.Referencias = new List<Referencia>();

            return dte;
        }

        public void GenerateDetails(DTE dte)
        {
            //DOCUMENTO - DETALLES
            dte.Documento.Detalles = new List<SimpleAPI.Models.DTE.Detalle>();
            var detalle = new SimpleAPI.Models.DTE.Detalle();
            detalle.NumeroLinea = 1;
            /*IndicadorExento = Sólo aplica si el producto es exento de IVA*/
            //detalle.IndicadorExento = ChileSystems.DTE.Engine.Enum.IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;

            detalle.Nombre = "SERVICIO DE FACTURACION ELECT";
            detalle.Cantidad = 12;
            detalle.Precio = 170;
            /*Monto del item*/
            /*Recordar que debe restarse el descuento del detalle y sumarse el recargo*/
            detalle.MontoItem = 2040;
            dte.Documento.Detalles.Add(detalle);

            detalle = new SimpleAPI.Models.DTE.Detalle();
            detalle.NumeroLinea = 2;
            //detalle.IndicadorExento = ChileSystems.DTE.Engine.Enum.IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
            detalle.Nombre = "DESARROLLO DE ETL";
            detalle.Cantidad = 20;
            detalle.Precio = 1050;
            detalle.MontoItem = 21000;
            dte.Documento.Detalles.Add(detalle);

            //DOCUMENTO - ENCABEZADO - TOTALES - CAMPOS OBLIGATORIOS
            calculosTotales(dte);
        }

        public void GenerateDetails(DTE dte, List<ItemBoleta> detalles)
        {
            //DOCUMENTO - DETALLES
            dte.Documento.Detalles = new List<SimpleAPI.Models.DTE.Detalle>();

            int contador = 1;
            foreach (var det in detalles)
            {
                var detalle = new SimpleAPI.Models.DTE.Detalle();
                detalle.NumeroLinea = contador;
                /*IndicadorExento = Sólo aplica si el producto es exento de IVA*/
                detalle.IndicadorExento = det.Afecto ? IndicadorFacturacionExencionEnum.NotSet : IndicadorFacturacionExencionEnum.NoAfectoOExento;

                detalle.Nombre = det.Nombre;
                detalle.Cantidad = (double)det.Cantidad;
                detalle.Precio = det.Precio;
                if (!string.IsNullOrEmpty(det.UnidadMedida))
                {
                    detalle.UnidadMedida = det.UnidadMedida;
                }
                /*Monto del item*/
                /*Recordar que debe restarse el descuento del detalle y sumarse el recargo*/
                if (det.Descuento != 0)
                {
                    detalle.Descuento = (int)Math.Round(det.Total * (det.Descuento / 100), 0);
                    //detalle.DescuentoPorcentaje = det.Descuento;
                }
                detalle.MontoItem = det.Total - detalle.Descuento;

                if (det.TipoImpuesto != TipoImpuesto.TipoImpuestoEnum.NotSet)
                {
                    detalle.CodigoImpuestoAdicional = new List<TipoImpuesto.TipoImpuestoEnum>();
                    detalle.CodigoImpuestoAdicional.Add(det.TipoImpuesto);
                }

                dte.Documento.Detalles.Add(detalle);
                contador++;
            }
            calculosTotales(dte);
        }

        private void calculosTotales(DTE dte)
        {
            try
            {
                //DOCUMENTO - ENCABEZADO - TOTALES - CAMPOS OBLIGATORIOS
                if (dte.Documento.Encabezado.IdentificacionDTE.TipoDTE != TipoDTE.DTEType.BoletaElectronica
                    && dte.Documento.Encabezado.IdentificacionDTE.TipoDTE != TipoDTE.DTEType.BoletaElectronicaExenta)
                {
                    dte.Documento.Encabezado.Totales.TasaIVA = Convert.ToDouble(19);
                    var neto = dte.Documento.Detalles
                        .Where(x => x.IndicadorExento == IndicadorFacturacionExencionEnum.NotSet)
                        .Sum(x => x.MontoItem);

                    var exento = dte.Documento.Detalles
                        .Where(x => x.IndicadorExento == IndicadorFacturacionExencionEnum.NoAfectoOExento)
                        .Sum(x => x.MontoItem);

                    var descuentos = dte.Documento.DescuentosRecargos?
                        .Where(x => x.TipoMovimiento == TipoMovimiento.TipoMovimientoEnum.Descuento
                        && x.TipoValor == ExpresionDinero.ExpresionDineroEnum.Porcentaje)
                        .Sum(x => x.Valor);

                    if (descuentos.HasValue && descuentos.Value > 0)
                    {
                        var montoDescuentoAfecto = (int)Math.Round(neto * (descuentos.Value / 100), 0, MidpointRounding.AwayFromZero);
                        neto -= montoDescuentoAfecto;
                    }
                    var iva = (int)Math.Round(neto * 0.19, 0);
                    int retenido = 0;

                    if (dte.Documento.Detalles.Any(x => x.CodigoImpuestoAdicional != null))
                    {
                        retenido = (int)Math.Round(
                            dte.Documento.Detalles
                            .Where(x => x.CodigoImpuestoAdicional.First() == TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal)
                            .Sum(x => x.MontoItem) * 0.19, 0);

                        if (retenido != 0)
                        {
                            dte.Documento.Encabezado.Totales.ImpuestosRetenciones = new List<ImpuestosRetenciones>();
                            dte.Documento.Encabezado.Totales.ImpuestosRetenciones.Add(new ImpuestosRetenciones()
                            {
                                MontoImpuesto = retenido,
                                TasaImpuesto = 19,
                                TipoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
                            });
                        }
                    }

                    dte.Documento.Encabezado.Totales.MontoNeto = neto;
                    dte.Documento.Encabezado.Totales.MontoExento = exento;
                    dte.Documento.Encabezado.Totales.IVA = iva;
                    dte.Documento.Encabezado.Totales.MontoTotal = neto + exento + iva - retenido;
                }
                else
                {


                    /*En las boletas, sólo es necesario informar el monto total*/
                    if (dte.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronica)
                    {
                        var totalBrutoAfecto = dte.Documento.Detalles
                        .Where(x => x.IndicadorExento == IndicadorFacturacionExencionEnum.NotSet)
                        .Sum(x => x.MontoItem);

                        var totalExento = dte.Documento.Detalles
                            .Where(x => x.IndicadorExento == IndicadorFacturacionExencionEnum.NoAfectoOExento)
                            .Sum(x => x.MontoItem);

                        var neto = totalBrutoAfecto / 1.19;
                        var iva = (int)Math.Round(neto * 0.19, 0, MidpointRounding.AwayFromZero);
                        dte.Documento.Encabezado.Totales.IVA = iva;
                        dte.Documento.Encabezado.Totales.MontoNeto = (int)Math.Round(neto, 0, MidpointRounding.AwayFromZero);
                        dte.Documento.Encabezado.Totales.MontoTotal = dte.Documento.Encabezado.Totales.MontoNeto + totalExento + iva;
                    }
                    else //Boleta electrónica exenta
                    {
                        var total = dte.Documento.Detalles.Sum(x => x.MontoItem);
                        dte.Documento.Encabezado.Totales.MontoExento = dte.Documento.Encabezado.Totales.MontoTotal = total;
                    }

                }
            }

            catch { }
        }


        /// <summary>
        /// Permite agregar referencias a un DTE
        /// </summary>
        /// <param name="dte">Objeto DTE que tendrá una nueva Referencia</param>
        /// <param name="operacionReferencia">Corresponde a Anulación, Corrige Montos, Corrige Texto o SET de pruebas</param>
        /// <param name="tipoDocumentoReferencia">Tipo de documento que se desea referenciar, como notas de crédito, ordenes de compra, entre otros.</param>
        /// <param name="fechaDocReferencia">Fecha del documento de referencia. NO de cuándo se genera la referencia.</param>
        /// <param name="folioReferencia">Folio del documento de referencia.</param>
        /// <param name="casoPrueba">N° de caso de prueba</param>        
        public void Referencias(DTE dte, TipoReferencia.TipoReferenciaEnum operacionReferencia, TipoDTE.TipoReferencia tipoDocumentoReferencia, DateTime? fechaDocReferencia, int? folioReferencia = 0, string casoPrueba = "")
        {
            if (operacionReferencia == TipoReferencia.TipoReferenciaEnum.SetPruebas)  //REFERENCIA A SET DE PRUEBAS
            {
                if (tipoDocumentoReferencia == TipoDTE.TipoReferencia.BoletaElectronica || tipoDocumentoReferencia == TipoDTE.TipoReferencia.BoletaExentaElectronica)
                {
                    dte.Documento.Referencias.Add(new Referencia()
                    {
                        CodigoReferencia = TipoReferencia.TipoReferenciaEnum.SetPruebas,
                        Numero = dte.Documento.Referencias.Count + 1,
                        RazonReferencia = casoPrueba,
                    });
                }
                else
                {
                    dte.Documento.Referencias.Add(new Referencia()
                    {
                        FechaDocumentoReferencia = fechaDocReferencia.Value,
                        FolioReferencia = folioReferencia.ToString(),
                        Numero = dte.Documento.Referencias.Count + 1,
                        RazonReferencia = casoPrueba,
                        TipoDocumento = TipoDTE.TipoReferencia.SetPruebas
                    });
                }
            }
            else
            {
                dte.Documento.Referencias.Add(new Referencia()
                {
                    CodigoReferencia = operacionReferencia,
                    FechaDocumentoReferencia = fechaDocReferencia.Value,
                    FolioReferencia = folioReferencia.ToString(),
                    Numero = dte.Documento.Referencias.Count + 1,
                    RazonReferencia = operacionReferencia == TipoReferencia.TipoReferenciaEnum.AnulaDocumentoReferencia ? "ANULA" : "CORRIGE" + " DOCUMENTO N° " + folioReferencia.ToString(),
                    TipoDocumento = tipoDocumentoReferencia
                });
            }
        }

        public string TimbrarYFirmarXMLDTE(DTE dte, string pathResult, string pathCaf)
        {
            /*En primer lugar, el documento debe timbrarse con el CAF que descargas desde el SII, es simular
             * cuando antes debías ir con las facturas en papel para que te las timbraran */
            string messageOut = string.Empty;
            dte.Documento.Timbrar(
                EnsureExists((int)dte.Documento.Encabezado.IdentificacionDTE.TipoDTE, dte.Documento.Encabezado.IdentificacionDTE.Folio, pathCaf), out messageOut);

            if (!string.IsNullOrEmpty(messageOut)) return messageOut;
            /*El documento timbrado se guarda en la variable pathResult*/

            /*Finalmente, el documento timbrado debe firmarse con el certificado digital*/
            /*Se debe entregar en el argumento del método Firmar, el "FriendlyName" o Nombre descriptivo del certificado*/
            /*Retorna el filePath donde estará el archivo XML timbrado y firmado, listo para ser enviado al SII*/
            var resultado = dte.Firmar(configuracion.Certificado.Nombre);
            return resultado;
        }

        public bool Validate(string filePath, SimpleAPI.Security.Firma.TipoXML tipo, string schema)
        {
            string messageResult = string.Empty;
            if (SimpleAPI.XML.XmlHandler.ValidateWithSchema(filePath, out messageResult, schema))
                if (SimpleAPI.Security.Firma.VerificarFirmaFromPath(filePath, tipo, out string messageOutFirma))
                    return true;
                else
                    MessageBox.Show("Error al validar firma electrónica: " + messageResult + "", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            MessageBox.Show("Error: " + messageResult + ". Verifique que contiene la carpeta XML con los XSD para validación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private string EnsureExists(int tipoDTE, long folio, string pathCaf)
        {
            var cafFile = string.Empty;
            foreach (var file in System.IO.Directory.GetFiles(pathCaf))
                if (ParseName((new FileInfo(file)).Name, tipoDTE, folio))
                    cafFile = file;
            if (string.IsNullOrEmpty(cafFile))
                throw new Exception("NO HAY UN CÓDIGO DE AUTORIZACIÓN DE FOLIOS (CAF) ASIGNADO PARA ESTE TIPO DE DOCUMENTO (" + tipoDTE + ") QUE INCLUYA EL FOLIO REQUERIDO (" + folio + ").");
            return cafFile;
        }

        private static bool ParseName(string name, int tipoDTE, long folio)
        {
            try
            {
                var values = name.Substring(0, name.IndexOf('.')).Split('_');
                int tipo = Convert.ToInt32(values[0]);
                int desde = Convert.ToInt32(values[1]);
                int hasta = Convert.ToInt32(values[2]);
                return tipoDTE == tipo && desde <= folio && folio <= hasta;
            }
            catch { return false; }
        }

        #endregion

        #region Envio

        public SimpleAPI.Models.Envios.EnvioDTE GenerarEnvioDTEToSII(List<DTE> dtes, List<string> xmlDtes)
        {
            var EnvioSII = new SimpleAPI.Models.Envios.EnvioDTE();
            EnvioSII.SetDTE = new SetDTE();
            EnvioSII.SetDTE.Id = "FENV010";
            /*Es necesario agregar en el envío, los objetos DTE como sus respectivos XML en strings*/
            foreach (var a in dtes)
                EnvioSII.SetDTE.DTEs.Add(a);
            foreach (var a in xmlDtes)
            {
                EnvioSII.SetDTE.dteXmls.Add(a);
                EnvioSII.SetDTE.signedXmls.Add(a);
            }


            EnvioSII.SetDTE.Caratula = new SimpleAPI.Models.Envios.Caratula();
            EnvioSII.SetDTE.Caratula.FechaEnvio = DateTime.Now;
            /*Fecha de Resolución y Número de Resolución se averiguan en el sitio del SII según ambiente de producción o certificación*/
            EnvioSII.SetDTE.Caratula.FechaResolucion = configuracion.Empresa.FechaResolucion;
            EnvioSII.SetDTE.Caratula.NumeroResolucion = configuracion.Empresa.NumeroResolucion;

            EnvioSII.SetDTE.Caratula.RutEmisor = configuracion.Empresa.RutEmpresa;
            EnvioSII.SetDTE.Caratula.RutEnvia = configuracion.Certificado.Rut;
            EnvioSII.SetDTE.Caratula.RutReceptor = "60803000-K"; //Este es el RUT del SII
            EnvioSII.SetDTE.Caratula.SubTotalesDTE = new List<SubTotalesDTE>();

            /*En la carátula del envío, se debe indicar cuantos documentos de cada tipo se están enviando*/

            if (EnvioSII.SetDTE.DTEs.Any(x => !string.IsNullOrEmpty(x.Documento.Id)))
            {
                var tipos = EnvioSII.SetDTE.DTEs.GroupBy(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE);
                foreach (var a in tipos)
                {
                    EnvioSII.SetDTE.Caratula.SubTotalesDTE.Add(new SubTotalesDTE()
                    {
                        Cantidad = a.Count(),
                        TipoDTE = a.ElementAt(0).Documento.Encabezado.IdentificacionDTE.TipoDTE
                    });
                }
            }
            return EnvioSII;
        }
        public SimpleAPI.Models.Envios.EnvioDTE GenerarEnvioCliente(DTE dte, string dteXML)
        {
            var EnvioCustomer = new SimpleAPI.Models.Envios.EnvioDTE();
            EnvioCustomer.SetDTE = new SetDTE();
            EnvioCustomer.SetDTE.DTEs.Add(dte);
            EnvioCustomer.SetDTE.dteXmls.Add(dteXML);
            EnvioCustomer.SetDTE.Caratula = new SimpleAPI.Models.Envios.Caratula();
            EnvioCustomer.SetDTE.Caratula.FechaEnvio = DateTime.Now;
            /*Fecha de Resolución y Número de Resolución se averiguan en el sitio del SII según ambiente de producción o certificación*/
            EnvioCustomer.SetDTE.Caratula.FechaResolucion = configuracion.Empresa.FechaResolucion;
            EnvioCustomer.SetDTE.Caratula.NumeroResolucion = configuracion.Empresa.NumeroResolucion;

            EnvioCustomer.SetDTE.Caratula.RutEmisor = configuracion.Empresa.RutEmpresa;
            EnvioCustomer.SetDTE.Caratula.RutEnvia = configuracion.Certificado.Rut;
            EnvioCustomer.SetDTE.Caratula.RutReceptor = dte.Documento.Encabezado.Receptor.Rut;
            /*Generalmente al cliente se le envía una sola factura, sin embargo si no es el caso, 
             se pueden agregar varias tal cual como está el método GenerarEnvioDTEToSII()*/
            EnvioCustomer.SetDTE.Caratula.SubTotalesDTE = new List<SubTotalesDTE>()
            {
                new SubTotalesDTE()
                {
                    Cantidad = 1,
                    TipoDTE = dte.Documento.Encabezado.IdentificacionDTE.TipoDTE
                }
            };

            return EnvioCustomer;
        }

        public async Task<(long, string)> EnviarEnvioDTEToSIIAsync(string filePathEnvio, AmbienteEnum ambiente, bool nuevaBoleta = false)
        {
            try
            {
                EnvioDTEResult responseEnvio = new EnvioDTEResult();

                if (nuevaBoleta) responseEnvio = await SimpleAPI.WS.Envio.EnvioBoleta.EnviarAsync(configuracion.Certificado.Rut, configuracion.Empresa.RutEmpresa, filePathEnvio, configuracion.Certificado.Nombre, ambiente, ".\\out\\tkn.dat");
                else responseEnvio = await SimpleAPI.WS.Envio.EnvioDTE.EnviarAsync(configuracion.Certificado.Rut, configuracion.Empresa.RutEmpresa, filePathEnvio, configuracion.Certificado.Nombre, ambiente, ".\\out\\tkn.dat", "");

                if (responseEnvio != null && responseEnvio.TrackId > 0)
                {
                    long trackID = responseEnvio.TrackId;
                    /*Aquí pueden obtener todos los datos de la respuesta, tal como:
                     * Estado
                     * Fecha
                     * Archivo
                     * Glosa
                     * XML
                     * Entre otros*/
                    return (trackID, string.Empty);
                }
                else
                {
                    return (0, responseEnvio.ResponseXml);
                }
            }
            catch (Exception ex)
            {
                return (0, ex.Message);
            }
        }


        #endregion


        #region Boletas Electrónicas

        public SimpleAPI.Models.Envios.EnvioBoleta GenerarEnvioBoletaDTEToSII(List<DTE> dtes, List<string> xmlDtes)
        {
            var EnvioSII = new SimpleAPI.Models.Envios.EnvioBoleta();
            EnvioSII.SetDTE = new SetDTE();
            EnvioSII.SetDTE.Id = "FENV010";
            /*Es necesario agregar en el envío, los objetos DTE como sus respectivos XML en strings*/
            foreach (var a in dtes)
                EnvioSII.SetDTE.DTEs.Add(a);
            foreach (var a in xmlDtes)
            {
                EnvioSII.SetDTE.dteXmls.Add(a);
                EnvioSII.SetDTE.signedXmls.Add(a);
            }

            EnvioSII.SetDTE.Caratula = new SimpleAPI.Models.Envios.Caratula();
            EnvioSII.SetDTE.Caratula.FechaEnvio = DateTime.Now;
            /*Fecha de Resolución y Número de Resolución se averiguan en el sitio del SII según ambiente de producción o certificación*/
            EnvioSII.SetDTE.Caratula.FechaResolucion = configuracion.Empresa.FechaResolucion;
            EnvioSII.SetDTE.Caratula.NumeroResolucion = configuracion.Empresa.NumeroResolucion;

            EnvioSII.SetDTE.Caratula.RutEmisor = configuracion.Empresa.RutEmpresa;
            EnvioSII.SetDTE.Caratula.RutEnvia = configuracion.Certificado.Rut;
            EnvioSII.SetDTE.Caratula.RutReceptor = "60803000-K"; //Este es el RUT del SII
            EnvioSII.SetDTE.Caratula.SubTotalesDTE = new List<SubTotalesDTE>();

            /*En la carátula del envío, se debe indicar cuantos documentos de cada tipo se están enviando*/
            var tipos = EnvioSII.SetDTE.DTEs.GroupBy(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE);
            foreach (var a in tipos)
            {
                EnvioSII.SetDTE.Caratula.SubTotalesDTE.Add(new SubTotalesDTE()
                {
                    Cantidad = a.Count(),
                    TipoDTE = a.ElementAt(0).Documento.Encabezado.IdentificacionDTE.TipoDTE
                });
            }
            return EnvioSII;
        }

        public ConsumoFolios GenerarRCOF(List<DTE> dtes)
        {
            var rcof = new ConsumoFolios();
            //preparo los datos segun los DTE seleccionados
            DateTime fechaInicio = dtes.Min(x => x.Documento.Encabezado.IdentificacionDTE.FechaEmision);
            DateTime fechaFinal = dtes.Max(x => x.Documento.Encabezado.IdentificacionDTE.FechaEmision);

            rcof.DocumentoConsumoFolios.Caratula.FechaFinal = fechaInicio;
            rcof.DocumentoConsumoFolios.Caratula.FechaInicio = fechaFinal;
            rcof.DocumentoConsumoFolios.Caratula.FechaResolucion = configuracion.Empresa.FechaResolucion;
            rcof.DocumentoConsumoFolios.Caratula.NroResol = configuracion.Empresa.NumeroResolucion;
            rcof.DocumentoConsumoFolios.Caratula.RutEmisor = configuracion.Empresa.RutEmpresa;
            rcof.DocumentoConsumoFolios.Caratula.RutEnvia = configuracion.Certificado.Rut;
            rcof.DocumentoConsumoFolios.Caratula.SecEnvio = "1";
            rcof.DocumentoConsumoFolios.Caratula.FechaEnvio = DateTime.Now;
            List<Resumen> resumenes = new List<Resumen>();

            /*datos de boletas electrónicas afectas*/
            /* Estos datos se deben calcular, debido a que no se informa IVA en boletas electrónicas 
             */
            int totalBrutoAfecto = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronica)
                        .Sum(x => x.Documento.Detalles
                        .Where(y => y.IndicadorExento == IndicadorFacturacionExencionEnum.NotSet)
                        .Sum(y => y.MontoItem));

            int totalExento = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronica)
                    .Sum(x => x.Documento.Detalles
                    .Where(y => y.IndicadorExento == IndicadorFacturacionExencionEnum.NoAfectoOExento)
                    .Sum(y => y.MontoItem));

            int totalNeto = (int)Math.Round(totalBrutoAfecto / 1.19, 0, MidpointRounding.AwayFromZero);
            int totalIVA = (int)Math.Round(totalNeto * 0.19, 0, MidpointRounding.AwayFromZero);
            int totalTotal = totalExento + totalNeto + totalIVA;

            /*Se calculan todos los rangos según el array de DTEs*/
            var resultRangos = new List<RangoUtilizados>();
            List<long> lst = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronica).Select(x => x.Documento.Encabezado.IdentificacionDTE.Folio).ToList();
            var minBoundaries = lst.Where(i => !lst.Contains(i - 1)).OrderBy(x => x).ToList();
            var maxBoundaries = lst.Where(i => !lst.Contains(i + 1)).OrderBy(x => x).ToList();
            for (int i = 0; i < maxBoundaries.Count; i++)
            {
                resultRangos.Add(new RangoUtilizados() { Inicial = minBoundaries[i], Final = maxBoundaries[i] });
            }

            resumenes.Add(new Resumen
            {
                FoliosAnulados = 0,
                FoliosEmitidos = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronica).Count(),
                FoliosUtilizados = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronica).Count(),
                MntExento = totalExento,
                MntIva = totalIVA,
                MntNeto = totalNeto,
                MntTotal = totalTotal,
                TasaIVA = 19,
                TipoDocumento = TipoDTE.DTEType.BoletaElectronica,
                RangoUtilizados = resultRangos
                //RangoAnulados = new List<ChileSystems.DTE.Engine.RCOF.RangoAnulados>() { new ChileSystems.DTE.Engine.RCOF.RangoAnulados() { Final = 0, Inicial = 0 } }
            });


            /*datos de boletas electrónicas exentas*/
            if (dtes.Any(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta))
            {
                totalExento = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta).Sum(x => x.Documento.Encabezado.Totales.MontoExento);
                resultRangos = new List<RangoUtilizados>();
                lst = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta).Select(x => x.Documento.Encabezado.IdentificacionDTE.Folio).ToList();
                minBoundaries = lst.Where(i => !lst.Contains(i - 1)).OrderBy(x => x).ToList();
                maxBoundaries = lst.Where(i => !lst.Contains(i + 1)).OrderBy(x => x).ToList();
                for (int i = 0; i < maxBoundaries.Count; i++)
                {
                    resultRangos.Add(new RangoUtilizados() { Inicial = minBoundaries[i], Final = maxBoundaries[i] });
                }

                resumenes.Add(new Resumen
                {
                    FoliosAnulados = 0,
                    FoliosEmitidos = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta).Count(),
                    FoliosUtilizados = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta).Count(),
                    MntExento = totalExento,
                    MntTotal = totalExento,
                    TipoDocumento = TipoDTE.DTEType.BoletaElectronicaExenta,
                    RangoUtilizados = resultRangos
                });
            }

            /*datos de notas de credito electronicas*/
            if (dtes.Any(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica))
            {
                totalNeto = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Sum(x => x.Documento.Encabezado.Totales.MontoNeto);
                totalExento = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Sum(x => x.Documento.Encabezado.Totales.MontoExento);
                totalIVA = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Sum(x => x.Documento.Encabezado.Totales.IVA);
                totalTotal = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Sum(x => x.Documento.Encabezado.Totales.MontoTotal);

                resultRangos = new List<RangoUtilizados>();
                lst = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Select(x => x.Documento.Encabezado.IdentificacionDTE.Folio).ToList();
                minBoundaries = lst.Where(i => !lst.Contains(i - 1)).OrderBy(x => x).ToList();
                maxBoundaries = lst.Where(i => !lst.Contains(i + 1)).OrderBy(x => x).ToList();
                for (int i = 0; i < maxBoundaries.Count; i++)
                {
                    resultRangos.Add(new RangoUtilizados() { Inicial = minBoundaries[i], Final = maxBoundaries[i] });
                }

                resumenes.Add(new Resumen
                {
                    FoliosAnulados = 0,
                    FoliosEmitidos = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Count(),
                    FoliosUtilizados = dtes.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica).Count(),
                    MntExento = totalExento,
                    MntIva = totalIVA,
                    MntNeto = totalNeto,
                    MntTotal = totalTotal,
                    TasaIVA = 19,
                    TipoDocumento = TipoDTE.DTEType.NotaCreditoElectronica,
                    RangoUtilizados = resultRangos
                    //RangoAnulados =new List<ChileSystems.DTE.Engine.RCOF.RangoAnulados>() { new ChileSystems.DTE.Engine.RCOF.RangoAnulados() { Final = 0, Inicial = 0 } }
                });
            }

            rcof.DocumentoConsumoFolios.Resumen = resumenes;
            return rcof;
        }

        public ConsumoFolios GenerarRCOFVacio(DateTime fecha)
        {
            var rcof = new ConsumoFolios();
            DateTime fechaInicio = fecha;
            DateTime fechaFinal = fecha;

            rcof.DocumentoConsumoFolios.Caratula.FechaFinal = fechaInicio;
            rcof.DocumentoConsumoFolios.Caratula.FechaInicio = fechaFinal;
            rcof.DocumentoConsumoFolios.Caratula.FechaResolucion = configuracion.Empresa.FechaResolucion;
            rcof.DocumentoConsumoFolios.Caratula.NroResol = configuracion.Empresa.NumeroResolucion;
            rcof.DocumentoConsumoFolios.Caratula.RutEmisor = configuracion.Empresa.RutEmpresa;
            rcof.DocumentoConsumoFolios.Caratula.RutEnvia = configuracion.Certificado.Rut;
            rcof.DocumentoConsumoFolios.Caratula.SecEnvio = "1";
            rcof.DocumentoConsumoFolios.Caratula.FechaEnvio = DateTime.Now;
            List<Resumen> resumenes = new List<Resumen>();

            /*datos de boletas electrónicas afectas*/

            resumenes.Add(new Resumen
            {
                FoliosAnulados = 0,
                FoliosEmitidos = 0,
                FoliosUtilizados = 0,
                MntExento = 0,
                MntIva = 0,
                MntNeto = 0,
                MntTotal = 0,
                TasaIVA = 19,
                TipoDocumento = TipoDTE.DTEType.BoletaElectronica
            });

            rcof.DocumentoConsumoFolios.Resumen = resumenes;
            return rcof;
        }

        #endregion

        #region IECV

        public LibroCompraVenta GenerateLibroVentas(SimpleAPI.Models.Envios.EnvioDTE envioAux)
        {
            var libro = new LibroCompraVenta();
            libro.EnvioLibro = new EnvioLibro();
            libro.EnvioLibro.Id = "ID_LIBRO1_1";

            /*Si el libro tiene código de autorización para rectificación, se debe ingresar en la carátula
             * del EnvioLibro. Esto es: libro.EnvioLibro.Caratula.CodigoAutorizacionRectificacionLibro*/

            libro.EnvioLibro.Caratula = new SimpleAPI.Models.LCV.Caratula()
            {
                RutEmisor = configuracion.Empresa.RutEmpresa,
                RutEnvia = configuracion.Certificado.Rut,
                PeriodoTributario = $"{DateTime.Now.Year}-{(DateTime.Now.Month >= 10 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month)}",
                FechaResolucion = configuracion.Empresa.FechaResolucion,
                NumeroResolucion = configuracion.Empresa.NumeroResolucion,
                TipoOperacion = TipoOperacionLibro.TipoOperacionLibroEnum.Venta,
                TipoLibro = TipoLibro.TipoLibroEnum.Especial,
                TipoEnvio = TipoEnvioLibro.TipoEnvioLibroEnum.Total,
                FolioNotificacion = 100,
                //Para cuando es SET de pruebas, siempre es 1,,                

            };

            libro.EnvioLibro.Caratula.TipoOperacion = TipoOperacionLibro.TipoOperacionLibroEnum.Venta;
            libro.EnvioLibro.Caratula.TipoLibro = TipoLibro.TipoLibroEnum.Especial;
            libro.EnvioLibro.Caratula.TipoEnvio = TipoEnvioLibro.TipoEnvioLibroEnum.Total;

            libro.EnvioLibro.ResumenPeriodo = new ResumenPeriodo();
            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo = new List<TotalPeriodo>();

            /**************TOTALES PARA LAS FACTURAS******************/
            int cantidadDocumentosFacturas = envioAux.SetDTE.DTEs.
                Count(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.FacturaElectronica);

            int totalExento = envioAux.SetDTE.DTEs.
                Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.FacturaElectronica)
                .Sum(x => x.Documento.Encabezado.Totales.MontoExento);

            int totalNeto = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.FacturaElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.MontoNeto);

            int totalIVA = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.FacturaElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.IVA);

            int totalTotal = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.FacturaElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.MontoTotal);

            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.FacturaElectronica,
                CantidadDocumentos = cantidadDocumentosFacturas,
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = totalExento,
                TotalMontoNeto = totalNeto,
                TotalMontoIva = totalIVA,
                TotalMonto = totalTotal
            });
            /**************************************************/

            /***************TOTALES PARA LAS NC*****************/
            int cantidadDocumentosNC = envioAux.SetDTE.DTEs.
                Count(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica);

            int totalExentoNC = envioAux.SetDTE.DTEs.
                Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica)
                .Sum(x => x.Documento.Encabezado.Totales.MontoExento);

            int totalNetoNC = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.MontoNeto);

            int totalIVANC = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.IVA);

            int totalTotalNC = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.MontoTotal);

            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.NotaCreditoElectronica,
                CantidadDocumentos = cantidadDocumentosNC,
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = totalExentoNC,
                TotalMontoNeto = totalNetoNC,
                TotalMontoIva = totalIVANC,
                TotalMonto = totalTotalNC
            });
            /**************************************************/

            /********************TOTALES PARA ND***************/
            int cantidadDocumentosND = envioAux.SetDTE.DTEs.
                Count(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaDebitoElectronica);

            int totalExentoND = envioAux.SetDTE.DTEs.
                Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaDebitoElectronica)
                .Sum(x => x.Documento.Encabezado.Totales.MontoExento);

            int totalNetoND = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaDebitoElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.MontoNeto);

            int totalIVAND = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaDebitoElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.IVA);

            int totalTotalND = envioAux.SetDTE.DTEs.
               Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaDebitoElectronica)
               .Sum(x => x.Documento.Encabezado.Totales.MontoTotal);

            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.NotaDebitoElectronica,
                CantidadDocumentos = cantidadDocumentosND,
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = totalExentoND,
                TotalMontoNeto = totalNetoND,
                TotalMontoIva = totalIVAND,
                TotalMonto = totalTotalND
            });
            /**************************************************/
            libro.EnvioLibro.Detalles = new List<SimpleAPI.Models.LCV.Detalle>();
            foreach (var dteAux in envioAux.SetDTE.DTEs)
            {
                TipoDTE.DTEType tipoDocumento = TipoDTE.DTEType.NotSet;

                if (dteAux.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.FacturaElectronica)
                    tipoDocumento = TipoDTE.DTEType.FacturaElectronica;

                else if (dteAux.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaCreditoElectronica)
                    tipoDocumento = TipoDTE.DTEType.NotaCreditoElectronica;

                else if (dteAux.Documento.Encabezado.IdentificacionDTE.TipoDTE == TipoDTE.DTEType.NotaDebitoElectronica)
                    tipoDocumento = TipoDTE.DTEType.NotaDebitoElectronica;

                libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
                {
                    TipoDocumento = tipoDocumento,
                    NumeroDocumento = dteAux.Documento.Encabezado.IdentificacionDTE.Folio,
                    IndicadorAnulado = IndicadorAnulado.IndicadorAnuladoEnum.NotSet,
                    TasaImpuestoOperacion = 0.19,
                    FechaDocumento = dteAux.Documento.Encabezado.IdentificacionDTE.FechaEmision,
                    RutDocumento = dteAux.Documento.Encabezado.Receptor.Rut,
                    RazonSocial = dteAux.Documento.Encabezado.Receptor.RazonSocial,
                    MontoExento = dteAux.Documento.Encabezado.Totales.MontoExento,
                    MontoNeto = dteAux.Documento.Encabezado.Totales.MontoNeto,
                    MontoIva = dteAux.Documento.Encabezado.Totales.IVA,
                    MontoTotal = dteAux.Documento.Encabezado.Totales.MontoTotal
                });
            }

            return libro;
        }

        public LibroCompraVenta GenerateLibroCompras()
        {
            var libro = new LibroCompraVenta();
            libro.EnvioLibro = new EnvioLibro();
            libro.EnvioLibro.Id = "ID_LIBRO2";

            /*Si el libro tiene código de autorización para rectificación, se debe ingresar en la carátula
             * del EnvioLibro. Esto es: libro.EnvioLibro.Caratula.CodigoAutorizacionRectificacionLibro*/

            libro.EnvioLibro.Caratula = new SimpleAPI.Models.LCV.Caratula()
            {
                RutEmisor = configuracion.Empresa.RutEmpresa,
                RutEnvia = configuracion.Certificado.Rut,
                PeriodoTributario = $"{DateTime.Now.Year}-{(DateTime.Now.Month >= 10 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month)}",
                FechaResolucion = configuracion.Empresa.FechaResolucion,
                NumeroResolucion = configuracion.Empresa.NumeroResolucion,
                TipoOperacion = TipoOperacionLibro.TipoOperacionLibroEnum.Compra,
                TipoLibro = TipoLibro.TipoLibroEnum.Especial,
                TipoEnvio = TipoEnvioLibro.TipoEnvioLibroEnum.Total,
                FolioNotificacion = 100, //Para cuando es SET de pruebas, siempre es 1
                //CodigoAutorizacionRectificacionLibro = "1NLKZLFX4S"

            };

            libro.EnvioLibro.Detalles = new List<SimpleAPI.Models.LCV.Detalle>();

            int neto = 60906;
            int exento = 0;
            int iva = (int)Math.Round(neto * 0.19, 0);
            int total = neto + iva + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.Factura,
                NumeroDocumento = 234,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = 0,
                MontoNeto = neto,
                MontoIva = iva,
                MontoTotal = total
            });
            neto = 12658;
            exento = 11061;
            iva = (int)Math.Round(neto * 0.19, 0);
            total = neto + iva + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.FacturaElectronica,
                NumeroDocumento = 32,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = exento,
                MontoNeto = neto,
                MontoIva = iva,
                MontoTotal = total,
                IVANoRecuperable = new List<TotalIVANoRecuperableDetalle>()
            });
            neto = 30263;
            exento = 0;
            iva = (int)Math.Round(neto * 0.19, 0);
            total = neto + iva + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.Factura,
                NumeroDocumento = 781,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = exento,
                MontoNeto = neto,
                //MontoIva = 5681,
                IVAUsoComun = iva, //Neto * 0.19 
                MontoTotal = total,
                TipoImpuesto = TipoImpuesto.TipoImpuestoResumido.Iva

            });

            neto = 2978;
            exento = 0;
            iva = (int)Math.Round(neto * 0.19, 0);
            total = neto + iva + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.NotaCredito,
                NumeroDocumento = 451,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = exento,
                MontoNeto = neto,
                MontoIva = iva,
                MontoTotal = total,
                TipoDocumentoReferencia = TipoDTE.TipoDocumentoLibro.FacturaManual,
                FolioDocumentoReferencia = 234
            });

            neto = 12640;
            exento = 0;
            iva = (int)Math.Round(neto * 0.19, 0);
            total = neto + iva + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.FacturaElectronica,
                NumeroDocumento = 67,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = exento,
                MontoNeto = neto,
                MontoTotal = total,
                IVANoRecuperable = new List<TotalIVANoRecuperableDetalle>()
                {
                    new TotalIVANoRecuperableDetalle()
                    {
                        CodigoIVANoRecuperable = CodigoIVANoRecuperable.CodigoIVANoRecuperableEnum.EntregaGratuita,
                        TotalMontoIVANoRecuperable = iva
                    }
                }
            });

            neto = 10885;
            exento = 0;
            iva = (int)Math.Round(neto * 0.19, 0);
            total = neto + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.FacturaCompraElectronica,
                NumeroDocumento = 9,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = exento,
                MontoNeto = neto,
                MontoIva = iva,
                MontoTotal = total,
                IVARetenidoTotal = iva,
                Impuestos = new List<ImpuestosDetalle>()
                {
                    new ImpuestosDetalle()
                    {
                        CodigoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal,
                        TotalMontoImpuesto = iva,
                        TasaImpuesto = 19
                    }
                }
            });

            neto = 10152;
            exento = 0;
            iva = (int)Math.Round(neto * 0.19, 0);
            total = neto + iva + exento;
            libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
            {
                TipoDocumento = TipoDTE.DTEType.NotaCredito,
                NumeroDocumento = 211,
                TasaImpuestoOperacion = 19,
                FechaDocumento = DateTime.Now,
                RutDocumento = "17096073-4",
                RazonSocial = "Razón Social",
                MontoExento = exento,
                MontoNeto = neto,
                MontoIva = iva,
                MontoTotal = total,
                TipoDocumentoReferencia = TipoDTE.TipoDocumentoLibro.FacturaElectronica,
                FolioDocumentoReferencia = 32
            });




            libro.EnvioLibro.ResumenPeriodo = new ResumenPeriodo();
            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo = new List<TotalPeriodo>();

            var manuales = libro.EnvioLibro.Detalles.Where(x => x.TipoDocumento == TipoDTE.DTEType.Factura);
            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.FacturaManual,
                CantidadDocumentos = manuales.Count(),
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = manuales.Sum(x => x.MontoExento),
                TotalMontoNeto = manuales.Sum(x => x.MontoNeto),
                TotalMontoIva = manuales.Sum(x => x.MontoIva),
                TotalIVAUsoComun = manuales.Sum(x => x.IVAUsoComun),
                TotalMonto = manuales.Sum(x => x.MontoTotal),
                FactorProporcionalidadIVA = 0.6,
                TotalCreditoIVAUsoComun = (int)Math.Round(manuales.Sum(x => x.IVAUsoComun) * 0.6, 0, MidpointRounding.AwayFromZero),
                CantidadOperacionesConIvaUsoComun = 1
            });

            var electronicas = libro.EnvioLibro.Detalles.Where(x => x.TipoDocumento == TipoDTE.DTEType.FacturaElectronica);
            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.FacturaElectronica,
                CantidadDocumentos = electronicas.Count(),
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = electronicas.Sum(x => x.MontoExento),
                TotalMontoNeto = electronicas.Sum(x => x.MontoNeto),
                TotalMontoIva = electronicas.Sum(x => x.MontoIva),
                TotalIVAUsoComun = electronicas.Sum(x => x.IVAUsoComun),
                TotalMonto = electronicas.Sum(x => x.MontoTotal),
                TotalIVANoRecuperable = new List<TotalIVANoRecuperable>()
                {
                    new TotalIVANoRecuperable()
                    {
                        CantidadOperacionesIVANoRecuperable = 1,
                        CodigoIVANoRecuperable = CodigoIVANoRecuperable.CodigoIVANoRecuperableEnum.EntregaGratuita,
                        TotalMontoIVANoRecuperable = electronicas.Sum(x=>x.IVANoRecuperable.Sum(y=>y.TotalMontoIVANoRecuperable))
                    }
                }
            });

            var nc = libro.EnvioLibro.Detalles.Where(x => x.TipoDocumento == TipoDTE.DTEType.NotaCredito);
            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.NotaCredito,
                CantidadDocumentos = nc.Count(),
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = nc.Sum(x => x.MontoExento),
                TotalMontoNeto = nc.Sum(x => x.MontoNeto),
                TotalMontoIva = nc.Sum(x => x.MontoIva),
                TotalMonto = nc.Sum(x => x.MontoTotal),
            });

            var fce = libro.EnvioLibro.Detalles.Where(x => x.TipoDocumento == TipoDTE.DTEType.FacturaCompraElectronica);
            libro.EnvioLibro.ResumenPeriodo.TotalesPeriodo.Add(new TotalPeriodo()
            {
                TipoDocumento = TipoDTE.TipoDocumentoLibro.FacturaCompraElectronica,
                CantidadDocumentos = fce.Count(),
                CantidadDocumentosAnulados = 0,
                TotalMontoExento = fce.Sum(x => x.MontoExento),
                TotalMontoNeto = fce.Sum(x => x.MontoNeto),
                TotalMontoIva = fce.Sum(x => x.MontoIva),
                TotalMonto = fce.Sum(x => x.MontoTotal),
                TotalIVARetenidoTotal = fce.Sum(x => x.IVARetenidoTotal),
                Impuestos = new List<ImpuestosPeriodo>()
                {
                    new ImpuestosPeriodo()
                    {
                        CodigoImpuesto = TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal,
                        TotalMontoImpuesto = fce.Sum(x=>x.IVARetenidoTotal)
                    }
                }
            });
            /**************************************************/

            /**************************************************/

            return libro;
        }

        #endregion

        #region Guias de despacho

        public LibroGuia GenerateLibroGuias(SimpleAPI.Models.Envios.EnvioDTE envioAux)
        {
            var libro = new LibroGuia();
            libro.EnvioLibro = new EnvioLibro();
            libro.EnvioLibro.Id = "ID_LIBRO_GUIAS_";


            libro.EnvioLibro.Caratula = new SimpleAPI.Models.LCV.Caratula()
            {
                RutEmisor = configuracion.Empresa.RutEmpresa,
                RutEnvia = configuracion.Certificado.Rut,
                PeriodoTributario = DateTime.Now.Year + "-0" + DateTime.Now.Month,
                FechaResolucion = configuracion.Empresa.FechaResolucion,
                NumeroResolucion = configuracion.Empresa.NumeroResolucion,
                TipoLibro = TipoLibro.TipoLibroEnum.Especial,
                TipoEnvio = TipoEnvioLibro.TipoEnvioLibroEnum.Total,
                FolioNotificacion = 100
            };


            libro.EnvioLibro.ResumenPeriodo = new ResumenPeriodo()
            {
                TotalesGuiasDeVentas = envioAux.SetDTE.DTEs.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoTraslado == TipoTraslado.TipoTrasladoEnum.OperacionConstituyeVenta).Count(),
                MontoTotalVentasGuia = envioAux.SetDTE.DTEs.Where(x => x.Documento.Encabezado.IdentificacionDTE.TipoTraslado == TipoTraslado.TipoTrasladoEnum.OperacionConstituyeVenta).Sum(x => x.Documento.Encabezado.Totales.MontoTotal),
                TotalesGuiasAnuladas = 0, //Dato opcional. No hay un indicador en el DTE para establecer que está anulado. Se debe entregar según datos del propio desarrollador,
                TotalesFoliosAnulados = 0, //Dato opcional. No hay un indicador en el DTE para establecer que su folio está anulado. Se debe entregar según datos del propio desarrollador,               

                //El traslado es opcional. Se repite hasta 6 veces, según los códigos de NO venta (2, 3, 4, 5, 6, 7).
                Traslados = new List<TotalTraslado>()
                {
                    new TotalTraslado()
                    {
                        TipoTraslado = TipoTraslado.TipoTrasladoEnum.TrasladosInternos,
                        CantidadGuia = 1,
                        MontoGuia = 0
                    }
                }
            };

            libro.EnvioLibro.Detalles = new List<SimpleAPI.Models.LCV.Detalle>();
            foreach (var dte in envioAux.SetDTE.DTEs)
            {
                libro.EnvioLibro.Detalles.Add(new SimpleAPI.Models.LCV.Detalle()
                {
                    MontoTotal = dte.Documento.Encabezado.Totales.MontoTotal,
                    Folio = dte.Documento.Encabezado.IdentificacionDTE.Folio,
                    TipoOperacion = dte.Documento.Encabezado.IdentificacionDTE.TipoTraslado,
                    //Para indicar que la guía está anulada se debe utilizar este atributo. Por defecto se omitirá
                    IndicadorAnulado = IndicadorAnulado.IndicadorAnuladoEnum.NotSet
                });
            }

            return libro;
        }


        #endregion


        #region Utilidades

        public DTE GenerateRandomDTE(int folio, TipoDTE.DTEType tipo)
        {
            // DOCUMENTO
            Random r = new Random();
            var dte = new DTE();
            dte.Documento.Id = "TEST_2" + folio.ToString() + "_" + tipo;
            dte.Documento.Encabezado.IdentificacionDTE.TipoDTE = tipo;
            dte.Documento.Encabezado.IdentificacionDTE.FechaEmision = DateTime.Now;
            dte.Documento.Encabezado.IdentificacionDTE.Folio = folio;

            //DOCUMENTO - ENCABEZADO - EMISOR - CAMPOS OBLIGATORIOS          
            dte.Documento.Encabezado.Emisor.Rut = configuracion.Empresa.RutEmpresa;
            dte.Documento.Encabezado.Emisor.RazonSocial = configuracion.Empresa.RazonSocial;
            dte.Documento.Encabezado.Emisor.Giro = configuracion.Empresa.Giro;
            dte.Documento.Encabezado.Emisor.DireccionOrigen = configuracion.Empresa.Direccion;
            dte.Documento.Encabezado.Emisor.ComunaOrigen = configuracion.Empresa.Comuna;


            dte.Documento.Encabezado.Emisor.ActividadEconomica = configuracion.Empresa.CodigosActividades.Select(x => x.Codigo).ToList();

            //DOCUMENTO - ENCABEZADO - RECEPTOR - CAMPOS OBLIGATORIOS

            dte.Documento.Encabezado.Receptor.Rut = "66666666-6";
            dte.Documento.Encabezado.Receptor.RazonSocial = "Razon Social de Cliente";
            dte.Documento.Encabezado.Receptor.Direccion = "Dirección de cliente";
            dte.Documento.Encabezado.Receptor.Comuna = "Comuna de cliente";
            dte.Documento.Encabezado.Receptor.Ciudad = "Ciudad de cliente";
            dte.Documento.Encabezado.Receptor.Giro = "Giro de cliente";

            dte.Documento.Detalles = new List<SimpleAPI.Models.DTE.Detalle>();

            if (tipo == TipoDTE.DTEType.NotaCreditoElectronica)
            {
                dte.Documento.Referencias = new List<Referencia>();
                dte.Documento.Referencias.Add(new Referencia()
                {
                    CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeMontos,
                    FechaDocumentoReferencia = DateTime.Now,
                    FolioReferencia = "40",
                    IndicadorGlobal = 0,
                    Numero = 1,
                    RazonReferencia = "CORRIGE MONTOS",
                    TipoDocumento = TipoDTE.TipoReferencia.FacturaElectronica
                });
                dte.Documento.Detalles.Add(new SimpleAPI.Models.DTE.Detalle()
                {
                    NumeroLinea = 1,
                    Nombre = "DEVOLUCION",
                    Cantidad = 1,
                    Precio = 100,
                    MontoItem = 100
                });
            }
            else if (tipo == TipoDTE.DTEType.NotaDebitoElectronica)
            {
                dte.Documento.Referencias = new List<Referencia>();
                dte.Documento.Referencias.Add(new Referencia()
                {
                    CodigoReferencia = TipoReferencia.TipoReferenciaEnum.CorrigeMontos,
                    FechaDocumentoReferencia = DateTime.Now,
                    FolioReferencia = "41",
                    IndicadorGlobal = 0,
                    Numero = 1,
                    RazonReferencia = "RECARGO DE INTERESES",
                    TipoDocumento = TipoDTE.TipoReferencia.FacturaElectronica
                });
                dte.Documento.Detalles.Add(new SimpleAPI.Models.DTE.Detalle()
                {
                    NumeroLinea = 1,
                    Nombre = "RECARGO",
                    Cantidad = 1,
                    Precio = 100,
                    MontoItem = 100
                });
            }
            else if (tipo == TipoDTE.DTEType.FacturaCompraElectronica)
            {
                dte.Documento.Detalles.Add(new SimpleAPI.Models.DTE.Detalle()
                {
                    NumeroLinea = 1,
                    Nombre = "TECLADOS INALAMBRICOS",
                    Cantidad = 1,
                    Precio = 100,
                    MontoItem = 100,
                    CodigoImpuestoAdicional = new List<TipoImpuesto.TipoImpuestoEnum>()
                    {
                        TipoImpuesto.TipoImpuestoEnum.IVARetenidoTotal
                    }
                });
            }
            else
            {
                //DOCUMENTO - DETALLES

                int max_detalles = r.Next(1, 5);
                List<string> detallesRandom = configuracion.ProductosSimulacion.Select(x => x.Nombre).ToList();

                for (int i = 1; i <= max_detalles; i++)
                {
                    var detalle = new SimpleAPI.Models.DTE.Detalle();
                    detalle.NumeroLinea = i;
                    //detalle.IndicadorExento = ChileSystems.DTE.Engine.Enum.IndicadorFacturacionExencion.IndicadorFacturacionExencionEnum.NoAfectoOExento;
                    detalle.Nombre = detallesRandom[r.Next(0, detallesRandom.Count - 1)];
                    detalle.Cantidad = r.Next(1, 5);
                    detalle.Precio = r.Next(1, 150000);
                    detalle.MontoItem = (int)detalle.Cantidad * (int)detalle.Precio;
                    dte.Documento.Detalles.Add(detalle);
                }
            }


            calculosTotales(dte);
            return dte;
        }

        public async Task<string> EnviarAceptacionReclamo(int tipoDocumento, int folio, TipoAceptacion accion, string rutEmpresa, AmbienteEnum ambiente, string passwordCertificado = "")
        {
            try
            {
                var responseEnvio = await SimpleAPI.WS.AceptacionReclamo.AceptacionReclamo.NotificarAceptacionReclamoAsync(new SimpleAPI.SII.AceptacionReclamoEntity(rutEmpresa, tipoDocumento, folio, accion)
                {
                    Accion = accion
                }, configuracion.Certificado.Nombre, ambiente, ".\\out\\tkn.dat", passwordCertificado);
                if (responseEnvio != null)
                {
                    return responseEnvio.Descripcion;
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
            return "Error";
        }

        public async Task<EstadoDTEResult> ConsultarEstadoDTEAsync(AmbienteEnum ambiente, string rutReceptor, TipoDTE.DTEType tipo, int folio, DateTime fechaEmision, int total)
        {
            var responseEstadoDTE = await EstadoDTE.GetEstadoDTEAsync(new SimpleAPI.SII.GetEstadoEntity(configuracion.Certificado.Rut, configuracion.Empresa.RutEmpresa, rutReceptor, fechaEmision, (int)tipo, folio, total), configuracion.Certificado.Nombre, ambiente, ".\\out\\tkn.dat");
            return responseEstadoDTE;
        }

        public async Task<EstadoBoletaResult> ConsultarEstadoBoletaAsync(AmbienteEnum ambiente, string rutReceptor, TipoDTE.DTEType tipo, int folio, DateTime fechaEmision, int total)
        {
            var responseEstadoDTE = await EstadoDTE.GetEstadoBoletaAsync(new SimpleAPI.SII.GetEstadoEntity(configuracion.Certificado.Rut, configuracion.Empresa.RutEmpresa, rutReceptor, fechaEmision, (int)tipo, folio, total), configuracion.Certificado.Nombre, ambiente, ".\\out\\tkn.dat");
            return responseEstadoDTE;
        }

        public async Task<EstadoEnvioResult> ConsultarEstadoEnvioDTEAsync(AmbienteEnum ambiente, long trackId)
        {
            EstadoEnvioResult responseEstadoEnvio = await EstadoEnvio.GetEstadoEnvioAsync(new SimpleAPI.SII.GetEstadoEnvioEntity(configuracion.Empresa.RutEmpresa, trackId.ToString()), ambiente, ".\\out\\tkn.dat", configuracion.Certificado.Nombre);
            return responseEstadoEnvio;
        }

        public async Task<EstadoEnvioBoletaResult> ConsultarEstadoEnvioBoletaAsync(AmbienteEnum ambiente, long trackId)
        {
            var responseEstadoEnvio = await EstadoEnvio.GetEstadoEnvioBoletaAsync(new SimpleAPI.SII.GetEstadoEnvioEntity(configuracion.Empresa.RutEmpresa, trackId.ToString()), ambiente, ".\\out\\tkn.dat", configuracion.Certificado.Nombre);
            return responseEstadoEnvio;
            // return new EstadoEnvioBoletaResult() { Response = responseEstadoEnvio };
        }

        public string GenerarRespuestaEnvio(List<DTE> dtes, string estadoDTE)
        {
            RespuestaDTE response = new RespuestaDTE();
            response.Resultado = new Resultado();
            var result = response.Resultado;

            result.Id = "R_ENVIO1";
            result.Caratula = new SimpleAPI.Models.RespuestaEnvio.Caratula();
            result.Caratula.Fecha = DateTime.Now;
            result.Caratula.IdRespuesta = 1;
            result.Caratula.MailContacto = "mailcontacto@mail.com";
            result.Caratula.NombreContacto = "Contacto";
            result.Caratula.RutResponde = configuracion.Empresa.RutEmpresa;

            result.Caratula.NumeroDetalles = 1;
            result.Caratula.RutRecibe = "88888888-8";

            result.RecepcionEnvio = new List<RecepcionEnvio>();
            var recepcionEnvio = new RecepcionEnvio();

            recepcionEnvio.CodigoEnvio = 4545;
            //recepcionEnvio.Digest = SIMPLE_SDK.Security.Firma.Firma.GetDigestValueFromFile(dte.DTEfilepath);
            recepcionEnvio.EnvioDTEId = "SetDoc";
            recepcionEnvio.FechaRecepcion = DateTime.Now;
            recepcionEnvio.NumeroDTE = 2;
            recepcionEnvio.RutEmisor = result.Caratula.RutRecibe;
            recepcionEnvio.RutReceptor = result.Caratula.RutResponde;
            recepcionEnvio.EstadoRecepcionEnvio = EstadoEnvioEmpresa.EstadoEnvioEmpresaEnum.OK;
            recepcionEnvio.GlosaEstadoRecepcionEnvio = "ENVIO OK";
            recepcionEnvio.NombreArchivoEnvio = "ENVIO_DTE_1072427";
            recepcionEnvio.RecepcionDTE = new List<RecepcionDTE>();

            //foreach (var dte in dtes)
            //{
            //    var recepcionDTE = new RecepcionDTE();
            //    recepcionDTE.FechaEmision = dte.Documento.Encabezado.IdentificacionDTE.FechaEmision;
            //    recepcionDTE.Folio = dte.Documento.Encabezado.IdentificacionDTE.Folio;
            //    recepcionDTE.MontoTotal = dte.Documento.Encabezado.Totales.MontoTotal;
            //    recepcionDTE.RutEmisor = dte.Documento.Encabezado.Emisor.Rut;
            //    recepcionDTE.RutReceptor = dte.Documento.Encabezado.Receptor.Rut;
            //    recepcionDTE.TipoDTE = dte.Documento.Encabezado.IdentificacionDTE.TipoDTE;
            //    recepcionDTE.EstadoRecepcionDTE = ChileSystems.DTE.Engine.Enum.EstadoRecepcionDTE.EstadoRecepcionDTEEnum.Ok;
            //    recepcionDTE.GlosaEstadoRecepcionDTE = ChileSystems.DTE.Engine.Enum.EstadoRecepcionDTE.Glosa(recepcionDTE.EstadoRecepcionDTE);
            //    recepcionEnvio.RecepcionDTE.Add(recepcionDTE);
            //}

            var recepcionDTE = new RecepcionDTE();
            var dte = dtes[0];
            recepcionDTE.FechaEmision = dte.Documento.Encabezado.IdentificacionDTE.FechaEmision;
            recepcionDTE.Folio = dte.Documento.Encabezado.IdentificacionDTE.Folio;
            recepcionDTE.MontoTotal = dte.Documento.Encabezado.Totales.MontoTotal;
            recepcionDTE.RutEmisor = dte.Documento.Encabezado.Emisor.Rut;
            recepcionDTE.RutReceptor = dte.Documento.Encabezado.Receptor.Rut;
            recepcionDTE.TipoDTE = dte.Documento.Encabezado.IdentificacionDTE.TipoDTE;
            recepcionDTE.EstadoRecepcionDTE = EstadoRecepcionDTE.EstadoRecepcionDTEEnum.Ok;
            recepcionDTE.GlosaEstadoRecepcionDTE = EstadoRecepcionDTE.Glosa(recepcionDTE.EstadoRecepcionDTE);
            recepcionEnvio.RecepcionDTE.Add(recepcionDTE);

            var dte2 = dtes[1];
            recepcionDTE = new RecepcionDTE();
            recepcionDTE.FechaEmision = dte2.Documento.Encabezado.IdentificacionDTE.FechaEmision;
            recepcionDTE.Folio = dte2.Documento.Encabezado.IdentificacionDTE.Folio;
            recepcionDTE.MontoTotal = dte2.Documento.Encabezado.Totales.MontoTotal;
            recepcionDTE.RutEmisor = dte2.Documento.Encabezado.Emisor.Rut;
            recepcionDTE.RutReceptor = dte2.Documento.Encabezado.Receptor.Rut;
            recepcionDTE.TipoDTE = dte2.Documento.Encabezado.IdentificacionDTE.TipoDTE;
            recepcionDTE.EstadoRecepcionDTE = EstadoRecepcionDTE.EstadoRecepcionDTEEnum.ErrorRutReceptor;
            recepcionDTE.GlosaEstadoRecepcionDTE = EstadoRecepcionDTE.Glosa(recepcionDTE.EstadoRecepcionDTE);
            recepcionEnvio.RecepcionDTE.Add(recepcionDTE);


            result.RecepcionEnvio.Add(recepcionEnvio);

            var filepath = response.Firmar(configuracion.Certificado.Nombre, "");

            return filepath;
        }

        public string ResponderIntercambio(int estado, DTE dte, string motivo)
        {
            try
            {
                RespuestaDTE response = new RespuestaDTE();
                response.Resultado = new Resultado();

                var result = response.Resultado;

                result.Id = "R_001";
                result.Caratula = new SimpleAPI.Models.RespuestaEnvio.Caratula();
                result.Caratula.Fecha = DateTime.Now;
                result.Caratula.IdRespuesta = 1;
                result.Caratula.MailContacto = "test@test.cl";
                result.Caratula.NombreContacto = "Nombre Contacto";
                result.Caratula.RutResponde = configuracion.Empresa.RutEmpresa;

                result.Caratula.NumeroDetalles = 1;
                result.Caratula.RutRecibe = "88888888-8";

                result.ResultadoDTE = new List<ResultadoDTE>();
                var resultadoDTE = new ResultadoDTE();

                resultadoDTE.CodigoEnvio = 1;

                resultadoDTE.CodigoRechazoODiscrepancia = -1;//(int)estado;
                resultadoDTE.EstadoDTE = (EstadoResultadoDTE.EstadoResultadoDTEEnum)estado;
                resultadoDTE.GlosaEstadoDTE = string.IsNullOrEmpty(motivo) ? resultadoDTE.EstadoDTE.ToString() : motivo;

                resultadoDTE.RutEmisor = "88888888-8";
                resultadoDTE.RutReceptor = configuracion.Empresa.RutEmpresa;
                resultadoDTE.TipoDTE = TipoDTE.DTEType.FacturaElectronica;
                resultadoDTE.Folio = 52576;
                resultadoDTE.FechaEmision = new DateTime(2019, 2, 19);
                resultadoDTE.MontoTotal = 18635;

                result.ResultadoDTE.Add(resultadoDTE);

                string resultFilePath = response.Firmar(configuracion.Certificado.Nombre, "");
                return resultFilePath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ResponderDTE(int estado, DTE dte, string motivo)
        {
            try
            {
                RespuestaDTE response = new RespuestaDTE();
                response.Resultado = new Resultado();

                var result = response.Resultado;

                result.Id = "APROBACION_COMERCIAL_";
                result.Caratula = new SimpleAPI.Models.RespuestaEnvio.Caratula();
                result.Caratula.Fecha = DateTime.Now;
                result.Caratula.IdRespuesta = 1;
                result.Caratula.MailContacto = "test@test.cl";
                result.Caratula.NombreContacto = "Nombre Contacto";
                result.Caratula.RutResponde = configuracion.Empresa.RutEmpresa;

                result.Caratula.NumeroDetalles = 1;
                result.Caratula.RutRecibe = "88888888-8";

                result.ResultadoDTE = new List<ResultadoDTE>();
                var resultadoDTE = new ResultadoDTE();

                resultadoDTE.CodigoEnvio = 1;

                resultadoDTE.CodigoRechazoODiscrepancia = -1;//(int)estado;
                resultadoDTE.EstadoDTE = (EstadoResultadoDTE.EstadoResultadoDTEEnum)estado;
                resultadoDTE.GlosaEstadoDTE = string.IsNullOrEmpty(motivo) ? resultadoDTE.EstadoDTE.ToString() : motivo;

                resultadoDTE.RutEmisor = "88888888-8";
                resultadoDTE.RutReceptor = configuracion.Empresa.RutEmpresa;
                resultadoDTE.TipoDTE = dte.Documento.Encabezado.IdentificacionDTE.TipoDTE;
                resultadoDTE.Folio = dte.Documento.Encabezado.IdentificacionDTE.Folio;
                resultadoDTE.FechaEmision = dte.Documento.Encabezado.IdentificacionDTE.FechaEmision;
                resultadoDTE.MontoTotal = dte.Documento.Encabezado.Totales.MontoTotal;

                result.ResultadoDTE.Add(resultadoDTE);

                string resultFilePath = response.Firmar(configuracion.Certificado.Nombre, "");
                return resultFilePath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AcuseReciboMercaderias(DTE dte)
        {
            try
            {
                EnvioRecibos envio = new EnvioRecibos();
                envio.SetRecibos = new SetRecibos()
                {
                    Id = "EARM00",
                    Caratula = new SimpleAPI.Models.ReciboMercaderia.Caratula()
                    {
                        RutRecibe = "88888888-8",
                        RutResponde = configuracion.Empresa.RutEmpresa,
                        NombreContacto = "Nombre Contacto"
                    }
                };

                envio.SetRecibos.Recibos = new List<Recibo>()
                {
                    new Recibo()
                    {
                         DocumentoRecibo = new DocumentoRecibo()
                         {
                            Id = "R01",
                            RutEmisor = "88888888-8",
                            RutReceptor = configuracion.Empresa.RutEmpresa,
                            FechaEmision = dte.Documento.Encabezado.IdentificacionDTE.FechaEmision,
                            Folio = dte.Documento.Encabezado.IdentificacionDTE.Folio,
                            MontoTotal = dte.Documento.Encabezado.Totales.MontoTotal,
                            TipoDocumento = dte.Documento.Encabezado.IdentificacionDTE.TipoDTE,
                            Recinto = "Recinto",
                            RutFirma = configuracion.Certificado.Rut
                         }
                    }
                };

                int id = -1;
                var recibo = envio.SetRecibos.Recibos.First();
                envio.SetRecibos.Id = "EARM" + id;
                recibo.DocumentoRecibo.Id = "RM" + id;

                envio.SetRecibos.signedXmls.Add(recibo.Firmar(configuracion.Certificado.Nombre));
                string filePath = envio.Firmar(configuracion.Certificado.Nombre);
                return filePath;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public static string TipoDTEString(TipoDTE.DTEType tipo)
        {
            switch (tipo)
            {
                case TipoDTE.DTEType.FacturaCompraElectronica: return "FACTURA DE COMPRA ELECTRÓNICA";
                case TipoDTE.DTEType.FacturaElectronica: return "FACTURA ELECTRÓNICA";
                case TipoDTE.DTEType.FacturaElectronicaExenta: return "FACTURA ELECTRÓNICA EXENTA";
                case TipoDTE.DTEType.GuiaDespachoElectronica: return "GUIA DE DESPACHO ELECTRÓNICA";
                case TipoDTE.DTEType.NotaCreditoElectronica: return "NOTA DE CRÉDITO ELECTRÓNICA";
                case TipoDTE.DTEType.NotaDebitoElectronica: return "NOTA DE DÉBITO ELECTRÓNICA";
                case TipoDTE.DTEType.Factura: return "FACTURA MANUAL";
                case TipoDTE.DTEType.NotaCredito: return "NOTA DE CRÉDITO MANUAL";
            }
            return "Not Set";
        }

        #endregion  

    }
}
