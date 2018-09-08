using FindieMobile.SQLite;
using FindieMobile.SQLite.Tables;
using FindieMobile.Services.Interfaces;

namespace FindieMobile.Services
{
    public class SQLiteService : ISQLiteService
    {
        public UserLocalInfo GetLocalUserInfo()
        {
            using (var controller = new SQLiteController())
            {
                var table = controller._sqLiteConnection.Table<UserLocalInfo>();

                UserLocalInfo user = (from account in table select account).FirstOrDefault();
                return user;
            }
        }
    }
}