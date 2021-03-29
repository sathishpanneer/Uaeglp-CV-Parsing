using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Uaeglp.Services
{
    public class RandomStringGeneratorService
    {
        private string m_pattern;
        /// <summary>True if characters can be repeated.</summary>
        public bool RepeatCharacters;
        /// <summary>True if it's not possible to create similar strings.</summary>
        public bool UniqueStrings;
        private bool m_UseUpperCaseCharacters;
        private bool m_UseLowerCaseCharacters;
        private bool m_UseNumericCharacters;
        private bool m_UseSpecialCharacters;
        private int m_MinUpperCaseCharacters;
        private int m_MinLowerCaseCharacters;
        private int m_MinNumericCharacters;
        private int m_MinSpecialCharacters;
        private bool PatternDriven;
        private char[] CurrentUpperCaseCharacters;
        private char[] CurrentLowerCaseCharacters;
        private char[] CurrentNumericCharacters;
        private char[] CurrentSpecialCharacters;
        private char[] CurrentGeneralCharacters;
        private RNGCryptoServiceProvider Random;
        private List<string> ExistingStrings;

        public RandomStringGeneratorService(
          bool UseUpperCaseCharacters = true,
          bool UseLowerCaseCharacters = true,
          bool UseNumericCharacters = true,
          bool UseSpecialCharacters = true)
        {
            m_UseUpperCaseCharacters = UseUpperCaseCharacters;
            m_UseLowerCaseCharacters = UseLowerCaseCharacters;
            m_UseNumericCharacters = UseNumericCharacters;
            m_UseSpecialCharacters = UseSpecialCharacters;
            CurrentGeneralCharacters = new char[0];
            UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            NumericCharacters = "0123456789".ToCharArray();
            SpecialCharacters = ",.;:?!/@#$%^&()=+*-_{}[]<>|~".ToCharArray();
            MinUpperCaseCharacters = MinLowerCaseCharacters = MinNumericCharacters = MinSpecialCharacters = 0;
            RepeatCharacters = true;
            PatternDriven = false;
            Pattern = "";
            Random = new RNGCryptoServiceProvider();
            ExistingStrings = new List<string>();
        }

        /// <summary>True if we need to use upper case characters</summary>
        public bool UseUpperCaseCharacters
        {
            get
            {
                return m_UseUpperCaseCharacters;
            }
            set
            {
                if (CurrentUpperCaseCharacters != null)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentUpperCaseCharacters).ToArray();
                if (value)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(CurrentUpperCaseCharacters).ToArray();
                m_UseUpperCaseCharacters = value;
            }
        }

        /// <summary>Sets or gets upper case character set.</summary>
        public char[] UpperCaseCharacters
        {
            get
            {
                return CurrentUpperCaseCharacters;
            }
            set
            {
                if (UseUpperCaseCharacters)
                {
                    if (CurrentUpperCaseCharacters != null)
                        CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentUpperCaseCharacters).ToArray();
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(value).ToArray();
                }
                CurrentUpperCaseCharacters = value;
            }
        }

        /// <summary>True if we need to use lower case characters</summary>
        public bool UseLowerCaseCharacters
        {
            get
            {
                return m_UseLowerCaseCharacters;
            }
            set
            {
                if (CurrentLowerCaseCharacters != null)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentLowerCaseCharacters).ToArray();
                if (value)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(CurrentLowerCaseCharacters).ToArray();
                m_UseLowerCaseCharacters = value;
            }
        }

        /// <summary>Sets or gets lower case character set.</summary>
        public char[] LowerCaseCharacters
        {
            get
            {
                return CurrentLowerCaseCharacters;
            }
            set
            {
                if (UseLowerCaseCharacters)
                {
                    if (CurrentLowerCaseCharacters != null)
                        CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentLowerCaseCharacters).ToArray();
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(value).ToArray();
                }
                CurrentLowerCaseCharacters = value;
            }
        }

        /// <summary>True if we need to use numeric characters</summary>
        public bool UseNumericCharacters
        {
            get
            {
                return m_UseNumericCharacters;
            }
            set
            {
                if (CurrentNumericCharacters != null)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentNumericCharacters).ToArray();
                if (value)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(CurrentNumericCharacters).ToArray();
                m_UseNumericCharacters = value;
            }
        }

        /// <summary>Sets or gets numeric character set.</summary>
        public char[] NumericCharacters
        {
            get
            {
                return CurrentNumericCharacters;
            }
            set
            {
                if (UseNumericCharacters)
                {
                    if (CurrentNumericCharacters != null)
                        CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentNumericCharacters).ToArray();
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(value).ToArray();
                }
                CurrentNumericCharacters = value;
            }
        }

        /// <summary>True if we need to use special characters</summary>
        public bool UseSpecialCharacters
        {
            get
            {
                return m_UseSpecialCharacters;
            }
            set
            {
                if (CurrentSpecialCharacters != null)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentSpecialCharacters).ToArray();
                if (value)
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(CurrentSpecialCharacters).ToArray();
                m_UseSpecialCharacters = value;
            }
        }

        /// <summary>Sets or gets special character set.</summary>
        public char[] SpecialCharacters
        {
            get
            {
                return CurrentSpecialCharacters;
            }
            set
            {
                if (UseSpecialCharacters)
                {
                    if (CurrentSpecialCharacters != null)
                        CurrentGeneralCharacters = CurrentGeneralCharacters.Except(CurrentSpecialCharacters).ToArray();
                    CurrentGeneralCharacters = CurrentGeneralCharacters.Concat(value).ToArray();
                }
                CurrentSpecialCharacters = value;
            }
        }

        /// <summary>
        /// Sets or gets minimal number of the upper case characters in generated strings.
        /// </summary>
        public int MinUpperCaseCharacters
        {
            get
            {
                return m_MinUpperCaseCharacters;
            }
            set
            {
                m_MinUpperCaseCharacters = value;
            }
        }

        /// <summary>
        /// Sets or gets minimal number of the lower case characters in generated strings.
        /// </summary>
        public int MinLowerCaseCharacters
        {
            get
            {
                return m_MinLowerCaseCharacters;
            }
            set
            {
                m_MinLowerCaseCharacters = value;
            }
        }

        /// <summary>
        /// Sets or gets minimal number of the numeric characters in generated strings.
        /// </summary>
        public int MinNumericCharacters
        {
            get
            {
                return m_MinNumericCharacters;
            }
            set
            {
                m_MinNumericCharacters = value;
            }
        }

        /// <summary>
        /// Sets or gets minimal number of the special characters in generated strings.
        /// </summary>
        public int MinSpecialCharacters
        {
            get
            {
                return m_MinSpecialCharacters;
            }
            set
            {
                m_MinSpecialCharacters = value;
            }
        }

        /// <summary>
        /// Defines the pattern to be followed to generate a string.
        /// This value is ignored if it equals empty string.
        /// Patterns are:
        /// L - for upper case letter
        /// l - for lower case letter
        /// n - for number
        /// s - for special character
        /// * - for any character
        /// </summary>
        private string Pattern
        {
            get
            {
                return m_pattern;
            }
            set
            {
                PatternDriven = !value.Equals(string.Empty);
                m_pattern = value;
            }
        }

        /// <summary>
        /// Generate a string which follows the pattern.
        /// Possible characters are:
        /// L - for upper case letter
        /// l - for lower case letter
        /// n - for number
        /// s - for special character
        /// * - for any character
        /// </summary>
        /// <param name="Pattern">The pattern to follow while generation</param>
        /// <returns>A random string which follows the pattern</returns>
        public string Generate(string Pattern)
        {
            this.Pattern = Pattern;
            string str = GenerateString(Pattern.Length);
            this.Pattern = "";
            return str;
        }

        /// <summary>
        /// Generate a string of a variable length from MinLength to MaxLength. The possible
        /// character sets should be defined before calling this function.
        /// </summary>
        /// <param name="MinLength">Minimal length of a string</param>
        /// <param name="MaxLength">Maximal length of a string</param>
        /// <returns>A random string from the selected range of length</returns>
        public string Generate(int MinLength, int MaxLength)
        {
            if (MaxLength < MinLength)
                throw new ArgumentException("Maximal length should be grater than minumal");
            return GenerateString(MinLength + GetRandomInt() % (MaxLength - MinLength));
        }

        /// <summary>
        /// Generate a string of a fixed length. The possible
        /// character sets should be defined before calling this function.
        /// </summary>
        /// <param name="FixedLength">The length of a string</param>
        /// <returns>A random string of the desirable length</returns>
        public string Generate(int FixedLength)
        {
            return GenerateString(FixedLength);
        }

        /// <summary>
        /// Main generation method which chooses the algorithm to use for the generation.
        /// It checks some exceptional situations as well.
        /// </summary>
        private string GenerateString(int length)
        {
            if (length == 0)
                throw new ArgumentException("You can't generate a string of a zero length");
            if (!UseUpperCaseCharacters && !UseLowerCaseCharacters && !UseNumericCharacters && !UseSpecialCharacters)
                throw new ArgumentException("There should be at least one character set in use");
            if (!RepeatCharacters && CurrentGeneralCharacters.Length < length)
                throw new ArgumentException("There is not enough characters to create a string without repeats");
            string s = !PatternDriven ? (MinUpperCaseCharacters != 0 || MinLowerCaseCharacters != 0 || MinNumericCharacters != 0 || MinSpecialCharacters != 0 ? GenerateAlgoWithLimits(length) : SimpleGenerateAlgo(length)) : PatternDrivenAlgo(Pattern);
            if (UniqueStrings && ExistingStrings.Contains(s))
                return GenerateString(length);
            AddExistingString(s);
            return s;
        }

        /// <summary>Generate a random string following the pattern</summary>
        private string PatternDrivenAlgo(string Pattern)
        {
            string str = "";
            List<char> existentItems = new List<char>();
            foreach (char ch1 in Pattern.ToCharArray())
            {
                char ch2 = ' ';
                switch (ch1)
                {
                    case '*':
                        ch2 = GetRandomCharFromArray(CurrentGeneralCharacters, existentItems);
                        break;
                    case 'L':
                        ch2 = GetRandomCharFromArray(CurrentUpperCaseCharacters, existentItems);
                        break;
                    case 'l':
                        ch2 = GetRandomCharFromArray(CurrentLowerCaseCharacters, existentItems);
                        break;
                    case 'n':
                        ch2 = GetRandomCharFromArray(CurrentNumericCharacters, existentItems);
                        break;
                    case 's':
                        ch2 = GetRandomCharFromArray(CurrentSpecialCharacters, existentItems);
                        break;
                    default:
                        throw new Exception("The character '" + ch1.ToString() + "' is not supported");
                }
                existentItems.Add(ch2);
                str += ch2.ToString();
            }
            return str;
        }

        /// <summary>
        /// The simpliest algorithm of the random string generation. It doesn't pay attention to
        /// limits and patterns.
        /// </summary>
        private string SimpleGenerateAlgo(int length)
        {
            string source = "";
            for (int index = 0; index < length; ++index)
            {
                char generalCharacter = CurrentGeneralCharacters[GetRandomInt() % CurrentGeneralCharacters.Length];
                if (!RepeatCharacters && source.Contains<char>(generalCharacter))
                {
                    do
                    {
                        generalCharacter = CurrentGeneralCharacters[GetRandomInt() % CurrentGeneralCharacters.Length];
                    }
                    while (source.Contains<char>(generalCharacter));
                }
                source += generalCharacter.ToString();
            }
            return source;
        }

        /// <summary>
        /// Generate a random string with specified number of minimal characters of each character set.
        /// </summary>
        private string GenerateAlgoWithLimits(int length)
        {
            if (MinUpperCaseCharacters + MinLowerCaseCharacters + MinNumericCharacters + MinSpecialCharacters > length)
                throw new ArgumentException("Sum of MinUpperCaseCharacters, MinLowerCaseCharacters, MinNumericCharacters and MinSpecialCharacters is greater than length");
            if (!RepeatCharacters && MinUpperCaseCharacters > CurrentUpperCaseCharacters.Length)
                throw new ArgumentException("Can't generate a string with this number of MinUpperCaseCharacters");
            if (!RepeatCharacters && MinLowerCaseCharacters > CurrentLowerCaseCharacters.Length)
                throw new ArgumentException("Can't generate a string with this number of MinLowerCaseCharacters");
            if (!RepeatCharacters && MinNumericCharacters > CurrentNumericCharacters.Length)
                throw new ArgumentException("Can't generate a string with this number of MinNumericCharacters");
            if (!RepeatCharacters && MinSpecialCharacters > CurrentSpecialCharacters.Length)
                throw new ArgumentException("Can't generate a string with this number of MinSpecialCharacters");
            int num = length - MinUpperCaseCharacters - MinLowerCaseCharacters - MinNumericCharacters - MinSpecialCharacters;
            string str = "";
            List<char> existentItems = new List<char>();
            for (int index = 0; index < MinUpperCaseCharacters; ++index)
                existentItems.Add(GetRandomCharFromArray(UpperCaseCharacters, existentItems));
            for (int index = 0; index < MinLowerCaseCharacters; ++index)
                existentItems.Add(GetRandomCharFromArray(LowerCaseCharacters, existentItems));
            for (int index = 0; index < MinNumericCharacters; ++index)
                existentItems.Add(GetRandomCharFromArray(NumericCharacters, existentItems));
            for (int index = 0; index < MinSpecialCharacters; ++index)
                existentItems.Add(GetRandomCharFromArray(SpecialCharacters, existentItems));
            for (int index = 0; index < num; ++index)
                existentItems.Add(GetRandomCharFromArray(CurrentGeneralCharacters, existentItems));
            for (int index1 = 0; index1 < length; ++index1)
            {
                int index2 = GetRandomInt() % existentItems.Count;
                char ch = existentItems[index2];
                existentItems.RemoveAt(index2);
                str += ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// Adding the string to the history array to support unique string generation.
        /// </summary>
        public void AddExistingString(string s)
        {
            ExistingStrings.Add(s);
        }

        /// <summary>A 16bit integer number generator.</summary>
        /// <returns>A random integer value from 0 to 65576</returns>
        private int GetRandomInt()
        {
            byte[] data = new byte[2];
            Random.GetNonZeroBytes(data);
            int num = BitConverter.ToInt16(data, 0);
            if (num < 0)
                num = -num;
            return num;
        }

        /// <summary>
        /// Get a random char from the selected array of chars. It pays attention to
        /// the RepeatCharacters flag.
        /// </summary>
        /// <param name="array">Source of symbols</param>
        /// <param name="existentItems">Existing symbols. Can be null if RepeatCharacters flag is false</param>
        /// <returns>A random character from the array</returns>
        private char GetRandomCharFromArray(char[] array, List<char> existentItems)
        {
            char ch;
            do
            {
                ch = array[GetRandomInt() % array.Length];
            }
            while (!RepeatCharacters && existentItems.Contains(ch));
            return ch;
        }

        public static string GenerateFixedLength(int FixedLength)
        {
            return new RandomStringGeneratorService(true, true, true, true).Generate(FixedLength);
        }

        public static string GenerateFixedLength(int FixedLength, bool useSpecialCharacters)
        {
            return new RandomStringGeneratorService(true, true, true, true)
            {
                UseSpecialCharacters = useSpecialCharacters
            }.Generate(FixedLength);
        }

        public static string GenerateMinMax(int MinLength, int MaxLength)
        {
            return new RandomStringGeneratorService(true, true, true, true).Generate(MinLength, MaxLength);
        }
    }
}
