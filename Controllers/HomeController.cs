using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BeltExam.Models;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private BeltExamContext _context;
        public HomeController(BeltExamContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/register")]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult NewUser(User input)
        {
            if (ModelState.IsValid){
                List<User> existingEmails = _context.Users.Where(u => u.Email == input.Email).ToList();
                if (existingEmails.Count > 0){
                    ViewBag.error = "This email already exists";
                    return View("Register");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                input.Password = Hasher.HashPassword(input, input.Password);
                User newUser = new User
                {
                    FName = input.FName,
                    LName = input.LName,
                    Email = input.Email,
                    Password = input.Password,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Register");
        }

        public IActionResult Login(User input)
        {
            if (input.Email != null && input.Password != null){
                User user = _context.Users.SingleOrDefault(u => u.Email == input.Email);
                if (user != null){
                    var Hasher = new PasswordHasher<User>();
                    var result = Hasher.VerifyHashedPassword(user, user.Password, input.Password);
                    if (result != 0){
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        return RedirectToAction("Dashboard");
                    }
                }
                ViewBag.error = "Invalid email/password combination";
                return View("Index");
            }
            ViewBag.error = "Invalid email/password combination";
            return View("Index");
        }

        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            List<Event> events = _context.Events.Include(e => e.Coordinator).Include(e => e.AttendingUsers).Where(e => e.StartingTime > DateTime.Now).OrderBy(e => e.StartingTime).ToList();
            ViewBag.error = TempData["error"];
            ViewBag.user = thisUser;
            ViewBag.events = events;
            return View();
        }

        [HttpGet("/new")]
        public IActionResult New()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult AddEvent(Event input)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid){
                DateTime startingtime = input.Date.Add(input.Time);
                if (startingtime < DateTime.Now){
                    ViewBag.error = "The event must start in the future";
                    return View("New");
                }
                if (input.DurationType == "days"){
                    if (input.DurationNumber == 1){
                        input.DurationType = input.DurationType.Substring(0, input.DurationType.Length-1);
                    }
                    DateTime endingtime = startingtime.AddDays(input.DurationNumber);
                    Event newEvent = new Event
                    {
                        EventName = input.EventName,
                        Description = input.Description,
                        StartingTime = startingtime,
                        EndingTime = endingtime,
                        DurationNumber = input.DurationNumber,
                        DurationType = input.DurationType,
                        CoordinatorId = thisUser.UserId
                    };
                    thisUser.CreatedEvents.Add(newEvent);
                    _context.Add(newEvent);
                    _context.SaveChanges();
                    return RedirectToAction("Display", new { EventId = newEvent.EventId });
                }
                else if (input.DurationType == "hours"){
                    if (input.DurationNumber == 1){
                        input.DurationType = input.DurationType.Substring(0, input.DurationType.Length-1);
                    }
                    DateTime endingtime = startingtime.AddHours(input.DurationNumber);
                    Event newEvent = new Event
                    {
                        EventName = input.EventName,
                        Description = input.Description,
                        StartingTime = startingtime,
                        EndingTime = endingtime,
                        DurationNumber = input.DurationNumber,
                        DurationType = input.DurationType,
                        CoordinatorId = thisUser.UserId
                    };
                    thisUser.CreatedEvents.Add(newEvent);
                    _context.Add(newEvent);
                    _context.SaveChanges();
                    return RedirectToAction("Display", new { EventId = newEvent.EventId });
                }
                else if (input.DurationType == "minutes"){
                    if (input.DurationNumber == 1){
                        input.DurationType = input.DurationType.Substring(0, input.DurationType.Length-1);
                    }
                    DateTime endingtime = startingtime.AddMinutes(input.DurationNumber);
                    Event newEvent = new Event
                    {
                        EventName = input.EventName,
                        Description = input.Description,
                        StartingTime = startingtime,
                        EndingTime = endingtime,
                        DurationNumber = input.DurationNumber,
                        DurationType = input.DurationType,
                        CoordinatorId = thisUser.UserId
                    };
                    thisUser.CreatedEvents.Add(newEvent);
                    _context.Add(newEvent);
                    _context.SaveChanges();
                    return RedirectToAction("Display", new { EventId = newEvent.EventId });
                }
            }
            return View("New");
        }
        [HttpGet("/join/{EventId}")]
        public IActionResult JoinEvent(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            Event thisEvent = _context.Events.SingleOrDefault(e => e.EventId == EventId);
            List<Attending> commitments = _context.Attendings.Include(a => a.Event).Where(a => a.UserId == thisUser.UserId).ToList();
            bool free = true;
            for (int i = 0; i < commitments.Count; i++){
                if (thisEvent.StartingTime > commitments[i].Event.StartingTime && thisEvent.StartingTime < commitments[i].Event.EndingTime || thisEvent.EndingTime > commitments[i].Event.StartingTime && thisEvent.EndingTime < commitments[i].Event.EndingTime){
                    free = false;
                }
            }
            if (free == true){
                Attending newAttending = new Attending  
                {
                    UserId = thisUser.UserId,
                    User = thisUser,
                    EventId = thisEvent.EventId,
                    Event = thisEvent
                };
                thisUser.AttendingEvents.Add(newAttending);
                thisEvent.AttendingUsers.Add(newAttending);
                _context.SaveChanges();
            }
            else{
                TempData["error"] = "That event conflicts with another event you are already attending";
            }            
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/leave/{EventId}")]
        public IActionResult LeaveEvent(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            Event thisEvent = _context.Events.SingleOrDefault(e => e.EventId == EventId);
            Attending thisAttending = _context.Attendings.SingleOrDefault(a => a.UserId == userId && a.EventId == EventId);
            _context.Remove(thisAttending);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/delete/{EventId}")]
        public IActionResult Delete(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            Event thisEvent = _context.Events.SingleOrDefault(e => e.EventId == EventId);
            _context.Remove(thisEvent);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/events/{EventId}")]
        public IActionResult Display(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
            if (thisUser == null){
                return RedirectToAction("Index");
            }
            Event thisEvent = _context.Events.Include(e => e.Coordinator).Include(e => e.AttendingUsers).ThenInclude(a => a.User).SingleOrDefault(e => e.EventId == EventId);
            ViewBag.thisEvent = thisEvent;
            ViewBag.user = thisUser;
            return View();
        }

        [HttpGet("/logoff")]
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
