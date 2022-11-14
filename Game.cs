namespace card_gameProtot
{
    public class Game
    {
        public static List<Player> PlayersInventary = new List<Player>();
        public static Dictionary<int, Character> CharactersInventary = Program.CharactersInventary;
        public static List<Relics> GraveYard = new List<Relics>();
        // public static Player player1;
        // public static Player player2;
        public static void game()
        {
            Console.Clear();
            Console.WriteLine("Elige el personaje que seras");
            Character character1 = CharactersInventary[int.Parse(Console.ReadLine())];
            Console.WriteLine("Elige el personaje que sera tu oponente");
            Character character2 = CharactersInventary[int.Parse(Console.ReadLine())];


            Player player1 = new Player(character1, "Player1");
            Player player2 = new Player(character2, "Player2");
            PlayersInventary.Add(player1);
            PlayersInventary.Add(player2);

            int turn = 1;
            Dictionary<int, Relics> CardsInventary = Program.CardsInventary;
            // List<int> Deck = CargarDeck(CardsInventary);


            player1.TakeFromDeck(player1, player2, 5, new List<Relics>());
            player2.TakeFromDeck(player2, player1, 5, new List<Relics>());
            
            Console.Clear();
            while (player1.life > 0 && player2.life > 0)
            {
                Console.WriteLine("Turn: "+turn);
                

                if (turn % 2 != 0) //Impar
                {
                    player1.TakeFromDeck(player1, player2, 1, new List<Relics>());                    
                    
                    //All activity of player 1 goes here 
                    int Option = 0;
                    while(Option != 3)
                    {
                        Console.Clear();
                        player1.printInfo();
                        Console.WriteLine("Presione 1: Para activar cartas");
                        Console.WriteLine("Presione 2: Para atacar");
                        Console.WriteLine("Presione 3: Para Pasar turno");
                        try
                        {
                            Option = int.Parse(Console.ReadLine());
                        }
                        catch (System.Exception){}
                        switch (Option)
                        {
                            case 1: ActivateCards(player1);
                            break;
                            case 2: Attack(player1, player2);
                            break;
                            default:
                            break;
                        }
                    }
                    UpdateBattleField(player1);
                    Console.Clear();
                }

                if (turn % 2 == 0) //Par
                {
                    player2.TakeFromDeck(player2, player1, 1, new List<Relics>());
                    player2.printInfo();

                    //All activity of player 2 goes here 
                    int Option = 0;
                    while(Option != 3)
                    {
                        Console.Clear();
                        player2.printInfo();
                        Console.WriteLine("Presione 1: Para activar cartas");
                        Console.WriteLine("Presione 2: Para atacar");
                        Console.WriteLine("Presione 3: Para Pasar turno");
                        try
                        {
                            Option = int.Parse(Console.ReadLine());
                        }
                        catch (System.Exception){}
                        switch (Option)
                        {
                            case 1: ActivateCards(player2);
                            break;
                            case 2: Attack(player2, player1);
                            break;
                            default:
                            break;
                        }
                    }
                    UpdateBattleField(player2);
                    Console.Clear();
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
        //We need to change enemy player state 
        public static void UpdateBattleField(Player player)
        {
            for (int index = 0; index < player.userBattleField.Length; index++)
            {
                if(player.userBattleField[index] != null)
                {
                    if (player.userBattleField[index].activeDuration == 1)
                    {
                        foreach (var effect in player.userBattleField[index].EffectsOrder)
                        {
                            if(effect.Key == 5)
                            {
                                effect.Value.affects = effect.Value.affects*(-1); 
                                player.userBattleField[index].Effect();
                            }
                            else if(effect.Key == 8)
                            {
                                player.userBattleField[index].Affected.state = State.Safe;
                            }
                        }
                        GraveYard.Add(player.userBattleField[index]); 
                        player.userBattleField[index] = null; // Removing card from battelfield
                    }
                    else
                    {
                        if (player.userBattleField[index].passiveDuration != 0)
                        {
                            player.userBattleField[index].passiveDuration--;
                        }
                        else
                        {
                            int Defaultpassive = Program.CardsInventary[player.userBattleField[index].id].passiveDuration;
                            player.userBattleField[index].passiveDuration = Defaultpassive;
                            player.userBattleField[index].activeDuration--;
                        }
                    }   
                }
            }
        }
        public static void ActivateCards(Player player)
        {
            do
            {
                Console.Clear();
                player.printInfo();
                Console.WriteLine("Elige la carta que quieres activar");
                ActivateEffect(player, int.Parse(Console.ReadLine()));
                Console.WriteLine("Si quiere activar otra carta presione: 1, si no presione 2");
            } while (int.Parse(Console.ReadLine()) != 2);
        }
        public static void ActivateTrapCards(Player player, double attack)
        {
            do
            {
                Console.Clear();
                player.printInfo();
                Console.WriteLine("Elige la carta que quieres activar");
                int HandPosition = int.Parse(Console.ReadLine());
                foreach (var effect in player.hand[HandPosition].EffectsOrder)
                {
                    if (effect.Key == 4)
                    {
                        effect.Value.affects = attack*-1;
                    }
                }
                player.hand[HandPosition].Effect();
                player.userBattleField.ToList().Add(player.hand[HandPosition]);
                player.hand[HandPosition].cardState = CardState.Activated;
                player.hand.Remove(player.hand[HandPosition]);                            
                Console.WriteLine("Si quiere activar otra carta presione: 1, si no presione 2");
            } while (int.Parse(Console.ReadLine()) != 2);
            
            
        }
        public static void ActivateEffect(Player player, int HandPossition)
        {
            player.hand[HandPossition].Effect();    
        }
        public static void Attack(Player player, Player enemy)
        {
            if(enemy.defense!=0)
            {
                Console.WriteLine("Desea defenderse de este ataques? 1: Si, 2: No");
                if(int.Parse(Console.ReadLine())==1)
                {
                    enemy.defense--;
                }
                else
                {
                    enemy.life = enemy.life - player.attack;
                }
            }
            else if(enemy.Trap())
            {
                Console.WriteLine("Desea activar una carta trampa? 1: Si, 2: No");
                if(int.Parse(Console.ReadLine())==1)
                {
                    ActivateTrapCards(enemy, player.attack);
                }
                else
                {
                    enemy.life = enemy.life - player.attack;
                }
            }
            
            
        }
    }
}