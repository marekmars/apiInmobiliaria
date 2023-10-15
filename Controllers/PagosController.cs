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
public class PagosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;

    private readonly IWebHostEnvironment environment;

    //==========================================
    public PagosController(DataContext context, IConfiguration config, IWebHostEnvironment env)
    {
        _context = context;
        this.config = config;
        environment = env;
    }
    //==========================================
[HttpGet("{idContrato}")]
	[Authorize]
	public async Task<IActionResult> GetPagos(int idContrato) // id es del contrato
	{
		try{
			var usuario = User.Identity.Name;
			if(usuario == null) return Unauthorized("Token no válido");
			return Ok(_context.Pagos.Include(e => e.Contrato).Where(e => e.IdContrato == idContrato).ToArray());
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