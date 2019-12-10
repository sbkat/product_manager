﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wedding_planner.Models;

namespace wedding_planner.Controllers
{
    public class HomeController : Controller
    {
    private MyContext dbContext;
    public HomeController(MyContext context)
    {
        dbContext = context;
    }
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == newUser.email))
            {
                ModelState.AddModelError("email", "Email is already registered.");
                return View("Index");
            }
            else
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.password = Hasher.HashPassword(newUser, newUser.password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("User", newUser.email);
                return RedirectToAction("Dashboard");
            }
        }
        else
        {
            return View("Index");
        }
    }
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("login")]
    public IActionResult Login(LoginUser existingUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == existingUser.email))
            {
                User userInDb = dbContext.Users.FirstOrDefault(user => user.email == existingUser.email);
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(existingUser, userInDb.password, existingUser.password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                else
                {
                    HttpContext.Session.SetString("User", existingUser.email);
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                ModelState.AddModelError("email", "Email has not been registered.");
                return View("Login");
            }
        }
        else
        {
            return View("Login");
        }
    }
    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            List<Wedding> allWeddings = dbContext.Weddings
                .Include(wedding => wedding.RSVPs)
                // .ThenInclude(rsvp => rsvp.Users)
                .OrderBy(w => w.Date).ToList();
            return View(allWeddings);
        }
    }    
    [HttpGet("wedding")]
    public IActionResult NewWedding()
    {
        return View();
    }    
    [HttpPost("wedding")]
    public IActionResult SubmitNewWedding(Wedding newWedding)
    {
        if(ModelState.IsValid)
        {
            if(newWedding.Date > DateTime.Now)
            {
                User thisUser = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
                newWedding.UserCreated = thisUser;
                dbContext.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                ModelState.AddModelError("Date", "Wedding date must be chosen for the future.");
                return View("NewWedding");
            }
        }
        return View("NewWedding");
    }
    [HttpGet("wedding/{id}")]
    public IActionResult WeddingDetails(int id)
    {
        Wedding thisWedding = dbContext.Weddings.FirstOrDefault(wedding => wedding.WeddingId == id);
        return View(new RSVP{Wedding = thisWedding});
    }
    [HttpPost("RSVP")]
    public IActionResult RSVP(int id, bool going) 
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        RSVP isGoing = dbContext.RSVPs.Where(r => r.RSVPId == id).FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
        if(isGoing==null)
        {
            RSVP newRSVP = new RSVP();
            newRSVP.IsGoing = going;
            newRSVP.RSVPId = id;
            newRSVP.UserId = (int)HttpContext.Session.GetInt32("UserId");
            dbContext.Add(newRSVP);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        if(isGoing.IsGoing == going)
        {
            dbContext.Remove(isGoing);
        }
        else
        {
            isGoing.IsGoing = going;
            dbContext.Update(isGoing);
        }
        dbContext.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Delete(int id)
        {
            Wedding thisWedding = dbContext.Weddings.FirstOrDefault(wedding => wedding.WeddingId == id);
            dbContext.Weddings.Remove(thisWedding);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
