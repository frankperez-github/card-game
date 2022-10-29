namespace card_gameProtot
{   
    // Action extends Condition because we need Cards to extend Condition
    public class Actions : Condition
    {
        List<int>Ids = new List<int>()
        {
            3,
            4,
            5,
            6
        };
        public void TakeFromDeck(Player Owner, int cards, List<int>Ids)
        {
            if (Ids.Count() == 0)
            {
                for (int i = 0; i < cards; i++)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(1, Program.CardsInventary.Count());
                    //Console.WriteLine(random);
                    Owner.hand.Add(Program.CardsInventary[random]);
                }
            }
            else
            {
                foreach (var card in Ids)
                {
                    Owner.hand.Add(Program.CardsInventary[card]);
                }
            }
        }
        public void TakeFromEnemyHand(Player Owner, Player player, int cards)
        {
            for (int i = 0; i < cards; i++)
            {
                if (player.hand.Count() != 0)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(0, player.hand.Count()-1);
                    int cardId = player.hand[random].id;
                    player.hand.RemoveAt(random);
                    Owner.hand.Add(Program.CardsInventary[cardId]);
                }
            }
        }
        public void TakeFromGraveyard(Player Owner, int cards, List<int>Ids)
        {
            if (Ids.Count() == 0)
            {
                for (int i = 0; i < cards; i++)
                {
                    try{
                        Random rnd = new Random();
                        int randomPosition = rnd.Next(0, Program.GraveYard.Count()-1);
                        Owner.hand.Add(Program.CardsInventary[Program.GraveYard[randomPosition]]);
                        Program.GraveYard.RemoveAt(randomPosition);
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
                    Owner.hand.Add(Program.CardsInventary[Program.GraveYard[card]]);
                    Program.GraveYard.RemoveAt(card);
                    }
                    catch(System.Exception)
                    {
                        Console.WriteLine("Intentaste añadir una carta que no esta ahi");
                    }
                }
            }
            
        }
        public void Life(Player player, int affects, double factor)
        {
            player.life += affects * factor;
        }    
        public void Defense(Player player, int defense, double factor)
        {
            player.defense += defense*factor;
        }
        public void Discard(Player player, int affects, double factor, List<int>Ids)
        {
            if (Ids.Count() == 0)
            {
                for (int i = 0; i < affects*factor; i++)
                {
                    if (player.hand.Count() != 0)
                    {
                        Random rnd = new Random();
                        int randomPosition = rnd.Next(0, player.hand.Count()-1);
                        player.hand.RemoveAt(randomPosition);
                    }
                }
            }
            else
            {
                foreach (var card in Ids)
                {
                    try
                    {
                        player.hand.Remove(Program.CardsInventary[card]);
                    }
                    catch(System.Exception)
                    {
                        Console.WriteLine("Intentaste descartar una carta que no esta ahi");
                    }
                }
            }
        }
        public void ChangeState(Player player, State state)
        {
            player.state = state;
        }
    }
}