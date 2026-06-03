namespace OptimizadorIO.Solvers;
using OptimizadorIO.Models;
using System.Diagnostics;

/// <summary>
/// Implementa el método de la Esquina Noroeste.
/// Algoritmo simple que inicia asignando desde la celda superior izquierda.
/// </summary>
public class EsquinaNoroesteSolver : SolverBase
{
    public override ResultadoMetodo Resolver(TransporteProblem problema)
    {
        var inicio = Stopwatch.StartNew();
        Problema = problema;
        Resultado = new ResultadoMetodo
        {
            NombreMetodo = "Esquina Noroeste",
            EsOptimo = false
        };

        try
        {
            int m = Problema.NumeroOrigenes;
            int n = Problema.NumeroDestinos;

            double[,] asignacion = new double[m, n];
            double[] ofertaRestante = ClonarVector(Problema.Oferta);
            double[] demandaRestante = ClonarVector(Problema.Demanda);

            int i = 0, j = 0;
            int paso = 1;

            while (i < m && j < n)
            {
                double cantidad = Math.Min(ofertaRestante[i], demandaRestante[j]);
                asignacion[i, j] = cantidad;

                var infoPaso = new PasoResolucion
                {
                    Descripcion = $"Paso {paso}: Asignar {cantidad:F0} en celda ({i},{j}). Oferta restante: {ofertaRestante[i] - cantidad:F0}, Demanda restante: {demandaRestante[j] - cantidad:F0}",
                    MatrizIntermedia = ClonarMatriz(asignacion),
                    CostoActual = CalcularCostoTotal(asignacion)
                };

                Resultado.AgregarPaso(infoPaso);

                ofertaRestante[i] -= cantidad;
                demandaRestante[j] -= cantidad;

                if (ofertaRestante[i] == 0) i++;
                else j++;

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
