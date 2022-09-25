

Meadow Postgre Adapter
============


By installing this library in your client code, you would be able to use
 meadow connected to a postgre database, just by calling __UsePostgre()__ 
method.

```c#
    var engine = new MeadowEngine(configuration).UsePostgre();
```

Postgre Considerations
--------------------

 * Meadow is based on Stored procedures, but in case of requests which 
return data, it can be tricky to use a procedure while defining a 
function instead does the job easily.
 * Meadow uses the exact names of classes and properties and attributes
   (along side some conventions) to communicate with database. 
   In the other hand, if a name is not quoted with double quotes, it will 
   be considered in lower case by postgre, so ___DO NOT FORGET TO QUOTE NAMES___.
 * In Meadow requests you create, also the name must be quoted, so 
 ___DO NOT FORGET TO OVERRIDE ```QuoteProcedureName``` METHOD IN REQUESTS___. 
 This method by default returns ```false```. For Postgre, you need to 
 * override it to return true.  



