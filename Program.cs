namespace card_gameProtot
{
    public class Program
    {
        public static List<Relics> CardsInventary= new List<Relics>();
        public static Dictionary<int, Character> CharactersInventary = new Dictionary<int, Character>();
        public static void Main(string[] args)
        {
            //Characters
            CharactersInventary.Add(1, new Character("El dragón indiferente", 1, 0, "imgpath1", 10, 3));
            CharactersInventary.Add(2, new Character("El toro alado", 3, 0, "imgpath2", 0, 5));
            CharactersInventary.Add(3, new Character("La serpiente truhana", 1, 0, "imgpath3", 5, 0));
            CharactersInventary.Add(4, new Character("El tigre recursivo", 1, 0, "imgpath4", 8, 0));
            CharactersInventary.Add(5, new Character("El leon amistoso", 2, 0, "imgpath", 0, 1));



            Player defaultPlayer = new Player(CharactersInventary[1], "default");

            //Espada del Destino
            //Te suma 15 de ataque
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 1, "Espada del destino", 0, 3, "img", false, "", "damage", "(Owner.Attack.15)"));

            //Capsula del Tiempo
            //Roba una carta del cementerio
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 2, "Capsula del Tiempo", 0, 1, "imgpath2", false, "", "draw", "(Owner.Draw.EnemyHand.1)"));

            //Anillo de Zeus
            //Ganas 5 de vida por cada carta en tu mano
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 3, "Anillo de Zeus", 0, 1, "imgpath3", false, "",  "cure", "(Owner.Cure.5.OwnerHand)"));

            //Escudo de la pobreza
            //Trap, evita el 50% del daño del enemigo
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 4, "Escudo de la pobreza", 0, 1, "imgpath", true, "", "defense", "(Owner.Defense.1.0,5)"));

            //Libro de los secretos 
            //Robas 2 cartas del deck
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 5, "Libro de los secretos", 0, 1, "imgpath4", false, "", "draw", "(Owner.Draw.Deck.random.2)"));
            
            // //Caliz de la Venganza
            // //Tu adversario descarta 2 cartas de su mano
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 6, "Caliz de la Venganza", 0, 1, "imgpath4", false, "", "draw", "(Enemy.Remove.EnemyHand.2)"));

            //Resfriado
            //El adversario queda congelado por 2 turnos
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 7, "Resfriado", 1, 2, "imgpath4", false, "", "state", "(Enemy.ChangeState.Freezed)"));

            // //Objetivo enemigo
            // //Destruye 1 reliquia que tenga activa enemigo
            CardsInventary.Add(new Relics(defaultPlayer, defaultPlayer, 8, "Objetivo Enemigo", 0, 1, "imgpath4", false, "", "trap", "(Enemy.Remove.EnemyBattlefield.Battlefield.random.1)"));
            
            Game.game();
        }
            
    }
}