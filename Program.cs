using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<MyDbContext>(x =>
            {
                x.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL"));
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Usuario/Login";
                    options.Cookie.Name = "MiCookieAuth"; // cookie name can remain custom
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
                });

            // Agregar el servicio de encriptación de contraseñas
            builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

            var app = builder.Build();

            //  Usuario admin por defecto
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

                context.Database.Migrate();

                // 1. ROLES
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Rol { Nombre = "Administrador" },
                        new Rol { Nombre = "Usuario" }
                    );

                    context.SaveChanges();
                }

                var adminRole = context.Roles.First(r => r.Nombre == "Administrador");

                // 2. ADMIN
                if (!context.Usuarios.Any(u => u.NombreUsuario == "admin"))
                {
                    var admin = new Usuario
                    {
                        Nombre = "Administrador",
                        NombreUsuario = "admin",
                        Apellido01 = "Sistema",
                        FechaNacimiento = new DateTime(1990, 1, 1),
                        Email = "admin@admin.com",
                        RolId = adminRole.Id
                    };

                    admin.Password = hasher.HashPassword(admin, "Admin123*");

                    context.Usuarios.Add(admin);
                    context.SaveChanges();
                }
            }
            //

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Usuario}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
