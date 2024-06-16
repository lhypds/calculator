let memory = 0;
let currentOperation = '';
let displayValue = '';

function clearDisplay() {
    displayValue = '';
    updateDisplay();
}

function updateDisplay() {
    document.getElementById('display').value = displayValue;
}

function appendNumber(number) {
    displayValue += number;
    updateDisplay();
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