﻿using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Pang
{
    class GestorDeNiveles
    {
        public Nivel NivelActual { get; set; }
        private List<Nivel> niveles;
        private int numeroNivelActual;

        public GestorDeNiveles(ContentManager Content)
        {
            niveles = new List<Nivel>();
            niveles.Add(new Nivel01(Content));
            niveles.Add(new Nivel02(Content));
            numeroNivelActual = 0;
            NivelActual = niveles[0];
        }

        public void AvanzarNivel()
        {
            numeroNivelActual++;
            if (numeroNivelActual >= niveles.Count)
                numeroNivelActual = 0;

            NivelActual = niveles[numeroNivelActual];
        }
    }
}
