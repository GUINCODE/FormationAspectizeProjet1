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
                    // Activer la journalisation détaillée
                    ctx.ExecutingWebRequest += (sender, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine(e.WebRequestExecutor);
                    };

                    ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(userName, securePassword);
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();

                    List byTitle = ctx.Web.Lists.GetByTitle("MaBibliotheque");

                    // Logique pour téléverser les fichiers dans la bibliothèque...
                    foreach (UploadedFile uploadedFile in uploadedFiles)
                    {
                        try
                        {
                            string fileName = uploadedFile.Name; // Nom du fichier à uploader
                            Stream fileStream = uploadedFile.Stream; // Stream du fichier

                            // Assurez-vous d'être connecté au contexte de SharePoint et d'avoir sélectionné la bibliothèque
                            List library = ctx.Web.Lists.GetByTitle("MaBibliotheque");

                            var fileCreationInformation = new FileCreationInformation
                            {
                                ContentStream = fileStream, // Mettez le contenu du fichier ici
                                Url = fileName, // Le nom du fichier dans SharePoint
                                Overwrite = true // Écrase le fichier s'il existe déjà
                            };

                            // Upload le fichier dans la bibliothèque
                            Microsoft.SharePoint.Client.File uploadFile = library.RootFolder.Files.Add(fileCreationInformation);
                            ctx.Load(uploadFile);
                            ctx.ExecuteQuery();

                            System.Diagnostics.Debug.WriteLine($"Fichier uploder sur sharepoint : {fileCreationInformation.Url}");

                            //ici on appelle la fonction permettant de stocker les méta-données du fichier

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erreur lors de l'upload du fichier dans SharePoint: {ex.Message}");
                            // Gérez l'erreur comme nécessaire
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Une erreur est survenue: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Détails de l'erreur interne: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception de type Exception : {ex.Message}");
            }
        }


      

    }
}
