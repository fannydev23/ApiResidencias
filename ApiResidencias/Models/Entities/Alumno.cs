using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class Alumno
{
    public int IdAlumno { get; set; }

    public string Nombre { get; set; } = null!;

    public string APaterno { get; set; } = null!;

    public string AMaterno { get; set; } = null!;

    public string NumeroControl { get; set; } = null!;

    public bool? Activo { get; set; }

    public int IdDivisionAcademica { get; set; }

    public int IdUsuario { get; set; }

    public virtual ICollection<AlumnoTarea> AlumnoTarea { get; set; } = new List<AlumnoTarea>();

    public virtual DivisionAcademica IdDivisionAcademicaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
