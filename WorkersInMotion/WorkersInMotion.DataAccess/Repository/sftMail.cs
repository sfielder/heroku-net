using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace WorkersInMotion.DataAccess.Repository
{
    public class sftMail
    {
        #region Local Varables
        private string _ToAddress = string.Empty;
        private string _FromAddress = string.Empty;
        private string _CCAddress = string.Empty;
        private string _strbody = string.Empty;
        private string _strToDisplayName = string.Empty;
        private static string _strFromDisplayName = string.Empty;
        private string _strEmailID = string.Empty;
        private string _strFirstName = string.Empty;
        private string _strLastName = string.Empty;
        private string _strJobTitle = string.Empty;
        private string _strCompany = string.Empty;
        private string _strPhone = string.Empty;
        private string _strAddress = string.Empty;
        private string _strNote = string.Empty;
        private string _strSubject = string.Empty;
        private bool _hasAttachments = false;
        private bool _isBodyHtml = true;
        private int _PortNo;

        private string m_cToAddress = ConfigurationManager.AppSettings["toAddress"];
        private string m_cFromAddress = ConfigurationManager.AppSettings["fromEmailId"];
        private string m_cSmtpAddress = ConfigurationManager.AppSettings["smtpAddress"];
        private string m_cPortNo = ConfigurationManager.AppSettings["MailPort"];
        private string m_cSMTPUserName = ConfigurationManager.AppSettings.Get("SMTPUserName");
        private string m_cSMTPPassword = ConfigurationManager.AppSettings.Get("SMTPPassword");

        #endregion

        #region Properties

        public string ToAddress
        {
            get
            {
                return _ToAddress;
            }
            set
            {
                _ToAddress = value;
            }
        }

        public string FromAddress
        {
            get
            {
                return _FromAddress;
            }
            set
            {
                _FromAddress = value;
            }
        }

        public string CCAddress
        {
            get
            {
                return _CCAddress;
            }
            set
            {
                _CCAddress = value;
            }
        }

        public string SmtpHost
        {
            get
            {
                return m_cSmtpAddress;
            }
            set
            {
                m_cSmtpAddress = value;
            }
        }

        public string SMTPUserName
        {
            get { return m_cSMTPUserName; }
            set { m_cSMTPUserName = value; }
        }

        public string SMTPPassword
        {
            get { return m_cSMTPPassword; }
            set { m_cSMTPPassword = value; }
        }

        public string MailBody
        {
            get
            {
                return _strbody;
            }
            set
            {
                _strbody = value;
            }
        }

        public string FromDisplayName
        {
            get
            {
                return _strFromDisplayName;
            }
            set
            {
                _strFromDisplayName = value;
            }
        }

        public string ToDisplayName
        {
            get
            {
                return _strToDisplayName;
            }
            set
            {
                _strToDisplayName = value;
            }
        }

        public string EmailID
        {
            get
            {
                return _strEmailID;
            }
            set
            {
                _strEmailID = value;
            }
        }

        public string FirstName
        {
            get
            {
                return _strFirstName;
            }
            set
            {
                _strFirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return _strLastName;
            }
            set
            {
                _strLastName = value;
            }
        }

        public string JobTitle
        {
            get
            {
                return _strJobTitle;
            }
            set
            {
                _strJobTitle = value;
            }
        }

        public string Company
        {
            get
            {
                return _strCompany;
            }
            set
            {
                _strCompany = value;
            }
        }

        public string Phone
        {
            get
            {
                return _strPhone;
            }
            set
            {
                _strPhone = value;
            }
        }

        public string Address
        {
            get
            {
                return _strAddress;
            }
            set
            {
                _strAddress = value;
            }
        }

        public string Notes
        {
            get
            {
                return _strNote;
            }
            set
            {
                _strNote = value;
            }
        }

        public string MailSubject
        {
            get
            {
                return _strSubject;
            }
            set
            {
                _strSubject = value;
            }
        }

        public bool HasMailAttachments
        {
            get
            {
                return _hasAttachments;

            }

        }

        public bool IsMailBodyHTML
        {
            get
            {
                return _isBodyHtml;
            }
            set
            {
                _isBodyHtml = value;
            }
        }

        public int PortNo
        {
            get
            {
                if (ConfigurationManager.AppSettings.Get("MailPort") != null && (int.TryParse(ConfigurationManager.AppSettings.Get("MailPort").ToString(), out _PortNo)))
                {
                    return _PortNo;
                }
                return _PortNo;
            }
            set { _PortNo = value; }
        }

        #endregion

        #region Public Methods

        public bool SendMail()
        {
            try
            {
                if (this.ToDisplayName == string.Empty)
                {
                    this.ToDisplayName = this.ToAddress;
                }
                MailAddress toAddress = new MailAddress(this.ToAddress, this.ToDisplayName);
                MailAddress fromAddress = new MailAddress(this.FromAddress, this.FromDisplayName);

                MailMessage lmailMessage = new MailMessage(fromAddress, toAddress);
                if (false == string.IsNullOrEmpty(CCAddress))
                {
                    MailAddress lCCAddress = new MailAddress(CCAddress);
                    lmailMessage.CC.Add(lCCAddress);
                }
                lmailMessage.Priority = MailPriority.Normal;
                lmailMessage.Subject = this.MailSubject;
                lmailMessage.IsBodyHtml = this.IsMailBodyHTML;
                if (this.HasMailAttachments)
                {
                }
                lmailMessage.Body = this.MailBody;

                ////Create the SMTPClient
                SmtpClient client = new SmtpClient(m_cSmtpAddress);
                client.Host = this.SmtpHost;
                client.Port = this.PortNo;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(m_cSMTPUserName, m_cSMTPPassword, "Basic");

                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                //client.ConnectType = SmtpConnectType.ConnectSSLAuto;
                //client.SendAsync(lmailMessage, lmailMessage);
                //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                client.Send(lmailMessage);



                //SmtpClient client = new SmtpClient("smtp.office365.com",587);
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.EnableSsl = true;
                ////mail.UseDefaultCredentials = true;  
                //client.UseDefaultCredentials = false;
                //client.Credentials = new NetworkCredential("apps@softtrends.com", "Softtrends@1");
                //MailMessage message = new MailMessage("apps@softtrends.com", "prabhun@softtrends.com");
                //message.Subject = "WorkersInMotion Test Subject";
                //message.Body = lmailMessage.Body;
                //client.Send(message);
                return true;
            }
            catch (SmtpException exe)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public sftMail()
        {
            this.ToAddress = m_cToAddress;
            this.SmtpHost = m_cSmtpAddress;
            this.FromDisplayName = this.FirstName + " " + this.LastName;
            this.ToDisplayName = "";

        }
        public sftMail(string _strTO, string _strFrom)
        {
            this.FromDisplayName = this.FirstName + " " + this.LastName;
            this.SmtpHost = m_cSmtpAddress;
            this.ToAddress = _strTO;
            this.FromAddress = _strFrom;
            this.ToDisplayName = "";
        }
        public static string ComposeMailBody(DictionaryEntry[] dentries)
        {
            StringBuilder sbMailBody = new StringBuilder();
            sbMailBody.Append("<html>");
            sbMailBody.Append("<head></head>");
            sbMailBody.Append("<body>");
            sbMailBody.Append("<table cellspacing=\"2\" cellpadding=\"2\" border=\"0\" width=\"300px\">");
            foreach (DictionaryEntry dentry in dentries)
            {

                sbMailBody.Append("<tr>");
                sbMailBody.Append("<td align=\"left\" width=\"100px\">");
                sbMailBody.Append(dentry.Key.ToString()).Append(":</td>");
                sbMailBody.Append("<td align=\"left\" width=\"200px\">").Append(dentry.Value);
                sbMailBody.Append("</td></tr>");
            }
            sbMailBody.Append("</body>");
            sbMailBody.Append("</table>");

            sbMailBody.Append("</html>");
            return sbMailBody.ToString();
        }
        #endregion.
    }
}