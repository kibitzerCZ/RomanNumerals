namespace kibitzerCZ.RomanNumerals.Model
{
    public class Node
    {
        private Node? Parent;
        private Node? Left;
        private Node? Right;

        private int Multiplier;
        private readonly Numeral? Numeral;

        private Node(Node? parent = null)
        {
            Parent = parent;
            Numeral = null;
            Multiplier = 0;
            Left = null;
            Right = null;
        }

        internal Node(Numeral literal, Node parent) : this(parent)
        {
            Numeral = literal;
            Parent = parent;
            Multiplier = 1;
        }

        /// <summary>
        /// Creates a root node (has no value and a child is always appended as Right)
        /// </summary>
        /// <returns></returns>
        internal static Node CreateRoot()
        {
            return new Node(null);
        }

        internal Node? Append(Numeral numeral, Options? options=null)
        {
            // If I am a root node, make the incoming numeral my right child.
            if (Numeral == null)
            {
                Right = new Node(numeral, this);
                return Right;
            }

            // If the incoming numeral is of same value as mine, just increment the Multiplier.            
            if (Numeral == numeral)
            {
                // Cannot increment multiplier if left child already exists
                if (Left != null)
                    throw new FormatException();

                // Multiplication of numerals V, L and D is not allowed
                if (numeral == RomanNumerals.Numeral.V || numeral == RomanNumerals.Numeral.L || numeral == RomanNumerals.Numeral.D)
                    throw new FormatException("Invalid format");

                // By default maximum consecutive repetition of three same numerals is allowed.
                // Can be modified to allow 4 by using Options.
                if (this.Multiplier == (options?.MaximumRepetitions is MaximumRepetitions.meFour ? 4 : 3))
                    throw new FormatException($"Too many repetitions of numeral {numeral}");

                Multiplier++;
                return this;
            }

            // Append everything as a right child.

            // If the incoming literal is LOWER than mine, append it as my RIGHT child.
            if (numeral < this.Numeral)
            {
                // Right child should not exist at this moment. If it does, it is an error.
                if (Right != null)
                    throw new Exception();

                // If I am about to create a right child with the same numeral as my left child, throw
                if (Left != null && Left.Numeral == numeral)
                    throw new FormatException("Invalid Roman numeral");

                Right = new Node(numeral, this);
                return Right;
            }

            // If the incoming numeral is HIGHER than mine, append it *temporarily* as my right child and perform left rotation.
            // This way I will become the left child of the incoming numeral:
            //
            //  \                                \
            // ( X )                            ( Y )
            //    \         will become          /
            //    ( Y )                       ( X )
            //
            if (numeral > this.Numeral)
            {
                // Right child should not exist yet. If it does, it is an error.
                if (Right != null)
                    throw new Exception();

                // Numerals V, L and D cannot be subtracted (i.e. cannot become the left child)
                if (Numeral == RomanNumerals.Numeral.V || Numeral == RomanNumerals.Numeral.L || Numeral == RomanNumerals.Numeral.D)
                    throw new FormatException("Invalid format");

                CheckNumeralsRatio(numeral);

                Node n = new(numeral, this);
                Right = n;

                RotateLeft();
                return n;
            }

            return null;
        }

        private void CheckNumeralsRatio(Numeral numeral)
        {
            if (this.Numeral == null)
                return;

            // "A numeral can only precede another numeral that is equal to ten times the value of the smaller numeral or less.
            // For example, I can only precede and, thus, be subtracted from V and X, which are equal to five and ten times the value of I,
            // respectively. Under this rule, the number 1999 cannot be represented as MIM, because M is equal to one thousand times the
            // value of I. The Roman representation of 1999 is MCMXCIX, or M (1000) + CM (1000-100) + XC (100-10) + IX (10-1)."
            // [https://www.encyclopedia.com/science/encyclopedias-almanacs-transcripts-and-maps/roman-numerals-their-origins-impact-and-limitations]
            int ratio = (int)numeral / (int)this.Numeral;
            if (ratio > 10)
                throw new FormatException("Invalid format");
        }

        private void RotateLeft()
        {
            if (Parent == null)
                throw new Exception("Cannot rotate node without a parent.");

            // Cannot rotate multiplied node (that would equal to subtracting more than one numeral which is not allowed).
            if (this.Multiplier > 1)
                throw new FormatException("Invalid Roman numeral");

            // Cannot rotate left if a left child already exists - that would lead to more than one numeral to be
            // subtracted which is not allowed.
            if (Left != null)
                throw new FormatException("Invalid Roman numeral");

            // If no right child exists, there is nothing to rotate.
            if (Right == null)
                return;

            // Should left rotation lead to a repetition of forbidden numerals, format is not valid
            // E.g. (asterisk denotes the root node of a subtree to be rotated)
            //      ( L )                       ( L )
            //         \                           \  
            //        ( I )*         -->          ( L )
            //           \                         /
            //          ( L )                   ( I )*
            if (Parent.Numeral == Right.Numeral)
            {
                if (Parent.Numeral == RomanNumerals.Numeral.V || Parent.Numeral == RomanNumerals.Numeral.L || Parent.Numeral == RomanNumerals.Numeral.D)
                    throw new FormatException("Invalid Roman numeral");
            }

            Parent.Right = Right;
            Right.Parent = Parent;
            Parent = Right;
            Right.Left = this;
            Right = null;
        }

        public int GetDecimalValue()
        {
            int result = 0;

            if (Numeral != null)
                result = Multiplier * (int)Numeral;
            if (Left != null)
                result -= Left.GetDecimalValue();
            if (Right != null)
                result += Right.GetDecimalValue();
            return result;
        }

        public override string ToString() => Numeral?.ToString() ?? string.Empty;
    }
}
