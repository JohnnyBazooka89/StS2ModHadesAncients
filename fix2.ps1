$uids = @{}

Get-ChildItem -Recurse -File |
    Where-Object {
        $_.FullName -notlike "*\.godot\*" -and
        $_.FullName -notlike "*\.uid_backup\*" -and
        $_.Extension -in ".uid",".import",".tscn",".tres",".material",".gdshader",".scn"
    } |
    ForEach-Object {
        $file = $_.FullName
        $content = Get-Content $file -Raw -ErrorAction SilentlyContinue

        if ($null -eq $content) { return }

        $matches = [regex]::Matches($content, 'uid://[a-zA-Z0-9]+')

        foreach ($m in $matches) {
            $uid = $m.Value

            if (-not $uids.ContainsKey($uid)) {
                $uids[$uid] = @()
            }

            $uids[$uid] += $file
        }
    }

$uids.GetEnumerator() |
    Where-Object { ($_.Value | Select-Object -Unique).Count -gt 1 } |
    ForEach-Object {
        Write-Host ""
        Write-Host "Duplicate UID: $($_.Key)"
        $_.Value | Select-Object -Unique | ForEach-Object {
            Write-Host "  $_"
        }
    }