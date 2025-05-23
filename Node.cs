namespace RomanNumbers
{
    public class Node
    {
        private Node? Parent;
        private Node? Left;
        private Node? Right;

        private int mMultiplier;
        private readonly Literal? mLiteral; 

        private Node()
        {
            this.mLiteral = null;
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

        internal Node(Literal literal, Node? parent) : this()
        {
            this.mLiteral = literal;
            this.Parent = parent;
            this.mMultiplier = 1;
        }

        internal Node? Append(Literal literal)
        {
            // If I am a root node, make the icnoming literal my right child.
            if (this.mLiteral == null)
            {
                this.Right = new Node(literal, this);
                return this.Right;
            }

            // If new literal is of same value as mine, just increment the Multiplier.            
            if (this.mLiteral == literal)
            {
                if (this.Left != null)
                    throw new FormatException();

                if (this.mMultiplier == 3)
                    throw new FormatException("Invalid Roman number");

                mMultiplier++;
                return this;
            }
            
            // If incoming literal is LOWER than mine, append it as my right child.
            if (literal < this.mLiteral)
            {
                if (this.Right != null)
                    throw new Exception();

                this.Right = new Node(literal, this);
                return this.Right;
            }
            
            // If incoming literal is HIGHER than mine, append it temporarily as my right child and perform left rotation.
            //  \                                \
            // ( X )                            ( Y )
            //    \         will become          /
            //    ( Y )                       ( X )
            if (literal > this.mLiteral)
            {
                if (this.Right != null)
                    throw new Exception();

                Node n = new (literal, this);
                this.Right = n;
                this.RotateLeft();
                return n;
            }

            return null;
        }

        public int GetValue()
        {
            int result = 0;

            if (mLiteral != null)
                result = mMultiplier * (int)mLiteral;
            if (Left != null)
                result -= Left.GetValue();
            if (Right != null)
                result += Right.GetValue();
            return result;
        }

        private void RotateLeft()
        {
            if (this.Parent == null)
                throw new Exception("Cannot rotate node without a parent.");
            if (this.mMultiplier > 1)
                throw new FormatException("Invalid Roman number");

            if (this.Right == null)
                return;

            this.Parent.Right = this.Right; 
            this.Right.Parent = this.Parent;
            this.Parent = this.Right;
            this.Right.Left = this;
            this.Right = null;
        }
    }
}
