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
    public class CategoriaObjetivoController : Controller
    {
        private readonly MyDbContext _context;

        public CategoriaObjetivoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaObjetivo
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriaObjetivos.ToListAsync());
        }

        // GET: CategoriaObjetivo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaObjetivo = await _context.CategoriaObjetivos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaObjetivo == null)
            {
                return NotFound();
            }

            return View(categoriaObjetivo);
        }

        // GET: CategoriaObjetivo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaObjetivo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,FechaCreacion,FechaActualizacion,UsuarioId")] CategoriaObjetivo categoriaObjetivo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaObjetivo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaObjetivo);
        }

        // GET: CategoriaObjetivo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaObjetivo = await _context.CategoriaObjetivos.FindAsync(id);
            if (categoriaObjetivo == null)
            {
                return NotFound();
            }
            return View(categoriaObjetivo);
        }

        // POST: CategoriaObjetivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,FechaCreacion,FechaActualizacion,UsuarioId")] CategoriaObjetivo categoriaObjetivo)
        {
            if (id != categoriaObjetivo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaObjetivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaObjetivoExists(categoriaObjetivo.Id))
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
            return View(categoriaObjetivo);
        }

        // GET: CategoriaObjetivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaObjetivo = await _context.CategoriaObjetivos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaObjetivo == null)
            {
                return NotFound();
            }

            return View(categoriaObjetivo);
        }

        // POST: CategoriaObjetivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaObjetivo = await _context.CategoriaObjetivos.FindAsync(id);
            if (categoriaObjetivo != null)
            {
                _context.CategoriaObjetivos.Remove(categoriaObjetivo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaObjetivoExists(int id)
        {
            return _context.CategoriaObjetivos.Any(e => e.Id == id);
        }
    }
}
