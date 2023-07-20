using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace psn.PH
{
    /// <summary>
    /// The EmailExtended_Ext class implements the IEmailExtended_Ext interface, providing
    /// the actual functionality of 
    /// (1) validation of email address
    /// (2) sending of message content to a SMTP server. The message content can be plain text or in HTML format.  
    /// </summary>

    public class EmailExtended_Ext : IEmailExtended_Ext
    {
        // Adapted from https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        /// <summary>
        /// Validate whether an email address is of valid format (RFC 3696 or Section 3.4 of RFC 5322)
        /// </summary>
        /// <param name="email">The email address to be validated.</param>
        /// <returns>returns true if the email address is of valid format else false</returns>
        public bool isValidateEmailAddress_Ext(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
#pragma warning disable CS0168
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
#pragma warning disable CS0168
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    // @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    // Take the regex from https://www.w3resource.com/javascript/form/email-validation.php 
                    @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends an email 
        /// </summary>
        /// <param name="server">The SMTP hostname</param>
        /// <param name="port">The SMTP port number</param>
        /// <param name="username">The username used for SMTP server authentication</param>
        /// <param name="pass">The password used for SMTP server authentication</param>
        /// <param name="from">The sender email address. NOTE: Some SMTP servers do not allow "from" different from "username"</param>
        /// <param name="to">List of email address of (To) receipients</param>
        /// <param name="cc">List of email address of (Cc) receipients</param>
        /// <param name="bcc">List of email address of (Bcc) receipients</param>
        /// <param name="subject">Subject title of the email</param>
        /// <param name="isHtml">true if the content is a HTML based document, false otherwise</param>
        /// <param name="content">Content of the email message</param>
        /// <returns>returns true if the email is sent</returns>
        public bool sendEmail_Ext(string server, int port, string username, string pass, string from, string[] to, string[] cc, string[] bcc, string subject, bool isHtml, string content)
        {
            MailAddress[] ma_to_array = new MailAddress[to.Length];
            for (int i = 0; i < to.Length; i++)
            {
                if (!isValidateEmailAddress_Ext(to[i]))
                {
                    return false;
                }
                ma_to_array[i] = new MailAddress(to[i]);
            }
            MailAddress ma_from = new MailAddress(from);
            MailMessage message = new MailMessage(ma_from, ma_to_array[0]);
            message.Subject = subject;
            message.Body = content;
            message.IsBodyHtml = isHtml;
            if (to.Length > 1)
            {
                for (int i = 1; i < to.Length; i++)
                {
                    message.To.Add(new MailAddress(to[i]));
                }
            }
            if (cc.Length > 0)
            {
                for (int i = 0; i < cc.Length; i++)
                {
                    if (!isValidateEmailAddress_Ext(cc[i]))
                    {
                        return false;
                    }
                    message.CC.Add(new MailAddress(cc[i]));
                }
            }
            if (bcc.Length > 0)
            {
                for (int i = 0; i < bcc.Length; i++)
                {
                    if (!isValidateEmailAddress_Ext(bcc[i]))
                    {
                        return false;
                    }
                    message.Bcc.Add(new MailAddress(bcc[i]));
                }
            }
            using (var smtp = new SmtpClient(server, port))
            {
                smtp.Credentials = new NetworkCredential(from, pass);
                smtp.EnableSsl = true;
                smtp.Send(message);
            }

            return true;
        }
    }
}
