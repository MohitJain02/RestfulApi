Asp.net core notes
This is the new framework that make the application runs on any of the plateform like windows , linux etc.
3framework is aspnetcore1.2
Changes
1) No use of global.asax file instead we use the startup.cs file to set the configuration for the application 
2) the file system in the disk is exactly similiar to the solution explorer inside the  project and if we make any changes to the file or its content add/edit then from the file system then the changes will be reflected to the browser as the asp.net will automatically compile the application when it encounters any changes as it monitors the application
3) there are 2 methods inside the startup.cs file
 Configure() this method is where we define the http processing piepline and handle the incoming request 
 ConfigureService() this method is where we set up the application components such as inversion of control and dependency injection
 4) There is no web.config to access the application configuration file
 5) We have the configurationBuilder() method to build the configuration that we want to build and  ind to the configuration
 6) To there are built in services that helps the aspnet to register them even before starting the startup.cs file 
 To resgister our customer service we need to register that inside the configureservice() method of the startup.cs file.
 7) There are 3 types of api available that defines the scope 
 Scoped - This will be unique for each http request but for same http request the same instance of the service will be used
 Singlton - Same instance of the service will be avaialbe through out the application
 Transient - New instance will be created for each time the service is being required
MiddleWare in asp.net core
MIddleware is the series of components that are all in the processing pipeline
For ex : Logger , Authorizer , Router middlware
It is the place where the http request gets processed in the pipeline.
It allows how to display the error message to the user 
how to apply the authentication and authorization for the incoming request
UseDeveloperException middleware is used in the development environment that waits for the reponse and if the rreponse contains any error then catches the error ans dispplays the developer the complete information in the browser along with the stack trace and the line no on which the exception occures
UseExceptionHandler() middleware is to have the custom exception in the production instead of the development env it can take any file /page which contains the custom error message or you can use the exceptionhandleroptions to create the meaningfull exception and it returns  the rerquestingdelegate type which takes the httpcontext as input and return the task
UseStaticFiles() moddleware is used to serve the static files from the file system which are inside the wwwroot folder
UseDefaultFiles() middleware is used to serve the html files that are present as default in wwwroot folder inside the filesystem and index.html is the default file that it point to for the incoming request and it should be placed before the usestaticfiles() middleware
UseFileServer() = usedefaultfiles+usestaticfile middleware conbination
In Mvc there are two ways to configure the routes one is the conventional way of defining the routes through the Maproute method and other is the attribute routing mechanism .
Conventionalroutes we use the parameters inside the curley braces
"{controller=Home}/{action=Index}/{id?}"another is the attribute way using the tokens
Route([controller]/[action])
A higher level of abstraction is being provided by the mvc action method to the controller instead of directly writing the response using the htttpcontext so we use the actionresult return type.
Actionresult is a formal way to encapsulate the decision of the controller.Inside the view there are two model 
@model is the directive that defines the storngly type model that view is going to take from the controller action method.
@Model is the dynamic property that is used to access the data out of the model properties which is beindied to that view 
Razor Syntax
GetEnumSelectList(typeof(EnumName)) this is used in side the html.dropdownlist helper if you want to build the dropdown from the empty model which will generate the options inside the select tag behind the scene.
Migration Commands
1) Add-migration "InitialCreate"
2) update-database scripts
3) update-database to actually update the
