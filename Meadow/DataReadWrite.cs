using System.Collections.Generic;
using System.Data;
using Meadow.Reflection.ObjectTree;
using Meadow.Reflection.ObjectTree.Mapping;

namespace Meadow
{
    public class DataReadWrite
    {



        public List<TModel> ReadData<TModel>(IDataReader dataReader,bool fullTree)
        {
            var node = new TypeAnalyzer().ToAccessNode<List<TModel>>(fullTree);

            var mapper = new ObjectDataMapper(node);
            
            mapper.Write(dataReader);

            return (List<TModel>) mapper.RootObject;
        }
    }
}