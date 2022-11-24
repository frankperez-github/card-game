namespace card_gameProtot
{
    using System.Text.RegularExpressions;
    class Expression
    {
        public string expressionA = "";
        public Player Owner;
        public Player Enemy;
        public Relics Relic;
        public Expression(Player Owner, Player Enemy, string expression, Relics Relic)
        {
            this.Owner = Owner;
            this.Enemy = Enemy;
            this.expressionA = expression;
            this.Relic = Relic;
        }
        public bool IsDigit(string expression)
        {
            Regex regex = new Regex("[0-9]");
            if (regex.Matches(expression).Count() == expression.Length)
            {
                return true;
            }
            return false;
        }
        public void Scan()
        {
            string[] expression = this.expressionA.Split('\n');
            Scan(expression, 0);
        }
        //Metodo Recursivo
        public void Scan(string[] expression, int index)
        {
            if(index==expression.Length)
            {
                return;
            }
            if (expression[index].Contains("if ("))
            {

                string condition = expression[index].Substring(expression[index].IndexOf("("), expression[index].Length -2 - expression[index].IndexOf("("));
                
                if (new BoolEx(condition, Owner, Enemy, this.Relic).ScanExpression())
                {
                    Scan(expression, index+1);
                }
                else
                {
                    //Si la condicion es falsa revisará hasta encontrar la llave de cierre correspondiente al if
                    for (int i = index+1; i < expression.Length; i++)
                    {
                        int key = 0;
                        if(expression[i].Contains("{"))
                        {
                            key++;
                        }
                        else if(expression[i].Contains("}"))
                        {
                            key--;
                        }
                        if(key == 0)
                        {
                            if(expression[i].Contains("else"))
                            {
                                Scan(expression, i+1);
                                break;
                            }
                            if(expression[i].Contains("else if ("))
                            {
                                expression[i] = expression[i].Replace("else ", "");
                                Scan(expression, i);
                                break;
                            }
                        }
                    }
                }
            }
            else if (!expression[index].Contains("{") && !expression[index].Contains("}"))
            {
                InterpretAction.InterpretExpression(expression[index], Relic);
                Scan(expression, index+1);
            }    
            //Si no es un if ni una accion es una llave y nos la saltamos
            else Scan(expression, index+1); 
        }
    }

    class AlgEx : Expression
    {
        public AlgEx(string expression, Player Owner, Player Enemy, Relics relics) : base(Owner, Enemy, expression, relics){}
        public int Evaluate(string leftExpression, string Operator, string rightExpression)
        {
            switch (Operator)
            {
                case "+":
                    return new Add(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case "-":
                    return new Rest(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case "*":
                    return new Mult(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case "/":
                    return new Div(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
            }
            return -1;
        }
        public int IsVariable(string expression)
        {
            switch (expression)
            {
                case "poisoned": return 2000;
                case "freezed": return 2001;
                case "asleep": return 2002;
                case "null": return 2003;
                case "safe": return 2004;
            }
            if (expression.Substring(0, expression.IndexOf('.')) == "Owner")
            {
                return Property(Owner, expression.Substring(expression.IndexOf('.') + 1, expression.Length - 1 - expression.IndexOf('.')));
            }
            else return Property(Enemy, expression.Substring(expression.IndexOf('.') + 1, expression.Length - 1 - expression.IndexOf('.')));
        }
        public int Property(Player player, string expression)
        {
            switch (expression)
            {
                case "Attack":
                    return (int)player.attack;
                case "Life":
                    return (int)player.life;
                case "Defense":
                    return (int)player.defense;
                case "Hand":
                    return (int)player.hand.Count();
                case "State":
                    switch (player.state)
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
            string rightExpression = "";
            string Operator = "=";
            int expressionParent = 0;
            if (IsDigit(this.expressionA))
            {
                return int.Parse(this.expressionA);
            }
            for (int i = 0; i < input.Length; i++)
            {
                if ((expressionParent == 0) && (input[i] == '+' || input[i] == '-' || input[i] == '*' || input[i] == '/'))
                {
                    leftExpression = input.Substring(0, i);
                    Operator = input[i] + "";
                    if (input[i + 1] == '(')
                    {
                        rightExpression = input.Substring(i + 2, input.Length - (i + 3));
                        return Evaluate(leftExpression, Operator, rightExpression);
                    }
                    rightExpression = input.Substring(i + 1, (input.Length) - (i + 1));
                    return Evaluate(leftExpression, Operator, rightExpression);
                }
                if (input[i] == '(')
                {
                    expressionParent++;
                }
                if (input[i] == ')')
                {
                    expressionParent--;
                    if (expressionParent == 0)
                    {
                        leftExpression = input.Substring(1, i - 1);
                        Operator = input[i + 1] + "";
                        rightExpression = input.Substring(i + 2, ((input.Length) - (i + 2)));
                        return Evaluate(leftExpression, Operator, rightExpression);
                    }
                }
            }
            return IsVariable(this.expressionA);
        }
    }
    class Add : AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Add(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy, Relic).ScanExpression() + new AlgEx(rightExpression, Owner, Enemy, Relic).ScanExpression();
        }
    }
    class Rest : AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Rest(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy, Relic).ScanExpression() - new AlgEx(rightExpression, Owner, Enemy, Relic).ScanExpression();
        }
    }
    class Mult : AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Mult(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy, Relic).ScanExpression() * new AlgEx(rightExpression, Owner, Enemy, Relic).ScanExpression();
        }
    }
    class Div : AlgEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Div(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public int Evaluate()
        {
            return new AlgEx(leftExpression, Owner, Enemy, Relic).ScanExpression() / new AlgEx(rightExpression, Owner, Enemy, Relic).ScanExpression();
        }
    }


    class BoolEx : Expression
    {
        public BoolEx(string expression, Player Owner, Player Enemy, Relics relics) : base(Owner, Enemy, expression, relics)
        {
        }
        public virtual bool Evaluate(string leftExpression, string Operator, string rightExpression)
        {
            switch (Operator)
            {
                case "y":
                    return new And(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case "o":
                    return new Or(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case "=":
                    return new Equal(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case "<":
                    return new Less_Than(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
                case ">":
                    return new Greater_Than(leftExpression, rightExpression, Owner, Enemy, Relic).Evaluate();
            }
            return false;
        }
        public bool ScanExpression()
        {

            string input = this.expressionA;
            string leftExpression = "";
            string rightExpression = "";
            string Operator = "=";
            int expressionParent = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ((expressionParent == 0) && (input[i] == '=' || input[i] == '>' || input[i] == '<'))
                {
                    leftExpression = input.Substring(0, i);
                    Operator = input[i] + "";
                    if (input[i + 1] == '(')
                    {
                        rightExpression = input.Substring(i + 2, input.Length - (i + 3));
                        break;
                    }
                    rightExpression = input.Substring(i + 1, (input.Length) - (i + 1));
                    break;
                }
                if (input[i] == '(')
                {
                    expressionParent++;
                }
                if (input[i] == ')')
                {
                    expressionParent--;
                    if (expressionParent == 0)
                    {
                        leftExpression = input.Substring(1, i - 1);
                        Operator = input[i + 1] + "";
                        if (input[i + 2] == '(')
                        {
                            rightExpression = input.Substring(i + 3, input.Length - (i + 5));
                            break;
                        }
                        rightExpression = input.Substring(i + 2, ((input.Length) - (i + 2)));
                        break;
                    }
                }
            }
            
            return Evaluate(leftExpression, Operator, rightExpression);
        }
    }
    class And : BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public And(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {

            return new BoolEx(leftExpression, Owner, Enemy, Relic).ScanExpression() && new BoolEx(rightExpression, Owner, Enemy, Relic).ScanExpression();
        }
    }
    class Or : BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Or(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }

        public bool Evaluate()
        {
            return new BoolEx(leftExpression, Owner, Enemy, Relic).ScanExpression() || new BoolEx(rightExpression, Owner, Enemy, Relic).ScanExpression();
        }

    }
    class Equal : BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Equal(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {
            if (new AlgEx(this.leftExpression, Owner, Enemy, Relic).ScanExpression() == new AlgEx(this.rightExpression, Owner, Enemy, Relic).ScanExpression())
            {
                return true;
            }
            return false;
        }
    }
    class Less_Than : BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Less_Than(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {
            if (new AlgEx(this.leftExpression, Owner, Enemy, Relic).ScanExpression() < new AlgEx(this.rightExpression, Owner, Enemy, Relic).ScanExpression())
            {
                return true;
            }
            return false;
        }
    }
    class Greater_Than : BoolEx
    {
        string leftExpression = "";
        string rightExpression = "";
        public Greater_Than(string leftExpression, string rightExpression, Player Owner, Player Enemy, Relics relics) : base(leftExpression, Owner, Enemy, relics)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }
        public bool Evaluate()
        {
            if (new AlgEx(this.leftExpression, Owner, Enemy, Relic).ScanExpression() > new AlgEx(this.rightExpression, Owner, Enemy, Relic).ScanExpression())
            {
                return true;
            }
            return false;
        }
    }


    class InterpreterList
    {
        public string condition = "";

        //He can ask for an specifics cards or specifics property of card  
        public List<Relics> FullList(string condition, Player player)
        {
            this.condition = condition;
            for (int i = 0; i < condition.Length; i++)
            {
                if (condition == "deck")
                {
                    return new List<Relics>();
                }
                if (condition[i] == '.')
                {
                    switch (condition.Substring(0, i))
                    {
                        case "Battlefield":
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), player.userBattleField.ToList());
                        case "Graveyard":
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), Game.GraveYard);
                        case "Hand":
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), player.hand);
                        case "Deck":
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), Program.CardsInventary);
                        default:
                            Console.WriteLine("Place not found xd");
                            return new List<Relics>();
                    }
                }
            }
            Console.WriteLine("Error: List condition not found");
            return new List<Relics>();
        }
        public List<Relics> AddForType(string condition, List<Relics> list)
        {
            Console.WriteLine("condition: " + condition);

            for (int i = 0; i < condition.Length; i++)
            {
                if (condition[i] == '.')
                {
                    switch (condition.Substring(0, i))
                    {
                        case "trap":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "trap");
                        case "cure":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "cure");
                        case "damage":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "damage");
                        case "defense":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "defense");
                        case "draw":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "draw");
                        case "state":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "state");
                        case "random":
                            return AddFinal(condition.Substring(i + 1, condition.Length - (i + 1)), list, "random");
                        default:
                            Console.WriteLine("type not found xd");
                            return new List<Relics>();
                    }
                }
            }
            Console.WriteLine("Type not found xd");
            return new List<Relics>();
        }
        public List<Relics> AddFinal(string condition, List<Relics> list, string type)
        {
            List<Relics> result = new List<Relics>();
            if (condition == "all")
            {
                foreach (var Relic in list)
                {
                    if (Relic != null)
                    {
                        if (type == "random")
                        {
                            result.Add(Relic);
                        }
                        else if (Relic.type == type)
                        {
                            result.Add(Relic);
                        }
                    }
                }
                return result;
            }
            Console.Clear();
            List<Relics> Cache = new List<Relics>();
            foreach (var Relic in list)
            {
                if (Relic != null)
                {
                    if (type == "random")
                    {
                        Cache.Add(Relic);
                        Console.WriteLine(Relic.name);
                    }
                    else if (Relic.type == type)
                    {
                        Cache.Add(Relic);
                    }
                }
            }
            if (Cache.Count() != 0)
            {
                Console.WriteLine("Seleccione las cartas que desee:");
                for (int i = 0; i < int.Parse(condition); i++)
                {
                    result.Add(Cache.ElementAt(int.Parse(Console.ReadLine())));
                }
            }

            return result;
        }

    }


    class InterpretAction
    {
        public Player Affected;
        public Player NotAffected;
        public string Action = "";
        public Relics card;
        public InterpretAction(string action, Relics card, Player Affected, Player NotAffected)
        {
            this.Action = action;
            this.card = card;
            if (Affected != null)
            {
                this.Affected = Affected;
                this.NotAffected = NotAffected;
            }
            else
            {

                this.Affected = SetAffected(NextWord(action));
                this.Action = this.Action.Replace(NextWord(action) + ".", "");
                this.NotAffected = SetNotAffected();
            }
        }
        public void Actions(string expression)
        {
            switch (NextWord(Action))
            {
                case "Attack":
                    new Attack(this.Action.Replace(NextWord(Action) + ".", ""), this.card, this.Affected, this.NotAffected).Effect();
                    break;
                case "Cure":
                    new Cure(this.Action.Replace(NextWord(Action) + ".", ""), this.card, this.Affected, this.NotAffected).Effect();
                    break;
                case "Draw":
                    new Draw(this.Action.Replace(NextWord(Action) + ".", ""), this.card, this.Affected, this.NotAffected).Effect();
                    break;
                case "Remove":
                    new Remove(this.Action.Replace(NextWord(Action) + ".", ""), this.card, this.Affected, this.NotAffected).Effect();
                    break;
                case "Defense":
                    new Defense(this.Action.Replace(NextWord(Action) + ".", ""), this.card, this.Affected, this.NotAffected).Effect();
                    break;
                case "ChangeState":
                    new ChangeState(this.Action.Replace(NextWord(Action) + ".", ""), this.card, this.Affected, this.NotAffected);
                    break;
            }
        }
        public string NextWord(string expression)
        {
            for (var i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '.')
                {
                    string word = expression.Substring(0, i);
                    return word;
                }
            }
            return expression;
        }
        public Player SetAffected(string player)
        {
            if (player == "Owner")
            {
                return card.Owner;
            }
            else
            {
                return card.Enemy;
            }
        }
        public Player SetNotAffected()
        {
            foreach (var player in Game.PlayersInventary)
            {
                if (player != this.Affected)
                {
                    return player;
                }
            }
            throw (new Exception("Error in Actions.SetEnemy()"));
        }
        public int setFactor()
        {
            switch (this.Action)
            {
                case "EnemyHand":
                    return this.card.Enemy.hand.Count();
                case "OwnerHand":
                    return this.card.Owner.hand.Count();
                case "EnemyBattleField":
                    return this.card.Enemy.userBattleField.Length;
                case "OwnerBattleField":
                    return this.card.Owner.userBattleField.Length;
                case "Graveyard":
                    return Game.GraveYard.Count();
                default:
                    return 1;
            }
        }
        public bool IsDigit(string expression)
        {
            Regex regex = new Regex("[0-9]");
            if (regex.Matches(expression).Count() == expression.Length)
            {
                return true;
            }
            return false;
        }
        public static void InterpretExpression(string action, Relics card)
        {
            int start = 0;
            int end = 0;
            for (var i = 0; i < action.Length; i++)
            {
                if (action[i] == '(')
                {
                    start = i + 1;
                }
                if (action[i] == ')')
                {
                    end = i;
                    string actualAction = action.Substring(start, end - start);
                    new InterpretAction(actualAction, card, null, null).Actions(actualAction);
                }
            }
        }
    }
    class Cure : InterpretAction
    {
        int vida;
        int factor;
        public Cure(string action, Relics Relic, Player Affected, Player NotAffected) : base(action, Relic, Affected, NotAffected)
        {
            this.vida = int.Parse(NextWord(this.Action));
            this.factor = 1;
            this.Action = this.Action.Replace(NextWord(action) + ".", "");
            if (this.Action.Contains("."))
            {
                if (IsDigit(this.Action))
                {
                    factor = int.Parse(this.Action);
                }
                else
                {
                    this.factor = setFactor();
                }
            }
        }
        public void Effect()
        {
            Affected.life += vida * factor;
        }
    }
    class Attack : InterpretAction
    {
        int damage;
        int factor;
        public Attack(string action, Relics Relic, Player Affected, Player NotAffected) : base(action, Relic, Affected, NotAffected)
        {
            this.damage = int.Parse(NextWord(this.Action));
            this.factor = 1;
            this.Action = this.Action.Replace(NextWord(action) + ".", "");
            if (this.Action.Contains("."))
            {
                if (IsDigit(this.Action))
                {
                    factor = int.Parse(this.Action);
                }
                else
                {
                    this.factor = setFactor();
                }
            }
        }
        public void Effect()
        {

            Affected.attack += damage * factor;
        }
    }
    class Draw : InterpretAction
    {
        public int cards = 1;
        public Draw(string action, Relics Relic, Player Affected, Player NotAffected) : base(action, Relic, Affected, NotAffected) { }
        public void Effect()
        {
            List<Relics> affectedCards;
            string Place = NextWord(this.Action);
            switch (Place)
            {
                case "EnemyHand":
                    //POSIBLEMENTE ESTO HAYA QUE MODIFICARLO EN UN FUTURO PARA AGREGAR LA OPCION DE QUE EL ENEMIGO PUEDA ROBA DE MI MANO
                    this.Action = this.Action.Replace(NextWord(Action) + ".", "");
                    if (IsDigit(NextWord(this.Action)))
                    {
                        NextDraw();
                        for (int i = 0; i < cards; i++)
                        {
                            if (this.card.Enemy.hand.Count() != 0)
                            {
                                Random rnd = new Random();
                                int random = rnd.Next(0, this.card.Enemy.hand.Count() - 1);
                                int cardId = this.card.Enemy.hand[random].id;
                                this.card.Enemy.hand.RemoveAt(random);
                                foreach (var card in Program.CardsInventary)
                                {
                                    if (card.id == cardId)
                                    {
                                        this.card.Owner.hand.Add(new Relics(this.card.Owner, this.card.Enemy, card.id, card.name, card.passiveDuration, card.activeDuration,
                                                card.imgAddress, card.isTrap, card.condition, card.type, card.effect));
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        this.Action = this.Action.Replace(NextWord(this.Action) + ".", "");
                        List<Relics> affectedCard = new InterpreterList().FullList(this.Action, this.card.Enemy);
                        foreach (var relic in affectedCard)
                        {
                            Console.WriteLine("Carlos no ha implementado el metodo robar cartas especificas de la mano enemiga");
                        }
                    }
                    break;
                case "OwnerBattleField":
                    affectedCards = new InterpreterList().FullList(this.Action, this.card.Enemy);
                    foreach (var relics in affectedCards)
                    {
                        for (int i = 0; i < this.card.Owner.userBattleField.Length; i++)
                        {
                            if (this.card.Owner.userBattleField[i] == relics)
                            {
                                int cardId = this.card.Owner.userBattleField[i].id;
                                foreach (var card in Program.CardsInventary)
                                {
                                    if (card.id == cardId)
                                    {
                                        this.card.Owner.hand.Add(new Relics(Affected, this.card.Enemy, card.id, card.name, card.passiveDuration, card.activeDuration,
                                                        card.imgAddress, card.isTrap, card.condition, card.type, card.effect));
                                        break;
                                    }
                                }
                                this.card.Owner.userBattleField[i] = null;
                            }
                        }
                    }
                    break;
                case "Graveyard":
                    affectedCards = new InterpreterList().FullList(this.Action, this.card.Enemy);
                    foreach (var card in affectedCards)
                    {
                        foreach (var Relic in Game.GraveYard)
                        {
                            if (Relic.id == card.id)
                            {
                                foreach (var cards in Program.CardsInventary)
                                {
                                    if (cards.id == card.id)
                                    {
                                        Affected.hand.Add(new Relics(Affected, this.card.Enemy, cards.id, cards.name, cards.passiveDuration, cards.activeDuration,
                                                cards.imgAddress, cards.isTrap, cards.condition, cards.type, cards.effect));
                                        break;
                                    }
                                }
                                Game.GraveYard.Remove(Relic);
                                break;
                            }
                        }
                    }
                    break;
                case "Deck":
                    if (IsDigit(NextWord(this.Action)))
                    {
                        NextDraw();
                        for (int i = 0; i < cards; i++)
                        {
                            Random rnd = new Random();
                            int random = rnd.Next(1, Program.CardsInventary.Count() + 1);
                            foreach (var card in Program.CardsInventary)
                            {
                                if (card.id == random)
                                {
                                    this.card.Owner.hand.Add(new Relics(Affected, this.card.Enemy, card.id, card.name, card.passiveDuration, card.activeDuration,
                                                    card.imgAddress, card.isTrap, card.condition, card.type, card.effect));
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        affectedCards = new InterpreterList().FullList(this.Action, this.card.Enemy);
                        foreach (var card in affectedCards)
                        {
                            foreach (var cardInventary in Program.CardsInventary)
                            {
                                if (card.id == cardInventary.id)
                                {
                                    Affected.hand.Add(new Relics(Affected, this.card.Enemy, cardInventary.id, cardInventary.name, cardInventary.passiveDuration, cardInventary.activeDuration,
                                            cardInventary.imgAddress, cardInventary.isTrap, cardInventary.condition, cardInventary.type, cardInventary.effect));
                                    break;
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void NextDraw()
        {
            this.cards = int.Parse(NextWord(this.Action));
            int factor = 1;
            if (NextWord(this.Action) != "")
            {
                factor = setFactor();
            }
            this.cards = this.cards * factor;
        }
    }
    class Defense : InterpretAction
    {
        int defense;
        double factor;
        public Defense(string action, Relics Relic, Player Affected, Player NotAffected) : base(action, Relic, Affected, NotAffected)
        {
            this.defense = int.Parse(NextWord(this.Action));
            this.factor = 1;
            this.Action = this.Action.Replace(NextWord(action) + ".", "");
            if (this.Action.Contains("."))
            {
                if (IsDigit(NextWord(this.Action)))
                {
                    factor = double.Parse(this.Action);
                }
                else
                {
                    this.factor = setFactor();
                }
            }
        }
        public void Effect()
        {
            Affected.defense += defense * factor;
        }
    }
    class ChangeState : InterpretAction
    {
        public ChangeState(string action, Relics Relic, Player Affected, Player NotAffected) : base(action, Relic, Affected, NotAffected)
        {
            switch (action)
            {
                case "Freezed":
                    this.Affected.state = State.Freezed;
                    break;
                case "Poisoned":
                    this.Affected.state = State.Poisoned;
                    break;
                case "Safe":
                    this.Affected.state = State.Safe;
                    break;
                case "Asleep":
                    this.Affected.state = State.Asleep;
                    break;
            }
        }
    }
    class Remove : InterpretAction
    {
        public Remove(string action, Relics Relic, Player Affected, Player NotAffected) : base(action, Relic, Affected, NotAffected) { }

        public void Effect()
        {
            string place = NextWord(this.Action);
            this.Action = this.Action.Replace(place + ".", "");
            switch (place)
            {

                case "OwnerHand":
                    if (IsDigit(NextWord(this.Action)))
                    {
                        RemoveForint(this.card.Owner.hand);
                        break;
                    }
                    RemoveForList(this.card.Owner.hand);
                    break;
                case "EnemyHand":
                    if (IsDigit(NextWord(this.Action)))
                    {
                        RemoveForint(this.card.Enemy.hand);
                        break;
                    }
                    RemoveForList(this.card.Enemy.hand);
                    break;
                case "OwnerBattlefield":
                    RemoveForBattlefiel(this.card.Owner.userBattleField);
                    break;
                case "EnemyBattlefield":
                    RemoveForBattlefiel(this.card.Enemy.userBattleField);
                    break;
            }
        }
        void RemoveForint(List<Relics> Place)
        {
            int cards = int.Parse(NextWord(this.Action));
            int factor = 1;
            if (NextWord(this.Action) != "")
            {
                factor = setFactor();
            }
            cards = cards * factor;
            for (int i = 0; i < cards; i++)
            {
                Random rnd = new Random();
                int random = rnd.Next(1, Place.Count());
                Place.RemoveAt(random);
            }
        }
        void RemoveForList(List<Relics> Place)
        {
            List<Relics> affectedCards = new InterpreterList().FullList(this.Action, this.card.Enemy);
            foreach (var listCard in affectedCards)
            {
                foreach (var cardPlace in Place)
                {
                    if (listCard == cardPlace)
                    {
                        Place.Remove(listCard);
                    }
                }
            }
        }
        void RemoveForBattlefiel(Relics[] Battlefield)
        {
            List<Relics> affectedCards = new InterpreterList().FullList(this.Action, this.card.Enemy);
            for (int i = 0; i < Battlefield.Length; i++)
            {
                foreach (var listCard in affectedCards)
                {
                    if (listCard == Battlefield[i])
                    {
                        Battlefield[i] = null;
                        break;
                    }
                }
            }
        }
    }
}