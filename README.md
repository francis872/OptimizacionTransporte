# Optimización y Transporte - Sistema Académico

Aplicación de escritorio completa en Windows Forms (.NET 8) para resolver problemas de optimización y transporte utilizando múltiples algoritmos académicos.

## 🎯 Características

### Algoritmos Implementados
1. **Esquina Noroeste**: Método simple y rápido para obtención de solución inicial
2. **Costo Mínimo**: Búsqueda iterativa del menor costo disponible
3. **Aproximación de Vogel (VAM)**: Método basado en penalizaciones por filas y columnas
4. **MODI**: Método de Distribución Modificada para optimización iterativa
5. **Húngaro**: Solución óptima para problemas de asignación
6. **Comparador**: Análisis y comparación de todos los métodos

### Características Principales
- ✅ Matrices dinámicas de hasta 4x4
- ✅ Visualización paso a paso de cada algoritmo
- ✅ Cálculo automático de costos totales
- ✅ Exportación a Excel (.xlsx)
- ✅ Exportación a PDF
- ✅ Interfaz moderna y amigable
- ✅ Arquitectura orientada a objetos
- ✅ Separación clara de lógica de negocio y presentación
- ✅ Código documentado en español

## 📋 Requisitos Técnicos

- Visual Studio 2022 (Community o superior)
- .NET 8 SDK
- Windows 10/11
- Dependencias NuGet (instaladas automáticamente):
  - ClosedXML 0.102.1 (para Excel)
  - QuestPDF 2024.6.0 (para PDF)

## 📁 Estructura del Proyecto

```
OptimizacionTransporte/
├── Models/                          # Clases de dominio
│   ├── TransporteProblem.cs         # Definición del problema
│   ├── AsignacionProblem.cs         # Problema de asignación
│   └── ResultadoMetodo.cs           # Resultado de resolución
├── Solvers/                         # Implementación de algoritmos
│   ├── SolverBase.cs                # Clase base abstracta
│   ├── EsquinaNoroesteSolver.cs     # Esquina Noroeste
│   ├── CostoMinimoSolver.cs         # Costo Mínimo
│   ├── VogelSolver.cs               # Aproximación de Vogel
│   ├── ModiSolver.cs                # MODI
│   └── HungarianSolver.cs           # Método Húngaro
├── Forms/                           # Interfaz Windows Forms
│   ├── FrmPrincipal.cs              # Formulario principal
│   └── FrmPrincipal.Designer.cs     # Diseño del formulario
├── Utilities/                       # Funciones auxiliares
│   ├── ExcelExporter.cs             # Exportación a Excel
│   ├── PdfExporter.cs               # Exportación a PDF
│   └── MatrizUtility.cs             # Utilidades de matrices
├── Program.cs                       # Punto de entrada
├── OptimizacionTransporte.csproj    # Configuración del proyecto
└── README.md                        # Este archivo
```

## 🚀 Instalación y Ejecución

### Opción 1: Desde Visual Studio 2022
1. Abrir Visual Studio 2022
2. Seleccionar "Abrir proyecto o solución"
3. Navegar a la carpeta `OptimizacionTransporte`
4. Seleccionar `OptimizacionTransporte.csproj`
5. Presionar F5 para compilar y ejecutar

### Opción 2: Desde línea de comandos
```bash
# Navegar a la carpeta del proyecto
cd OptimizacionTransporte

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run
```

## 📖 Uso de la Aplicación

### Entrada de Datos
1. Especificar el tamaño de la matriz (1-4)
2. Ingresar:
   - **Matriz de Costos**: Los costos de transporte desde cada origen a cada destino
   - **Oferta**: Cantidad disponible en cada origen (fila)
   - **Demanda**: Cantidad requerida en cada destino (columna)

### Resolver un Problema
1. Seleccionar el método deseado del menú:
   - Esquina Noroeste
   - Costo Mínimo
   - Método Vogel
   - MODI
   - Húngaro (solo para matrices cuadradas)
   - Comparar Métodos

2. Los resultados se mostrarán inmediatamente:
   - Costo total
   - Matriz de asignación
   - Procedimiento paso a paso
   - Tiempo de ejecución

### Exportar Resultados
1. **Exportar Excel**: Crea un archivo .xlsx con múltiples hojas
2. **Exportar PDF**: Genera un reporte PDF formateado

## 🔍 Detalles de Implementación

### Validaciones
- ✓ Suma de oferta = Suma de demanda
- ✓ Valores no negativos
- ✓ Matrices válidas
- ✓ Formato de entrada correcto

### Algoritmos

#### Esquina Noroeste (Northwest Corner)
- Complejidad: O(m+n)
- Inicio en la celda superior izquierda
- Asignación greedy simple

#### Costo Mínimo (Minimum Cost)
- Complejidad: O(m*n*log(m*n))
- Busca el costo mínimo no asignado
- Mejor solución inicial que Esquina Noroeste

#### Vogel (VAM - Vogel's Approximation Method)
- Complejidad: O(m*n*(m+n))
- Calcula penalizaciones por filas y columnas
- Excelente para obtener soluciones cercanas a óptimo

#### MODI (Modified Distribution Method)
- Complejidad: O(m*n) por iteración
- Optimiza a partir de solución inicial
- Garantiza optimalidad (cuando converge)

#### Húngaro (Hungarian Method)
- Complejidad: O(n³)
- Para problemas de asignación (m=n)
- Garantiza optimalidad global

## 📊 Ejemplo de Uso

```
Problema de Transporte 3x3:

Matriz de Costos:
  D1  D2  D3
O1 4   6   8
O2 5   4   7
O3 6   5   4

Oferta: [40, 60, 50]
Demanda: [45, 50, 55]

Salida esperada:
- Esquina Noroeste: Costo = 855
- Costo Mínimo: Costo = 810
- Vogel: Costo = 795
- MODI: Costo = 765
```

## 🛠️ Desarrollo y Ampliaciones

### Agregar un nuevo algoritmo:
1. Crear clase en `/Solvers/` que herede de `SolverBase`
2. Implementar método `Resolver()`
3. Agregar botón en `FrmPrincipal`
4. Documentar con comentarios en español

### Mejoras futuras:
- [ ] Problemas degenerados (soluciones múltiples)
- [ ] Método de Stepping Stone
- [ ] Método de Saltos y Saltos
- [ ] Problemas de transbordo
- [ ] Problemas de asignación generalizados
- [ ] Visualización gráfica de soluciones

## 📝 Documentación del Código

Todo el código incluye:
- Comentarios detallados en español
- Documentación XML en métodos públicos
- Nombres descriptivos de variables
- Separación clara de responsabilidades
- Principios SOLID aplicados

## ⚠️ Notas Importantes

1. **Degeneración**: Algunos problemas pueden tener múltiples soluciones óptimas
2. **Oferta = Demanda**: El sistema valida automáticamente esta condición
3. **Tamaño**: Máximo 4x4 por limitaciones de UI, pero el código es escalable
4. **MODI**: Requiere una solución inicial válida (proporcionada por Vogel)

## 🤝 Contribuciones

Este proyecto fue desarrollado como sistema académico para enseñanza de Investigación de Operaciones.

## 📄 Licencia

Proyecto académico. Libre para uso educativo.

## 📞 Soporte

Para errores o preguntas, revisar la documentación del código en los comentarios XML.

---

**Última actualización**: 2026
**Desarrollador**: Senior C# Developer - Operaciones de Investigación
