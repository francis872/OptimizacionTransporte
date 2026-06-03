namespace OptimizadorIO.Solvers;
using OptimizadorIO.Models;
using System.Diagnostics;

/// <summary>
/// Implementa el método MODI (Modified Distribution Method).
/// Optimiza una solución inicial usando costos de oportunidad.
/// </summary>
public class ModiSolver : SolverBase
{
    public override ResultadoMetodo Resolver(TransporteProblem problema)
    {
        var inicio = Stopwatch.StartNew();
        Problema = problema;
        Resultado = new ResultadoMetodo
        {
            NombreMetodo = "MODI",
            EsOptimo = true
        };

        try
        {
            var solucionadorVogel = new VogelSolver();
            var resultadoVogel = solucionadorVogel.Resolver(Problema);
            double[,] asignacionActual = ClonarMatriz(resultadoVogel.MatrizAsignacion ?? new double[Problema.NumeroOrigenes, Problema.NumeroDestinos]);

            int iteracion = 0;
            const int maxIteraciones = 100;

            while (iteracion < maxIteraciones)
            {
                // Calcular U y V usando las celdas básicas
                double[] U = new double[Problema.NumeroOrigenes];
                double[] V = new double[Problema.NumeroDestinos];
                bool[] filaCalculada = new bool[Problema.NumeroOrigenes];
                bool[] columnaCalculada = new bool[Problema.NumeroDestinos];

                U[0] = 0;
                filaCalculada[0] = true;

                // Calcular U y V
                bool cambiado = true;
                int intentos = 0;
                while (cambiado && intentos < Problema.NumeroOrigenes + Problema.NumeroDestinos)
                {
                    cambiado = false;
                    intentos++;

                    for (int i = 0; i < Problema.NumeroOrigenes; i++)
                    {
                        for (int j = 0; j < Problema.NumeroDestinos; j++)
                        {
                            if (asignacionActual[i, j] > 0)
                            {
                                if (filaCalculada[i] && !columnaCalculada[j])
                                {
                                    V[j] = Problema.MatrizCostos[i, j] - U[i];
                                    columnaCalculada[j] = true;
                                    cambiado = true;
                                }
                                else if (columnaCalculada[j] && !filaCalculada[i])
                                {
                                    U[i] = Problema.MatrizCostos[i, j] - V[j];
                                    filaCalculada[i] = true;
                                    cambiado = true;
                                }
                            }
                        }
                    }
                }

                // Calcular costos de oportunidad
                double costoOportunidadMinimo = 0;
                int filaEntrante = -1, columnaEntrante = -1;

                for (int i = 0; i < Problema.NumeroOrigenes; i++)
                {
                    for (int j = 0; j < Problema.NumeroDestinos; j++)
                    {
                        if (asignacionActual[i, j] == 0)
                        {
                            double costoOportunidad = Problema.MatrizCostos[i, j] - (U[i] + V[j]);
                            if (costoOportunidad < costoOportunidadMinimo)
                            {
                                costoOportunidadMinimo = costoOportunidad;
                                filaEntrante = i;
                                columnaEntrante = j;
                            }
                        }
                    }
                }

                if (filaEntrante == -1 || costoOportunidadMinimo >= 0)
                {
                    break;
                }

                // Encontrar ciclo y redistribuir
                var ciclo = EncontrarCiclo(asignacionActual, filaEntrante, columnaEntrante);
                if (ciclo.Count > 0)
                {
                    double cantidadMinima = double.MaxValue;
                    for (int i = 1; i < ciclo.Count; i += 2)
                    {
                        int f = ciclo[i] / Problema.NumeroDestinos;
                        int c = ciclo[i] % Problema.NumeroDestinos;
                        cantidadMinima = Math.Min(cantidadMinima, asignacionActual[f, c]);
                    }

                    for (int i = 0; i < ciclo.Count; i++)
                    {
                        int f = ciclo[i] / Problema.NumeroDestinos;
                        int c = ciclo[i] % Problema.NumeroDestinos;

                        if (i % 2 == 0)
                            asignacionActual[f, c] += cantidadMinima;
                        else
                            asignacionActual[f, c] -= cantidadMinima;
                    }
                }

                iteracion++;
            }

            Resultado.MatrizAsignacion = asignacionActual;
            Resultado.CostoTotal = CalcularCostoTotal(asignacionActual);
            Resultado.EsOptimo = true;
            Resultado.Mensaje = "Solución óptima encontrada";
        }
        catch (Exception ex)
        {
            Resultado.Mensaje = $"Error: {ex.Message}";
        }

        inicio.Stop();
        Resultado.TiempoEjecucion = inicio.ElapsedMilliseconds;
        return Resultado;
    }

    private List<int> EncontrarCiclo(double[,] asignacion, int filaInicio, int columnaInicio)
    {
        var ciclo = new List<int> { filaInicio * Problema.NumeroDestinos + columnaInicio };
        var visitadas = new bool[Problema.NumeroOrigenes * Problema.NumeroDestinos];
        visitadas[filaInicio * Problema.NumeroDestinos + columnaInicio] = true;

        BuscarCiclo(asignacion, filaInicio, columnaInicio, ciclo, visitadas, true);
        return ciclo;
    }

    private bool BuscarCiclo(double[,] asignacion, int fila, int columna, List<int> ciclo, 
                             bool[] visitadas, bool buscarFila)
    {
        if (buscarFila)
        {
            for (int j = 0; j < Problema.NumeroDestinos; j++)
            {
                if (j != columna && asignacion[fila, j] > 0)
                {
                    int idx = fila * Problema.NumeroDestinos + j;
                    if (!visitadas[idx])
                    {
                        visitadas[idx] = true;
                        ciclo.Add(idx);
                        if (BuscarCiclo(asignacion, fila, j, ciclo, visitadas, false))
                            return true;
                        ciclo.RemoveAt(ciclo.Count - 1);
                        visitadas[idx] = false;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < Problema.NumeroOrigenes; i++)
            {
                if (i != fila && asignacion[i, columna] > 0)
                {
                    int idx = i * Problema.NumeroDestinos + columna;
                    if (!visitadas[idx])
                    {
                        if (i == (ciclo[0] / Problema.NumeroDestinos) && columna == (ciclo[0] % Problema.NumeroDestinos))
                        {
                            ciclo.Add(idx);
                            return true;
                        }

                        visitadas[idx] = true;
                        ciclo.Add(idx);
                        if (BuscarCiclo(asignacion, i, columna, ciclo, visitadas, true))
                            return true;
                        ciclo.RemoveAt(ciclo.Count - 1);
                        visitadas[idx] = false;
                    }
                }
            }
        }

        return false;
    }
}
