using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace psn.PH
{
    /// <summary>
    /// The EmailExtended_Ext class implements the IEmailExtended_Ext interface, providing
    /// the actual functionality of 
    /// (1) validation of email address
    /// (2) sending of message content to a SMTP server. The message content can be plain text or in HTML format.  
    /// 
    /// To run this test, you should have a smtp server. 
    /// You can use something like https://github.com/rnwood/smtp4dev/wiki/Installation
    /// The settings that is used is found in configuration/appsettings.json
    /// </summary>

    public class EmailExtended_Ext : IEmailExtended_Ext
    {
        private bool SMTP_IGNORE_EMAIL_ADDRESS_VALIDATION = false;
        // Adapted from https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        /// <summary>
        /// Validate whether an email address is of valid format (RFC 6530)
        /// </summary>
        /// <param name="email">The email address to be validated.</param>
        /// <returns>returns true if the email address is of valid format else false</returns>
        public bool isValidateEmailAddress_Ext(string email)
        {
            // switch to ignore email validation
            if (SMTP_IGNORE_EMAIL_ADDRESS_VALIDATION)
            {
                return true;
            }

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
        /// Sends an email with server certificate validation
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
        /// <param name="ignoreServerCertificateValidation">If true, will not perform SMTP server certificate validation. Default false</param>
        /// <returns>returns true if the email is sent</returns>
        public bool sendEmail_Ext(string server, int port, string username, string pass, string from, string[] to, string[] cc, string[] bcc, string subject, bool isHtml, string content)
        {
            return this.sendEmail_Ext(server, port, username, pass, from, to, cc, bcc, subject, isHtml, content, true, false);
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
        /// <param name="ignoreServerCertificateValidation">If true, will not perform SMTP server certificate validation. Default false</param>
        /// <returns>returns true if the email is sent</returns>
        public bool sendEmail_Ext(string server, int port, string username, string pass, string from, string[] to, string[] cc, string[] bcc, string subject, bool isHtml, string content, bool ignoreServerCertificateValidation, bool ignoreEmailAddressValidation)
        {
            SMTP_IGNORE_EMAIL_ADDRESS_VALIDATION = ignoreEmailAddressValidation;
            MailAddress[] ma_to_array = new MailAddress[to.Length];
            for (int i = 0; i < to.Length; i++)
            {
                if (!isValidateEmailAddress_Ext(to[i]))
                {
                    return false;
                }
                ma_to_array[i] = new MailAddress(to[i]);
            }
            MailAddress ma_from;
            if (isValidateEmailAddress_Ext(username) && (from.Trim().Length == 0 || !isValidateEmailAddress_Ext(from)))
            {
                ma_from = new MailAddress(username);
            }
            else
            {
                if (isValidateEmailAddress_Ext(from))
                {
                    ma_from = new MailAddress(from);
                }
                else
                {
                    return false; // the username is not a valid email address and the from address is invalid too
                }
            }

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
            string hostName = server.Equals("secure-gateway") ? Environment.GetEnvironmentVariable("SECURE_GATEWAY") ?? "hostname-undefined" : server;
            using (var smtp = new SmtpClient(hostName, port))
            {
                var scvcb = ServicePointManager.ServerCertificateValidationCallback;
                if (ignoreServerCertificateValidation)
                {
                    // ignore server certificate validation in case of self-signed certs
                    ServicePointManager.ServerCertificateValidationCallback =
                            (sender, certificate, chain, sslPolicyErrors) =>
                            {
                                return true;
                            };

                }

                smtp.Credentials = new NetworkCredential(from, pass);
                smtp.EnableSsl = true;
                smtp.Send(message);
                // restore ServicePointManager.ServerCertificateValidationCallback 
                // if it was ignore for this particular request so that subsequent 
                // calls will revert back to validation by default
                ServicePointManager.ServerCertificateValidationCallback = scvcb;
            }

            return true;
        }

        public string getBuildInfo_Ext()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var filePath = assembly.Location;
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(bytes, 0, bytes.Length);
            }

            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(secondsSince1970).ToLocalTime().ToString("yyyyMMddHHmmss");
        }
    }
}
