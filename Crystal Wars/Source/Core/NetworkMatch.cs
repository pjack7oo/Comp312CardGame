using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using Crystal_Wars.Source.Objects;
using Crystal_Wars.Source.Field;
using System.Threading;

namespace Crystal_Wars.Source.Core
{
    class NetworkMatch : Match
    {
        public static OnlineState onlineState;
        private Thread serverThread;
        private Boolean hasDoneUpdate;
        private System.TimeSpan pressedTime;
        public enum OnlineState
        {
            Setup,
            Waiting,
            Playing
        }
        public NetworkMatch(Player self, RenderWindow window) : base(self, window)
        {
            SetupButtonsForConnection();
            onlineState = OnlineState.Setup;
        }

        private void StartMatch()
        {
            Console.WriteLine("server");
            MatchState = MatchState.Player;
            field.UpdateMatchStateText();
            serverThread = new Thread(new ThreadStart(() =>
            {

                Thread thread = new Thread(new ThreadStart(() =>
                {
                    var cards = new List<string>();
                    var deck = players[0].activeDeck.cards;
                    Thread.Sleep(5000);
                    Console.WriteLine("sent");
                    foreach (var card in deck)
                    {
                        Console.Write($"{card.ingameID} , ");

                        string cardStr = "";
                        if (card is EffectMonster effectMonster)
                        {
                            cardStr = JMessage.Serialize(JMessage.FromValue(effectMonster));
                        }
                        else if (card is MonsterCard monster)
                        {
                            cardStr = JMessage.Serialize(JMessage.FromValue(monster));
                        }
                        else if (card is SpellCard spell)
                        {
                            cardStr = JMessage.Serialize(JMessage.FromValue(spell));
                        }
                        cards.Add(cardStr);
                    }
                    var str = string.Join("','", cards);
                    str = "['" + str + "']";
                    Networking.SendData(str);
                }));
                thread.Start();
                Networking.Update();
            }));
            new Thread(new ThreadStart(() =>
            {
                Networking.Start();
                serverThread.Start();

            })).Start();



            SetupButtonsForWaitingServer();
            onlineState = OnlineState.Waiting;

        }

        private void ConnectToMatch()
        {
            MatchState = MatchState.Opponent;
            field.UpdateMatchStateText();
            serverThread = new Thread(new ThreadStart(() =>
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    var cards = new List<string>();
                    var deck = players[0].activeDeck.cards;
                    Thread.Sleep(5000);
                    Console.WriteLine("sent");
                    foreach (var card in deck)
                    {

                        Console.Write($"{card.ingameID} , ");

                        string cardStr = "";
                        if (card is EffectMonster effectMonster)
                        {
                            cardStr = JMessage.Serialize(JMessage.FromValue(effectMonster));
                        }
                        else if (card is MonsterCard monster)
                        {
                            cardStr = JMessage.Serialize(JMessage.FromValue(monster));
                        }
                        else if (card is SpellCard spell)
                        {
                            cardStr = JMessage.Serialize(JMessage.FromValue(spell));
                        }
                        cards.Add(cardStr);
                    }

                    var str = string.Join("','", cards);
                    str = "['" + str + "']";
                    //Console.WriteLine(str);
                    Networking.SendData(str);
                }));
                thread.Start();
                Networking.ClientUpdate();
            }));
            new Thread(new ThreadStart(() =>
            {
                Networking.Connect("127.0.0.1");
                serverThread.Start();

            })).Start();

            SetupButtonsForWaitingClient();
            onlineState = OnlineState.Waiting;


        }

        public override void AddButtons()
        {
            Game.InputHandler.ClearButtons();
            Game.InputHandler.AddButton(new Button("Next Phase", 20, new Vector2f(Game.ScreenWidth - 500, Game.ScreenHeight / 2), Color.Black, () =>
            {
                if (MatchState == MatchState.Player)
                {
                    NextTurnState();
                    Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.NextPhase))));
                }

            }
            ));
            Game.InputHandler.AddButton(new Button("Exit", 20, new Vector2f(Game.ScreenWidth - 250, Game.ScreenHeight / 2), Color.Black, Exit));
        }

        public override void Exit()
        {
            Game.InitiallizeMainPage();
            Networking.EndSeverConnection();
            Networking.EndClientConnection();

        }

        private void SetupButtonsForConnection()
        {
            Game.InputHandler.ClearButtons();
            Game.InputHandler.AddButton(new Button("Start Server", 15, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2), Color.Black, StartMatch, new Vector2f(1.25f, 1.25f)));
            Game.InputHandler.AddButton(new Button("Connect Server", 15, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 100), Color.Black, ConnectToMatch, new Vector2f(1.25f, 1.25f)));
            Game.InputHandler.AddButton(new Button("Exit", 40, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 200), Color.Black, () => { Game.InitiallizeMainPage(); Game.GameState = GameState.MainPage; }));
        }

        private void SetupButtonsForWaitingServer()
        {
            Game.InputHandler.ClearButtons();

            Game.InputHandler.AddButton(new Button("Exit", 40, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 200), Color.Black, () => { SetupButtonsForConnection(); onlineState = OnlineState.Setup; Networking.EndSeverConnection(); }));
        }
        private void SetupButtonsForWaitingClient()
        {
            Game.InputHandler.ClearButtons();

            Game.InputHandler.AddButton(new Button("Exit", 40, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 200), Color.Black, () => { SetupButtonsForConnection(); onlineState = OnlineState.Setup; Networking.EndClientConnection(); }));
        }


        public override void Update(System.TimeSpan time)
        {


            var duration = TimeSpan.FromSeconds(3);
            switch (onlineState)
            {
                case OnlineState.Setup:

                    break;
                case OnlineState.Waiting:

                    break;
                case OnlineState.Playing:
                    foreach (var player in players)//do player updates that arent based on turn state
                    {
                        player.Update();
                    }
                    switch (MatchState)//TODO 
                    {
                        case MatchState.Player:
                            switch (TurnState)
                            {
                                case TurnState.Drawing://draw a card and give crystals to player and update card mana

                                    if (!hasDoneUpdate)
                                    {
                                        pressedTime = Game.GetTimeStamp();
                                        players[(int)MatchState].DrawACardFromDeck();
                                        players[(int)MatchState].GetCrystals();
                                        field.GiveCardsMana(MatchState.Player);
                                        hasDoneUpdate = true;


                                    }
                                    else
                                    {
                                        if (time - pressedTime >= duration)
                                        {
                                            NextTurnState();
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.NextPhase))));
                                        }
                                    }
                                    break;
                                case TurnState.Primary://player can place down cards check if he has crystals, use spells
                                    if (players[(int)MatchState].Crystals == 0 && !field.PlayerFieldHasUsableSpellCard(PlayerType.Player) && !players[(int)MatchState].HasPlayableCard())
                                    {
                                        NextTurnState();
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.NextPhase))));
                                    }
                                    break;
                                case TurnState.Attack://player can attack, use spells
                                    if (!field.PlayerFieldHasUsableSpellCard(PlayerType.Player) && !players[(int)MatchState].HasPlayableSpellCard() && !field.PlayerFieldHasUsableMonster(PlayerType.Player))
                                    {
                                        NextTurnState();
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.NextPhase))));
                                    }
                                    break;
                                case TurnState.Secondary://player can use spells
                                    if (players[(int)MatchState].Crystals == 0 && !field.PlayerFieldHasUsableSpellCard(PlayerType.Player) && !players[(int)MatchState].HasPlayableSpellCard())
                                    {
                                        NextTurnState();
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.NextPhase))));
                                    }
                                    break;
                                case TurnState.End://end turn apply status effects/field effects possibly
                                                   //todo apply status effects
                                    if (!hasDoneUpdate)
                                    {
                                        pressedTime = Game.GetTimeStamp();
                                        hasDoneUpdate = true;


                                    }
                                    else
                                    {
                                        if (time - pressedTime >= duration)
                                        {
                                            NextTurnState();
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.NextPhase))));
                                        }
                                    }

                                    break;
                                default:
                                    break;
                            }
                            break;
                        case MatchState.Opponent://this is for regular match so we need AI code here so will need an AI class
                            switch (TurnState)
                            {
                                case TurnState.Drawing://draw a card and give crystals to player
                                    if (!hasDoneUpdate)
                                    {
                                        players[(int)MatchState].DrawACardFromDeck();
                                        players[(int)MatchState].GetCrystals();

                                        hasDoneUpdate = true;
                                    }

                                    break;
                                case TurnState.Primary://player can place down cards check if he has crystals, use spells


                                    players[1].Update();
                                    if (players[(int)MatchState].Crystals == 0 && !field.PlayerFieldHasUsableSpellCard(PlayerType.Player) && !players[(int)MatchState].HasPlayableCard())
                                    {
                                        //NextTurnState();
                                    }

                                    break;
                                case TurnState.Attack://player can attack, use spells
                                    if (!field.PlayerFieldHasUsableSpellCard(PlayerType.Enemy) && !players[(int)MatchState].HasPlayableSpellCard() && !field.PlayerFieldHasUsableMonster(PlayerType.Enemy))
                                    {
                                        // NextTurnState();
                                    }
                                    break;
                                case TurnState.Secondary://player can use spells
                                    if (players[(int)MatchState].Crystals == 0 && !field.PlayerFieldHasUsableSpellCard(PlayerType.Enemy) && !players[(int)MatchState].HasPlayableSpellCard())
                                    {
                                        //NextTurnState();
                                    }
                                    break;
                                case TurnState.End://end turn apply status effects/field effects possibly
                                                   //todo apply status effects
                                                   //NextTurnState();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void ExecuteAction(PlayerAction action)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                if (action.Type == PlayerAction.ActionType.NextPhase)
                {
                    NextTurnState();
                }
                else if (action.Type == PlayerAction.ActionType.Move)
                {
                    //Console.WriteLine(action);
                    var card = players[1].GetCard(action.Item);
                    Console.WriteLine($"{action.ItemType}, {action.Target}");
                    field.PlaceCardOnField(PlayerType.Enemy, action.ItemType, action.Target, card);
                    if (card is EffectMonster effectmonster)
                    {
                        //effectmonster.AddEffectButtons();
                        players[1].RemoveCrystals(effectmonster.CrystalCost);
                        effectmonster.State = CardState.Front;
                        effectmonster.Location = CardLocation.Field;
                        effectmonster.Selected = false;
                    }
                    else if (card is MonsterCard monster)
                    {
                        players[1].RemoveCrystals(monster.CrystalCost);
                        monster.State = CardState.Front;
                        monster.Location = CardLocation.Field;
                        monster.Selected = false;

                    }
                    else if (card is SpellCard spell)
                    {
                        //spell.AddEffectButtons();
                        players[1].RemoveCrystals(spell.CrystalCost);

                        spell.Location = CardLocation.Field;
                        spell.Selected = false;
                    }
                    players[1].AddCardToRemoveQueue(card);
                }
                else if (action.Type == PlayerAction.ActionType.Attack)
                {
                    
                    if (action.ItemType == null && action.Target == null)//attack opponent
                    {
                        AttackPlayerOnline(((int)action.Item), PlayerType.Player);
                    }
                    else
                    {
                        CalculateCardAttackOnline((int)action.Item, (int)action.Target, PlayerType.Player);
                    }
                }
                else if (action.Type == PlayerAction.ActionType.Effect)
                {

                }
                else if (action.Type == PlayerAction.ActionType.Retry)//might be used to resend data we shall see
                {
                    Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.Ready))));
                }
                else if (action.Type == PlayerAction.ActionType.Ready)
                {
                    if (Networking.CardsLoaded)
                    {
                        players[0].SetDeckPosition();
                        players[1].SetDeckPosition();
                        AddButtons();
                        onlineState = OnlineState.Playing;
                        DrawInitialCards();

                    }
                    else
                    {
                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(new PlayerAction(PlayerAction.ActionType.Retry))));
                    }

                }
            }));
            thread.Start();
        }

        public void AttackPlayerOnline(int cardId, PlayerType player)
        {
            if (player == PlayerType.Player)
            {
                FieldPosition position = field.GetMonsterCard(PlayerType.Enemy, cardId);
                if (((MonsterCard)position.Card).Mana - ((MonsterCard)position.Card).attackManaCost >= 0)
                {
                    Console.WriteLine($"{((MonsterCard)position.Card)} attacked Player");

                    players[0].ApplyDamage(((MonsterCard)position.Card).Attack);



                    ((MonsterCard)position.Card).Mana -= ((MonsterCard)position.Card).attackManaCost;

                }
            }
            else
            {
                FieldPosition position = field.GetMonsterCard(PlayerType.Player, cardId);
                if (((MonsterCard)position.Card).Mana - ((MonsterCard)position.Card).attackManaCost >= 0)
                {
                    Console.WriteLine($"{((MonsterCard)position.Card)} attacked Opponent");



                    players[1].ApplyDamage(((MonsterCard)position.Card).Attack);

                    ((MonsterCard)position.Card).Mana -= ((MonsterCard)position.Card).attackManaCost;

                }
            }
        }
        public void CalculateCardAttackOnline(int cardId, int targetId,PlayerType target)
        {
            MonsterCard card1;
            MonsterCard card2;
            if (target == PlayerType.Player)
            {
                card1 = (MonsterCard)field.GetMonsterCard(PlayerType.Enemy, cardId).Card;

                var targetField = field.GetMonsterCard(PlayerType.Player, targetId);
                card2 = (MonsterCard)targetField.Card;
                Console.WriteLine($"{card1} attacked {card2}");
                if (card1.Mana - card1.attackManaCost >= 0)
                {
                    if (card1.Attack > card2.Defense)//attack worked, send card2 to graveyard and deal difference of attack - defense to opponent
                    {
                        players[0].SendCardToGraveyard(card2);

                        players[0].ApplyDamage(card1.Attack - card2.Defense);

                        targetField.ResetCard();

                    }
                    else //attack failed you get dealt difference of defense -attack
                    {
                        players[1].ApplyDamage(card2.Defense - card1.Attack);
                    }
                    card1.Mana -= card1.attackManaCost;

                }
            }
            else
            {
                card1 = (MonsterCard)field.GetMonsterCard(PlayerType.Player, cardId).Card;
                var targetField = field.GetMonsterCard(PlayerType.Enemy, targetId);
                card2 = (MonsterCard)targetField.Card;
                Console.WriteLine($"{card1} attacked {card2}");
                if (card1.Mana - card1.attackManaCost >= 0)
                {
                    if (card1.Attack > card2.Defense)//attack worked, send card2 to graveyard and deal difference of attack - defense to opponent
                    {
                        players[1].SendCardToGraveyard(card2);

                        players[1].ApplyDamage(card1.Attack - card2.Defense);

                        targetField.ResetCard();

                    }
                    else //attack failed you get dealt difference of defense -attack
                    {
                        players[0].ApplyDamage(card2.Defense - card1.Attack);
                    }
                    card1.Mana -= card1.attackManaCost;

                }
            }
            
            
            
        }

        public override void Render()
        {
            switch (onlineState)
            {
                case OnlineState.Setup:
                    window.SetView(Game.defaultView);
                    window.Draw(Game.InputHandler);
                    break;
                case OnlineState.Waiting:
                    window.SetView(Game.defaultView);
                    window.Draw(Game.InputHandler);
                    break;
                case OnlineState.Playing:
                    base.Render();
                    break;
                default:
                    break;
            }
        }


        public override void MouseReleased(Vector2f mouse)
        {
            if (MatchState == MatchState.Player)
            {

                Tuple<PlayerType, FieldPosition> target = field.GetTarget(mouse);
                switch (TurnState)
                {
                    case TurnState.Primary:


                        if (selectedCard != null && target != null)//
                        {
                            //handle releasing on player field position this will include spell handling
                            if (target.Item1 == PlayerType.Player)
                            {
                                if (target.Item2 != null)
                                {
                                    if (!target.Item2.HasCard && selectedCard.Location == CardLocation.Moving)
                                    {
                                        if (target.Item2.fieldType == FieldType.Monster && selectedCard is MonsterCard)
                                        {
                                            if (selectedCard is EffectMonster card)
                                            {
                                                field.PlaceCardOnField(target.Item1, target.Item2, selectedCard);
                                                card.AddEffectButtons();
                                                players[(int)MatchState].RemoveCrystals(selectedCard.CrystalCost);
                                                target.Item2.Card = selectedCard;
                                                selectedCard.Location = CardLocation.Field;
                                                selectedCard.Selected = false;
                                                players[0].AddCardToRemoveQueue(selectedCard);
                                                var action = new PlayerAction(PlayerAction.ActionType.Move, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position);
                                                Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                                //cardsToRemove.Enqueue(selectedCard);
                                                //players[0].RemoveCard(selectedCard);
                                                selectedCard = null;
                                            }
                                            else
                                            {
                                                field.PlaceCardOnField(target.Item1, target.Item2, selectedCard);
                                                players[(int)MatchState].RemoveCrystals(selectedCard.CrystalCost);
                                                target.Item2.Card = selectedCard;
                                                selectedCard.Location = CardLocation.Field;
                                                selectedCard.Selected = false;
                                                players[0].AddCardToRemoveQueue(selectedCard);
                                                var action = new PlayerAction(PlayerAction.ActionType.Move, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position);
                                                Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                                //cardsToRemove.Enqueue(selectedCard);
                                                //players[0].RemoveCard(selectedCard);
                                                selectedCard = null;
                                            }

                                        }
                                        else if (target.Item2.fieldType == FieldType.Spell && selectedCard is SpellCard card)
                                        {
                                            field.PlaceCardOnField(target.Item1, target.Item2, selectedCard);
                                            card.AddEffectButtons();
                                            players[(int)MatchState].RemoveCrystals(selectedCard.CrystalCost);
                                            target.Item2.Card = selectedCard;
                                            selectedCard.Location = CardLocation.Field;
                                            selectedCard.Selected = false;
                                            players[0].AddCardToRemoveQueue(selectedCard);
                                            var action = new PlayerAction(PlayerAction.ActionType.Move, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                            //cardsToRemove.Enqueue(selectedCard);
                                            //players[0].RemoveCard(selectedCard);
                                            selectedCard = null;
                                        }
                                        else
                                        {
                                            ResetCard();
                                        }

                                    }
                                }
                                else
                                {
                                    ResetCard();
                                }


                            }
                            else //handle release on opponent field in necessary
                            {
                                ResetCard();
                            }
                        }
                        else if (selectedCard != null && target == null) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                        {
                            ResetCard();

                        }
                        else if (selectedCard != null && target.Item2 == null) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                        {
                            ResetCard();

                        }
                        break;
                    case TurnState.Attack:
                        //Tuple<PlayerType, FieldPosition> target = field.GetTarget(mouse);

                        if (selectedCard != null && target != null)//
                        {
                            //handle releasing on player field position this will include spell handling
                            if (target.Item1 == PlayerType.Player)
                            {
                                if (target.Item2 != null)
                                {
                                    if (!target.Item2.HasCard && selectedCard.Location == CardLocation.Moving)
                                    {

                                        if (target.Item2.fieldType == FieldType.Spell && selectedCard is SpellCard card)
                                        {
                                            field.PlaceCardOnField(target.Item1, target.Item2, selectedCard);
                                            card.AddEffectButtons();
                                            players[(int)MatchState].RemoveCrystals(selectedCard.CrystalCost);
                                            target.Item2.Card = selectedCard;
                                            selectedCard.Location = CardLocation.Field;
                                            selectedCard.Selected = false;
                                            players[0].AddCardToRemoveQueue(selectedCard);
                                            var action = new PlayerAction(PlayerAction.ActionType.Move, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));

                                            selectedCard = null;
                                        }
                                        else
                                        {
                                            ResetCard();
                                        }

                                    }
                                }
                                else
                                {
                                    ResetCard();
                                }


                            }
                            else
                            {
                                ResetCard();
                            }

                        }
                        else if (selectedCard != null && target == null) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                        {
                            ResetCard();

                        }
                        break;
                    case TurnState.Secondary:
                        if (selectedCard != null && target != null)//
                        {
                            //handle releasing on player field position this will include spell handling
                            if (target.Item1 == PlayerType.Player)
                            {
                                if (target != null)
                                {
                                    if (!target.Item2.HasCard && selectedCard.Location == CardLocation.Moving)
                                    {
                                        if (target.Item2.fieldType == FieldType.Spell && selectedCard is SpellCard card)
                                        {
                                            field.PlaceCardOnField(target.Item1, target.Item2, selectedCard);
                                            card.AddEffectButtons();
                                            players[(int)MatchState].RemoveCrystals(selectedCard.CrystalCost);
                                            target.Item2.Card = selectedCard;
                                            selectedCard.Location = CardLocation.Field;
                                            selectedCard.Selected = false;
                                            players[0].AddCardToRemoveQueue(selectedCard);
                                            var action = new PlayerAction(PlayerAction.ActionType.Move, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                            //cardsToRemove.Enqueue(selectedCard);
                                            //players[0].RemoveCard(selectedCard);
                                            selectedCard = null;
                                        }
                                        else
                                        {
                                            ResetCard();
                                        }

                                    }
                                }

                                else
                                {
                                    ResetCard();
                                }
                            }
                            else
                            {
                                ResetCard();
                            }

                        }
                        else if (selectedCard != null && target == null) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                        {
                            ResetCard();

                        }
                        break;
                    default:
                        if (selectedCard != null)
                        {
                            ResetCard();
                        }

                        break;
                }
            }

        }

        public override void NextTurnState()
        {
            base.NextTurnState();
            hasDoneUpdate = false;

        }


        public override void MouseClick(Vector2f mouse)
        {

            //check buttons
            var tempForCardZoom = field.SelectAnyCard(mouse);
            if (tempForCardZoom != null)
            {
                lastSelectedCard = tempForCardZoom;
            }
            if (MatchState == MatchState.Player)
            {
                switch (TurnState) //TODO: needs handler for each phase
                {
                    case TurnState.Drawing:
                        selectedCard = players[0].HandleMouseClick(mouse);
                        if (selectedCard != null)
                        {
                            lastSelectedCard = selectedCard;
                        }

                        break;
                    case TurnState.Primary:
                        var selectedCard2 = players[0].HandleMouseClickForOppenentTurn(mouse);
                        if (selectedCard2 != null)
                        {
                            lastSelectedCard = selectedCard2;
                        }
                        if (selectedEffect != null)//button will make the selected = the effect there will be cancel button or space bar
                        {

                            Tuple<PlayerType, FieldPosition> target;
                            target = field.GetTarget(mouse);
                            if (target != null)
                            {
                                if (selectedEffect.TargetPlayer == PlayerType.Player && target.Item1 == PlayerType.Player)
                                {
                                    if (selectedEffect.TargetCard == FieldType.Monster)
                                    {
                                        if (target.Item2.Card is MonsterCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                    else if (selectedEffect.TargetCard == FieldType.Spell) // target is player spell
                                    {
                                        if (target.Item2.Card is SpellCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }

                                    }
                                    else if (selectedEffect.TargetCard == null)//effect on self
                                    {
                                        selectedEffect.ActivateEffect(players[0]);
                                        var action = new PlayerAction(PlayerAction.ActionType.Effect, null, selectedCard.ingameID, -1, selectedEffect.id);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                    }

                                }
                                else if (selectedEffect.TargetPlayer == PlayerType.Enemy && target.Item1 == PlayerType.Enemy)//enemy
                                {
                                    if (selectedEffect.TargetCard == FieldType.Monster)//enemy monster
                                    {
                                        if (target.Item2.Card is MonsterCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                    else if (selectedEffect.TargetCard == FieldType.Spell) // target is enemy spell
                                    {
                                        if (target.Item2.Card is SpellCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    else if (selectedEffect.TargetCard == null)//effect on opponent
                                    {
                                        selectedEffect.ActivateEffect(players[1]);
                                        var action = new PlayerAction(PlayerAction.ActionType.Effect, null, selectedCard.ingameID, 0, selectedEffect.id);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                    }
                                }

                                selectedCard.Selected = false;
                                if (selectedCard is SpellCard card)
                                {
                                    card.DeactivateEffectButtons();
                                    if (!card.isFieldType)
                                    {
                                        players[0].SendCardToGraveyard(card);
                                        field.RemoveCard(card);
                                    }

                                }
                                else if (selectedCard is EffectMonster monster)
                                {
                                    monster.DeactivateEffectButtons();
                                    //monster.UseMana(selectedEffect);
                                    selectedEffect.DrainMana();
                                }
                                selectedEffect = null;




                                selectedCard = null;
                                ClearAlertText();
                                return;
                            }
                            else
                            {
                                //check cancel button
                            }

                        }
                        else
                        {

                            if (selectedCard != null && selectedCard is SpellCard card)
                            {

                                if (card.CheckButtonClick(mouse))
                                {

                                    return;
                                }

                            }

                            var temp = field.SelectPlayerSpellCard(mouse);
                            if (temp != null)
                            {
                                selectedCard = temp;


                                lastSelectedCard = selectedCard;
                                return;

                            }
                            selectedCard = players[0].HandleMouseClick(mouse);
                            if (selectedCard != null)
                            {
                                lastSelectedCard = selectedCard;
                                if (players[0].Crystals >= selectedCard.CrystalCost)
                                {
                                    selectedCard.Location = CardLocation.Moving;

                                }
                                else
                                {
                                    selectedCard = null;
                                }

                            }
                        }
                        break;
                    case TurnState.Attack:
                        selectedCard2 = players[0].HandleMouseClickForOppenentTurn(mouse);
                        if (selectedCard2 != null)
                        {
                            lastSelectedCard = selectedCard2;
                        }
                        if (selectedEffect != null)//button will make the selected = the effect there will be cancel button or space bar
                        {

                            Tuple<PlayerType, FieldPosition> target;
                            target = field.GetTarget(mouse);
                            if (target != null)
                            {
                                if (selectedEffect.TargetPlayer == PlayerType.Player && target.Item1 == PlayerType.Player)
                                {
                                    if (selectedEffect.TargetCard == FieldType.Monster)
                                    {
                                        if (target.Item2.Card is MonsterCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                    else if (selectedEffect.TargetCard == FieldType.Spell) // target is player spell
                                    {
                                        if (target.Item2.Card is SpellCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }

                                    }
                                    else if (selectedEffect.TargetCard == null)//effect on self
                                    {
                                        selectedEffect.ActivateEffect(players[0]);
                                        var action = new PlayerAction(PlayerAction.ActionType.Effect, null, selectedCard.ingameID, -1, selectedEffect.id);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                    }

                                }
                                else if (selectedEffect.TargetPlayer == PlayerType.Enemy && target.Item1 == PlayerType.Enemy)//enemy
                                {
                                    if (selectedEffect.TargetCard == FieldType.Monster)//enemy monster
                                    {
                                        if (target.Item2.Card is MonsterCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                    else if (selectedEffect.TargetCard == FieldType.Spell) // target is enemy spell
                                    {
                                        if (target.Item2.Card is SpellCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    else if (selectedEffect.TargetCard == null)//effect on opponent
                                    {
                                        selectedEffect.ActivateEffect(players[1]);
                                        var action = new PlayerAction(PlayerAction.ActionType.Effect, null, selectedCard.ingameID, 0, selectedEffect.id);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                    }
                                }

                                selectedCard.Selected = false;
                                if (selectedCard is SpellCard card)
                                {
                                    card.DeactivateEffectButtons();
                                    if (!card.isFieldType)
                                    {
                                        players[0].SendCardToGraveyard(card);
                                        field.RemoveCard(card);
                                    }

                                }
                                else if (selectedCard is EffectMonster monster)
                                {
                                    monster.DeactivateEffectButtons();
                                    //monster.UseMana(selectedEffect);
                                    selectedEffect.DrainMana();
                                }
                                selectedEffect = null;




                                selectedCard = null;
                                ClearAlertText();
                                return;
                            }
                            else
                            {
                                //check cancel button
                            }

                        }
                        else
                        {
                            var selectedCard3 = players[0].HandleMouseClick(mouse);
                            if (selectedCard3 != null)
                            {
                                lastSelectedCard = selectedCard3;
                                if (players[0].Crystals >= selectedCard3.CrystalCost && selectedCard3 is SpellCard)
                                {
                                    selectedCard = selectedCard3;
                                    selectedCard3.Location = CardLocation.Moving;
                                    return;
                                }
                                else
                                {
                                    selectedCard3 = null;
                                }

                            }
                            if (selectedCard != null && selectedCard is SpellCard card)
                            {

                                if (card.CheckButtonClick(mouse))
                                {

                                    return;
                                }

                            }
                            else if (selectedCard != null && selectedCard is EffectMonster monster)
                            {
                                //Console.WriteLine("check button");
                                if (monster.CheckButtonClick(mouse))
                                {

                                    return;
                                }

                            }
                            var temp = field.SelectPlayerCard(mouse);
                            if (temp != null)
                            {
                                selectedCard = temp;


                                lastSelectedCard = selectedCard;

                            }
                            else
                            {
                                if (selectedCard is MonsterCard monster)
                                {
                                    //Console.WriteLine(selectedCard is MonsterCard);
                                    Tuple<PlayerType, FieldPosition> target;
                                    target = field.GetTarget(mouse);
                                    if (target != null && target.Item1 == PlayerType.Enemy && selectedCard != null && target.Item2 == null)
                                    {
                                        AttackPlayer(monster, PlayerType.Enemy);
                                        var action = new PlayerAction(PlayerAction.ActionType.Attack, null, selectedCard.ingameID, null);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        selectedCard.Selected = false;
                                        selectedCard = null;
                                        ClearAlertText();
                                    }
                                    else if (target != null && target.Item1 == PlayerType.Enemy && selectedCard != null && target.Item2.fieldType == FieldType.Monster) //handle attacking card
                                    {
                                        var action = new PlayerAction(PlayerAction.ActionType.Attack, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.Card.ingameID);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        CalculateCardAttack(monster, target);
                                        
                                        selectedCard.Selected = false;
                                        selectedCard = null;
                                        ClearAlertText();
                                    }

                                }
                                else if (selectedCard is SpellCard)//spell card
                                {


                                }


                            }
                        }


                        break;
                    case TurnState.Secondary:

                        selectedCard2 = players[0].HandleMouseClickForOppenentTurn(mouse);
                        if (selectedCard2 != null)
                        {
                            lastSelectedCard = selectedCard2;
                        }
                        if (selectedEffect != null)//button will make the selected = the effect there will be cancel button or space bar
                        {

                            Tuple<PlayerType, FieldPosition> target;
                            target = field.GetTarget(mouse);
                            if (target != null)
                            {
                                if (selectedEffect.TargetPlayer == PlayerType.Player && target.Item1 == PlayerType.Player)
                                {
                                    if (selectedEffect.TargetCard == FieldType.Monster)
                                    {
                                        if (target.Item2.Card is MonsterCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                    else if (selectedEffect.TargetCard == FieldType.Spell) // target is player spell
                                    {
                                        if (target.Item2.Card is SpellCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }

                                    }
                                    else if (selectedEffect.TargetCard == null)//effect on self
                                    {
                                        selectedEffect.ActivateEffect(players[0]);
                                        var action = new PlayerAction(PlayerAction.ActionType.Effect, null, selectedCard.ingameID, -1, selectedEffect.id);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                    }

                                }
                                else if (selectedEffect.TargetPlayer == PlayerType.Enemy && target.Item1 == PlayerType.Enemy)//enemy
                                {
                                    if (selectedEffect.TargetCard == FieldType.Monster)//enemy monster
                                    {
                                        if (target.Item2.Card is MonsterCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Monster, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                    else if (selectedEffect.TargetCard == FieldType.Spell) // target is enemy spell
                                    {
                                        if (target.Item2.Card is SpellCard)
                                        {
                                            selectedEffect.ActivateEffect(target.Item2.Card);
                                            var action = new PlayerAction(PlayerAction.ActionType.Effect, PlayerAction.CardType.Spell, selectedCard.ingameID, target.Item2.position, selectedEffect.id);
                                            Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    else if (selectedEffect.TargetCard == null)//effect on opponent
                                    {
                                        selectedEffect.ActivateEffect(players[1]);
                                        var action = new PlayerAction(PlayerAction.ActionType.Effect, null, selectedCard.ingameID, 0, selectedEffect.id);
                                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(action)));
                                    }
                                }

                                selectedCard.Selected = false;
                                if (selectedCard is SpellCard card)
                                {
                                    card.DeactivateEffectButtons();
                                    if (!card.isFieldType)
                                    {
                                        players[0].SendCardToGraveyard(card);
                                        field.RemoveCard(card);
                                    }

                                }
                                else if (selectedCard is EffectMonster monster)
                                {
                                    monster.DeactivateEffectButtons();
                                    //monster.UseMana(selectedEffect);
                                    selectedEffect.DrainMana();
                                }
                                selectedEffect = null;


                                //players[0].SendCardToGraveyard(selectedCard);
                                //selectedEffect.DeactivateButtons();
                                //selectedEffect = null;
                                //selectedCard.Selected = false;
                                //field.RemoveCard(selectedCard);
                                //selectedCard = null;
                                //ClearAlertText();



                                selectedCard = null;
                                ClearAlertText();
                                return;
                            }
                            else
                            {
                                //check cancel button
                            }

                        }
                        else
                        {

                            if (selectedCard != null && selectedCard is SpellCard card)
                            {

                                if (card.CheckButtonClick(mouse))
                                {

                                    return;
                                }

                            }

                            var temp = field.SelectPlayerSpellCard(mouse);
                            if (temp != null)
                            {
                                selectedCard = temp;


                                lastSelectedCard = selectedCard;
                                return;

                            }
                            selectedCard = players[0].HandleMouseClick(mouse);
                            if (selectedCard != null)
                            {
                                lastSelectedCard = selectedCard;
                                if (players[0].Crystals >= selectedCard.CrystalCost && selectedCard is SpellCard)
                                {
                                    selectedCard.Location = CardLocation.Moving;
                                    return;
                                }
                                else
                                {
                                    selectedCard = null;
                                }

                            }
                        }
                        break;
                    case TurnState.End:
                        selectedCard = players[0].HandleMouseClick(mouse);
                        if (selectedCard != null)
                        {
                            lastSelectedCard = selectedCard;
                        }
                        break;
                    default:
                        break;

                }
            }
            else
            {
                selectedCard = players[0].HandleMouseClickForOppenentTurn(mouse);
                if (selectedCard != null)
                {
                    lastSelectedCard = selectedCard;
                }
            }
        }

    }
}
