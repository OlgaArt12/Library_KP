using Library_KP.Data;
using Library_KP.Models;
using Library_KP.Models.BooksModel;
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

namespace Library_KP.Controllers
{
    public class BookController : Controller
    {
        private  LibraryContext db;

        public BookController(LibraryContext context)
        {
            db = context;
        }

        // GET: BookController
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index(int? part, string nameBook, int page = 1, SortState sortOrder = SortState.NameBookAsc)
        {
             int pageSize = 15;

            //фильтрация
            IQueryable<Book> books = db.Books.Include(x => x.PartitionNameNavigation);

            if (part != null && part != 0)
            {
                books = books.Where(p => p.PartitionNameNavigation.PartitionId == part);
            }
            if (!String.IsNullOrEmpty(nameBook))
            {
                books = books.Where(p => p.NameBook.Contains(nameBook));
            }

            // сортировка
            switch (sortOrder)
            {
                case SortState.NameBookDesc:
                    books = books.OrderByDescending(s => s.NameBook);
                    break;
                case SortState.YearAsc:
                    books = books.OrderBy(s => s.YearOfPublication);
                    break;
                case SortState.YearDesc:
                    books = books.OrderByDescending(s => s.YearOfPublication);
                    break;
                case SortState.NumbAsc:
                    books = books.OrderBy(s => s.NumberOfPage);
                    break;
                case SortState.NumbDesc:
                    books = books.OrderByDescending(s => s.NumberOfPage);
                    break;
                case SortState.AuthAsc:
                    books = books.OrderBy(s => s.Author);
                    break;
                case SortState.AuthDesc:
                    books = books.OrderByDescending(s => s.Author);
                    break;
                case SortState.PartAsc:
                    books = books.OrderBy(s => s.PartitionNameNavigation.NamePartition);
                    break;
                case SortState.PartDesc:
                    books = books.OrderByDescending(s => s.PartitionNameNavigation.NamePartition);
                    break;
                default:
                    books = books.OrderBy(s => s.NameBook);
                    break;
            }

            // пагинация
            var count = await books.CountAsync();
            var items = await books.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(db.Partitions.ToList(), part, nameBook),
                Books = items
            };
            return View(viewModel);
        }

        // GET: BookController/Details/5
        [Authorize(Roles = "admin, user")]
        public ActionResult Details(int id)
        {
            Book book = db.Books.Where(b => b.RegistrationId == id).Include(p => p.PartitionNameNavigation).FirstOrDefault();
            var countBook = (from cB in db.Terminals where cB.RegistrationBookId == id select cB).Count();
            ViewBag.countBook = countBook;
            return View(book);
        }

        // GET: BookController/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                ViewBag.Message = "У вас недостаочно прав для этого действия. Ваша роль: " + role;
                return View("Login");
            }
            Book book = new();
            ViewBag.PartitionName = new SelectList(db.Partitions, "PartitionId", "NamePartition", book.PartitionName);
            return View(book);
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            try
            {
                db.Add(book);
                await db.SaveChangesAsync();
                ViewBag.PartitionName = new SelectList(db.Partitions, "PartitionId", "NamePartition", book.PartitionName);
                return RedirectToAction("Create");
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        // GET: BookController/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View();
            }
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                ViewBag.Message = "У вас недостаочно прав для этого действия. Ваша роль: " + role;
                return View("Login");
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.PartitionName = new SelectList(db.Partitions, "PartitionId", "NamePartition",book.PartitionName);
            return View(book);
        }

        // POST: Subscribers_has_Editions/Edit/5
         [HttpPost]
         [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            try
            {
                
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.PartitionName = new SelectList(db.Partitions, "PartitionId", "NamePartition", book.PartitionName);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }


        // GET: BookController/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                if (role == "user")
                {
                    ViewBag.Message = "У вас недостаочно прав для этого действия. Ваша роль: " + role;
                    return View("Login");
                }
                var books = await db.Books.FindAsync(id);
                db.Books.Remove(books);
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
