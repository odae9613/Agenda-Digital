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
    public class CategoriaGastoController : Controller
    {
        private readonly MyDbContext _context;

        public CategoriaGastoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaEvento
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriaGastos.ToListAsync());
        }

        // GET: CategoriaEvento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaGasto = await _context.CategoriaGastos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaGasto == null)
            {
                return NotFound();
            }

            return View(categoriaGasto);
        }

        // GET: CategoriaEvento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaEvento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] CategoriaGasto categoriaGasto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaGasto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaGasto);
        }

        // GET: CategoriaEvento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaGasto = await _context.CategoriaGastos.FindAsync(id);
            if (categoriaGasto == null)
            {
                return NotFound();
            }
            return View(categoriaGasto);
        }

        // POST: CategoriaEvento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] CategoriaEvento categoriaGasto)
        {
            if (id != categoriaGasto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaGasto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaGastoExists(categoriaGasto.Id))
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
            return View(categoriaGasto);
        }

        // GET: CategoriaEvento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaGasto = await _context.CategoriaGastos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaGasto == null)
            {
                return NotFound();
            }

            return View(categoriaGasto);
        }

        // POST: CategoriaEvento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaGasto = await _context.CategoriaGastos.FindAsync(id);
            if (categoriaGasto != null)
            {
                _context.CategoriaGastos.Remove(categoriaGasto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaGastoExists(int id)
        {
            return _context.CategoriaGastos.Any(e => e.Id == id);
        }
    }
}
