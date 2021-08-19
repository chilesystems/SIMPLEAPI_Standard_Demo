using SimpleAPI.Enum;
using SimpleAPI.Models.DTE;
using SimpleAPI.Models.Envios;
using SimpleAPI.Models.ReciboMercaderia;
using SimpleAPI.Models.RespuestaEnvio;
using System;
using System.Collections.Generic;
using System.Linq;
using Caratula = SimpleAPI.Models.Envios.Caratula;

namespace TestUnitarios
{
    public class Handler
    {
        private class Empresa
        {
            public DateTime FechaResolucion { get { return new DateTime(2021, 1, 1); } }
            public int NumeroResolucion { get { return 0; } }
            public string RutEmpresa { get { return "11111111-1"; } }
            public string RutCertificado { get { return "17096073-4"; } }
        }

        public DTE GenerateDTE(TipoDTE.DTEType tipoDTE, int folio)
        {
            Empresa empresa = new Empresa();
            // DOCUMENTO
            var dte = new DTE();
            //
            // DOCUMENTO - ENCABEZADO - CAMPO OBLIGATORIO
            //Id = puede ser compuesto según tus propios requerimientos pero debe ser único                  
            dte.Documento.Id = "DTE_" + DateTime.Now.Ticks.ToString();

            // DOCUMENTO - ENCABEZADO - IDENTIFICADOR DEL DOCUMENTO - CAMPOS OBLIGATORIOS
            dte.Documento.Encabezado.IdentificacionDTE.TipoDTE = tipoDTE;
            dte.Documento.Encabezado.IdentificacionDTE.FechaEmision = DateTime.Now;
            dte.Documento.Encabezado.IdentificacionDTE.Folio = folio;

            dte.Documento.Encabezado.Emisor = new Emisor();
            dte.Documento.Encabezado.Emisor.Rut = empresa.RutEmpresa;
            dte.Documento.Encabezado.Emisor.ComunaOrigen = "Comuna";
            dte.Documento.Encabezado.Emisor.DireccionOrigen = "Dirección";
            //Para boletas electrónicas
            if (tipoDTE == TipoDTE.DTEType.BoletaElectronica || tipoDTE == TipoDTE.DTEType.BoletaElectronicaExenta)
            {
                dte.Documento.Encabezado.IdentificacionDTE.IndicadorServicio = IndicadorServicio.IndicadorServicioEnum.BoletaVentasYServicios;
                dte.Documento.Encabezado.Emisor.RazonSocialBoleta = "Razón Social";
                dte.Documento.Encabezado.Emisor.GiroBoleta = "Giro";
            }
            else
            {
                dte.Documento.Encabezado.Emisor.ActividadEconomica = new List<int>() { 5, 10, 15 };
                dte.Documento.Encabezado.Emisor.RazonSocial = "Razón Social";
                dte.Documento.Encabezado.Emisor.Giro = "Giro";
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
            dte.Documento.Detalles = new List<Detalle>();
            var detalle = new Detalle();
            detalle.NumeroLinea = 1;
            detalle.Nombre = "SERVICIO DE FACTURACION ELECT";
            detalle.Cantidad = 12;
            detalle.Precio = 170;
            detalle.MontoItem = 2040;
            dte.Documento.Detalles.Add(detalle);

            detalle = new Detalle();
            detalle.NumeroLinea = 2;
            detalle.Nombre = "DESARROLLO DE ETL";
            detalle.Cantidad = 20;
            detalle.Precio = 1050;
            detalle.MontoItem = 21000;
            dte.Documento.Detalles.Add(detalle);

            //DOCUMENTO - ENCABEZADO - TOTALES - CAMPOS OBLIGATORIOS
            calculosTotales(dte);
        }

        private void calculosTotales(DTE dte)
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

        public EnvioDTE GenerarEnvioDTEToSII(List<DTE> dtes, List<string> xmlDtes)
        {
            Empresa empresa = new Empresa();
            var EnvioSII = new EnvioDTE();
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


            EnvioSII.SetDTE.Caratula = new Caratula();
            EnvioSII.SetDTE.Caratula.FechaEnvio = DateTime.Now;
            /*Fecha de Resolución y Número de Resolución se averiguan en el sitio del SII según ambiente de producción o certificación*/
            EnvioSII.SetDTE.Caratula.FechaResolucion = empresa.FechaResolucion;
            EnvioSII.SetDTE.Caratula.NumeroResolucion = empresa.NumeroResolucion;

            EnvioSII.SetDTE.Caratula.RutEmisor = empresa.RutEmpresa;
            EnvioSII.SetDTE.Caratula.RutEnvia = empresa.RutCertificado;
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
            else if (EnvioSII.SetDTE.DTEs.Any(x => !string.IsNullOrEmpty(x.Exportaciones.Id)))
            {
                var tipos = EnvioSII.SetDTE.DTEs.GroupBy(x => x.Exportaciones.Encabezado.IdentificacionDTE.TipoDTE);
                foreach (var a in tipos)
                {
                    EnvioSII.SetDTE.Caratula.SubTotalesDTE.Add(new SubTotalesDTE()
                    {
                        Cantidad = a.Count(),
                        TipoDTE = a.ElementAt(0).Exportaciones.Encabezado.IdentificacionDTE.TipoDTE
                    });
                }
            }

            return EnvioSII;
        }


        public RespuestaDTE GenerarRespuestaEnvio(List<RecepcionDTE> dtes)
        {
            Empresa empresa = new Empresa();

            RespuestaDTE response = new RespuestaDTE();
            response.Resultado = new Resultado();
            var result = response.Resultado;

            result.Id = $"R_ENVIO_{DateTime.Now.Ticks}";
            result.Caratula = new SimpleAPI.Models.RespuestaEnvio.Caratula();
            result.Caratula.Fecha = DateTime.Now;
            result.Caratula.IdRespuesta = 1;
            result.Caratula.MailContacto = "mailcontacto@mail.com";
            result.Caratula.NombreContacto = "Contacto";
            result.Caratula.RutResponde = empresa.RutEmpresa;

            result.Caratula.NumeroDetalles = 1;
            result.Caratula.RutRecibe = "88888888-8";

            result.RecepcionEnvio = new List<RecepcionEnvio>();
            var recepcionEnvio = new RecepcionEnvio();

            recepcionEnvio.CodigoEnvio = 4545;
            recepcionEnvio.EnvioDTEId = "SetDoc";
            recepcionEnvio.FechaRecepcion = DateTime.Now;
            recepcionEnvio.NumeroDTE = 2;
            recepcionEnvio.RutEmisor = result.Caratula.RutRecibe;
            recepcionEnvio.RutReceptor = result.Caratula.RutResponde;
            recepcionEnvio.EstadoRecepcionEnvio = EstadoEnvioEmpresa.EstadoEnvioEmpresaEnum.OK;
            recepcionEnvio.GlosaEstadoRecepcionEnvio = "ENVIO OK";
            recepcionEnvio.NombreArchivoEnvio = "ENVIO_DTE_1072427";
            recepcionEnvio.RecepcionDTE = new List<RecepcionDTE>();

            foreach (var dte_ in dtes)
            {
                //Aquí indicar el estado de la recepción
                dte_.EstadoRecepcionDTE = EstadoRecepcionDTE.EstadoRecepcionDTEEnum.Ok;
                dte_.GlosaEstadoRecepcionDTE = SimpleAPI.Enum.EstadoRecepcionDTE.Glosa(dte_.EstadoRecepcionDTE);
                recepcionEnvio.RecepcionDTE.Add(dte_);
            }

            result.RecepcionEnvio.Add(recepcionEnvio);

            return response;
        }

        public RespuestaDTE GenerarRespuestaDTE(List<ResultadoDTE> dtes)
        {
            Empresa empresa = new Empresa();
            RespuestaDTE response = new RespuestaDTE();
            response.Resultado = new Resultado();

            var result = response.Resultado;
            result.Id = $"APROBACION_COMERCIAL_{DateTime.Now.Ticks}";
            result.Caratula = new SimpleAPI.Models.RespuestaEnvio.Caratula();
            result.Caratula.Fecha = DateTime.Now;
            result.Caratula.IdRespuesta = 1;
            result.Caratula.MailContacto = "test@test.cl";
            result.Caratula.NombreContacto = "Nombre Contacto";
            result.Caratula.RutResponde = empresa.RutEmpresa;

            result.Caratula.NumeroDetalles = 1;
            result.Caratula.RutRecibe = "88888888-8";

            result.ResultadoDTE = dtes;
            return response;
        }
        public EnvioRecibos AcuseReciboMercaderias(Recibo recibo)
        {
            recibo.DocumentoRecibo.Id = $"RM_{DateTime.Now.Ticks}";
            Empresa empresa = new Empresa();
            EnvioRecibos envio = new EnvioRecibos();
            envio.SetRecibos = new SetRecibos()
            {
                Id = "EARM00",
                Caratula = new SimpleAPI.Models.ReciboMercaderia.Caratula()
                {
                    RutRecibe = "88888888-8",
                    RutResponde = empresa.RutEmpresa,
                    NombreContacto = "Nombre Contacto"
                }
            };

            envio.SetRecibos.Recibos = new List<Recibo>()
            {
                recibo
            };

            envio.SetRecibos.Id = $"EARM_{DateTime.Now.Ticks}";            
            envio.SetRecibos.signedXmls.Add(recibo.filePath);

            return envio;
        }
    }
}
