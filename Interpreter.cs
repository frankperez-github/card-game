namespace card_gameProtot
{
    using System.Text.RegularExpressions;
    public class EditExpression
    {
        public bool IsDigit(string expression)
        {
            Regex regex = new Regex("[0-9]");
            if (regex.Matches(expression).Count() == expression.Length)
            {
                return true;
            }
            return false;
        }
        public string NextWord(string expression)
        {
            if(expression.Contains("("))
            {
                string word = expression.Substring(1, expression.IndexOf(".")-1);
                return word;
            }
            if(expression.Contains("."))
            {
                string word = expression.Substring(0, expression.IndexOf("."));
                return word;
            }
            return expression;
        }
        public string CutExpression(string expression)
        {
            if(expression.Contains("."))
            {
                return expression.Substring(expression.IndexOf(".")+1, (expression.Length - expression.IndexOf(".")-1));
            }
            else
            {
                return "";
            }
        }
    }
    public abstract class Expression : EditExpression
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
    }
    class ScanExpression
    {
        public string leftExpression{get;}
        public string rightExpression{get;}
        public string Operator{get;}
        
        public ScanExpression(string expressionA)
        {
            int expressionParent = 0;
            for (int i = 0; i < expressionA.Length; i++)
            {
                if (expressionA[i] == '(')
                {
                    expressionParent++;
                }
                if (expressionA[i] == ')')
                {
                    expressionParent--;
                    if (expressionParent == 0)
                    {
                        this.leftExpression = expressionA.Substring(1, i - 1);
                        this.Operator = expressionA[i + 1] + "";
                        this.rightExpression = expressionA.Substring(i + 2, ((expressionA.Length) - (i + 2)));
                        break;
                    }
                }
            }
        }
    }

    class AlgEx : Expression
    {
        public AlgEx(string expression, Player Owner, Player Enemy, Relics relics) : base(Owner, Enemy, expression, relics){}
        public int Evaluate()
        {
            if (IsDigit(this.expressionA))
            {
                return int.Parse(this.expressionA);
            }
            ScanExpression Scan = new ScanExpression(this.expressionA);
            if(Scan.leftExpression == null && Scan.rightExpression == null)
            {
                return IsVariable(this.expressionA);
            }
            switch (Scan.Operator)
            {
                case "+":
                    return new Add(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case "-":
                    return new Rest(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case "*":
                    return new Mult(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case "/":
                    return new Div(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
            }
            return -1;
        }
        public int IsVariable(string expression)
        {
            switch (expression)
            {
                case "Poisoned": return 2000;
                case "Freezed": return 2001;
                case "Asleep": return 2002;
                case "Safe": return 2003;
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
                    return (int)player.character.attack;
                case "Life":
                    return (int)player.life;
                case "Defense":
                    return (int)player.character.defense;
                case "Hand":
                    return (int)player.hand.Count();
                case "State":
                    switch (player.state)
                    {
                        case State.Poisoned: return 2000;
                        case State.Freezed: return 2001;
                        case State.Asleep: return 2002;
                        case State.Safe: return 2003;
                    }
                    break;
            }
            return -1;
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
            return new AlgEx(leftExpression, Owner, Enemy, Relic).Evaluate() + new AlgEx(rightExpression, Owner, Enemy, Relic).Evaluate();
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
            return new AlgEx(leftExpression, Owner, Enemy, Relic).Evaluate() - new AlgEx(rightExpression, Owner, Enemy, Relic).Evaluate();
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
            return new AlgEx(leftExpression, Owner, Enemy, Relic).Evaluate() * new AlgEx(rightExpression, Owner, Enemy, Relic).Evaluate();
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
            return new AlgEx(leftExpression, Owner, Enemy, Relic).Evaluate() / new AlgEx(rightExpression, Owner, Enemy, Relic).Evaluate();
        }
    }


    class BoolEx : Expression
    {
        public BoolEx(string expression, Player Owner, Player Enemy, Relics relics) : base(Owner, Enemy, expression, relics){}
        public bool Evaluate()
        {
            ScanExpression Scan = new ScanExpression(this.expressionA);
            switch (Scan.Operator)
            {
                case "y":
                    return new And(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case "o":
                    return new Or(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case "=":
                    return new Equal(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case "<":
                    return new Less_Than(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
                case ">":
                    return new Greater_Than(Scan.leftExpression, Scan.rightExpression, Owner, Enemy, Relic).Evaluate();
            }
            return false;
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
            return new BoolEx(leftExpression, Owner, Enemy, Relic).Evaluate() && new BoolEx(rightExpression, Owner, Enemy, Relic).Evaluate();
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
            return new BoolEx(leftExpression, Owner, Enemy, Relic).Evaluate() || new BoolEx(rightExpression, Owner, Enemy, Relic).Evaluate();
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
            if (new AlgEx(this.leftExpression, Owner, Enemy, Relic).Evaluate() == new AlgEx(this.rightExpression, Owner, Enemy, Relic).Evaluate())
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
            if (new AlgEx(this.leftExpression, Owner, Enemy, Relic).Evaluate() < new AlgEx(this.rightExpression, Owner, Enemy, Relic).Evaluate())
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
            if (new AlgEx(this.leftExpression, Owner, Enemy, Relic).Evaluate() > new AlgEx(this.rightExpression, Owner, Enemy, Relic).Evaluate())
            {
                return true;
            }
            return false;
        }
    }


    public class InterpreterList
    {
        public string condition;
        InterpretAction action; 

        public InterpreterList(string expression, InterpretAction action)
        {
            condition = expression;
            this.action = action;
        }
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
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), this.action.Affected.Game.GraveYard);
                        case "Hand":
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), player.hand);
                        case "Deck":
                            return AddForType(condition.Substring(i + 1, condition.Length - (i + 1)), this.action.Affected.Game.CardsInventary);
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


    public class InterpretAction : Expression
    {
        public Player Affected;
        public Player NotAffected;
        public InterpreterList FullList;
        public InterpretAction(string action, Relics card, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(Owner, Enemy, action, card)
        {
            this.Affected = Affected;
            this.NotAffected = NotAffected;
            FullList = new InterpreterList(action, this);
        }
        
        public int setFactor()
        {
            switch (this.expressionA)
            {
                case "EnemyHand":
                    return this.Relic.Enemy.hand.Count();
                case "OwnerHand":
                    return this.Relic.Owner.hand.Count();
                case "EnemyBattleField":
                    return this.Relic.Enemy.userBattleField.Length;
                case "OwnerBattleField":
                    return this.Relic.Owner.userBattleField.Length;
                case "Graveyard":
                    return this.Affected.Game.GraveYard.Count();
                default:
                    return 1;
            }
        }
        public virtual void Effect(){} 
    }
    class Cure : InterpretAction
    {
        int vida;
        int factor;
        public Cure(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy)
        {
            this.vida = int.Parse(NextWord(this.expressionA));
            this.factor = 1;
            if (this.expressionA.Contains("."))
            {   
                this.expressionA = CutExpression(this.expressionA);
                Console.WriteLine(expressionA);
                if (IsDigit(this.expressionA))
                {
                    factor = int.Parse(this.expressionA);
                }
                else
                {
                    this.factor = setFactor();
                }
            }
        }
        public override void Effect()
        {
            Affected.life += vida * factor;
        }
    }
    class Attack : InterpretAction
    {
        int damage;
        int factor;
        public Attack(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy)
        {
            this.damage = int.Parse(NextWord(this.expressionA));
            this.factor = 1;
            if (this.expressionA.Contains("."))
            {
                this.expressionA = CutExpression(this.expressionA);
                if (IsDigit(this.expressionA))
                {
                    factor = int.Parse(this.expressionA);
                }
                else
                {
                    this.factor = setFactor();
                }
            }
        }
        public override void Effect()
        {

            Affected.character.attack += damage * factor;
        }
    }
    class Draw : InterpretAction
    {
        public int cards = 1;
        public Draw(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy){}
        public override void Effect()
        {
            List<Relics> affectedCards;
            string Place = NextWord(this.expressionA);
            switch (Place)
            {
                case "EnemyHand":
                    //POSIBLEMENTE ESTO HAYA QUE MODIFICARLO EN UN FUTURO PARA AGREGAR LA OPCION DE QUE EL ENEMIGO PUEDA ROBA DE MI MANO
                    this.expressionA = CutExpression(this.expressionA);
                    if (IsDigit(NextWord(this.expressionA)))
                    {
                        NextDraw();
                        for (int i = 0; i < cards; i++)
                        {
                            if (this.Relic.Enemy.hand.Count() != 0)
                            {
                                Random rnd = new Random();
                                int random = rnd.Next(0, this.Relic.Enemy.hand.Count() - 1);
                                int cardId = this.Relic.Enemy.hand[random].id;
                                this.Relic.Enemy.hand.RemoveAt(random);
                                foreach (var card in this.Affected.Game.CardsInventary)
                                {
                                    if (card.id == cardId)
                                    {
                                        this.Relic.Owner.hand.Add(new Relics(this.Relic.Owner, this.Relic.Enemy, card.id, card.name, card.passiveDuration, card.activeDuration,
                                                card.imgAddress, card.isTrap, card.type, card.effect, card.description));
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        this.expressionA = this.expressionA.Replace(NextWord(this.expressionA) + ".", "");
                        List<Relics> affectedCard = FullList.FullList(this.expressionA, this.Relic.Enemy);
                        foreach (var relic in affectedCard)
                        {
                            Console.WriteLine("Carlos no ha implementado el metodo robar cartas especificas de la mano enemiga");
                        }
                    }
                    break;
                case "OwnerBattleField":
                    affectedCards = FullList.FullList(this.expressionA, this.Relic.Enemy);
                    foreach (var relics in affectedCards)
                    {
                        for (int i = 0; i < this.Relic.Owner.userBattleField.Length; i++)
                        {
                            if (this.Relic.Owner.userBattleField[i] == relics)
                            {
                                int cardId = this.Relic.Owner.userBattleField[i].id;
                                foreach (var card in this.Affected.Game.CardsInventary)
                                {
                                    if (card.id == cardId)
                                    {
                                        this.Relic.Owner.hand.Add(new Relics(Affected, this.Relic.Enemy, card.id, card.name, card.passiveDuration, card.activeDuration,
                                                        card.imgAddress, card.isTrap, card.type, card.effect, card.description));
                                        break;
                                    }
                                }
                                this.Relic.Owner.userBattleField[i] = null;
                            }
                        }
                    }
                    break;
                case "Graveyard":
                    affectedCards = FullList.FullList(this.expressionA, this.Relic.Enemy);
                    foreach (var card in affectedCards)
                    {
                        foreach (var Relic in this.Affected.Game.GraveYard)
                        {
                            if (Relic.id == card.id)
                            {
                                foreach (var cards in this.Affected.Game.CardsInventary)
                                {
                                    if (cards.id == card.id)
                                    {
                                        Affected.hand.Add(new Relics(Affected, this.Relic.Enemy, cards.id, cards.name, cards.passiveDuration, cards.activeDuration,
                                                cards.imgAddress, cards.isTrap, cards.type, cards.effect, card.description));
                                        break;
                                    }
                                }
                                this.Affected.Game.GraveYard.Remove(Relic);
                                break;
                            }
                        }
                    }
                    break;
                case "Deck":
                    if (IsDigit(NextWord(this.expressionA)))
                    {
                        NextDraw();
                        for (int i = 0; i < cards; i++)
                        {
                            Random rnd = new Random();
                            int random = rnd.Next(1, this.Affected.Game.CardsInventary.Count() + 1);
                            foreach (var card in this.Affected.Game.CardsInventary)
                            {
                                if (card.id == random)
                                {
                                    this.Relic.Owner.hand.Add(new Relics(Affected, this.Relic.Enemy, card.id, card.name, card.passiveDuration, card.activeDuration,
                                                    card.imgAddress, card.isTrap, card.type, card.effect, card.description));
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        affectedCards = FullList.FullList(this.expressionA, this.Relic.Enemy);
                        foreach (var card in affectedCards)
                        {
                            foreach (var cardInventary in this.Affected.Game.CardsInventary)
                            {
                                if (card.id == cardInventary.id)
                                {
                                    Affected.hand.Add(new Relics(Affected, this.Relic.Enemy, cardInventary.id, cardInventary.name, cardInventary.passiveDuration, cardInventary.activeDuration,
                                            cardInventary.imgAddress, cardInventary.isTrap, cardInventary.type, cardInventary.effect, card.description));
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
            this.cards = int.Parse(NextWord(this.expressionA));
            int factor = 1;
            if (NextWord(this.expressionA) != "")
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
        public Defense(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy)
        {
            this.defense = int.Parse(NextWord(this.expressionA));
            this.factor = 1;
            if (this.expressionA.Contains("."))
            {
                this.expressionA = CutExpression(this.expressionA);
                if (IsDigit(NextWord(this.expressionA)))
                {
                    factor = double.Parse(this.expressionA);
                }
                else
                {
                    this.factor = setFactor();
                }
            }
        }
        public override void Effect()
        {
            Affected.character.defense += defense * factor;
        }
    }
    class ChangeState : InterpretAction
    {
        public ChangeState(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy)
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
        public Remove(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy){}

        public override void Effect()
        {
            string place = NextWord(this.expressionA);
            this.expressionA = this.expressionA.Replace(place + ".", "");
            switch (place)
            {

                case "OwnerHand":
                    if (IsDigit(NextWord(this.expressionA)))
                    {
                        RemoveForint(this.Relic.Owner.hand);
                        break;
                    }
                    RemoveForList(this.Relic.Owner.hand);
                    break;
                case "EnemyHand":
                    if (IsDigit(NextWord(this.expressionA)))
                    {
                        RemoveForint(this.Relic.Enemy.hand);
                        break;
                    }
                    RemoveForList(this.Relic.Enemy.hand);
                    break;
                case "OwnerBattlefield":
                    RemoveForBattlefiel(this.Relic.Owner.userBattleField);
                    break;
                case "EnemyBattlefield":
                    RemoveForBattlefiel(this.Relic.Enemy.userBattleField);
                    break;
            }
        }
        void RemoveForint(List<Relics> Place)
        {
            int cards = int.Parse(NextWord(this.expressionA));
            int factor = 1;
            if (NextWord(this.expressionA) != "")
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
            List<Relics> affectedCards = FullList.FullList(this.expressionA, this.Relic.Enemy);
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
            List<Relics> affectedCards = FullList.FullList(this.expressionA, this.Relic.Enemy);
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
    class Show : InterpretAction
    {
        int cards;
        public Show(string action, Relics Relic, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, Relic, Affected, NotAffected, Owner, Enemy){}
        public override void Effect()
        {
            string count = NextWord(this.expressionA);
            List<Relics> show = new List<Relics>();
            if(IsDigit(count))
            {
                cards = int.Parse(count);
                Console.Clear();
                Console.WriteLine("El enemygo cuenta con " + Affected.hand.Count() + " cartas en su mano, cuales desea ver?");
                for (int i = 0; i < cards; i++)
                {
                    try
                    {
                        show.Add(Affected.hand.ElementAt(int.Parse(Console.ReadLine())));
                    }
                    catch(System.Exception)
                    {
                        break;
                    }
                    
                }
                foreach (var card in show)
                {
                    Console.WriteLine(card.name);
                }
            }
            else if(count == "all")
            {
                Console.Clear();
                cards = Affected.hand.Count();
                foreach (var card in Affected.hand)
                {
                    show.Add(card);
                    Console.WriteLine(card.name);
                }
            }
            //Metodo que debe hacer Frank para visualizar las cartas
            ShowCards(show);
        }
        public void ShowCards(List<Relics> show)
        {

        }
    }
    class StopAttack : InterpretAction
    {
        public StopAttack(string action, Relics card, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, card, Affected, NotAffected, Owner, Enemy)
        {
            //no hay nada xd
        }
    }
    class DamageReduction : InterpretAction
    {
        int ReductionFactor;
        public DamageReduction(string action, Relics card, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, card, Affected, NotAffected, Owner, Enemy)
        {
            //Aqui debe haber un numero que represente el por ciento de reduccion de da??o
            this.ReductionFactor = int.Parse(NextWord(this.expressionA));
        }
        public override void Effect()
        {
            Affected.life -=  (double)((Affected.Enemy.character.attack * (((double)ReductionFactor)/100)));
        }
    }
    class Reverse : InterpretAction
    {
        public Reverse(string action, Relics card, Player Affected, Player NotAffected, Player Owner, Player Enemy) : base(action, card, Affected, NotAffected, Owner, Enemy)
        {
        }
    }
}
