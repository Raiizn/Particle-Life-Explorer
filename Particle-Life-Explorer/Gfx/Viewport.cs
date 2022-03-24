
using OpenGL;

namespace Particle_Life_Explorer.Gfx
{
    internal class Viewport
    {
        OpenGL.GlControl glControl;
        bool isSizeChangeDeferred = false;
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }


        public float AspectRatio
        {
            get
            {
                return Width / Height;
            }
        }


        float[] PixelsPerUnit
        {
            get
            {
                return new float[] { glControl.Width / Width, glControl.Height / Height };
            }
        }
       

        public Viewport(OpenGL.GlControl glControl, float width, float height)
        {
            this.glControl = glControl;
            Width = width;
            Height = height;
            X = Y = 0;
            glControl.SizeChanged += GlControl_SizeChanged;
        }


        public Matrix4x4f GetViewMatrix()
        {
            var projection = Matrix4x4f.Ortho2D(-Width / 2, Width / 2, -Height / 2, Height / 2);
            var view_pos = Matrix4x4f.Translated(X, Y, 0);
            return view_pos * projection;
        }


        private void GlControl_SizeChanged(object sender, System.EventArgs ev)
        {
            if (isSizeChangeDeferred)
                    return;

            isSizeChangeDeferred = true;
            try
            {
                glControl.Height = (int)(glControl.Width / AspectRatio);
            }
            finally
            {
                isSizeChangeDeferred = false;
                if (glControl.Parent != null)
                {
                    glControl.Top = (glControl.Parent.Height - glControl.Height) / 2;
                }
            }
        }
    }
}
