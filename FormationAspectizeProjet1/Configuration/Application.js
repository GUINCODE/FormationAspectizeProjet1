
var app = newApplication();

app.Directories = "Bootstrap, JQueryExtension";
app.VersionInfo = "1";

//app.AddAuthorizationRole(aas.Roles.Anonymous, aas.Enum.AccessControl.ReadWrite);


var ctxData = newContextData();

ctxData.Name = "MainData";
ctxData.NameSpaceList = "FormationAspectizeProjet1";

var ctxDataUpload = newContextData();
ctxDataUpload.Name = "UploadData";
ctxDataUpload.NameSpaceList = "Upload";

