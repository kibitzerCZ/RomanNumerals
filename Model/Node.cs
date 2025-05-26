namespace kibitzerCZ.RomanNumerals.Model
{
    public class Node
    {
        /// <summary>
        /// Parent node
        /// </summary>
        private Node? Parent;
        /// <summary>
        /// Left child
        /// </summary>
        private Node? Left;
        /// <summary>
        /// Right child
        /// </summary>
        private Node? Right;


        /// <summary>
        /// Numeral this node holds
        /// </summary>
        private readonly Numeral? Numeral;

        /// <summary>
        /// Same consecutive numerals counter
        /// </summary>
        private int Multiplier;

        private Node(Node? parent = null)
        {
            Parent = parent;
            Numeral = null;
            Multiplier = 0;
            Left = null;
            Right = null;
        }

        internal Node(Numeral numeral, Node parent) : this(parent)
        {
            Numeral = numeral;
            Parent = parent;
            Multiplier = 1;
        }

        /// <summary>
        /// Creates a root node (has no parent, no children and no value)
        /// </summary>
        /// <returns></returns>
        internal static Node CreateRoot()
        {
            return new Node(null);
        }

        /// <summary>
        /// Appends new node with given numeral to the current node.
        /// </summary>
        /// <param name="numeral"></param>
        /// <param name="options"></param>
        /// <returns>The currently appended node (node with the input numeral)</returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="Exception"></exception>
        internal Node? Append(Numeral numeral, Options? options=null)
        {
            // If I am a root node, make the incoming numeral my right child.
            if (this.Numeral == null)
            {
                Right = new Node(numeral, this);
                return Right;
            }

            // If the incoming numeral is of same value as mine, just increment the Multiplier.            
            if (this.Numeral == numeral)
            {
                // Cannot increment multiplier if left child (i.e. lower numeral) already exists (e.g. IX is valid, but IXX is not)
                if (Left != null)
                    throw new FormatException();

                // Multiplication of numerals V, L and D is not allowed
                if (CheckUnrepeatableNumeral(numeral))
                    throw new FormatException("Invalid format");

                // By default maximum consecutive repetition of three same numerals is allowed.
                // Can be modified to allow 4 by using Options.
                if (this.Multiplier == (options?.MaximumRepetitions is MaximumRepetitions.mrFour ? 4 : 3))
                    throw new FormatException($"Invalid format. Too many repetitions of numeral {numeral}");

                Multiplier++;
                return this;
            }

            // Append everything else as a RIGHT child.

            // If the incoming numeral is LOWER than mine, append it as my right child (for good).
            if (numeral < this.Numeral)
            {
                // Right child should not exist at this moment. If it does, it is an error.
                if (Right != null)
                    throw new Exception();

                // If I am about to create a right child with the same numeral as my left child, throw
                if (Left != null && Left.Numeral == numeral)
                    throw new FormatException("Invalid format");

                Right = new Node(numeral, this);
                return Right;
            }

            // If the incoming numeral is HIGHER than mine, append it *temporarily* as my right child and perform left rotation.
            // This way I (X) will become the left child of the incoming numeral (Y):
            //
            //  \            \                     \
            // ( X )        ( X )                 ( Y )
            //         ->      \         =>        /
            //                ( Y )             ( X )
            //
            if (numeral > this.Numeral)
            {
                // Right child should not exist yet. If it does, it is an error.
                if (Right != null)
                    throw new Exception("Right child already exists");

                CheckNumeralsRatio(numeral);

                // Numerals V, L and D cannot be subtracted (i.e. cannot become the left child). So check my numeral before rotation.
                if (CheckUnsubtractableNumeral(this.Numeral.Value))
                    throw new FormatException("Invalid format");

                Node n = new(numeral, this);
                Right = n;

                RotateLeft();
                return n;
            }

            return null;
        }

        /// <summary>
        /// Checks if lower numeral I am about to append as a left child is at most 10 times lower than my numeral.
        /// </summary>
        /// <param name="numeral"></param>
        /// <exception cref="FormatException">Thrown if numeral's ratio to my numeral is larger than 10.</exception>
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

        /// <summary>
        /// Checks whether the given numeral belongs to the set of unrepeatable numerals (V, L and D)
        /// </summary>
        /// <param name="numeral"></param>
        /// <returns>True if numeral is unrepeatable, false otherwise</returns>
        private static bool CheckUnrepeatableNumeral(Numeral numeral)
        {
            return (numeral == RomanNumerals.Numeral.V || numeral == RomanNumerals.Numeral.L || numeral == RomanNumerals.Numeral.D);
        }

        /// <summary>
        /// Checks whether the given numeral can be substracted (i.e. can be a left child)
        /// </summary>
        /// <param name="numeral"></param>
        /// <returns></returns>
        private static bool CheckUnsubtractableNumeral(Numeral numeral)
        {
            //the same set as for unrepeatable numerals
            return CheckUnrepeatableNumeral(numeral);
        }

        /// <summary>
        /// Performs left rotation to the current node. The node's right child will become parent to the node (the node will become left
        /// child of the new parent).
        /// </summary>
        /// <exception cref="Exception">Rotation is not possible due to data error</exception>
        /// <exception cref="FormatException">Rotation is not possible due to rules breaching</exception>
        private void RotateLeft()
        {
            if (Parent == null)
                throw new Exception("Cannot rotate node without a parent.");

            // Cannot rotate multiplied node (that would equal to subtracting more than one numeral which is not allowed).
            if (this.Multiplier > 1)
                throw new FormatException("Invalid format");

            // Cannot rotate left if a left child already exists - that would lead to more than one numeral to be
            // subtracted which is not allowed.
            if (Left != null)
                throw new FormatException("Invalid format");

            // If no right child exists, there is nothing to rotate.
            if (Right == null)
                return;

            // Should left rotation lead to a repetition of forbidden numerals, format is not valid
            // E.g. (asterisk denotes the root node of a subtree to be rotated)
            //      ( L )                       ( L )
            //         \                           \  
            //        ( I )*         -->          ( L )
            //           \                         /
            //          ( L )                   ( I )
            if (Parent.Numeral != null && Parent.Numeral == Right.Numeral)
            {
                if (CheckUnrepeatableNumeral(Parent.Numeral.Value))
                    throw new FormatException("Invalid format");
            }

            // My right child will become my parent's new right child (and my new parent - I will become his left child)
            Parent.Right = Right;
            Right.Parent = Parent;
            Right.Left = this;
            Parent = Right;
            // I will lose my right child
            Right = null;
        }

        /// <summary>
        /// Gets decimal value of given numeral
        /// </summary>
        /// <returns></returns>
        internal int GetDecimalValue()
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
