using System;
using FindieMobile.SQLite.Tables;
using SQLite;
using Xamarin.Forms;

namespace FindieMobile.SQLite
{
    class SQLiteController : ContentPage, IDisposable
    {
        private readonly SQLiteConnection _sqLiteConnection;

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

        /// <summary>
        /// This Gets User Account Data like Login, password, email, states etc.
        /// </summary>
        /// <param name="name">To get access to data at first it need to get login name</param>
        /// <returns>It returns instance with all user data</returns>
        public UserLocalInfo GetUserData()
        {
            var table = this._sqLiteConnection.Table<UserLocalInfo>();

            UserLocalInfo list;

            return list = (from account in table select account).First();
        }

        public void DeleteUserFromLocalDb()
        {
           this. _sqLiteConnection.DeleteAll<UserLocalInfo>();
        }
        #endregion
    }
}