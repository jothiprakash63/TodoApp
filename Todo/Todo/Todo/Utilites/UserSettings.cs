using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Todo.Utilites
{
    public static class UserSettings
    {

        #region Setting Constants
        private static readonly int
            DEFAULT_INT = 0;
        private static readonly string DEFAULT_STRING = string.Empty;
        private static readonly bool DEFAULT_BOOL = false;
        private static readonly DateTime DEFAULT_DATE = DateTime.MinValue;
        #endregion

        public static bool AllowBiometricUnlock
        {
            get => Preferences.Get(nameof(AllowBiometricUnlock), DEFAULT_BOOL);
            set => Preferences.Set(nameof(AllowBiometricUnlock), value);
        }
        public static bool AllowFaceUnlock
        {
            get => Preferences.Get(nameof(AllowFaceUnlock), DEFAULT_BOOL);
            set => Preferences.Set(nameof(AllowFaceUnlock), value);
        }



        public static void ClearAllData()
        {
            Preferences.Clear();
        }
    }
}
