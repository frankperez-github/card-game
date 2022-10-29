namespace card_gameProtot
{
    public class MainMenu
    {
        public static List<int> Deck = loadDeck();
        public static List<int> Characters = loadCharacters();
        public static List<int> Graveyard = new List<int>();


        public static List<int> loadDeck()
        {
            List<int> deck = new List<int>();
            foreach (var cardId in Program.CardsInventary)
            {
                deck.Add(cardId.Key);
            } 
            return deck;
        }     

        public static List<int> loadCharacters()
        {
            List<int> characters = new List<int>();
            foreach (var cardId in Program.CharactersInventary)
            {
                characters.Add(cardId.Key);
            } 
            return characters;
        }                                   
    }
}