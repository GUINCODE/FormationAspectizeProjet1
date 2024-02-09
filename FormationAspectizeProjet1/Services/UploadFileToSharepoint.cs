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

            string userName = "agb@Guinsoft.onmicrosoft.com";
            string Password = "@BouguiBasta2030";
            var securePassword = new SecureString();
            foreach (char c in Password)
            {
                securePassword.AppendChar(c);
            }
            try
            {
                using (var ctx = new ClientContext("https://guinsoft.sharepoint.com/sites/guicode"))
                {
                    ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(userName, securePassword);
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();
                    List byTitle = ctx.Web.Lists.GetByTitle("MaBibliotheque");

                    // V�rifier si le dossier existe d�j�
                    Folder folder = null;
                    var folders = byTitle.RootFolder.Folders;
                    ctx.Load(folders);
                    ctx.ExecuteQuery();
                    foreach (Folder existingFolder in folders)
                    {
                        if (existingFolder.Name.Equals("FilesUplodedFromAMI", StringComparison.InvariantCultureIgnoreCase))
                        {
                            folder = existingFolder;
                            break;
                        }
                    }

                    // Si le dossier n'existe pas, le cr�er
                    if (folder == null)
                    {
                        ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation
                        {
                            UnderlyingObjectType = FileSystemObjectType.Folder,
                            LeafName = "FilesUplodedFromAMI"
                        };

                        ListItem listItem = byTitle.AddItem(listItemCreationInformation);
                        listItem["Title"] = "FilesUplodedFromAMI";
                        listItem.Update();
                        ctx.ExecuteQuery();

                        // Recharger les dossiers apr�s cr�ation pour obtenir le dossier nouvellement cr��
                        ctx.Load(byTitle.RootFolder.Folders);
                        ctx.ExecuteQuery();
                        folder = byTitle.RootFolder.Folders.FirstOrDefault(f => f.Name == "FilesUplodedFromAMI");
                    }

                    // Logique pour t�l�verser les fichiers dans le dossier...
                    foreach (UploadedFile uploadedFile in uploadedFiles)
                    {
                        // Votre logique d'upload existante ici, en utilisant 'folder' pour d�terminer o� t�l�verser les fichiers
                        try
                        {
                            string fileName = uploadedFile.Name; // Nom du fichier � uploader
                            Stream fileStream = uploadedFile.Stream; // Stream du fichier

                            // Assurez-vous d'�tre connect� au contexte de SharePoint et d'avoir s�lectionn� la biblioth�que
                            // (ici, j'utilise "MaBibliotheque" et le dossier "NewFolderFromCSOM" que vous avez cr�� pr�c�demment)
                            List library = ctx.Web.Lists.GetByTitle("MaBibliotheque");
                            ctx.Load(library.RootFolder);
                            ctx.Load(library.RootFolder.Folders);
                            ctx.ExecuteQuery();

                            Folder newFolder = library.RootFolder.Folders.FirstOrDefault(f => f.Name == "FilesUplodedFromAMI");
                            if (newFolder == null)
                            {
                                throw new Exception("Dossier non trouv�.");
                            }

                            var fileCreationInformation = new FileCreationInformation
                            {
                                ContentStream = fileStream, // Mettez le contenu du fichier ici
                                Url = fileName, // Le nom du fichier dans SharePoint
                                Overwrite = true // �crase le fichier s'il existe d�j�
                            };

                            // Upload le fichier dans le dossier
                            Microsoft.SharePoint.Client.File uploadFile = newFolder.Files.Add(fileCreationInformation);
                            ctx.Load(uploadFile);
                            ctx.ExecuteQuery();
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
                System.Diagnostics.Debug.WriteLine($"Exception de type Exception :  {ex.Message}");
            }
        }


    }
}
