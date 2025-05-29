using CraftiqueBE.Data.Models.EmailModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Helper.EmailHelper
{
	public class EmailHelper : IEmailHelper
	{
		private readonly EmailConfiguration _emailConfig;

		public EmailHelper(IOptions<EmailConfiguration> emailConfig)
		{
			_emailConfig = emailConfig.Value;
		}

		public async Task SendMailAsync(CancellationToken cancellationToken, EmailRequestModel emailRequest)
		{
			try
			{
				SmtpClient smtpClient = new SmtpClient(_emailConfig.Provider, _emailConfig.Port);
				smtpClient.Credentials = new NetworkCredential(_emailConfig.DefaultSender, _emailConfig.Password);
				smtpClient.UseDefaultCredentials = false;
				smtpClient.EnableSsl = true;

				MailMessage mailMessage = new MailMessage();

				mailMessage.From = new MailAddress(_emailConfig.DefaultSender);
				mailMessage.To.Add(emailRequest.To);
				mailMessage.IsBodyHtml = true;
				mailMessage.Subject = emailRequest.Subject;
				mailMessage.Body = emailRequest.Body;

				if (emailRequest.AttachmentFilePaths.Length > 0)
				{
					foreach (var filePath in emailRequest.AttachmentFilePaths)
					{
						Attachment attachment = new Attachment(filePath);

						mailMessage.Attachments.Add(attachment);
					}
				}

				await smtpClient.SendMailAsync(mailMessage, cancellationToken);

				mailMessage.Dispose();
			}
			catch (Exception ex)
			{
				//log
				throw;
			}
		}
	}
}
