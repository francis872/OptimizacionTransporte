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
  else resultado = solveVogel(costos, oferta, demanda);

  resultado.tiempo = Math.round(performance.now() - start);
  mostrarResultado(resultado);
}

generarBtn.addEventListener('click', () => {
  crearTablaCostos(Number(origenesInput.value), Number(destinosInput.value));
  mostrarAlerta('');
});
resolverBtn.addEventListener('click', resolverProblema);
crearTablaCostos(Number(origenesInput.value), Number(destinosInput.value));
