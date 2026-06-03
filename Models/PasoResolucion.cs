namespace OptimizadorIO.Models;

/// <summary>
/// Representa un paso individual en la resolución de un problema de optimización.
/// </summary>
public class PasoResolucion
{
    /// <summary>
    /// Número secuencial del paso (1, 2, 3, ...).
    /// </summary>
    public int Numero { get; set; }

    /// <summary>
    /// Descripción detallada del paso realizado.
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Matriz intermedia después de este paso (si aplica).
    /// </summary>
    public double[,]? MatrizIntermedia { get; set; }

    /// <summary>
    /// Valores adicionales del paso (Ui, Vj, penalizaciones, etc.).
    /// </summary>
    public Dictionary<string, object> Detalles { get; set; } = new();

    /// <summary>
    /// Costo parcial o total en este paso.
    /// </summary>
    public double CostoActual { get; set; }

    /// <summary>
    /// Crea una copia profunda del paso.
    /// </summary>
    public PasoResolucion Clone()
    {
        var paso = new PasoResolucion
        {
            Numero = Numero,
            Descripcion = Descripcion,
            CostoActual = CostoActual,
            Detalles = new Dictionary<string, object>(Detalles)
        };

        if (MatrizIntermedia != null)
        {
            paso.MatrizIntermedia = (double[,])MatrizIntermedia.Clone();
        }

        return paso;
    }
}
