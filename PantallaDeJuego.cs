﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pang
{
    public class PantallaDeJuego
    {
        public Marcador marcador;
        private GestorDeNiveles gestorDeNiveles;
        private Personaje personaje;
        private Disparo disparo;
        private GestorDePantallas gestor;
        public bool Terminado { get; set; }
        private SpriteFont fuente;
        private Song musicaDeFondo;
        private SoundEffect sonidoDeDisparo;
        private bool tiempoTerminado;
        private int fotogramasRestantes;

        public PantallaDeJuego(GestorDePantallas gestor)
        {
            this.gestor = gestor;
            Terminado = false;
        }

        public void CargarContenidos(ContentManager Content)
        {
            fuente = Content.Load<SpriteFont>("Arial");
            personaje = new Personaje(549, 538, Content);
            disparo = new Disparo(0, 0, Content);
            marcador = new Marcador(Content);
            gestorDeNiveles = new GestorDeNiveles(Content);
            sonidoDeDisparo = Content.Load<SoundEffect>("sonidoDisparo");
            musicaDeFondo = Content.Load<Song>("musicaJuego");
            MediaPlayer.Play(musicaDeFondo);
            MediaPlayer.IsRepeating = true;
            tiempoTerminado = false;
            Reiniciar(Content);
        }

        public void Actualizar(GameTime gameTime, ContentManager Content)
        {
            MoverElementos(Content, gameTime);
            ComprobarEntrada(Content, gameTime);
            ComprobarColisiones(Content, gameTime);
            if (gestorDeNiveles.NivelActual.TiempoTerminado)
            {
                gestorDeNiveles.AvanzarNivel();
            }
        }

        private void MoverElementos(ContentManager Content, GameTime gameTime)
        {
            if (!tiempoTerminado)
            {
                foreach (Bola b in gestorDeNiveles.NivelActual.bolas)
                {
                    if (b.Caida && b.Y + b.Alto < gestorDeNiveles.NivelActual.Fondo.Alto -
                        gestorDeNiveles.NivelActual.Marco && b.Chocable)
                    {
                        b.Y += 10;
                    }
                    else
                    {
                        b.Caida = false;
                    }
                }

                foreach (Bola b in gestorDeNiveles.NivelActual.bolas)
                {
                    if (!b.Caida && b.Chocable)
                    {
                        b.MoverY();

                        b.X += b.VelocX;
                        if (b.X > gestorDeNiveles.NivelActual.Fondo.Ancho -
                            b.Ancho - gestorDeNiveles.NivelActual.Marco
                            || b.X < gestorDeNiveles.NivelActual.Marco)
                        {
                            b.VelocX = -b.VelocX;
                        }
                    }
                }

                disparo.Mover(gameTime, Content);

                gestorDeNiveles.NivelActual.Animar(gameTime);
                marcador.SetSegundosRestantes(gestorDeNiveles.NivelActual.SegundosRestantes);
            }
            if (gestorDeNiveles.NivelActual.TiempoTerminado && !tiempoTerminado)
            {
                tiempoTerminado = true;
                fotogramasRestantes = 100;
            }

            if (tiempoTerminado)
            {
                fotogramasRestantes--;
                if (fotogramasRestantes <= 0)
                {
                    tiempoTerminado = false;
                    PerderVida();
                }
            }

            foreach (Bola b in gestorDeNiveles.NivelActual.bolas)
            {
                b.Animar(gameTime);
            }
        }

        public void ComprobarEntrada(ContentManager Content, GameTime gameTime)
        {
            var estadoTeclado = Keyboard.GetState();

            if (estadoTeclado.IsKeyDown(Keys.S))
            {
                gestor.modoActual = GestorDePantallas.MODO.BIENVENIDA;
                Reiniciar(Content);
            }

            if (!estadoTeclado.IsKeyDown(Keys.Left) 
                    && !estadoTeclado.IsKeyDown(Keys.Right))
                personaje.PermanecerEstatico(gameTime);

            if (estadoTeclado.IsKeyDown(Keys.Left))
                personaje.MoverIzquierda(gameTime);

            

            if (estadoTeclado.IsKeyDown(Keys.Right))
                personaje.MoverDerecha(gameTime);


            if (!disparo.Chocable && estadoTeclado.IsKeyDown(Keys.Space))
            {
                sonidoDeDisparo.Play();
                disparo.X = personaje.X + 18;
                disparo.Y = 585;
                disparo.PosDisparo.Add(new Disparo((int)disparo.X, (int)disparo.Y, Content));
                disparo.Chocable = true;
                disparo.Visible = true;
            }
        }

        private void ComprobarColisiones(ContentManager Content, GameTime gameTime)
        {
            int i = 0;
            while (i < disparo.PosDisparo.Count && disparo.Chocable)
            {
                Sprite rDisparo = new Sprite(
                    (int)disparo.PosDisparo[i].X, (int)disparo.PosDisparo[i].Y,
                    "disparo", Content);

                for (int j = 0; j < gestorDeNiveles.NivelActual.bolas.Count; j++)
                {
                    if (gestorDeNiveles.NivelActual.bolas[j].ColisionaCon(rDisparo))
                    {
                        gestorDeNiveles.NivelActual.bolas[j].Explotar();
                        marcador.IncrementarPuntos(100);
                        disparo.Visible = false;
                        disparo.Chocable = false;
                        disparo.PosDisparo.Clear();
                        gestorDeNiveles.NivelActual.CrearNuevaBola(Content);
                    }
                }
                i++;
            }

            foreach (Bola b in gestorDeNiveles.NivelActual.bolas)
            {
                if (b.ColisionaCon(personaje))
                {
                    PerderVida();
                }
            }
        }

        private void PerderVida()
        {
            personaje.Vidas--;
            marcador.SetVidas(personaje.Vidas);
            gestorDeNiveles.NivelActual.Reiniciar();
            personaje.MoverAPosicionInicial();
            foreach (Bola b in gestorDeNiveles.NivelActual.bolas)
            {
                b.MoverAPosicionInicial();
                b.PosParabolaActual = 0;
            }
            
            disparo.Chocable = false;
            disparo.PosDisparo.Clear();

            Terminado = personaje.Vidas <= 0 ? true: false;
        }

        private void Reiniciar(ContentManager Content)
        {
            Terminado = false;
            gestorDeNiveles.NivelActual.Reiniciar();
            personaje.Vidas = 3;
            marcador.SetVidas(personaje.Vidas);
            marcador.ReiniciarPuntos();
            personaje.MoverAPosicionInicial();
            foreach (Bola b in gestorDeNiveles.NivelActual.bolas)
                b.MoverAPosicionInicial();
            disparo.Visible = false;
            disparo.Chocable = false;
            disparo.PosDisparo.Clear();
        }

        public void Dibujar(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gestorDeNiveles.NivelActual.Dibujar(spriteBatch);
            disparo.Dibujar(spriteBatch);
            personaje.Dibujar(spriteBatch);
            marcador.Dibujar(spriteBatch);

            if (tiempoTerminado)
            {
                spriteBatch.DrawString(fuente,
                "TIEMPO ACABADO",
                new Vector2(450, 200), Color.White);
            }
        }
    }
}
