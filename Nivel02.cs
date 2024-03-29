﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pang
{
    class Nivel02 : Nivel
    {
        public Nivel02(ContentManager Content)
            : base(Content)
        {
            Fondo = new Sprite(0, 0, "fondoNivel2", Content);
            Bolas.Add(new Bola(249, 40, Content));
            Bolas.Add(new Bola(560, 40, Content));
            Item = new Sprite(900, 565, "reloj", Content);
        }


        public override void ActivarItem(bool itemUsado)
        {
            if (SegundosRestantes <= 50 && SegundosRestantes >= 40 && !itemUsado)
            {
                Item.Chocable = true;
                Item.Visible = true;
            }
            else
            {
                Item.Chocable = false;
                Item.Visible = false;
            }
        }
    }
}