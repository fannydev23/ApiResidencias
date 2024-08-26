using ApiResidencias.Helpers;
using ApiResidencias.Models.DTO_s;
using ApiResidencias.Models.Entities;
using ApiResidencias.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiResidencias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        Repository<Usuario> usuarioRepository;
        Repository<Alumno> alumnoRepository;
        Repository<DivisionAcademica> divisionRepository;
        Repository<Coordinador> coordinadorRepository;
        Cifrado cf;
        public LoginController(residenciasContext residenciasContext)
        {
            usuarioRepository = new(residenciasContext);
            alumnoRepository = new(residenciasContext);
            divisionRepository = new(residenciasContext);
            coordinadorRepository = new(residenciasContext);
            cf = new Cifrado();
        }

        [HttpPost]
        public IActionResult Login(LoginDTO loginDTO)
        {
            try
            {
                var usuario = usuarioRepository.Get().FirstOrDefault(x => x.Correo.Trim().ToUpper()== loginDTO.Usuario.Trim().ToUpper() 
                && x.Contrasena == cf.CifradoTexto(loginDTO.Contraseña));
                //checar si existe en la bd
                if (usuario != null)
                {


                    //List<Claim> cliams = new()
                    //{
                    //    new Claim("Id","100"),
                    //    new Claim(ClaimTypes.Name,loginDTO.Usuario),
                    //    new Claim(ClaimTypes.Role,"Admin"),
                    //    new Claim("Carrera", "Sistemas")
                    //};

                    List<Claim> cliams = GetClaimsByTypeUser(usuario);


                    SecurityTokenDescriptor tokenDescriptor = new()
                    {
                        Issuer = "residenciasTEC.sistemas19.com",
                        Audience = "residenciatec",
                        IssuedAt = DateTime.UtcNow,
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TuMiChiquitita83_"))
                                , SecurityAlgorithms.HmacSha256),
                        Subject = new ClaimsIdentity(cliams, JwtBearerDefaults.AuthenticationScheme)
                    };

                    JwtSecurityTokenHandler handler = new();
                    var token = handler.CreateToken(tokenDescriptor);

                    return Ok(handler.WriteToken(token));
                }
                else
                    return Unauthorized("Nombre de usuario o contraseña incorrectas.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        private List<Claim> GetClaimsByTypeUser(Usuario usuario)
        {
            List<Claim> claims;

            if (usuario.IdTipoUsuario == 1)
            {
                claims = new()
                {
                    new Claim("IdUsuario",usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Role,"Admin"),
                    new Claim(ClaimTypes.Email, usuario.Correo)
                };
            }
            else if (usuario.IdTipoUsuario == 2)
            {
                var coordinador = coordinadorRepository.Get().Include(x => x.IdDivisionNavigation).FirstOrDefault(x => x.IdUsuario == usuario.IdUsuario);

                claims = new()
                {
                    new Claim("IdUsuario",usuario.IdUsuario.ToString()),
                    new Claim("Id", coordinador.IdCoordinador.ToString()),
                    new Claim(ClaimTypes.Name, coordinador.Nombre),
                    new Claim(ClaimTypes.Role,"Coordinador"),
                    new Claim("Carrera", coordinador.IdDivisionNavigation.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                   new Claim("IdCarrera", coordinador.IdDivision.ToString())
                };
            }
            else // sino es 3 que es alumno
            {
                var alumno = alumnoRepository.Get().Include(x=>x.IdDivisionAcademicaNavigation).FirstOrDefault(x => x.IdUsuario == usuario.IdUsuario);

                claims = new()
                {
                    new Claim("IdUsuario",usuario.IdUsuario.ToString()),
                    new Claim("Id", alumno.IdAlumno.ToString()),
                    new Claim(ClaimTypes.Name, $"{alumno.Nombre} {alumno.APaterno} {alumno.AMaterno}"),
                    new Claim(ClaimTypes.Role,"Alumno"),
                    new Claim("Carrera", alumno.IdDivisionAcademicaNavigation.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim("NunControl",alumno.NumeroControl)
                };
            }

            return claims;
        }
    }
}
