using SQLite;

namespace FindieMobile.SQLite.Tables
{
    public class UserLocalInfo
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
        public string IsBanned { get; set; }

        public override string ToString()
        {
            return string.Format("Login : {0}, Password : {1}, IsBanned : {2}", this.Login, this.Password, this.IsBanned);
        }
    }
}