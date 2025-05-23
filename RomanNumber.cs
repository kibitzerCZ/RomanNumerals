namespace RomanNumbers
{
    public class RomanNumber
    {
        public static int Parse(string s)
        {
            Node root = Node.CreateRoot();      //this is a root node (has no value and whatever child it has it is always on the right)
            Node? lastAppendedNode = root;      //we need to keep track of which literal Node was added last

            for (int i = 0; i < s.Length; i++)
            {
                // Get another character from input string. If it is a valid literal, append it to the last node.
                if (Enum.TryParse(s[i].ToString(), true, out Literal lit))
                {
                    lastAppendedNode = lastAppendedNode?.Append(lit);
                }
                else
                    throw new FormatException("Invalid Roman number");
            }

            return root.GetValue();
        }
    }
}
