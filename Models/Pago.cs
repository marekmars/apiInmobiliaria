
using System.ComponentModel.DataAnnotations.Schema;


namespace Api_Inmobiliaria.Models;
[Table("pagos")]
public class Pago{
[Column("id")]
    public int Id { get ; set ;}
    [Column("nroPago")]
    public int NroPago { get ; set;}
    [Column("idContrato")]
    public int IdContrato  { get ; set ; }
    [Column("fechaPago")]
    public DateTime FechaPago { get ; set ; }
    public double Importe { get ; set ; }
    
    [ForeignKey(nameof(IdContrato))]
    public Contrato? Contrato { get ; set ; }

}