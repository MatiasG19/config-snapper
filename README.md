[![CI](https://github.com/MatiasG19/config-snapper/actions/workflows/ci.yml/badge.svg)](https://github.com/MatiasG19/config-snapper/actions/workflows/ci.yml)

<br />

<div align="center" style="display: flex; justify-content: center; align-items: center;">
  <a href="https://github.com/MatiasG19/config-snapper">
    <img src="logo/logo.svg" alt="Logo" width="80" height="80">
  </a>
</div>

<h3 align="center" style="display: flex; justify-content: center; align-items: center;">Config Snapper</h3>

<div align="center" style="justify-content: center; align-items: center;">
  <p>Create automatic snapshots of configuration files (or any other file) or directories. No more lost configurations, manual backups and painful rollbacks. Config Snapper helps you to keep track of your configuration changes.</p>
  <p>Under the hood Config Snapper uses Git to create a snapshot history und uses file system watchers to watch every file change.</p>
</div>

## Features

- Create snaphots on application startup or watch file changes
- File change history (through git)
- Create backups of each file change (for easy rollback)
- Open Telemetry support
- Available as standalone console app or as nuget integration

## Usage

### Command line

`./ConfigSnapperConsole <optional_path_to_appSettings>`

## Configuration

| Property          | Description                                                                                                                                               |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| SnapshotSources   | Key-Values of the files to keep snapshots (and backups) of. The key can be any name. The value is the path to the file.                                   |
| SnapshotDirectory | Relative or absolute path for the snapshot directory. Leave `null` create the snapshot directory in the application root directory.                       |
| Watch             | By default Config Snapper creates snapshots (and backups) on application startup. With `Watch` set to `true` all file changes are registered immediately. |
| Backup            | When set to `true`, individual file Backups are created. Each backup file receives a timestamp in the filename.                                           |
| BackupDirectory   | Relative or absolute path for file backups. Leave `null` to store Backups in the same directory of the source file.                                       |
| OpenTelemetry     | Open Telemetry is enabled when set to `true`.                                                                                                             |
| GitRemoteUrl      | Git remote url, e.g. `https://github.com/MatiasG19/test-config-snapper.git`.                                                                              |
| GitBranch         | Git remote branch name, default `main`.                                                                                                                   |

**Example configuration:**

```json
  "ConfigSnapper": {
    "SnapshotSourceFiles": {
      "Test": "C:\\Users\\Snapper\\appSettings.json"
    },
    "SnapshotSourceDirectory": null,
    "SnapshotDirectory": "C:\\Users\\Snapper",
    "Watch": true,
    "Backup": true,
    "BackupDirectory": "C:\\Users\\Snapper",
    "OpenTelemetry":  true,
    "GitRemoteUrl": "https://github.com/MatiasG19/my-repo.git",
    "GitBranch": "main"
```

### Git remote repository

> When using a existing remote repository, make sure to resolve merge conflicts. otherwise Config Snapper will not be able to push changes.

#### Store credentials

1. Execute to use GCM to store credentials for repository:

```sh
git config credential.helper store
```

2. Login to GitHub (or whatever) once, by pushing changes once. This way credentials will be stored for future pushes.

```sh
git push origin <branch_name>
```
