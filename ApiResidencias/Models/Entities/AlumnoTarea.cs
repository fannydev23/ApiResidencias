using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class AlumnoTarea
{
    public int Id { get; set; }

    public int IdTarea { get; set; }

    public int IdAlumno { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public int Estado { get; set; }

    public virtual Estado EstadoNavigation { get; set; } = null!;

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;

    public virtual Tarea IdTareaNavigation { get; set; } = null!;
}
