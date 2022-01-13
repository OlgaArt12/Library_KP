using Library_KP.Data;
using Library_KP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Controllers
{
    public class PartitionController : Controller
    {
        private readonly LibraryContext db;

        public PartitionController(LibraryContext context)
        {
            db = context;
        }

        // GET: PartitionController
        public ActionResult Index()
        {
            List<Partition> partitions = db.Partitions.ToList();
            return View(partitions);
        }

        // GET: PartitionController/Details/5
        public ActionResult Details(int id)
        {
            Partition partition = db.Partitions.Where(p => p.PartitionId == id).FirstOrDefault();
            var countBook = (from t in db.Books where t.PartitionNameNavigation.PartitionId == id select t).Count();
            ViewBag.countBook = countBook;
            return View(partition);
        }

        // GET: PartitionController/Create
        public ActionResult Create()
        {
            Partition partition = new Partition();
            return View(partition);
        }

        // POST: PartitionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Partition partition)
        {
            try
            {
                db.Add(partition);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Данный раздел уже существует! ";
                return View();
            }
        }

        // GET: PartitionController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                    Partition partition = await db.Partitions.FirstOrDefaultAsync(p => p.PartitionId == id);
                    if (partition != null)
                        return View(partition);
            }
            return NotFound();
        }

        // POST: PartitionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Partition partition)
        {
            try
            {
               db.Attach(partition);
               db.Entry(partition).State = EntityState.Modified;
               await db.SaveChangesAsync();
               return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Данный раздел уже существует! ";
                return View();
            }
        }

        // GET: PartitionController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var countBook = (from t in db.Books where t.PartitionNameNavigation.PartitionId == id select t).Count();
            if (countBook >= 100)
            {
                ViewBag.Message = "Количество книг в разделе больше 100! (" + countBook + ")";
                Partition partition = db.Partitions.Where(p => p.PartitionId == id).FirstOrDefault();
                return View(partition);
            }

            var part = await db.Partitions.FindAsync(id);
            db.Partitions.Remove(part);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
