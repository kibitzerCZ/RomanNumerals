namespace kibitzerCZ.RomanNumerals
{
    public class Node
    {
        private Node? Parent;
        private Node? Left;
        private Node? Right;

        private int mMultiplier;
        private readonly Numeral? mNumeral; 

        private Node()
        {
            this.mNumeral = null;
            this.mMultiplier = 0;
            this.Parent = null;
            this.Left = null;
            this.Right = null;
        }
        
        /// <summary>
        /// Creates a root node (has no value and a child is always appended as Right)
        /// </summary>
        /// <returns></returns>
        internal static Node CreateRoot()
        {
            return new Node();
        }

        internal Node(Numeral literal, Node? parent) : this()
        {
            this.mNumeral = literal;
            this.Parent = parent;
            this.mMultiplier = 1;
        }

        internal Node? Append(Numeral numeral)
        {
            // If I am a root node, make the incoming numeral my right child.
            if (this.mNumeral == null)
            {
                this.Right = new Node(numeral, this);
                return this.Right;
            }

            // If the incoming numeral is of same value as mine, just increment the Multiplier.            
            if (this.mNumeral == numeral)
            {
                if (this.Left != null)
                    throw new FormatException();

                if (numeral == Numeral.V || numeral == Numeral.L || numeral == Numeral.D)
                    throw new FormatException("Invalid format");

                if (this.mMultiplier == 3)
                    throw new FormatException($"Too many repetitions of numeral {numeral}");

                mMultiplier++;
                return this;
            }

            // Append everything as a right child.

            // If the incoming literal is LOWER than mine, append it as my RIGHT child.
            if (numeral < this.mNumeral)
            {
                // Right child should not exist at this moment. If it does, it is an error.
                if (this.Right != null)
                    throw new Exception();

                // If I am about to create a right child with the same numeral as my left child, throw
                if (this.Left != null && this.Left.mNumeral == numeral)
                    throw new FormatException("Invalid Roman numeral");

                this.Right = new Node(numeral, this);
                return this.Right;
            }
            
            // If the incoming numeral is HIGHER than mine, append it *temporarily* as my right child and perform left rotation.
            // This way I will become the left child of the incoming numeral:
            //
            //  \                                \
            // ( X )                            ( Y )
            //    \         will become          /
            //    ( Y )                       ( X )
            //
            if (numeral > this.mNumeral)
            {
                // Right child should not exist yet. If it does, it is an error.
                if (this.Right != null)
                    throw new Exception();

                // Numerals V, L and D cannot be subtracted (i.e. cannot become the left child)
                if (this.mNumeral == Numeral.V || this.mNumeral == Numeral.L || this.mNumeral == Numeral.D)
                    throw new FormatException("Invalid format");

                // "A numeral can only precede another numeral that is equal to ten times the value of the smaller numeral or less.
                // For example, I can only precede and, thus, be subtracted from V and X, which are equal to five and ten times the value of I,
                // respectively. Under this rule, the number 1999 cannot be represented as MIM, because M is equal to one thousand times the
                // value of I. The Roman representation of 1999 is MCMXCIX, or M (1000) + CM (1000-100) + XC (100-10) + IX (10-1)."
                // [https://www.encyclopedia.com/science/encyclopedias-almanacs-transcripts-and-maps/roman-numerals-their-origins-impact-and-limitations]
                int ratio = (int)numeral / (int)this.mNumeral;
                if (ratio > 10)
                    throw new FormatException("Invalid format");

                Node n = new (numeral, this);
                this.Right = n;
                this.RotateLeft();
                return n;
            }

            return null;
        }

        private void RotateLeft()
        {
            if (this.Parent == null)
                throw new Exception("Cannot rotate node without a parent.");

            // Cannot rotate multiplied node (that would equal to subtracting more than one numeral which is not allowed).
            if (this.mMultiplier > 1)
                throw new FormatException("Invalid Roman numeral");

            // Cannot rotate left if a left child already exists - that would lead to more than one numeral to be
            // subtracted which is not allowed.
            if (this.Left != null)
                throw new FormatException("Invalid Roman numeral");

            // If no right child exists, there is nothing to rotate.
            if (this.Right == null)
                return;

            // Should left rotation lead to a child with the same numeral as its parent, throw
            if (this.Parent.mNumeral == this.Right.mNumeral)
                throw new FormatException("Invalid Roman numeral");

            this.Parent.Right = this.Right; 
            this.Right.Parent = this.Parent;
            this.Parent = this.Right;
            this.Right.Left = this;
            this.Right = null;
        }

        public int GetDecimalValue()
        {
            int result = 0;

            if (mNumeral != null)
                result = mMultiplier * (int)mNumeral;
            if (Left != null)
                result -= Left.GetDecimalValue();
            if (Right != null)
                result += Right.GetDecimalValue();
            return result;
        }

        public override string ToString()
        {
            return this.mNumeral?.ToString() ?? string.Empty;
        }
    }
}
