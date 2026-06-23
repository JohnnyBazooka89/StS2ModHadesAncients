Write-Host "Scanning .import files..."
$seenImportUids = @{}
$duplicateImportFiles = @()

Get-ChildItem -Path $projectRoot -Recurse -Filter "*.import" | ForEach-Object {
    $file = $_.FullName
    $content = Get-Content $file -Raw

    if ($content -match '(?m)^uid="([^"]+)"') {
        $uid = $matches[1]

        if ($seenImportUids.ContainsKey($uid)) {
            $duplicateImportFiles += $file
            Write-Host "Duplicate .import UID:"
            Write-Host "  First:     $($seenImportUids[$uid])"
            Write-Host "  Duplicate: $file"
        } else {
            $seenImportUids[$uid] = $file
        }
    }
}

foreach ($file in $duplicateImportFiles) {
    $content = Get-Content $file -Raw
    $content = $content -replace '(?m)^uid="[^"]+"\r?\n?', ''
    Set-Content -Path $file -Value $content -NoNewline
}

Write-Host ""
Write-Host "Scanning .uid files..."
$seenUidValues = @{}
$duplicateUidFiles = @()

Get-ChildItem -Path $projectRoot -Recurse -Filter "*.uid" | ForEach-Object {
    $file = $_.FullName
    $uid = (Get-Content $file -Raw).Trim()

    if ([string]::IsNullOrWhiteSpace($uid)) {
        return
    }

    if ($seenUidValues.ContainsKey($uid)) {
        $duplicateUidFiles += $file
        Write-Host "Duplicate .uid:"
        Write-Host "  UID:       $uid"
        Write-Host "  First:     $($seenUidValues[$uid])"
        Write-Host "  Duplicate: $file"
    } else {
        $seenUidValues[$uid] = $file
    }
}

foreach ($file in $duplicateUidFiles) {
    Write-Host "Deleting duplicate .uid file:" $file
    Remove-Item $file -Force
}

Write-Host ""
Write-Host "Done."
Write-Host "Removed UID lines from duplicate .import files: $($duplicateImportFiles.Count)"
Write-Host "Deleted duplicate .uid files: $($duplicateUidFiles.Count)"
Write-Host "Now reopen Godot and let it regenerate/reimport."