const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const path = require('path');

let mainWindow;

function createWindow() {
    mainWindow = new BrowserWindow({
        width: 800,
        height: 600,
        title: "Calculator",
        titleBarStyle: 'hidden',
        resizable: false,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: true,
            contextIsolation: false
        },
        frame: false,  // No frame to make the whole window draggable
    });

    // Load the URL served by the .NET backend
    mainWindow.loadURL('http://localhost:5000');
}

app.whenReady().then(() => {
    // Start the .NET backend application
    const dotnetProcess = spawn('dotnet', ['CalculatorAppDotNet.dll'], { cwd: path.join(__dirname, 'CalculatorAppDotNet') });

    dotnetProcess.stdout.on('data', (data) => {
        console.log(`.NET: ${data}`);
    });

    dotnetProcess.stderr.on('data', (data) => {
        console.error(`.NET Error: ${data}`);
    });

    dotnetProcess.on('close', (code) => {
        console.log(`.NET process exited with code ${code}`);
    });

    // Delay to wait for the .NET backend to be ready
    setTimeout(createWindow, 5000);
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
        createWindow();
    }
});