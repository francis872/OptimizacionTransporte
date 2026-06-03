namespace OptimizadorIO.Solvers;
using OptimizadorIO.Models;

/// <summary>
/// Clase abstracta base para todos los solucionadores de problemas de transporte.
/// </summary>
public abstract class SolverBase
{
    /// <summary>
    /// Problema de transporte a resolver.
    /// </summary>
    protected TransporteProblem Problema { get; set; }

    /// <summary>
    /// Resultado de la resolución.
    /// </summary>
    protected ResultadoMetodo Resultado { get; set; }

    protected SolverBase()
    {
        Problema = new TransporteProblem();
        Resultado = new ResultadoMetodo();
    }

    /// <summary>
    /// Resuelve el problema de transporte.
    /// </summary>
    public abstract ResultadoMetodo Resolver(TransporteProblem problema);

    /// <summary>
    /// Calcula el costo total de una asignación.
    /// </summary>
    protected double CalcularCostoTotal(double[,] asignacion)
    {
        double costo = 0;
        int m = Problema.NumeroOrigenes;
        int n = Problema.NumeroDestinos;

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                costo += asignacion[i, j] * Problema.MatrizCostos[i, j];
            }
        }

        return costo;
    }

    /// <summary>
    /// Convierte una matriz a formato textual.
    /// </summary>
    protected string MatrizATexto(double[,] matriz, int decimales = 0)
    {
        if (matriz == null) return "null";

        var resultado = new System.Text.StringBuilder();
        int filas = matriz.GetLength(0);
        int columnas = matriz.GetLength(1);

        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (decimales == 0)
                    resultado.Append(((int)matriz[i, j]).ToString().PadLeft(6));
                else
                    resultado.Append(matriz[i, j].ToString($"F{decimales}").PadLeft(8));
            }
            resultado.AppendLine();
        }

        return resultado.ToString();
    }

    /// <summary>
    /// Clona una matriz.
    /// </summary>
    protected double[,] ClonarMatriz(double[,] matriz)
    {
        return (double[,])matriz.Clone();
    }

    /// <summary>
    /// Clona un vector.
    /// </summary>
    protected double[] ClonarVector(double[] vector)
    {
        return (double[])vector.Clone();
    }

    /// <summary>
    /// Encuentra el mínimo de una matriz (ignorando ceros).
    /// </summary>
    protected (int fila, int columna, double valor) EncontrarMinimo(double[,] matriz, bool[,] usadas)
    {
        double minimo = double.MaxValue;
        int filaMin = -1, colMin = -1;

        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                if (!usadas[i, j] && matriz[i, j] < minimo)
                {
                    minimo = matriz[i, j];
                    filaMin = i;
                    colMin = j;
                }
            }
        }

        return (filaMin, colMin, minimo);
    }
}
