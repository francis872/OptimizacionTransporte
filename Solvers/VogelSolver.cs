namespace OptimizadorIO.Solvers;
using OptimizadorIO.Models;
using System.Diagnostics;

/// <summary>
/// Implementa el método de Aproximación de Vogel (VAM).
/// Calcula penalizaciones y asigna a celdas con máxima penalización.
/// </summary>
public class VogelSolver : SolverBase
{
    public override ResultadoMetodo Resolver(TransporteProblem problema)
        {
        var inicio = Stopwatch.StartNew();
        Problema = problema;
        Resultado = new ResultadoMetodo
        {
            NombreMetodo = "Aproximación de Vogel",
            EsOptimo = true
        };

        try
        {
            int m = Problema.NumeroOrigenes;
            int n = Problema.NumeroDestinos;

            double[,] asignacion = new double[m, n];
            double[,] costos = ClonarMatriz(Problema.MatrizCostos);
            double[] ofertaRestante = ClonarVector(Problema.Oferta);
            double[] demandaRestante = ClonarVector(Problema.Demanda);
            bool[] filaEliminada = new bool[m];
            bool[] columnaEliminada = new bool[n];

            int iteracion = 0;
            while (iteracion < Problema.NumeroOrigenes + Problema.NumeroDestinos - 1)
            {
                // Calcular penalizaciones para filas
                double[] penalizacionesFilas = new double[Problema.NumeroOrigenes];
                for (int i = 0; i < Problema.NumeroOrigenes; i++)
                {
                    if (filaEliminada[i] || ofertaRestante[i] == 0) continue;

                    double minimo1 = double.MaxValue, minimo2 = double.MaxValue;
                    for (int j = 0; j < Problema.NumeroDestinos; j++)
                    {
                        if (!columnaEliminada[j] && demandaRestante[j] > 0)
                        {
                            if (costos[i, j] < minimo1)
                            {
                                minimo2 = minimo1;
                                minimo1 = costos[i, j];
                            }
                            else if (costos[i, j] < minimo2)
                                minimo2 = costos[i, j];
                        }
                    }
                    penalizacionesFilas[i] = minimo2 - minimo1;
                }

                // Calcular penalizaciones para columnas
                double[] penalizacionesColumnas = new double[Problema.NumeroDestinos];
                for (int j = 0; j < Problema.NumeroDestinos; j++)
                {
                    if (columnaEliminada[j] || demandaRestante[j] == 0) continue;

                    double minimo1 = double.MaxValue, minimo2 = double.MaxValue;
                    for (int i = 0; i < Problema.NumeroOrigenes; i++)
                    {
                        if (!filaEliminada[i] && ofertaRestante[i] > 0)
                        {
                            if (costos[i, j] < minimo1)
                            {
                                minimo2 = minimo1;
                                minimo1 = costos[i, j];
                            }
                            else if (costos[i, j] < minimo2)
                                minimo2 = costos[i, j];
                        }
                    }
                    penalizacionesColumnas[j] = minimo2 - minimo1;
                }

                // Encontrar mayor penalización
                double penalizacionMaxima = 0;
                int filaMaxima = -1, columnaMaxima = -1;
                bool esFila = true;

                for (int i = 0; i < Problema.NumeroOrigenes; i++)
                {
                    if (penalizacionesFilas[i] > penalizacionMaxima && !filaEliminada[i] && ofertaRestante[i] > 0)
                    {
                        penalizacionMaxima = penalizacionesFilas[i];
                        filaMaxima = i;
                        esFila = true;
                    }
                }

                for (int j = 0; j < Problema.NumeroDestinos; j++)
                {
                    if (penalizacionesColumnas[j] > penalizacionMaxima && !columnaEliminada[j] && demandaRestante[j] > 0)
                    {
                        penalizacionMaxima = penalizacionesColumnas[j];
                        columnaMaxima = j;
                        esFila = false;
                    }
                }

                if (esFila)
                {
                    // Encontrar costo mínimo en la fila
                    double costoMinimo = double.MaxValue;
                    int columnaSeleccionada = -1;
                    for (int j = 0; j < Problema.NumeroDestinos; j++)
                    {
                        if (!columnaEliminada[j] && demandaRestante[j] > 0 && costos[filaMaxima, j] < costoMinimo)
                        {
                            costoMinimo = costos[filaMaxima, j];
                            columnaSeleccionada = j;
                        }
                    }

                    if (columnaSeleccionada != -1)
                    {
                        double asignar = Math.Min(ofertaRestante[filaMaxima], demandaRestante[columnaSeleccionada]);
                        asignacion[filaMaxima, columnaSeleccionada] = asignar;
                        ofertaRestante[filaMaxima] -= asignar;
                        demandaRestante[columnaSeleccionada] -= asignar;

                        if (ofertaRestante[filaMaxima] == 0) filaEliminada[filaMaxima] = true;
                        if (demandaRestante[columnaSeleccionada] == 0) columnaEliminada[columnaSeleccionada] = true;

                        var pasoInfo = new PasoResolucion
                        {
                            Descripcion = $"Iteración {iteracion + 1}: Penalización máxima {penalizacionMaxima:F2} (fila {filaMaxima + 1}). Asignar {asignar:F0} con costo {costoMinimo:F2}",
                            MatrizIntermedia = ClonarMatriz(asignacion),
                            CostoActual = CalcularCostoTotal(asignacion)
                        };
                        pasoInfo.Detalles["Tipo"] = "Fila";
                        pasoInfo.Detalles["Penalizacion"] = penalizacionMaxima;
                        Resultado.AgregarPaso(pasoInfo);
                    }
                }
                else
                {
                    // Encontrar costo mínimo en la columna
                    double costoMinimo = double.MaxValue;
                    int filaSeleccionada = -1;
                    for (int i = 0; i < Problema.NumeroOrigenes; i++)
                    {
                        if (!filaEliminada[i] && ofertaRestante[i] > 0 && costos[i, columnaMaxima] < costoMinimo)
                        {
                            costoMinimo = costos[i, columnaMaxima];
                            filaSeleccionada = i;
                        }
                    }

                    if (filaSeleccionada != -1)
                    {
                        double asignar = Math.Min(ofertaRestante[filaSeleccionada], demandaRestante[columnaMaxima]);
                        asignacion[filaSeleccionada, columnaMaxima] = asignar;
                        ofertaRestante[filaSeleccionada] -= asignar;
                        demandaRestante[columnaMaxima] -= asignar;

                        if (ofertaRestante[filaSeleccionada] == 0) filaEliminada[filaSeleccionada] = true;
                        if (demandaRestante[columnaMaxima] == 0) columnaEliminada[columnaMaxima] = true;

                        var pasoInfo = new PasoResolucion
                        {
                            Descripcion = $"Iteración {iteracion + 1}: Penalización máxima {penalizacionMaxima:F2} (columna {columnaMaxima + 1}). Asignar {asignar:F0} con costo {costoMinimo:F2}",
                            MatrizIntermedia = ClonarMatriz(asignacion),
                            CostoActual = CalcularCostoTotal(asignacion)
                        };
                        pasoInfo.Detalles["Tipo"] = "Columna";
                        pasoInfo.Detalles["Penalizacion"] = penalizacionMaxima;
                        Resultado.AgregarPaso(pasoInfo);
                    }
                }

                Resultado.AgregarMatrizIntermedia(asignacion);
                iteracion++;
            }

        Resultado.MatrizAsignacion = asignacion;
        Resultado.CostoTotal = CalcularCostoTotal(asignacion);
        Resultado.Mensaje = "Solución encontrada con penalizaciones";
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
