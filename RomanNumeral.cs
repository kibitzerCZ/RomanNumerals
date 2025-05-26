using kibitzerCZ.RomanNumerals.Model;

namespace kibitzerCZ.RomanNumerals
{
    public class RomanNumeral
    {
        /// <summary>
        /// Parses a Roman numeral string. 
        /// Throws a FormatException if numeral is not valid.
        /// </summary>
        /// <param name="s">Roman numeral</param>
        /// <returns>Corresponding decimal value</returns>
        /// <exception cref="FormatException">Thrown when an invalid formatting is detected</exception>
        public static int Parse(string s, Options? options=null)
        {
            Node root = Node.CreateRoot();      //this is a root node (has no parent, no numeral and whatever child it has it is always on the right)
            Node? lastAppendedNode = root;      //we need to keep track of which numeral Node was added last
            
            for (int i = 0; i < s.Length; i++)
            {
                // Get a character from input string. If it is a valid numeral, try to append it to the last node.
                if (!Enum.TryParse(s[i].ToString(), true, out Numeral num))
                    throw new FormatException("Invalid Roman number");

                lastAppendedNode = lastAppendedNode?.Append(num, options);
            }

            return root.GetDecimalValue();
        }

        /// <summary>
        /// Tries to parse a Roman numeral string without throwing an exception.
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
