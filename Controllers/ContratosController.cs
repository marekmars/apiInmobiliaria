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
public class ContratosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;

    private readonly IWebHostEnvironment environment;

    //==========================================
    public ContratosController(DataContext context, IConfiguration config, IWebHostEnvironment env)
    {
        _context = context;
        this.config = config;
        environment = env;
    }
    //==========================================
[HttpGet("{idInmueble}")]
	[Authorize]
	public async Task<IActionResult> getContrato(int idInmueble)// id es de la propiedad
	{
		try{
			var usuario = User.Identity.Name;
			if(usuario == null) return Unauthorized("Token no válido");
			var user =await _context.Propietarios.SingleOrDefaultAsync(x => x.Correo == usuario);
			var fecha = DateTime.Today;

			var inmueble = _context.Contratos
				.Include(e => e.Inmueble)
				.ThenInclude(i => i.Propietario)
				.Where(e => e.Inmueble.Propietario.Id == user.Id)
				.Where(e=> e.Estado==true && e.FechaInicio<=fecha && e.FechaFin>=fecha)
				.Single(e => e.InmuebleId == idInmueble);

			return Ok(_context.Contratos.Include(e => e.Inquilino).SingleOrDefault(e => e.Id == inmueble.Id));
		}catch(Exception e){
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