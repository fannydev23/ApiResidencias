using ApiResidencias.Helpers;
using ApiResidencias.Models.DTO_s;
using ApiResidencias.Models.Entities;
using ApiResidencias.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace ApiResidencias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TareaController : ControllerBase
    {
        Repository<Tarea> tareaRepository;
        Repository<Alumno> alumnoRepository;
        Repository<AlumnoTarea> alumnoTareaRepository;
        string rootpath = "";
        public TareaController(residenciasContext context, IWebHostEnvironment hostEnvironment)
        {
            alumnoTareaRepository = new(context);
            tareaRepository = new(context);
            alumnoRepository = new(context);
            rootpath = hostEnvironment.WebRootPath;
        }

        #region Get

        [HttpGet("individual/{id}")]
        public IActionResult GetTarea(int id)
        {
            try
            {

                var tareaExistente = alumnoTareaRepository.Get().Include(x=>x.IdTareaNavigation).Include(x=>x.EstadoNavigation).
                    FirstOrDefault(x=>x.Id==id);
                if (tareaExistente == null)
                {
                    return BadRequest("No se encontro la tarea");
                }

                var tarea = new TareaDTO()
                {
                    IdTarea = tareaExistente.Id,
                    Descripcion = tareaExistente.IdTareaNavigation.Descripcion,
                    NombreTarea = tareaExistente.IdTareaNavigation.NombreTarea,
                    EstadoTarea = tareaExistente.EstadoNavigation.Estado1,
                    FechaEntrega = tareaExistente.FechaEntrega ?? DateTime.MinValue,
                    FechaVencimiento = tareaExistente.IdTareaNavigation.FechaVencimiento,
                    IdAlumno = tareaExistente.IdAlumno
                };

                // Asignar las rutas en una etapa separada
                tarea.Ruta = GetPDF(tarea.IdTarea);
                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AlumnoTarea/{id}")]
        public IActionResult GetTareas(int id)
        {
            try
            {

                var alumno = alumnoRepository.Get(id);
                if (alumno == null)
                {
                    return BadRequest("No se encontro al alumno");
                }

                AlumnoTareaDTO alumnoTareaDTO = new AlumnoTareaDTO();
                alumnoTareaDTO.IdAlumnno = alumno.IdAlumno;
                alumnoTareaDTO.NumeroControl = alumno.NumeroControl;
                alumnoTareaDTO.Nombre = $"{alumno.Nombre} {alumno.APaterno} {alumno.AMaterno}";

                var tareas = alumnoTareaRepository.Get().Include(x => x.IdAlumnoNavigation).Include(x => x.IdTareaNavigation).Include(x => x.EstadoNavigation)
                    .Where(x => x.IdAlumno == id).OrderBy(x => x.IdTareaNavigation.FechaVencimiento)
                    .Select(x => new TareaDTO()
                    {
                        IdTarea = x.Id,
                        Descripcion = x.IdTareaNavigation.Descripcion,
                        NombreTarea = x.IdTareaNavigation.NombreTarea,
                        EstadoTarea = x.EstadoNavigation.Estado1,
                        FechaEntrega = x.FechaEntrega ?? DateTime.MinValue,
                        FechaVencimiento = x.IdTareaNavigation.FechaVencimiento,
                        IdAlumno = x.IdAlumno
                    }).ToList();

                // Asignar las rutas en una etapa separada
                foreach (var tarea in tareas)
                {
                    tarea.Ruta = GetPDF(tarea.IdTarea);
                }

                alumnoTareaDTO.Tareas = tareas;
                return Ok(alumnoTareaDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Alumno/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var tareas = alumnoTareaRepository.Get().Include(x => x.IdAlumnoNavigation).Include(x => x.IdTareaNavigation).Include(x => x.EstadoNavigation)
                    .Where(x => x.IdAlumno == id).OrderBy(x => x.IdTareaNavigation.FechaVencimiento)
                    .Select(x => new TareaDTO()
                    {
                        IdTarea = x.Id,
                        Descripcion = x.IdTareaNavigation.Descripcion,
                        NombreTarea = x.IdTareaNavigation.NombreTarea,
                        EstadoTarea = x.EstadoNavigation.Estado1,
                        FechaEntrega = x.FechaEntrega ?? DateTime.MinValue,
                        FechaVencimiento = x.IdTareaNavigation.FechaVencimiento,
                        IdAlumno = x.IdAlumno
                    }).ToList();

                // Asignar las rutas en una etapa separada
                foreach (var tarea in tareas)
                {
                    tarea.Ruta = GetPDF(tarea.IdTarea);
                }
                return Ok(tareas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Alumno/Pendientes")]
        public IActionResult GetPendientes()
        {
            try
            {
                ////Encontrar el id del usuario
                var idalumno = User.FindFirst("Id").Value;
                var id = 0;
                if (!string.IsNullOrWhiteSpace(idalumno))
                    id = int.Parse(idalumno);
                else
                    return BadRequest("Ha ocurrido un error, credenciales incorrectas");

                var ahora = DateTime.Now.ToMexicoTime();
                var tareas = alumnoTareaRepository.Get().Include(x => x.IdAlumnoNavigation).Include(x => x.IdTareaNavigation).Include(x => x.EstadoNavigation)
                    .Where(x => x.IdAlumno == id && (x.Estado != 2) && x.IdTareaNavigation.FechaVencimiento >= ahora).OrderBy(x => x.IdTareaNavigation.FechaVencimiento)
                    .Select(x => new TareaDTO()
                    {
                        IdTarea = x.Id,
                        NombreTarea = x.IdTareaNavigation.NombreTarea,
                        EstadoTarea = x.EstadoNavigation.Estado1,
                        FechaEntrega = x.FechaEntrega ?? DateTime.MinValue,
                        FechaVencimiento = x.IdTareaNavigation.FechaVencimiento,
                        IdAlumno = x.IdAlumno
                    }).OrderBy(x=>x.FechaVencimiento).ThenBy(x=>x.NombreTarea);

                // Asignar las rutas en una etapa separada
                foreach (var tarea in tareas)
                {
                    tarea.Ruta = GetPDF(tarea.IdTarea);
                }
                return Ok(tareas);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpGet("Alumno/Vencidas")]
        public IActionResult GetVencidas()
        {
            try
            {
                var idalumno = User.FindFirst("Id").Value;
                var id = 0;
                if (!string.IsNullOrWhiteSpace(idalumno))
                    id = int.Parse(idalumno);
                else
                    return BadRequest("Ha ocurrido un error, credenciales incorrectas");

                var ahora = DateTime.Now.ToMexicoTime();
                var tareas = alumnoTareaRepository.Get().Include(x => x.IdAlumnoNavigation).Include(x => x.IdTareaNavigation).Include(x => x.EstadoNavigation)
                    .Where(x => x.IdAlumno == id && (x.Estado != 2) && x.IdTareaNavigation.FechaVencimiento <= ahora).OrderBy(x => x.IdTareaNavigation.FechaVencimiento)
                    .Select(x => new TareaDTO()
                    {
                        IdTarea = x.Id,
                        NombreTarea = x.IdTareaNavigation.NombreTarea,
                        EstadoTarea = x.EstadoNavigation.Estado1,
                        FechaEntrega = x.FechaEntrega ?? DateTime.MinValue,
                        FechaVencimiento = x.IdTareaNavigation.FechaVencimiento,
                        IdAlumno = x.IdAlumno
                    }).OrderBy(x => x.FechaVencimiento).ThenBy(x => x.NombreTarea);

                // Asignar las rutas en una etapa separada
                foreach (var tarea in tareas)
                {
                    tarea.Ruta = GetPDF(tarea.IdTarea);
                }
                return Ok(tareas);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Alumno/Enviadas")]
        public IActionResult GetEnviadas()
        {
            try
            {
                var idalumno = User.FindFirst("Id").Value;
                var id = 0;
                if (!string.IsNullOrWhiteSpace(idalumno))
                    id = int.Parse(idalumno);
                else
                    return BadRequest("Ha ocurrido un error, credenciales incorrectas");

                var ahora = DateTime.Now.ToMexicoTime();
                var tareas = alumnoTareaRepository.Get().Include(x => x.IdAlumnoNavigation).Include(x => x.IdTareaNavigation).Include(x => x.EstadoNavigation)
                    .Where(x => x.IdAlumno == id && x.Estado == 2).OrderBy(x => x.IdTareaNavigation.FechaVencimiento)
                    .Select(x => new TareaDTO()
                    {
                        IdTarea = x.Id,
                        NombreTarea = x.IdTareaNavigation.NombreTarea,
                        EstadoTarea = x.EstadoNavigation.Estado1,
                        FechaEntrega = x.FechaEntrega ?? DateTime.MinValue,
                        FechaVencimiento = x.IdTareaNavigation.FechaVencimiento,
                        IdAlumno = x.IdAlumno
                    }).OrderByDescending(x => x.FechaVencimiento).ThenByDescending(x => x.NombreTarea);

                // Asignar las rutas en una etapa separada
                foreach (var tarea in tareas)
                {
                    tarea.Ruta = GetPDF(tarea.IdTarea);
                }
                return Ok(tareas);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }



        string GetPDF(int idTarea)
        {
            string host = HttpContext.Request.Host.Value;
            var pdfPath = $"{rootpath}/Tareas/{idTarea}/evidencia.pdf";
            pdfPath = pdfPath.Replace("\\", "/");
            var path = "";
            if (System.IO.File.Exists(pdfPath))
                path = $"https://{host}/Tareas/{idTarea}/evidencia.pdf";
            else
                path = "";

            return path;
        }
        #endregion
        #region Post
        //Admi asigna tarea
        [HttpPost]
        public IActionResult Post(TareaDTO tarea)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tarea.NombreTarea))
                    return BadRequest("El titulo de la tarea no debe ir en blanco");

                if (tarea.FechaVencimiento == DateTime.MinValue)
                    return BadRequest("Fecha no valida");

                if (tarea.FechaVencimiento < DateTime.Now.ToMexicoTime())
                    return BadRequest("La fecha no puede ser menor a la actual");

                var alumnos = alumnoRepository.Get()
                    .Where(x => x.Activo == true).Select(x => x.IdAlumno).ToList();


                Tarea t = new()
                {
                    IdTarea = 0,
                    NombreTarea = tarea.NombreTarea,
                    Descripcion = tarea.Descripcion,
                    FechaVencimiento = tarea.FechaVencimiento
                };

                tareaRepository.Insert(t);
                AlumnoTarea alumnoTarea = new AlumnoTarea();
                foreach (var alumno in alumnos)
                {
                    alumnoTarea = new AlumnoTarea()
                    {
                        Id = 0,
                        IdTarea = t.IdTarea,
                        IdAlumno = alumno,
                        Estado = 3
                    };
                    alumnoTareaRepository.Insert(alumnoTarea);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Put
        [HttpPut("Rechazar/{id}")]
        public IActionResult Rechazar(int id)
        {
            try
            {
                var tarea = alumnoTareaRepository.Get(id);
                if (tarea == null)
                    return BadRequest("Tarea no encontrada");
                tarea.Estado = 5;
                tarea.FechaEntrega = null;
                alumnoTareaRepository.Update(tarea);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Aceptar/{id}")]
        public IActionResult Aceptar(int id)
        {
            try
            {
                var tarea = alumnoTareaRepository.Get(id);
                if (tarea == null)
                    return BadRequest("Tarea no encontrada");
                tarea.Estado = 6;
                alumnoTareaRepository.Update(tarea);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Visto/{id}")]
        public IActionResult Visto(int id)
        {
            try
            {
                var tarea = alumnoTareaRepository.Get(id);
                if (tarea == null)
                    return BadRequest("Tarea no encontrada");
                tarea.Estado = 1;
                alumnoTareaRepository.Update(tarea);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Entregar")]
        public IActionResult Entregar(EntregarDTO tareaDTO)
        {
            try
            {
                if (tareaDTO.IdTarea < 1)
                    return BadRequest("Debe indicar la tarea.");

                //if (tareaDTO.idAlumno < 1)
                //    return BadRequest("Debe indicar el alumno.");

                if (string.IsNullOrWhiteSpace(tareaDTO.PDF))
                    return BadRequest("Debe enviar la evidencia");

                //if (alumnoRepository.Get(tareaDTO.idAlumno) == null)
                //    return BadRequest("Alumno no encontrado");

                var tarea = alumnoTareaRepository.Get(tareaDTO.IdTarea);
                if (tarea == null)
                    return BadRequest("Tarea no encontrada");

                tarea.Estado = 2;
                tarea.FechaEntrega = DateTime.Now.ToMexicoTime();


                var directorio = rootpath + "/Tareas/" + tareaDTO.IdTarea;

                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                var bytesimg = Convert.FromBase64String(tareaDTO.PDF);
                var rutaPDF = $"{directorio}/evidencia.pdf";
                System.IO.File.WriteAllBytes(rutaPDF, bytesimg);



                alumnoTareaRepository.Update(tarea);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
