using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Particle_Life_Explorer.Gfx
{

    interface IDrawable
    {
        void SetDrawColor(Color color);
        void Render(Matrix4x4f projection, Vector2 pos);
    }


    /// <summary>
    /// Renders a circle to a given coordinate
    /// </summary>
    internal class Circle : IDrawable
    {
        static float[] shared_vertices = null;
        private static void GenerateGeometry(int vertex_count=20)
        {
            List<float> buffer = new List<float>();
            for (double i = 0; i < 2 * Math.PI; i += 2 * Math.PI / vertex_count)
            {
                buffer.Add((float)Math.Cos(i));
                buffer.Add((float)Math.Sin(i));
            }
            shared_vertices = buffer.ToArray();
        }


        ShaderProgram shader;
        VertexArray vertexArray;
        public float Radius { get; set; }

        public Circle(ShaderProgram glProgram, float radius)
        {
            if (shared_vertices == null)
                GenerateGeometry();

            this.shader = glProgram;
            Radius = radius;
            vertexArray = new VertexArray(shader, shared_vertices);
        }



        public void SetDrawColor(Color color)
        {
            float[] _colorf = new float[] { color.R / 255f, color.G / 255f, color.B / 255f };
            Gl.Uniform3(shader.GetUniformID("color"), _colorf);
        }


        public void Render(Matrix4x4f projection, Vector2 pos)
        {
            Gl.BindVertexArray(vertexArray.ArrayName);
            Matrix4x4f translation = Matrix4x4f.Translated(pos.X, pos.Y, 0.0f);
            Matrix4x4f scale = Matrix4x4f.Scaled(Radius, Radius, 1);

            Gl.UniformMatrix4f(shader.LocationMVP, 1, false, (scale * translation) * projection);
            Gl.DrawArrays(PrimitiveType.TriangleFan, 0, shared_vertices.Length);
        }
    }
}
