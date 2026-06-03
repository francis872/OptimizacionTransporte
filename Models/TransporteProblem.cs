namespace OptimizadorIO.Models;

/// <summary>
/// Representa un problema de transporte con m orígenes y n destinos.
/// </summary>
public class TransporteProblem
{
    private double[,] _matrizCostos;
    private double[] _oferta;
    private double[] _demanda;

    /// <summary>
    /// Matriz de costos unitarios (m × n).
    /// </summary>
    public double[,] MatrizCostos
    {
        get => _matrizCostos;
        set => _matrizCostos = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Vector de oferta de cada origen.
    /// </summary>
    public double[] Oferta
    {
        get => _oferta;
        set => _oferta = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Vector de demanda de cada destino.
    /// </summary>
    public double[] Demanda
    {
        get => _demanda;
        set => _demanda = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Número de orígenes (filas).
    /// </summary>
    public int NumeroOrigenes => _matrizCostos.GetLength(0);

    /// <summary>
    /// Número de destinos (columnas).
    /// </summary>
    public int NumeroDestinos => _matrizCostos.GetLength(1);

    /// <summary>
    /// Oferta total disponible.
    /// </summary>
    public double OfertaTotal => _oferta.Sum();

    /// <summary>
    /// Demanda total requerida.
    /// </summary>
    public double DemandaTotal => _demanda.Sum();

    /// <summary>
    /// Indica si el problema está balanceado (oferta = demanda).
    /// </summary>
    public bool EstaBalanceado => Math.Abs(OfertaTotal - DemandaTotal) < 0.001;

    public TransporteProblem()
    {
        _matrizCostos = new double[0, 0];
        _oferta = new double[0];
        _demanda = new double[0];
    }

    /// <summary>
    /// Crea un nuevo problema de transporte.
    /// </summary>
    /// <param name="matrizCostos">Matriz de costos unitarios</param>
    /// <param name="oferta">Vector de oferta</param>
    /// <param name="demanda">Vector de demanda</param>
    public TransporteProblem(double[,] matrizCostos, double[] oferta, double[] demanda)
    {
        if (matrizCostos == null) throw new ArgumentNullException(nameof(matrizCostos));
        if (oferta == null) throw new ArgumentNullException(nameof(oferta));
        if (demanda == null) throw new ArgumentNullException(nameof(demanda));

        if (matrizCostos.GetLength(0) != oferta.Length)
            throw new ArgumentException("El número de filas de la matriz debe coincidir con la oferta.");

        if (matrizCostos.GetLength(1) != demanda.Length)
            throw new ArgumentException("El número de columnas de la matriz debe coincidir con la demanda.");

        ValidarDatos(matrizCostos, oferta, demanda);

        _matrizCostos = (double[,])matrizCostos.Clone();
        _oferta = (double[])oferta.Clone();
        _demanda = (double[])demanda.Clone();
    }

    /// <summary>
    /// Valida que los datos sean consistentes.
    /// </summary>
    private void ValidarDatos(double[,] matrizCostos, double[] oferta, double[] demanda)
    {
        // Validar que no haya valores negativos
        for (int i = 0; i < matrizCostos.GetLength(0); i++)
        {
            if (oferta[i] < 0)
                throw new ArgumentException($"La oferta en el origen {i} no puede ser negativa.");

            for (int j = 0; j < matrizCostos.GetLength(1); j++)
            {
                if (matrizCostos[i, j] < 0)
                    throw new ArgumentException($"El costo en la celda [{i},{j}] no puede ser negativo.");

                if (j == matrizCostos.GetLength(1) - 1 && demanda[j] < 0)
                    throw new ArgumentException($"La demanda en el destino {j} no puede ser negativa.");
            }
        }

        // Validar que demanda no sea nula si la oferta tampoco lo es
        if (demanda.Length > 0 && demanda.Any(d => d < 0))
            throw new ArgumentException("La demanda no puede contener valores negativos.");

        // Validar balance
        if (Math.Abs(oferta.Sum() - demanda.Sum()) > 0.001)
            throw new ArgumentException("La suma de oferta debe ser igual a la suma de demanda.");
    }

    /// <summary>
    /// Crea una copia profunda del problema.
    /// </summary>
    public TransporteProblem Clone()
    {
        return new TransporteProblem(
            (double[,])_matrizCostos.Clone(),
            (double[])_oferta.Clone(),
            (double[])_demanda.Clone()
        );
    }

    /// <summary>
    /// Obtiene una representación textual del problema.
    /// </summary>
    public override string ToString()
    {
        return $"Transporte {NumeroOrigenes}x{NumeroDestinos} - " +
               $"Oferta: {OfertaTotal:F0}, Demanda: {DemandaTotal:F0}, " +
               $"Balanceado: {EstaBalanceado}";
    }
}
