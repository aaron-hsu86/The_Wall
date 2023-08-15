using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _8_The_Wall.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace _8_The_Wall.Controllers;

[SessionCheck]
public class WallController : Controller
{
    private readonly ILogger<WallController> _logger;
    public MyContext db;
    public WallController(ILogger<WallController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("messages")]
    public IActionResult Index()
    {
        List<Message> allMessages = db.Messages.Include(m => m.Poster).Include(m => m.Comments).ThenInclude(c => c.User).ToList();
        return View(allMessages);
    }

    [HttpPost("messages/create")]
    public IActionResult CreateMessage(Message newMessage)
    {
        if (!ModelState.IsValid)
        {
            List<Message> allMessages = db.Messages.Include(m => m.Poster).Include(m => m.Comments).ThenInclude(c => c.User).ToList();
            return View("Index", allMessages);
        }
        Console.WriteLine("add message to db");
        db.Messages.Add(newMessage);
        db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost("messages/{messageId}/destroy")]
    public IActionResult DestroyMessage(int messageId)
    {
        Message? msgInDB = db.Messages.FirstOrDefault(m => m.MessageId == messageId);
        if (msgInDB == null || msgInDB.UserId != HttpContext.Session.GetInt32("UserId"))
        {
            return RedirectToAction("Index");
        }
        db.Messages.Remove(msgInDB);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost("comments/create")]
    public IActionResult CreateComment(Comment newComment)
    {
        if (!ModelState.IsValid)
        {
            List<Message> allMessages = db.Messages.Include(m => m.Poster).Include(m => m.Comments).ThenInclude(c => c.User).ToList();
            return View("Index", allMessages);
        }

        db.Comments.Add(newComment);
        db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost("comments/{commentId}/destroy")]
    public IActionResult DestroyComment(int commentId)
    {
        Comment? commentInDB = db.Comments.FirstOrDefault(c => c.CommentId == commentId);
        if (commentInDB == null || commentInDB.UserId != HttpContext.Session.GetInt32("UserId"))
        {
            return RedirectToAction("Index");
        }
        db.Comments.Remove(commentInDB);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if(userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}