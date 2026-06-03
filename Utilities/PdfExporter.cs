namespace OptimizadorIO.Utilities
{
    using OptimizadorIO.Models;
    using QuestPDF.Fluent;
    using QuestPDF.Infrastructure;

    /// <summary>
    /// Utilidad para exportar resultados a PDF.
    /// </summary>
    public static class PdfExporter
    {
        /// <summary>
        /// Exporta un resultado a un archivo PDF.
        /// </summary>
        public static void ExportarResultado(ResultadoMetodo resultado, string rutaArchivo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text(resultado.NombreMetodo ?? "Resultado").FontSize(20).Bold();

                    page.Content().Column(column =>
                    {
                        // Resumen
                        column.Item().Text("Resumen").FontSize(14).Bold();
                        column.Item().PaddingVertical(5);
                        column.Item().Text($"Método: {resultado.NombreMetodo ?? "N/A"}");
                        column.Item().Text($"Costo Total: {resultado.CostoTotal:F2}");
                        column.Item().Text($"Tiempo de Ejecución: {resultado.TiempoEjecucion}ms");

                        column.Item().PaddingVertical(10);

                        // Matriz de Asignación
                        column.Item().Text("Matriz de Asignación").FontSize(12).Bold();
                        column.Item().PaddingVertical(5);

                        if (resultado.MatrizAsignacion != null)
                        {
                            column.Item().Table(table =>
                            {
                                int filas = resultado.MatrizAsignacion.GetLength(0);
                                int columnas = resultado.MatrizAsignacion.GetLength(1);

                                table.ColumnsDefinition(columns =>
                                {
                                    for (int j = 0; j < columnas; j++)
                                        columns.RelativeColumn();
                                });

                                for (int i = 0; i < filas; i++)
                                {
                                    for (int j = 0; j < columnas; j++)
                                    {
                                        table.Cell().Text(resultado.MatrizAsignacion[i, j].ToString("F0"));
                                    }
                                }
                            });
                        }

                        column.Item().PaddingVertical(10);

                        // Procedimiento
                        if (resultado.Procedimiento.Count > 0)
                        {
                            column.Item().Text("Procedimiento Paso a Paso").FontSize(12).Bold();
                            column.Item().PaddingVertical(5);
                            
                            foreach (var paso in resultado.Procedimiento.Take(10))
                            {
                                column.Item().Text(paso.Descripcion).FontSize(9);
                            }

                            if (resultado.Procedimiento.Count > 10)
                            {
                                column.Item().PaddingVertical(5);
                                column.Item().Text($"... y {resultado.Procedimiento.Count - 10} pasos más").FontSize(9).Italic();
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text("Generado por Sistema de Optimización y Transporte");
                });
            })
            .GeneratePdf(rutaArchivo);
        }

        /// <summary>
        /// Exporta resultados comparativos a PDF.
        /// </summary>
        public static void ExportarComparacion(List<ResultadoMetodo> resultados, string rutaArchivo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var mejorMetodo = resultados.OrderBy(r => r.CostoTotal).First();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Comparación de Métodos de Optimización").FontSize(20).Bold();

                    page.Content().Column(column =>
                    {
                        column.Item().Text("Resultados Comparativos").FontSize(14).Bold();
                        column.Item().PaddingVertical(5);
                        
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Encabezados
                            table.Cell().Text("Método").Bold();
                            table.Cell().Text("Costo Total").Bold();
                            table.Cell().Text("Tiempo (ms)").Bold();

                            // Datos
                            foreach (var resultado in resultados.OrderBy(r => r.CostoTotal))
                            {
                                table.Cell().Text(resultado.NombreMetodo ?? "N/A");
                                
                                var textoCosto = resultado.NombreMetodo == mejorMetodo.NombreMetodo 
                                    ? $"{resultado.CostoTotal:F2} ★" 
                                    : $"{resultado.CostoTotal:F2}";
                                table.Cell().Text(textoCosto);
                                
                                table.Cell().Text(resultado.TiempoEjecucion.ToString());
                            }
                        });

                        column.Item().PaddingVertical(10);
                        column.Item().Text($"Mejor Método: {mejorMetodo.NombreMetodo ?? "N/A"} (Costo: {mejorMetodo.CostoTotal:F2})")
                            .FontSize(12).Bold();
                    });

                    page.Footer().AlignCenter().Text("Generado por Sistema de Optimización y Transporte");
                });
            })
            .GeneratePdf(rutaArchivo);
        }
    }
}
