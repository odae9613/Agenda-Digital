using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;

namespace ImOdNotes.Controllers
{
    public class ObjetivoController : Controller
    {
        private readonly MyDbContext _context;

        public ObjetivoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Objetivo
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Objetivos.Include(o => o.CategoriaObjetivo).Include(o => o.Usuario);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Objetivo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objetivo = await _context.Objetivos
                .Include(o => o.CategoriaObjetivo)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (objetivo == null)
            {
                return NotFound();
            }

            return View(objetivo);
        }

        // GET: Objetivo/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaObjetivos, "Id", "Nombre");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01");
            return View();
        }

        // POST: Objetivo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,Progreso,FechaObjetivo,Completado,FechaCreacion,FechaActualizacion,Favorito,UsuarioId,CategoriaId")] Objetivo objetivo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objetivo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaObjetivos, "Id", "Nombre", objetivo.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", objetivo.UsuarioId);
            return View(objetivo);
        }

        // GET: Objetivo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objetivo = await _context.Objetivos.FindAsync(id);
            if (objetivo == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaObjetivos, "Id", "Nombre", objetivo.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", objetivo.UsuarioId);
            return View(objetivo);
        }

        // POST: Objetivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,Progreso,FechaObjetivo,Completado,FechaCreacion,FechaActualizacion,Favorito,UsuarioId,CategoriaId")] Objetivo objetivo)
        {
            if (id != objetivo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objetivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjetivoExists(objetivo.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaObjetivos, "Id", "Nombre", objetivo.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", objetivo.UsuarioId);
            return View(objetivo);
        }

        // GET: Objetivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objetivo = await _context.Objetivos
                .Include(o => o.CategoriaObjetivo)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (objetivo == null)
            {
                return NotFound();
            }

            return View(objetivo);
        }

        // POST: Objetivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objetivo = await _context.Objetivos.FindAsync(id);
            if (objetivo != null)
            {
                _context.Objetivos.Remove(objetivo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObjetivoExists(int id)
        {
            return _context.Objetivos.Any(e => e.Id == id);
        }
    }
}
