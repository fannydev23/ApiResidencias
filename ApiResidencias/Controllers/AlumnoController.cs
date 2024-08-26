using ApiResidencias.Models.DTO_s;
using ApiResidencias.Models.Entities;
using ApiResidencias.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiResidencias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlumnoController : ControllerBase
    {
        Repository<Alumno> alumnoRepository;
        Repository<DivisionAcademica> divisionRepository;
        Repository<Usuario> usuarioRepository;
        public AlumnoController(residenciasContext context)
        {
            alumnoRepository = new(context);
            divisionRepository = new(context);
            usuarioRepository = new(context);
        }
        #region Get
        [HttpGet] // Se trae a todos los alumnos vigentes
        public IActionResult Get()
        {
            var alumnos = alumnoRepository.Get()
                .Include(x => x.IdUsuarioNavigation)
                .Include(x => x.IdDivisionAcademicaNavigation)
                .Where(x => x.Activo == true)
                .Select(x => new AlumnoDTO
                {
                    IdAlumno = x.IdAlumno,
                    Nombre = x.Nombre,
                    AMaterno = x.AMaterno,
                    APaterno = x.APaterno,
                    NumeroControl = x.NumeroControl,
                    IdUsuario = x.IdUsuario,
                    Correo = x.IdUsuarioNavigation.Correo,
                    DivisionAcademica = x.IdDivisionAcademicaNavigation.Nombre,
                }).OrderBy(x => x.Nombre).ToList();
            return Ok(alumnos);
        }
        [HttpGet("carrera/{clave}")] // Filtro de carrera
        public IActionResult Get(int clave) 
        {
            var division = divisionRepository.Get(clave);
            if (division == null)
                return BadRequest("Division no encontrada");

            DivisionAlumnoDTO divisionAlumnoDTO = new();
            divisionAlumnoDTO.NombreDivision = division.Nombre;

            var alumnos = alumnoRepository.Get()
                .Include(x => x.IdUsuarioNavigation)
                .Include(x => x.IdDivisionAcademicaNavigation)
                .Where(x => x.Activo == true && x.IdDivisionAcademica == clave)
                .Select(x => new AlumnoDTO
                {
                    IdAlumno=x.IdAlumno,
                    Nombre = x.Nombre,
                    AMaterno = x.AMaterno,
                    APaterno = x.APaterno,
                    NumeroControl = x.NumeroControl,
                    IdUsuario = x.IdUsuario,
                    Correo =x.IdUsuarioNavigation.Correo,
                    DivisionAcademica = x.IdDivisionAcademicaNavigation.Nombre,
                    IdDivisionAcademica = x.IdDivisionAcademica
                }).OrderBy(x => x.Nombre);
            divisionAlumnoDTO.Alumnos = alumnos;
            return Ok(divisionAlumnoDTO);
        }
        //Se trae a las divisiones
        [HttpGet("divisiones/All")]
        public IActionResult GetCarreras()
        {
            var divisiones = divisionRepository.Get()
                
                .Select(x => new DivisionAcademicaDTO
                {
                    IdDivisionAcademica=x.IdDivisionAcademica,
                    Nombre = x.Nombre,

                })
                .Distinct()
                .ToList();
            return Ok(divisiones);
        }

        [HttpGet("info")]
        public IActionResult GetAlumno()
        {
            var idalumno = User.FindFirst("Id").Value;
            var id = 0;
            if (!string.IsNullOrWhiteSpace(idalumno))
                id = int.Parse(idalumno);
            else
                return BadRequest("Ha ocurrido un error, credenciales incorrectas");

            var alumno = alumnoRepository.Get().Include(x=>x.IdUsuarioNavigation).FirstOrDefault(x=>x.IdAlumno==id);
            if (alumno == null)
                return NotFound();

            var a = new AlumnoDTO()
            {
                IdAlumno = alumno.IdAlumno,
                Nombre = alumno.Nombre + alumno.APaterno + alumno.AMaterno,
                NumeroControl = alumno.NumeroControl,
                Correo = alumno.IdUsuarioNavigation.Correo
            };

            return Ok(a);
        }

        [HttpGet("{id}")]
        public IActionResult GetAlumnoById(int id)
        {

            var alumno = alumnoRepository.Get().Include(x => x.IdUsuarioNavigation).FirstOrDefault(x => x.IdAlumno == id);
            if (alumno == null)
                return NotFound();

            var a = new AlumnoDTO()
            {
                IdAlumno = alumno.IdAlumno,
                Nombre = alumno.Nombre,
                APaterno = alumno.APaterno,
                AMaterno= alumno.AMaterno,
                IdDivisionAcademica = alumno.IdDivisionAcademica,
                NumeroControl = alumno.NumeroControl,
                Correo = alumno.IdUsuarioNavigation.Correo,
                IdUsuario = alumno.IdUsuario
            };

            return Ok(a);
        }

        #endregion
        #region Post
        [HttpPost]
        public IActionResult Post(AlumnoDTO alumno) 
        {
            try
            {
                //VALIDACION
                if (string.IsNullOrWhiteSpace(alumno.Nombre)) 
                {
                    return BadRequest("El nombre no debe ir vacio");
                }
                if (string.IsNullOrWhiteSpace(alumno.APaterno))
                {
                    return BadRequest("El apellido paterno no debe ir vacio");
                }

                if (string.IsNullOrWhiteSpace(alumno.AMaterno))
                {
                    return BadRequest("El apellido materno no debe ir vacio");
                }

                if (string.IsNullOrWhiteSpace(alumno.NumeroControl))
                {
                    return BadRequest("El numero de control no debe ir vacio");
                }
                if (alumnoRepository.Get().FirstOrDefault(x => x.NumeroControl.Trim().ToUpper() == alumno.NumeroControl) != null)
                    return BadRequest("Ya hay un alumno con ese numero de control vinculado");
                //HACEMOS UN ALUMNOS
                Alumno a = new()
                {
                    IdAlumno = 0,
                    NumeroControl = alumno.NumeroControl.ToUpper(),
                    Nombre = alumno.Nombre.ToUpper(),
                    APaterno = alumno.APaterno.ToUpper(),
                    AMaterno = alumno.AMaterno.ToUpper(),
                    Activo = true,
                    IdUsuario = alumno.IdUsuario,
                    IdDivisionAcademica = alumno.IdDivisionAcademica
                   
                };
                alumnoRepository.Insert(a);

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        #endregion
        #region Put
        [HttpPut]
        public IActionResult Put(AlumnoDTO alumno)
        {
            try
            {
                //VALIDACION
                if (string.IsNullOrWhiteSpace(alumno.Nombre))
                {
                    return BadRequest("El nombre no debe ir vacio");
                }
                if (string.IsNullOrWhiteSpace(alumno.APaterno))
                {
                    return BadRequest("El apellido paterno no debe ir vacio");
                }

                if (string.IsNullOrWhiteSpace(alumno.AMaterno))
                {
                    return BadRequest("El apellido materno no debe ir vacio");
                }

                if (string.IsNullOrWhiteSpace(alumno.NumeroControl))
                {
                    return BadRequest("El numero de control no debe ir vacio");
                }

                if(string.IsNullOrWhiteSpace(alumno.Correo))
                    return BadRequest("El correo no debe ir vacio");

                var al = alumnoRepository.Get().FirstOrDefault(x => x.IdAlumno != alumno.IdAlumno &&
                x.NumeroControl.Trim().ToUpper() == alumno.NumeroControl);
                if (al != null)
                    return BadRequest("Ya hay un alumno con ese numero de control vinculado");

                if (!divisionRepository.Get().Any(x => x.IdDivisionAcademica == alumno.IdDivisionAcademica))
                    return BadRequest("No se encontro la division academica");

                var alumnos = alumnoRepository.Get()
                    .Include(x => x.IdUsuarioNavigation);

                var a = alumnos.FirstOrDefault(x=>x.IdAlumno==alumno.IdAlumno);

                if (usuarioRepository.Get().Any(x=>x.Correo.Trim() == alumno.Correo.Trim() && x.IdUsuario!=a.IdUsuario))
                    return BadRequest("Ya hay un alumno con ese correo registrado");

                if (a!=null)
                {

                    a.NumeroControl = alumno.NumeroControl.ToUpper();
                    a.Nombre = alumno.Nombre.ToUpper();
                    a.APaterno = alumno.APaterno.ToUpper();
                    a.AMaterno = alumno.AMaterno.ToUpper();
                    a.IdUsuarioNavigation.Correo=alumno.Correo.ToUpper();
                    a.IdDivisionAcademica = alumno.IdDivisionAcademica;
                    alumnoRepository.Update(a);
                }
                else
                    return BadRequest("No se encuentra al alumno");

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [HttpPut("cambiarEstado/{Id}")]
        public IActionResult CambiarEstado(int Id)
        {
            try
            {
                var alumno = alumnoRepository.Get(Id);

                if (alumno == null)
                {
                    return NotFound("Alumno no encontrado");
                }

                // Cambiar el estado de vigencia a false
                alumno.Activo = false;
                alumnoRepository.Update(alumno);

                return Ok("Estado de vigencia actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.InnerException);
            }
        }

        [HttpPut("cambiarEstadoActivo/{Id}")]
        public IActionResult ActivarAlumno(int Id)
        {
            try
            {
                var alumno = alumnoRepository.Get(Id);

                if (alumno == null)
                {
                    return NotFound("Alumno no encontrado");
                }

                // Cambiar el estado de vigencia a false
                alumno.Activo = true;
                alumnoRepository.Update(alumno);

                return Ok("Estado de vigencia actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.InnerException);
            }
        }

        [HttpPut("cambiarContra/{Id}")]
        public IActionResult CambiarContra(int Id)
        {
            try
            {
                var alumno = alumnoRepository.Get().Include(x=>x.IdUsuarioNavigation).FirstOrDefault(x=>x.IdAlumno==Id);

                if (alumno == null)
                {
                    return NotFound("Alumno no encontrado");
                }

                

                return Ok("Estado de vigencia actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.InnerException);
            }
        }


        #endregion


        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            try
            {
                var alumno = alumnoRepository.Get(Id);

                if (alumno == null)
                {
                    return NotFound("Alumno no encontrado");
                }

                var usuario = usuarioRepository.Get(alumno.IdUsuario);
                alumnoRepository.Delete(alumno);
                usuarioRepository.Delete(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.InnerException);
            }
        }
    }
}
