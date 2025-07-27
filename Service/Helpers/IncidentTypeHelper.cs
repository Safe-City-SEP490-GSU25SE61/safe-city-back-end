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



    public static IEnumerable<(string Value, string DisplayName)> GetDisplayValues<TEnum>() where TEnum : Enum
            {
                return Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .Select(e =>
                    {
                        var field = typeof(TEnum).GetField(e.ToString());
                        var attr = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                            .FirstOrDefault() as DisplayAttribute;
                        return (
                            Value: e.ToString(),
                            DisplayName: attr?.Name ?? e.ToString()
                        );
                    });
         }

        public static IEnumerable<(string Value, string DisplayName)> GetAllDisplayValues()
                => GetDisplayValues<IncidentType>();

        public static IEnumerable<(string Value, string DisplayName)> GetSubCategories(IncidentType type)
            {
                return type switch
                {
                    IncidentType.Traffic => GetDisplayValues<TrafficSubCategory>(),
                    IncidentType.Security => GetDisplayValues<SecuritySubCategory>(),
                    IncidentType.Infrastructure => GetDisplayValues<InfrastructureSubCategory>(),
                    IncidentType.Environment => GetDisplayValues<EnvironmentSubCategory>(),
                    IncidentType.Other => GetDisplayValues<OtherSubCategory>(),
                    _ => Enumerable.Empty<(string, string)>()
                };
            }

        public static IEnumerable<(string Value, string DisplayName)> GetPriorityLevels()
                => GetDisplayValues<PriorityLevel>();




    }

}
