

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