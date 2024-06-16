let memory = 0;
let currentOperation = '';
let displayValue = '';


function initCalculator() {
  displayValue = '0';
  updateDisplay();
  memory = 0;
  currentOperation = '';
}

function clearDisplay() {
  displayValue = '0';
  updateDisplay();
}

function updateDisplay() {
  document.getElementById('display').value = displayValue;
}

function appendNumber(number) {
  // Limit the number of characters in the display to 20
  if (displayValue.length < 20) {
    if (displayValue === '0') {
      if (number === '00') {
        number = '0';
      }
      displayValue = number;
    } else {
      displayValue += number;
    }
    updateDisplay();
  }
}

function appendOperation(operation) {
  if (displayValue !== '') {
    displayValue += operation;
    updateDisplay();
  }
}

function calculateResult() {
  try {
    displayValue = eval(displayValue).toString();
    updateDisplay();
  } catch (e) {
    alert('Invalid expression');
  }
}

function memoryAdd() {
  memory += parseFloat(displayValue);
  clearDisplay();
}

function memorySubtract() {
  memory -= parseFloat(displayValue);
  clearDisplay();
}

function memoryClear() {
  displayValue = memory.toString();
  updateDisplay();
}

function clearAll() {
  memory = 0;
  currentOperation = '';
  clearDisplay();
}