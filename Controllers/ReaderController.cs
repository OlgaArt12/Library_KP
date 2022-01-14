using Library_KP.Data;
using Library_KP.Models;
using Library_KP.Models.ReadersModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library_KP.Controllers
{
    public class ReaderController : Controller
    {
        private readonly LibraryContext db;
        private readonly ApplicationContext aC;

        public ReaderController(LibraryContext context, ApplicationContext context1) 
        {
            db = context;
            aC = context1;
        }

        // GET: ReaderController
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index(string name, int page = 1, SortState sortOrder = SortState.NameAsc)
        {
            const int pageSize = 15;

            IQueryable<Reader> readers;

            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    readers = db.Readers.OrderByDescending(s => s.Fcs);
                    break;
                case SortState.PassAsc:
                    readers = db.Readers.OrderBy(s => s.PassportData);
                    break;
                case SortState.PassDesc:
                    readers = db.Readers.OrderByDescending(s => s.PassportData);
                    break;
                case SortState.HomeAsc:
                    readers = db.Readers.OrderBy(s => s.HomeAddress);
                    break;
                case SortState.HomeDesc:
                    readers = db.Readers.OrderByDescending(s => s.HomeAddress);
                    break;
                default:
                    readers = db.Readers.OrderBy(s => s.Fcs);
                    break;
            }

            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                var user = User.Identity.Name;
                int userId = (from i in aC.Users where i.Email == user select i.Id).Single();
                readers = db.Readers.Where(vM => vM.NumberTicket == userId);
            }

            // пагинация
            var count = await readers.CountAsync();
            var items = await readers.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                Readers = items
            };
            return View(viewModel);
        }

        // GET: ReaderController/Details/5
        [Authorize(Roles = "admin, user")]
        public ActionResult Details(int id)
        {
            Reader reader = db.Readers.Where(r => r.NumberTicket == id).FirstOrDefault();
            var countBook = (from cB in db.Terminals where (DateTime.Today.AddMonths(-1) >= cB.DateIssue && cB.ReturnDate == null) || (cB.ReturnDate >= cB.DateIssue.AddMonths(1)) && cB.NumberTickets == id select cB).Count();
            ViewBag.countBook = countBook;
            return View(reader);
        }

        // GET: ReaderController/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                ViewBag.Message = "У вас недостаочно прав для этого действия!";
                return View();
            }
            Reader reader = new Reader();
            return View(reader);
        }

        // POST: ReaderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reader reader)
        {
            try
            {
                var passDate = (from r in db.Readers
                               where r.PassportData == reader.PassportData
                               select r).Count();
                if(passDate == 0)
                {
                    db.Add(reader);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.Message = "Введенные данные не уникальны!";
                return View();
            }
            catch
            {
                ViewBag.Message = "Введенные данные не уникальны!";
                return View();
            }
        }

        // GET: ReaderController/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                ViewBag.Message = "У вас недостаочно прав для этого действия!";
                return View();
            }
            if (id != null)
            {
                Reader reader = await db.Readers.FirstOrDefaultAsync(r => r.NumberTicket == id);
                if (reader != null)
                    return View(reader);
            }
            return NotFound();
        }

        // POST: ReaderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Reader reader)
        {
            try
            {
                var passDate = (from r in db.Readers
                                where r.PassportData == reader.PassportData && r.NumberTicket != reader.NumberTicket
                                select r).Count();
                if(passDate == 0)
                {
                    db.Attach(reader);
                    db.Entry(reader).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.Message = "Введенные данные не уникальны! ";
                return View("Edit");
            }
            catch
            {
                ViewBag.Message = "Произошла ошибка! ";
                return View("Edit");
            }
        }

        // GET: ReaderController/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                if (role == "user")
                {
                    ViewBag.Message = "У вас недостаочно прав для этого действия!";
                    return View();
                }
                var reader = await db.Readers.FindAsync(id);
                db.Readers.Remove(reader);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
