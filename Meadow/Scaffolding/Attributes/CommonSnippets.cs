using System.Collections.Generic;
using System.Linq;

namespace Meadow.Scaffolding.Attributes;

public enum CommonSnippets
{
    CreateTable,
    DeleteProcedure,
    InsertProcedure,
    ReadProcedure,
    UpdateProcedure,
    SaveProcedure,
    EventStreamScript,
    FilteringProcedures,
    FindPaged,
    FullTreeView,
    DataBound,
}

public class CommonSnippetsInfo
{

    private readonly Dictionary<CommonSnippets, bool> _idAwares;

    public CommonSnippetsInfo()
    {
        _idAwares = new Dictionary<CommonSnippets, bool>();
        
        _idAwares.Add(CommonSnippets.CreateTable,false);
        _idAwares.Add(CommonSnippets.DeleteProcedure,true);
        _idAwares.Add(CommonSnippets.InsertProcedure,false);
        _idAwares.Add(CommonSnippets.ReadProcedure,true);
        _idAwares.Add(CommonSnippets.UpdateProcedure,false);
        _idAwares.Add(CommonSnippets.SaveProcedure,false);
        _idAwares.Add(CommonSnippets.EventStreamScript,false);
        _idAwares.Add(CommonSnippets.FilteringProcedures,false);
        _idAwares.Add(CommonSnippets.FullTreeView,false);
        _idAwares.Add(CommonSnippets.DataBound,false);
    }

    public bool IsIdAware(CommonSnippets snippet)
    {
        if (_idAwares.ContainsKey(snippet))
        {
            return _idAwares[snippet];
        }

        return false;
    }
}