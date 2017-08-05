cd "$PSScriptRoot" 

$src = "$env:appdata\Markdown Monster\Addins\TocAddin"

"Cleaning up build files..."
del ..\Build\addin.zip

remove-item -recurse -force ..\Build\Distribution
md ..\Build\Distribution

"Copying files..."
copy $src\*.dll ..\Build\Distribution
copy $src\version.json ..\Build\Distribution
copy $src\license.txt ..\Build\Distribution

copy $src\version.json ..\Build
copy $src\icon.png ..\Build
copy screenshot.png ..\Build
copy $src\license.txt ..\Build

"Zipping up setup file..."

$source = (get-item "$PSScriptRoot").parent.FullName + "\Build\Distribution\*.*"
$zipPath = (get-item "$PSScriptRoot").parent.FullName + "\Build\addin.zip"

Compress-Archive -Path $source -CompressionLevel Optimal -DestinationPath $zipPath