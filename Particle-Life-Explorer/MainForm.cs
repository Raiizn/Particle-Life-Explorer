using Khronos;
using Newtonsoft.Json;
using OpenGL;
using Particle_Life_Explorer.Gfx;
using Particle_Life_Explorer.Properties;
using Particle_Life_Explorer.Sim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Particle_Life_Explorer
{
    public partial class MainForm : Form
    {
        ShaderProgram shader;
        Viewport view;
        Simulation simulation;
        Random random;
        int PIXELS_PER_UNIT = 8;
        Vector2 initial_size;
        IDrawable partGfx = null;
        bool linear_forces = false;
        public MainForm()
        {
            InitializeComponent();
            Select();
            random = new Random();
            play_btn.Size = new Size(42, 42);
            speed_btn.Size = new Size(42, 42);
            popout_interaction_btn.Size = new Size(42, 42);
            btn_settings.Size = new Size(42, 42);
        }


        float RandRange(Vector2 range)
        {
            float delta = range.Y - range.X;
            return range.X + (float)random.NextDouble() * delta; 
        }

        void CreateSimulation()
        {
            GlControl glControl = openGL_control;
            float friction = 0.1f;
            int particle_count = 200;
            // Set up the different particle types
            ParticleClass.ResetIndices();
            var particle_types = new List<ParticleClass>()
            {
                new ParticleClass("A", Color.Red),
                new ParticleClass("B", Color.Blue),
                new ParticleClass("C", Color.Green),
                new ParticleClass("D", Color.Magenta),
                new ParticleClass("E", Color.Aqua),
                new ParticleClass("F", Color.Yellow)
            };
            simulation = new Simulation(PIXELS_PER_UNIT, initial_size.X, initial_size.Y, particle_types, particle_count, friction);

            // Interaction parameters for the randomly generated forces
            Vector2 min_range = new Vector2(0, 25);
            Vector2 max_range = new Vector2(25, 75);
            Vector2 strength_range = new Vector2(-0.25f, 0.25f);
            bool use_linear = linear_forces;

            // Create the interactions
            List<InteractionDef> interactions = new List<InteractionDef>();
            foreach (ParticleClass a in particle_types)
                foreach (ParticleClass b in particle_types)
                    interactions.Add(new InteractionDef(a, b,
                                                        RandRange(min_range),
                                                        RandRange(max_range),
                                                        RandRange(strength_range),
                                                        use_linear ? 1 : 2));
            foreach (var def in interactions)
                simulation.AddInteraction(def);

            // Particle gfx
            simulation.SetParticleGraphic(partGfx);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            initial_size = new Vector2(openGL_control.Width, openGL_control.Height);
            CreateSimulation();
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
            view.X -= glControl.Width/2;
            view.Y -= glControl.Height/2;
            partGfx = new Circle(shader, PIXELS_PER_UNIT / 2);
        }



        private void openGL_control_ContextDestroying(object sender, OpenGL.GlControlEventArgs e)
        {
            shader.Dispose();
        }


        private void openGL_control_ContextUpdate(object sender, OpenGL.GlControlEventArgs e)
        {
            simulation.Update();
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
            int w = senderControl.ClientSize.Width;
            int h = senderControl.ClientSize.Height;
            Gl.Viewport(0, 0, w, h);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            // Render the test circles
            var viewMatrix = view.GetViewMatrix();
            simulation.Draw(viewMatrix);
        }
        #endregion

        private void resetBtn_Click(object sender, EventArgs e)
        {
            var gfx = simulation.PartGraphic;
            CreateSimulation();
            simulation.SetParticleGraphic(gfx);
            openGL_control.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            simulation.ReseedParticles(200);
            openGL_control.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Basic serialization
            string path = "settings.json";
            var json_settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };

            // Setup wrapping structures
            Dictionary<string, object> values = new Dictionary<string, object>();
            List<ParticleClass> particles = new List<ParticleClass>(simulation.ParticleClasses);
            List<object> interactions = new List<object>();
           
            values["Particles"] = particles;
            values["Interactions"] = interactions;
            foreach(var partA in particles)
            {
                foreach(var partB in particles)
                {
                    var interaction = simulation.GetInteraction(partA, partB);
                    if (interaction != null)
                        interactions.Add(new Dictionary<string, object>()
                        {
                            {"self", partA.Name },
                            {"other", partB.Name },
                            {"force", interaction }
                        });
                }
            }

            values["Universe"] = new Dictionary<string, object>()
            {
                {"friction", simulation.Friction },
                {"width", simulation.Width },
                {"height", simulation.Height },
            };
            string json = JsonConvert.SerializeObject(values, json_settings);
            File.WriteAllText(path, json);
            openGL_control.Focus();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            linear_forces = checkBox1.Checked;
            foreach (var interaction in simulation.Interactions)
                interaction.exponent = linear_forces ? 1 : 2;
        }

        private void play_btn_Click(object sender, EventArgs e)
        {
            if (simulation.Speed != 1)
            {
                simulation.ChangeSpeed(1);
                play_btn.BackgroundImage = Resources.icon_pause;
            }
            else 
            {
                simulation.ChangeSpeed(0);
                play_btn.BackgroundImage = Resources.icon_play;
            }
            openGL_control.Focus();
        }

        private void speed_btn_Click(object sender, EventArgs e)
        {
            if(simulation.Speed < 6)
            {
                play_btn.BackgroundImage = Resources.icon_play;
                simulation.ChangeSpeed(6);
            }
            else
            {
                simulation.ChangeSpeed(1);
                play_btn.BackgroundImage = Resources.icon_pause;
            }
        }

        private void popout_interaction_btn_Click(object sender, EventArgs e)
        {
            if (openGL_control.ParentForm == this)
            {
                openGL_parent = openGL_control.Parent;
                Form form = new Form();
                Size size = openGL_control.Size;
                form.Controls.Add(openGL_control);
                form.Show();
                form.FormClosing += delegate (object s, FormClosingEventArgs ev)
                  {
                      openGL_parent.Controls.Add(openGL_control);
                  };

                form.Size = size;
                popout_interaction_btn.BackgroundImage = Resources.icon_popin;
                form.Focus();
            }
            else
            {
                openGL_control.ParentForm.Close();
                popout_interaction_btn.BackgroundImage = Resources.icon_popout;
                this.Focus();
            }
        }

        Control openGL_parent = null;
    }
}
