using ECSL;
using System;
using System.Linq;


namespace Arkanoid.Components
{
    /// <summary>
    /// Represent a HP component.
    /// </summary>
    public class HP : IComponent
    {

        public int Points { get; set; }

        /// <summary>
        /// Initialize new HP
        /// </summary>
        public HP(int points)
        {
            Points = points;
        }

    }
}
