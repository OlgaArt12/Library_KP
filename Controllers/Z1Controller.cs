using Library_KP.Data;
using Library_KP.Models;
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
    public class Z1Controller : Controller
    {
        private readonly LibraryContext db;
        private readonly ApplicationContext aC;
        public Z1Controller(LibraryContext context, ApplicationContext context1)
        {
            db = context;
            aC = context1;
        }

        // GET: Z1Controller
        [Authorize(Roles = "admin, user")]
        public ActionResult Index()
        {
            IQueryable<ZaprosNew> z1;
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "user")
            {
                var user = User.Identity.Name;
                int userId = (from i in aC.Users where i.Email == user select i.Id).Single();
                z1 = db.ZaprosNews.Where(vM => vM.NumberTickets == userId);
                var rez = z1.ToList();
                return View(rez);
            }
            List<ZaprosNew> z = db.ZaprosNews.ToList();
            return View(z);
        }
    }
}
