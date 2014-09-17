using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web.Configuration;

namespace CrimeBusters.WebApp.Models.Util
{
    /// <summary>
    /// Used to send emails from the application.
    /// </summary>
    public class Email
    { 
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHighImportance { get; set; }
        public Attachment Attachment { get; set; }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <returns>returns "success" if successful.</returns>
        public string SendEmail()
        {
            try
            {
                MailMessage message = CreateMessage();
                SmtpClient smtpClient = SetSmtpCredentials();

                smtpClient.Send(message);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets the MailMessage object that will be sent.
        /// </summary>
        /// <returns>MailMessage object</returns>
        private MailMessage CreateMessage()
        {
            MailMessage message = new MailMessage
            {
                Subject = this.Subject,
                Body = this.Body,
                IsBodyHtml = true,
                Priority = this.IsHighImportance ? MailPriority.High : MailPriority.Normal,
            };

            MailAddress fromMailAddress = new MailAddress(this.FromEmail, this.FromName);            
            MailAddress toMailAddress = new MailAddress(this.ToEmail);
            message.From = fromMailAddress;
            message.To.Add(toMailAddress);

            if (Attachment != null)
            {
                message.Attachments.Add(Attachment);
            }
            return message;
        }

        /// <summary>
        /// Set up the SMTP credentials
        /// </summary>
        /// <returns>SmtpClient object with the credentials</returns>
        private SmtpClient SetSmtpCredentials()
        {
            Configuration configurationFile;
            try
            {
                configurationFile = WebConfigurationManager.OpenWebConfiguration("~/Web.config");
            }
            catch (Exception)
            {
                configurationFile = ConfigurationManager.OpenExeConfiguration("CrimeBusters.WebApp.Tests.dll");
            }

            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
            if (mailSettings == null)
            {
                throw new Exception("Please set the mail settings in the root web.config file.");
            }

            int port = mailSettings.Smtp.Network.Port;
            string host = mailSettings.Smtp.Network.Host;
            string password = mailSettings.Smtp.Network.Password;
            string userName = mailSettings.Smtp.Network.UserName;
            bool defaultCredentials = mailSettings.Smtp.Network.DefaultCredentials;

            SmtpClient smtpClient = new SmtpClient(host, port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = port,
                Host = host,
                UseDefaultCredentials = defaultCredentials,
                Credentials = new NetworkCredential(userName, password)
            };
            return smtpClient;
        }
    }
}