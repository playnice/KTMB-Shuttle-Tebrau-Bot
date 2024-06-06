//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using MailKit;
//using MailKit.Security;
//using System.Net.Mail;


//internal class SmtpEmail
//{
//    static void Main()
//    {

//        var smtpClient = new SmtpClient("smtp.gmail.com")
//        {
//            Port = 587,
//            Credentials = new NetworkCredential("wongzx96@gmail.com", "fnekhicwwzfxuevt"),
//            EnableSsl = true,
//        };

//        smtpClient.Send("wongzx96@gmail.com", "holycowhell3@gmail.com", "KTMB Available Ticket Alert", "But NOW!");
//        Console.WriteLine("Email sent...");
//    }
//}

