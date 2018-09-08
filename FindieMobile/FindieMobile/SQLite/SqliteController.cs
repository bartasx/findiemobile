using System;
using FindieMobile.SQLite.Tables;
using SQLite;
using Xamarin.Forms;

namespace FindieMobile.SQLite
{
    public class SQLiteController : ContentPage, IDisposable
    {
        public readonly SQLiteConnection _sqLiteConnection;

        #region Dispose Method
        public void Dispose()
        {
        }

        #endregion

        #region Constructor

        public SQLiteController()
        {
            this._sqLiteConnection = DependencyService.Get<ISQLite>().GetConnection();
            this._sqLiteConnection.CreateTable<UserLocalInfo>();
        }

        #endregion

        #region public methods
        /// <summary>
        /// Method which is save to application data login.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void AddLoginData(string login, string password)
        {
            this._sqLiteConnection.Insert(new UserLocalInfo()
            {
                Login = login,
                Password = password,
                IsBanned = "false"
            });
        }

        public void DeleteUserFromLocalDb()
        {
           this. _sqLiteConnection.DeleteAll<UserLocalInfo>();
        }
        #endregion
    }
}