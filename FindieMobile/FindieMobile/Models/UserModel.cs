namespace FindieMobile.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string AccountDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsInFriendsList { get; set; }
        public string Localization { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IsBanned { get; set; }
        public string GroupName { get; set; }
        public bool IsUserOnline { get; set; }
    }
}