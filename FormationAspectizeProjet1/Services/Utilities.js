
Global.Utilities = {

    aasService: 'Utilities',
    aasPublished: true,

    Init: function () {
        console.log("Initialisation de l'application")
    },

    Test: function () {
        console.log("Test function")

    },

    IsCorrectFileIfo: function (data) {
        console.log("data: ", data)
    },

    UplodFile: function (dataset, titre, description, autreInfos) {

        var cmd = Aspectize.Host.PrepareCommand();

        console.log("datafund: ");
        console.log(dataset);

        console.log("titre: " + titre + ", desc: " + description + ", autreInfos: " + autreInfos);

       
        console.log("HAHAHAH", vUpload.txtAutreInfo.value)
   
        
            //ici je souhaite afficher la structure de file:

            cmd.Attributes.aasAsynchronousCall = true;
            cmd.Attributes.aasShowWaiting = true;

            cmd.OnComplete = function (result) {

                if (!result) {
                   console.log("Fichier uploader avec succès")
                }
                else {
                    console.log("Erreur d'upload fichier ")
                }
            };

            cmd.Call('Server/UploadFileAndSaveMetaData.TeleverserInSharePointDeux', dataset, titre, description, autreInfos);

        
    }


    

};

