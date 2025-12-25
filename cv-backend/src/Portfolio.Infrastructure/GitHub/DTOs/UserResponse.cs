namespace Portfolio.Infrastructure.GitHub.DTOs
{
    public class UserResponseData
    {
        public User User {get; set;} = default!;
    }
    public class User
    {
        public string Id {get; set;} = default!;
    }
}