var vUpload = Aspectize.CreateView("FileUpload", aas.Controls.FileUpload);

vUpload.Uploader.MultipleFiles.BindData(false); // Changez à true si vous voulez permettre plusieurs fichiers.
vUpload.Uploader.Text.BindData("Choose a file..."); // Texte affiché sur le bouton d'upload.
vUpload.Uploader.ToolTip.BindData("Select a file to upload"); // Infobulle du bouton.
//pload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploaderServiceLocal.UploadFiles(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);
//vUpload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploadShartPointFileService.TeleverserFichiers(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);

//vUpload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploaderServiceLocal.SelectedFileFunction(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);

//binder les champs du formulaire
vUpload.txtTitreFile.value.BindData(aas.Data.MainData.Attributes.TitreDocument);
vUpload.txtDescription.value.BindData(aas.Data.MainData.Attributes.DescriptionDocument);
vUpload.txtAutreInfo.value.BindData(aas.Data.MainData.Attributes.AutreInfos);
//vUpload.mycheckBox.checked.BindData(aas.Data.MainData.Attributes.isCorrectFOrmulaire)

//executé l'enregsitrement de fichier.
//
//vUpload.btnAddSaveFile.click.BindCommand(aas.Services.Server.GeneralServices.IsCorrectInfoFile(vUpload.Uploader.SelectedFile, vUpload.txtTitreFile.value, vUpload.txtDescription.value, vUpload.txtAutreInfo.value), aas.Data.MainData, true, true);


vUpload.btnAddSaveFile.click.BindCommand(aas.Services.Server.UploadFileAndSaveMetaData.UploadAndSave(vUpload.Uploader.SelectedFile, vUpload.txtTitreFile.value, vUpload.txtDescription.value, vUpload.txtAutreInfo.value), aas.Data.MainData, true, true);

