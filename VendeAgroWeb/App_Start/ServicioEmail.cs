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
            _mailSalida = "santiatlas11@hotmail.com";
            _credenciales = new NetworkCredential
            {
                UserName = "santiatlas11@hotmail.com",
                Password = "FDEZm2306$%"
            };
        }
        public async Task<bool> SendAsync(string mensaje, string asunto, string destinatario)
        {
            // Plug in your email service here to send an email.
            var message = new MailMessage();
            message.To.Add(new MailAddress(destinatario));  // replace with valid value 
            message.From = new MailAddress(_mailSalida);  // replace with valid value
            message.Subject = asunto;
            message.Body = mensaje;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Credentials = _credenciales;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
            return true;
        }
    }
}