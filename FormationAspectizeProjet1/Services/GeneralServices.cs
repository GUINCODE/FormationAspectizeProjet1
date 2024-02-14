using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;

namespace FormationAspectizeProjet1.Services
{
    public interface IGeneralServices
    {
        bool IsCorrectInfoFile(UploadedFile[] uploadedFiles, string titreDocument, string description, string autreInfos);
    }

    [Service(Name = "GeneralServices")]
    public class GeneralServices : IGeneralServices //, IInitializable, ISingleton
    {
        public bool IsCorrectInfoFile(UploadedFile[] uploadedFiles, string titreDocument, string description, string autreInfos)
        {
            
            //System.Diagnostics.Debug.WriteLine($"taille tableau de fichier: {uploadedFiles.Length}");
            // Vérifier si l'objet attributes est null
            if (uploadedFiles == null )
            {
                System.Diagnostics.Debug.WriteLine($"tableau de fichier null");

                return false;
            }
            if( uploadedFiles.Length == 0)
            {
                System.Diagnostics.Debug.WriteLine($"tableau de fichier vide");

                return false;
            }

            if(uploadedFiles[0].Stream != null)
            {
                System.Diagnostics.Debug.WriteLine("le strem n'est pas null");
            } else
            {
                System.Diagnostics.Debug.WriteLine("le strem n'est pas null");

            }
            foreach (var file in uploadedFiles)
            {
                System.Diagnostics.Debug.WriteLine($"Nom de fichier: {file.Name}, Taille: {file.ContentLength} bytes");
            }


            System.Diagnostics.Debug.WriteLine($"Taille du tab: {uploadedFiles.Length}");
            System.Diagnostics.Debug.WriteLine($"type de fichier : {uploadedFiles[0].Stream.GetType()}");

            foreach (var file in uploadedFiles)
            {
                System.Diagnostics.Debug.WriteLine($"Nom de fichier: {file.Name}");
            }

            // Vérifier si les champs spécifiques sont nuls ou vides
            bool isTitreDocumentValid = !string.IsNullOrEmpty(titreDocument);
            bool isDescriptionDocumentValid = !string.IsNullOrEmpty(description);
            bool isAutreInfosValid = !string.IsNullOrEmpty(autreInfos);

            // Si tous les champs requis sont non nuls et non vides, retourner true
            
                var result = isTitreDocumentValid && isDescriptionDocumentValid && isAutreInfosValid;
            System.Diagnostics.Debug.WriteLine($"resultat est: {result}");
            return result;
        }


    }

}
