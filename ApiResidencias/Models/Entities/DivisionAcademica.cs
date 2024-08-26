using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class DivisionAcademica
{
    public int IdDivisionAcademica { get; set; }

    public string Nombre { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public int IdCoordinador { get; set; }

    public string? Correo { get; set; }

    public virtual ICollection<Alumno> Alumno { get; set; } = new List<Alumno>();

    public virtual ICollection<Coordinador> Coordinador { get; set; } = new List<Coordinador>();

    public virtual Coordinador IdCoordinadorNavigation { get; set; } = null!;
}
