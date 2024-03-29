

1.0.1
-------

   * reflect Execution success/failure-exception in returning request
   
   
1.0.3
-------

   * Add Save Operation to known operations
   * Fixed missing excluded fields in MeadowRequest
     
1.0.5
------
   * Reflect Configuration failures regarding meadow requests being able to project execution result
   * Update reflection package
   * Fix retrieving direct leaves in sql data adapter base
   
   
1.0.6
-----
  * Fix un expected directory for scripts issue  
  
 1.1.1
 -------
  * Update Standard <-> Storage field name resolution
  * Make Meadow able to get Runner assembly so it would not get confused when running in unit tests
  * Update Reflection to 1.0.0
 
 1.1.4
 -------
  * Update FieldManipulation and marking
  
1.1.6
-----
 * Use Microsoft logging abstraction for logging
 * Set Logging similar to MeadowCore at configuration time
 
 1.1.8
 -----
 * AdoDataAccessCoreBase has been added to make implementation of sql databases easier
 * Logging issues in MeadowEngine Has been fixed
 * Logging issues in MeadowDataAccessCoreBase has been fixed
 * Detailed logs in StandardIndexAccumulator are being logged as trace logs.
   

1.2.2
-----
 * implement async functionality
 * fix 'go' in model name issue

1.2.3
-----
 * Splitting issues are fixed
 * Connection pool clearing for Ado based data bases is managed
 * Connection pool is cleared in SqlServer and open-connection issue got solved


1.2.4
-----
 * Supports alternative types
 * Un ordered full-tree requests would be processed and converted to objects correctly
 * Fixed null IdLeaf issue for types without id fields 
 * Added mat for scaffolding and other helpful tools
 * Added Macros
 * Added Builtin macros for crud operations and event-streams
 * Implement crud and event stream macros for sql-server, my-sql, sqlite and postgre
 * Fixed postgre build-up issues 
 * Use compression for meadow history
 * MAT is refactored, Added Apply macro command
 * Added Extract history scripts command to MAT
 * Added builtin JsonConfigurationProvider
 