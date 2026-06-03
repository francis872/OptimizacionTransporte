namespace OptimizadorIO.Utilities
{
    using OptimizadorIO.Models;
    using ClosedXML.Excel;
    using System.IO;

    /// <summary>
    /// Utilidad para exportar resultados a Excel.
    /// </summary>
    public static class ExcelExporter
    {
        /// <summary>
        /// Exporta un resultado a un archivo Excel.
        /// </summary>
        public static void ExportarResultado(ResultadoMetodo resultado, string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                // Hoja de resumen
                var hojaResumen = workbook.Worksheets.Add("Resumen");
                hojaResumen.Cell("A1").Value = "Método";
                hojaResumen.Cell("B1").Value = resultado.NombreMetodo;
                hojaResumen.Cell("A2").Value = "Costo Total";
                hojaResumen.Cell("B2").Value = resultado.CostoTotal;
                hojaResumen.Cell("A3").Value = "Tiempo de Ejecución (ms)";
                hojaResumen.Cell("B3").Value = resultado.TiempoEjecucion;

                // Hoja de asignación
                var hojaAsignacion = workbook.Worksheets.Add("Asignación");
                ExportarMatriz(hojaAsignacion, resultado.MatrizAsignacion, "Matriz de Asignación", 1);

                // Hoja de procedimiento
                if (resultado.Procedimiento.Count > 0)
                {
                    var hojaProcedimiento = workbook.Worksheets.Add("Procedimiento");
                    for (int i = 0; i < resultado.Procedimiento.Count; i++)
                    {
                        hojaProcedimiento.Cell($"A{i + 1}").Value = resultado.Procedimiento[i].Descripcion;
                    }
                }

                // Hoja de matrices intermedias
                if (resultado.MatricesIntermedias.Count > 0)
                {
                    var hojaMatrices = workbook.Worksheets.Add("Matrices Intermedias");
                    int filaActual = 1;
                    for (int k = 0; k < resultado.MatricesIntermedias.Count; k++)
                    {
                        hojaMatrices.Cell($"A{filaActual}").Value = $"Iteración {k + 1}";
                        filaActual += 2;
                        for (int i = 0; i < resultado.MatricesIntermedias[k].GetLength(0); i++)
                        {
                            for (int j = 0; j < resultado.MatricesIntermedias[k].GetLength(1); j++)
                            {
                                hojaMatrices.Cell(filaActual + i, j + 1).Value = 
                                    resultado.MatricesIntermedias[k][i, j];
                            }
                        }
                        filaActual += resultado.MatricesIntermedias[k].GetLength(0) + 2;
                    }
                }

                workbook.SaveAs(rutaArchivo);
            }
        }

        /// <summary>
        /// Exporta resultados comparativos de múltiples métodos.
        /// </summary>
        public static void ExportarComparacion(List<ResultadoMetodo> resultados, string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                var hoja = workbook.Worksheets.Add("Comparación");

                // Encabezados
                hoja.Cell("A1").Value = "Método";
                hoja.Cell("B1").Value = "Costo Total";
                hoja.Cell("C1").Value = "Tiempo (ms)";

                // Datos
                for (int i = 0; i < resultados.Count; i++)
                {
                    hoja.Cell($"A{i + 2}").Value = resultados[i].NombreMetodo;
                    hoja.Cell($"B{i + 2}").Value = resultados[i].CostoTotal;
                    hoja.Cell($"C{i + 2}").Value = resultados[i].TiempoEjecucion;

                    // Resaltar el mejor costo
                    if (resultados[i].CostoTotal == resultados.Min(r => r.CostoTotal))
                    {
                        hoja.Cell($"B{i + 2}").Style.Fill.BackgroundColor = XLColor.YellowGreen;
                        hoja.Cell($"B{i + 2}").Style.Font.Bold = true;
                    }
                }

                // Formato
                hoja.Columns("A", "C").AdjustToContents();

                workbook.SaveAs(rutaArchivo);
            }
        }

        /// <summary>
        /// Exporta una matriz en la hoja de trabajo.
        /// </summary>
        private static void ExportarMatriz(IXLWorksheet hoja, double[,] matriz, string titulo, int filaInicio)
        {
            hoja.Cell($"A{filaInicio}").Value = titulo;
            int filaActual = filaInicio + 1;

            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    hoja.Cell(filaActual + i, j + 1).Value = matriz[i, j];
                }
            }
        }
    }
}
