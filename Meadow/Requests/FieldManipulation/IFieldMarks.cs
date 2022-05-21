namespace Meadow.Requests.FieldManipulation
{
    public interface IFieldMarks
    {

        bool IsIncluded(string fieldName);

        string GetPracticalName(string fieldName);
        
    }
}