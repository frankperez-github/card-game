namespace card_gameProtot
{
    public class Game
    {
        public static List<Player> PlayersInventary = new List<Player>();
        public static Dictionary<int, Character> CharactersInventary = Program.CharactersInventary;
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
            player1.hand[0].cardState = CardState.Activated;


            player1.TakeFromDeck(player1, player2, relativePlayer.Owner, 5, new List<int>());
            player2.TakeFromDeck(player2, player1, relativePlayer.Owner, 5, new List<int>());
            

            while (player1.life != 0 && player2.life != 0)
            {
                Console.WriteLine("Turn: "+turn);
                

                if (turn % 2 != 0) //Impar
                {
                    player1.TakeFromDeck(player1, player2, relativePlayer.Owner, 1, new List<int>());
                    
                    player1.printInfo();


                    //All activity of player 1 goes here 


                    // PRINT LAST CARD ACTIVATED=============================================================================================================================
                    // foreach (var card in player1.hand)
                    // {
                    //     if (card.cardState == CardState.Activated)
                    //     {
                    //         Console.WriteLine("Last added card will be desactivated in: "+card.activeDuration);
                    //     }
                    // }


                    // Discounting activeDuration on each turn, eliminating card if activeDuration = 0 
                    for (int index = 0; index < player1.hand.Count(); index++)
                    {
                        if (player1.hand[index].cardState == CardState.Activated)
                        {
                            if (player1.hand[index].activeDuration == 1)
                            {
                                // foreach (var activeEffect in activeCard.Key.EffectsOrder)
                                // {
                                //     // Removing effects of card after activeDuration is empty
                                //     activeCard.Key.EffectsOrder[activeEffect.Key].affects = activeCard.Key.EffectsOrder[activeEffect.Key].affects * (-1);
                                //     player1.hand[index].Effect(turn);
                                // }
                                player1.hand[index].cardState = CardState.OnGraveyard; // Removing card from battelfield
                            }
                            else
                            {
                                player1.hand[index].activeDuration--;
                            }
                        }
                    }
                    
                    Console.ReadKey();
                }

                if (turn % 2 == 0) //Par
                {
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