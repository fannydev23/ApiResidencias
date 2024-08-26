using System.ComponentModel;

namespace ApiResidencias.Models.DTO_s
{
    public class DivisionAlumnoDTO
    {
        public string NombreDivision { get; set; } = "";
        public IEnumerable<AlumnoDTO>? Alumnos { get; set; }
    }
}
