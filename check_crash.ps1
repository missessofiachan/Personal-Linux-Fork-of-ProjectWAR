Get-EventLog -LogName Application -EntryType Error -Newest 10 | Where-Object { $_.Source -match 'Application Error|Windows Error|WAR' } | Select-Object TimeGenerated, Source, Message | Format-List
