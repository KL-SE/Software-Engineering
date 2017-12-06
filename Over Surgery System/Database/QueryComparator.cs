using System;

namespace OverSurgerySystem
{
    // Comparator of a databse query.
    public class QueryComparator : QueryElement
    {
        public static class Modes
        {
            public const int Or     = 1;
            public const int Not    = 2;
            public const int Equal  = 4;
            public const int Less   = 8;
            public const int More   = 16;
            public const int Like   = 32;
        }
        
        private int mode;
        public QueryElement Source  { set; get; }
        public QueryElement Operand { set; get; }

        // Mode operators
        public void SetMode( int m , bool value )
        {
            if( value )
            {
                SetMode( m );
            }
            else
            {
                ClearMode( m );
            }
        }

        public void SetMode( int m )
        {
            mode |= m;
        }

        public void ClearMode( int m )
        {
            mode &= ~m;
        }

        public bool HasMode( int m )
        {
            return ( mode & m ) != 0;
        }

        // Mode's properties
        public bool Or
        {
            set
            {
                SetMode( Modes.Or , value );
                if( value )
                    SetMode( Modes.Equal | Modes.Less | Modes.More | Modes.Like , false );
            }
            get
            {
                return HasMode( Modes.Or ) && !Equal && !Less && !More && !Like;
            }
        }
        
        public bool And
        {
            set
            {
                SetMode( Modes.Or , !value );
                if( !value )
                    SetMode( Modes.Equal | Modes.Less | Modes.More | Modes.Like , false );
            }
            get
            {
                return !HasMode( Modes.Or ) && !Equal && !Less && !More && !Like;
            }
        }
        
        public bool Not
        {
            set
            {

                SetMode( Modes.Not , value );
            }
            get
            {
                return HasMode( Modes.Not );
            }
        }
        
        public bool Equal
        {
            set
            {

                SetMode( Modes.Equal , value );
            }
            get
            {
                return HasMode( Modes.Equal );
            }
        }
        
        public bool Less
        {
            set
            {

                SetMode( Modes.Less , value );
            }
            get
            {
                return HasMode( Modes.Less );
            }
        }
        
        public bool More
        {
            set
            {

                SetMode( Modes.More , value );
            }
            get
            {
                return HasMode( Modes.More );
            }
        }
        
        public bool Like
        {
            set
            {
                SetMode( Modes.Like , value );
                if( value )
                    SetMode( Modes.Equal | Modes.Less | Modes.More | Modes.Or , false );
            }
            get
            {
                return HasMode( Modes.Like ) && !HasMode( Modes.Or ) && !Equal && !Less && !More;
            }
        }

        // Function to apply brackets to an inner comparator if it has a dfferent sign than the current comparator.
        public string Bracketify( QueryElement element )
        {
            if( element is QueryComparator )
            {
                QueryComparator comparator  = element as QueryComparator;
                bool incompatibleOperator   = comparator.And && this.Or || comparator.Or && this.And;
                if( !comparator.Not && incompatibleOperator )
                {
                    return String.Format( "({0})" , element.Stringify() );
                }
            }

            return element.Stringify();
        }

        // Convert the comparator values into string.
        public override string Stringify()
        {
            bool isOperandQuery     = Operand.Value is DatabaseQuery;
            string sourceString     = Bracketify( Source );
            string operandString    = Bracketify( Operand );
            string operatorString   = isOperandQuery ? "IN" : "";
            
            if( !isOperandQuery )
            {
                // Cases:
                //  - Less: <
                //  - More: >
                //  - Equal: =
                //  - Less & Equal: <=
                //  - More & Equal: >=
                if( Less  ) operatorString = "<"; else
                if( More  ) operatorString = ">";
                if( Equal ) operatorString += "=";

                // OR/AND operator, if there are no other comparison operators present.
                if( Or )
                {
                    operatorString = "OR";
                }
                else if( And )
                {
                    operatorString = "AND";
                }
                else if( Like )
                {
                    operatorString = "LIKE";
                }
            }

            // Add a NOT operator behind the expression if needed, and return the formed string.
            return String.Format( Not ? "NOT ({0} {1} {2})" : "{0} {1} {2}" , sourceString , operatorString , operandString );
        }
        
        // Convert the comparator values into string.
        public override string ToString()
        {
            return Stringify();
        }
    }
}
