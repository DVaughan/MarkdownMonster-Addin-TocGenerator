cd "$PSScriptRoot" 

$src = "$env:appdata\Markdown Monster\Addins\TocAddin"

"Cleaning up build files..."
del ..\Build\addin.zip

remove-item -recurse -force ..\Build\Distribution
md ..\Build\Distribution

"Copying files..."
copy $src\*.dll ..\Build\Distribution
copy version.json ..\Build\Distribution
copy version.json ..\Build\

[Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")

function ZipFiles($zipfilename, $sourcedir)
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}

"Zipping up setup file..."

$source = (get-item "$PSScriptRoot").parent.FullName + "\Build\Distribution\*.*"
$zipPath = (get-item "$PSScriptRoot").parent.FullName + "\Build\addin.zip"

Compress-Archive -Path $source -CompressionLevel Optimal -DestinationPath $zipPath