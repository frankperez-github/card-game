namespace card_gameProtot
{
    public class Game
    {
        public static List<Player> PlayersInventary = new List<Player>();
        public static Dictionary<int, Character> CharactersInventary = Program.CharactersInventary;
        public static void game()
        {
            Console.Clear();
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
            // player1.hand.Add( new Relics(player1, player2, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
            //                         relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
            
            //Corregido   // Efecto se aplica una vez, no en cada turno
            //player1.hand[0].Effect();

            // Putting activated card in battlefield
            //player1.hand[0].cardState = CardState.Activated;

            player1.TakeFromDeck(player1, player2, 5, new List<int>());
            player2.TakeFromDeck(player2, player1, 5, new List<int>());
            
            Console.Clear();
            while (player1.life != 0 && player2.life != 0)
            {
                Console.WriteLine("Turn: "+turn);
                

                if (turn % 2 != 0) //Impar
                {
                    //player1.TakeFromDeck(player1, player2, 1, new List<int>());
                    
                    player1.printInfo();

                    Console.WriteLine("Elige la carta que quieres activar");
                    ActiveEffect(player1, int.Parse(Console.ReadLine()));

                    UpdateBattleField(player1);
                    //All activity of player 1 goes here 
                    

                    // PRINTS LAST CARD ACTIVATED=============================================================================================================================
                    // foreach (var card in player1.hand)
                    // {
                    //     if (card.cardState == CardState.Activated)
                    //     {
                    //         Console.WriteLine("Last added card will be desactivated in: "+card.activeDuration);
                    //     }
                    // }


                    // Discounting activeDuration on each turn, eliminating card if activeDuration = 0 
                    

                    // Undonng effect of cards where activeDuration = 0



                    Console.ReadKey();
                    Console.Clear();
                }

                if (turn % 2 == 0) //Par
                {
                    player2.TakeFromDeck(player2, player1, 1, new List<int>());
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
        public static void UpdateBattleField(Player player)
        {
            for (int index = 0; index < player.hand.Count(); index++)
            {
                if (player.hand[index].cardState == CardState.Activated)
                {

                    if (player.hand[index].activeDuration == 1)
                    {
                        foreach (var effect in player.hand[index].EffectsOrder)
                        {
                            if(effect.Key == 5)
                            {
                                effect.Value.affects = effect.Value.affects*(-1); 
                                player.hand[index].Effect();
                                effect.Value.affects = effect.Value.affects*(-1);
                            }
                        }
                        player.hand[index].cardState = CardState.OnGraveyard; // Removing card from battelfield
                    }
                    else
                    {
                        if (player.hand[index].passiveDuration != 0)
                        {
                            player.hand[index].passiveDuration--;
                        }
                        else
                        {
                            int Defaultpassive = Program.CardsInventary[player.hand[index].id].passiveDuration;
                            player.hand[index].passiveDuration = Defaultpassive;
                            player.hand[index].activeDuration--;
                        }
                    }
                }
            }
        }
        public static void ActiveEffect(Player player, int HandPossition)
        {
            foreach (var card in player.hand)
            {
                if(card.cardState==CardState.OnHand)
                {
                    if(HandPossition==0)
                    {
                        card.Effect();
                        card.cardState = CardState.Activated;     
                        break;
                    }
                    HandPossition--;
                }
            }
            
        }
    }
}