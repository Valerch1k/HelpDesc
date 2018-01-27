using ActionMailer.Net.Mvc;
using HelpDesc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HelpDesc.Controllers
{
    public class EmailProvider : MailerBase
    {
        public EmailResult Subscription(EmailTemplate message, string email)
        {
            To.Add(email);
            Subject = "HelpDesk уведомление";
            MessageEncoding = Encoding.UTF8;
            return Email("Subscription", message);
        }

    }
}