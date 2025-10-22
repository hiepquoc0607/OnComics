using System.Text;

namespace OnComics.Application.Utils
{
    public class Util
    {
        #region String Utils
        //Format Name First Letter To Uppercase For Each Word
        public string FormatStringName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            StringBuilder result = new StringBuilder(name.Length);
            bool newWord = true;

            foreach (char c in name)
            {
                if (char.IsWhiteSpace(c))
                {
                    result.Append(c);
                    newWord = true;
                }
                else
                {
                    result.Append(newWord ? char.ToUpper(c) : char.ToLower(c));
                    newWord = false;
                }
            }

            return result.ToString();
        }

        //Compare 2 String Array And Get Differ
        public string[] CompareStringArray(string[] a, string[] b)
        {
            HashSet<string> setB = new HashSet<string>(b);

            return a.Where(x => setB.Contains(x)).ToArray();
        }
        #endregion

        #region Password Utils
        //Compare 2 Encrypted Password
        public bool CompareHashedPassword(string pass1, string pass2)
        {
            return BCrypt.Net.BCrypt.Verify(pass1, pass2);
        }

        //Check Password Input Validation Error Type
        public string CheckPasswordErrorType(string password)
        {
            bool hasDigit = false;
            bool hasLower = false;
            bool hasUpper = false;
            bool hasSpecial = false;

            foreach (char ch in password)
            {
                if (char.IsDigit(ch)) hasDigit = true;
                else if (char.IsLower(ch)) hasLower = true;
                else if (char.IsUpper(ch)) hasUpper = true;
                else if (!char.IsLetterOrDigit(ch)) hasSpecial = true;

                if (hasDigit && hasLower && hasUpper && hasSpecial)
                {
                    return "None";
                }
            }

            if (!hasDigit) return "Number";
            if (!hasLower) return "Lower";
            if (!hasUpper) return "Upper";
            if (!hasSpecial) return "Special";

            return "None";
        }

        //Encrypt Input Password
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        #endregion

        #region Number Ultils
        //Compare 2 Int Array And Get Differ
        public int[] CompareIntArray(int[] a, int[] b)
        {
            HashSet<int> setB = new HashSet<int>(b);

            return a.Where(x => !setB.Contains(x)).ToArray();
        }

        public Dictionary<int, int> CompareIntDictionary(Dictionary<int, int> a, Dictionary<int, int> b)
        {
            var dictionary = new Dictionary<int, int>();

            foreach (var kv in a)
            {
                if (b.TryGetValue(kv.Key, out int value) && kv.Value == value)
                {
                    dictionary[kv.Key] = kv.Value;
                }
            }

            return dictionary;
        }
        #endregion
    }
}
