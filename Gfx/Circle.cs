
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Particle_Life_Explorer.Gfx
{
    /// <summary>
    /// Class to generate and hold shared geometry for circles
    /// </summary>
    internal class CircleGeometry
    {
        public float[] Vertices;
        public readonly float Radius;
        int vertex_count;

        public CircleGeometry(float radius, int vertex_count)
        {
            this.vertex_count = vertex_count;
            this.Radius = radius;
            GenerateVertices();
        }

        void GenerateVertices()
        {
            List<float> buffer = new List<float>();
            for (double i = 0; i < 2 * Math.PI; i += 2 * Math.PI / vertex_count)
            {
                buffer.Add((float)Math.Cos(i) * Radius);
                buffer.Add((float)Math.Sin(i) * Radius);
            }
            Vertices = buffer.ToArray();
        }
    }


    /// <summary>
    /// Render a circle to a given coordinate
    /// </summary>
    internal class Circle
    {
        static CircleGeometry shared_geometry = null;
        Color _color;
        GlProgram glProgram;
        VertexArray vertexArray;  
        public float X, Y;


        public Circle(GlProgram glProgram, float radius, float x = 0, float y = 0, Color color=default(Color))
        {
            if(shared_geometry == null)
                shared_geometry =new CircleGeometry(1, 24);
            if (color == default(Color))
                color = Color.White;
            this.X = x;
            this.Y = y;
            this.glProgram = glProgram;

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
            for(int i = 0; i<shared_geometry.Vertices.Length; i += 1)
            {
                buffer.Add(r);
                buffer.Add(g);
                buffer.Add(b);
            }
            vertexArray = new VertexArray(glProgram, shared_geometry.Vertices, buffer.ToArray());
        }
       

        public void Render(GlProgram glProgram, Matrix4x4f projection)
        {
            Gl.BindVertexArray(vertexArray.ArrayName);
            Matrix4x4f view = Matrix4x4f.Translated(X, Y, 0.0f);
            Matrix4x4f scale = Matrix4x4f.Scaled(Radius, Radius, Radius);
            Gl.UniformMatrix4f(glProgram.LocationMVP, 1, false, projection * scale * view);
            Gl.DrawArrays(PrimitiveType.TriangleFan, 0, shared_geometry.Vertices.Length);
        }
    }
}
