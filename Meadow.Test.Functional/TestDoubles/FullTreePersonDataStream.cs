using Meadow.Reflection.ObjectTree.DataSource;

namespace Meadow.Test.Functional.TestDoubles
{
    public class FullTreePersonDataStream : InMemoryDataStream
    {
        public FullTreePersonDataStream()
        {
            Add("Persons.Id", "Mani")
                .Add("Surname", "Moayedi")
                .Add("Age", 37)
                .Add("JobId", 3)
                .Add("Jobs.Id", 3)
                .Add("Title", "Project Manager")
                .Add("IncomeInRials", 100000)
                .Add("JobDescription", "Plan Plan PLan")
                .Add("City", "Tehran")
                .Add("Street", "FirstSt")
                .Add("AddressName", "Home")
                .Add("Block", 1)
                .Add("Plate", 12)
                .Add("Addresses.Id", 1)
                .Add("PersonId", 1)
                
                .Add("Persons.Id", "Mani")
                .Add("Surname", "Moayedi")
                .Add("Age", 37)
                .Add("JobId", 3)
                .Add("Jobs.Id", 3)
                .Add("Title", "Project Manager")
                .Add("IncomeInRials", 100000)
                .Add("JobDescription", "Plan Plan PLan")
                .Add("City", "Tehran")
                .Add("Street", "SecondSt")
                .Add("AddressName", "Work")
                .Add("Block", 1)
                .Add("Plate", 14)
                .Add("Addresses.Id", 2)
                .Add("PersonId", 1)

                .Add("Persons.Id", "Mona")
                .Add("Surname", "Moayedi")
                .Add("Age", 38)
                .Add("JobId", 3)
                .Add("Jobs.Id", 3)
                .Add("Title", "Project Manager")
                .Add("IncomeInRials", 100000)
                .Add("JobDescription", "Plan Plan PLan");
        }
    }
}