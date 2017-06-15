using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;

namespace GameStore.Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings _emailSettings;

        public EmailOrderProcessor(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = _emailSettings.UseSsl;
                smtpClient.Host = _emailSettings.ServerName;
                smtpClient.Port = _emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

                if (_emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder();
                body.AppendLine("Новый заказ обработан").AppendLine("---").AppendLine("Товары:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Game.Price*line.Quantity;
                    body.AppendFormat("{0} x {1} (итого: {2:c}",
                        line.Quantity, line.Game.Name, subtotal);
                }

                body.AppendFormat("Общая стоимость: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Доставка:").AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.Country)
                    .AppendLine("---")
                    .AppendFormat("Подарочная упаковка: {0}",
                        shippingDetails.GiftWrap ? "Да" : "Нет");

                MailMessage mailMessage = new MailMessage(_emailSettings.MailFromAddress,_emailSettings.MailToAddress, "Новый заказ!", body.ToString());
                if (_emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                }

                smtpClient.Send(mailMessage);
            }

        }


    }

    public class EmailSettings
    {
        public string MailToAddress = "your@gmail.com";
        public string MailFromAddress = "your@gmail.com";
        public bool UseSsl = true;
        public string Username = "your@gmail.com";
        public string Password = "your_GMAIL_pass_for_app_https://security.google.com/settings/security/apppasswords";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\game_store_emails"; //need to create this folder beforehand
    }
}
