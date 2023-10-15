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
public class PropietariosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;

    private readonly IWebHostEnvironment environment;


    public PropietariosController(DataContext context, IConfiguration config, IWebHostEnvironment env)
    {
        _context = context;
        this.config = config;
        environment = env;
    }

    [HttpGet("user")]
    [Authorize]

    public async Task<ActionResult<Propietario>> GetUser() // devuelve el propietario logueado
    {
        try
        {
            var usuario = User.Identity?.Name;
            if (usuario == null) return Unauthorized("Token no válido");
            var dbUser = await _context.Propietarios.SingleOrDefaultAsync(x => x.Correo == usuario);
            if (dbUser == null) return BadRequest("El usuario no existe");
            return dbUser;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("editar")]
    [Authorize]
    public async Task<IActionResult> Editar([FromForm] Propietario propietario)
    {
        try
        {
            var usuario = User.Identity?.Name;
            if (usuario == null) return Unauthorized("Token incorrecto");
            var dbUser = await _context.Propietarios.SingleOrDefaultAsync(x => x.Correo == usuario);
            if (dbUser == null) return BadRequest("No se encontro el usuario");

            dbUser.Nombre = !string.IsNullOrEmpty(propietario.Nombre) ? propietario.Nombre : dbUser.Nombre;
            dbUser.Apellido = !string.IsNullOrEmpty(propietario.Apellido) ? propietario.Apellido : dbUser.Apellido;
            dbUser.Dni = !string.IsNullOrEmpty(propietario.Dni) ? propietario.Dni : dbUser.Dni;
            dbUser.Telefono = !string.IsNullOrEmpty(propietario.Telefono) ? propietario.Telefono : dbUser.Telefono;
            dbUser.Correo = !string.IsNullOrEmpty(propietario.Correo) ? propietario.Correo : dbUser.Correo;

            if (propietario.Clave != null && propietario.Clave != "" && propietario.Clave != dbUser.Clave)
            {
                dbUser.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: propietario.Clave,
                salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            }
            _context.Update(dbUser);
            _context.SaveChanges();
            return Ok(dbUser);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]

    public async Task<IActionResult> Login([FromForm] LoginView loginView) // para loguearse
    {
        try
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginView.Clave,
                salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            ));

            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Correo == loginView.Correo);
            if (p == null || p.Clave != hashed)
            {
                return BadRequest("Nombre de usuario o clave incorrectos");
            }
            else
            {
                var key = new SymmetricSecurityKey(
                    System.Text.Encoding.ASCII.GetBytes(
                        config["TokenAuthentication:SecretKey"]
                    )
                );
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, p.Correo),
                    new Claim("FullName", p.Nombre + " " + p.Apellido),
                    // new Claim(ClaimTypes.Role, "Propietario")
                };

                var token = new JwtSecurityToken(
                    issuer: config["TokenAuthentication:Issuer"],
                    audience: config["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credenciales
                );

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("email")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByEmail([FromForm] string email)
    {
        try
        { //método sin autenticar, busca el propietario x email
            Console.WriteLine($"Email: {email}");
            var propietario = await _context.Propietarios.FirstOrDefaultAsync(x => x.Correo == email);
            var link = "";
            string localIPv4 = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                 ?.ToString();
            var dominio = environment.IsDevelopment() ? localIPv4 : "www.misitio.com";

            if (propietario != null)
            {
                var key = new SymmetricSecurityKey(
                                   System.Text.Encoding.ASCII.GetBytes(
                                       config["TokenAuthentication:SecretKey"]
                                   )
                               );
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, propietario.Correo),
                    new Claim("FullName", propietario.Nombre + " " + propietario.Apellido),
                };

                var token = new JwtSecurityToken(
                    issuer: config["TokenAuthentication:Issuer"],
                    audience: config["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: credenciales
                );


                link = $"https://{dominio}:5001/api/Propietarios/token?access_token={new JwtSecurityTokenHandler().WriteToken(token)}";


                Console.WriteLine(link);

                string subject = "Pedido de REcuperacion de Contraseña";
                string body = @$"<html>
                <body>
                    <h1>Recuperación de Contraseña</h1>
                    <p>Estimado {propietario.Nombre},</p>
                    <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                    <p>Por favor, haz clic en el siguiente enlace para crear una nueva contraseña:</p>
                    <p><a href='{link}'>Restablecer Contraseña</a></p>
                    <p>Si no solicitaste el restablecimiento de contraseña, puedes ignorar este correo electrónico.</p>
                    <p>Este enlace expirará en 24 horas por motivos de seguridad.</p>
                    <p>Atentamente,</p>
                    <p>Tu equipo de soporte</p>
                </body>
            </html>";

                await enviarMail(email, subject, body);

                return Ok();
            }
            else
            {
                return BadRequest("Nombre de usuario o clave incorrectos");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("ERRRROR");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("token")]
    [Authorize]
    public async Task<IActionResult> Token()
    {
        try
        {
            var perfil = new
            {
                Email = User.Identity?.Name,
                Nombre = User.Claims.First(x => x.Type == "FullName").Value,
            };
            Console.WriteLine("ASDASD0" + perfil.Nombre);
            Random rand = new Random(Environment.TickCount);
            string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            string nuevaClave = "";
            for (int i = 0; i < 8; i++)
            {
                nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
            }

            string subject = "Nueva Clave de Ingreso";
            string body = @$"<html>
                <body>
                    <h1>Recuperación de Contraseña</h1>
                    <p>Estimado {perfil.Nombre},</p>
                    <p>Hemos generado una nueva contraseña para tu cuenta.</p>
                    <p>Tu nueva contraseña es: <strong>{nuevaClave}</strong></p>
                    <p>Por favor, inicia sesión con esta nueva contraseña y cámbiala lo antes posible.</p>
                    <p>Si no solicitaste un cambio de contraseña, por favor contáctanos de inmediato.</p>
                    <p>Atentamente,</p>
                    <p>Tu equipo de soporte</p>
                </body>
            </html>";
            await enviarMail(perfil.Email, subject, body);

            var propietario = await _context.Propietarios.FirstOrDefaultAsync(x => x.Correo == perfil.Email);

            if (propietario != null)
            {
                propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: nuevaClave,
                salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
                _context.Update(propietario);
                _context.SaveChanges();
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


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
    [HttpGet("sendEmail")]
    private async Task<IActionResult> enviarMail(string email, string subject, string body)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Sistema", config["SMTPUser"]));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = body, };

        using (var client = new SmtpClient())
        {
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.Auto);
            await client.AuthenticateAsync(config["SMTPUser"], config["SMTPPass"]);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
        return Ok();
    }

}