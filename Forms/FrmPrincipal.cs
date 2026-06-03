namespace OptimizadorIO.Forms
{
    using OptimizadorIO.Models;
    using OptimizadorIO.Solvers;
    using OptimizadorIO.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Formulario principal de la aplicación de Optimización y Transporte.
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        private ResultadoMetodo? resultadoActual;
        private TransporteProblem? problemaActual;
        private List<ResultadoMetodo>? resultadosComparacion;

        public FrmPrincipal()
        {
            InitializeComponent();
            InicializarComponentes();
        }

        /// <summary>
        /// Inicializa los componentes de la interfaz.
        /// </summary>
        private void InicializarComponentes()
        {
            // Configurar título
            this.Text = "Sistema de Optimización y Transporte";
            this.Size = new System.Drawing.Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Panel principal
            var panelPrincipal = new Panel();
            panelPrincipal.Dock = DockStyle.Fill;
            this.Controls.Add(panelPrincipal);

            // Panel lateral (Menú)
            var panelLateral = new Panel();
            panelLateral.Width = 200;
            panelLateral.Dock = DockStyle.Left;
            panelLateral.BackColor = System.Drawing.Color.DarkBlue;
            panelPrincipal.Controls.Add(panelLateral);

            // Botones del menú
            var botones = new[]
            {
                ("Esquina Noroeste", new EventHandler((s, e) => ResolverEsquinaNoroeste())),
                ("Costo Mínimo", new EventHandler((s, e) => ResolverCostoMinimo())),
                ("Método Vogel", new EventHandler((s, e) => ResolverVogel())),
                ("MODI", new EventHandler((s, e) => ResolverModi())),
                ("Húngaro", new EventHandler((s, e) => ResolverHungaro())),
                ("Comparar Métodos", new EventHandler((s, e) => CompararMetodos())),
                ("Exportar Excel", new EventHandler((s, e) => ExportarExcel())),
                ("Exportar PDF", new EventHandler((s, e) => ExportarPdf()))
            };

            int yPos = 10;
            foreach (var (texto, click) in botones)
            {
                var btn = new Button();
                btn.Text = texto;
                btn.Width = 180;
                btn.Height = 40;
                btn.Location = new System.Drawing.Point(10, yPos);
                btn.BackColor = System.Drawing.Color.LightBlue;
                btn.Click += click;
                panelLateral.Controls.Add(btn);
                yPos += 50;
            }

            // Panel central
            var panelCentral = new Panel();
            panelCentral.Dock = DockStyle.Fill;
            panelCentral.BackColor = System.Drawing.Color.White;
            panelPrincipal.Controls.Add(panelCentral);

            // Crear tabla con dos columnas: una para entrada y otra para resultados
            var tablaPrincipal = new TableLayoutPanel();
            tablaPrincipal.Dock = DockStyle.Fill;
            tablaPrincipal.ColumnCount = 2;
            tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panelCentral.Controls.Add(tablaPrincipal);

            // Sección de entrada de datos
            var panelEntrada = new Panel();
            panelEntrada.Dock = DockStyle.Fill;
            panelEntrada.BorderStyle = BorderStyle.FixedSingle;
            tablaPrincipal.Controls.Add(panelEntrada, 0, 0);

            var lblEntrada = new Label();
            lblEntrada.Text = "Entrada de Datos";
            lblEntrada.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            lblEntrada.Location = new System.Drawing.Point(10, 10);
            panelEntrada.Controls.Add(lblEntrada);

            var lblTamano = new Label();
            lblTamano.Text = "Tamaño (máx 4):";
            lblTamano.Location = new System.Drawing.Point(10, 40);
            panelEntrada.Controls.Add(lblTamano);

            var txtTamano = new NumericUpDown();
            txtTamano.Name = "txtTamano";
            txtTamano.Minimum = 1;
            txtTamano.Maximum = 4;
            txtTamano.Value = 3;
            txtTamano.Location = new System.Drawing.Point(120, 40);
            txtTamano.Width = 50;
            txtTamano.ValueChanged += (s, e) => GenerarMatrices();
            panelEntrada.Controls.Add(txtTamano);

            var lblMatrizCostos = new Label();
            lblMatrizCostos.Text = "Matriz de Costos:";
            lblMatrizCostos.Location = new System.Drawing.Point(10, 70);
            panelEntrada.Controls.Add(lblMatrizCostos);

            var dgvCostos = new DataGridView();
            dgvCostos.Name = "dgvCostos";
            dgvCostos.Location = new System.Drawing.Point(10, 95);
            dgvCostos.Width = 170;
            dgvCostos.Height = 120;
            dgvCostos.AllowUserToAddRows = false;
            panelEntrada.Controls.Add(dgvCostos);

            var lblOferta = new Label();
            lblOferta.Text = "Oferta:";
            lblOferta.Location = new System.Drawing.Point(10, 220);
            panelEntrada.Controls.Add(lblOferta);

            var dgvOferta = new DataGridView();
            dgvOferta.Name = "dgvOferta";
            dgvOferta.Location = new System.Drawing.Point(10, 240);
            dgvOferta.Width = 170;
            dgvOferta.Height = 80;
            dgvOferta.AllowUserToAddRows = false;
            panelEntrada.Controls.Add(dgvOferta);

            var lblDemanda = new Label();
            lblDemanda.Text = "Demanda:";
            lblDemanda.Location = new System.Drawing.Point(10, 325);
            panelEntrada.Controls.Add(lblDemanda);

            var dgvDemanda = new DataGridView();
            dgvDemanda.Name = "dgvDemanda";
            dgvDemanda.Location = new System.Drawing.Point(10, 345);
            dgvDemanda.Width = 170;
            dgvDemanda.Height = 80;
            dgvDemanda.AllowUserToAddRows = false;
            panelEntrada.Controls.Add(dgvDemanda);

            // Sección de resultados
            var panelResultados = new Panel();
            panelResultados.Dock = DockStyle.Fill;
            panelResultados.BorderStyle = BorderStyle.FixedSingle;
            tablaPrincipal.Controls.Add(panelResultados, 1, 0);

            var lblResultado = new Label();
            lblResultado.Text = "Resultados";
            lblResultado.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            lblResultado.Location = new System.Drawing.Point(10, 10);
            panelResultados.Controls.Add(lblResultado);

            var lblCostoTotal = new Label();
            lblCostoTotal.Name = "lblCostoTotal";
            lblCostoTotal.Text = "Costo Total: -";
            lblCostoTotal.Location = new System.Drawing.Point(10, 40);
            lblCostoTotal.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            panelResultados.Controls.Add(lblCostoTotal);

            var lblMetodo = new Label();
            lblMetodo.Name = "lblMetodo";
            lblMetodo.Text = "Método: -";
            lblMetodo.Location = new System.Drawing.Point(10, 65);
            panelResultados.Controls.Add(lblMetodo);

            var lblTiempo = new Label();
            lblTiempo.Name = "lblTiempo";
            lblTiempo.Text = "Tiempo (ms): -";
            lblTiempo.Location = new System.Drawing.Point(10, 90);
            panelResultados.Controls.Add(lblTiempo);

            var lblMatrizAsignacion = new Label();
            lblMatrizAsignacion.Text = "Matriz de Asignación:";
            lblMatrizAsignacion.Location = new System.Drawing.Point(10, 120);
            panelResultados.Controls.Add(lblMatrizAsignacion);

            var dgvAsignacion = new DataGridView();
            dgvAsignacion.Name = "dgvAsignacion";
            dgvAsignacion.Location = new System.Drawing.Point(10, 145);
            dgvAsignacion.Width = 170;
            dgvAsignacion.Height = 120;
            dgvAsignacion.AllowUserToAddRows = false;
            panelResultados.Controls.Add(dgvAsignacion);

            var lblProcedimiento = new Label();
            lblProcedimiento.Text = "Procedimiento:";
            lblProcedimiento.Location = new System.Drawing.Point(10, 270);
            panelResultados.Controls.Add(lblProcedimiento);

            var txtProcedimiento = new TextBox();
            txtProcedimiento.Name = "txtProcedimiento";
            txtProcedimiento.Location = new System.Drawing.Point(10, 295);
            txtProcedimiento.Width = 170;
            txtProcedimiento.Height = 130;
            txtProcedimiento.Multiline = true;
            txtProcedimiento.ScrollBars = ScrollBars.Vertical;
            txtProcedimiento.ReadOnly = true;
            panelResultados.Controls.Add(txtProcedimiento);

            GenerarMatrices();
        }

        /// <summary>
        /// Genera las matrices de entrada basadas en el tamaño especificado.
        /// </summary>
        private void GenerarMatrices()
        {
            var numTamano = this.Controls.Find("txtTamano", true)[0] as NumericUpDown;
            int tamano = (int)numTamano.Value;

            var dgvCostos = this.Controls.Find("dgvCostos", true)[0] as DataGridView;
            var dgvOferta = this.Controls.Find("dgvOferta", true)[0] as DataGridView;
            var dgvDemanda = this.Controls.Find("dgvDemanda", true)[0] as DataGridView;

            ConfigurarDataGridView(dgvCostos, tamano, tamano, "Costos");
            ConfigurarDataGridView(dgvOferta, 1, tamano, "Oferta");
            ConfigurarDataGridView(dgvDemanda, 1, tamano, "Demanda");
        }

        /// <summary>
        /// Configura un DataGridView para mostrar una matriz.
        /// </summary>
        private void ConfigurarDataGridView(DataGridView dgv, int filas, int columnas, string nombreBase)
        {
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            for (int j = 0; j < columnas; j++)
            {
                dgv.Columns.Add($"{nombreBase}_{j}", $"C{j + 1}");
            }

            for (int i = 0; i < filas; i++)
            {
                dgv.Rows.Add();
                for (int j = 0; j < columnas; j++)
                {
                    dgv.Rows[i].Cells[j].Value = 0;
                }
            }
        }

        /// <summary>
        /// Lee los datos de entrada del formulario.
        /// </summary>
        private bool ObtenerDatosEntrada(out double[,] matrizCostos, out double[] oferta, out double[] demanda)
        {
            matrizCostos = null;
            oferta = null;
            demanda = null;

            var dgvCostos = this.Controls.Find("dgvCostos", true)[0] as DataGridView;
            var dgvOferta = this.Controls.Find("dgvOferta", true)[0] as DataGridView;
            var dgvDemanda = this.Controls.Find("dgvDemanda", true)[0] as DataGridView;

            try
            {
                int m = dgvCostos.Rows.Count;
                int n = dgvCostos.Columns.Count;

                matrizCostos = new double[m, n];
                oferta = new double[m];
                demanda = new double[n];

                // Leer matriz de costos
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (!double.TryParse(dgvCostos.Rows[i].Cells[j].Value?.ToString() ?? "0", out double valor))
                            valor = 0;
                        matrizCostos[i, j] = valor;
                    }
                }

                // Leer oferta
                for (int i = 0; i < m; i++)
                {
                    if (!double.TryParse(dgvOferta.Rows[0].Cells[i].Value?.ToString() ?? "0", out double valor))
                        valor = 0;
                    oferta[i] = valor;
                }

                // Leer demanda
                for (int j = 0; j < n; j++)
                {
                    if (!double.TryParse(dgvDemanda.Rows[0].Cells[j].Value?.ToString() ?? "0", out double valor))
                        valor = 0;
                    demanda[j] = valor;
                }

                // Validar que suma de oferta = suma de demanda
                double sumaOferta = oferta.Sum();
                double sumaDemanda = demanda.Sum();

                if (Math.Abs(sumaOferta - sumaDemanda) > 0.0001)
                {
                    MessageBox.Show("La suma de oferta debe ser igual a la suma de demanda", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Muestra los resultados en la interfaz.
        /// </summary>
        private void MostrarResultados()
        {
            if (resultadoActual == null) return;

            var lblCostoTotal = this.Controls.Find("lblCostoTotal", true)[0] as Label;
            var lblMetodo = this.Controls.Find("lblMetodo", true)[0] as Label;
            var lblTiempo = this.Controls.Find("lblTiempo", true)[0] as Label;
            var dgvAsignacion = this.Controls.Find("dgvAsignacion", true)[0] as DataGridView;
            var txtProcedimiento = this.Controls.Find("txtProcedimiento", true)[0] as TextBox;

            lblCostoTotal.Text = $"Costo Total: {resultadoActual.CostoTotal:F2}";
            lblMetodo.Text = $"Método: {resultadoActual.NombreMetodo}";
            lblTiempo.Text = $"Tiempo (ms): {resultadoActual.TiempoEjecucion}";

            // Mostrar matriz de asignación
            MostrarMatrizEnDataGridView(dgvAsignacion, resultadoActual.MatrizAsignacion);

            // Mostrar procedimiento
            txtProcedimiento.Clear();
            foreach (var paso in resultadoActual.Procedimiento)
            {
                txtProcedimiento.AppendText(paso.Descripcion + Environment.NewLine);
            }
        }

        /// <summary>
        /// Muestra una matriz en un DataGridView.
        /// </summary>
        private void MostrarMatrizEnDataGridView(DataGridView dgv, double[,] matriz)
        {
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            int m = matriz.GetLength(0);
            int n = matriz.GetLength(1);

            for (int j = 0; j < n; j++)
            {
                dgv.Columns.Add($"Col{j}", $"C{j + 1}");
            }

            for (int i = 0; i < m; i++)
            {
                dgv.Rows.Add();
                for (int j = 0; j < n; j++)
                {
                    dgv.Rows[i].Cells[j].Value = matriz[i, j].ToString("F0");
                }
            }
        }

        // Métodos de resolución
        private void ResolverEsquinaNoroeste()
        {
            if (!ObtenerDatosEntrada(out var costos, out var oferta, out var demanda)) return;

            try
            {
                problemaActual = new TransporteProblem(costos, oferta, demanda);
                var solver = new EsquinaNoroesteSolver();
                resultadoActual = solver.Resolver(problemaActual);
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResolverCostoMinimo()
        {
            if (!ObtenerDatosEntrada(out var costos, out var oferta, out var demanda)) return;

            try
            {
                problemaActual = new TransporteProblem(costos, oferta, demanda);
                var solver = new CostoMinimoSolver();
                resultadoActual = solver.Resolver(problemaActual);
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResolverVogel()
        {
            if (!ObtenerDatosEntrada(out var costos, out var oferta, out var demanda)) return;

            try
            {
                problemaActual = new TransporteProblem(costos, oferta, demanda);
                var solver = new VogelSolver();
                resultadoActual = solver.Resolver(problemaActual);
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResolverModi()
        {
            if (!ObtenerDatosEntrada(out var costos, out var oferta, out var demanda)) return;

            try
            {
                problemaActual = new TransporteProblem(costos, oferta, demanda);
                var solver = new ModiSolver();
                resultadoActual = solver.Resolver(problemaActual);
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResolverHungaro()
        {
            if (!ObtenerDatosEntrada(out var costos, out var _, out var __)) return;

            try
            {
                if (!MatrizUtility.EsCuadrada(costos))
                {
                    MessageBox.Show("El método Húngaro requiere una matriz cuadrada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var problema = new AssignmentProblem(costos);
                var solver = new HungarianSolver();
                resultadoActual = solver.Resolver(problema);
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CompararMetodos()
        {
            if (!ObtenerDatosEntrada(out var costos, out var oferta, out var demanda)) return;

            try
            {
                resultadosComparacion = new List<ResultadoMetodo>();
                problemaActual = new TransporteProblem(costos, oferta, demanda);

                var solvers = new SolverBase[]
                {
                    new EsquinaNoroesteSolver(),
                    new CostoMinimoSolver(),
                    new VogelSolver(),
                    new ModiSolver()
                };

                foreach (var solver in solvers)
                {
                    resultadosComparacion.Add(solver.Resolver(problemaActual));
                }

                MostrarComparacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarComparacion()
        {
            var tabla = new StringBuilder();
            tabla.AppendLine("=== COMPARACIÓN DE MÉTODOS ===\n");
            tabla.AppendLine("Método                    | Costo Total | Tiempo (ms)");
            tabla.AppendLine("---------------------------------------------------");

            double costoMinimo = resultadosComparacion.Min(r => r.CostoTotal);

            foreach (var resultado in resultadosComparacion.OrderBy(r => r.CostoTotal))
            {
                string marca = resultado.CostoTotal == costoMinimo ? " ★ MEJOR" : "";
                tabla.AppendLine($"{resultado.NombreMetodo,-25} | {resultado.CostoTotal:F2,-11} | {resultado.TiempoEjecucion}{marca}");
            }

            var txtProcedimiento = this.Controls.Find("txtProcedimiento", true)[0] as TextBox;
            txtProcedimiento.Text = tabla.ToString();
        }

        private void ExportarExcel()
        {
            if (resultadoActual == null)
            {
                MessageBox.Show("Primero debe resolver un problema", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.DefaultExt = ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExcelExporter.ExportarResultado(resultadoActual, sfd.FileName);
                    MessageBox.Show("Archivo exportado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportarPdf()
        {
            if (resultadoActual == null)
            {
                MessageBox.Show("Primero debe resolver un problema", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files|*.pdf";
            sfd.DefaultExt = ".pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PdfExporter.ExportarResultado(resultadoActual, sfd.FileName);
                    MessageBox.Show("Archivo exportado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
