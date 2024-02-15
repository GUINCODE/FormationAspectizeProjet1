using System;
using System.Security;
using Microsoft.SharePoint.Client;
using Aspectize.Core;
using System.IO;
using System.Net;
using System.Linq;
using Upload;
using System.Threading;

namespace FormationAspectizeProjet1.Services
{
    public interface IUploadFileAndSaveMetaData
    {
        bool UploadAndSave(UploadedFile[] uploadedFiles, string titreDocument, string description, string autreInfos);
    }

    [Service(Name = "UploadFileAndSaveMetaData")]
    public class UploadFileAndSaveMetaData : IUploadFileAndSaveMetaData //, IInitializable, ISingleton
    {
        int tentative = 0;
        
        public bool UploadAndSave(UploadedFile[] uploadedFiles, string titreDocument="null", string description="null", string autreInfos="null")
        {
            tentative++;

            System.Diagnostics.Debug.WriteLine($"Tentative N°{tentative} d'upload de fichier dans sharepoint  ");

            var result = false;
            result =IisCorrectDataReceived(uploadedFiles, titreDocument, description,autreInfos);

            if(result == false)
            {
                System.Diagnostics.Debug.WriteLine("Les données ne sont pas bonnes ");
                return false;
            }

           

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


                            // Répétez pour chaque champ, en exécutant ctx.ExecuteQuery() après chaque mise à jour

                            // Upload le fichier dans la bibliothèque
                            Microsoft.SharePoint.Client.File uploadFile = library.RootFolder.Files.Add(fileCreationInformation);

                            // Important: Chargez l'objet uploadFile et ses ListItemAllFields avant d'assigner des valeurs
                            ctx.Load(uploadFile, file => file.ListItemAllFields);
                            ctx.ExecuteQuery(); // Exécutez la requête pour créer le fichier et charger les champs

                            // Après avoir chargé l'objet, assignez les valeurs aux champs
                            uploadFile.ListItemAllFields["TitreFichier"] = titreDocument.ToString();
                            // Mettez à jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Exécutez la requête pour appliquer les changements

                            uploadFile.ListItemAllFields["DescriptionFichier"] = description.ToString();
                            // Mettez à jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Exécutez la requête pour appliquer les changements

                            uploadFile.ListItemAllFields["Avis"] = autreInfos.ToString();
                            // Mettez à jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Exécutez la requête pour appliquer les changements






                            System.Diagnostics.Debug.WriteLine($"Fichier uploder sur sharepoint : {fileCreationInformation.Url}");

                            //ici on appelle la fonction permettant de stocker les méta-données du fichier
                            SaveMetaData(titreDocument, description, autreInfos);
                            tentative = 0;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            if (tentative >= 5)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'upload du fichier dans SharePoint: {ex.Message}");
                            // Gérez l'erreur comme nécessaire
                            return false;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                                UploadAndSave(uploadedFiles, titreDocument, description, autreInfos);
                            }
                        }
                        
                    }
                }
            }
            catch (WebException ex)
            {
                if(tentative >= 5)
                {
                    System.Diagnostics.Debug.WriteLine($"Une erreur est survenue: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Détails de l'erreur interne: {ex.InnerException.Message}");
                    }
                    tentative = 0;
                    return false;
                }else
                {
                    Thread.Sleep(1000);
                    UploadAndSave(uploadedFiles, titreDocument, description, autreInfos);
                }
                


               
            }
            catch (Exception ex)
            {
                if (tentative >= 5)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception de type Exception : {ex.Message}");
                    tentative = 0;
                    return false;
                }else
                {
                    Thread.Sleep(1000);
                    UploadAndSave(uploadedFiles, titreDocument, description, autreInfos);
                }
                 
            }

            return false;
        }


         bool IisCorrectDataReceived(UploadedFile[] uploadedFiles, string titreDocument, string description, string autreInfos)
        {

            //System.Diagnostics.Debug.WriteLine($"taille tableau de fichier: {uploadedFiles.Length}");
            // Vérifier si l'objet attributes est null
            if (uploadedFiles == null)
            {
                System.Diagnostics.Debug.WriteLine($"tableau de fichier null");

                return false;
            }
            if (uploadedFiles.Length == 0)
            {
                System.Diagnostics.Debug.WriteLine($"tableau de fichier vide");

                return false;
            }

            if (uploadedFiles[0].Stream != null)
            {
              //  System.Diagnostics.Debug.WriteLine("le strem n'est pas null");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("le strem est  null");
                return false;

            }
           // foreach (var file in uploadedFiles)
            //{
              //  System.Diagnostics.Debug.WriteLine($"Nom de fichier: {file.Name}, Taille: {file.ContentLength} bytes");
            //}


          

            // Vérifier si les champs spécifiques sont nuls ou vides
            bool isTitreDocumentValid = !string.IsNullOrEmpty(titreDocument);
            bool isDescriptionDocumentValid = !string.IsNullOrEmpty(description);
            bool isAutreInfosValid = !string.IsNullOrEmpty(autreInfos);

            // Si tous les champs requis sont non nuls et non vides, retourner true

            var result = isTitreDocumentValid && isDescriptionDocumentValid && isAutreInfosValid;
          //  System.Diagnostics.Debug.WriteLine($"resultat est: {result}");
            return result;
        }

         bool SaveMetaData( string titreDocument, string description, string autreInfos)
        {
            IDataManager dm = EntityManager.FromDataBaseService("DataService");
            IEntityManager em = dm as IEntityManager;

            var metaData = em.CreateInstance<DocumentInfo>();
            metaData.Name = titreDocument;
           // metaData.DateAjout = DateTime.Now; // Pour    // ou
            metaData.DateAjout = DateTime.UtcNow; // Pour l'heure UTC
            metaData.AutreInfos = autreInfos;
            metaData.Description = description;
            metaData.Taille = "100 ko";
            metaData.Type = "PDF";

            dm.SaveTransactional();
            System.Diagnostics.Debug.WriteLine("Fonction enregistrement de meta data effectué");
            return true;
        }

    }

}
