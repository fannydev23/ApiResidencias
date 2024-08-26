namespace ApiResidencias.Models.DTO_s
{
    public class UsuarioCoordinadorDTO
    {
        public string Contrasena { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public int IdDivision { get; set; }
    }
}
