using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Particle_Life_Explorer.Sim
{
    /// <summary>
    /// Represents a class of particle and its properties and interactions.
    /// </summary>
    [Serializable]
    internal class ParticleClass
    {
        static int last_index = 0;
        public static int IndexCount { get { return last_index; } }

        public static void ResetIndices()
        {
            last_index = 0;
        }


        public string Name { get; protected set; }

        public Color Color { get; protected set; }

        [JsonIgnore()]
        public int Index { get; protected set; }


        public ParticleClass(string name, Color color)
        {
            Index = last_index;
            Name = name;
            Color = color;
            last_index += 1;
        }


        public ParticleClass(int index, string name, Color color)
        {
            Index = index;
            Name = name;
            Color = color;
        }

        public float GetAttraction(Interaction interaction, float distance)
        {
            if (distance <=0 || interaction == null)
                return 0f;
            if (distance > interaction.max_radius)
                return 0f;

            if (distance >= interaction.min_radius)
            {
                float delta = distance / (interaction.max_radius - interaction.min_radius);
                return (float)(interaction.strength / Math.Pow(delta, interaction.exponent));
            }
            else
            {
                float smoothing = 2.0f;
                return smoothing * interaction.min_radius * (1.0f / (interaction.min_radius + smoothing) - 1.0f / (distance + smoothing));
            }
        }

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
        Vector2 velocity;

        public Vector2 Pos
        {
            get
            {
                return new Vector2(x, y);
            }
        }

        public ParticleClass ParticleType
        {
            get
            {
                return type;
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
            velocity.X = (float)(Math.Cos(dir) * speed);
            velocity.Y = (float)(Math.Sin(dir) * speed);
        }


        public void Interact(Interaction interaction, Particle other)
        {
            Vector2 delta = new Vector2(other.x - x, other.y - y);
            if (delta == Vector2.Zero || other == this)
                return;

            // Account for the simulation wrapping in the distance delta
            if (delta.X > simulation.Width / 2)
                delta.X -= simulation.Width;
            else if (delta.X < -simulation.Width / 2)
                delta.X += simulation.Width;

            if (delta.Y > simulation.Height / 2)
                delta.Y -= simulation.Height;
            else if (delta.Y < -simulation.Height / 2)
                delta.Y += simulation.Height;

            // No force at a small enough distance
            float distance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            if (distance < 0.1)
                return;

            // Determine force to apply for this interaction
            float force = 0;
            if (distance <= 1)
                force = -1f * (float)Math.Max(0.2f, Math.Pow(velocity.Length(), 1f)) / distance*distance; // Repelling force for close particles
            else
                force = type.GetAttraction(interaction, distance); // Interaction based force

            velocity += Vector2.Normalize(delta) * force;
        }


        public void Update(double deltaTime)
        {
            // Apply velocity and friction
            float speed_delta = 1 / 10f;
            x += (float)(velocity.X * deltaTime * speed_delta);
            y += (float)(velocity.Y * deltaTime * speed_delta);
            velocity = (1.0f - simulation.Friction) * velocity;

            // Wrap
            if (x < 0)
                x += simulation.Width;
            else if (x >= simulation.Width) 
                x-=simulation.Width;
            if (y < 0)
                y += simulation.Height;
            else if (y >= simulation.Height)
                y -= simulation.Height;
        }
    }
}
