namespace Cataloguer.Database.Commands.Base;

[AttributeUsage(AttributeTargets.Method)]
public class MethodNameAttribute : Attribute
{
    public MethodNameAttribute()
    {
        MethodName = string.Empty;
    }

    public MethodNameAttribute(string name)
    {
        MethodName = name;
    }

    public string MethodName { get; set; }
}