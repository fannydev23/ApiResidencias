using System;
using System.Collections.Generic;

namespace ApiResidencias.Models.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Contrasena { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public int IdTipoUsuario { get; set; }

    public virtual ICollection<Alumno> Alumno { get; set; } = new List<Alumno>();

    public virtual ICollection<Coordinador> Coordinador { get; set; } = new List<Coordinador>();

    public virtual Tipousuario IdTipoUsuarioNavigation { get; set; } = null!;
}
