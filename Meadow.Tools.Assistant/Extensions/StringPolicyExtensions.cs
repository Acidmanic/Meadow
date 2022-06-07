using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Meadow.Scaffolding;
using Meadow.Scaffolding.OnExistsPolicy;

namespace Meadow.Tools.Assistant
{
    public static class StringPolicyExtensions
    {
        private static readonly Dictionary<string, OnExistsPolicies> PolicyMap =
            new Dictionary<string, OnExistsPolicies>
            {
                {"skip", OnExistsPolicies.Skip},
                {"alter", OnExistsPolicies.Alter},
                {"drop", OnExistsPolicies.DropAndReCreate}
            };

        private static readonly Dictionary<string, DbObjectTypes> DbTypeMap = new Dictionary<string, DbObjectTypes>
        {
            {"t", DbObjectTypes.Tables},
            {"p", DbObjectTypes.StoredProcedures}
        };

        public static Result<OnExistsRule> AsPolicy(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Result.Failure<OnExistsRule>();
            }

            if (PolicyMap.ContainsKey(value))
            {
                return Result.Successful<OnExistsRule>(o => PolicyMap[value]);
            }

            var dash = value.LastIndexOf("-", StringComparison.Ordinal);

            if (dash > -1)
            {
                string head;
                string tail;

                head = value.Substring(0, dash);

                tail = value.Substring(dash + 1, value.Length - dash - 1);

                if (PolicyMap.ContainsKey(head)) // Its By Name
                {
                    var policy = PolicyMap[head];

                    return Result.Successful<OnExistsRule>(o => o.Name == tail ? policy : OnExistsPolicies.NoPolicies);
                }
                else if (PolicyMap.ContainsKey(tail)) // Its By Type
                {
                    if (DbTypeMap.ContainsKey(head))
                    {
                        var policy = PolicyMap[tail];
                        var dbType = DbTypeMap[head];

                        return Result.Successful<OnExistsRule>(o =>
                            o.Type == dbType ? policy : OnExistsPolicies.NoPolicies);
                    }
                }
            }

            return Result.Failure<OnExistsRule>();
        }
    }

    public static class StringArrayExtensions
    {
        public static OnExistsPolicyManager AsPolicyManager(this string[] policyStrings)
        {
            var manager = new OnExistsPolicyManager();

            if (policyStrings != null)
            {
                foreach (var policyString in policyStrings)
                {
                    var result = policyString.AsPolicy();

                    if (result.Success)
                    {
                        manager.Add(result.Value);
                    }
                }
            }

            return manager;
        }
    }
}