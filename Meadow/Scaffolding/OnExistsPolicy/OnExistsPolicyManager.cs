using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Meadow.Scaffolding.OnExistsPolicy
{
    public class OnExistsPolicyManager
    {
        private readonly List<OnExistsRule> _rules;

        public OnExistsPolicyManager()
        {
            _rules = new List<OnExistsRule>
            {
                o => OnExistsPolicies.Alter,
                o => o.Type == DbObjectTypes.Tables ? OnExistsPolicies.Alter : OnExistsPolicies.Skip
            };
        }

        public OnExistsPolicies OnExists(string name, DbObjectTypes type)
        {
            var obj = new DatabaseObject
            {
                Name = name,
                Type = type
            };
            return OnExists(obj);
        }


        public OnExistsPolicies OnExists(DatabaseObject obj)
        {
            for (int i = _rules.Count - 1; i >= 0; i--)
            {
                var policy = _rules[i](obj);

                if (policy != OnExistsPolicies.NoPolicies)
                {
                    return policy;
                }
            }

            return OnExistsPolicies.NoPolicies;
        }

        public OnExistsPolicyManager AddByName(string name, OnExistsPolicies policy)
        {
            return Add(
                o => o.Name == name ? policy : OnExistsPolicies.NoPolicies
            );
        }

        public OnExistsPolicyManager AddByType(DbObjectTypes type, OnExistsPolicies policy)
        {
            return Add(
                o => o.Type == type ? policy : OnExistsPolicies.NoPolicies
            );
        }

        public OnExistsPolicyManager AddByRegex(string regex, OnExistsPolicies policy)
        {
            return Add(
                o => Regex.IsMatch(o.Name, regex) ? policy : OnExistsPolicies.NoPolicies
            );
        }

        public OnExistsPolicyManager Add(OnExistsRule rule)
        {
            _rules.Add(rule);

            return this;
        }

    }
}