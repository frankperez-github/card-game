namespace card_gameProtot
{   
    // Action extends Condition because we need Cards to extend Condition
    public class Actions : Condition
    {
        public void TakeFromDeck(Player Owner, Player Enemy, relativePlayer relativePlayer, int cards, List<int>Ids)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            Player enemy = SetEnemy(player);

            if (Ids.Count() == 0)
            {
                for (int i = 0; i < cards; i++)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(1, Program.CardsInventary.Count());
                    Relics relic = Program.CardsInventary[random];
                    
                    player.hand.Add( new Relics(relic.Owner, enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                }
            }
            else
            {
                foreach (var card in Ids)
                {
                    Relics relic = Program.CardsInventary[card];
                    player.hand.Add( new Relics(player, enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                }
            }
        }
        public void TakeFromEnemyHand(Player Owner, Player Enemy,relativePlayer relativePlayer, int cards)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            Player enemy = SetEnemy(player);

            for (int i = 0; i < cards; i++)
            {
                if (player.hand.Count() != 0)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(0, player.hand.Count()-1);
                    int cardId = player.hand[random].id;
                    player.hand.RemoveAt(random);
                    Relics relic = Program.CardsInventary[random];
                    enemy.hand.Add( new Relics(player, enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                }
            }
        }
        public void TakeFromGraveyard(Player Owner, Player Enemy, relativePlayer relativePlayer, int cards, List<int>Ids)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            Player enemy = SetEnemy(player);

            if (Ids.Count() == 0)
            {
                for (int i = 0; i < cards; i++)
                {
                    try{
                        Random rnd = new Random();
                        int random = rnd.Next(0, Program.GraveYard.Count()-1);
                        Relics relic = Program.CardsInventary[random];
                        player.hand.Add( new Relics(player, enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
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
                        player.hand.Add( new Relics(player, enemy, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
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
        public void Life(Player Owner, Player Enemy, relativePlayer relativePlayer, int affects, double factor)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            player.life += affects * factor;
            Console.WriteLine(factor);
        }   
        public void Attack(Player Owner, Player Enemy, relativePlayer relativePlayer, int affects, double factor)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            player.attack += affects * factor;
        }    
        public void Defense(Player Owner, Player Enemy, relativePlayer relativePlayer, int defense, double factor)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            player.defense += defense*factor;
        }
        public void Discard(Player Owner, Player Enemy, relativePlayer relativePlayer, int affects, double factor, List<int>Ids)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
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
        public void ChangeState(Player Owner, Player Enemy, relativePlayer relativePlayer, State state)
        {
            Player player = SetPlayer(Owner, Enemy, relativePlayer);
            player.state = state;
        }
        public static Player SetPlayer(Player Owner, Player Enemy, relativePlayer relativePlayer)
        {
            Player player;
            if (relativePlayer == relativePlayer.Owner)
            {
                player = Owner;
            }
            else
            {
                player = Enemy;
            }
            return player;
        }
        public static Player SetEnemy(Player Owner)
        {
            Player Enemy;

            foreach (var player in Game.PlayersInventary)
            {
                if (player != Owner)
                {
                    Enemy = player;
                    return Enemy;
                }
            }
            throw(new Exception("Error in Actions.SetEnemy()"));
        }
    }
}