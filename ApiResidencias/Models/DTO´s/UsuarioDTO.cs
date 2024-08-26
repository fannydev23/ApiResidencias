namespace ApiResidencias.Models.DTO_s
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }

        public string Contrasena { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public int IdTipoUsuario { get; set; }
    }
}
