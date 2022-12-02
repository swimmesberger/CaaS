using System.Reflection;

namespace CaaS.Api.Base.Swagger; 

public static class AttributeExtensions {
    public static bool HasAttribute<T>(this MemberInfo methodInfo) where T : Attribute => methodInfo.GetAttribute<T>() != null;
    
    public static T? GetAttribute<T>(this MemberInfo methodInfo) where T: Attribute {
        if (methodInfo.GetCustomAttribute(typeof(T)) is T mAttribute) 
            return mAttribute;
        if (methodInfo.DeclaringType?.GetCustomAttribute(typeof(T)) is T cAttribute)
            return cAttribute;

        return null;
    }
}