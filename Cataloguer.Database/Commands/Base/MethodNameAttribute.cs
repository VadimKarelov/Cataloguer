namespace Cataloguer.Database.Commands.Base
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodNameAttribute : Attribute
    {
        public string MethodName { get; set; }

        public MethodNameAttribute()
        {
            MethodName = string.Empty;
        }

        public MethodNameAttribute(string name)
        {
            MethodName = name;
        }
    }
}
