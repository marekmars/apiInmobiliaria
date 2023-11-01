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
            var usuario = User.Identity.Name;
             if (usuario == null) return Unauthorized("Token no válido");
            var inmueble = await _context.Inmuebles
                .Include(e => e.Propietario)
                .SingleOrDefaultAsync(e => e.Id == id);
                

            if (inmueble != null)
            {
                 if (inmueble.Propietario.Correo != usuario) return Unauthorized("Acceso denegado");
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

            var inmuebles = await _context.Contratos
               .Include(e => e.Inmueble)
               .ThenInclude(i => i.Propietario)
               .Where(e => e.Inmueble.Propietario.Id == user.Id)
               .Where(e => e.Estado == true && e.FechaInicio <= fecha && e.FechaFin >= fecha)
               .Select(e => e.Inmueble)
               .ToListAsync();
            Console.WriteLine("COUNT: " + inmuebles.Count);
            return Ok(inmuebles);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //==========================================
    [HttpPost("crear")]
    public async Task<IActionResult> CrearInmueble([FromBody] Inmueble inmueble)
    {
        try
        {
            Console.WriteLine("FOTO: " + inmueble.Foto);
            var usuario = User.Identity.Name;
            if (usuario == null) return Unauthorized("Token incorrecot");
            var user = await _context.Propietarios.SingleOrDefaultAsync(x => x.Correo == usuario);
            Inmueble inmuebleF = inmueble;
            inmueble.PropietarioId = user.Id;
            _context.Inmuebles.Add(inmueble);
            _context.SaveChanges(); // Guarda los cambios en la base de datos

            string nombreFoto = $"img_inmueble_{user.Id}_{inmueble.Id}.jpg";

           if (inmuebleF.Foto.Contains(","))
            {
                inmuebleF.Foto = inmuebleF.Foto.Split(',')[1];
            }

            // Convierte la cadena base64 en bytes
            byte[] imageBytes = Convert.FromBase64String(inmuebleF.Foto);

            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads","inmuebles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = nombreFoto;
            string pathCompleto = Path.Combine(path, fileName);
            // inmueble.Foto = Path.Combine("/Uploads", fileName);


            // Crea una memoria en la secuencia de bytes
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                // Crea una imagen a partir de la secuencia de bytes
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                image.Save(pathCompleto, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            inmueble.Foto = $"uploads/inmuebles/{nombreFoto}";
            _context.Update(inmueble);

            await _context.SaveChangesAsync();

            return Ok(inmueble);
        }
        catch (Exception ex)
        {
            return BadRequest("Error al convertir la cadena base64 a imagen: " + ex.Message);
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