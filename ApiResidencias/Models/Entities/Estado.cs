using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class Estado
{
    public int Id { get; set; }

    public string Estado1 { get; set; } = null!;

    public virtual ICollection<AlumnoTarea> AlumnoTarea { get; set; } = new List<AlumnoTarea>();
}
