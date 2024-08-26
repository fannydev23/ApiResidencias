using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class Tipousuario
{
    public int IdTipoUsuario { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
