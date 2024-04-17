using System.Collections.Generic;
using Meadow.Configuration;

namespace Meadow.Requests.Configuration.Abstractions;

public interface IConfigurationAlteringRequest
{
    MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration, Dictionary<string, string> connectionString);
}