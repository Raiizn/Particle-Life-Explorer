
namespace Particle_Life_Explorer.Gfx
{
    public static class GlShaders
    {
        public readonly static string[] VertexSource =
        {
            "#version 150 compatibility\n",
            "uniform mat4 uMVP;\n",
            "in vec2 aPosition;\n",
            "in vec3 aColor;\n",
            "out vec3 vColor;\n",
            "void main() {\n",
            "	gl_Position = uMVP * vec4(aPosition, 0.0, 1.0);\n",
            "	vColor = aColor;\n",
            "}\n"
        };


        public readonly static string[] FragmentSource =
        {
            "#version 150 compatibility\n",
            "in vec3 vColor;\n",
            "void main() {\n",
            "	gl_FragColor = vec4(vColor, 1.0);\n",
            "}\n"
        };

        public readonly static string[] VertexBasic =
        {
            "#version 330 core\n",
            "uniform mat4 uMVP;\n",
            "in vec2 aPosition;\n",
            "out vec4 gl_Position;\n",
            "void main() {\n",
            "	gl_Position = uMVP * vec4(aPosition, 0.0, 1.0);\n",
            "}\n"
        };

        public readonly static string[] FragmentSingleColor =
        {
            "#version 330 core\n",
            "uniform vec3 color;\n",
            "out vec4 gl_FragColor;\n",
            "void main() {\n",
            "   gl_FragColor = vec4(color, 1.0);\n",
            "}\n"
        };
    }
}
