var vUpload = Aspectize.CreateView("FileUpload", aas.Controls.FileUpload);

vUpload.Uploader.MultipleFiles.BindData(false); // Changez à true si vous voulez permettre plusieurs fichiers.
vUpload.Uploader.Text.BindData("Choose a file..."); // Texte affiché sur le bouton d'upload.
vUpload.Uploader.ToolTip.BindData("Select a file to upload"); // Infobulle du bouton.
//pload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploaderServiceLocal.UploadFiles(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);
//vUpload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploadShartPointService.UploadFilesToSharePoint(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);
vUpload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploadShartPointFileService.TeleverserFichiers(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);
