namespace card_gameProtot
{
    public class Game
    {
        Player player1{get;}
        Player player2{get;}
        public List<Character> CharactersInventary;
        public List<Relics> CardsInventary;
        public List<Relics> GraveYard;
        int turn;
        public Game(List<Character> CharactersInventary, List<Relics> CardsInventary)
        {
            this.CharactersInventary = CharactersInventary;
            this.CardsInventary = CardsInventary;
            GraveYard = new List<Relics>();
            this.player1 = new Player(CharactersInventary[0], "Player1");
            this.player2 = new Player(CharactersInventary[0], "Player2");
            player1.SetAux(player2, this);
            player2.SetAux(player1, this);
            turn = 1;
        }

        public void game()
        {
            Console.Clear();
            Console.WriteLine("Elige el personaje que seras");
            player1.character = CharactersInventary[int.Parse(Console.ReadLine())];
            Console.WriteLine("Elige el personaje que sera tu oponente");
            player2.character = CharactersInventary[int.Parse(Console.ReadLine())];

            player1.TakeFromDeck(5);
            player2.TakeFromDeck(5);
            
            Console.Clear();
            while (player1.life > 0 && player2.life > 0)
            {
                Console.WriteLine("Turn: "+turn);
                

                if (turn % 2 != 0) //Impar
                {
                    player1.TakeFromDeck(1);                    
                    
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
                    player2.TakeFromDeck(1);
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
        //We need to change enemy player state
        public void UpdateBattleField(Player player)
        {
            for (int index = 0; index < player.userBattleField.Length; index++)
            {
                if(player.userBattleField[index] != null)
                {
                    if (player.userBattleField[index].activeDuration == 1)
                    {
                        foreach (var Action in player.userBattleField[index].Actions)
                        {
                            if(Action.GetType().ToString() == "card_gameProtot.Attack")
                            {
                                Action.Effect();
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
                            int Defaultpassive = CardsInventary[player.userBattleField[index].id].passiveDuration;
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
                // foreach (var effect in player.hand[HandPosition].EffectsOrder)
                // {
                //     if (effect.Key == 4)
                //     {
                //         effect.Value.affects = attack*-1;
                //     }
                // }
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
            if(enemy.character.defense!=0)
            {
                Console.WriteLine("Desea defenderse de este ataques? 1: Si, 2: No");
                if(int.Parse(Console.ReadLine())==1)
                {
                    enemy.character.defense--;
                }
                else
                {
                    enemy.life = enemy.life - player.character.attack;
                }
            }
            else if(enemy.Trap())
            {
                Console.WriteLine("Desea activar una carta trampa? 1: Si, 2: No");
                if(int.Parse(Console.ReadLine())==1)
                {
                    ActivateTrapCards(enemy, player.character.attack);
                }
                else
                {
                    enemy.life = enemy.life - player.character.attack;
                }
            }
            
            
        }
    }
}