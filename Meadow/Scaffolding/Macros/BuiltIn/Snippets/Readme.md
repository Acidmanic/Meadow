

Snippets
=========

A **Snippet** is an object that can produce a piece of  
code and has a unique handle to be recognized by.

For builtin macros, meadow deals with a situation that it 
needs to collect and use snippets which are implemented 
differently for each database system.

Snippets in this context are classes that 

  * Implement ```ICodeGenerator``` interface
  * Have a constructor with the signature of ```ctor(SnippetConstruction,SnippetConfiguration)```
  * Are Annotated with ```[CommonSnippet]``` attribute 

Implementation quality
---------------------

A good practice would be implementing snippets in a way that 
they either support and implement a feature/option or warn 
the developer about not supporting it


IdAwareness
-----------

Some snippets can generate different codes if they are being 
employed in 'by-id' mode, rather than 'all' mode. Like read operations,
or delete operations.

these would be the IdAware snippets. 

If you are implementing an IdAware snippet, you would need to a 
way to inform your implementation that which mode is it being used at. 
So the practice of choice here is to implement ```IIdAware``` interface. 
this will force you to have a boolean property which would be valid 
just immediately after construction. and you can use this property 
 to generate the code needed.

If your implementation generates is an id agnostic code, like 
an splitter line, you dont need to implement any interfaces about it.

