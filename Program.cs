namespace OptimizadorIO;
using OptimizadorIO.Forms;
using System.Windows.Forms;

/// <summary>
/// Punto de entrada de la aplicación OPTIMIZADOR IO.
/// </summary>
static class Program
{
    /// <summary>
    /// Punto principal de la aplicación.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FrmPrincipal());
    }
}
