using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CadastroEmpresa.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CadastroEmpresa.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly Context _context;

        public EmpresasController(Context context)
        {
            _context = context;
        }

        // GET: Empresas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empresas.ToListAsync());
        }

        public async Task<IActionResult> Filtrar(string valor, string tipoFiltro)
        {
            ViewData["Valor"] = valor;

            if (!string.IsNullOrWhiteSpace(valor) && tipoFiltro == "PorCNPJ")
            {
                ViewData[tipoFiltro] = "checked";
                return View("Index", await _context.Empresas.Where(x => x.CNPJ.Contains(valor)).ToListAsync());
            }
            else if(!string.IsNullOrWhiteSpace(valor) && tipoFiltro == "PorNome")
            {
                ViewData[tipoFiltro] = "checked";
                return View("Index", await _context.Empresas.Where(x => x.Nome.Contains(valor)).ToListAsync());
            }
            else
            {
                return View("Index", await _context.Empresas.ToListAsync());
            }
        }

        // GET: Empresas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // GET: Empresas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CNPJ,Status,Message,Nome,NomeFantasia,Email,Telefone")] Empresa empresa)
        {
            if (EmpresaExists(empresa))
            {
                ViewData["erro"] = "Empresa já cadastrada";
                return View(empresa);
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(empresa.CNPJ))
            {
                _context.Add(empresa.FormataCNPJ());
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }

            return View(empresa);
        }

        public async Task<IActionResult> Pesquisar(string empresaCNPJ)
        {
            var empresa = new Empresa
            {
                CNPJ = empresaCNPJ
            };

            if (string.IsNullOrEmpty(empresa.CNPJ))
            {
                return View("Create");
            }

            using (var client = new HttpClient())
            {
                var URI = "https://www.receitaws.com.br/v1/cnpj/" + empresa.CNPJSemFormatacao();

                HttpResponseMessage response = await client.GetAsync(URI);
                if (response.IsSuccessStatusCode)
                {
                    var EmpresaJson = await response.Content.ReadAsStringAsync();
                    empresa = JsonConvert.DeserializeObject<Empresa>(EmpresaJson);

                    if (empresa.Status == "ERROR")
                        ViewData["erro"] = empresa.Message;
                    else
                        return View("Create", empresa);
                }
                else
                    ViewData["erro"] = $"Você está realizando muitas solicitações, aguarde alguns segundos e tente novamente.";

                return View("Create");
            }
        }

        // GET: Empresas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CNPJ,Status,Message,Nome,NomeFantasia,Email,Telefone")] Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.Id))
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
            return View(empresa);
        }

        // GET: Empresas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.Id == id);
        }

        private bool EmpresaExists(Empresa empresa)
        {
            return _context.Empresas.ToList().Any(e => e.CNPJSemFormatacao() == empresa.CNPJSemFormatacao());
        }
    }
}
