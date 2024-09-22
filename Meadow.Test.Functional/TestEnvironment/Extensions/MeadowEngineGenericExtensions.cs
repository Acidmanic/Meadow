using System.Collections;
using Acidmanic.Utilities.Reflection;
using Meadow.Requests;

namespace Meadow.Test.Functional.TestEnvironment.Extensions;

public static class MeadowEngineGenericExtensions
{
    public record PerformResponse(bool Success, List<object> FromStorage, Exception? FailureException);

    public static PerformResponse Perform(this MeadowEngine engine, object request, bool fullTree = false)
    {
        if (request is MeadowRequest meadowRequest)
        {
            var ancestors = Ancestors(meadowRequest);

            var genericMeadowRequest = typeof(MeadowRequest<,>);

            var genericParameterRequestType = ancestors.FirstOrDefault
                (t => t.IsGenericType && t.GetGenericTypeDefinition() == genericMeadowRequest);

            if (genericParameterRequestType is { } gpr)
            {
                var genericArguments = gpr.GetGenericArguments();

                var method = typeof(MeadowEngine).GetMethod(nameof(MeadowEngine.PerformRequest));

                if (method is { } m)
                {
                    var typedMethod = m.MakeGenericMethod(genericArguments);

                    try
                    {
                        var result = typedMethod.Invoke(engine, new object[] { meadowRequest, fullTree });

                        if (result is MeadowRequest response)
                        {
                            if (response.Failed) return new PerformResponse(response.Failed, new List<object>(), response.FailureException);

                            if (!response.ReturnsValue) return new PerformResponse(true, new List<object>(), null);

                            var fromStorage = ReadFromStorageProperty(response, meadowRequest.GetType());

                            return new PerformResponse(true, fromStorage, null);
                        }
                    }
                    catch (Exception e)
                    {
                        return new PerformResponse(false, new List<object>(), e);
                    }
                }
            }
        }

        return new PerformResponse(false, new List<object>(), new ArgumentException("Bad Request"));
    }


    private static List<object> ReadFromStorageProperty(object response, Type tRequest)
    {
        try
        {
            var property = tRequest.GetProperty(nameof(MeadowRequest<object, object>.FromStorage));

            if (property is { } p)
            {
                var readFromStorage = p.GetValue(response);
                
                if (readFromStorage is IEnumerable fromStorageEnumerable)
                {
                    var fromStorageObjects = new List<object>();

                    foreach (var item in fromStorageEnumerable)
                    {
                        if(item is {} i) fromStorageObjects.Add(i);
                    }

                    return fromStorageObjects;
                }
            }
        }
        catch (Exception e) { }

        return new List<object>();
    }

    private static List<Type> Ancestors(object o)
    {
        var ancestors = new List<Type>();

        var type = o.GetType();

        while (type != null)
        {
            ancestors.Add(type);

            type = type.BaseType;
        }

        ancestors.Reverse();

        return ancestors;
    }
}