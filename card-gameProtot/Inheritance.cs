namespace card_gameProtot
{
    //PassiveDuration: Cada cuanto tiempo se activa el efecto
    //ActiveDuration: Cuanto tiempo dura el efecto
    public class Condition
    {
        public bool condition = false;

        public Condition()
        {
            this.condition = true;
        }
        public Condition(int a, int b, int operId)
        {
            switch (operId)
            {
                case 1: // = operator
                    if (a == b)
                    {
                        this.condition = true;
                    }
                    break; 

                case 2: // < operator
                    if (a < b)
                    {
                        this.condition = true;
                    }
                    break; 

                case 3: // > operator
                    if (a > b)
                    {
                        this.condition = true;
                    }
                    break;

                case 4: // <= operator
                    if (a <= b)
                    {
                        this.condition = true;
                    }
                    break; 

                case 5: // >= operator
                    if (a >= b)
                    {
                        this.condition = true;
                    }
                    break;  

            }
        }
        public Condition(State a, State b)
        {
            if (a == b)
            {
                this.condition = true;
            }
        }
        public Condition(CardState a, CardState b)
        {
            if (a == b)
            {
                this.condition = true;
            }
        }


    }
    public class Cards : Actions
    {
        public string name = "";
        public int activeDuration;
        public int passiveDuration;
        public string imgAddress = "";
        Condition condition;

        public Cards(string name, int passiveDuration, int activeDuration, string imgAddress)
        {
            this.name = name;
            this.imgAddress = imgAddress;
            this.passiveDuration = passiveDuration;
            this.activeDuration = activeDuration;
        }
    } 
    public class Relics : Cards
    {
        public int id;
        public Dictionary<int, ActionInfo> EffectsOrder = new Dictionary<int, ActionInfo>();
        Condition Condition;
        public bool isTrap;
        CardState cardState;

        public Relics(int id, string Name, int passiveDuration, int activeDuration, string imgAddress, bool isTrap, Condition condition, Dictionary<int, ActionInfo> EffectsOrder) 
                      : base (Name, passiveDuration, activeDuration, imgAddress)
        {
            this.id = id;
            this.EffectsOrder = EffectsOrder;
            this.Condition = condition;
            this.cardState = CardState.OnDeck;
            this.isTrap = isTrap;
        }


        public void Effect(int turn, Player Owner, Player Enemy)
        {
            if (Condition.condition)
            {
                foreach(var Effect in EffectsOrder)
                {
                    switch (Effect.Key)
                    {
                        case 1:
                            TakeFromDeck(Owner, Effect.Value.affects, Effect.Value.affectedIds);
                            break;

                        case 2:
                            TakeFromEnemyHand(Owner, Effect.Value.player, Effect.Value.affects);
                            break;

                        case 3:
                            TakeFromGraveyard(Owner, Effect.Value.affects, Effect.Value.affectedIds);
                            break;

                        case 4:
                            Life(Effect.Value.player, Effect.Value.affects, Effect.Value.factor);
                            break;

                        case 5:
                            Defense(Effect.Value.player, Effect.Value.affects, Effect.Value.factor);
                            break;
                        
                        case 6:
                            Discard(Effect.Value.player, Effect.Value.affects, Effect.Value.factor, Effect.Value.affectedIds);
                            break;

                        case 7:
                            ChangeState(Effect.Value.player, Effect.Value.state);
                            break;

                    }
                }
            }
        }
    }
    public class Character : Cards
    {
        public int attack;
        public double defense;

        /// <returns>Construye un personaje</returns>
        public Character(string name, int passiveDuration, int activeDuration, string imgAddress, int attack, double defense) : base(name, passiveDuration, activeDuration, imgAddress)
        {
            this.attack = attack;
            this.defense = defense;
        }


        public static void SpecialAttack(Player player, int attack, int defense, int life, State state)
        {
            player.attack += attack;
            player.defense += defense;
            player.state = state;
            player.life = life;
        }
    }
    public class Player : Character 
    {
        public string nick = "";
        public double life;
        public List<Relics> hand = new List<Relics>();
        public State state = State.Safe;

        public Player(Character character, string nick): base(character.name, character.passiveDuration, character.activeDuration, character.imgAddress, character.attack, character.defense) 
        {
            this.nick = nick;
            this.life = 100;
        }

        public void PrintHand()
        {
            foreach (var card in hand)
            {
                Console.Write(" " + card.id);            
            }
        }
    }

    #region auxiliar classes
    public class FullList
    {
        public List<int> affectedIds = new List<int>();
        public FullList(List<int> Place, bool isTrap)
        {
            foreach (var card in Place)
            {
                if(isTrap)
                {
                    if (Program.CardsInventary[card].isTrap)
                    {
                        this.affectedIds.Add(card);
                    }
                }
                if (!isTrap)
                {
                    if (!Program.CardsInventary[card].isTrap)
                    {
                        this.affectedIds.Add(card);
                    }
                }
                
            }
        }

        public FullList(List<int> Place, Dictionary<int, ActionInfo> EffectsOrder, int ActionId, Player player, string condicion)
        {
            char operater = condicion[0];
            string conditionClean = condicion.Remove(0,1);
            if(operater=='+')
            {
                foreach (var card in Place)
                {
                    if(Program.CardsInventary[card].EffectsOrder[ActionId].player==player && 
                    Program.CardsInventary[card].EffectsOrder[ActionId].affects>=int.Parse(condicion))
                    {
                        this.affectedIds.Add(card);
                    }
                }
            }
            if(operater=='-')
            {
                foreach (var card in Place)
                {
                    if(Program.CardsInventary[card].EffectsOrder[ActionId].player==player && 
                    Program.CardsInventary[card].EffectsOrder[ActionId].affects<=int.Parse(condicion))
                    {
                        this.affectedIds.Add(card);
                    }
                }
            }
            if(operater=='=')
            {
                foreach (var card in Place)
                {
                    if(Program.CardsInventary[card].EffectsOrder[ActionId].player==player && 
                    Program.CardsInventary[card].EffectsOrder[ActionId].affects==int.Parse(condicion))
                    {
                        this.affectedIds.Add(card);
                    }
                }
            }
            
        }
    }
    public class ActionInfo
    {
        public Player player;
        public State state;
        public double factor;
        public int affects;
        public List<int> affectedIds;

        public ActionInfo(int affects, List<int> affectedIds)
        {
            this.affects = affects;
            this.affectedIds = affectedIds;
        }

        public ActionInfo(Player player, int affects, List<int> affectedIds)
        {
            this.player = player;
            this.affects = affects;
            this.affectedIds = affectedIds;
        }

        public ActionInfo(Player player, int affects, double factor)
        {
            this.player = player;
            this.affects = affects;
            this.factor = factor;
        }

        public ActionInfo(Player player, int affects, List<int> affectedIds, double factor)
        {
            this.player = player;
            this.affects = affects;
            this.affectedIds = affectedIds;
            this.factor = factor;
        }

        public ActionInfo(Player player, State state)
        {
            this.player = player;
            this.state = state;
        }
    }
    public enum CardState
    {
        OnDeck,
        Activated,
        OnHand,
        OnGraveyard
    }
    public enum State
    {
        Safe,
        Poisoned,
        Freezed,
        Asleep,
    }
    
    #endregion
}