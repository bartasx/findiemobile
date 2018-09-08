using FindieMobile.SQLite.Tables;

namespace FindieMobile.Services.Interfaces
{
    public interface ISQLiteService
    {
        UserLocalInfo GetLocalUserInfo();
    }
}