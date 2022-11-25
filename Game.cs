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
                if (turn % 2 != 0) //Impar
                {
                    PlayGame(player1);
                }

                if (turn % 2 == 0) //Par
                {
                    PlayGame(player2);
                }
                turn++;
            }
        }
        public void PlayGame(Player player)
        {
            if(player.state == State.Poisoned)
            {
                player.life -= 10;
            }
            
            player.TakeFromDeck(1);
            //All activity of player 1 goes here 
            int Option = 0;
            while(Option != 3)
            {
                Console.Clear();
                player.printInfo();
                if(player.state == State.Freezed)
                {
                    Console.WriteLine("Presione 1: Para atacar");
                    Console.WriteLine("Presione 3: Para Pasar turno");
                    try
                    {
                        Option = int.Parse(Console.ReadLine());
                    }
                    catch (System.Exception){}
                    switch (Option)
                    {
                        case 1: Attack(player);
                        break;
                        default:
                        break;
                    }
                }
                else if(player.state == State.Asleep)
                {
                    Console.WriteLine("Presione 1: Para activar cartas");
                    Console.WriteLine("Presione 3: Para Pasar turno");
                    try
                    {
                        Option = int.Parse(Console.ReadLine());
                    }
                    catch (System.Exception){}
                    switch (Option)
                    {
                        case 1: ActivateCards(player);
                        break;
                        default:
                        break;
                    }
                }
                else
                {
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
                        case 1: ActivateCards(player);
                        break;
                        case 2: Attack(player);
                        break;
                        default:
                        break;
                    }
                }
            }
            UpdateBattleField(player);
            Console.Clear();
        }
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
                            else if(Action.GetType().ToString() == "card_gameProtot.ChangeState")
                            {
                                player.Enemy.state = State.Safe;
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
        public void ActivateCards(Player player)
        {
            do
            {
                Console.Clear();
                player.printInfo();
                Console.WriteLine("Elige la carta que quieres activar");
                if(player.Enemy.HaveTrap())
                {

                }
                AddtoBattleField(player.hand[int.Parse(Console.ReadLine())]);
                player.hand[int.Parse(Console.ReadLine())].Effect();
                Console.WriteLine("Si quiere activar otra carta presione: 1, si no presione 2");
            } while (int.Parse(Console.ReadLine()) != 2);
        }
        public  void AddtoBattleField(Relics relic)
        {
            for (int i = 0; i < relic.Owner.userBattleField.Length; i++)
            {
                if(relic.Owner.userBattleField[i] == null)
                {
                    relic.Owner.userBattleField[i] = relic;
                    relic.Owner.hand.Remove(relic);
                    break;
                }
            }
        } 
        public static void ActivateTrapCards(Player enemy, double attack)
        {
            do
            {
                Console.Clear();
                enemy.printInfo();
                Console.WriteLine("Elige la carta que quieres activar");
                int HandPosition = int.Parse(Console.ReadLine());
                Relics relic = enemy.hand.ElementAt(HandPosition);
                relic.Effect();
                if(relic.Actions.Count() != 0)
                {
                    foreach (var Action in relic.Actions)
                    {
                        if(Action.GetType().ToString() == "card_gameProtot.StopAttack")
                        {
                            return;
                        }
                        if(Action.GetType().ToString() == "card_gameProtot.DamageReduction")
                        {
                            Action.Effect();
                            return;
                        }
                    }
                }
                Console.WriteLine("Si quiere activar otra carta presione: 1, si no presione 2");
            } while (int.Parse(Console.ReadLine()) != 2);
            enemy.life = enemy.life - enemy.Enemy.character.attack;
        }
        public static void Attack(Player player)
        {
            if(player.Enemy.character.defense!=0)
            {
                Console.WriteLine("Desea defenderse de este ataques? 1: Si, 2: No");
                if(int.Parse(Console.ReadLine())==1)
                {
                    player.Enemy.character.defense--;
                }
                else
                {
                    player.Enemy.life = player.Enemy.life - player.character.attack;
                }
            }
            else if(player.Enemy.HaveTrap())
            {
                Console.WriteLine("Desea activar una carta trampa? 1: Si, 2: No");
                if(int.Parse(Console.ReadLine())==1)
                {
                    ActivateTrapCards(player.Enemy, player.character.attack);
                }
                else
                {
                    player.Enemy.life = player.Enemy.life - player.character.attack;
                }
            }
        }
    }
}