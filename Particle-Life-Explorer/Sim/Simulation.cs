using OpenGL;
using Particle_Life_Explorer.Gfx;
using System;
using System.Collections.Generic;

namespace Particle_Life_Explorer.Sim
{
    /// <summary>
    /// Main simulation class.
    /// </summary>
    internal class Simulation
    {
        Dictionary<ParticleClass, List<Particle>> particles;
        DateTime timer;
        public Circle PartGraphic { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }


        /// <summary>
        /// Main constructor.
        /// </summary>
        public Simulation(float width, float height, List<ParticleClass> particle_types, int initial_particle_count)
        {
            this.Width = width;
            this.Height = height;
            this.particles = new Dictionary<ParticleClass, List<Particle>>();
            foreach (var type in particle_types)
                particles[type] = new List<Particle>();

            // Evenly generate particles for each given particle class 
            for (int i = 0; i < initial_particle_count; i++)
            {
                ParticleClass type = particle_types[i % particle_types.Count];
                particles[type].Add(new Particle(this, type));
            }

            // Add a random velocity to the particles
            foreach (var container in particles.Values)
                foreach (Particle part in container)
                    part.RandomVelocity(100);
            timer = DateTime.MaxValue;
        }


        public void SetParticleGraphic(Circle circle)
        {
            PartGraphic = circle;
        }


        /// <summary>
        /// Updates all particles according to their rulesets.
        /// </summary>
        public void Update()
        {
            // Get the time between frames
            double timedelta = 0;
            if (timer != DateTime.MaxValue)
                timedelta = DateTime.Now.Subtract(timer).TotalSeconds;
            timer = DateTime.Now;

            // Update
            foreach (var container in particles.Values)
                foreach (Particle part in container)
                    part.Update(timedelta);
        }


        /// <summary>
        /// Renders all particles in the simulation using the stored part graphic.
        /// </summary>
        public void Draw(Matrix4x4f projection)
        {
            foreach (ParticleClass type in particles.Keys)
            {
                PartGraphic.SetDrawColor(type.Color);
                foreach (var particle in particles[type])
                    PartGraphic.Render(projection, particle.Pos);
            }
        }
    }
}
