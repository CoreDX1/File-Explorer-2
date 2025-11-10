namespace Domain.Monads;

internal static class TypeExtensions
{
    public static string GetFriendlyTypeName(this Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var genericTypeName = type.GetGenericTypeDefinition().Name;
        var index = genericTypeName.IndexOf('`');
        if (index > 0)
            genericTypeName = genericTypeName.Substring(0, index);

        var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
        return $"{genericTypeName}<{genericArgs}>";
    }
}
