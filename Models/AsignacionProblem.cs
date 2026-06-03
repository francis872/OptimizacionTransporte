namespace OptimizadorIO.Models
{
    /// <summary>
    /// Representa un problema de asignación óptima (Método Húngaro).
    /// </summary>
    public class AsignacionProblem
    {
        /// <summary>
        /// Matriz de costos para la asignación.
        /// </summary>
        public double[,] MatrizCostos { get; set; }

        /// <summary>
        /// Número de tareas/trabajadores.
        /// </summary>
        public int Tamaño { get; }

        /// <summary>
        /// Inicializa una nueva instancia de AsignacionProblem.
        /// </summary>
        /// <param name="matrizCostos">Matriz cuadrada de costos n×n</param>
        public AsignacionProblem(double[,] matrizCostos)
        {
            if (matrizCostos == null)
                throw new ArgumentNullException(nameof(matrizCostos));

            if (matrizCostos.GetLength(0) != matrizCostos.GetLength(1))
                throw new ArgumentException("La matriz debe ser cuadrada");

            MatrizCostos = (double[,])matrizCostos.Clone();
            Tamaño = MatrizCostos.GetLength(0);
        }

        /// <summary>
        /// Obtiene una copia profunda del problema.
        /// </summary>
        public AsignacionProblem Clone()
        {
            return new AsignacionProblem(MatrizCostos);
        }
    }
}
