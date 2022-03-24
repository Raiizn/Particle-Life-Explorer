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
        Dictionary<ParticleClass, List<Particle>> particle_dict;
        Interaction[,] interaction_table;

        DateTime timer;
        public IDrawable PartGraphic { get; protected set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public float PixelsPerUnit { get; set; }
        public float Friction { get; set; }
        public float Speed { get; protected set; }

        public IEnumerable<ParticleClass> ParticleClasses
        {
            get { return particle_dict.Keys; }
        }

        public IEnumerable<Interaction> Interactions
        {
            get
            {
                for(int i = 0; i<interaction_table.GetLength(0); i++)
                    for(int j = 0; j<interaction_table.GetLength(1); j++)
                        yield return interaction_table[i, j];
            }
        }

        public IEnumerable<Particle> Particles
        {
            get
            {
                foreach(ParticleClass type in particle_dict.Keys)
                    foreach(var particle in particle_dict[type])
                        yield return particle;
            }
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
            this.particle_dict = new Dictionary<ParticleClass, List<Particle>>();
            foreach (var type in particle_types)
                particle_dict[type] = new List<Particle>();

            interaction_table = new Interaction[ParticleClass.IndexCount, ParticleClass.IndexCount];
            for (int i = 0; i < interaction_table.GetLength(0); i++)
                for (int j = 0; j < interaction_table.GetLength(1); j++)
                    interaction_table[i, j] = null;

            ReseedParticles(initial_particle_count);
            timer = DateTime.MaxValue;
            Speed = 1;
        }

        public void ChangeSpeed(float new_speed)
        {
            Speed = new_speed;
        }


        public void ReseedParticles(int particle_count)
        {
            Random random = new Random();

            // Evenly generate particles for each given particle class 
            foreach (var particle_list in particle_dict.Values)
                particle_list.Clear();
            List<ParticleClass> particle_types = new List<ParticleClass>(particle_dict.Keys);
            for (int i = 0; i < particle_count; i++)
            {
                ParticleClass type = particle_types[i % particle_types.Count];
                particle_dict[type].Add(new Particle(this, type, (float)random.NextDouble() * Width, (float)random.NextDouble() * Height));
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

            if (Speed > 0)
            {
                timedelta *= Speed;
                // Update attractions
                foreach (Particle partA in Particles)
                    foreach (Particle partB in Particles)
                        partA.Interact(GetInteraction(partA.ParticleType, partB.ParticleType), partB);

                // Other updates
                foreach (Particle part in Particles)
                    part.Update(timedelta);
            }
        }


        /// <summary>
        /// Renders all particles in the simulation using the stored part graphic.
        /// </summary>
        public void Draw(Matrix4x4f projection)
        {
            foreach (ParticleClass type in particle_dict.Keys)
            {
                PartGraphic.SetDrawColor(type.Color);
                foreach (var particle in particle_dict[type])
                    PartGraphic.Render(projection, particle.Pos*PixelsPerUnit);
            }
        }
    }
}
