package xPackage {	
class xClass {
public static function xFunction () {
                
var Aspectize = {};
var aas = {	Expression:function (p) {}, Controls:{}, Services:{}, Zones:{}, Data:{}, Path:{}, View:{}, ViewName:{}, ColumnType:{}};
function IIF(b, t, f) { return b ? t : f; }
function Count(list) { return 0; }
function Len(s) { return 0; }
function Substring(s, start, length) { return ''; }
function Sum(field) { return 0; }
function Avg(field) { return 0; }
function Min(field) { return 0; }
function Max(field) { return 0; }
function Parent(internalRelationName) {  }
function Child(internalRelationName) {  }
function AspectizeBuildDynamicViews () {

Aspectize.ErrorInFile = 'C:/Users/barry/onedrive/documents/visual studio 2015/Projects/FormationAspectizeProjet1/FormationAspectizeProjet1/Configuration/Views/Upload.js';
var vUpload = Aspectize.CreateView("FileUpload", aas.Controls.FileUpload);

vUpload.Uploader.MultipleFiles.BindData(false); // Changez à true si vous voulez permettre plusieurs fichiers.
vUpload.Uploader.Text.BindData("Choose a file..."); // Texte affiché sur le bouton d'upload.
vUpload.Uploader.ToolTip.BindData("Select a file to upload"); // Infobulle du bouton.
//pload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploaderServiceLocal.UploadFiles(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);
//vUpload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploadShartPointFileService.TeleverserFichiers(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);


//binder les champs du formulaire
vUpload.txtTitreFile.value.BindData(aas.Data.MainData.Attributes.TitreDocument);
vUpload.txtDescription.value.BindData(aas.Data.MainData.Attributes.DescriptionDocument);
vUpload.txtAutreInfo.value.BindData(aas.Data.MainData.Attributes.AutreInfos);
//vUpload.mycheckBox.checked.BindData(aas.Data.MainData.Attributes.isCorrectFOrmulaire)

//executé l'enregsitrement de fichier.
//

vUpload.Uploader.OnFileSelected.BindCommand(aas.Services.Server.UploaderServiceLocal.SelectedFileFunction(vUpload.Uploader.SelectedFile), aas.Data.UploadData, true, true);


//vUpload.btnAddSaveFile.click.BindCommand(aas.Services.Server.UploadFileAndSaveMetaData.UploadAndSave(aas.Data.UploadData.UploadedFichier, vUpload.txtTitreFile.value, vUpload.txtDescription.value, vUpload.txtAutreInfo.value), aas.Data.MainData, true, true);
vUpload.btnAddSaveFile.click.BindCommand(aas.Services.Browser.Utilities.UplodFile(aas.Data.UploadData.UploadedFichier, vUpload.txtTitreFile.value, vUpload.txtDescription.value, vUpload.txtAutreInfo.value));
Aspectize.ErrorInFile = 'C:/Users/barry/onedrive/documents/visual studio 2015/Projects/FormationAspectizeProjet1/FormationAspectizeProjet1/Configuration/Views/View.js';

var mainView = Aspectize.CreateView("MainView", aas.Controls.MainControl);

//mainView.OnActivated.BindCommand(aas.Services.Browser.DataService.ClearData(aas.Path.MainData.Customer));
//mainView.OnActivated.BindCommand(aas.Services.Browser.DataService.AddRowAndSelect(aas.Path.MainData.Customer));
mainView.OnActivated.BindCommand(aas.Services.Server.CustomerService.GetCustomers(aas.Data.MainData), aas.Data.MainData);
mainView.OnActivated.BindCommand(aas.Services.Browser.Utilities.Init());

mainView.grdCustomers.BindGrid(aas.Data.MainData.Customer);
mainView.grdCustomers.className.BindData("table table-hover table-striped");
mainView.grdCustomers.PageSize.BindData(10);

var colFirstName = mainView.grdCustomers.AddGridColumn("FirstName", "Span");
colFirstName.Text.BindData(mainView.grdCustomers.DataSource.firstName);
colFirstName.HeaderText.BindData("Prénom");

var colLastName = mainView.grdCustomers.AddGridColumn("LastName", "Span");
colLastName.Text.BindData(mainView.grdCustomers.DataSource.lastName);
colLastName.HeaderText.BindData("Nom");


var colLastFullName = mainView.grdCustomers.AddGridColumn("FullName", "Span");
colLastFullName.Text.BindData(aas.Expression(mainView.grdCustomers.DataSource.firstName + ' '+ mainView.grdCustomers.DataSource.lastName));
colLastFullName.HeaderText.BindData("Full Name");


mainView.CustomerCount.BindData(aas.Expression(IIF(mainView.grdCustomers.RowCount > 0, mainView.grdCustomers.RowCount + " Clients trouvés", "Aucun client")));

mainView.txtFirstname.value.BindData(aas.Data.MainData.Customer.firstName);
mainView.txtLastname.value.BindData(aas.Data.MainData.Customer.lastName);

mainView.btnAddCustomer.click.BindCommand(aas.Services.Browser.DataService.AddRowAndSelect(aas.Path.MainData.Customer));

mainView.btnDeleteCustomers.click.BindCommand(aas.Services.Browser.DataService.DeleteRow(aas.Path.MainData.Customer));

mainView.btnSaveCustomers.click.BindCommand(aas.Services.Server.DataService.SaveTransactional(aas.Data.MainData, 2));

mainView.btnNaviguerVersUpload.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.FileUpload));

mainView.btnTester.click.BindCommand(aas.Services.Browser.Utilities.Test());

Aspectize.HandleLayout(); }}}}