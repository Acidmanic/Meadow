

![Meadow Logo](../Graphics/MeadowIcon.png)



Meadow Assistant Tool
=====================


Meadow Assistant Tool (mat) is a small dotnet tool can be installed using  
```dotnet tool install --global Meadow.Tools.Assistant```. This tool can provide 
some handy tasks for you while you are using meadow-framework libraries. 
Currently it provides __apply-macros__ (__am__) command, for auto generating macros. 
including builtin macros: 
```sql
   -- {{Crud <Type.Fullname>}}
   -- {{EventStream <Type.Fullname>}}
   -- {{Table <Type.Fullname>}}
   -- {{Insert <Type.Fullname>}}
   -- {{ReadAll <Type.Fullname>}}
   -- {{ReadById <Type.Fullname>}}
   -- {{DeleteById <Type.Fullname>}}
   -- {{DeleteAll <Type.Fullname>}}
   -- {{Update <Type.Fullname>}}
   -- {{Save <Type.Fullname>}}
   -- {{Coffee}} (Sure why not?!)
```

and any macro you created for your project by implementing ```IMacro``` interface.

you can run ```mat --help``` in your command line to see the arguments and find out 
how to use it the way you need. you can also use --help flag on each argument to see their 
descriptions.