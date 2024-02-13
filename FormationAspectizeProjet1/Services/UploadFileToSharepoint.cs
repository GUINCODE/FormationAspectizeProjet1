using System;
using System.Security;
using Microsoft.SharePoint.Client;
using Aspectize.Core;
using System.IO;
using System.Net;
using System.Linq;

namespace FormationAspectizeProjet1.Services
{
    public interface IUploadShartPointFileService
    {
        void TeleverserFichiers(UploadedFile[] uploadedFiles);
    }

    [Service(Name = "UploadShartPointFileService")]
    public class UploadShartPointFileService : IUploadShartPointFileService
    {

        public void TeleverserFichiers(UploadedFile[] uploadedFiles)
        {
            // Forcer l'utilisation de TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string userName = "agb@gncoder.onmicrosoft.com";
            string Password = "@P@$$w@rd2030";
            var securePassword = new SecureString();
            foreach (char c in Password)
            {
                securePassword.AppendChar(c);
            }

            try
            {
                using (var ctx = new ClientContext("https://gncoder.sharepoint.com/sites/guincode"))
                {
                    // Activer la journalisation d�taill�e
                    ctx.ExecutingWebRequest += (sender, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine(e.WebRequestExecutor);
                    };

                    ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(userName, securePassword);
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();

                    List byTitle = ctx.Web.Lists.GetByTitle("MaBibliotheque");

                    // Logique pour t�l�verser les fichiers dans la biblioth�que...
                    foreach (UploadedFile uploadedFile in uploadedFiles)
                    {
                        try
                        {
                            string fileName = uploadedFile.Name; // Nom du fichier � uploader
                            Stream fileStream = uploadedFile.Stream; // Stream du fichier

                            // Assurez-vous d'�tre connect� au contexte de SharePoint et d'avoir s�lectionn� la biblioth�que
                            List library = ctx.Web.Lists.GetByTitle("MaBibliotheque");

                            var fileCreationInformation = new FileCreationInformation
                            {
                                ContentStream = fileStream, // Mettez le contenu du fichier ici
                                Url = fileName, // Le nom du fichier dans SharePoint
                                Overwrite = true // �crase le fichier s'il existe d�j�
                            };

                            // Upload le fichier dans la biblioth�que
                            Microsoft.SharePoint.Client.File uploadFile = library.RootFolder.Files.Add(fileCreationInformation);
                            ctx.Load(uploadFile);
                            ctx.ExecuteQuery();

                            System.Diagnostics.Debug.WriteLine($"Fichier uploder sur sharepoint : {fileCreationInformation.Url}");

                            //ici on appelle la fonction permettant de stocker les m�ta-donn�es du fichier

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erreur lors de l'upload du fichier dans SharePoint: {ex.Message}");
                            // G�rez l'erreur comme n�cessaire
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Une erreur est survenue: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"D�tails de l'erreur interne: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception de type Exception :�{ex.Message}");
            }
        }


      

    }
}
