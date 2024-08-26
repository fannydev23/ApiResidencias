using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class Coordinador
{
    public int IdCoordinador { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }

    public int IdUsuario { get; set; }

    public int IdDivision { get; set; }

    public virtual ICollection<DivisionAcademica> DivisionAcademica { get; set; } = new List<DivisionAcademica>();

    public virtual DivisionAcademica IdDivisionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
