using Generacion.Application.Common;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Controllers;
using Generacion.Models;
using MediatR;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;

namespace Generacion.Application.ReporteModulacion.Command
{
    public class GuardarfacturaModulacionQuery : IRequest<Respuesta<string>>
    {
        public string RutaArchivo { get; set; }
    }
    public class GuardarfacturaModulacionHandler : IRequestHandler<GuardarfacturaModulacionQuery, Respuesta<string>>
    {
        private readonly CacheDatos _cacheDatos;
        private readonly Function _function;
        public GuardarfacturaModulacionHandler(CacheDatos cacheDatos, Function function)
        {
            _function = function;
            _cacheDatos = cacheDatos;
        }
        public async Task<Respuesta<string>> Handle(GuardarfacturaModulacionQuery request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            List<DetalleFacturacionModulacion> listaDetalle = new List<DetalleFacturacionModulacion>();

            try
            {
                FacturacionModulacion facturacionModulacion = new FacturacionModulacion();
                // List<DetalleFacturacionModulacion> listaDetalle = new List<DetalleFacturacionModulacion>();
                Pef_MGD pef_MGD_Luren = new Pef_MGD();
                Pef_MGD pef_MGD_Pedregal = new Pef_MGD();
                using (var package = new ExcelPackage(new FileInfo(request.RutaArchivo)))
                {
                    var worksheet = package.Workbook.Worksheets[1]; // Se asume que los datos están en la primera hoja

                    // Obtener datos de la columna O (asumiendo que empieza desde la segunda fila)
                    int rowCount = worksheet.Dimension.Rows;
                    ValoresPEM valoresPEM = new ValoresPEM();
                    valoresPEM.Potencia = new Dictionary<string, decimal>();
                    valoresPEM.Energia = new Dictionary<string, decimal>();


                    //Columna  K , L , M
                    pef_MGD_Luren.Pef = decimal.Parse(worksheet.Cells[2, 11].Text);
                    pef_MGD_Luren.Operacion1 = decimal.Parse(worksheet.Cells[2, 12].Text);
                    pef_MGD_Luren.Operacion1 = decimal.Parse(worksheet.Cells[2, 13].Text);
                    facturacionModulacion.Pef_MGD_Luren = pef_MGD_Luren;
                    //Columna  K , L , M
                    pef_MGD_Pedregal.Pef = decimal.Parse(worksheet.Cells[3, 11].Text);
                    pef_MGD_Pedregal.Operacion1 = decimal.Parse(worksheet.Cells[3, 12].Text);
                    pef_MGD_Pedregal.Operacion1 = decimal.Parse(worksheet.Cells[3, 13].Text);
                    facturacionModulacion.Pef_MGD_Pedreal = pef_MGD_Pedregal;

                    for (int i = 5; i <= rowCount; i++)
                    {

                        if (!string.IsNullOrEmpty(worksheet.Cells[i, 1].Text))
                        {
                            ValoresPEMFSoles valoresPEMFAnterior = new ValoresPEMFSoles();
                            ValoresPEMFSoles valoresPEMFActual = new ValoresPEMFSoles();
                            // Crear una nueva instancia en cada iteración
                            DetalleFacturacionModulacion modulacion = new DetalleFacturacionModulacion();

                            string[] formatos = { "M/d/yy H:mm", "M/d/yy HH:mm", "MM/dd/yy HH:mm", "MM/dd/yy H:mm", "M/dd/yyyy H:mm" };
                            DateTime fecha;
                            if (DateTime.TryParseExact(worksheet.Cells[i, 1].Text, formatos, new CultureInfo("es-ES"), DateTimeStyles.None, out fecha))
                            {
                                //Columna A
                                modulacion.Fecha = fecha.ToString("dd/MM/yyyy HH:mm:ss");
                                modulacion.Hora = fecha.ToString().Split(' ')[1];
                            }


                            modulacion.Dia = int.Parse(worksheet.Cells[i, 2].Text);
                            modulacion.BH = int.Parse(worksheet.Cells[i, 4].Text);

                            if (modulacion.Dia == 13)
                            {
                                bool asdas = true;
                            }
                            if (modulacion.Dia == 12)
                            {
                                bool asdas = true;
                            }

                            //Columna E   ,  F 
                            modulacion.CTLuren = new CTCentralModulacion();
                            modulacion.CTLuren.MWH = worksheet.Cells[i, 5].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 5].Text);
                            modulacion.CTLuren.MW = worksheet.Cells[i, 6].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 6].Text);

                            //Columna G   ,   H
                            modulacion.CTPedregal = new CTCentralModulacion();
                            modulacion.CTPedregal.MWH = worksheet.Cells[i, 7].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 7].Text);
                            modulacion.CTPedregal.MW = worksheet.Cells[i, 8].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 8].Text);

                            if (i == 11)
                            {
                                //Columna k
                                facturacionModulacion.TC = decimal.Parse(worksheet.Cells[i, 11].Text);
                            }
                            if (i >= 6 && i <= 9)
                            {
                                //Columna L   ,   k                              
                                string keyPotencia = worksheet.Cells[i, 11].Text;
                                decimal valorPemp = decimal.Parse(worksheet.Cells[i, 12].Text);
                                valoresPEM.Potencia.Add(keyPotencia.Split(' ')[1], valorPemp);
                            }
                            if (i >= 6 && i <= 9)
                            {
                                //Columna L   ,   M
                                string keyEnergia = worksheet.Cells[i, 11].Text;
                                decimal valorPemf = decimal.Parse(worksheet.Cells[i, 13].Text);
                                valoresPEM.Energia.Add(keyEnergia.Split(' ')[1], valorPemf);
                                facturacionModulacion.ValoresPEM = valoresPEM;
                            }

                            if (i == 14)
                            {
                                valoresPEMFAnterior.PEMP = decimal.Parse(worksheet.Cells[i, 11].Text);
                                valoresPEMFAnterior.PEMF = decimal.Parse(worksheet.Cells[i, 12].Text);
                                facturacionModulacion.PEMFAnterior = valoresPEMFAnterior;
                            }
                            if (i == 15)
                            {
                                valoresPEMFActual.PEMP = decimal.Parse(worksheet.Cells[i, 11].Text);
                                valoresPEMFActual.PEMF = decimal.Parse(worksheet.Cells[i, 12].Text);
                                facturacionModulacion.PEMFActual = valoresPEMFActual;
                            }

                            //Columna O , P , Q , R , S
                            modulacion.CTLurenTarifas = new CostosBarraYMarginal();
                            modulacion.CTLurenTarifas.VBEM = worksheet.Cells[i, 15].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 15].Text);
                            modulacion.CTLurenTarifas.TarifaEnBarra = worksheet.Cells[i, 16].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 16].Text);
                            modulacion.CTLurenTarifas.CostoMarginal = worksheet.Cells[i, 17].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 17].Text);
                            modulacion.CTLurenTarifas.TM = worksheet.Cells[i, 18].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 18].Text);
                            modulacion.CTLurenTarifas.PagoPorModulacion = worksheet.Cells[i, 19].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 19].Text);

                            //Columna T , U , V , W , X
                            modulacion.CTPedregalTarifas = new CostosBarraYMarginal();

                            modulacion.CTPedregalTarifas.VBEM = worksheet.Cells[i, 20].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 20].Text);
                            modulacion.CTPedregalTarifas.TarifaEnBarra = worksheet.Cells[i, 21].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 21].Text);
                            modulacion.CTPedregalTarifas.CostoMarginal = worksheet.Cells[i, 22].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 22].Text);
                            modulacion.CTPedregalTarifas.TM = worksheet.Cells[i, 23].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 23].Text);
                            modulacion.CTPedregalTarifas.PagoPorModulacion = worksheet.Cells[i, 24].Text.Trim().Equals("-") ? 0 : decimal.Parse(worksheet.Cells[i, 24].Text);


                            listaDetalle.Add(modulacion);
                        }
                    }
                    facturacionModulacion.DetalleFacturacionModulacion = new List<DetalleFacturacionModulacion>();
                    facturacionModulacion.DetalleFacturacionModulacion = listaDetalle;
                }

                var datosOperario = await _function.ObtenerDatosOperario();
                string key = $"{Constante.facturaModulacion}_{datosOperario.IdSitio}";
                _cacheDatos.GuardarDatosCache(key, JsonConvert.SerializeObject(facturacionModulacion));
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
