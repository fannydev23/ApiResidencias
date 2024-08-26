using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class Tarea
{
    public int IdTarea { get; set; }

    public string NombreTarea { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaVencimiento { get; set; }

    public virtual ICollection<AlumnoTarea> AlumnoTarea { get; set; } = new List<AlumnoTarea>();
}
