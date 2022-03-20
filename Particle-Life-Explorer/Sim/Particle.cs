using System;
using System.Drawing;
using System.Globalization;

namespace Particle_Life_Explorer.Sim
{
    
    /// <summary>
    /// Represents a class of particle and its properties and interactions.
    /// </summary>
    internal class ParticleClass
    {
        public ParticleClass(Color color)
        {
            Color = color;
        }


        public System.Drawing.Color Color { get; set; }
    }


    /// <summary>
    /// Particle instance class to simulate the rules defined by its ParticleClass.
    /// </summary>
    internal class Particle
    {
        static Random random = new Random();
        Simulation simulation;
        ParticleClass type;
        float x, y;
        float x_vel, y_vel;

        public float[] Pos
        {
            get
            {
                return new float[] { x, y };
            }
        }


        public Particle(Simulation sim, ParticleClass type, float x =0, float y = 0)
        {
            simulation = sim;
            this.type = type;
            this.x = x;
            this.y = y;
        }


        public void RandomVelocity(double max)
        {
            double speed = random.NextDouble() * max;
            double dir = random.NextDouble() * Math.PI * 2;
            x_vel = (float)(Math.Cos(dir) * speed);
            y_vel = (float)(Math.Sin(dir) * speed);
        }


        public void Update(double deltaTime)
        {
            x += (float)(x_vel * deltaTime);
            y += (float)(y_vel * deltaTime);
        }
    }
}
