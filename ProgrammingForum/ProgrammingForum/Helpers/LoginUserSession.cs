using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProgrammingForum.Helpers
{
    public class LoginUserSession
    {
        #region Properties
        public int UserID { get; private set; }
        public string Username { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public bool IsAdmin { get; private set; }
        #endregion

        #region Constructors
        private LoginUserSession()
        {
            IsAuthenticated = false;
        }
        #endregion

        #region Public properties
        public static LoginUserSession Current
        {
            get
            {
                LoginUserSession loginUserSession = (LoginUserSession)HttpContext.Current.Session["LoginUser"];
                if (loginUserSession == null)
                {
                    loginUserSession = new LoginUserSession();
                    HttpContext.Current.Session["LoginUser"] = loginUserSession;
                }
                return loginUserSession;
            }
        }
        #endregion

        #region public methods
        public void SetCurrentUser(int userID, string username, bool isAdmin)
        {
            this.IsAuthenticated = true;
            this.UserID = userID;
            this.Username = username;
            this.IsAdmin = isAdmin;
        }

        public void Logout()
        {
            this.IsAuthenticated = false;
            this.UserID = 0;
            this.Username = string.Empty;
            this.IsAdmin = false;
        }
        #endregion
    }
}