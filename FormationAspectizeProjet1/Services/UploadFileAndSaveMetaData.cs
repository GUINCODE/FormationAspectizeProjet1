using System;
using System.Security;
using Microsoft.SharePoint.Client;
using Aspectize.Core;
using System.IO;
using System.Net;
using System.Linq;
using Upload;
using System.Threading;
using System.Data;

namespace FormationAspectizeProjet1.Services
{
    public interface IUploadFileAndSaveMetaData
    {
        bool UploadAndSave(UploadedFile[] uploadedFiles, string titreDocument, string description, string autreInfos);
        bool TeleverserInSharePoint(UploadedFichier[] uploadedFiles, string titreDocument, string description, string autreInfos);
        bool TeleverserInSharePointDeux(DataSet uploadedFiles, string titreDocument, string description, string autreInfos);
    }

    [Service(Name = "UploadFileAndSaveMetaData")]
    public class UploadFileAndSaveMetaData : IUploadFileAndSaveMetaData //, IInitializable, ISingleton
    {
        int tentative = 0;



       public  bool TeleverserInSharePointDeux(DataSet dataset, string titreDocument = "null", string description = "null", string autreInfos = "null")
        {
            if (dataset == null)
            {
                System.Diagnostics.Debug.WriteLine("Le DataSet 'dataset' est null.");
                return false; // Sortir de la m�thode si dataset est null
            }

            foreach (var property in dataset.GetType().GetProperties())
            {
                try
                {
                    // Tentative d'acc�s � la valeur de la propri�t�
                    var value = property.GetValue(dataset);
                    System.Diagnostics.Debug.WriteLine($"{property.Name}: {value}");
                }
                catch (NullReferenceException ex)
                {
                    // Gestion de l'exception si l'acc�s � la propri�t� �choue
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de l'acc�s � la propri�t� {property.Name}: {ex.Message}");
                }
            }

            return false;
        }



        public bool TeleverserInSharePoint(UploadedFichier[] uploadedFiles, string titreDocument = "null", string description = "null", string autreInfos = "null")
        {
            // V�rifier si uploadedFiles est null avant de continuer
            if (uploadedFiles == null)
            {
                System.Diagnostics.Debug.WriteLine("uploadedFiles est null");
                return false; // Retourner false ou g�rer l'erreur comme n�cessaire
            }

            System.Diagnostics.Debug.WriteLine($"Taille du tableau : {uploadedFiles.Count()}");


            tentative++;

            System.Diagnostics.Debug.WriteLine($"Tentative N�{tentative} d'upload de fichier dans sharepoint  ");

            var result = true;
           // result = IisCorrectDataReceived(uploadedFiles, titreDocument, description, autreInfos);

            if (result == false)
            {
                System.Diagnostics.Debug.WriteLine("Les donn�es ne sont pas bonnes ");
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
                    foreach (UploadedFichier uploadedFile in uploadedFiles)
                    {
                        try
                        {
                            string fileName = uploadedFile.Name; // Nom du fichier � uploader
                            Stream fileStream = ArrayByteToStream(uploadedFile.Stream); // Stream du fichier

                            // Assurez-vous d'�tre connect� au contexte de SharePoint et d'avoir s�lectionn� la biblioth�que
                            List library = ctx.Web.Lists.GetByTitle("MaBibliotheque");

                            var fileCreationInformation = new FileCreationInformation
                            {
                                ContentStream = fileStream, // Mettez le contenu du fichier ici
                                Url = fileName, // Le nom du fichier dans SharePoint
                                Overwrite = true // �crase le fichier s'il existe d�j�
                            };


                            // R�p�tez pour chaque champ, en ex�cutant ctx.ExecuteQuery() apr�s chaque mise � jour

                            // Upload le fichier dans la biblioth�que
                            Microsoft.SharePoint.Client.File uploadFile = library.RootFolder.Files.Add(fileCreationInformation);

                            // Important: Chargez l'objet uploadFile et ses ListItemAllFields avant d'assigner des valeurs
                            ctx.Load(uploadFile, file => file.ListItemAllFields);
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour cr�er le fichier et charger les champs

                            // Apr�s avoir charg� l'objet, assignez les valeurs aux champs
                            uploadFile.ListItemAllFields["TitreFichier"] = titreDocument.ToString();
                            // Mettez � jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour appliquer les changements

                            uploadFile.ListItemAllFields["DescriptionFichier"] = description.ToString();
                            // Mettez � jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour appliquer les changements

                            uploadFile.ListItemAllFields["Avis"] = autreInfos.ToString();
                            // Mettez � jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour appliquer les changements






                            System.Diagnostics.Debug.WriteLine($"Fichier uploder sur sharepoint : {fileCreationInformation.Url}");

                            //ici on appelle la fonction permettant de stocker les m�ta-donn�es du fichier
                            SaveMetaData(titreDocument, description, autreInfos);
                            tentative = 0;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            if (tentative >= 5)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'upload du fichier dans SharePoint: {ex.Message}");
                                // G�rez l'erreur comme n�cessaire
                                return false;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                                TeleverserInSharePoint(uploadedFiles, titreDocument, description, autreInfos);
                            }
                        }

                    }
                }
            }
            catch (WebException ex)
            {
                if (tentative >= 5)
                {
                    System.Diagnostics.Debug.WriteLine($"Une erreur est survenue: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"D�tails de l'erreur interne: {ex.InnerException.Message}");
                    }
                    tentative = 0;
                    return false;
                }
                else
                {
                    Thread.Sleep(1000);
                    TeleverserInSharePoint(uploadedFiles, titreDocument, description, autreInfos);
                }




            }
            catch (Exception ex)
            {
                if (tentative >= 5)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception de type Exception :�{ex.Message}");
                    tentative = 0;
                    return false;
                }
                else
                {
                    Thread.Sleep(1000);
                    TeleverserInSharePoint(uploadedFiles, titreDocument, description, autreInfos);
                }

            }

            return false;
        }

        public bool UploadAndSave(UploadedFile[] uploadedFiles, string titreDocument="null", string description="null", string autreInfos="null")
        {
            tentative++;

            System.Diagnostics.Debug.WriteLine($"Tentative N�{tentative} d'upload de fichier dans sharepoint  ");

            var result = false;
            result =IisCorrectDataReceived(uploadedFiles, titreDocument, description,autreInfos);

            if(result == false)
            {
                System.Diagnostics.Debug.WriteLine("Les donn�es ne sont pas bonnes ");
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


                            // R�p�tez pour chaque champ, en ex�cutant ctx.ExecuteQuery() apr�s chaque mise � jour

                            // Upload le fichier dans la biblioth�que
                            Microsoft.SharePoint.Client.File uploadFile = library.RootFolder.Files.Add(fileCreationInformation);

                            // Important: Chargez l'objet uploadFile et ses ListItemAllFields avant d'assigner des valeurs
                            ctx.Load(uploadFile, file => file.ListItemAllFields);
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour cr�er le fichier et charger les champs

                            // Apr�s avoir charg� l'objet, assignez les valeurs aux champs
                            uploadFile.ListItemAllFields["TitreFichier"] = titreDocument.ToString();
                            // Mettez � jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour appliquer les changements

                            uploadFile.ListItemAllFields["DescriptionFichier"] = description.ToString();
                            // Mettez � jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour appliquer les changements

                            uploadFile.ListItemAllFields["Avis"] = autreInfos.ToString();
                            // Mettez � jour l'objet ListItem pour enregistrer les changements
                            uploadFile.ListItemAllFields.Update();
                            ctx.ExecuteQuery(); // Ex�cutez la requ�te pour appliquer les changements






                            System.Diagnostics.Debug.WriteLine($"Fichier uploder sur sharepoint : {fileCreationInformation.Url}");

                            //ici on appelle la fonction permettant de stocker les m�ta-donn�es du fichier
                            SaveMetaData(titreDocument, description, autreInfos);
                            tentative = 0;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            if (tentative >= 5)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'upload du fichier dans SharePoint: {ex.Message}");
                            // G�rez l'erreur comme n�cessaire
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
                        System.Diagnostics.Debug.WriteLine($"D�tails de l'erreur interne: {ex.InnerException.Message}");
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
                    System.Diagnostics.Debug.WriteLine($"Exception de type Exception :�{ex.Message}");
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
            // V�rifier si l'objet attributes est null
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


          

            // V�rifier si les champs sp�cifiques sont nuls ou vides
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
            System.Diagnostics.Debug.WriteLine("Fonction enregistrement de meta data effectu�");
            return true;
        }


        public static Stream ArrayByteToStream(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            // Cr�e un MemoryStream � partir du tableau de bytes.
            // Notez que le MemoryStream prend possession du tableau de bytes fourni,
            // donc il ne faut pas le modifier apr�s avoir cr�� le stream.
            MemoryStream stream = new MemoryStream(bytes);

            // Positionne le MemoryStream au d�but pour garantir que la lecture commencera depuis le d�but.
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

    }

}
