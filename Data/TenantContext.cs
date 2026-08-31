namespace MiniVehicle.Data;

public interface ITenantContext { Guid OrgId { get; set; } }

public sealed class TenantContext : ITenantContext
{
    public static readonly Guid DefaultOrgId = new("44444444-4444-4444-4444-444444444444");
    public const string CookieName = "org_key";
    public Guid OrgId { get; set; } = DefaultOrgId;
}
