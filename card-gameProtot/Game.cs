namespace card_gameProtot
{
    public class Game
    {
        public static List<Relics> BattleField = new List<Relics>();
        public static List<Player> PlayersInventary = new List<Player>();
        public static Dictionary<int, Character> CharactersInventary = Program.CharactersInventary;
        public static List<int> GraveYard = new List<int>();
        public static Dictionary<Relics, int>[] activeEffects = {
            (new Dictionary<Relics, int>()),
            (new Dictionary<Relics, int>())
        };  // for each player a Dict to save active relics

        public static void game()
        {

            Console.WriteLine("Elige el personaje que seras");
            Character character1 = CharactersInventary[int.Parse(Console.ReadLine())];
            Console.WriteLine("Elige el personaje que sera tu oponente");
            Character character2 = CharactersInventary[int.Parse(Console.ReadLine())];


            Player player1 = new Player(character1, "loquito");
            Player player2 = new Player(character2, "juancito");
            PlayersInventary.Add(player1);
            PlayersInventary.Add(player2);

            int turn = 1;
            Dictionary<int, Relics> CardsInventary = Program.CardsInventary;
            List<int> Deck = CargarDeck(CardsInventary);

            Relics relic = Program.CardsInventary[1];

            // Corregido  // Carta agregada de primera, para poder acceder a ella siempre con el mismo indice
            player1.hand.Add( new Relics(player1, player2, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
            
            //Corregido   // Efecto se aplica una vez, no en cada turno
            player1.hand[0].Effect(turn);

            // Putting activated card in battlefield
            BattleField.Add(new Relics(player1, player2, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                            relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
            
            foreach (var effect in relic.EffectsOrder)
            {
                activeEffects[0].Add(relic, effect.Key); //Saving record of relics and their active effects
            }

            player1.TakeFromDeck(player1, player2, relativePlayer.Owner, 5, new List<int>());
            player2.TakeFromDeck(player2, player1, relativePlayer.Owner, 5, new List<int>());
            

            while (player1.life != 0 && player2.life != 0)
            {
                int playerId;
                if (turn % 2 != 0) //Impar
                {
                    playerId = 0;
                    player1.TakeFromDeck(player1, player2, relativePlayer.Owner, 1, new List<int>());
                    
                    // player1.PrintHand();

                    // Discounting activeDuration on each turn, eliminating card if activeDuration = 0 
                    int index = 0;

                    foreach (var activeCard in activeEffects[playerId])
                    {
                        if (activeCard.Key.activeDuration == 0)
                        {
                            foreach (var activeEffect in activeCard.Key.EffectsOrder)
                            {
                                // Removing effects of card after activeDuration is empty
                                activeCard.Key.EffectsOrder[activeEffect.Key].affects = activeCard.Key.EffectsOrder[activeEffect.Key].affects * (-1);
                                player1.hand[index].Effect(turn);
                            }
                            BattleField.RemoveAt(index); // Removing card from battelfield
                            GraveYard.Add(activeCard.Key.id); // Passing used card to graveyard
                            index++;
                            break;
                        }
                        else
                        {
                            Console.WriteLine(activeCard.Key.name);
                            activeCard.Key.activeDuration--;
                        }
                    }

                    Console.WriteLine("player1 defense: "+player1.defense);
                    Console.WriteLine("player1 attack: "+player1.attack);
                    Console.WriteLine("player1 life: "+player1.life);
                    if (activeEffects.Count() != 0)
                    {
                        foreach (var active in activeEffects[playerId])
                        {
                            Console.WriteLine("Last added card will be desactivated in: "+active.Key.activeDuration);
                        }
                    }
                    Console.ReadKey();
                }

                if (turn % 2 == 0) //Par
                {
                    playerId = 1;
                    player2.TakeFromDeck(player2, player1, relativePlayer.Owner, 1, new List<int>());
                }
                turn++; 
            }
        }
        public static List<int> CargarDeck(Dictionary<int, Relics> CardsInventary)
        {
            List<int> result = new List<int>();
            foreach (var card in CardsInventary)
            {
                result.Add(card.Value.id);    
            }
            //Console.WriteLine(string.Join(" ", result));
            return result;
        }
        

    }
}