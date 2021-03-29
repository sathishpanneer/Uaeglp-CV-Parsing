using System;
using System.Net;
using System.Net.Mail;

namespace Uaeglp.Utilities
{
	public class EMailProvider : IDisposable
	{
		private string _username;
		private string _password;
		private string _host;
		private string _port;
		private Random _random = new Random();

		public EMailProvider(string username, string password, string host, string port)
		{
			_host = host;
			_port = port;
			_username = username;
			_password = password;
		}
		public bool SendOtp(string fromEmail, string toEmail, int? OTP, string UserName, String MailType)
		{
			try
			{
				var subject = "Confirm OTP";
				var body = OTP.ToString();

				if (MailType == "UserResgistration")
				{
					subject = "Qiyadat - Login using Security Code ";
				}
				else if (MailType == "ResendOtp")
				{
					subject = "Qiyadat - Login using Security Code";
				}
				else if (MailType == "ForgotPass")
				{
					subject = "Qiyadat - Login using Security Code";
				}


				using (MailMessage message = new MailMessage(fromEmail, toEmail))
				{
					message.Subject = subject;
					if (MailType == "UserResgistration")
					{
						//message.Body = "<table lang = \"en\" bgcolor = \"#ffffff\" width = \"90%\" style = \"background-color:white !Important; width: 90% !Important; direction:ltr !Important;text-align: left !Important;font-family: Helvetica, sans-serif !Important; font-size: 25px !Important; border-bottom-color: white !Important;padding: 5% auto auto auto !Important;\" cellspacing = \"0\" align = \"center\"> " +
						//		   "<tbody> <tr> <td width = \"10%\"></td> <td style = \"text-align:left !important;\"> <p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> Hi " + UserName + ",</p> " +
						//		   "<p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> Thanks for verifying your account! <p> " +
						//			"<p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> Here is your security code: " + OTP + "</p>" +
						//			"<p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p> " +
						//		   "<p style = \"margin-top:30px !important;font-size:18px !important;\"> Regards,<br> UAE GLP TEAM </p> </td> <td width = \"10%\"></td> </tr></tbody></table>";

						message.Body = "<table lang = \"en\" bgcolor = \"#ffffff\" width = \"90%\" style = \"background-color:white !Important; width: 90% !Important; direction:ltr !Important;text-align: left !Important;font-family: Helvetica, sans-serif !Important; font-size: 25px !Important; border-bottom-color: white !Important;padding: 5% auto auto auto !Important;\" cellspacing = \"0\" align = \"center\"> " +
								   "<tbody> <tr> <td width = \"10%\"></td> <td style = \"text-align:left !important;\"> <p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> Hi " + UserName + ",</p> " +
								   "<p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> Confirm the Security Code : " + OTP + "</p>" +
									"</td> <td width = \"10%\"></td> </tr></tbody></table>";
						//Confirm the Security Code

						message.IsBodyHtml = true;
					}
                    else if (MailType == "ForgotPass" || MailType == "ResendOtp")
                    {
                        message.Body = "<table lang = \"en\" bgcolor = \"#ffffff\" width = \"90%\" style = \"background-color:white !Important; width: 90% !Important; direction:ltr !Important;text-align: left !Important;font-family: Helvetica, sans-serif !Important; font-size: 25px !Important; border-bottom-color: white !Important;padding: 5% auto auto auto !Important;\" cellspacing = \"0\" align = \"center\"> " +
                                       "<tbody> <tr> <td width = \"10%\"></td> <td style = \"text-align:left !important;\"> <p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> Hi " + UserName + ",</p> " +
									   "<p style = \"margin-bottom:20px !important;font-size: 18px !important;\"> You have requested to reset your password.Confirm the Security Code : " + OTP + "</p>" +
                                       "</td> <td width = \"10%\"></td> </tr></tbody></table>";
                        //Confirm the Security Code

                        message.IsBodyHtml = true;
					}
                    else
					{
						message.Body = body;
						message.IsBodyHtml = false;
					}

					using (var smtp = new SmtpClient())
					{
						smtp.Host = _host;
						smtp.EnableSsl = true;
						NetworkCredential NetworkCred = new NetworkCredential(_username, _password);
						smtp.UseDefaultCredentials = true;
						smtp.Credentials = NetworkCred;
						smtp.Port = int.Parse(_port);
						smtp.Send(message);
					}
					//System.Threading.Thread.Sleep(3000);
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public int GenerateOtp()
		{
			return _random.Next(1000, 9999);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~EMailProvider()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
