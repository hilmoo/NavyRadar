$repo = $env:REPO
$tag = $env:TAG
$name = "$repo-$tag.zip"
$invalid = [IO.Path]::GetInvalidFileNameChars()
foreach($c in $invalid) { 
    $name = $name -replace [regex]::Escape($c), '_' 
}
Write-Host 'Archive name:' $name
if(Test-Path $name) { 
    Remove-Item $name -Force 
}
Compress-Archive -Path 'publish\*' -DestinationPath $name -Force
Write-Host 'Created:' (Resolve-Path $name).Path