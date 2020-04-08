﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;


namespace Game
{

    public class PlayerCharacter
    {
        public Control Player;
        public bool goRight { get; set; }
        public bool goLeft { get; set; }
        public bool goUp { get; set; }
        public bool goDown { get; set; }
        public bool action { get; set; }

        static public int playerSpeed = 5;

        public PlayerItemInventory Items = new PlayerItemInventory();

        public enum Directions { Left, Right, Up, Down }

        public PlayerCharacter(Control player)
        {
            Player = player;
        }

        public void MovePlayer()
        {

            if (goRight)
            {
                Player.Parent.Left -= playerSpeed;
                Player.Left += playerSpeed;

            }
            if (goLeft)
            {
                Player.Parent.Left += playerSpeed;
                Player.Left -= playerSpeed;
            }
            if (goUp)
            {
                Player.Parent.Top += playerSpeed;
                Player.Top -= playerSpeed;
            }
            if (goDown)
            {
                Player.Parent.Top -= playerSpeed;
                Player.Top += playerSpeed;
            }

        }

        public void playerCollision(string tag)
        {
            //wykrywa kolizje gracza z pictureboxami z tagiem przekazanym jako argument funkcji
            foreach (Control x in Player.Parent.Controls)
            {
                if (x is PictureBox && x.Tag == tag)
                {
                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        if (Player.Right > x.Left && Player.Right < x.Left + (playerSpeed + 1) && Player.Left < x.Left)
                        {
                            goRight = false;
                        }
                        if (Player.Left < x.Right && Player.Left > x.Right - (playerSpeed + 1) && Player.Right > x.Right)
                        {
                            goLeft = false;
                        }
                        if (Player.Bottom >= x.Top && Player.Bottom < x.Top + (playerSpeed + 1) && Player.Top < x.Top)
                        {
                            goDown = false;
                        }
                        if (Player.Top <= x.Bottom && Player.Top > x.Bottom - (playerSpeed + 1) && Player.Bottom > x.Bottom)
                        {
                            goUp = false;
                        }
                    }
                }
            }
        }

        public void pushMovableObjects()
        {
            foreach (Control x in Player.Parent.Controls)
            {
                if (x is PictureBox && x.Tag == "movable_object")
                {
                    if (action) //gdy gracz ma wcisnieta spacje i podejdzie do movable_object, to moze go przesuwac na wszystkie strony, rowniez ciagnac do siebie (potem mozna dodac warunek znajomosci jakiegos zaklecia)
                    {
                        if (Player.Bounds.IntersectsWith(x.Bounds))
                        {
                            playerSpeed = 3;
                            if (goRight)
                            {
                                x.Left += playerSpeed;
                            }
                            if (goLeft)
                            {
                                x.Left -= playerSpeed;
                            }
                            if (goDown)
                            {
                                x.Top += playerSpeed;
                            }
                            if (goUp)
                            {
                                x.Top -= playerSpeed;
                            }
                        }
                    }
                    else
                    {
                        playerSpeed = 5;
                        playerCollision("movable_object");
                    }
                }
            }
        }

        //wykrywa kolizje ruchomych obiektow z innymi obiektami
        public void MovableObjectsCollision()
        {
            //poniewaz ruch ruchomych obiektow jest zalezny tylko od ruchu gracza, podczas kolizji blokujemy ruch gracza w danym kierunku 
            foreach (Control mv in Player.Parent.Controls) //mv = movable, objekt ktory sie rusza i ma sie zatrzymac 
            {
                if (mv is PictureBox && mv.Tag == "movable_object")
                {
                    foreach (Control st in Player.Parent.Controls) //st = static, po kolizji mv z tym obiektem zatrzymywany jest ruch
                    {
                        if (st is PictureBox)
                        {
                            if (st.Tag == "wall" || st.Tag == "door_closed" || st.Tag == "movable_object") 
                            {
                                if (mv.Bounds.IntersectsWith(st.Bounds)&& Player.Bounds.IntersectsWith(mv.Bounds))
                                {
                                    if (mv.Right > st.Left && mv.Left < st.Left)
                                    {
                                        goRight = false;
                                    }
                                    if (mv.Left < st.Right && mv.Right > st.Right)
                                    {
                                        goLeft = false;
                                    }
                                    if (mv.Bottom >= st.Top && mv.Top < st.Top)
                                    {
                                        goDown = false;
                                    }
                                    if (mv.Top <= st.Bottom && mv.Bottom > st.Bottom)
                                    {
                                        goUp = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DoorsInteraction()
        {
            foreach (PictureBox x in Player.Parent.Controls)
            {
                if (x is PictureBox && x.Tag == "door_closed")
                {
                    //jesli gracz dotyka drzwi i nacisnie spacje, to drzwi zmieniaja tag i kolor
                    if (Player.Bounds.IntersectsWith(x.Bounds) && action)
                    {
                        x.Tag = "door_open";
                        x.BackColor = Color.Green;
                    }
                }
            }
        }



    }


    public class PlayerItemInventory
    {

        private List<Item> _items = new List<Item>();

        /// <summary>
        /// Inserts an item into the players inventory, but if the item already exists - it will update the existing item with the new amount
        /// </summary>
        /// <param name="itemName">The item name to give the player</param>
        /// <param name="amount">The amount of the item to give the player</param>
        public void InsertItem(string itemName, int amount = 1)
        {
            var foundItem = FindItem(itemName);
            if (foundItem == null)
            {
                // insert a new item into the _items list, as it does not exist already
                _items.Add(new Item(itemName, amount));
            }
            else
            {
                // Update the existing item
                foundItem.amount += amount;
            }
        }

        public void RemoveItem(string itemName, int amount = 1)
        {
            //todo: Do stuff here plox
            var foundItem = FindItem(itemName);
            if (foundItem != null)
            {
                _items.Remove(foundItem);
            }
        }

        /// <summary>
        /// Returns the item if found by item name, otherwise returns null
        /// </summary>
        /// <param name="itemName">The item name to find</param>
        /// <returns></returns>
        public Item FindItem(string itemName)
        {

            /*
            // The code below is doing this, but in a single line using Microsoft LINQ - Read up about it, it's extremely powerful.
            foreach ( var item in _items )
            {
                if ( item.name.Equals(itemName))
                {
                    return item;
                }
            }

            return null;
            */

            return _items.DefaultIfEmpty(null).FirstOrDefault(x => x != null && x.name.Equals(itemName));
        }

    }
}