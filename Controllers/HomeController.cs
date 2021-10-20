using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController( MyContext context)
        {
            _context = context;
        }

        //======================
        //  Reg / Login Page
        //======================
        public IActionResult Index()
        {
            return View("RegLoginHome");
        }

        //========================
        //  Register Post Route
        //========================
        [HttpPost("register")]
        public IActionResult RegisterUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                //thus searches for User Email and returns bool becaue of "Any."
                if(_context.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Register");
                }

                // Console.WriteLine(newUser.Password);
                PasswordHasher<User> Hasher = new PasswordHasher<User>(); 
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                // Console.WriteLine(newUser.Password);
                
                // Adds User to DB
                _context.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("LoggedUserId", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        //========================
        //  Login Post Route
        //========================
        [HttpPost("login")]
        public IActionResult Login(LoginUser checkMe)
        {
            if(ModelState.IsValid)
            {
                //find User with Email
                User userInDb = _context.Users.FirstOrDefault(use => use.Email == checkMe.LoginEmail);
                //if User doesn't exist
                    //send Validation Error
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login");

                    return View("LoginPage");
                }
                //verify Password
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(checkMe, userInDb.Password, checkMe.LoginPassword);
                //if wrong Password
                    //send Validation error
                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login");
                    return View("LoginPage");
                }
                //put user id in session
                HttpContext.Session.SetInt32("LoggedUserId", userInDb.UserId);

                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        //=======================
        //  Dashboard Page
        //=======================
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            //pervents a jump into Success Page
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            // if(loggedUserId == null) return RedirectToAction("LoginPage");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.AllWeddings = _context.Weddings
                .Include(wed => wed.Rsvp)
                .ThenInclude(rsvp => rsvp.User)
                .ToList();

            ViewBag.User = Convert.ToInt32(loggedUserId);

            return View();
        }

        //======================
        //   Logout Btn
        //======================
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        //================================
        //  Make New Wedding & Post Route
        //================================
        [HttpGet("wedding/new")]
        public IActionResult NewWedding()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.CreatorId = Convert.ToInt32(loggedUserId);
            
            return View();
        }

        [HttpPost("/wedding/create")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            if(ModelState.IsValid)
            {
                _context.Add(newWedding);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }
            return View("NewWedding");
        }

        //====================
        //  View A Wedding
        //====================
        [HttpGet("/wedding/{id}")]
        public IActionResult ViewWedding(int id)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.SingleWedding = _context.Weddings
                .Include(wed => wed.Rsvp)
                .ThenInclude(rsvp => rsvp.User)
                .FirstOrDefault(wed => wed.WeddingId == id);

            return View();
        }

        //=================
        //     RSVP
        //=================
        [HttpGet("/rsvp/{WeddingId}")]
        public IActionResult Rsvp(int WeddingId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId == null) return RedirectToAction("Index");

            bool rsvpExist = _context.Rsvps.Any(rsvp => rsvp.UserId == loggedUserId && rsvp.WeddingId == WeddingId);
            if(!rsvpExist)
            {
                Rsvp newRsvp = new Rsvp();
                newRsvp.UserId = (int)loggedUserId;
                newRsvp.WeddingId = WeddingId;

                _context.Add(newRsvp);
                _context.SaveChanges();
            }
            else
            {
                Rsvp unRsvp = _context.Rsvps.FirstOrDefault(rsvp => rsvp.UserId == loggedUserId && rsvp.WeddingId == WeddingId);

                _context.Rsvps.Remove(unRsvp);
                _context.SaveChanges();
            }
            
            return RedirectToAction ("Dashboard");
        }

        //==================
        // RSVP Delete
        //==================
        [HttpGet("wedding/{WeddingId}/Delete")]
        public IActionResult DeleteRsvp(int WeddingId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId == null) return RedirectToAction("Index");

        
            Wedding DeleteMe = _context.Weddings
                .FirstOrDefault(maker => maker.WeddingId == WeddingId);

            if(DeleteMe.CreatorId != loggedUserId)
            {
                return RedirectToAction ("Dashboard");
            }
            
            _context.Remove(DeleteMe);
            _context.SaveChanges();
            
            return RedirectToAction ("Dashboard");
        }






        //==================================
        //==================================
        //==================================
        ///Existing Template stuffs
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
