
using System.ComponentModel.DataAnnotations.Schema;
namespace Api_Inmobiliaria.Models;

public class Inmueble
{
        public int Id { get; set; }
        public string? Lat { get; set; }
        public string? Lon { get; set; }
        public int? Uso { get; set; }
        public int? Tipo { get; set; }
        public int? Ambientes { get; set; }
        public bool Disponible { get; set; }
        public string? Direccion { get; set; }
        public double Precio { get; set; }
        public string? Foto { get; set; }
        public int PropietarioId { get; set; }
        [ForeignKey(nameof(PropietarioId))]
        public Propietario? Propietario { get; set; }

}
public enum TipoInmueble
{
        Casa = 1,
        Apartamento = 2,
        Oficina = 3,
        LocalComercial = 4,
        Terreno = 5,
        Otro = 6
}

public enum UsoInmueble
{
        Residencial = 1,
        Comercial = 2
}
