using Microsoft.AspNetCore.Authorization;

namespace projectBackend.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AdminAuthorizeAttribute : AuthorizeAttribute
{
    public bool LogEnabled { get; set; } = true;

    public AdminAuthorizeAttribute()
    {
    }

    public AdminAuthorizeAttribute(bool logEnabled)
    {
        LogEnabled = logEnabled;
    }
}
