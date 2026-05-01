namespace CampingCore.Domain.Entities;

public class RolePermission
{
    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }

    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    protected RolePermission() { }

    public RolePermission(int roleId, int permissionId)
    {
        RoleId       = roleId;
        PermissionId = permissionId;
    }
}
