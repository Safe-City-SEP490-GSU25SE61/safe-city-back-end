using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public static class IncidentTypeHelper
    {
        public static bool TryGetEnumFromDisplayName(string displayName, out IncidentType result)
        {
            foreach (var field in typeof(IncidentType).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<DisplayAttribute>();
                if (attr?.Name == displayName)
                {
                    result = (IncidentType)field.GetValue(null)!;
                    return true;
                }
            }

            result = default;
            return false;
        }
        public static IEnumerable<(string Value, string DisplayName)> GetAllDisplayValues()
        {
            return Enum.GetValues(typeof(IncidentType))
                .Cast<IncidentType>()
                .Select(e =>
                {
                    var field = typeof(IncidentType).GetField(e.ToString());
                    var attr = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                        .FirstOrDefault() as DisplayAttribute;
                    return (
                        Value: e.ToString(),
                        DisplayName: attr?.Name ?? e.ToString()
                    );
                });
        }

    }

}
