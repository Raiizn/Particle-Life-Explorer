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
        ShaderProgram shader;
        VertexArray vertexArray;


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


        public Circle(ShaderProgram glProgram, float radius)
        {
            if (shared_vertices == null)
                GenerateGeometry();

            this.shader = glProgram;
            Radius = radius;
            vertexArray = new VertexArray(shader, shared_vertices);
        }


        public float Radius
        {
            get; set;
        }


        public void SetDrawColor(Color color)
        {
            float[] _colorf = new float[] { color.R / 255f, color.G / 255f, color.B / 255f };
            Gl.Uniform3(shader.GetUniformID("color"), _colorf);
        }


        public void Render(Matrix4x4f projection, float[] pos)
        {
            Gl.BindVertexArray(vertexArray.ArrayName);
            Matrix4x4f translation = Matrix4x4f.Translated(pos[0], pos[1], 0.0f);
            Matrix4x4f scale = Matrix4x4f.Scaled(Radius, Radius, 1);

            Gl.UniformMatrix4f(shader.LocationMVP, 1, false, (scale * translation) * projection);
            Gl.DrawArrays(PrimitiveType.TriangleFan, 0, shared_vertices.Length);
        }
    }
}
