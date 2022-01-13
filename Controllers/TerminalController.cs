using Library_KP.Data;
using Library_KP.Models;
using Library_KP.Models.Administrirovanie;
using Library_KP.Models.TerminalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Library_KP.Controllers
{
    public class TerminalController : Controller
    {
        private readonly LibraryContext db;
        private readonly ApplicationContext aC;

        public TerminalController(LibraryContext context, ApplicationContext context1) 
        {
            db = context;
            aC = context1;
        }

        // GET: TerminalController
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index(int page = 1, SortState sortOrder = SortState.FioAsc)
        {
            int pageSize = 15;

            IQueryable<Terminal> ter = db.Terminals.Include(x => x.NumberTicketsNavigation).Include(b => b.RegistrationBook);

            // сортировка
            switch (sortOrder)
            {
                case SortState.FioDesc:
                    ter = ter.OrderByDescending(s => s.NumberTicketsNavigation.Fcs);
                    break;
                case SortState.NameBookAsc:
                    ter = ter.OrderBy(s => s.RegistrationBook.NameBook);
                    break;
                case SortState.NameBookDesc:
                    ter = ter.OrderByDescending(s => s.RegistrationBook.NameBook);
                    break;
                case SortState.DIAsc:
                    ter = ter.OrderBy(s => s.DateIssue);
                    break;
                case SortState.DIDesc:
                    ter = ter.OrderByDescending(s => s.DateIssue);
                    break;
                case SortState.RDAsc:
                    ter = ter.OrderBy(s => s.ReturnDate);
                    break;
                case SortState.RDDesc:
                    ter = ter.OrderByDescending(s => s.ReturnDate);
                    break;
                default:
                    ter = ter.OrderBy(s => s.NumberTicketsNavigation.Fcs);
                    break;
            }

            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                var user = User.Identity.Name;
                int userId = (from i in aC.Users where i.Email == user select i.Id).Single();
                ter = db.Terminals.Where(vM => vM.NumberTickets == userId).Include(x => x.NumberTicketsNavigation).Include(b => b.RegistrationBook);
                //return View(viewModel);
            }

            // пагинация
            var count = await ter.CountAsync();
            var items = await ter.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                Terminals = items
            };

            return View(viewModel);
        }

        // GET: TerminalController/Details/5
        [Authorize(Roles = "admin, user")]
        public ActionResult Details(int id)
        {
            Terminal terminal = db.Terminals.Where(t => t.TerminalId == id).Include(r => r.NumberTicketsNavigation).Include(b => b.RegistrationBook).FirstOrDefault();
            var dI = (from dT in db.Terminals where dT.TerminalId == id select dT.DateIssue).Single();
            var rI = (from rT in db.Terminals where rT.TerminalId == id  select rT.ReturnDate).Single();
            DateTime rI1 = Convert.ToDateTime(rI); 
            if((rI1 - dI).TotalDays > 30)
            {
                ViewBag.div = "Этот человек должник!";
                return View(terminal);
            }
            return View(terminal);
        }

        // GET: TerminalController/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            Terminal ter = new();
            ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
            ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
            return View(ter);
        }

        // POST: TerminalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Terminal ter)
        {
            try
            {
                db.Add(ter);
                await db.SaveChangesAsync();
                ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: TerminalController/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View();
            }
            Terminal ter = db.Terminals.Find(id);
            if (ter == null)
            {
                return NotFound();
            }
            ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
            ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
            return View(ter);
        }

        // POST: TerminalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Terminal ter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ter).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                return View(ter);
            }
            catch
            {
                return View();
            }
        }

        // GET: TerminalController/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var ter = await db.Terminals.FindAsync(id);
                db.Terminals.Remove(ter);
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
