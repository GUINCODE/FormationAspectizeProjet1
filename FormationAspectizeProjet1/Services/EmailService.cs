using System;
using System.Threading.Tasks;
using Aspectize.Core;
using Mailjet.Client;
using Newtonsoft.Json.Linq;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client.TransactionalEmails.Response;

namespace FormationAspectizeProjet1.Services
{
    public interface IEmailService
    {
        Task SendMail(string recipientEmail, string recipientName, string subject, string htmlContent);
    }

    [Service(Name = "EmailService")]
    public class EmailService : IEmailService
    {
        private readonly MailjetClient client;

        public EmailService()
        {
            // Utilisation des variables d'environnement pour une meilleure sécurité
            string apiKey = "a45a4bfcafdb19cdccabf05f4b13abf6";
            string apiSecret = "9d5563a92d8cfc18b89dc4183d0b2df4";

            client = new MailjetClient(apiKey, apiSecret);
        }

        public async Task SendMail(string recipientEmail, string recipientName, string subject, string htmlContent)
        {
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("abarry@et.intechinfo.fr", "Agb coder"))
                .WithSubject(subject)
                .WithHtmlPart(htmlContent)
                .WithTo(new SendContact(recipientEmail, recipientName))
                .Build();

            TransactionalEmailResponse response = await client.SendTransactionalEmailAsync(email);
            if (response.Messages != null && response.Messages.Length > 0)
            {
                var firstMessage = response.Messages[0];
                if (firstMessage.Status == "success")
                {
                    System.Diagnostics.Debug.WriteLine("Email envoyé avec succès.");
                }
                else
                {
                    // Gestion des erreurs
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de l'envoi de l'email. Error: {firstMessage.Errors[0].ErrorMessage}");
                }
            }
            else
            {
                // Traitement en cas de réponse inattendue ou d'erreur non spécifiée
                System.Diagnostics.Debug.WriteLine("Erreur lors de l'envoi de l'email.");
            }


        }
    }
}
