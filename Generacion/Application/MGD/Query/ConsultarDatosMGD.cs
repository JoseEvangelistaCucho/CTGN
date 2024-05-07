using Generacion.Application.DataBase;
using Generacion.Models.ION;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.MGD.Query
{
    public class ConsultarDatosMGD
    {
        private readonly IConexionBD _conexion;
        public ConsultarDatosMGD(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Respuesta<List<DatosFormatoMGD>>> ObtenerDatosMGD(DateTime fechaIni , DateTime FechaFin)
        {
            Respuesta<List<DatosFormatoMGD>> respuesta = new Respuesta<List<DatosFormatoMGD>>();
            respuesta.Detalle = new List<DatosFormatoMGD>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ConsultarDatosMGD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fechaIni", OracleDbType.Varchar2).Value = fechaIni.ToString();
                        command.Parameters.Add("p_fechaFin", OracleDbType.Varchar2).Value = FechaFin.ToString();
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            List<DatosFormatoMGD> datos = new List<DatosFormatoMGD>();
                            DatosFormatoMGD dato = new DatosFormatoMGD();
                            DateTime fechaHora;
                            string fecha = string.Empty;
                            string hora = string.Empty;
                            foreach (DataRow row in dataTable.Rows)
                            {

                                fechaHora = DateTime.Parse(row["FECHA"].ToString());

                                fecha = fechaHora.ToShortDateString();
                                hora = fechaHora.ToString("HH:mm");

                                dato = new DatosFormatoMGD();
                                dato.Hora = hora;
                                dato.KWDelInt = decimal.Parse(row["Potencia_Activa_MW"].ToString());
                                dato.KVARDelInt = decimal.Parse(row["Potencia_Reactiva_MVAR"].ToString());
                                dato.KWRecInt = decimal.Parse(row["tension_kV"].ToString());
                                dato.KVARRecInt = decimal.Parse(row["Frecuencia_hz"].ToString());

                                datos.Add(dato);
                            }

                            respuesta.Detalle = datos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
