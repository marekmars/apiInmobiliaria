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
public class InquilinosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;

    private readonly IWebHostEnvironment environment;

    //==========================================
    public InquilinosController(DataContext context, IConfiguration config, IWebHostEnvironment env)
    {
        _context = context;
        this.config = config;
        environment = env;
    }
    //==========================================
    
[HttpGet("inquiloActual/{inmuebleId}")]
[Authorize]
public async Task<IActionResult> GetInquilinoActivoPorInmueble(int inmuebleId)
{
    try
    {
        var usuario = User.Identity.Name;
        if (usuario == null) return Unauthorized("Token incorrecto");
        var user = await _context.Propietarios.SingleOrDefaultAsync(x => x.Correo == usuario);
        var fecha = DateTime.Today;

        var contratoActivo = await _context.Contratos
            .Include(c => c.Inquilino)
            .FirstOrDefaultAsync(c =>
                c.InmuebleId == inmuebleId &&
                c.Inmueble.Propietario.Id == user.Id &&
                c.Estado == true &&
                c.FechaInicio <= fecha &&
                c.FechaFin >= fecha);

        if (contratoActivo != null)
        {
            var inquilinoActivo = contratoActivo.Inquilino;
            return Ok(inquilinoActivo);
        }
        else
        {
            return NotFound("No hay inquilino activo para este inmueble.");
        }
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