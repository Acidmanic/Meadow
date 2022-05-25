

xx Still Under Construction xx
==============================


**Meadow**
=========
 __M__ anaged / __E__ nhanced __Ado__ __W__ rapper
 
 This library is designed to help using ado.net easier and with some good practices consistently.
 
 
 
 The Practice
 ------------
 
 Meadow provides easier access to your database using ADO.net. The Idea is to
 
  * Design and Maintain the database almost Manually by build-up scripts (They would be treated like source code)
  * Perform any acquiring or data manipulations using StoredProcedures
  * Wrap these procedures in Meadow requests by driving from ```MeadowRequest``` class  
  * Execute these requests at anytime needed using a ```MeadowEngine``` object.
  
The ```MeadowEngine``` can take some configurations. It also performs some useful operations 
for you like creating/deleting and building up the database. 

 * NOTE: The best practice would be to use __Meadow__ behind your ___Repository Pattern___ implementation.   
 
 So in summery
  1. You would use meadow for retrieving/manipulating data through the MeadowRequests
  2. You would use meadow for creation/deletion or building up your database (usually at first deployment) via 

How to Use
----------

 1. Prepare your connection string
 2. Create a directory in your source codes for your build-up scripts.
    * Directory name does not matter.
    * Make sure this directory will be copied alongside your binaries when you build/publish your project 
 3. For each set of changes during the process of evolving your database, Create a script inside the build-up scripts directory.
    * Its best that you create a new file whenever you applied the last one. You also can delete and rebuild your database during the development.
    * Each build-up script file should __has .sql extension__ and __Start with 4 digit number__. The numbers would present the files order. 
    smaller numbers will be applied before larger numbers. (ie.: ___0000_initial-database.sql___ or ___000-AddUserInformationTables.sql___)
  4. Add Meadow library to your project. (You can install the NuGet package)
  5. To call each of your procedures, create a MeadowRequest and it's required models.
    * For creating a request, you can drive from ```MeadowRequest``` class.
    * Its a better practice to keep your request classes under a grouped and managed directory/namespace.
  6. Use meadow!
    * Create and configure ```MeadowEngine```. Configurations include the connection string and the build-up script directory.
    * Wherever you need to perform a database task. you will instantiate an object of the request class you created, and call 
    ```MeadowEngine.PerformRequest(request)``` 