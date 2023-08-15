#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace _8_The_Wall.Models;
public class Message
{
    [Key]
    public int MessageId {get;set;}

    [Required]
    [MaxLength(45, ErrorMessage = "Please keep message under 45 characters")]
    public string MessageText {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int UserId {get;set;}

    public User? Poster {get;set;}

    public List<Comment> Comments {get;set;} = new List<Comment>();
}