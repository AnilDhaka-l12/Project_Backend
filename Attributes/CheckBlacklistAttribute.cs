namespace ProjectBackend.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CheckJwtBlacklistAttribute : Attribute
{
}
