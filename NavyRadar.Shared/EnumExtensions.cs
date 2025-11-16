using System.ComponentModel;
using System.Reflection;

namespace NavyRadar.Shared;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        if (member == null)
        {
            return value.ToString();
        }

        var attribute = member.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString();
    }
}