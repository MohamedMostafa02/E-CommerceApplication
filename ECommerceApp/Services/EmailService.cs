using System.Net;
using System.Net.Mail;


namespace ECommerceApp.Services
{
    public class EmailService
    {
        // Configuring property to access application settings.
        private readonly IConfiguration _configuration;

        // Constructor that injects the configuration dependency.
        public EmailService(IConfiguration configuration)
        {
            // Save the configuration object for later use.
            _configuration = configuration;     
        }

        // Method to send an email asynchrounously
        public Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            // Retrieve the mail server (SMTP host) from the configuration.
            string? MailServer = _configuration["EmailSettings:MailServer"];

            // Retrieve the sender email address from the configuration.
            string? FromEmail = _configuration["EmailSettings:FromEmail"];

            // Retrieve teh sender email password from the configuration.
            string? Password = _configuration["EmailSettings:Password"];

            // Retrieve the sender's display name from the configuration.
            string? SenderName = _configuration["EmailSettings:SenderName"];

            // Retrieve the SMTP port from the configuration and convert it to an integer.
            int port = Convert.ToInt32(_configuration["EmailSettings:MailPort"]);


            // Create a new instance of SmtpClient using the mail server and port number.
            var client = new SmtpClient(MailServer, port)
            {
                // Set the credentials (email and password) for the SMTP server.
                Credentials = new NetworkCredential(FromEmail, Password),

                // Enable SSL for secure email communication.
                EnableSsl = true
            };

            // Create a MailAddress object with the sender's email and display
            MailAddress fromAddress = new MailAddress(FromEmail, SenderName);

            // Create a new MailMessage object to define the email's properties.
            MailMessage mailMessage = new MailMessage
            {
                From = fromAddress,      // Set teh sender's email address with display name.
                Subject = Subject,       // Set the email subject line.
                Body = Body,             // Set the email body content.
                IsBodyHtml = IsBodyHtml  // Specify whether the body content is in HTML format.
            };

            // Add the recipient's email address to the message.
            mailMessage.To.Add(ToEmail);

            // Send the email asynchronously using the SmtpClient instance.
            return client.SendMailAsync(mailMessage); 
        }


    }
}
