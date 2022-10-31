namespace card_gameProtot
{
    public class Game
    {
        public static List<Player> PlayersInventary = new List<Player>();
        public static Dictionary<int, Character> CharactersInventary = Program.CharactersInventary;
        public static List<int> GraveYard = new List<int>();
        public static void game()
        {

            Console.WriteLine("Elige el personaje que seras");
            Character character1 = CharactersInventary[int.Parse(Console.ReadLine())];
            Console.WriteLine("Elige el personaje que sera tu oponente");
            Character character2 = CharactersInventary[int.Parse(Console.ReadLine())];
            



            Player player1 = new Player(character1, "pepito");
            Player player2 = new Player(character2, "juancito");
            PlayersInventary.Add(player1);
            PlayersInventary.Add(player2);

            int turn = 1;
            Dictionary<int, Relics> CardsInventary = Program.CardsInventary;
            List<int> Deck = CargarDeck(CardsInventary);


            player1.TakeFromDeck(player1, player2, relativePlayer.Owner, 5, new List<int>());
            player2.TakeFromDeck(player2, player1, relativePlayer.Owner, 5, new List<int>());

            while (player1.life != 0 && player2.life != 0)
            {
                if (turn % 2 != 0) //Impar
                {
                    player1.TakeFromDeck(player1, player2, relativePlayer.Owner, 1, new List<int>());
                    Relics relic = Program.CardsInventary[3];
                    player1.hand.Add( new Relics(player1, player2, relic.id, relic.name, relic.passiveDuration, relic.activeDuration, 
                                    relic.imgAddress,relic.isTrap, relic.Condition, relic.EffectsOrder));
                    player1.PrintHand();

                    // Console.WriteLine((player1.hand[player1.hand.Count()-1].EffectsOrder.ElementAt(0).Key));
                    
                    Console.ReadKey();
                    player1.hand[player1.hand.Count()-1].Effect(turn);
                    Console.WriteLine("player1 defense: "+player1.defense);
                    Console.WriteLine("player1 attack: "+player1.attack);
                    Console.WriteLine("player1 life: "+player1.life);
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