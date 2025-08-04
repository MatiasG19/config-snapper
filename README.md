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

| Property          | Description                                                                                                                                                                      |
| ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| SnapshotSources   | Key-Values of the files to keep snapshots (and backups) of. The key can be any name. The value is the path to the file.                                                          |
| SnapshotDirectory | Relative or absolute path for the snapshot directory. Leave `null` create the snapshot directory in the application root directory.                                              |
| Watch             | By default Config Snapper creates snapshots (and backups) on application startup. With `Watch` set to `true` all file changes are registered immediately.                        |
| Backup            | When set to `true`, individual file Backups are created. Each backup file receives a timestamp in the filename.                                                                  |
| BackupDirectory   | Relative or absolute path for file backups. Leave `null` to store Backups in the same directory of the source file.                                                              |
| OpenTelemetry     | Open Telemetry is enabled when set to `true`.                                                                                                                                    |
| GitRemoteUrl      | Git remote url, e.g. `https://github.com/MatiasG19/test-config-snapper.git`. Remote can be only added when `.git` repository is initialized, otherwise it has be added manually. |
| GitBranch         | Git remote branch name, default `main`.                                                                                                                                          |

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
    "GitRemoteName": "origin",
    "GitBranchName": "main"
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

## Docker

### Build

`docker build --platform linux/amd64 -t configsnapper .`

### Pull

`docker pull ghcr.io/matiasg19/configsnapper:latest`

### Run

```sh
docker run \
  -v $(pwd)/appSettings.json:/app/appSettings.json \
  -v $(pwd)/logs:/app/logs \
  -v /home/matias/Downloads/ConfigSnapperSnapshots:/app/SnapshotSourceDirectory \
  configsnapper
```

## Install as systemd service

Create service:

```sh
sudo nano /etc/systemd/system/config-snapper.service
```

```sh
[Unit]
Description=Config Snapper at Startup

[Service]
ExecStartPre=/bin/sleep 300 # Delay for 5 minutes
ExecStart=/usr/bin/sudo /path/to/your/config-snapper
Type=oneshot
User=root
TimeoutStartSec=350s

[Install]
WantedBy=multi-user.target

```

Create timer:

```sh
sudo nano /etc/systemd/system/config-snapper.timer
```

```sh
[Unit]
Description=Run Config Snapper every Tuesday at 4 AM

[Timer]
OnCalendar=Tue *-*-* 04:00:00
Persistent=true

[Install]
WantedBy=timers.target
```

Reload daemon:

```sh
sudo systemctl daemon-reload
```

Add script to sudoers, to be able to execute it without password:

```sh
sudo visudo
```

```sh
root ALL=(ALL) NOPASSWD: /path/to/your/config-snapper
```

Enable and verify service:

```sh
sudo systemctl enable config-snapper.service
sudo systemctl status config-snapper.service
```

Test run script:

```sh
sudo systemctl start config-snapper.service
```
