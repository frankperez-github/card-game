namespace card_gameProtot
{
    public class Program
    {
        public static Dictionary<int, Relics> CardsInventary= new Dictionary<int, Relics>();
        public static Dictionary<int, Character> CharactersInventary = new Dictionary<int, Character>();
        public static List<int> GraveYard = new List<int>();
            
        public static void Main(string[] args)
        {
            //Characters
            CharactersInventary.Add(1, new Character("El dragón indiferente", 1, 0, "imgpath1", 10, 0));
            CharactersInventary.Add(2, new Character("El toro alado", 3, 0, "imgpath2", 0, 2));
            CharactersInventary.Add(3, new Character("La serpiente truhana", 1, 0, "imgpath3", 5, 0));
            CharactersInventary.Add(4, new Character("El tigre recursivo", 1, 0, "imgpath4", 8, 0));
            CharactersInventary.Add(5, new Character("El leon amistoso", 2, 0, "imgpath", 0, 1));




            Player player1 = new Player(CharactersInventary[1], "pepito");
            Player player2 = new Player(CharactersInventary[2], "juancito");

            //Espada del Destino
            //Te suma 15 de dano
            Dictionary<int, ActionInfo> card1Dict = new Dictionary<int, ActionInfo>();
            ActionInfo card1Info = new ActionInfo(player2, -15, new List<int>());
            card1Dict.Add(4, card1Info);
            CardsInventary.Add(1, new Relics(1,"Espada del destino", 1, 3, "img", false, new Condition(), card1Dict));

            //Capsula del Tiempo
            //Roba una carta del cementerio
            Dictionary<int, ActionInfo> card2Dict = new Dictionary<int, ActionInfo>();
            ActionInfo card2Info = new ActionInfo(player1, 1, new List<int>());
            card2Dict.Add(3, card1Info);
            CardsInventary.Add(2,new Relics(2, "Capsula del Tiempo", 1, 1, "imgpath2", false,new Condition(), card2Dict));

            //Anillo de Zeus
            //Ganas  de vida por cada carta en tu mano
            Dictionary<int, ActionInfo> card3Dict = new Dictionary<int, ActionInfo>();
            ActionInfo card3Info = new ActionInfo(player1, 5, player1.hand.Count());
            card3Dict.Add(4, card1Info);
            CardsInventary.Add(3 ,new Relics(3, "Anillo de Zeus", 1, 1, "imgpath3", false,  new Condition(), card3Dict));

            //Escudo de la pobreza
            //Trap, evita el 50% del dano del enemigo
            Dictionary<int, ActionInfo> card4Dict = new Dictionary<int, ActionInfo>();
            ActionInfo card4Info = new ActionInfo(player1, 1, 0.5);
            card4Dict.Add(4, card1Info);
            CardsInventary.Add(4,new Relics(4, "Escudo de la pobreza", 1, 1, "imgpath", true, new Condition(), card4Dict));

            //Libro de los secretos 
            //Robas 2 cartas del deck
            Dictionary<int, ActionInfo> card5Dict = new Dictionary<int, ActionInfo>();
            ActionInfo card5Info = new ActionInfo(2, new List<int>());
            card5Dict.Add(1, card1Info);
            CardsInventary.Add(5,new Relics(5, "Libro de los secretos", 1, 1, "imgpath4", false, new Condition(), card5Dict));
            
            //Caliz de la Venganza
            //Tu adversario descarta 2 cartas de su mano
            Dictionary<int, ActionInfo> card6Dict = new Dictionary<int, ActionInfo>();
            ActionInfo card6Info = new ActionInfo(player2, 2, new List<int>());
            card6Dict.Add(6, card1Info);
            CardsInventary.Add(6,new Relics(5, "Libro de los secretos", 1, 1, "imgpath4", false, new Condition(), card6Dict));



            Game.game();
        }
        public static void game(Character character1, Character character2)
        {

            
            
            




            
        }
            
    }
}