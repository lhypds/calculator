const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const path = require('path');

let mainWindow;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 440,
    height: 510,
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
  setTimeout(createWindow, 3000);

  app.on('activate', function () {
    // On macOS it's common to re-create a window in the app when the dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

// Quit when all windows are closed, except on macOS. There, it's common for applications and their menu bar to stay active until the user quits explicitly with Cmd + Q.
app.on('window-all-closed', function () {
  if (process.platform !== 'darwin') app.quit();
});

app.on('activate', function () {
  // On macOS it's common to re-create a window in the app when the dock icon is clicked and there are no other windows open.
  if (BrowserWindow.getAllWindows().length === 0) createWindow();
});