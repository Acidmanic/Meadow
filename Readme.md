

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
  * Like Migrations in EntityFramework, each change in the database would be maintained as the 
    project source code.
  * There will be no object-tracing and each data-access operation closes the connection which helps
   reducing the chance of concurrency and improves performance in load peaks.
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
================

 1. Prepare your connection string, regarding your database-framework of choice. (Ms-Sql-server, MySql, SQLite)
 2. Setup "_Build-up_ scripts"
 3. Add Meadow library to your project. (You can install the NuGet package)
 4. To use each of your procedures in your c# code, create a MeadowRequest. and it's required models.
 5. Use meadow!


Build-up Scripts
---------------
Create a directory in your source codes for your build-up scripts. Directory name does not matter. Make sure this directory will be copied alongside your binaries when you build/publish your project.

ex:
__your-project.csproj__

    ```xml

  <ItemGroup>
    <Content Include="Scripts\**">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

    ```

For each set of changes during the process of evolving your database, Create a script inside the build-up scripts directory. It's best 
to create script files, regarding you data structure changes, and in a way that i's easy to read and find where to look for scripts related to a specific part of the data base. It this sense, it would be similar to migrations in ORMs.
Also try to avoid manipulating last scripts, and always create new scripts for manipulation of already existing db-objects.


* ⚠️ NOTE: _MANIPULATING PREVIOUSLY ADDED SCRIPTS, WOULD REQUIRE THE DATABASE TO BE RE-CREATED WHICH IS A BIG COST FOR PRODUCTION, VERSA KEEPING THE CHANGES IN A ALWAYS FORWARDING MANNER MAKES IT SMOOTHER AND SAFER FOR PRODUCT UPDATE WITHOUT DOWN-TIME._

Each build-up script file should __has .sql extension__ and __Start with 4 digit number__. The numbers would represent the files order. 
    smaller numbers will be applied before larger numbers. (ie.: ___0000_initial-database.sql___ or ___0001-AddUserInformationTables.sql___)


Creating MeadowRequests
-----------------------

For creating a request, you can drive from ```MeadowRequest``` class. Its a better practice to keep your request classes under a grouped and managed directory/namespace. You can use Conventions to write minimum code. __By Convention, a MeadowRequest named__ ```AbcRequest```, __would be resolved for to execute a procedure named:__ ```spAbc```. If you prefer not to rely on conventions, you can provide procedure name manually by overriding the the property ```RequestText``` in your meadow request.


Using Meadow In Your Project
----------------------------

Create and configure ```MeadowEngine``` by passing a ```MeadowConfiguration``` object to its constructor. A MeadowConfiguration object has a _ConnectionString_ property and a _BuildupScriptDirectory_ property which is where you save build-up scripts.
Then all you need to do is to instantiate an object of your meadow request, and call  ```MeadowEngine.PerformRequest(request)```. Meadow creates and disposes resources per request, therefore it's not 
necessary to keep the engine object. You can keep the engine object or instantiate it per request.

You can also instantiate the ```MeadowEngine``` also by passing an object of Type ```IMeadowConfigurationProvider```. This would be helpful when you want to use different configurations controlled by your DI.


Examples
--------

Meadow can be used to connect to Ms-SqlServer, My-Sql and SQLite. For each of these databases, these is an examples in the project. You can see that using Meadow is the same in each example, but the only difference is in the actual sql scripts written for each database.


ℹ️ NOTE: SQLITE, DOES NOT SUPPORT STORED PROCEDURES, AND MEADOW IS BASED ON STORED PROCEDURES. SO MEADOW DOES STORE YOUR PROCEDURES SEAMLESSLY.


When The Darkness Caresses The Meadows.... (Bugs and Issues)[^1]
=======

Meadow For Ms-SqlServer is currently being tested in my other projects, but for MySql and SQLite,
It still has not being tested in practice. So Please consider dropping a mail or comment if you 
faced a bug or an issue. Very Thanks.




Contact
-------

Please feel free to contact me about meadow, or any related questions. 

acidmanic.moayedi@gmail.com

https://www.linkedin.com/in/mani-moayedi

https://www.instagram.com/acidmanix/




Thanks And Regards.
Mani.












[^1] [IGNEA, Jahi.](https://www.youtube.com/watch?v=ZvLWn29l9tY)