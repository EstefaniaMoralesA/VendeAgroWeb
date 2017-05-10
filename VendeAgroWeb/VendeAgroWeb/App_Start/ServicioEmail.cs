using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace VendeAgroWeb
{
    public class ServicioEmail
    {
        private string _mailSalida;
        private NetworkCredential _credenciales;
        public ServicioEmail()
        {
            _mailSalida = "buzon@mercampo.mx";
            _credenciales = new NetworkCredential
            {
                UserName = "mercampomx@gmail.com",
                Password = "Mercampomx2306"
            };
        }

        public string MailContacto => "buzon@mercampo.mx";

        public async Task<bool> SendAsync(string mensaje, string asunto, string destinatario)
        {
            // Plug in your email service here to send an email.
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(destinatario));  // replace with valid value
            message.Sender = new MailAddress(_mailSalida, "mercampo.mx");
            message.From = new MailAddress(_mailSalida, "mercampo.mx");  // replace with valid value
            message.ReplyToList.Add(new MailAddress(_mailSalida, "mercampo.mx"));
            message.Subject = asunto;
            message.Body = mensaje;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Credentials = _credenciales;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch(SmtpException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
    }
}