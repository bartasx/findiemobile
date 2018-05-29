using SQLite;

namespace FindieMobile.SQLite
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
