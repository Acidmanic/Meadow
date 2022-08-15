

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
 2. [Setup "_Build-up_ scripts"](https://github.com/Acidmanic/Meadow#build-up-scripts)
 3. [Add Meadow library to your project](https://github.com/Acidmanic/Meadow#add-meadow-library). (You can install the NuGet package)
 4. [Create a MeadowRequest and it's required models](https://github.com/Acidmanic/Meadow#creating-meadowrequests) To use each of your procedures in your c# code.
 5. [Use meadow!](https://github.com/Acidmanic/Meadow#using-meadow-in-your-project)


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


[^ [Back to How To Use]](https://github.com/Acidmanic/Meadow#how-to-use)


Add Meadow library
------------------
Meadow libraries are available on Nuget.  To use meadow, you would add

1. Add [___Meadow.Framework___](https://www.nuget.org/packages/Meadow.Framework).

    |  |  |
    |:--------:|:---------------:|
    |PackageReference |```<PackageReference Include="Meadow.Framework" Version="1.0.1" />```|
    |PackageManager|```Install-Package Meadow.Framework -Version 1.0.1```|
    |Dotnet CLI|```dotnet add package Meadow.Framework --version 1.0.1```|

2. Add ___Meadow.&lt;Db-Framework&gt;___. For each database-framework supported by Meadow, you would add it's own db-framework adapter:
   
   1. [SqlServer](https://www.nuget.org/packages/Meadow.SqlServer)

        |  |  |
        |:--------:|:---------------:|
        |PackageReference |```<PackageReference Include="Meadow.SqlServer" Version="1.0.0" />```|
        |PackageManager|```dotnet add package Meadow.SqlServer --version 1.0.0```|
        |Dotnet CLI|```dotnet add package Meadow.SqlServer --version 1.0.0```|
    
   2. [SQLite](https://www.nuget.org/packages/Meadow.SQLite)
    
        |  |  |
        |:--------:|:---------------:|
        |PackageReference |```<PackageReference Include="Meadow.SQLite" Version="1.0.0" />```|
        |PackageManager|```Install-Package Meadow.SQLite -Version 1.0.0```|
        |Dotnet CLI|```dotnet add package Meadow.SQLite --version 1.0.0```|
    
   3. [MySql](https://www.nuget.org/packages/Meadow.MySql)
    
        |  |  |
        |:--------:|:---------------:|
        |PackageReference |```<PackageReference Include="Meadow.MySql" Version="1.0.0" />```|
        |PackageManager|```Install-Package Meadow.MySql -Version 1.0.0```|
        |Dotnet CLI|```dotnet add package Meadow.MySql --version 1.0.0```|




* ⚠️ NOTE: _VERSIONS APPEARING IN THIS SECTION ARE NOT NECESSARILY LATEST. PLEASE CHECK OUT THE NUGET PAGE FOR EACH TO SEE THE LATEST VERSION_



[^ [Back to How To Use]](https://github.com/Acidmanic/Meadow#how-to-use)

Creating MeadowRequests
-----------------------

For creating a request, you can drive from ```MeadowRequest``` class. Its a better practice to keep your request classes under a grouped and managed
 directory/namespace. You can use Conventions to write minimum code. 
 
 __By Convention, a MeadowRequest named__ ```AbcRequest```, __would be resolved for to execute a procedure named:__ ```spAbc```. 
 
 If you prefer not to rely on conventions, you can provide procedure name manually by overriding the the property ```RequestText``` in your meadow request.
 
 
 Meadow also contains several pre-built requests for common operations that you can drive from:
    
 1. ```InsertSpRequest<TModel>```
           Inserts an Entity Of type TModel, Into the database. It expects the corresponding
            procedure (which you would write in build-up scripts) to 
             (a) have one input parameter corresponding to each effectively-primitive [^1] property of TModel. (Except for the Id Parameter)  
             (b) return newly inserted model.
 2. ```UpdateSpRequest<TModel>```  
        Updates an Entity Of Type TEntity then returns the updated value. Therefore it also  
        expects your stored-procedure, to 
         (a) have one input parameter corresponding to each effectively-primitive property of TModel. (Except for the Id Parameter)  
         (b) return newly inserted model.
 3. ```ReadAllSpRequest<TModel>```  
        Reads all records of type TModel. If you pass 'True' for the constructor argument: __fullTree__,
         then this request expects your procedure to return all fields from all other tables (joined), so
         it can glue all together and construct an instance of TModel and all it's properties.
          If you pass 'False' for full-tree argument, then a simple procedure like ```select * from <table-name> ...``` 
          would be all it needs.
4. ```ReadByIdSpRequest<TModel, TId>```  
        Reads the Entity identified by the given Id. This Request and it's descendants, would 
        have an Id property of type: ```TId```. You can set this property before performing 
        the request.
        This request, expects its Corresponding procedure to have one argument named after
        the Identity Field of your Model.
        the full-tree argument in this request is the same as __ReadAllSpRequest__. 
 5. ```DeleteByIdSpRequest<TModel,TId>```  
        This request is for deleting a record by its Id. Like ReadById, It uses the Id property. It 
        returns the success result of the deletion as ```DeletionResult```. So it expects your 
        procedure to return a true/false value with the field-name of __Success__.
 6. ```ByIdRequestBase```  
        This request is the base class for ```ReadByIdSpRequest``` and ```DeleteByIdSpRequest```. 
        It can be used for creating any other ById operations of interest.
 7. ```SaveSpRequest<TModel>```
        This Request to have a data inserted when it's not present in the database, and updated 
        if it's already there. The expected input parameters are the same as Insert or Update. Also
        no matter the insertion happened or an update took place, the procedure should return the 
        inserted/updated model. 
 More On Requests
 ----------------
 
 __Output/Input__
 
 
When you drive ```MeadowRequest<TIn,TOut>```, TIn would be the type of the data you send towards the database when you perform that request.
 in example when you are inserting a data. The same way, TOut would be the type of data returned from the database. In a case that you do not 
 send any data towards the database, you should use the type ```MeadowVoid``` for TIn. Also for those requests that would not return any data,
 you will use this class.
 
 
 __Constructor__


Another point to consider is ```MeadowRequest```'s Constructor. It takes a boolean argument to determine if this request is obligated to read returned data.  


__Name Conventions__

In a case that you needed to know, what Db Object Names Meadow would prefer to be used for a specific Entity, 
you can instantiate ```NameConvention<TEntity>``` class. This class will provide predefined names for common 
procedures and tables regarding given type (TEntity).



   
[^ [Back to How To Use]](https://github.com/Acidmanic/Meadow#how-to-use)

Using Meadow In Your Project
----------------------------

Create and configure ```MeadowEngine``` by passing a ```MeadowConfiguration``` object to its constructor. A MeadowConfiguration object has a _ConnectionString_ property and a _BuildupScriptDirectory_ property which is where you save build-up scripts.
Then all you need to do is to instantiate an object of your meadow request, and call  ```MeadowEngine.PerformRequest(request)```. Meadow creates and disposes resources per request, therefore it's not 
necessary to keep the engine object. You can keep the engine object or instantiate it per request.

You can also instantiate the ```MeadowEngine``` also by passing an object of Type ```IMeadowConfigurationProvider```. This would be helpful when you want to use different configurations controlled by your DI.


[^ [Back to How To Use]](https://github.com/Acidmanic/Meadow#how-to-use)

Examples
--------

Meadow can be used to connect to Ms-SqlServer, My-Sql and SQLite. For each of these databases, these is an examples in the project. You can see that using Meadow is the same in each example, but the only difference is in the actual sql scripts written for each database.


ℹ️ NOTE: SQLITE, DOES NOT SUPPORT STORED PROCEDURES, AND MEADOW IS BASED ON STORED PROCEDURES. SO MEADOW DOES STORE YOUR PROCEDURES SEAMLESSLY.


When The Darkness Caresses The Meadows...[^2] (Bugs and Issues)
-------------------------------------------

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

.
------------------------
[^1]: For Meadow, __Effectively Primitive__ mean any type that is able to be inserted directly into the database. It involves all primitives, plus the String, DateTime and etc....
[^2]: [IGNEA, Jahi.](https://www.youtube.com/watch?v=ZvLWn29l9tY)