namespace ApiResidencias.Models.DTO_s
{
    public class AlumnoTareaDTO
    {
        public int IdAlumnno { get; set; }
        public string NumeroControl { get; set; } = "";
        public string Nombre { get; set; } = "";
        public List<TareaDTO> Tareas { get; set; } = new();
    }
}
