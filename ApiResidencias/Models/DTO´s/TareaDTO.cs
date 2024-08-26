namespace ApiResidencias.Models.DTO_s
{
    public class TareaDTO
    {
        public int IdTarea { get; set; }

        public string NombreTarea { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public DateTime FechaVencimiento { get; set; }

        public string? EstadoTarea { get; set; } = null!;

        public int IdAlumno { get; set; }

        public DateTime FechaEntrega { get; set; }

        public string Ruta { get; set; } = "";
    }
}
