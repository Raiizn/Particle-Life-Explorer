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
        Interaction[,] interaction_table;

        DateTime timer;
        public IDrawable PartGraphic { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float PixelsPerUnit { get; set; }
        public float Friction { get; set; }
        public IEnumerable<ParticleClass> ParticleClasses
        {
            get { return particles.Keys; }
        }

        /// <summary>
        /// Main constructor.
        /// </summary>
        public Simulation(int pixels_per_unit, float width, float height, List<ParticleClass> particle_types, int initial_particle_count, float friction)
        {
            PixelsPerUnit = pixels_per_unit; ;
            this.Width = width / PixelsPerUnit;
            this.Height = height / PixelsPerUnit;
            Friction = friction;
            this.particles = new Dictionary<ParticleClass, List<Particle>>();
            foreach (var type in particle_types)
                particles[type] = new List<Particle>();

            interaction_table = new Interaction[ParticleClass.IndexCount, ParticleClass.IndexCount];
            for (int i = 0; i < interaction_table.GetLength(0); i++)
                for (int j = 0; j < interaction_table.GetLength(1); j++)
                    interaction_table[i, j] = null;

            ReseedParticles(initial_particle_count);
            timer = DateTime.MaxValue;
        }


        public void ReseedParticles(int particle_count)
        {
            Random random = new Random();

            // Evenly generate particles for each given particle class 
            foreach (var particle_list in particles.Values)
                particle_list.Clear();
            List<ParticleClass> particle_types = new List<ParticleClass>(particles.Keys);
            for (int i = 0; i < particle_count; i++)
            {
                ParticleClass type = particle_types[i % particle_types.Count];
                particles[type].Add(new Particle(this, type, (float)random.NextDouble() * Width, (float)random.NextDouble() * Height));
            }

        }


        public void SetParticleGraphic(IDrawable drawable)
        {
            PartGraphic = drawable;
        }


        public void AddInteraction(InteractionDef def)
        {
            interaction_table[def.A.Index, def.B.Index] = def.Interaction;
        }



        public Interaction GetInteraction(ParticleClass A, ParticleClass B)
        {
            if(A.Index<interaction_table.GetLength(0) && B.Index<interaction_table.GetLength(1))
                return interaction_table[A.Index, B.Index];
            return null;
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

            // Update attractions
            foreach (var container in particles.Values)
                foreach (Particle partA in container)
                    foreach (var containerB in particles.Values)
                        foreach (Particle partB in containerB)
                            partA.Interact(GetInteraction(partA.ParticleType, partB.ParticleType), partB);

            // Other updates
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
                    PartGraphic.Render(projection, particle.Pos*PixelsPerUnit);
            }
        }
    }
}
