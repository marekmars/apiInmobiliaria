namespace Api_Inmobiliaria.Models;

public class Propietario
{ // id apellido nombre dni telefono correo estado Agregar clave a DB
    public int Id { get; set; }
    public string? Apellido { get; set; }
    public string? Nombre { get; set; }
    public string? Dni { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? Avatar {get; set; }
    public string? Clave { get; set; }
    
}