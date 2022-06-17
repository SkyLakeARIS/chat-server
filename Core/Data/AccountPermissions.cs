namespace Core.Data;

[Flags]
public enum AccountPermissions
{
    User = 0,
    Chat = 1 << 0,
    CreateRoom = 1 << 1,
    KickUser = 1 << 2
}