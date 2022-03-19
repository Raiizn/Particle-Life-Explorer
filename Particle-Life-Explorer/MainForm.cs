using Khronos;
using OpenGL;
using Particle_Life_Explorer.Gfx;
using System;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Particle_Life_Explorer
{
    public partial class MainForm : Form
    {
        ShaderProgram shader;
        Circle circleOne, circleTwo;
        Viewport view;

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

            shader = new ShaderProgram(GlShaders.VertexBasic, GlShaders.FragmentSingleColor);
            Gl.UseProgram(shader.ProgramName);
            view = new Viewport(glControl, glControl.Width, glControl.Height);
            circleOne = new Circle(shader, 25);
            circleTwo = new Circle(shader, 10, Color.Red);

        }


        private void openGL_control_ContextDestroying(object sender, OpenGL.GlControlEventArgs e)
        {
            shader.Dispose();
        }


        private void openGL_control_ContextUpdate(object sender, OpenGL.GlControlEventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void openGL_control_Render(object sender, OpenGL.GlControlEventArgs e)
        {
            // Common GL commands
            Control senderControl = (Control)sender;

            // Clear
            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            // Render the test circles
            var viewMatrix = view.GetViewMatrix();
            circleOne.Render(viewMatrix, new float[] { 0, 0});
            circleTwo.Render(viewMatrix, new float[] { 230, 0 });

        }
        #endregion

    }
}
