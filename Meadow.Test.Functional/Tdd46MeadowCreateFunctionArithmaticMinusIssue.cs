using System;
using System.Linq;
using Meadow.MySql.Comments;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd46MeadowCreateFunctionArithmaticMinusIssue : PersonUseCaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseMySql();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            base.Main(engine, logger);

            string[] testStrings =
            {
                "(contentLength - replacementLength)",
                "contentLength / replacementLength"
            };

            foreach (var testString in testStrings)
            {
                var unComment = testString.ClearMySqlComments();

                if (testString != unComment)
                {
                    logger.LogError("Wrong uncommented code: " +
                                    "\n{Code} instead of " +
                                    "\n{Original}", unComment, testString);

                    return;
                }
            }

            logger.LogInformation("Fixed");
        }

        private string FunctionDeclaration = @"
-- ---------------------------------------------------------------------------------------------------------------------
create function fnCount(Content nvarchar(2048),Find nvarchar(100)) returns bigint(16) READS SQL DATA DETERMINISTIC begin
    
    declare lowerContent nvarchar(1024) default ''; 
    declare lowerFind nvarchar(100) default '';
    declare contentLength bigint(16) default 0;
    declare replacedContent nvarchar(1024) default '';
    declare deltaLength int default 0;
    declare replacementLength int default 0;
    
    set  lowerContent =  lower(Content);    
    set lowerFind =  lower(Find);
    set contentLength = length(Content); 
    set replacedContent = replace(lowerContent,lowerFind,'');
    set replacementLength = length(replacedContent);
    
#     set deltaLength = (contentLength - replacementLength);
    set deltaLength = (contentLength-replacementLength);
    
    return  deltaLength div length(Find);
end;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create  function fnOccurenceIndex(Content nvarchar(2048),Find nvarchar(100)) returns bigint(16) DETERMINISTIC begin
    set @at = INSTR(lower(Content),lower(Find));
    set @cl = LENGTH(content);
    set @locationScore = 100000000;
    if ( @at > 0 ) then
        set @locationScore = @at;
    end if;
    return @locationScore;

end;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}