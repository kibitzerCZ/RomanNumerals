using kibitzerCZ.RomanNumerals.Model;

namespace kibitzerCZ.RomanNumerals
{
    public class RomanNumeral
    {
        /// <summary>
        /// Parses a string Roman numeral. 
        /// Throws a FormatException
        /// </summary>
        /// <param name="s">Roman numeral</param>
        /// <returns>Corresponding decimal value</returns>
        /// <exception cref="FormatException">Thrown when an invalid formatting is detected</exception>
        public static int Parse(string s, Options? options=null)
        {
            Node root = Node.CreateRoot();      //this is a root node (has no parent, no numeral and whatever child it has it is always on the right)
            Node? lastAppendedNode = root;      //we need to keep track of which literal Node was added last

            for (int i = 0; i < s.Length; i++)
            {
                // Get a character from input string. If it is a valid literal, try to append it to the last node.
                if (!Enum.TryParse(s[i].ToString(), true, out Numeral num))
                    throw new FormatException("Invalid Roman number");

                lastAppendedNode = lastAppendedNode?.Append(num, options);
            }

            return root.GetDecimalValue();
        }

        /// <summary>
        /// Tries to parse a string Roman numeral without throwing an exception.
        /// </summary>
        /// <param name="s">Roman numeral</param>
        /// <param name="decimalValue">Corresponding decimal value. If parsing failes, value is set to 0.</param>
        /// <returns>True if parsing succeeds, false otherwise</returns>
        public static bool TryParse(string s, out int decimalValue, Options? options=null)
        {
            try
            {
                decimalValue = Parse(s, options);
                return true;
            }
            catch(Exception)
            {
                decimalValue = 0;
                return false;
            }
        }
    }
}
