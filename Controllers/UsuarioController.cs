using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using ImOdNotes.Models.ViewModels;
using ImOdNotes.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly PaginacionService _paginacionService;
        private readonly MyDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioController(MyDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _paginacionService = new PaginacionService();
        }

        // GET: Usuario
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index(int page = 1)
        {
            //  PAGINACIÓN MANUAL
            var usuariosQuery = _context.Usuarios;
            var paginado = _paginacionService.Paginacion<Usuario>(usuariosQuery, page);

            return View(paginado);
        }

        // GET: Usuario/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,NombreUsuario,Apellido01,Apellido02,FechaNacimiento,LugarNacimiento,Nacionalidad,Email,Genero,RolId,Password,URLFotoPerfil")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                await CrearCategoriasPorDefecto(usuario.Id);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RolId = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,NombreUsuario,Apellido01,Apellido02,FechaNacimiento,LugarNacimiento,Nacionalidad,Email,Genero,RolId,Password,URLFotoPerfil")] Usuario usuario)
        {
            var usuarioEditado = await _context.Usuarios.FindAsync(id);
            if (usuarioEditado == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(usuario);
                    usuarioEditado.Nombre = usuario.Nombre;
                    usuarioEditado.NombreUsuario = usuario.NombreUsuario;
                    usuarioEditado.Apellido01 = usuario.Apellido01;
                    usuario.Apellido02 = usuario.Apellido02;
                    usuarioEditado.Email = usuario.Email;
                    usuarioEditado.URLFotoPerfil = usuario.URLFotoPerfil;
                    usuarioEditado.Genero = usuario.Genero;
                    usuarioEditado.Nacionalidad = usuario.Nacionalidad;
                    usuarioEditado.FechaNacimiento = usuario.FechaNacimiento;
                    usuarioEditado.LugarNacimiento = usuario.LugarNacimiento;
                    usuarioEditado.RolId = usuario.RolId;
                    //  Hashear contraseña
                    if (!string.IsNullOrWhiteSpace(usuario.Password))
                    {
                        usuarioEditado.Password = _passwordHasher.HashPassword(usuarioEditado, usuario.Password);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Registro
        public IActionResult Registro()
        {
            return View();
        }
        // POST: Usuario/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = new Usuario
            {
                NombreUsuario = model.NombreUsuario,
                Email = model.Email,
                Nombre = model.NombreUsuario,
                Apellido01 = "N/A",
                FechaNacimiento = DateTime.Now,
                RolId = 3 // Rol correspondiente a "usuario estándar"
            };

            usuario.Password = _passwordHasher.HashPassword(usuario, model.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await CrearCategoriasPorDefecto(usuario.Id);

            return RedirectToAction(nameof(Login));
        }

        // GET: Usuario/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario || u.Email == model.NombreUsuario);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario inválido.");
                return View(model);
            }

            var verification = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);
            if (verification == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Contraseña inválida.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario ?? string.Empty),
                new Claim("FullName", usuario.Nombre ?? string.Empty)
            };

            if (usuario.Rol != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, usuario.Rol.Nombre ?? usuario.RolId.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            return RedirectToAction("Inicio", "Home");
        }

        // POST: Usuario/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        private async Task CrearCategoriasPorDefecto(int usuarioId)
        {
            var categoriaEvento = new CategoriaEvento
            {
                Nombre = "General",
                UsuarioId = usuarioId
            };

            var categoriaNota = new CategoriaNota
            {
                Nombre = "Ideas",
                UsuarioId = usuarioId
            };

            var categoriaGasto = new CategoriaGasto
            {
                Nombre = "General",
                UsuarioId = usuarioId
            };

            var categoriaTarea = new CategoriaTarea
            {
                Nombre = "General",
                UsuarioId = usuarioId
            };

            var categoriaObjetivo = new CategoriaObjetivo
            {
                Nombre = "General",
                UsuarioId = usuarioId
            };

            _context.CategoriaEventos.Add(categoriaEvento);
            _context.CategoriaNotas.Add(categoriaNota);
            _context.CategoriaGastos.Add(categoriaGasto);
            _context.CategoriaTareas.Add(categoriaTarea);
            _context.CategoriaObjetivos.Add(categoriaObjetivo);

            await _context.SaveChangesAsync();
        }

    }
}