using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Api_Inmobiliaria.Models;
using MailKit.Net.Smtp;
using MimeKit;


namespace ApiInmobiliarias.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InmueblesController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;

    private readonly IWebHostEnvironment environment;

    //==========================================
    public InmueblesController(DataContext context, IConfiguration config, IWebHostEnvironment env)
    {
        _context = context;
        this.config = config;
        environment = env;
    }
    //==========================================
    [HttpGet("propiedadesUsuario")]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        try
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("Token no válido o usuario no identificado");
            }

            var propiedades = _context.Inmuebles
                .Include(e => e.Propietario)
                .Where(e => e.Propietario.Correo == usuario)
                .ToList(); // Materializa la consulta

            if (propiedades.Any())
            {
                return Ok(propiedades);
            }
            else
            {
                return NotFound("No se encontraron propiedades para el usuario.");
            }
        }
        catch (Exception e)
        {
            return BadRequest("Error en la solicitud: " + e.Message);
        }
    }

    //==========================================
    [HttpPut("toogleEstado/{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id)
    {
        try
        {

            Console.WriteLine("Id: " + id);
            var usuario = User.Identity.Name;
            if (usuario == null) return Unauthorized("Token no válido");
            var inmueble = _context.Inmuebles.Include(e => e.Propietario).SingleOrDefault(e => e.Id == id);
            if (inmueble == null) return NotFound();
            if (inmueble.Propietario.Correo != usuario) return Unauthorized("Acceso denegado");
            inmueble.Disponible = !inmueble.Disponible;
            _context.Update(inmueble);

            await _context.SaveChangesAsync();


            return Ok(inmueble);
        }
        catch (Exception e)
        {
            // Manejo de errores
            return BadRequest(e.Message);
        }
    }

    //==========================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetInmueble(int id)
    {
        try
        {
            var inmueble = await _context.Inmuebles
                .Include(e => e.Propietario)
                .SingleOrDefaultAsync(e => e.Id == id);

            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            else
            {
                return NotFound("Inmueble no encontrado");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //==========================================

    //  [HttpGet("{id}/foto")]
    //  [AllowAnonymous]
    //     public IActionResult GetInmuebleFoto(int id)
    //     {
    //         var inmueble = _context.Inmuebles.FirstOrDefault(i => i.Id == id);

    //         if (inmueble == null)
    //         {
    //             return NotFound();
    //         }
    //         var imageName=inmueble.Foto;
    //         var wwwPath = environment.WebRootPath;
    //         var path = Path.Combine(wwwPath, imageName);

    //         if (System.IO.File.Exists(path))
    //         {
    //             Console.WriteLine("ENTRO AFOTO");
    //             var imageBytes = System.IO.File.ReadAllBytes(path);
    //             var mimeType = GetMimeType(imageName); // Obtén el tipo MIME de la imagen.
    //             return File(imageBytes, mimeType);
    //         }else{
    //             return NotFound();
    //         }

    //         // Si tienes la URL de la foto en la propiedad FotoUrl, puedes devolverla como respuesta

    //     }
    //      private string GetMimeType(string fileName)
    //     {
    //         // Implementa la lógica para determinar el tipo MIME de la imagen basado en su extensión.
    //         // Por ejemplo, puedes usar la extensión del archivo para determinar el tipo MIME.
    //         // Asegúrate de manejar diferentes tipos de imágenes, como JPEG, PNG, etc.

    //         // Ejemplo:
    //         if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
    //         {
    //             return "image/jpeg";
    //         }
    //         else if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
    //         {
    //             return "image/png";
    //         }

    //         // Devuelve un tipo MIME predeterminado si no se puede determinar.
    //         return "application/octet-stream";
    //     }

    //==========================================

    [HttpGet("alquiladas")]
    [Authorize]
    public async Task<IActionResult> GetAlquiladas()
    {
        try
        {
            var usuario = User.Identity.Name;
            if (usuario == null) return Unauthorized("Token incorrecot");
            var user = await _context.Propietarios.SingleOrDefaultAsync(x => x.Correo == usuario);
            var fecha = DateTime.Today;

            var contrato = await _context.Contratos.SingleOrDefaultAsync();
            Console.WriteLine($"fechaini: {contrato.FechaInicio} fechafin: {contrato.FechaFin}");
            var inmuebles = await _context.Contratos
               .Include(e => e.Inmueble)
               .ThenInclude(i => i.Propietario)
               .Where(e => e.Inmueble.Propietario.Id == user.Id)
               .Where(e => e.Estado == true && e.FechaInicio <= fecha && e.FechaFin >= fecha)
               .Select(e => e.Inmueble)
               .ToListAsync();
    
            return Ok(inmuebles);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //==========================================
    [HttpGet("test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        try
        {
            return Ok("anduvo");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


}