namespace ApiResidencias.Models.DTO_s
{
    public enum TipoUsuario { Administrador = 1, Alumno = 2, Coordinador=3 }
    public class AlumnoDTO
    {
        public int IdAlumno { get; set; }

        public string Nombre { get; set; } = "";
        public string APaterno { get; set; } = "";

        public string AMaterno { get; set; } = "";

        public string NumeroControl { get; set; } = "";

        public string Correo { get; set; } = "";
        public string DivisionAcademica { get; set; } = "";
        public int IdDivisionAcademica { get; set; }
        public int IdUsuario { get; set; }

       
    }
}
