using ApiResidencias.Models.DTO_s;
using ApiResidencias.Models.Entities;
using ApiResidencias.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiResidencias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DivisionController : ControllerBase
    {
        Repository<Coordinador> coordinadorRepository;
        public DivisionController(residenciasContext context)
        {
            coordinadorRepository = new(context);
        }

        [HttpGet]
        public IActionResult GetCoordinadores()
        {
            try
            {
                var coordinadores = coordinadorRepository.Get().Include(x => x.IdDivisionNavigation).OrderBy(x => x.IdDivisionNavigation.Nombre)
                    .Select(x => new
                    DivisionAcademicaDTO()
                    {
                        IdDivisionAcademica = x.IdDivision,
                        Coordinador = x.Nombre,
                        Correo = x.IdDivisionNavigation.Correo ?? "",
                        Nombre = x.IdDivisionNavigation.Nombre
                    });
                return Ok(coordinadores);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
