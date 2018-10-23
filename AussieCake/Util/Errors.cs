﻿
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace AussieCake.Util
{
    public static class Errors
    {
        public static bool ThrowErrorMsg(ErrorType errorType, object error)
        {
            MessageBox.Show("Error " + errorType.ToDesc() + ". \nObject " + error.ToString(), errorType.ToDesc(), MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        public static bool IsNullSmallOrBigger(string original)
        {
            return IsNullSmallOrBigger(original, int.MinValue, int.MaxValue);
        }

        public static bool IsNullSmallOrBigger(string original, int minimum)
        {
            return IsNullSmallOrBigger(original, minimum, int.MaxValue);
        }

        public static bool IsNullSmallOrBigger(string original, int minimum, int maximum)
        {
            if (original.IsEmpty())
                return !ThrowErrorMsg(ErrorType.NullOrEmpty, original);
            else if (original.Length < minimum)
                return !ThrowErrorMsg(ErrorType.TooSmall, original);
            else if (original.Length > maximum)
                return !ThrowErrorMsg(ErrorType.TooBig, original);

            return false;
        }

        public static bool IsNotDigitsOnly(string input)
        {
            if (!input.IsDigitsOnly())
                return ThrowErrorMsg(ErrorType.InvalidCharacters, input);

            return true;
        }
    }

    public enum ErrorType
    {
        [Description("AlreadyInserted")]
        AlreadyInserted,

        [Description("TooSmall")]
        TooSmall,

        [Description("InvalidModelType")]
        InvalidModelType,

        [Description("NullOrEmpty")]
        NullOrEmpty,

        [Description("InvalidCharacters")]
        InvalidCharacters,

        [Description("TooBig")]
        TooBig,

        [Description("Inexistent")]
        Inexistent,

        [Description("SQLite")]
        SQLite,

        [Description("Importance.Any")]
        InvalidImportanceAny,

        [Description("InitUnrealItem")]
        InitUnrealItem,

        [Description("LoadNonInitItem")]
        LoadNonInitItem,

        [Description("NoPunctuation")]
        NoPunctuation,

        [Description("InitialLowerCase")]
        InitialLowerCase,
    }
}