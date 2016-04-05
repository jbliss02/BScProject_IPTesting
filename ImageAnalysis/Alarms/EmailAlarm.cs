using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images;
using ImageAnalysis.Images.Jpeg;
using System.Net.Mail;
using System.Net;
using System.Net.Configuration;
using System.Configuration;
namespace ImageAnalysis.Alarms
{
    /// <summary>
    /// Subscribes to an image created event, sends an email to alert about movement
    /// </summary>
    public class EmailAlarm : AlarmBase, IAlarm 
    {
        public string emailAddress { get; set; }

        public override void OnImageExtracted()
        {

        }

        /// <summary>
        /// Sends a motion detected email
        /// Most config data is automatically pulled from config files
        /// </summary>
        public void SendEmail()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");


            MailMessage mail = new MailMessage();
            mail.To.Add(emailAddress);
            mail.Subject = "Motion Alarm";
            mail.Body  = "Motion has been detected";

            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password);
            smtp.Timeout = 30000;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mail.Dispose();
            }

        }

    }
}
