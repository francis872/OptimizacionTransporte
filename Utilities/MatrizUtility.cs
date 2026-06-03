namespace OptimizadorIO.Utilities
{
    /// <summary>
    /// Utilidades para trabajar con matrices.
    /// </summary>
    public static class MatrizUtility
    {
        /// <summary>
        /// Convierte una matriz a texto formateado.
        /// </summary>
        public static string MatrizATexto(double[,] matriz, string titulo = "")
        {
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(titulo))
                sb.AppendLine(titulo);

            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    sb.Append($"{matriz[i, j]:F2,8}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Valida que una matriz sea cuadrada.
        /// </summary>
        public static bool EsCuadrada(double[,] matriz)
        {
            return matriz.GetLength(0) == matriz.GetLength(1);
        }

        /// <summary>
        /// Obtiene el valor máximo en una matriz.
        /// </summary>
        public static double ObtenerMaximo(double[,] matriz)
        {
            double maximo = matriz[0, 0];
            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    if (matriz[i, j] > maximo)
                        maximo = matriz[i, j];
                }
            }
            return maximo;
        }

        /// <summary>
        /// Obtiene el valor mínimo en una matriz.
        /// </summary>
        public static double ObtenerMinimo(double[,] matriz)
        {
            double minimo = matriz[0, 0];
            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    if (matriz[i, j] < minimo)
                        minimo = matriz[i, j];
                }
            }
            return minimo;
        }
    }
}
