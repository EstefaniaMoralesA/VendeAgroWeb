using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace VendeAgroWeb
{
    public class ServicioEmail
    {
        public ServicioEmail()
        {
        }

        public string MailContacto => "buzon@mercampo.mx";

        public async Task<bool> SendAsync(string mensaje, string asunto, string destinatario)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://mercampomail.azurewebsites.net/api/mail/send/" +
                destinatario + "/" + asunto);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("body=" + mensaje);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                if (result.Equals("Error"))
                {
                    httpResponse.Close();
                    return false;
                }
            }
            httpResponse.Close();
            return true;
        }
    }
}