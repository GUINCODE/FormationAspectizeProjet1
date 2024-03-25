using System;
using System.IO;
using System.Data;
using Aspectize.Core;
using Upload;

namespace FormationAspectizeProjet1.Services
{
    public interface IUploaderService
    {
        void UploadFiles(UploadedFile[] uploadedFiles);
        DataSet SelectedFileFunction(UploadedFile[] uploadedFiles);

    }

    [Service(Name = "UploaderServiceLocal")]
    public class UploaderService : IUploaderService
    {
        private readonly string uploadFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploadedFiles");

        public void UploadFiles(UploadedFile[] uploadedFiles)
        {
            try
            {
                // Vérifiez si le répertoire existe; sinon, créez-le
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                    System.Diagnostics.Debug.WriteLine($"Répertoire créé: {uploadFolderPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la création du répertoire: {ex.Message}");
                // Considérez de renvoyer l'exception ou d'arrêter l'exécution ici, selon votre gestion d'erreur
                return; // Stoppez l'exécution si le répertoire ne peut pas être créé
            }

            foreach (UploadedFile uploadedFile in uploadedFiles)
            {
                try
                {
                   // System.Diagnostics.Debug.WriteLine("Dans la boucle foreach");
                    string timestamp = DateTime.Now.ToString("dd_MM_yyyy_H_mm_ss");
                    string fileName = $"{Path.GetFileNameWithoutExtension(uploadedFile.Name)}_{timestamp}{Path.GetExtension(uploadedFile.Name)}";
                    string fullPath = Path.Combine(uploadFolderPath, fileName);

                    // Utilisez l'attribut Stream pour accéder au contenu du fichier
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        uploadedFile.Stream.CopyTo(fileStream);
                        System.Diagnostics.Debug.WriteLine($"Fichier uploadé avec succès: {fileName}  avec le path complet: {fullPath}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de l'upload du fichier {uploadedFile.Name}: {ex.Message}");
                    // Gérez l'erreur comme nécessaire
                }
            }
        }

        DataSet IUploaderService.SelectedFileFunction(UploadedFile[] uploadedFiles)
        {
            IDataManager dm = EntityManager.FromDataBaseService("DataService");
            IEntityManager em = dm as IEntityManager;

            foreach (UploadedFile uploadedFile in uploadedFiles)
            {
                var attachment = em.CreateInstance<UploadedFichier>();

                attachment.Name = uploadedFile.Name;
                attachment.ContentLength = uploadedFile.ContentLength;
                attachment.ContentType = uploadedFile.ContentType;
                attachment.Stream = StreamToArrayByte(uploadedFile.Stream);

                System.Diagnostics.Debug.WriteLine($"attachemet name: {uploadedFile.Name}");
            }

            dm.Data.AcceptChanges();

            return dm.Data;
        }



        public static byte[] StreamToArrayByte(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // Si le stream supporte la recherche, nous nous assurons qu'il est positionné au début.
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            // Crée un BinaryReader pour lire les bytes du stream.
            using (var binaryReader = new BinaryReader(stream))
            {
                byte[] bytes = binaryReader.ReadBytes((int)stream.Length);
                return bytes;
            }
        }
    }
}
