using Meadow.Reflection.ObjectTree.DataSource;

namespace Meadow.Test.Functional.TestDoubles
{
    public class FullTreePersonDataReader : InMemoryDataReader
    {
        public FullTreePersonDataReader()
        {
            CreateRecord()
                .InsertField("Persons.Id", 1)
                .InsertField("Name", "Mani")
                .InsertField("Surname", "Moayedi")
                .InsertField("Age", 37)
                .InsertField("JobId", 3)
                .InsertField("Jobs.Id", 3)
                .InsertField("Title", "Project Manager")
                .InsertField("IncomeInRials", 100000)
                .InsertField("JobDescription", "Plan Plan PLan")
                .InsertField("Addresses.Id", 1)
                .InsertField("City", "Tehran")
                .InsertField("Street", "FirstSt")
                .InsertField("AddressName", "Home")
                .InsertField("Block", 1)
                .InsertField("Plate", 12);
            
            CreateRecord()
                .InsertField("PersonId", 1)
                .InsertField("Persons.Id", 1)
                .InsertField("Name", "Mani")
                .InsertField("Surname", "Moayedi")
                .InsertField("Age", 37)
                .InsertField("JobId", 3)
                .InsertField("Jobs.Id", 3)
                .InsertField("Title", "Project Manager")
                .InsertField("IncomeInRials", 100000)
                .InsertField("JobDescription", "Plan Plan PLan")
                .InsertField("Addresses.Id", 2)
                .InsertField("City", "Tehran")
                .InsertField("Street", "SecondSt")
                .InsertField("AddressName", "Work")
                .InsertField("Block", 1)
                .InsertField("Plate", 14);

            CreateRecord()
                .InsertField("Persons.Id", 2)
                .InsertField("Name", "Mona")
                .InsertField("Surname", "Moayedi")
                .InsertField("Age", 38)
                .InsertField("JobId", 3)
                .InsertField("Jobs.Id", 3)
                .InsertField("Title", "Project Manager")
                .InsertField("IncomeInRials", 100000)
                .InsertField("JobDescription", "Plan Plan PLan");
        }
    }
}