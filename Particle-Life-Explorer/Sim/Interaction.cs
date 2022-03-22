using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle_Life_Explorer.Sim
{
    /// <summary>
    /// Class that holds the settings for a particle-particle interaction
    /// </summary>
    internal class Interaction
    {
        public float min_radius;
        public float max_radius;
        public float strength;
        public float exponent;
        public Interaction(float min_radius=0, float max_radius=0, float strength=0, float exponent = 2)
        {
            this.min_radius = min_radius;
            this.max_radius = max_radius;
            this.strength = strength;
            this.exponent = exponent;
        }
        public static Interaction[,] CreateInteractionTable(List<InteractionDef> interactions)
        {
            Interaction[,] table = new Interaction[ParticleClass.IndexCount, ParticleClass.IndexCount];
            for (int i = 0; i < ParticleClass.IndexCount; i++)
                for (int j = 0; j < ParticleClass.IndexCount; j++)
                    table[i, j] = null;
            foreach (InteractionDef def in interactions)
                table[def.A.Index, def.B.Index] = def.Interaction;
            return table;
        }
    }


    /// <summary>
    /// Data class for managing interaction definitions
    /// </summary>
    class InteractionDef
    {
        public ParticleClass A, B;
        public Interaction Interaction;
        public InteractionDef(ParticleClass a, ParticleClass b, Interaction interaction)
        {
            this.A = a;
            this.B = b;
            this.Interaction = interaction;
        }
        public InteractionDef(ParticleClass a, ParticleClass b, float min_radius = 0, float max_radius = 0, float strength = 0, float exponent = 2)
        {
            this.A = a;
            this.B = b;
            this.Interaction = new Interaction(min_radius, max_radius, strength, exponent);
        }
    }
}
