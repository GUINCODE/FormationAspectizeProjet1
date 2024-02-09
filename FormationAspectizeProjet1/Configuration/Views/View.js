
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