using System.Text;

namespace OnComics.Library.Utils.Utils
{
    public class Util
    {
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
    }
}
