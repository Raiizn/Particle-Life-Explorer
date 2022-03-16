using Khronos;
using OpenGL;
using Particle_Life_Explorer.Gfx;
using System;
using System.Text;
using System.Windows.Forms;

namespace Particle_Life_Explorer
{
    public partial class MainForm : Form
    {
        GlProgram glProgram;
        VertexArray vertices;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {

        }


        #region OpenGL event handling
        private static void GLDebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string strMessage;

            // Decode message string
            unsafe
            {
                strMessage = Encoding.ASCII.GetString((byte*)message.ToPointer(), length);
            }

            Console.WriteLine($"{source}, {type}, {severity}: {strMessage}");
        }


        private void openGL_control_ContextCreated(object sender, OpenGL.GlControlEventArgs e)
        {
            GlControl glControl = (GlControl)sender;
            // GL Debugging
            if (Gl.CurrentExtensions != null && Gl.CurrentExtensions.DebugOutput_ARB)
            {
                Gl.DebugMessageCallback(GLDebugProc, IntPtr.Zero);
                Gl.DebugMessageControl(DebugSource.DontCare, DebugType.DontCare, DebugSeverity.DontCare, 0, null, true);
            }

            // Allocate resources and/or setup GL states
            bool supported = (Gl.CurrentVersion.Api == KhronosVersion.ApiGl && Gl.CurrentVersion >= Gl.Version_320);
            if(!supported)
                throw new Exception("Unsupported OpenGL version: " + Gl.CurrentVersion);

            glProgram = GlProgram.Default();
            Gl.UseProgram(glProgram.ProgramName);
            circleOne = new Circle(glProgram, 32, 1f, 0);
            circleTwo = new Circle(glProgram, 64, 0, 0, System.Drawing.Color.Blue);

        }


        Circle circleOne, circleTwo;

        private void openGL_control_ContextDestroying(object sender, OpenGL.GlControlEventArgs e)
        {
            glProgram.Dispose();
        }


        private void openGL_control_ContextUpdate(object sender, OpenGL.GlControlEventArgs e)
        {

        }


        private void openGL_control_Render(object sender, OpenGL.GlControlEventArgs e)
        {
            // Common GL commands
            Control senderControl = (Control)sender;

            // Clear
            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            // Render the test circles
            float w = senderControl.ClientSize.Width / 2f;
            float h = senderControl.ClientSize.Height / 2f;
            Matrix4x4f projection = Matrix4x4f.Ortho2D(-w, w, -h, h);
            circleOne.Render(glProgram, projection);
            circleTwo.Render(glProgram, projection);

        }
        #endregion

    }
}
