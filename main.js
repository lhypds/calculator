const { app, BrowserWindow } = require('electron');
const path = require('path');
const axios = require('axios');

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
        icon: path.join(__dirname, 'build/icons/icon.png')
    });

    // Load the URL served by the .NET backend
    mainWindow.loadURL('http://localhost:5000');
}

async function tryConnectToDotNetBackend() {
    try {
        const response = await axios.get('http://localhost:5000/health');
        if (response.status === 200) {
            console.log('Connected to .NET backend:', response.data);
            // If the connection is successful, cancel further attempts and proceed
            clearInterval(intervalId);
            createWindow();
        } else {
            console.error('Received non-healthy status:', response.status);
        }
    } catch (error) {
        console.error('Failed to connect to .NET backend:', error.message);
    }
}

app.whenReady().then(() => {
    intervalId = setInterval(tryConnectToDotNetBackend, 500);

    // Set a timeout to stop trying after 10 seconds
    setTimeout(() => {
        clearInterval(intervalId);
        createWindow();
    },10000);
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