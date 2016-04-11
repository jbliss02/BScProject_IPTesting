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
using Tools;

namespace ImageAnalysis.Alarms
{
    /// <summary>
    /// Subscribes to an image created event, sends an email to alert about movement
    /// </summary>
    public class EmailAlarm : AlarmBase, IAlarm 
    {
        public string emailAddress { get; set; }

        private int maxEmailImages; //limit the number of attachments in the email

        public EmailAlarm() : base() { maxEmailImages = ConfigurationManager.AppSettings["maxEmailImages"].ToString().StringToInt(); }

        public override void RaiseAlarm()
        {
            SendAlarmEmail();
        }

        /// <summary>
        /// Sends a motion detected email
        /// Most config data is automatically pulled from config files
        /// </summary>
        public void SendAlarmEmail()
        {
            SendEmail(ConfigurationManager.AppSettings["alarmText"]);
        }

        /// <summary>
        /// Sends an email to inform that the alarm is primed and notifications
        /// will be sent if movement is detected
        /// </summary>
        public void SendAlarmPrimedAlarm()
        {
            SendEmail(ConfigurationManager.AppSettings["alarmPrimedText"]);
        }

        private void SendEmail(string emailBody)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

            MailMessage mail = new MailMessage();
            mail.To.Add(emailAddress);
            mail.Subject = ConfigurationManager.AppSettings["alarmEmailSubject"];
            mail.Body = emailBody;

            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password);
            smtp.Timeout = 30000;

            //add the image attachments
            foreach(String imagePath in images)
            {
                if(mail.Attachments.Count >= maxEmailImages) { break; }
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(imagePath);
                mail.Attachments.Add(attachment);
            }

            //send
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
