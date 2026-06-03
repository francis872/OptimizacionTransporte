namespace OptimizadorIO.Solvers;
using OptimizadorIO.Models;
using System.Diagnostics;

/// <summary>
/// Implementa el método del Costo Mínimo.
/// Busca la celda con menor costo y asigna la máxima cantidad posible.
/// </summary>
public class CostoMinimoSolver : SolverBase
{
    public override ResultadoMetodo Resolver(TransporteProblem problema)
    {
        var inicio = Stopwatch.StartNew();
        Problema = problema;
        Resultado = new ResultadoMetodo
        {
            NombreMetodo = "Costo Mínimo",
            EsOptimo = true
        };

        try
        {
            int m = Problema.NumeroOrigenes;
            int n = Problema.NumeroDestinos;

            double[,] asignacion = new double[m, n];
            double[] ofertaRestante = ClonarVector(Problema.Oferta);
            double[] demandaRestante = ClonarVector(Problema.Demanda);
            bool[,] celdasUsadas = new bool[m, n];

            int paso = 1;

            while (true)
            {
                // Encontrar celda con costo mínimo no asignada
                double costoMinimo = double.MaxValue;
                int filaMin = -1, colMin = -1;

                for (int i = 0; i < m; i++)
                {
                    if (ofertaRestante[i] == 0) continue;

                    for (int j = 0; j < n; j++)
                    {
                        if (demandaRestante[j] == 0 || celdasUsadas[i, j]) continue;

                        if (Problema.MatrizCostos[i, j] < costoMinimo)
                        {
                            costoMinimo = Problema.MatrizCostos[i, j];
                            filaMin = i;
                            colMin = j;
                        }
                    }
                }

                if (filaMin == -1) break;

                double cantidad = Math.Min(ofertaRestante[filaMin], demandaRestante[colMin]);
                asignacion[filaMin, colMin] = cantidad;
                celdasUsadas[filaMin, colMin] = true;

                var infoPaso = new PasoResolucion
                {
                    Descripcion = $"Paso {paso}: Costo mínimo {costoMinimo:F2} en ({filaMin},{colMin}). Asignar {cantidad:F0}",
                    MatrizIntermedia = ClonarMatriz(asignacion),
                    CostoActual = CalcularCostoTotal(asignacion)
                };
                infoPaso.Detalles["CostoMinimo"] = costoMinimo;
                Resultado.AgregarPaso(infoPaso);

                ofertaRestante[filaMin] -= cantidad;
                demandaRestante[colMin] -= cantidad;
                paso++;
            }

            Resultado.MatrizAsignacion = asignacion;
            Resultado.CostoTotal = CalcularCostoTotal(asignacion);
            Resultado.Mensaje = "Solución inicial encontrada";
        }
        catch (Exception ex)
        {
            Resultado.Mensaje = $"Error: {ex.Message}";
        }

        inicio.Stop();
        Resultado.TiempoEjecucion = inicio.ElapsedMilliseconds;
        return Resultado;
    }
}
