using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiResidencias.Models.Entities;

public partial class residenciasContext : DbContext
{
    public residenciasContext()
    {
    }

    public residenciasContext(DbContextOptions<residenciasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumno { get; set; }

    public virtual DbSet<AlumnoTarea> AlumnoTarea { get; set; }

    public virtual DbSet<Coordinador> Coordinador { get; set; }

    public virtual DbSet<DivisionAcademica> DivisionAcademica { get; set; }

    public virtual DbSet<Estado> Estado { get; set; }

    public virtual DbSet<Tarea> Tarea { get; set; }

    public virtual DbSet<Tipousuario> Tipousuario { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=myserver;user=myuser;password=mypassword;database=myDB;treattinyasboolean=true", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.20-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.IdAlumno).HasName("PRIMARY");

            entity
                .ToTable("alumno")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdDivisionAcademica, "fk_Alumno_Division");

            entity.HasIndex(e => e.IdUsuario, "fk_Alumno_usuario");

            entity.Property(e => e.IdAlumno)
                .HasColumnType("int(11)")
                .HasColumnName("idAlumno");
            entity.Property(e => e.AMaterno)
                .HasMaxLength(200)
                .HasColumnName("a_materno");
            entity.Property(e => e.APaterno)
                .HasMaxLength(200)
                .HasColumnName("a_paterno");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.IdDivisionAcademica)
                .HasColumnType("int(11)")
                .HasColumnName("idDivision_academica");
            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("idUsuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroControl)
                .HasMaxLength(8)
                .HasColumnName("numeroControl");

            entity.HasOne(d => d.IdDivisionAcademicaNavigation).WithMany(p => p.Alumno)
                .HasForeignKey(d => d.IdDivisionAcademica)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Alumno_Division");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Alumno)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Alumno_usuario");
        });

        modelBuilder.Entity<AlumnoTarea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alumno_tarea");

            entity.HasIndex(e => e.IdAlumno, "fk_aumnoTarea_idx");

            entity.HasIndex(e => e.Estado, "fk_estado_idx");

            entity.HasIndex(e => e.IdTarea, "fk_tarea_idx");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasColumnType("int(11)")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.IdAlumno)
                .HasColumnType("int(11)")
                .HasColumnName("idAlumno");
            entity.Property(e => e.IdTarea)
                .HasColumnType("int(11)")
                .HasColumnName("idTarea");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.AlumnoTarea)
                .HasForeignKey(d => d.Estado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_estado");

            entity.HasOne(d => d.IdAlumnoNavigation).WithMany(p => p.AlumnoTarea)
                .HasForeignKey(d => d.IdAlumno)
                .HasConstraintName("fk_aumno");

            entity.HasOne(d => d.IdTareaNavigation).WithMany(p => p.AlumnoTarea)
                .HasForeignKey(d => d.IdTarea)
                .HasConstraintName("fk_tarea");
        });

        modelBuilder.Entity<Coordinador>(entity =>
        {
            entity.HasKey(e => e.IdCoordinador).HasName("PRIMARY");

            entity
                .ToTable("coordinador")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdDivision, "fk_coordinador_division_idx");

            entity.HasIndex(e => e.IdUsuario, "fk_coordinador_usuario");

            entity.Property(e => e.IdCoordinador)
                .HasColumnType("int(11)")
                .HasColumnName("idCoordinador");
            entity.Property(e => e.Correo)
                .HasMaxLength(60)
                .HasColumnName("correo");
            entity.Property(e => e.IdDivision)
                .HasColumnType("int(11)")
                .HasColumnName("idDivision");
            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("idUsuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDivisionNavigation).WithMany(p => p.Coordinador)
                .HasForeignKey(d => d.IdDivision)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_coordinador_division");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Coordinador)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_coordinador_usuario");
        });

        modelBuilder.Entity<DivisionAcademica>(entity =>
        {
            entity.HasKey(e => e.IdDivisionAcademica).HasName("PRIMARY");

            entity
                .ToTable("division_academica")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdDivisionAcademica, "Uk_coordinadordivision").IsUnique();

            entity.HasIndex(e => e.IdCoordinador, "fk_division_coordinador");

            entity.Property(e => e.IdDivisionAcademica)
                .HasColumnType("int(11)")
                .HasColumnName("idDivision_academica");
            entity.Property(e => e.Clave)
                .HasMaxLength(45)
                .HasColumnName("clave");
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.IdCoordinador)
                .HasColumnType("int(11)")
                .HasColumnName("idCoordinador");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdCoordinadorNavigation).WithMany(p => p.DivisionAcademica)
                .HasForeignKey(d => d.IdCoordinador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_division_coordinador");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("estado");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Estado1)
                .HasMaxLength(45)
                .HasColumnName("Estado");
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.IdTarea).HasName("PRIMARY");

            entity
                .ToTable("tarea")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdTarea)
                .HasColumnType("int(11)")
                .HasColumnName("idTarea");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("datetime")
                .HasColumnName("fechaVencimiento");
            entity.Property(e => e.NombreTarea)
                .HasMaxLength(100)
                .HasColumnName("nombreTarea");
        });

        modelBuilder.Entity<Tipousuario>(entity =>
        {
            entity.HasKey(e => e.IdTipoUsuario).HasName("PRIMARY");

            entity
                .ToTable("tipousuario")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdTipoUsuario).HasColumnType("int(11)");
            entity.Property(e => e.Tipo).HasMaxLength(60);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity
                .ToTable("usuario")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdTipoUsuario, "fkUsuario_TipoUsuario");

            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("idUsuario");
            entity.Property(e => e.Contrasena)
                .HasColumnType("text")
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(60)
                .HasColumnName("correo");
            entity.Property(e => e.IdTipoUsuario).HasColumnType("int(11)");

            entity.HasOne(d => d.IdTipoUsuarioNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdTipoUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkUsuario_TipoUsuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
