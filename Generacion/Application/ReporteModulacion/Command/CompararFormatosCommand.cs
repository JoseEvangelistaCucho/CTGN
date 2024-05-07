using Generacion.Application.Common;
using Generacion.Controllers;
using Generacion.Models;
using Generacion.Models.ReporteModulacion;
using MediatR;

namespace Generacion.Application.ReporteModulacion.Command
{
    public class CompararFormatosCommand : IRequest<Respuesta<List<DetalleFacturacionModulacion>>>
    {
        public decimal Tc { get; set; }
        public ValoresMT Potencia { get; set; }
        public ValoresMT Energia { get; set; }
        public ValoresPEMFSoles ValoresPEMActuales { get; set; }
        public ValoresPEMFSoles ValoresPEMAnteriores { get; set; }


    }

    public class CompararFormatosHandler : IRequestHandler<CompararFormatosCommand, Respuesta<List<DetalleFacturacionModulacion>>>
    {
        private readonly ProcessExecutionContextExtensions _context;
        private string _mensajeNull;
        private List<CostoMarginal> _costosMarginales;

        public CompararFormatosHandler(ProcessExecutionContextExtensions context)
        {
            _context = context;
        }
        public async Task<Respuesta<List<DetalleFacturacionModulacion>>> Handle(CompararFormatosCommand request, CancellationToken cancellationToken)
        {
            Respuesta<List<DetalleFacturacionModulacion>> respuesta = new Respuesta<List<DetalleFacturacionModulacion>>();
            try
            {
                _costosMarginales = (List<CostoMarginal>)_context["costosMarginales"];
                var facutarcionModulacion = (FacturacionModulacion)_context["facutarcionModulacion"];

                if (_costosMarginales == null || facutarcionModulacion == null)
                {
                    _mensajeNull = string.Concat("Falta subir el archivo ", _costosMarginales == null ? "Costos Marginales" : "Facturación Modulación");
                    respuesta.IdRespuesta = 99;
                    respuesta.Mensaje = _mensajeNull;
                    throw new NotImplementedException();
                }

                _mensajeNull = await CompararFactorPerdidasMedias(request.Energia, request.Potencia, facutarcionModulacion.ValoresPEM);
                _mensajeNull = await CompararTipoCambio(request.Tc, facutarcionModulacion.TC);
                _mensajeNull = await CompararValoresPEM(request.ValoresPEMActuales, facutarcionModulacion.PEMFActual, "actual");

                //traer informacion de la base de datos
                //_mensajeNull = await CompararValoresPEM(request.ValoresPEMAnteriores, facutarcionModulacion.PEMFActual,"anterior");

                if (!string.IsNullOrEmpty(_mensajeNull))
                {
                    throw new NotImplementedException();
                }

                List<DetalleFacturacionModulacion> diferenciasLuren = await CompararDatosFacturacion("luren", facutarcionModulacion, request.Energia);
                List<DetalleFacturacionModulacion> diferenciasPedregal = await CompararDatosFacturacion("pedregal", facutarcionModulacion, request.Energia);
                diferenciasLuren.AddRange(diferenciasPedregal);

                if (diferenciasLuren != null && diferenciasLuren.Count > 0)
                {
                    respuesta.Detalle = new List<DetalleFacturacionModulacion>();
                    respuesta.Detalle = diferenciasLuren;

                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = string.IsNullOrEmpty(_mensajeNull) ? "Error al procesar la solicitud." : _mensajeNull;
            }
            return respuesta;
        }

        public async Task<string> CompararValoresPEM(ValoresPEMFSoles pem, ValoresPEMFSoles pemReporte, string tipo)
        {
            string respuesta = "";
            try
            {
                if (pem.PEMP != pemReporte.PEMP)
                {
                    respuesta = $"Diferencia en valores PEMP {tipo}.";
                }

                if (pem.PEMF != pemReporte.PEMF)
                {
                    respuesta = $"Diferencia en valores PEMF {tipo}.";
                }

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<string> CompararTipoCambio(decimal tc, decimal tcReporte)
        {
            string respuesta = "";
            try
            {
                if (tc != tcReporte)
                {
                    respuesta = "Diferencia en el tipo de cambio.";
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<string> CompararFactorPerdidasMedias(ValoresMT energia, ValoresMT potencia, ValoresPEM valoresPEM)
        {
            string respuesta = "";
            string respuestaEnergia = "";
            string respuestaPotencia = "";
            try
            {
                if (energia.SistemaMAT != valoresPEM.Energia["MAT"])
                {
                    respuestaEnergia = "Diferencias en Energia Sistema MAT";
                }
                if (energia.SistemaAT != valoresPEM.Energia["AT"])
                {
                    respuestaEnergia = "Diferencias en Energia Sistema AT ," + respuestaEnergia;
                }
                if (energia.SistemaMATAT != valoresPEM.Energia["MAT/AT"])
                {
                    respuestaEnergia = "Diferencias en Energia Sistema MAT/AT ," + respuestaEnergia;
                }
                if (energia.SistemaMT != valoresPEM.Energia["MT"])
                {
                    respuestaEnergia = "Diferencias en Energia Sistema MT ," + respuestaEnergia;
                }

                if (potencia.SistemaMAT != valoresPEM.Potencia["MAT"])
                {
                    respuestaPotencia = "Diferencias en Potencia Sistema MAT ,";
                }
                if (potencia.SistemaAT != valoresPEM.Potencia["AT"])
                {
                    respuestaPotencia = "Diferencias en Potencia Sistema AT ," + respuestaPotencia;
                }
                if (potencia.SistemaMATAT != valoresPEM.Potencia["MAT/AT"])
                {
                    respuestaPotencia = "Diferencias en Potencia Sistema MAT/AT ," + respuestaPotencia;
                }
                if (potencia.SistemaMT != valoresPEM.Potencia["MT"])
                {
                    respuestaPotencia = "Diferencias en Potencia Sistema MT ," + respuestaPotencia;
                }
                respuesta = string.Concat(respuestaEnergia, respuestaPotencia);
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
        public async Task<List<DetalleFacturacionModulacion>> CompararDatosFacturacion(string CT, FacturacionModulacion modulacion, ValoresMT energia)
        {
            List<DetalleFacturacionModulacion> listaDiferencias = new List<DetalleFacturacionModulacion>();
            try
            {
                if (CT.Equals("pedregal"))
                {
                    int index = 0;
                    foreach (var item in modulacion.DetalleFacturacionModulacion)
                    {
                        decimal diferenciaVbem = await compararVBEM(item.CTPedregal, energia, modulacion.Pef_MGD_Pedreal, item.CTPedregalTarifas);

                        decimal diferenciaTarifaBarra = await compararTaficaEnBarra(item, modulacion);
                        decimal diferenciaCostoMarginal = await compararCostoMarginal(item, index);

                        var valorDireferencia = item;

                        if (diferenciaVbem != 0)
                        {
                            valorDireferencia.CTPedregalTarifas.VBEM = diferenciaVbem;
                            listaDiferencias.Add(valorDireferencia);
                        }
                        if (diferenciaTarifaBarra != 0)
                        {
                            valorDireferencia.CTPedregalTarifas.TarifaEnBarra = diferenciaTarifaBarra;
                            listaDiferencias.Add(valorDireferencia);
                        }
                        index++;
                    }
                }
                if (CT.Equals("luren"))
                {
                    foreach (var item in modulacion.DetalleFacturacionModulacion)
                    {
                        decimal diferenciaVbem = await compararVBEM(item.CTLuren, energia, modulacion.Pef_MGD_Luren, item.CTLurenTarifas);
                        if (diferenciaVbem != 0)
                        {
                            var valorDireferencia = item;
                            valorDireferencia.CTLurenTarifas.VBEM = diferenciaVbem;

                            listaDiferencias.Add(valorDireferencia);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return listaDiferencias;
        }
        public async Task<decimal> compararCostoMarginal(DetalleFacturacionModulacion detalle, int index)
        {
            decimal respuesta = 0;
            try
            {
                decimal valor = 0;

                decimal costo = _costosMarginales[index].CostoConvertido;

                decimal valorSinRound = _costosMarginales[index].Costo / decimal.Parse("3.842") * 1000;
                var valorRedondeado = Math.Round(valorSinRound, 2, MidpointRounding.AwayFromZero);
                var valorRedondeado2 = Math.Ceiling(valorSinRound * 100) / 100;

                if (detalle.CTLurenTarifas.CostoMarginal != costo || detalle.CTPedregalTarifas.CostoMarginal != costo)
                {
                    respuesta = costo;
                }

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }


        public async Task<decimal> compararTaficaEnBarra(DetalleFacturacionModulacion detalle, FacturacionModulacion modulacion)
        {
            decimal respuesta = 0;
            try
            {
                //B8 = DIA ,D8 = BH, L14  = PEMF Ant ,K11 = TC , k14 = PEMP Ant, L15  = PEMF Act , K15 = PEMP Act
                decimal valor = 0;
                if (detalle.Dia <= 3)
                {
                    if (detalle.BH == 1)
                    {
                        //$L$14*10/$K$11
                        valor = await compararVBEM(modulacion.PEMFAnterior.PEMF, modulacion.TC);
                    }
                    else
                    {
                        //$K$14*10/$K$11
                        valor = await compararVBEM(modulacion.PEMFAnterior.PEMP, modulacion.TC);
                    }
                }
                else
                {
                    if (detalle.BH == 1)
                    {
                        //$L$15*10/$K$11
                        valor = await compararVBEM(modulacion.PEMFActual.PEMF, modulacion.TC);
                    }
                    else
                    {
                        //$$K$15*10/$K$11
                        valor = await compararVBEM(modulacion.PEMFActual.PEMP, modulacion.TC);
                    }
                }

                if (valor != detalle.CTPedregalTarifas.TarifaEnBarra || valor != detalle.CTLurenTarifas.TarifaEnBarra)
                {
                    respuesta = valor;
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<decimal> compararVBEM(decimal pem, decimal tc)
        {
            decimal valor = 0;
            try
            {
                valor = Math.Round(pem * 10 / tc, 2);
            }
            catch (Exception ex)
            {

            }
            return valor;
        }

        public async Task<decimal> compararVBEM(CTCentralModulacion cTCentral, ValoresMT energia, Pef_MGD pef_MGD, CostosBarraYMarginal costosBarraYMarginal)
        {
            decimal respuesta = 0;

            try
            {
                decimal mwh = cTCentral.MW / 4;

                decimal valor = Math.Round((pef_MGD.Pef * decimal.Parse("0.95") / 4 - mwh) * energia.SistemaAT * energia.SistemaMT, 2);

                if (valor > 0 && valor != costosBarraYMarginal.VBEM)
                {
                    respuesta = valor;
                }

            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }

    }
}
