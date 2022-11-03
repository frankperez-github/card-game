namespace card_gameProtot
{   
    // Action extends Condition because we need Cards to extend Condition
    public class Actions : Condition
    {
        public void TakeFromDeck(Player Owner, Player Enemy, int cards, List<int>Ids)
        {
            if (Ids.Count() == 0)
            {
                for (int i = 0; i < cards; i++)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(1, Program.CardsInventary.Count());
                    Relics relic = Program.CardsInventary[random];
                    
                    Owner.hand.Add( new Relics(relic.Owner, Enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress, relic.isTrap, relic.Condition, relic.EffectsOrder));
                }
            }
            else
            {
                foreach (var card in Ids)
                {
                    Relics relic = Program.CardsInventary[card];
                    Owner.hand.Add( new Relics(Owner, Enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                }
            }
        }
        public void TakeFromEnemyHand(Player Owner, Player Enemy, int cards)
        {

            for (int i = 0; i < cards; i++)
            {
                if (Owner.hand.Count() != 0)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(0, Owner.hand.Count()-1);
                    int cardId = Owner.hand[random].id;
                    Owner.hand.RemoveAt(random);
                    Relics relic = Program.CardsInventary[random];
                    Enemy.hand.Add( new Relics(Owner, Enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                }
            }
        }
        public void TakeFromGraveyard(Player Owner, Player Enemy, int cards, List<int>Ids)
        {
            if (Ids.Count() == 0)
            {
                for (int i = 0; i < cards; i++)
                {
                    try{
                        Random rnd = new Random();
                        int random = rnd.Next(0, Program.GraveYard.Count()-1);
                        Relics relic = Program.CardsInventary[random];
                        Owner.hand.Add( new Relics(Owner, Enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                        relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                        Program.GraveYard.RemoveAt(random);
                    }
                    catch(System.Exception)
                    {
                        Console.WriteLine("Intentaste añadir una carta que no esta ahi");
                    }
                }
            }
            else
            {
                foreach (var card in Ids)
                {
                    try{
                        Relics relic = Program.CardsInventary[card];
                        Owner.hand.Add( new Relics(Owner, Enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                        relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                        Program.GraveYard.RemoveAt(card);
                    }
                    catch(System.Exception)
                    {
                        Console.WriteLine("Intentaste añadir una carta que no esta ahi");
                    }
                }
            }
            
        }
        public void Life(Player Owner, Player Enemy, int affects, double factor)
        {
            Owner.life += affects * factor;
        }   
        public void Attack(Player Owner, Player Enemy, int affects, double factor)
        {
            Owner.attack += affects * factor;
        }    
        public void Defense(Player Owner, Player Enemy, int defense, double factor)
        {
            Owner.defense += defense*factor;
        }
        public void Discard(Player Owner, Player Enemy, int affects, double factor, List<int>Ids)
        {
            if (Ids.Count() == 0)
            {
                for (int i = 0; i < affects*factor; i++)
                {
                    if (Owner.hand.Count() != 0)
                    {
                        Random rnd = new Random();
                        int randomPosition = rnd.Next(0, Owner.hand.Count()-1);
                        Owner.hand.RemoveAt(randomPosition);
                    }
                }
            }
            else
            {
                foreach (var card in Ids)
                {
                    try
                    {
                        Owner.hand.Remove(Program.CardsInventary[card]);
                    }
                    catch(System.Exception)
                    {
                        Console.WriteLine("Intentaste descartar una carta que no esta ahi");
                    }
                }
            }
        }
        public void ChangeState(Player Owner, Player Enemy,  State state)
        {
            Owner.state = state;
        }

    }
}