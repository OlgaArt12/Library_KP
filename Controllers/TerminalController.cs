﻿using Library_KP.Data;
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
using Library_KP.Models.BooksModel;

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
        public async Task<IActionResult> Index(int page = 1, SortState1 sortOrder = SortState1.FioAsc)
        {
            int pageSize = 15;

            IQueryable<Terminal> ter = db.Terminals.Include(x => x.NumberTicketsNavigation).Include(b => b.RegistrationBook);

            // сортировка
            switch (sortOrder)
            {
                case SortState1.FioDesc:
                    ter = ter.OrderByDescending(s => s.NumberTicketsNavigation.Fcs);
                    break;
                case SortState1.NameBookAsc:
                    ter = ter.OrderBy(s => s.RegistrationBook.NameBook);
                    break;
                case SortState1.NameBookDesc:
                    ter = ter.OrderByDescending(s => s.RegistrationBook.NameBook);
                    break;
                case SortState1.DIAsc:
                    ter = ter.OrderBy(s => s.DateIssue);
                    break;
                case SortState1.DIDesc:
                    ter = ter.OrderByDescending(s => s.DateIssue);
                    break;
                case SortState1.RDAsc:
                    ter = ter.OrderBy(s => s.ReturnDate);
                    break;
                case SortState1.RDDesc:
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
            IndexViewModel1 viewModel = new IndexViewModel1
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel1(sortOrder),
                Terminals = items
            };

            return View(viewModel);
        }

        // GET: TerminalController/Details/5
        [Authorize(Roles = "admin, user")]
        public ActionResult Details(int id)
        {
            Terminal terminal = db.Terminals.Where(t => t.TerminalId == id).Include(r => r.NumberTicketsNavigation).Include(b => b.RegistrationBook).FirstOrDefault();            
            var dC = (from d in db.Terminals where (d.ReturnDate == null) && d.TerminalId == id select d).Count();
            if(dC != 0)
            {
                var dI = (from dT in db.Terminals where dT.TerminalId == id select dT.DateIssue).Single();
                var dateIssRet = dI.AddMonths(1);
                if((dateIssRet - dI).TotalDays > 30)
                {
                    ViewBag.div = "Этот человек должник!";
                    return View(terminal);
                }
            }
            else if(dC == 0)
            {
                var dI = (from dT in db.Terminals where dT.TerminalId == id select dT.DateIssue).Single();
                var dR = (from dT in db.Terminals where dT.TerminalId == id && dT.ReturnDate != null select dT.ReturnDate).Single();
                DateTime dateRet = Convert.ToDateTime(dR);
                if((dateRet - dI).TotalDays > 30)
                {
                    ViewBag.div = "Этот человек должник!";
                    return View(terminal);
                }
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
            return View("Create");
        }

        // POST: TerminalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Terminal ter)
        {
            try
            {

                var regBookRet = (from rBr in db.Terminals
                                  where  rBr.RegistrationBookId == ter.RegistrationBookId && rBr.ReturnDate == null
                                  select rBr).Count();

                if(regBookRet != 0)
                {
                    ViewBag.Message = "К сожалению вы не можете выдать книгу, которая уже выдана!";
                    ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                    ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                    return View("Create");
                }
                else 
                {
                    var unic = (from u in db.Terminals
                                where u.NumberTickets == ter.NumberTickets && u.RegistrationBookId == ter.RegistrationBookId && u.ReturnDate == ter.ReturnDate && u.DateIssue == ter.DateIssue
                                select u).Count();

                    if (unic != 0)
                    {
                        ViewBag.Message = "Такая запись уже существует!";
                        ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                        ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                        return View("Create");
                    }
                    else
                    {
                        if (ter.DateIssue >= ter.ReturnDate || ter.DateIssue > DateTime.Today)
                        {
                            ViewBag.Message = "Вы ввели не корректные данный: дата выдачи не может быть больше сегодняшней даты или больше даты возврата!";
                            ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                            ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                            return View("Create");
                        }
                        else
                        {
                            if(ter.DateIssue <= DateTime.Today && ter.ReturnDate > DateTime.Today)
                            {
                                ViewBag.Message = "Вы ввели не корректные данный: дата возврата не может быть больше сегодняшней даты, если книгу выдали сегодня!";
                                ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                                ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                                return View("Create");
                            }
                            else
                            {
                                var dat = (from dN in db.Terminals where ((dN.DateIssue <= ter.DateIssue && ter.DateIssue <= dN.ReturnDate) || (ter.DateIssue <= dN.DateIssue && (ter.ReturnDate == null || ter.ReturnDate >= dN.DateIssue))) && ter.RegistrationBookId == dN.RegistrationBookId select dN).Count();
                                if(dat != 0)
                                {
                                    ViewBag.Message = "Книгу уже выдали!";
                                    ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                                    ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                                    return View("Create");
                                }
                                else
                                {
                                    var countBook = (from cB in db.Terminals where (DateTime.Today.AddMonths(-1) >= cB.DateIssue && cB.ReturnDate == null) || (cB.ReturnDate >= cB.DateIssue.AddMonths(1)) 
                                                        && cB.NumberTickets == ter.NumberTickets select cB).Count();
                                    if(countBook >= 3)
                                    {
                                        ViewBag.Message = "Читатель не вернул больше 3х книг!";
                                        ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                                        ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                                        return View("Create");
                                    }
                                    else
                                    {
                                        Book b1 = (from b in db.Books
                                                   where b.RegistrationId == ter.RegistrationBookId
                                                   select b).Single();

                                        if (b1.YearOfPublication > ter.DateIssue.Year)
                                        {
                                            ViewBag.Message = "Книгу еще не издали!";
                                            ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                                            ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                                            return View("Create");
                                        }
                                        else
                                        {
                                            db.Add(ter);
                                            await db.SaveChangesAsync();
                                            ViewBag.RegistrationBookId = new SelectList(db.Books, "RegistrationId", "NameBook", ter.RegistrationBook);
                                            ViewBag.NumberTickets = new SelectList(db.Readers, "NumberTicket", "Fcs", ter.NumberTicketsNavigation);
                                            return RedirectToAction("Index");
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }

                }
             
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
