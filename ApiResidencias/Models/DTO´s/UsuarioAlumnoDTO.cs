namespace ApiResidencias.Models.DTOs
{
    public class UsuarioAlumnoDTO
    {
        public string Contrasena { get; set; } = "";
        public string Correo { get; set; } = "";
        public string NumeroControl { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string APaterno { get; set; } = "";
        public string AMaterno { get; set; } = "";
        public int DivisionAcademica { get; set;}
    }
}
