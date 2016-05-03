using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using System.Net;

namespace SportsStore.Domain.Concrete
{
    public class Emailsettings
    {
        public string MailToAddress = "orders@example.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "MySmtpUsername";
        public string Password = "MySmtpPassword";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\sports_store_emails";
    }
    public class EmailOrderProcessor:IOrderprocessor
    {
        private Emailsettings emailsettings;

        public EmailOrderProcessor(Emailsettings settings)
        {
            emailsettings = settings;
        }

        public void ProcessOrder(Cart cart,ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailsettings.UseSsl;
                smtpClient.Host = emailsettings.ServerName;
                smtpClient.Port = emailsettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailsettings.Username, emailsettings.Password);

                if(emailsettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailsettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder().AppendLine("A new order has been submitted").AppendLine("----").AppendLine("Items");

                foreach(var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal:{2:c}", line.Quantity, line.Product.Name, subtotal);
                }
                body.AppendFormat("Total order value: {0:c", cart.ComputeTotalValue())
                    .AppendLine("-----")
                    .AppendLine("ship to:")
                    .AppendLine(shippingInfo.Name)
                    .AppendLine(shippingInfo.line1)
                    .AppendLine(shippingInfo.line2 ?? "")
                    .AppendLine(shippingInfo.line3 ?? "")
                    .AppendLine(shippingInfo.City)
                    .AppendLine(shippingInfo.State ?? "")
                    .AppendLine(shippingInfo.Country)
                    .AppendLine(shippingInfo.zip)
                    .AppendLine("----")
                    .AppendFormat("Gift wrap: {0}", shippingInfo.GiftWrap ? "Yes" : "No");


                MailMessage mailMessage = new MailMessage(emailsettings.MailFromAddress,
                                                        emailsettings.MailToAddress,
                                                        "New Order submitted!", body.ToString());

                if(emailsettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }
}
