using OpenGL;
using System;
using System.Text;

/// <summary>
/// Open GL Abstractions
/// These are taken from https://github.com/luca-piccioni/OpenGL.Net/blob/master/Samples/HelloTriangle/SampleForm.cs
/// </summary>
namespace Particle_Life_Explorer.Gfx
{

    /// <summary>
    /// Shader object abstraction.
    /// </summary>
    class GLObject : IDisposable
    {
        public GLObject(ShaderType shaderType, string[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Create, upload, and compile the shader
            ShaderName = Gl.CreateShader(shaderType);
            Gl.ShaderSource(ShaderName, source);
            Gl.CompileShader(ShaderName);

            // Check compilation status
            int compiled;
            Gl.GetShader(ShaderName, ShaderParameterName.CompileStatus, out compiled);
            if (compiled != 0)
                return;

            // Throw exception on compilation errors
            const int logMaxLength = 1024;

            StringBuilder infolog = new StringBuilder(logMaxLength);
            int infologLength;

            Gl.GetShaderInfoLog(ShaderName, logMaxLength, out infologLength, infolog);

            throw new InvalidOperationException($"unable to compile shader: {infolog}");
        }

        public readonly uint ShaderName;

        public void Dispose()
        {
            Gl.DeleteShader(ShaderName);
        }
    }


    /// <summary>
    /// Buffer abstraction.
    /// </summary>
    class Buffer : IDisposable
    {
        public Buffer(float[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // Generate a buffer name: buffer does not exists yet
            BufferName = Gl.GenBuffer();
            // First bind create the buffer, determining its type
            Gl.BindBuffer(BufferTarget.ArrayBuffer, BufferName);
            // Set buffer information, 'buffer' is pinned automatically
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(4 * buffer.Length), buffer, BufferUsage.StaticDraw);
        }

        public readonly uint BufferName;

        public void Dispose()
        {
            Gl.DeleteBuffers(BufferName);
        }
    }


    /// <summary>
    /// Vertex array abstraction.
    /// </summary>
    class VertexArray : IDisposable
    {
        public VertexArray(ShaderProgram program, float[] positions, float[] colors)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            // Allocate buffers referenced by this vertex array
            _BufferPosition = new Buffer(positions);
            _BufferColor = new Buffer(colors);

            // Generate VAO name
            ArrayName = Gl.GenVertexArray();
            // First bind create the VAO
            Gl.BindVertexArray(ArrayName);

            // Set position attribute

            // Select the buffer object
            Gl.BindBuffer(BufferTarget.ArrayBuffer, _BufferPosition.BufferName);
            // Format the vertex information: 2 floats from the current buffer
            Gl.VertexAttribPointer((uint)program.LocationPosition, 2, VertexAttribType.Float, false, 0, IntPtr.Zero);
            // Enable attribute
            Gl.EnableVertexAttribArray((uint)program.LocationPosition);

            // As above, but for color attribute
            Gl.BindBuffer(BufferTarget.ArrayBuffer, _BufferColor.BufferName);
            Gl.VertexAttribPointer((uint)program.LocationColor, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);
            Gl.EnableVertexAttribArray((uint)program.LocationColor);
        }

        public readonly uint ArrayName;

        private readonly Buffer _BufferPosition;

        private readonly Buffer _BufferColor;

        public void Dispose()
        {
            Gl.DeleteVertexArrays(ArrayName);

            _BufferPosition.Dispose();
            _BufferColor.Dispose();
        }
    }


    /// <summary>
    ///GL Program abstraction.
    /// </summary>
    class ShaderProgram : IDisposable
    {
        public static ShaderProgram Default()
        {
            return new ShaderProgram(GlShaders.VertexSource, GlShaders.FragmentSource);
        }


        public ShaderProgram(string[] vertexSource, string[] fragmentSource)
        {
            // Create vertex and fragment shaders
            // Note: they can be disposed after linking to program; resources are freed when deleting the program
            using (GLObject vObject = new GLObject(ShaderType.VertexShader, vertexSource))
            using (GLObject fObject = new GLObject(ShaderType.FragmentShader, fragmentSource))
            {
                // Create program
                ProgramName = Gl.CreateProgram();
                // Attach shaders
                Gl.AttachShader(ProgramName, vObject.ShaderName);
                Gl.AttachShader(ProgramName, fObject.ShaderName);
                // Link program
                Gl.LinkProgram(ProgramName);

                // Check linkage status
                int linked;

                Gl.GetProgram(ProgramName, ProgramProperty.LinkStatus, out linked);

                if (linked == 0)
                {
                    const int logMaxLength = 1024;

                    StringBuilder infolog = new StringBuilder(logMaxLength);
                    int infologLength;

                    Gl.GetProgramInfoLog(ProgramName, 1024, out infologLength, infolog);

                    throw new InvalidOperationException($"unable to link program: {infolog}");
                }

                // Get uniform locations
                if ((LocationMVP = Gl.GetUniformLocation(ProgramName, "uMVP")) < 0)
                    throw new InvalidOperationException("no uniform uMVP");

                // Get attributes locations
                if ((LocationPosition = Gl.GetAttribLocation(ProgramName, "aPosition")) < 0)
                    throw new InvalidOperationException("no attribute aPosition");
                if ((LocationColor = Gl.GetAttribLocation(ProgramName, "aColor")) < 0)
                    throw new InvalidOperationException("no attribute aColor");
            }
        }

        public readonly uint ProgramName;

        public readonly int LocationMVP;

        public readonly int LocationPosition;

        public readonly int LocationColor;

        public void Dispose()
        {
            Gl.DeleteProgram(ProgramName);
        }
    }
}
