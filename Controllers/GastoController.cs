using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes.Controllers
{
    public class GastoController : Controller
    {
        private readonly MyDbContext _context;

        public GastoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Gasto
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }
            var gasto = _context.Gastos
                .Include(g => g.CategoriaGasto)
                .Include(g => g.Usuario)
                .Where(g => g.UsuarioId == usuarioId)
                .ToListAsync();
            return View(await gasto);
        }

        // GET: Gasto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gasto = await _context.Gastos
                .Include(g => g.CategoriaGasto)
                .Include(g => g.Usuario)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gasto == null)
            {
                return NotFound();
            }

            return View(gasto);
        }

        // GET: Gasto/Create
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            ViewBag.CategoriaId = new SelectList(
                _context.CategoriaGastos
                    .Where(c => c.UsuarioId == usuarioId),
                "Id",
                "Nombre"
            );
            return View();
        }

        // POST: Gasto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Monto,FechaCreacion,FechaActualizacion,Notas,Tipo,Favorito,CategoriaId")] Gasto gasto)
        {
            // Obtener id del usuario autenticado desde los claims
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var usuarioId))
            {
                //return RedirectToAction("Login", "Usuario");
                return Unauthorized();
            }

            gasto.UsuarioId = usuarioId;
            // Ignorar validación de propiedades de navegación
            ModelState.Remove("Usuario");
            ModelState.Remove("CategoriaGasto");
            // Validar que se haya seleccionado una categoría
            if (gasto.CategoriaId <= 0)
            {
                ModelState.AddModelError("CategoriaId", "Seleccione una categoría.");
            }
            if (ModelState.IsValid)
            {
                _context.Gastos.Add(gasto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaGastos, "Id", "Nombre", gasto.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", gasto.UsuarioId);
            return View(gasto);
        }

        // GET: Gasto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
            if (gasto == null)
            {
                return NotFound();
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaGastos, "Id", "Nombre", gasto.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", gasto.UsuarioId);
            return View(gasto);
        }

        // POST: Gasto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Monto,FechaCreacion,FechaActualizacion,Notas,Tipo,Favorito,CategoriaId")] Gasto gasto)
        {
            if (id != gasto.Id)
            {
                return NotFound();
            }

            // Usuario autenticado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            // Buscar la nota original del usuario
            var gastoDb = await _context.Gastos
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);

            if (gastoDb == null)
            {
                return NotFound();
            }

            // Actualizar SOLO los campos editables
            gastoDb.Titulo = gasto.Titulo;
            gastoDb.Monto = gasto.Monto;
            gastoDb.Notas = gasto.Notas;
            gastoDb.Tipo = gasto.Tipo;
            gastoDb.Favorito = gasto.Favorito;
            gastoDb.CategoriaId = gasto.CategoriaId;

            // Mantener usuario logeado
            gastoDb.UsuarioId = usuarioId;
            // Fecha actualización
            gastoDb.FechaActualizacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(gasto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GastoExists(gasto.Id))
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
            ViewBag.CategoriaId = new SelectList(_context.CategoriaGastos, "Id", "Nombre", gasto.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", gasto.UsuarioId);
            return View(gasto);
        }

        // GET: Gasto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var gasto = await _context.Gastos
                .Include(g => g.CategoriaGasto)
                .Include(g => g.Usuario)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gasto == null)
            {
                return NotFound();
            }

            return View(gasto);
        }

        // POST: Gasto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto != null)
            {
                _context.Gastos.Remove(gasto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Favorito(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }
            Gasto gasto = _context.Gastos
                .FirstOrDefault(x => x.Id == id && x.UsuarioId == usuarioId);
            if (gasto != null)
            {
                gasto.Favorito = !gasto.Favorito;
                await _context.SaveChangesAsync();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> ListaFavoritos(int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            int totalGastosFavoritos = await _context.Gastos
                                        .Where(e => e.Favorito && e.UsuarioId == usuarioId)  // Filtra solo los gastos favoritos del usuario
                                        .CountAsync();
            int pageSize = 5;
            int totalPaginas = (int)Math.Ceiling((double)totalGastosFavoritos / pageSize);
            ViewBag.TotalRegistros = totalGastosFavoritos;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;

            List<Gasto> data = await _context.Gastos
                                .Where(e => e.Favorito && e.UsuarioId == usuarioId)
                                .OrderBy(e => e.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return View(data);
        }

        public IActionResult Balance()
        {
            var movimientos = _context.Gastos.ToList();

            decimal ingresos = movimientos
                .Where(x => x.Tipo == "Ingreso")
                .Sum(x => x.Monto);

            decimal gastos = movimientos
                .Where(x => x.Tipo == "Gasto")
                .Sum(x => x.Monto);

            decimal balance = ingresos - gastos;


            DateTime mes = DateTime.Now;
            ViewBag.mes = mes;
            ViewBag.Ingresos = ingresos;
            ViewBag.Gastos = gastos;
            ViewBag.Balance = balance;

            return View(movimientos);
        }

        private bool GastoExists(int id)
        {
            return _context.Gastos.Any(e => e.Id == id);
        }
    }
}
