using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.Identity.Client;
using Aspectize.Core;

namespace FormationAspectizeProjet1.Services
{
    public interface IUploderByTokenService
    {
        Task UploaderFichiersAsync(UploadedFile[] uploadedFiles);
    }

    [Service(Name = "UploderByTokenService")]
    public class UploderByTokenService : IUploderByTokenService
    {
        private readonly string clientId = "VOTRE_ID_CLIENT";
        private readonly string tenantId = "eb043698-1afe-45c9-80a0-4d13f3ed461e";
        private readonly string clientSecret = "VOTRE_SECRET_CLIENT";
        private readonly string siteUrl = "https://guinsoft.sharepoint.com/sites/guicode";

        public async Task UploaderFichiersAsync(UploadedFile[] uploadedFiles)
        {
            var context = await GetConfiguredContextAsync();
            // Votre logique pour téléverser les fichiers en utilisant context...
            foreach (var uploadedFile in uploadedFiles)
            {
                try
                {
                    var folderUrl = "/sites/guicode/MaBibliotheque/FilesUplodedFromAMI";
                    var folder = context.Web.GetFolderByServerRelativeUrl(folderUrl);
                    context.Load(folder);
                    await context.ExecuteQueryAsync();

                    var fileCreationInfo = new FileCreationInformation
                    {
                        ContentStream = uploadedFile.Stream,
                        Url = Path.Combine(folder.ServerRelativeUrl, uploadedFile.Name),
                        Overwrite = true
                    };

                    var uploadFile = folder.Files.Add(fileCreationInfo);
                    context.Load(uploadFile);
                    await context.ExecuteQueryAsync();

                    // Optionnel: ici, vous pouvez appeler la fonction pour stocker les metadonnées du fichier
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'upload du fichier dans SharePoint: {ex.Message}");
                }
            }
        }

        private async Task<ClientContext> GetConfiguredContextAsync()
        {
            var authResult = await GetAccessTokenAsync();
            var context = new ClientContext(siteUrl)
            {
                ExecutingWebRequest = delegate (object sender, WebRequestEventArgs e)
                {
                    e.WebRequestExecutor.WebRequest.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
                }
            };
            return context;
        }

        private async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            var result = await app.AcquireTokenForClient(new[] { "https://guinsoft.sharepoint.com/.default" }).ExecuteAsync();
            return result;
        }
    }

    
}
