using Generacion.Application.Funciones;
using System.ComponentModel;

namespace Generacion.Models.ControlGAS
{
    public class ContratoGas: ValidarPropiedadesNulasOVaciasBase
    {
        public string IdContratoGas { get; set; }
        [DisplayName("Ingrese un valor en Contrato de gas.")]
        public decimal ConsumoContrato { get; set; }
       // public string Fecha { get; set; }
        public int Activo { get; set; }
       // public string IdOperario { get; set; }
    }
    public class ConsumoGas : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdConsumoGas { get; set; }
        //public string Fecha { get; set; }
        public int DiasMes { get; set; }
        [DisplayName("Ingrese un valor en Consumo del mes")]
        public decimal ConsumoDelMes { get; set; }
        public string IdContratoGas { get; set; }
        //public string IdOperario { get; set; }
    }
    public class DetalleConsumoGas : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdDetalleConsumoGas { get; set; }
        public string Fecha { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public decimal ConsumoTotalActual { get; set; }
        [DisplayName("Ingrese un valor en Consumo diario")]
        public decimal ConsumoDiario { get; set; }
        public string IdConsumoGas { get; set; }
    }

}
