namespace card_gameProtot
{
    public class Game
    {
        public static Dictionary<int, Relics> CardsInventary = Program.CardsInventary;
        public static Dictionary<int, Character> CharactersInventary = Program.CharactersInventary;
        public static List<int> GraveYard = new List<int>();
        public static void game()
        {
            List<int> Deck = CargarDeck(CardsInventary);

            Console.WriteLine("Elige el personaje que seras");
            Character character1 = CharactersInventary[int.Parse(Console.ReadLine())];
            Console.WriteLine("Elige el personaje que sera tu oponente");
            Character character2 = CharactersInventary[int.Parse(Console.ReadLine())];

            Player player1 = new Player(character1, "pepito");
            Player player2 = new Player(character2, "juancito");
            int turn = 1;

            player1.TakeFromDeck(player1, 5, new List<int>());
            player2.TakeFromDeck(player2, 5, new List<int>());

            while (player1.life != 0 && player2.life != 0)
            {
                if (turn % 2 != 0) //Impar
                {
                    player1.TakeFromDeck(player1, 1, new List<int>());
                    player1.PrintHand();
                    Console.ReadKey();
                }

                if (turn % 2 == 0) //Par
                {
                    player2.TakeFromDeck(player2, 1,new List<int>());

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