namespace mPower.Framework
{
    public interface IUserIdentity
    {
        string Id { get; set; }
        string Email { get; set; }
        string Name { get; set; }
        string Password { get; set; }
    }
}