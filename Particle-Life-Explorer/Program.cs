using System.Windows.Forms;

namespace Particle_Life_Explorer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new MainForm();
            form.ShowDialog();
        }
    }
}