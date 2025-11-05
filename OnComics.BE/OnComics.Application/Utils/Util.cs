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

        //Check If File Extension Is Word
        public bool CheckWordExtension(string input)
        {
            string[] word = new string[]
            {
                ".doc",
                ".docx",
                ".rtf",
                ".txt"
            };

            if (word.Contains(input))
                return true;

            return false;
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

        #region Guid Ultils
        //Compare 2 Guid Array And Get Differ
        public Guid[] CompareGuidArray(Guid[] a, Guid[] b)
        {
            HashSet<Guid> setB = new HashSet<Guid>(b);

            return a.Where(x => !setB.Contains(x)).ToArray();
        }

        //Compare 2 Guid Dictionary And Get Same
        public Dictionary<Guid, int> CompareGuidDictionary(Dictionary<Guid, int> a, Dictionary<Guid, int> b)
        {
            var dictionary = new Dictionary<Guid, int>();

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

        #region Date Ultils
        //Compare 2 Guid Array And Get Differ
        public bool CheckDob(DateOnly date)
        {
            int requireDay = date.Day;
            int requireMonth = date.Month;
            int requireYear = DateOnly.FromDateTime(DateTime.UtcNow).Year - 13;

            DateOnly requireDate = new DateOnly(requireYear, requireMonth, requireDay);

            if (date >= requireDate)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
