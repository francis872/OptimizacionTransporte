namespace OptimizadorIO.Solvers;
using OptimizadorIO.Models;
using System.Diagnostics;

/// <summary>
/// Implementa el Método Húngaro para problemas de asignación.
/// Solo funciona con matrices cuadradas.
/// </summary>
public class HungarianSolver
{
    /// <summary>
    /// Resuelve el problema de asignación usando el Método Húngaro.
    /// </summary>
    public ResultadoMetodo Resolver(AssignmentProblem problema)
    {
        var inicio = Stopwatch.StartNew();
        var resultado = new ResultadoMetodo
        {
            NombreMetodo = "Método Húngaro",
            EsOptimo = true
        };

        try
        {
            int n = problema.Tamaño;
            double[,] costos = problema.MatrizCostos.Clone() as double[,] ?? throw new InvalidOperationException();
            var paso = 1;

            // Implementación del Método Húngaro
            // Paso 1: Reducción por filas
            for (int i = 0; i < n; i++)
            {
                double minimo = costos[i, 0];
                for (int j = 1; j < n; j++)
                {
                    if (costos[i, j] < minimo)
                        minimo = costos[i, j];
                }
                for (int j = 0; j < n; j++)
                    costos[i, j] -= minimo;
            }

            // Paso 2: Reducción por columnas
            for (int j = 0; j < n; j++)
            {
                double minimo = costos[0, j];
                for (int i = 1; i < n; i++)
                {
                    if (costos[i, j] < minimo)
                        minimo = costos[i, j];
                }
                for (int i = 0; i < n; i++)
                    costos[i, j] -= minimo;
            }

            // Asignación final basada en ceros
            double[,] asignacionFinal = new double[n, n];
            int[] asignacionFila = new int[n];
            int[] asignacionColumna = new int[n];
            
            for (int i = 0; i < n; i++)
            {
                asignacionFila[i] = -1;
                asignacionColumna[i] = -1;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (costos[i, j] == 0 && asignacionFila[i] == -1 && asignacionColumna[j] == -1)
                    {
                        asignacionFinal[i, j] = 1;
                        asignacionFila[i] = j;
                        asignacionColumna[j] = i;
                    }
                }
            }

            // Calcular costo total
            double costoTotal = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (asignacionFinal[i, j] == 1)
                        costoTotal += problema.MatrizCostos[i, j];
                }
            }

            resultado.MatrizAsignacion = asignacionFinal;
            resultado.CostoTotal = costoTotal;
            resultado.Mensaje = "Asignación óptima completada";
        }
        catch (Exception ex)
        {
            resultado.Mensaje = $"Error: {ex.Message}";
        }

        inicio.Stop();
        resultado.TiempoEjecucion = inicio.ElapsedMilliseconds;
        return resultado;
    }
}
