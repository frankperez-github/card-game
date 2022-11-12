namespace card_gameProtot
{
    using System.Text.RegularExpressions;
    abstract class Expression
    {
        public string expressionA = ""; 
        public Player Owner;
        public Player Enemy;
        public bool IsDigit(string expression)
        {
            Regex regex = new Regex("[0-9]");
            if(regex.Matches(expression).Count() == expression.Length)
            {
                return true;
            }
            return false;
        }    
    }

    class AlgEx : Expression
    {
        public AlgEx(string expression, Player Owner, Player Enemy)
        {
            this.expressionA = expression;
            this.Owner = Owner;
            this.Enemy = Enemy;
        }
        public int Evaluate(string leftExpression,string Operator, string rightExpression)
        {
            switch (Operator)
            {
                case "+":
                    return new Add(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case "-":
                    return new Rest(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case "*":
                    return new Mult(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case "/":
                    return new Div(leftExpression, rightExpression, Owner, Enemy).Evaluate();
            }
            return -1;
        }
        public int IsVariable(string expression)
        {
            switch(expression)
            {
                case "poisoned": return 2000;
                case "freezed": return 2001;
                case "asleep": return 2002;
                case "null": return 2003;
                case "safe": return 2004;
            }
            if(expression.Substring(0, expression.IndexOf('.')) == "Owner")
            {
                return Property(Owner, expression.Substring(expression.IndexOf('.')+1, expression.Length-1-expression.IndexOf('.')));
            }
            else return Property(Enemy, expression.Substring(expression.IndexOf('.')+1, expression.Length-1-expression.IndexOf('.')));
        }
        public int Property(Player player, string expression)
        {
            switch(expression)
            {
                case "attack":
                return (int) player.attack;
                case "life":
                return (int) player.life;
                case "defense":
                return (int) player.defense;
                case "hand":
                return (int) player.hand.Count();
                case "state":
                switch(player.state)
                {
                    case State.Poisoned: return 2000;
                    case State.Freezed: return 2001;
                    case State.Asleep: return 2002;
                    case State.NULL: return 2003;
                    case State.Safe: return 2004;
                }
                break;
            }
            return -1;
        }
        public int ScanExpression()
        {  
            string input = expressionA;
            string leftExpression = "";
            string rightExpression= "";
            string Operator = "=";
            int expressionParent = 0;
            if(IsDigit(this.expressionA))
            {
                return int.Parse(this.expressionA);
            }
            for (int i = 0; i < input.Length; i++)
            {
                if((expressionParent == 0) && (input[i] == '+' || input[i] == '-' || input[i] == '*' || input[i] == '/'))
                {
                    leftExpression = input.Substring(0, i);
                    Operator = input[i] + "";
                    if(input[i+1] == '(')
                    {
                        rightExpression = input.Substring(i+2, input.Length - (i+3));
                        return Evaluate(leftExpression, Operator, rightExpression);
                    }
                    rightExpression = input.Substring(i+1, (input.Length) - (i+1));
                    return Evaluate(leftExpression, Operator, rightExpression);
                }
                if(input[i]=='(')
                {
                    expressionParent++;
                }
                if(input[i] == ')')
                {
                    expressionParent--;
                    if(expressionParent == 0)
                    {
                        leftExpression = input.Substring(1, i-1);
                        Operator = input[i+1] + "";
                        rightExpression = input.Substring(i+2, ((input.Length) - (i+2)));
                        return Evaluate(leftExpression, Operator, rightExpression);
                    }
                }
            }
            
            return IsVariable(this.expressionA);
        }
   }
    class Add: AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Add(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy).ScanExpression() + new AlgEx(rightExpression, Owner, Enemy).ScanExpression();
        }
    }
    class Rest: AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Rest(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy).ScanExpression() - new AlgEx(rightExpression, Owner, Enemy).ScanExpression();
        }
    }
    class Mult: AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Mult(string leftExpression, string rightExpression,Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy).ScanExpression() * new AlgEx(rightExpression, Owner, Enemy).ScanExpression();
        }
    }
    class Div: AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Div(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy).ScanExpression() / new AlgEx(rightExpression, Owner, Enemy).ScanExpression();
        }
    }
    
    
    class BoolEx:Expression
    {
        public BoolEx (string expression, Player Owner, Player Enemy)
        {
            this.expressionA = expression;
            this.Owner = Owner;
            this.Enemy = Enemy;

        }
        public virtual bool Evaluate(string leftExpression,string Operator, string rightExpression)
        {
            switch (Operator)
            {
                case "y":
                    return new And(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case "o":
                    return new Or(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case "=":
                    return new Equal(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case "<":
                    return new Less_Than(leftExpression, rightExpression, Owner, Enemy).Evaluate();
                case ">":
                    return new Greater_Than(leftExpression, rightExpression, Owner, Enemy).Evaluate();
            }
            return false;
        }
        public bool ScanExpression()
        {
            
            string input = this.expressionA;
            string leftExpression = "";
            string rightExpression= "";
            string Operator = "=";
            int expressionParent = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if((expressionParent == 0) && (input[i] == '=' || input[i] == '>' || input[i] == '<'))
                {
                    leftExpression = input.Substring(0, i);
                    Operator = input[i] + "";
                    if(input[i+1] == '(')
                    {
                        rightExpression = input.Substring(i+2, input.Length - (i+3));
                        break;
                    }
                    rightExpression = input.Substring(i+1, (input.Length) - (i+1));
                    break;
                }
                if(input[i]=='(')
                {
                    expressionParent++;
                }
                if(input[i] == ')')
                {
                    expressionParent--;
                    if(expressionParent == 0)
                    {
                        leftExpression = input.Substring(1, i-1);
                        Operator = input[i+1] + "";
                        if(input[i+2] == '(')
                        {
                            rightExpression = input.Substring(i+3, input.Length - (i+4));
                            break;
                        }
                        rightExpression = input.Substring(i+2, ((input.Length) - (i+2)));
                        break;
                    }
                }
            }
            return Evaluate(leftExpression, Operator, rightExpression);
        }
   }
    class And: BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public And(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {

            return new BoolEx(leftExpression, Owner, Enemy).ScanExpression() && new BoolEx(rightExpression, Owner, Enemy).ScanExpression();
        }
    }
    class Or: BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Or(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        
        public bool Evaluate()
        {
                return new BoolEx(leftExpression, Owner, Enemy).ScanExpression() || new BoolEx(rightExpression, Owner, Enemy).ScanExpression();
        }
        
    }
    class Equal: BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Equal(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {
            if(new AlgEx(this.leftExpression, Owner, Enemy).ScanExpression() == new AlgEx(this.rightExpression, Owner, Enemy).ScanExpression())
            {
                return true;
            }
            return false;
        }
    }
    class Less_Than: BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Less_Than(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {
            if(new AlgEx(this.leftExpression, Owner, Enemy).ScanExpression() < new AlgEx(this.rightExpression, Owner, Enemy).ScanExpression())
            {
                return true;
            }
            return false;
        }
    }
    class Greater_Than: BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Greater_Than(string leftExpression, string rightExpression, Player Owner, Player Enemy): base(leftExpression, Owner, Enemy)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {
            if(new AlgEx(this.leftExpression, Owner, Enemy).ScanExpression() > new AlgEx(this.rightExpression, Owner, Enemy).ScanExpression())
            {
                return true;
            }
            return false;
        }
    }
    
}