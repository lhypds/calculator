
Calculator
==========

A simple calculator app that can work on Linux, macOS and Windows.  
The backend part, also can run as a stand alone web application.  


Dependencies
------------

.NET 8, https://dotnet.microsoft.com/en-us/download/dotnet/8.0  
Electron, https://www.electronjs.org/docs  


Build
-----

`npm install`  
`npm run build`  


Run
---

Method 1. `npm start`  

Method 2. Build the executable.  
The built executables are located in the publish folder, e.g., Calculator-win32-x64.  
On windows, run `Calculator.exe`  
Press, `ESC` to quit application.  

Method 3. Run with a web browser  
`cd CalculatorAppDotNet` then `dotnet run`.  
Then you can access from `localhost:5000`.  
