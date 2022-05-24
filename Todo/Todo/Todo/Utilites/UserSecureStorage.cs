using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Todo.Utilites
{
    public static class UserSecureStorage
    {
        #region Setting Constants
        private static readonly int
            DEFAULT_INT = 0;
        private static readonly string DEFAULT_STRING = string.Empty;
        private static readonly bool DEFAULT_BOOL = false;
        private static readonly DateTime DEFAULT_DATE = DateTime.MinValue;
        #endregion

        private static string JwtBearerTokenKey = "JwtBearerToken";
        public static string JwtBearerToken
        {
            get => SecureStorage.GetAsync(nameof(JwtBearerTokenKey)).Result;
            set => SecureStorage.SetAsync(nameof(JwtBearerTokenKey), value);
        }


        public static string UserId
        {
            get => SecureStorage.GetAsync(nameof(UserId)).Result;
            set => SecureStorage.SetAsync(nameof(UserId), value);
        }
    }
}
