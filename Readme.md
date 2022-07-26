

![Meadow Logo](Graphics/MeadowIcon.png)



**Meadow**
=========
 __M__<nothing>anaged/<nothing>__E__<nothing>nhanced<nothing>__Ado__<nothing>__W__<nothing>rapper
 
 This library is designed to help using ado.net easier and with some good practices consistently.
 
 Features
 --------
 
  * It Supports __MS-SqlServer__, __My Sql__ and __SQLite__ database frameworks. 
  * The application would create/buildup and update database by itself after deployments.
  * Your data access logic will be fully implemented in SQL language, therefore:
    * You would have access to what ever feature your database framework provides with no limitations.
    * Non of your data-access business would be transported to server as pure sql text 
  * Like Migrations in EntityFramework each change in the database would be maintained as the 
    project source code.
  * There will be no object-tracing and each data-access operation closes the connection which helps
   reducing the chance of concurrency and improves performance in lead peaks.
  * The conversion of data to/from POCOs are internally covered so working with data is easy.
  
  
 
 The Practice
 ------------
 
 Meadow is designed to help implementing a practice as follows:
  
  * Design and Maintain the database, step by step, by creating build-up scripts (They would be treated like source code)
  * For each data access operation, Create a Stored Procedure in a build-up script.
  * For each data access operation, drive a well named request class, from ```MeadowRequest``` class.
  * Perform the request and receive the results (if any) whenever needed, using ```MeadowEngine``` and an instance of 
   corresponding request.
   
   
  
The ```MeadowEngine``` can take some configurations. It also performs some useful operations 
for you like creating/deleting and building up the database. 

 * NOTE: The best practice would be to use __Meadow__ behind your ___Repository Pattern___ implementation.   
 
 In summery
  1. You would use meadow for retrieving/manipulating data through the MeadowRequests
  2. You would use meadow for creation/deletion or building up your database (usually at the startup section of the application) via ```MeadowEngine```

How to Use
----------

 1. Prepare your connection string, regarding your database-framework of choice. (Ms-Sql-server, MySql, SQLite)
 2. Create a directory in your source codes for your build-up scripts.
    * Directory name does not matter.
    * Make sure this directory will be copied alongside your binaries when you build/publish your project 
 3. For each set of changes during the process of evolving your database, Create a script inside the build-up scripts directory.
    * Its best that you create a new file whenever you applied the last one. You also can delete and rebuild your database during the development.
    * Each build-up script file should __has .sql extension__ and __Start with 4 digit number__. The numbers would represent the files order. 
    smaller numbers will be applied before larger numbers. (ie.: ___0000_initial-database.sql___ or ___0001-AddUserInformationTables.sql___)
  4. Add Meadow library to your project. (You can install the NuGet package)
  5. To call each of your procedures, create a MeadowRequest and it's required models.
    * For creating a request, you can drive from ```MeadowRequest``` class.
    * Its a better practice to keep your request classes under a grouped and managed directory/namespace.
    * You can use Conventions to write minimum code. __By Convention, a MeadowRequest named__ ```AbcRequest```,
    __would be resolved for to execute a procedure named:__ ```spAbc```. If you prefer not to rely on conventions, you can provide procedure name
    manually by overriding the the property ```RequestText``` in your meadow request.    
  6. Use meadow!
    * Create and configure ```MeadowEngine```. Configurations include the connection string and the build-up script directory.
    * Wherever you need to perform a database task. you will instantiate an object of the request class you created, and call 
    ```MeadowEngine.PerformRequest(request)``` 