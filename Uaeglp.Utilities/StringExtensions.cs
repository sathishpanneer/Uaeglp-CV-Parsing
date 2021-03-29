using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Uaeglp.Utilities
{
    public static class StringExtensions
    {
        private const int LastIndexSubstring = 50;
        private const int StartIndexSubstring = 0;

        public static bool IsAlphanumeric(this string str)
        {
            return str.All(char.IsLetterOrDigit);
        }

        public static bool StartsWithLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            return char.IsLetter(str.First<char>());
        }

        public static bool IsArabicString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            return Regex.IsMatch(Regex.Replace(text, "\\t|\\n|\\r", ""), "\\p{IsArabic}|^[\\x20-\\x40]+$");
        }

        public static bool IsEnglishString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            return Regex.IsMatch(Regex.Replace(text, "\\t|\\n|\\r", ""), "^[ -~]+$|\n   ");
        }

        public static bool IsLetters(this string str)
        {
            return !str.Any<char>((Func<char, bool>)(c => !char.IsLetter(c)));
        }

        public static bool IsLettersOrSpace(this string str)
        {
            return !str.Any<char>((Func<char, bool>)(c =>
            {
                if (c != ' ')
                    return !char.IsLetter(c);
                return false;
            }));
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ToPascalCase(this string str, string cultureInfoName)
        {
            return new CultureInfo(cultureInfoName, false).TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ReplaceMany(this string s, string newVal, params char[] oldValues)
        {
            if (oldValues == null || s == "" || newVal == null)
                return s;
            string[] strArray = s.Split(oldValues, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(newVal, strArray);
        }

        public static string ReplaceMany(this string s, string newVal, params string[] oldValues)
        {
            if (oldValues == null || s == "" || newVal == null)
                return s;
            string[] strArray = s.Split(oldValues, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(newVal, strArray);
        }

        public static string CleanString(this string text, bool trim)
        {
            string str = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(text, "[‘’‚]", "'"), "[“”„]", "\""), "…", "..."), "[–—]", "-"), "ˆ", "^"), "‹", "<"), "›", ">"), "[˜ ]", " "), "\x200B", "").Replace("\v", "\r\n");
            if (trim)
                return str.Trim();
            return str;
        }

        public static string CleanString(this string text)
        {
            return text.CleanString(false);
        }

        public static string GetSafePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            path = path.CleanString(true);
            foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
                path = path.Replace(invalidFileNameChar.ToString(), "");
            return path;
        }

        public static string GetSafePath(this string path, int substringAfter)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            path = path.CleanString(true);
            foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
                path = path.Replace(invalidFileNameChar.ToString(), "");
            if (path.Length > substringAfter)
                path = path.Substring(0, substringAfter);
            return path;
        }

        public static string Space(this string str1, string other)
        {
            return str1 + " " + other;
        }

        //public static string And(this string str1, string other)
        //{
        //    return str1 + " " + UI.And + " " + other;
        //}

        public static string GetLanguage(string en, string ar)
        {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == nameof(en))
            {
                if (string.IsNullOrWhiteSpace(en))
                    return ar;
                return en;
            }
            if (string.IsNullOrWhiteSpace(ar))
                return en;
            return ar;
        }

        public static string GetLanguage(string en, string ar, string langKey)
        {
            if (langKey == nameof(en))
            {
                if (string.IsNullOrWhiteSpace(en))
                    return ar;
                return en;
            }
            if (string.IsNullOrWhiteSpace(ar))
                return en;
            return ar;
        }

        public static string OrArabic(this string en, string ar)
        {
            return StringExtensions.GetLanguage(en, ar);
        }

        public static string OrArabic(this string en, string ar, string langKey)
        {
            return StringExtensions.GetLanguage(en, ar, langKey);
        }

        public static string OrEnglish(this string ar, string en)
        {
            return StringExtensions.GetLanguage(en, ar);
        }

        public static string OrEnglish(this string ar, string en, string langKey)
        {
            return StringExtensions.GetLanguage(en, ar, langKey);
        }

        public static string Substring(string str)
        {
            if (str.Length > 50)
                return str.Substring(0, 50) + "...";
            return str;
        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string SubstringSafePath(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Length < length ? str.GetSafePath() : str.Substring(0, length).GetSafePath();
        }

        //public static string ToLiteral(this string input)
        //{
        //    using (StringWriter stringWriter = new StringWriter())
        //    {
        //        using (CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp"))
        //        {
        //            provider.GenerateCodeFromExpression((CodeExpression)new CodePrimitiveExpression((object)input), (TextWriter)stringWriter, (CodeGeneratorOptions)null);
        //            return stringWriter.ToString();
        //        }
        //    }
        //}

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsLetterOrDigit(c))
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        public static string Truncate(this string text, int length)
        {
            return text.Length <= length ? text : text.Substring(0, length);
        }

        //public static string GenerateSysName(
        //  this string name,
        //  Func<string, bool> isNameExistsReturnsTrueIfExists,
        //  bool removeSpecialChars = true,
        //  bool toPascalCase = true)
        //{
        //    if (string.IsNullOrEmpty(name))
        //        return Guid.NewGuid().ToString("N");
        //    bool flag = true;
        //    do
        //    {
        //        if (toPascalCase)
        //            name = name.ToPascalCase();
        //        if (removeSpecialChars)
        //            name = name.RemoveSpecialCharacters();
        //        name = name.Truncate(100);
        //        if (!flag)
        //        {
        //            if (name.Length > 75)
        //                name = name.Substring(0, 75);
        //            name += RandomStringGenerator.GenerateFixedLength(25, false);
        //        }
        //        flag = false;
        //    }
        //    while (isNameExistsReturnsTrueIfExists(name));
        //    return name;
        //}

        //public static bool IsValidCSharpIdentifier(this string text)
        //{
        //    return CodeDomProvider.CreateProvider("C#").IsValidIdentifier(text);
        //}

        /// <summary>
        /// Removes the last slash in a url http://asd.aa/ will be http://asd.aa
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns></returns>
        public static string RemoveURLTrailingSlash(this string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("cannot be NullOrEmpty", nameof(url));
            if (!url.EndsWith("/"))
                return url;
            return url.Substring(0, url.Length - 1);
        }
    }
}
