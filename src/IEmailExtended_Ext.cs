using OutSystems.ExternalLibraries.SDK;

namespace psn.PH
{
    /// <summary>
    /// The IEmailExtended_Ext interface defines the methods (exposed as server actions)
    /// for validating email address and sending emails (in plain text format or HTML format)
    /// </summary>
    [OSInterface(Description = "Provides validation of email address and sending of messages in plain text or HTML format to a SMTP server.", Name = "EmailExtended_Ext", IconResourceName = "psn.PH.EmailExtIcon.png")]
    public interface IEmailExtended_Ext
    {
        /// <summary>
        /// Validates an email address (RFC 6530). True if valid, False otherwise.
        /// </summary>
        [OSAction(Description = "Validates an email address.", ReturnName = "isValid")]
        public bool isValidateEmailAddress_Ext(string address);
        /// <summary>
        /// Send an email message (either in plain text format or HTML format) to a list of receipients. 
        /// </summary>
        [OSAction(Description = "Send an email message through a SMTP server.", ReturnName = "hasSent")]
        public bool sendEmail_Ext(string server, int port, string username, string pass, string from, string[] to, string[] cc, string[] bcc, string subject, bool isHtml, string content);

    }
}