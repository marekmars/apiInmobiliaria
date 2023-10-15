

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api_Inmobiliaria.Models;

public class Contrato {
    [Display(Name ="Codigo")]
    public int Id { get ; set ; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get ; set ; }

    public bool Estado { get ; set ; }
    public double Mensualidad { get ; set ; }
    public int InmuebleId { get ; set ; }
    [ForeignKey(nameof(InmuebleId))]
    public Inmueble? Inmueble { get ; set ;}
    public int InquilinoId { get ; set ; }
    [ForeignKey(nameof(InquilinoId))]
    public Inquilino? Inquilino { get ; set ; }
}