﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using Crystal_Wars.Source.Objects;
using Crystal_Wars.Source.Core;

namespace Crystal_Wars.Source.Field
{
    class PlayerField
    {
        readonly Random random = new Random();
        FieldPosition[] playerMonsterField;
        FieldPosition[] playerSpellField;

        PlayerType playerType;
        
        public PlayerField(PlayerType playerType)
        {
            playerMonsterField = new FieldPosition[5];
            playerSpellField = new FieldPosition[5];
            this.playerType = playerType;
            if (playerType == PlayerType.Player)
            {
                int count = 1;
                for (int i = 0; i < 5; i++)
                {
                    playerMonsterField[i] = new FieldPosition(playerType,count, FieldType.Monster);
                    count++;
                }
                count = 1;
                for (int i = 0; i < 5; i++)
                {
                    playerSpellField[i] = new FieldPosition(playerType, count, FieldType.Spell);
                    count++;
                }
            }
            else
            {
                int count = 1;
                for (int i = 0; i < 5; i++)
                {
                    playerMonsterField[i] = new FieldPosition(playerType, count, FieldType.Monster);
                    count++;
                }
                count = 1;
                for (int i = 0; i < 5; i++)
                {
                    playerSpellField[i] = new FieldPosition(playerType, count, FieldType.Spell);
                    count++;
                }
            }
            
        }

        public FieldPosition GetRandomUnusedMonsterFieldPosition()
        {
            var randomInt = random.Next(0, playerMonsterField.Length);
            if (!playerMonsterField[randomInt].HasCard)
            {
                return playerMonsterField[randomInt];
            }
            else
            {

                foreach (var fieldPos in playerMonsterField)
                {
                    if (!fieldPos.HasCard)
                    {
                        return fieldPos;
                    }

                }
                return null;
            }
        }


        public FieldPosition GetRandomUnusedSpellFieldPosition()
        {
            var randomInt = random.Next(0, playerSpellField.Length);
            if (!playerSpellField[randomInt].HasCard)
            {
                return playerSpellField[randomInt];
            }
            else
            {

                foreach (var fieldPos in playerSpellField)
                {
                    if (!fieldPos.HasCard)
                    {
                        return fieldPos;
                    }

                }
                return null;
            }
        }
        //remove the red outline of any selected card
        public void ResetCardSelection()
        {
            foreach (var fieldPos in playerMonsterField)//make not selected cards inactive
            {
                if (fieldPos.HasCard)
                {
                    fieldPos.Card.Selected = false;
                    if (fieldPos.Card is EffectMonster card)
                    {
                        card.DeactivateEffectButtons();
                    }
                }
            }
            foreach (var fieldPos in playerSpellField)//make not selected cards inactive
            {
                if (fieldPos.HasCard)
                {
                    //((SpellCard)fieldPos.Card).DrawEffectButtons = false;
                    fieldPos.Card.State = CardState.Back;
                    fieldPos.Card.Selected = false;
                    ((SpellCard)fieldPos.Card).DeactivateEffectButtons();
                    Match.ClearAlertText();
                }
            }
        }

        public void RemoveCard(Card card)
        {
            foreach(var pos in playerMonsterField)
            {
                if (pos.HasCard)
                {
                    if (pos.Card.Equals(card))
                    {
                        pos.ResetCard();
                    }
                    
                }
            }
            foreach (var pos in playerSpellField)
            {
                if (pos.HasCard)
                {
                    if (pos.Card.Equals(card))
                    {
                        pos.ResetCard();
                    }
                    
                }
            }
        }

        public void GiveCardsMana()
        {
            foreach(var fieldPos in playerMonsterField)
            {
                if (fieldPos.HasCard)
                {
                    ((MonsterCard)fieldPos.Card).GiveMana();
                }
            }
        }

        public Card SelectSpellCard(Vector2f mouse)
        {
            Card result = null;
            foreach (var fieldPos in playerMonsterField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse))
                {
                    //fieldPos.Card.Selected = true;
                    //if (fieldPos.Card is EffectMonster card)
                    //{
                        //card.ActivateEffectButtons();
                        //Match.AlertText.DisplayedString = "Select A Effect On The Card Or Attack";
                    //}
                    result = fieldPos.Card;
                }

            }
            foreach (var fieldPos in playerSpellField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse))
                {
                    fieldPos.Card.Selected = true;
                    fieldPos.Card.State = CardState.Front;
                    ((SpellCard)fieldPos.Card).ActivateEffectButtons();
                    //((SpellCard)fieldPos.Card).DrawEffectButtons = true;
                    Match.AlertText.DisplayedString = "Select A Effect On The Card";
                    result = fieldPos.Card;
                }

            }
            if (result != null)
            {
                foreach (var fieldPos in playerSpellField)//make not selected cards inactive
                {
                    if (fieldPos.HasCard)
                    {
                        if (!result.Equals(fieldPos.Card) && fieldPos.Card.Selected)
                        {
                            //((SpellCard)fieldPos.Card).DrawEffectButtons = false;
                            fieldPos.Card.State = CardState.Back;
                            ((SpellCard)fieldPos.Card).DeactivateEffectButtons();

                            fieldPos.Card.Selected = false;
                        }
                    }

                }
            }

            if (result is MonsterCard)
            {
                result = null;
            }
            if (result is null)
            {
                Match.ClearAlertText();
            }
            return result;
            }

        public bool HasMonstersOnTheField()
        {
            for (int i = 0; i < playerMonsterField.Length;i++)
            {
                if (playerMonsterField[i].HasCard)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PlaceCardOnField(PlayerAction.CardType? cardType, int? fieldPosition, Card card)
        {
            if (cardType == PlayerAction.CardType.Monster)
            {
                foreach (var fieldPos in playerMonsterField)//go through field and select the card that contains mouse
                {
                    Console.WriteLine($"{fieldPos.position}, {fieldPosition}");
                    if (!fieldPos.HasCard && fieldPos.position == fieldPosition)
                    {
                        fieldPos.Card = card;
                        card.UpdatePositions();
                        return true;
                    }

                }
            }
            else
            {
                foreach (var fieldPos in playerSpellField)//go through field and select the card that contains mouse
                {
                    if (!fieldPos.HasCard && fieldPos.position == fieldPosition)
                    {
                        fieldPos.Card = card;
                        card.UpdatePositions();
                        return true;
                    }

                }
            }
            return false;
        }

        public bool HasUsableSpellCard()
        {
            foreach (var fieldPos in playerSpellField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard)
                {
                    return true;
                }

            }
            return false;
        }
        public bool HasUsableMonster()
        {
            foreach (var fieldPos in playerMonsterField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && ((MonsterCard)fieldPos.Card).Mana > 0)
                {
                    return true;
                }

            }
            return false;
        }

        public Card SelectCard(Vector2f mouse)
        {
            Card result = null;
            foreach (var fieldPos in playerMonsterField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse) && ((MonsterCard)fieldPos.Card).Mana>0)
                {
                    fieldPos.Card.Selected = true;
                    if (fieldPos.Card is EffectMonster card)
                    {
                        card.ActivateEffectButtons();
                        Match.AlertText.DisplayedString = "Select A Effect On The Card Or Attack";
                    }
                    else
                    {
                        Match.AlertText.DisplayedString = "Attack";
                    }
                    result = fieldPos.Card;
                }
                else if (fieldPos.HasCard && fieldPos.Contains(mouse) && ((MonsterCard)fieldPos.Card).Mana == 0)
                {
                    foreach (var fieldPos2 in playerMonsterField)//make not selected cards inactive
                    {
                        if (result != fieldPos2.Card && fieldPos2.HasCard)
                        {
                            fieldPos2.Card.Selected = false;
                            if (fieldPos2.Card is EffectMonster card)
                            {
                                card.DeactivateEffectButtons();
                                
                            }
                            Match.ClearAlertText();
                        }
                    }
                }

            }
            
            foreach (var fieldPos in playerSpellField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse))
                {
                    fieldPos.Card.Selected = true;
                    fieldPos.Card.State = CardState.Front;
                    ((SpellCard)fieldPos.Card).ActivateEffectButtons();
                    //((SpellCard)fieldPos.Card).DrawEffectButtons = true;
                    Match.AlertText.DisplayedString = "Select A Effect On The Card";
                    result = fieldPos.Card;
                }

            }
            if (result != null)
            {
                foreach (var fieldPos in playerSpellField)//make not selected cards inactive
                {
                    if (fieldPos.HasCard)
                    {
                        if (!result.Equals(fieldPos.Card)  && fieldPos.Card.Selected)
                        {
                            //((SpellCard)fieldPos.Card).DrawEffectButtons = false;
                            fieldPos.Card.State = CardState.Back;
                            ((SpellCard)fieldPos.Card).DeactivateEffectButtons();
                            
                            fieldPos.Card.Selected = false;
                        }
                    }
                    
                }
                foreach (var fieldPos in playerMonsterField)//make not selected cards inactive
                {
                    if (result != fieldPos.Card && fieldPos.HasCard)
                    {
                        fieldPos.Card.Selected = false;
                        if (fieldPos.Card is EffectMonster card)
                        {
                            card.DeactivateEffectButtons();
                            
                        }
                    }
                }
            }
            return result;
        }

        public Card SelectCardWithoutActivation(Vector2f mouse)
        {
            Card result = null;
            foreach (var fieldPos in playerMonsterField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse))
                {
                    
                    
                    result = fieldPos.Card;
                }

            }
            foreach (var fieldPos in playerSpellField)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse) && (fieldPos.Card.State == CardState.Front || playerType == PlayerType.Player))
                {


                    result = fieldPos.Card;
                }

            }

            return result;
        }

        public FieldPosition GetMonsterCard(int id)
        {
            for(int i = 0;i <playerMonsterField.Length;i++)
            {
                if (playerMonsterField[i].HasCard)
                {
                    if (playerMonsterField[i].Card is MonsterCard)
                    {
                        return playerMonsterField[i];
                    }
                }
            }
            return null;
        }

        //drawing the field positions 
        public void Draw(RenderWindow window)
        {
            
            for (int i = 0; i < playerMonsterField.Count(); i++)
            {
                window.Draw(playerMonsterField[i]);
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
            }
            for (int i = 0; i < playerSpellField.Count(); i++)
            {
                window.Draw(playerSpellField[i]);
                //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
            }
            //target.Draw(CardOutlineRectangle(1400,600));
        }

        public Tuple<PlayerType, FieldPosition> GetTarget(Vector2f mouse)
        {

            for (int i = 0; i < playerMonsterField.Count(); i++)
            {
                if (playerMonsterField[i].Contains(mouse))
                {
                    return new Tuple<PlayerType, FieldPosition>(playerType, playerMonsterField[i]);
                }
            }
            for (int i = 0; i < playerSpellField.Count(); i++)
            {
                if (playerSpellField[i].Contains(mouse))
                {
                    return new Tuple<PlayerType, FieldPosition>(playerType, playerSpellField[i]);
                }
            }
            //for (int i = 0; i < player2Field.Count(); i++)//check if we clicked on opponent field
            //{
            //    if (Match.TurnState == TurnState.Attack)//handle click on opponent in attack phase
            //    {
            //        if (player2Field[i].Contains(mouse) && player2Field[i].HasCard)
            //        {
            //            return new Tuple<PlayerType, FieldPosition>(PlayerType.Enemy, player2Field[i]);
            //        }
            //    }

            //}
            return null;
        }
    }
}
