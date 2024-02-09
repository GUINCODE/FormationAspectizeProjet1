var DataService = Aspectize.ConfigureNewService("DataService", aas.ConfigurableServices.DataBaseService);
DataService.DataBaseType = aas.ConfigurableServices.DataBaseService.DBMS.SQLServer2012;
DataService.ConnectionString = "Data Source=localhost\SQLEXPRESS2012;Initial Catalog=db_aspectize_app;User Id=adminSqlServer;Password=P@$$w0rd";


