
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Particle_Life_Explorer.Gfx
{
    /// <summary>
    /// Renders a circle to a given coordinate
    /// </summary>
    internal class Circle
    {
        static float[] shared_vertices = null;
        Color _color;
        ShaderProgram shader;
        VertexArray vertexArray;

        private static void GenerateGeometry(int vertex_count=20)
        {
            List<float> buffer = new List<float>();
            for (double i = 0; i < 2 * Math.PI; i += 2 * Math.PI / vertex_count)
            {
                buffer.Add((float)Math.Cos(i) * 0.5f);
                buffer.Add((float)Math.Sin(i) * 0.5f);
            }
            shared_vertices = buffer.ToArray();
        }


        public Circle(ShaderProgram glProgram, float radius, Color color = default(Color))
        {
            if (shared_vertices == null)
                GenerateGeometry();

            if (color == default(Color))
                color = Color.White;
            this.shader = glProgram;

            Radius = radius;
            _color = color;
            GenerateVertexArray();
        }


        public float Radius
        {
            get; set;
        }


        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (value != _color)
                {
                    _color = value;
                    GenerateVertexArray();
                }
            }
        }


        void GenerateVertexArray()
        {
            List<float> buffer = new List<float>();
            float r = Color.R / 255f;
            float g = Color.G / 255f;
            float b = Color.B / 255f;
            for (int i = 0; i < shared_vertices.Length; i += 1)
            {
                buffer.Add(r);
                buffer.Add(g);
                buffer.Add(b);
            }
            vertexArray = new VertexArray(shader, shared_vertices, buffer.ToArray());
        }


        public void Render(Matrix4x4f projection, float[] pos)
        {
            Gl.BindVertexArray(vertexArray.ArrayName);
            Matrix4x4f translation = Matrix4x4f.Translated(pos[0], pos[1], 0.0f);
            Matrix4x4f scale = Matrix4x4f.Scaled(2 *Radius, 2 *Radius, 1);
            Gl.UniformMatrix4f(shader.LocationMVP, 1, false, scale * translation * projection);
            Gl.DrawArrays(PrimitiveType.TriangleFan, 0, shared_vertices.Length);
        }
    }
}
