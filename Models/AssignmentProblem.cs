namespace OptimizadorIO.Models;

/// <summary>
/// Representa un problema de asignación óptima (matriz cuadrada).
/// </summary>
public class AssignmentProblem
{
    private double[,] _matrizCostos;

    /// <summary>
    /// Matriz de costos cuadrada (n × n).
    /// </summary>
    public double[,] MatrizCostos
    {
        get => _matrizCostos;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.GetLength(0) != value.GetLength(1))
                throw new ArgumentException("La matriz debe ser cuadrada.");

            _matrizCostos = value;
        }
    }

    /// <summary>
    /// Tamaño de la matriz (n × n).
    /// </summary>
    public int Tamaño => _matrizCostos.GetLength(0);

    public AssignmentProblem()
    {
        _matrizCostos = new double[0, 0];
    }

    /// <summary>
    /// Crea un nuevo problema de asignación.
    /// </summary>
    /// <param name="matrizCostos">Matriz de costos cuadrada n×n</param>
    public AssignmentProblem(double[,] matrizCostos)
    {
        if (matrizCostos == null)
            throw new ArgumentNullException(nameof(matrizCostos));

        if (matrizCostos.GetLength(0) != matrizCostos.GetLength(1))
            throw new ArgumentException("La matriz debe ser cuadrada para un problema de asignación.");

        ValidarDatos(matrizCostos);

        _matrizCostos = (double[,])matrizCostos.Clone();
    }

    /// <summary>
    /// Valida que los datos sean consistentes.
    /// </summary>
    private void ValidarDatos(double[,] matrizCostos)
    {
        int n = matrizCostos.GetLength(0);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (matrizCostos[i, j] < 0)
                    throw new ArgumentException($"El costo en la celda [{i},{j}] no puede ser negativo.");
            }
        }
    }

    /// <summary>
    /// Crea una copia profunda del problema.
    /// </summary>
    public AssignmentProblem Clone()
    {
        return new AssignmentProblem((double[,])_matrizCostos.Clone());
    }

    /// <summary>
    /// Obtiene una representación textual del problema.
    /// </summary>
    public override string ToString()
    {
        return $"Asignación {Tamaño}x{Tamaño}";
    }
}
