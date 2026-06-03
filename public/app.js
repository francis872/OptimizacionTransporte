const metodoSelect = document.getElementById('metodo');
const origenesInput = document.getElementById('origenes');
const destinosInput = document.getElementById('destinos');
const generarBtn = document.getElementById('generar');
const resolverBtn = document.getElementById('resolver');
const tablaCostos = document.getElementById('tabla-costos');
const resultadoContainer = document.getElementById('resultado');
const pasosContainer = document.getElementById('pasos');
const alertBox = document.getElementById('alert');
const balanceInfo = document.getElementById('balance-info');
const exportBtn = document.getElementById('export-csv');
const pasteBtn = document.getElementById('paste-clipboard');

function mostrarAlerta(texto) {
  alertBox.textContent = texto;
  alertBox.style.display = texto ? 'block' : 'none';
}

function crearTablaCostos(rows, cols) {
  tablaCostos.innerHTML = '';

  const thead = document.createElement('thead');
  const headerRow = document.createElement('tr');
  headerRow.appendChild(document.createElement('th'));

  for (let j = 0; j < cols; j++) {
    const th = document.createElement('th');
    th.textContent = `Destino ${j + 1}`;
    headerRow.appendChild(th);
  }

  const ofertaHead = document.createElement('th');
  ofertaHead.textContent = 'Oferta';
  headerRow.appendChild(ofertaHead);
  thead.appendChild(headerRow);
  tablaCostos.appendChild(thead);

  const tbody = document.createElement('tbody');
  for (let i = 0; i < rows; i++) {
    const row = document.createElement('tr');
    const label = document.createElement('th');
    label.textContent = `Origen ${i + 1}`;
    row.appendChild(label);

    for (let j = 0; j < cols; j++) {
      const cell = document.createElement('td');
      const input = document.createElement('input');
      input.type = 'number';
      input.min = '0';
      input.step = '1';
      input.value = '0';
      input.className = 'cell-input';
      input.dataset.row = i;
      input.dataset.col = j;
      cell.appendChild(input);
      row.appendChild(cell);
    }

    const ofertaCell = document.createElement('td');
    const ofertaInput = document.createElement('input');
    ofertaInput.type = 'number';
    ofertaInput.min = '0';
    ofertaInput.step = '1';
    ofertaInput.value = '0';
    ofertaInput.className = 'cell-input oferta-input';
    ofertaInput.dataset.row = i;
    ofertaCell.appendChild(ofertaInput);
    row.appendChild(ofertaCell);
    tbody.appendChild(row);
  }

  const demandaRow = document.createElement('tr');
  const demandaHeader = document.createElement('th');
  demandaHeader.textContent = 'Demanda';
  demandaRow.appendChild(demandaHeader);

  for (let j = 0; j < cols; j++) {
    const cell = document.createElement('td');
    const input = document.createElement('input');
    input.type = 'number';
    input.min = '0';
    input.step = '1';
    input.value = '0';
    input.className = 'cell-input demanda-input';
    input.dataset.col = j;
    cell.appendChild(input);
    demandaRow.appendChild(cell);
  }

  demandaRow.appendChild(document.createElement('td'));
  tbody.appendChild(demandaRow);
  tablaCostos.appendChild(tbody);

  document.querySelectorAll('.cell-input').forEach(input => {
    input.addEventListener('input', actualizarBalance);
  });
  actualizarBalance();
}

function leerMatrizCostos() {
  const rows = Number(origenesInput.value);
  const cols = Number(destinosInput.value);
  const matriz = Array.from({ length: rows }, () => Array(cols).fill(0));

  tablaCostos.querySelectorAll('input.cell-input').forEach(input => {
    if (input.classList.contains('oferta-input') || input.classList.contains('demanda-input')) return;
    const i = Number(input.dataset.row);
    const j = Number(input.dataset.col);
    matriz[i][j] = Number(input.value) || 0;
  });

  return matriz;
}

function leerVector(selector) {
  const inputs = Array.from(tablaCostos.querySelectorAll(selector));
  return inputs.map(input => Number(input.value) || 0);
}

function actualizarBalance() {
  const oferta = leerVector('.oferta-input');
  const demanda = leerVector('.demanda-input');
  const totalOferta = oferta.reduce((a, b) => a + b, 0);
  const totalDemanda = demanda.reduce((a, b) => a + b, 0);
  balanceInfo.textContent = `Oferta total: ${totalOferta} · Demanda total: ${totalDemanda}`;
  balanceInfo.className = totalOferta === totalDemanda ? 'balance-ok' : 'balance-warning';
}

function validarDatos() {
  const oferta = leerVector('.oferta-input');
  const demanda = leerVector('.demanda-input');
  const totalOferta = oferta.reduce((a, b) => a + b, 0);
  const totalDemanda = demanda.reduce((a, b) => a + b, 0);

  if (totalOferta !== totalDemanda) {
    mostrarAlerta('La oferta total debe ser igual a la demanda total. Ajusta los valores.');
    return false;
  }

  if (oferta.some(v => v < 0) || demanda.some(v => v < 0)) {
    mostrarAlerta('Todos los valores de oferta y demanda deben ser positivos.');
    return false;
  }

  return true;
}

function generarPasoHtml(paso, index) {
  return `
    <div class="paso-card">
      <strong>Paso ${index + 1}:</strong> ${paso.descripcion}
      ${paso.costo !== undefined ? `<div class="detalle">Costo actual: ${paso.costo}</div>` : ''}
      ${paso.matriz ? `<div class="detalle">${renderMatrixHtml(paso.matriz)}</div>` : ''}
    </div>
  `;
}

function renderMatrixHtml(matrix) {
  if (!matrix || matrix.length === 0) return '';
  const rows = matrix.length;
  const cols = matrix[0].length;
  let html = '<table class="resultado-matriz"><tbody>';
  for (let i = 0; i < rows; i++) {
    html += '<tr>';
    for (let j = 0; j < cols; j++) {
      html += `<td>${matrix[i][j] || 0}</td>`;
    }
    html += '</tr>';
  }
  html += '</tbody></table>';
  return html;
}

function mostrarResultado(resultado) {
  const html = `
    <div class="resultado-tarjeta">
      <h2>${resultado.nombre}</h2>
      <p><strong>Costo total:</strong> ${resultado.costoTotal}</p>
      <p><strong>Tiempo estimado:</strong> ${resultado.tiempo} ms</p>
      <div><strong>Matriz de asignación:</strong>${renderMatrixHtml(resultado.matriz)}</div>
    </div>
  `;
  resultadoContainer.innerHTML = html;

  if (resultado.pasos && resultado.pasos.length) {
    pasosContainer.innerHTML = resultado.pasos.map((paso, index) => generarPasoHtml(paso, index)).join('');
  } else {
    pasosContainer.innerHTML = '<p>No hay pasos intermedios disponibles.</p>';
  }
}

function solveNorthwest(costos, oferta, demanda) {
  const filas = costos.length;
  const cols = costos[0].length;
  const matriz = Array.from({ length: filas }, () => Array(cols).fill(0));
  const ofertaRestante = oferta.slice();
  const demandaRestante = demanda.slice();
  const pasos = [];
  let i = 0;
  let j = 0;

  while (i < filas && j < cols) {
    const valor = Math.min(ofertaRestante[i], demandaRestante[j]);
    matriz[i][j] = valor;
    ofertaRestante[i] -= valor;
    demandaRestante[j] -= valor;

    pasos.push({
      descripcion: `Asignar ${valor} de Origen ${i + 1} a Destino ${j + 1}`,
      costo: valor * costos[i][j],
      matriz: matriz.map(row => [...row])
    });

    if (ofertaRestante[i] === 0) i += 1;
    if (demandaRestante[j] === 0) j += 1;
  }

  return {
    nombre: 'Esquina Noroeste',
    matriz,
    costoTotal: calcularCostoTotal(matriz, costos),
    pasos,
    tiempo: 0
  };
}

function solveMinimumCost(costos, oferta, demanda) {
  const filas = costos.length;
  const cols = costos[0].length;
  const matriz = Array.from({ length: filas }, () => Array(cols).fill(0));
  const ofertaRestante = oferta.slice();
  const demandaRestante = demanda.slice();
  const pasos = [];
  const indices = [];

  for (let i = 0; i < filas; i++) {
    for (let j = 0; j < cols; j++) {
      indices.push({ i, j, costo: costos[i][j] });
    }
  }

  indices.sort((a, b) => a.costo - b.costo);

  for (const item of indices) {
    if (ofertaRestante[item.i] === 0 || demandaRestante[item.j] === 0) continue;
    const valor = Math.min(ofertaRestante[item.i], demandaRestante[item.j]);
    matriz[item.i][item.j] = valor;
    ofertaRestante[item.i] -= valor;
    demandaRestante[item.j] -= valor;

    pasos.push({
      descripcion: `Asignar ${valor} de Origen ${item.i + 1} a Destino ${item.j + 1} (costo ${item.costo})`,
      costo: valor * item.costo,
      matriz: matriz.map(row => [...row])
    });
  }

  return {
    nombre: 'Costo Mínimo',
    matriz,
    costoTotal: calcularCostoTotal(matriz, costos),
    pasos,
    tiempo: 0
  };
}

function solveVogel(costos, oferta, demanda) {
  const filas = costos.length;
  const cols = costos[0].length;
  const matriz = Array.from({ length: filas }, () => Array(cols).fill(0));
  const ofertaRestante = oferta.slice();
  const demandaRestante = demanda.slice();
  const eliminadoFila = Array(filas).fill(false);
  const eliminadoCol = Array(cols).fill(false);
  const pasos = [];

  for (let iter = 0; iter < filas + cols - 1; iter++) {
    const penalidadesFilas = Array(filas).fill(-1);
    const penalidadesCols = Array(cols).fill(-1);

    for (let i = 0; i < filas; i++) {
      if (eliminadoFila[i] || ofertaRestante[i] === 0) continue;
      const costosValidos = [];
      for (let j = 0; j < cols; j++) {
        if (!eliminadoCol[j] && demandaRestante[j] > 0) costosValidos.push(costos[i][j]);
      }
      if (costosValidos.length >= 2) {
        costosValidos.sort((a, b) => a - b);
        penalidadesFilas[i] = costosValidos[1] - costosValidos[0];
      } else if (costosValidos.length === 1) {
        penalidadesFilas[i] = costosValidos[0];
      }
    }

    for (let j = 0; j < cols; j++) {
      if (eliminadoCol[j] || demandaRestante[j] === 0) continue;
      const costosValidos = [];
      for (let i = 0; i < filas; i++) {
        if (!eliminadoFila[i] && ofertaRestante[i] > 0) costosValidos.push(costos[i][j]);
      }
      if (costosValidos.length >= 2) {
        costosValidos.sort((a, b) => a - b);
        penalidadesCols[j] = costosValidos[1] - costosValidos[0];
      } else if (costosValidos.length === 1) {
        penalidadesCols[j] = costosValidos[0];
      }
    }

    let maxPenalidad = -1;
    let filaSeleccionada = -1;
    let colSeleccionada = -1;
    let seleccionarFila = true;

    for (let i = 0; i < filas; i++) {
      if (penalidadesFilas[i] !== -1 && penalidadesFilas[i] > maxPenalidad) {
        maxPenalidad = penalidadesFilas[i];
        filaSeleccionada = i;
        seleccionarFila = true;
      }
    }
    for (let j = 0; j < cols; j++) {
      if (penalidadesCols[j] !== -1 && penalidadesCols[j] > maxPenalidad) {
        maxPenalidad = penalidadesCols[j];
        colSeleccionada = j;
        seleccionarFila = false;
      }
    }

    if (seleccionarFila) {
      let mejorCol = -1;
      let mejorCosto = Infinity;
      for (let j = 0; j < cols; j++) {
        if (!eliminadoCol[j] && demandaRestante[j] > 0 && costos[filaSeleccionada][j] < mejorCosto) {
          mejorCosto = costos[filaSeleccionada][j];
          mejorCol = j;
        }
      }
      if (mejorCol === -1) break;
      const valor = Math.min(ofertaRestante[filaSeleccionada], demandaRestante[mejorCol]);
      matriz[filaSeleccionada][mejorCol] = valor;
      ofertaRestante[filaSeleccionada] -= valor;
      demandaRestante[mejorCol] -= valor;
      if (ofertaRestante[filaSeleccionada] === 0) eliminadoFila[filaSeleccionada] = true;
      if (demandaRestante[mejorCol] === 0) eliminadoCol[mejorCol] = true;
      pasos.push({
        descripcion: `Iteración ${iter + 1}: asignar ${valor} de Origen ${filaSeleccionada + 1} a Destino ${mejorCol + 1} (fila)`,
        costo: valor * mejorCosto,
        matriz: matriz.map(row => [...row])
      });
    } else {
      let mejorFila = -1;
      let mejorCosto = Infinity;
      for (let i = 0; i < filas; i++) {
        if (!eliminadoFila[i] && ofertaRestante[i] > 0 && costos[i][colSeleccionada] < mejorCosto) {
          mejorCosto = costos[i][colSeleccionada];
          mejorFila = i;
        }
      }
      if (mejorFila === -1) break;
      const valor = Math.min(ofertaRestante[mejorFila], demandaRestante[colSeleccionada]);
      matriz[mejorFila][colSeleccionada] = valor;
      ofertaRestante[mejorFila] -= valor;
      demandaRestante[colSeleccionada] -= valor;
      if (ofertaRestante[mejorFila] === 0) eliminadoFila[mejorFila] = true;
      if (demandaRestante[colSeleccionada] === 0) eliminadoCol[colSeleccionada] = true;
      pasos.push({
        descripcion: `Iteración ${iter + 1}: asignar ${valor} de Origen ${mejorFila + 1} a Destino ${colSeleccionada + 1} (columna)`,
        costo: valor * mejorCosto,
        matriz: matriz.map(row => [...row])
      });
    }
  }

  return {
    nombre: 'Aproximación de Vogel',
    matriz,
    costoTotal: calcularCostoTotal(matriz, costos),
    pasos,
    tiempo: 0
  };
}

function solveMODI(initialMatriz, costos, oferta, demanda, maxIter = 50) {
  // MODI optimization: improve an initial basic feasible solution
  const filas = costos.length;
  const cols = costos[0].length;
  let matriz = initialMatriz.map(row => row.map(v => v));

  function getBasicCells(mat) {
    const basics = [];
    for (let i = 0; i < filas; i++) {
      for (let j = 0; j < cols; j++) {
        if (mat[i][j] > 0) basics.push([i, j]);
      }
    }
    return basics;
  }

  function computeUV(basics) {
    const u = Array(filas).fill(null);
    const v = Array(cols).fill(null);
    u[0] = 0;
    let changed = true;
    while (changed) {
      changed = false;
      for (const [i, j] of basics) {
        if (u[i] !== null && v[j] === null) { v[j] = costos[i][j] - u[i]; changed = true; }
        if (v[j] !== null && u[i] === null) { u[i] = costos[i][j] - v[j]; changed = true; }
      }
    }
    return { u, v };
  }

  function reducedCosts(u, v) {
    const deltas = [];
    for (let i = 0; i < filas; i++) {
      for (let j = 0; j < cols; j++) {
        if (matriz[i][j] === 0) {
          const ui = u[i] ?? 0;
          const vj = v[j] ?? 0;
          deltas.push({ i, j, delta: costos[i][j] - (ui + vj) });
        }
      }
    }
    return deltas;
  }

  function findCycle(startI, startJ) {
      // BFS to find an alternating cycle (row -> col -> row ...)
      const basics = new Set(getBasicCells(matriz).map(x => x.join(',')));
      basics.add([startI, startJ].join(','));

      function rowNeighbors(i, j) {
        const out = [];
        for (let jj = 0; jj < cols; jj++) if (jj !== j && basics.has([i, jj].join(','))) out.push([i, jj]);
        return out;
      }
      function colNeighbors(i, j) {
        const out = [];
        for (let ii = 0; ii < filas; ii++) if (ii !== i && basics.has([ii, j].join(','))) out.push([ii, j]);
        return out;
      }

      const queue = [];
      // state: i, j, byRow (next move type), path
      queue.push({ i: startI, j: startJ, byRow: true, path: [[startI, startJ]] });
      const seen = new Set();

      while (queue.length) {
        const state = queue.shift();
        const key = state.i + ',' + state.j + ',' + state.byRow + ',' + state.path.length;
        if (seen.has(key)) continue;
        seen.add(key);

        const neigh = state.byRow ? rowNeighbors(state.i, state.j) : colNeighbors(state.i, state.j);
        for (const [ni, nj] of neigh) {
          // if we close cycle and length >=4 and even
          if (ni === startI && nj === startJ && state.path.length >= 3) {
            const cycle = state.path.concat([[ni, nj]]);
            if (cycle.length % 2 === 0 && cycle.length >= 4) return cycle;
          }
          // avoid revisiting same cell in current path
          if (state.path.some(p => p[0] === ni && p[1] === nj)) continue;
          const newPath = state.path.concat([[ni, nj]]);
          queue.push({ i: ni, j: nj, byRow: !state.byRow, path: newPath });
        }
      }
      return null;
  }

  function adjustCycle(cycle) {
    // cycle is sequence of coords, ensure even length and alternate + - starting at 0 as +
    const positions = cycle.map(([i, j]) => [i, j]);
    // find theta = min allocation on negative positions (odd indices)
    let theta = Infinity;
    for (let idx = 1; idx < positions.length; idx += 2) {
      const [i, j] = positions[idx];
      theta = Math.min(theta, matriz[i][j]);
    }
    for (let idx = 0; idx < positions.length; idx++) {
      const [i, j] = positions[idx];
      if (idx % 2 === 0) matriz[i][j] += theta; else matriz[i][j] -= theta;
    }
    // clean tiny negatives to zero
    for (let i = 0; i < filas; i++) for (let j = 0; j < cols; j++) if (Math.abs(matriz[i][j]) < 1e-9) matriz[i][j] = 0;
  }

  let iter = 0;
  while (iter < maxIter) {
    const basics = getBasicCells(matriz);
    if (basics.length < filas + cols - 1) {
      // degeneracy: add a very small epsilon to some zero to create basis
      outer: for (let i = 0; i < filas; i++) {
        for (let j = 0; j < cols; j++) {
          if (matriz[i][j] === 0) { matriz[i][j] = 1e-6; break outer; }
        }
      }
    }
    const { u, v } = computeUV(getBasicCells(matriz));
    const deltas = reducedCosts(u, v);
    deltas.sort((a, b) => a.delta - b.delta);
    if (deltas.length === 0 || deltas[0].delta >= -1e-9) break;
    const enter = deltas[0];
    const cycle = findCycle(enter.i, enter.j);
    if (!cycle) break;
    adjustCycle(cycle);
    iter++;
  }

  return {
    nombre: 'MODI (opt.)',
    matriz,
    costoTotal: calcularCostoTotal(matriz, costos),
    pasos: [],
    tiempo: 0
  };
}

function calcularCostoTotal(matriz, costos) {
  let total = 0;
  for (let i = 0; i < matriz.length; i++) {
    for (let j = 0; j < matriz[i].length; j++) {
      total += matriz[i][j] * costos[i][j];
    }
  }
  return total;
}

function resolverProblema() {
  mostrarAlerta('');
  if (!validarDatos()) return;

  const costos = leerMatrizCostos();
  const oferta = leerVector('.oferta-input');
  const demanda = leerVector('.demanda-input');
  const metodo = metodoSelect.value;

  let resultado;
  const start = performance.now();

  if (metodo === 'esquina') resultado = solveNorthwest(costos, oferta, demanda);
  else if (metodo === 'costo') resultado = solveMinimumCost(costos, oferta, demanda);
  else if (metodo === 'vogel') resultado = solveVogel(costos, oferta, demanda);
  else if (metodo === 'modi') {
    // Build initial feasible solution using Vogel then optimize with MODI
    const inicial = solveVogel(costos, oferta, demanda);
    resultado = solveMODI(inicial.matriz, costos, oferta, demanda);
  } else resultado = solveVogel(costos, oferta, demanda);

  resultado.tiempo = Math.round(performance.now() - start);
  mostrarResultado(resultado);
}

function exportCSV() {
  const costos = leerMatrizCostos();
  const oferta = leerVector('.oferta-input');
  const demanda = leerVector('.demanda-input');
  if (!costos.length) { mostrarAlerta('Genera la matriz antes de exportar.'); return; }
  const filas = costos.length;
  const cols = costos[0].length;
  let csv = '';
  // header
  csv += ',' + Array.from({ length: cols }, (_, j) => `Destino ${j + 1}`).join(',') + ',Oferta\n';
  for (let i = 0; i < filas; i++) {
    csv += `Origen ${i + 1},` + costos[i].map(v => v ?? 0).join(',') + ',' + (oferta[i] ?? 0) + '\n';
  }
  csv += 'Demanda,' + (demanda.map(d => d ?? 0).join(',')) + ',\n';

  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url; a.download = 'optimizador_matriz.csv'; document.body.appendChild(a); a.click(); a.remove(); URL.revokeObjectURL(url);
}

async function pasteFromClipboard() {
  try {
    const text = await navigator.clipboard.readText();
    if (!text) { mostrarAlerta('El portapapeles está vacío o no contiene texto.'); return; }

    const rows = text.trim().split(/\r?\n/).map(r => r.split(/\t|,/).map(c => c.trim()));
    // detect exported format: header row starts with empty or 'Destino' and last row starts with 'Demanda'
    const first = rows[0][0].toLowerCase();
    const last = rows[rows.length - 1][0].toLowerCase();
    if (last.includes('demanda')) {
      const cols = rows[0].length - 2; // first empty, destinations..., oferta
      const origenes = rows.length - 2; // minus header and demanda
      origenesInput.value = origenes;
      destinosInput.value = cols;
      crearTablaCostos(origenes, cols);
      // fill matrix and oferta
      for (let i = 0; i < origenes; i++) {
        const row = rows[i + 1];
        for (let j = 0; j < cols; j++) {
          const selector = `input.cell-input[data-row="${i}"][data-col="${j}"]`;
          const inp = document.querySelector(selector);
          if (inp) inp.value = Number(row[j + 1]) || 0;
        }
        // oferta is last column
        const ofertaVal = Number(row[row.length - 1]) || 0;
        const ofertaInputEl = document.querySelector(`input.oferta-input[data-row="${i}"]`);
        if (ofertaInputEl) ofertaInputEl.value = ofertaVal;
      }
      // demanda row
      const demandaRow = rows[rows.length - 1];
      for (let j = 0; j < cols; j++) {
        const demVal = Number(demandaRow[j + 1]) || 0;
        const demInput = document.querySelector(`input.demanda-input[data-col="${j}"]`);
        if (demInput) demInput.value = demVal;
      }
      actualizarBalance();
      mostrarAlerta('Pegado desde portapapeles con formato detectado.');
    } else {
      mostrarAlerta('Formato no reconocido. Pega la tabla exportada desde la interfaz (Exportar CSV) o una tabla con encabezados similares.');
    }
  } catch (err) {
    mostrarAlerta('Error leyendo portapapeles: ' + (err.message || err));
  }
}

generarBtn.addEventListener('click', () => {
  crearTablaCostos(Number(origenesInput.value), Number(destinosInput.value));
  mostrarAlerta('');
});
resolverBtn.addEventListener('click', resolverProblema);
exportBtn && exportBtn.addEventListener('click', exportCSV);
pasteBtn && pasteBtn.addEventListener('click', pasteFromClipboard);
crearTablaCostos(Number(origenesInput.value), Number(destinosInput.value));
