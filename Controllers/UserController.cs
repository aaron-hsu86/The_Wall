using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _8_The_Wall.Models;
using Microsoft.AspNetCore.Identity;

namespace _8_The_Wall.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    public MyContext db;
    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(!ModelState.IsValid)
        {
            return View("Index");
        }

        PasswordHasher<User> hasher = new PasswordHasher<User>();

        newUser.Password = hasher.HashPassword(newUser, newUser.Password);
        db.Users.Add(newUser);
        db.SaveChanges();
        HttpContext.Session.SetInt32("UserId", newUser.UserId);
        HttpContext.Session.SetString("UserName", newUser.FirstName);

        return RedirectToAction("Index", "Wall");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser userCheck)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        User? userInDb = db.Users.FirstOrDefault(u => u.Email == userCheck.EmailCheck);
        if (userInDb == null)
        {
            ModelState.AddModelError("EmailCheck", "Invalid Email/Password");
            ModelState.AddModelError("PasswordCheck", "Invalid Email/Password");
            return View("Index");
        }

        PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

        var result = hasher.VerifyHashedPassword(userCheck, userInDb.Password, userCheck.PasswordCheck);

        if(result == 0)
        {
            ModelState.AddModelError("EmailCheck", "Invalid Email/Password");
            ModelState.AddModelError("PasswordCheck", "Invalid Email/Password");
            return View("Index");
        }

        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
        HttpContext.Session.SetString("UserName", userInDb.FirstName);
        return RedirectToAction("Index", "Wall");
    }

    [HttpPost("logout")]
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
}
