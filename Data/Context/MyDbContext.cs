using ImOdNotes.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes.Data.Context
{
    public class MyDbContext: DbContext
    {
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Nota> Notas { get; set; } = null!;
        public DbSet<Evento> Eventos { get; set; } = null!;
        public DbSet<Gasto> Gastos { get; set; } = null!;
        public DbSet<Tarea> Tareas { get; set; } = null!;
        public DbSet<Objetivo> Objetivos { get; set; } = null!;
        public DbSet<CategoriaNota> CategoriaNotas { get; set; } = null!;
        public DbSet<CategoriaGasto> CategoriaGastos { get; set; } = null!;
        public DbSet<CategoriaEvento> CategoriaEventos { get; set; } = null!;
        public DbSet<CategoriaTarea> CategoriaTareas { get; set; } = null!;
        public DbSet<CategoriaObjetivo> CategoriaObjetivos { get; set; } = null!;
        public DbSet<Estado> Estados { get; set; } = null!;

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración de la relación entre Usuario y Rol
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();
            //  GASTO
            modelBuilder.Entity<Gasto>()
                .Property(g => g.Monto)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Usuario)
                .WithMany(u => u.Gastos)
                .HasForeignKey(g => g.UsuarioId);
            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.CategoriaGasto)
                .WithMany(c => c.Gastos)
                .HasForeignKey(g => g.CategoriaId);
            //  NOTA
            modelBuilder.Entity<Nota>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notas)
                .HasForeignKey(n => n.UsuarioId);
            modelBuilder.Entity<Nota>()
                .HasOne(n => n.CategoriaNota)
                .WithMany(c => c.Notas)
                .HasForeignKey(n => n.CategoriaId);
            //  EVENTO
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.Usuario)
                .WithMany(u => u.Eventos)
                .HasForeignKey(e => e.UsuarioId); 
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.CategoriaEvento)
                .WithMany(c => c.Eventos)
                .HasForeignKey(e => e.CategoriaId);
            //  TAREA
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.UsuarioId);
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.CategoriaTarea)
                .WithMany(c => c.Tareas)
                .HasForeignKey(t => t.CategoriaId);
            //  OBJETIVO
            modelBuilder.Entity<Objetivo>()
                .HasOne(o => o.Usuario)
                .WithMany(u => u.Objetivos)
                .HasForeignKey(o => o.UsuarioId);
            modelBuilder.Entity<Objetivo>()
                .HasOne(o => o.CategoriaObjetivo)
                .WithMany(c => c.Objetivos)
                .HasForeignKey(o => o.CategoriaId);
        }
    }
}
