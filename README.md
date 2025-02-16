# Config Snapper

Create automatic snapshots of configuration files (or any other file). No more lost configurations files, manual backups and painful rollbacks. Config Snapper helps you to keep track of your configuration changes.

Under the hood Config Snapper uses git to create a snapshot history und uses file system watchers to watch every file change.

Features:

- Create snaphots on application startup or watch file changes
- File change history (through git)
- Create backups of each file change (for easy rollback)
- Open Telemetry support
- Available as standalone console app or as nuget integration

## Configuration

| Property          | Description                                                                                                                                              |
| ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| SnapshotSources   | Key-Values of the files to keep snapshots (and backups) of. The key can be any name. The value is the path to the file.                                  |
| SnapshotDirectory | Relative or absolute path for the snapshot direcotry. Leave `null` create the snapshot directory in the application root directory.                      |
| Watch             | By default Cofig Snapper creates snapshots (and backups) on application startup. With `Watch` set to `true` all file changes are registered immediately. |
| Backup            | When set to `true`, individual file Backups are created. Each backup file receives a timestamp in the filename.                                          |
| BackupDirectory   | Relative or absolute path for file backups. Leave `null` to store Backups in the same direcotroy of the source file.                                     |
| OpenTelemetry     | Open Telemetry is enabled when set to `true`.                                                                                                            |

```json
  "ConfigSnapper": {
    "SnapshotSources": {
      "Test": "C:\\Users\\Snapper\\appSettings.json"
    },
    "SnapshotDirectory": "C:\\Users\\Snapper",
    "Watch": true,
    "Backup": true,
    "BackupDirectory": "C:\\Users\\Snapper",
    "OpenTelemetry":  true
```
