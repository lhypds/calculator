const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');
const os = require('os');

const filesToCopy = ['main.js', 'preload.js'];

try {
  // Detect the current platform
  const platform = os.platform();
  console.log(`Detected platform: ${platform}`);

  // Map Node.js platform to electron-packager platform
  const platformMap = {
    win32: 'win32',
    darwin: 'darwin',
    linux: 'linux'
  };

  if (!platformMap[platform]) {
    throw new Error(`Unsupported platform: ${platform}`);
  }

  // Step 1: Publish the .NET application
  console.log('Publishing .NET application...');
  execSync('dotnet publish ./CalculatorAppDotNet -c Release -o ./publish/CalculatorAppDotNet', { stdio: 'inherit' });

  // Step 2: Copy necessary files to the publish directory
  filesToCopy.forEach(file => {
    const srcPath = path.join(__dirname, file);
    const destPath = path.join(__dirname, 'publish', file);

    if (fs.existsSync(srcPath)) {
      console.log(`Copying ${file} to publish directory...`);
      fs.copyFileSync(srcPath, destPath);
    } else {
      console.warn(`Warning: ${file} does not exist and was not copied.`);
    }
  });

  // Step 3: Copy the custom package.json to the publish directory
  const customPackageJsonPath = path.join(__dirname, 'publish-package.json');
  const publishPackageJsonPath = path.join(__dirname, 'publish', 'package.json');
  console.log('Copying custom package.json to publish directory...');
  fs.copyFileSync(customPackageJsonPath, publishPackageJsonPath);

  // Step 4: Verify package.json
  console.log('Verifying package.json...');
  const packageJson = require(publishPackageJsonPath);
  console.log('package.json:', packageJson);

  // Step 5: Run electron-packager
  console.log('Packaging Electron application...');

  // Change the working directory to the 'publish' directory
  const publishDir = path.join(__dirname, 'publish');
  process.chdir(publishDir);

  // Run electron-packager for the detected platform
  console.log(`Packaging for ${platformMap[platform]}...`);
  execSync(`npx electron-packager . Calculator --platform=${platformMap[platform]} --arch=x64 --overwrite --verbose`, { stdio: 'inherit' });

  console.log('Build and packaging complete!');
} catch (error) {
  console.error('Error during build and packaging process:', error);
}