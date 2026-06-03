namespace OptimizadorIO.Models;

/// <summary>
/// Contiene el resultado completo de la resolución de un problema de optimización.
/// </summary>
public class ResultadoMetodo
{
    /// <summary>
    /// Nombre del algoritmo utilizado (Esquina Noroeste, Vogel, etc.).
    /// </summary>
    public string NombreMetodo { get; set; } = string.Empty;

    /// <summary>
    /// Matriz de asignación final.
    /// </summary>
    public double[,]? MatrizAsignacion { get; set; }

    /// <summary>
    /// Costo total óptimo o solución final.
    /// </summary>
    public double CostoTotal { get; set; }

    /// <summary>
    /// Secuencia de pasos realizados durante la resolución.
    /// </summary>
    public List<PasoResolucion> Procedimiento { get; set; } = new();

    /// <summary>
    /// Matrices intermedias capturadas durante cada iteración.
    /// </summary>
    public List<double[,]> MatricesIntermedias { get; set; } = new();

    /// <summary>
    /// Información adicional específica del método (Ui, Vj, penalizaciones, etc.).
    /// </summary>
    public Dictionary<string, object> DatosAdicionales { get; set; } = new();

    /// <summary>
    /// Tiempo de ejecución en milisegundos.
    /// </summary>
    public long TiempoEjecucion { get; set; }

    /// <summary>
    /// Indica si se alcanzó la solución óptima.
    /// </summary>
    public bool EsOptimo { get; set; } = true;

    /// <summary>
    /// Mensaje de estado o validación.
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Agrega un paso al procedimiento.
    /// </summary>
    public void AgregarPaso(PasoResolucion paso)
    {
        paso.Numero = Procedimiento.Count + 1;
        Procedimiento.Add(paso);
    }

    /// <summary>
    /// Agrega una matriz intermedia.
    /// </summary>
    public void AgregarMatrizIntermedia(double[,] matriz)
    {
        if (matriz != null)
        {
            MatricesIntermedias.Add((double[,])matriz.Clone());
        }
    }

    /// <summary>
    /// Obtiene un resumen de los resultados.
    /// </summary>
    public string ObtenerResumen()
    {
        return $"Método: {NombreMetodo}\n" +
               $"Costo Total: {CostoTotal:F2}\n" +
               $"Pasos: {Procedimiento.Count}\n" +
               $"Tiempo: {TiempoEjecucion}ms\n" +
               $"Óptimo: {(EsOptimo ? "Sí" : "No")}";
    }
}
