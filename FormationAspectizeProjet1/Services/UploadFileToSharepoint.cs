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

                    // Vérifier si le dossier existe déjà
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

                    // Si le dossier n'existe pas, le créer
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

                        // Recharger les dossiers après création pour obtenir le dossier nouvellement créé
                        ctx.Load(byTitle.RootFolder.Folders);
                        ctx.ExecuteQuery();
                        folder = byTitle.RootFolder.Folders.FirstOrDefault(f => f.Name == "FilesUplodedFromAMI");
                    }

                    // Logique pour téléverser les fichiers dans le dossier...
                    foreach (UploadedFile uploadedFile in uploadedFiles)
                    {
                        // Votre logique d'upload existante ici, en utilisant 'folder' pour déterminer où téléverser les fichiers
                        try
                        {
                            string fileName = uploadedFile.Name; // Nom du fichier à uploader
                            Stream fileStream = uploadedFile.Stream; // Stream du fichier

                            // Assurez-vous d'être connecté au contexte de SharePoint et d'avoir sélectionné la bibliothèque
                            // (ici, j'utilise "MaBibliotheque" et le dossier "NewFolderFromCSOM" que vous avez créé précédemment)
                            List library = ctx.Web.Lists.GetByTitle("MaBibliotheque");
                            ctx.Load(library.RootFolder);
                            ctx.Load(library.RootFolder.Folders);
                            ctx.ExecuteQuery();

                            Folder newFolder = library.RootFolder.Folders.FirstOrDefault(f => f.Name == "FilesUplodedFromAMI");
                            if (newFolder == null)
                            {
                                throw new Exception("Dossier non trouvé.");
                            }

                            var fileCreationInformation = new FileCreationInformation
                            {
                                ContentStream = fileStream, // Mettez le contenu du fichier ici
                                Url = fileName, // Le nom du fichier dans SharePoint
                                Overwrite = true // Écrase le fichier s'il existe déjà
                            };

                            // Upload le fichier dans le dossier
                            Microsoft.SharePoint.Client.File uploadFile = newFolder.Files.Add(fileCreationInformation);
                            ctx.Load(uploadFile);
                            ctx.ExecuteQuery();
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
                System.Diagnostics.Debug.WriteLine($"Exception de type Exception :  {ex.Message}");
            }
        }


    }
}
