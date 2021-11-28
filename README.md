# BasicEC.Secret

This is a pet project that provides a tool to encrypt and decrypt files.

## Usage

There's only one user interface for now which is represented by console:

```shell
$ ./BasicEC.Secret.Console --help
BasicEC.Secret.Console 0.3.0-alpha
Copyright (C) 2021 BasicEC.Secret.Console

  decrypt    Decrypt file.

  encrypt    Encrypt file.

  key        Key management commands.

  help       Display more information on a specific command.

  version    Display version information.
```

You could use `--help` option after any command to display additional information:
```shell
$ ./BasicEC.Secret.Console key --help
...
```
You can also use it after the chain of words:
```shell
$ ./BasicEC.Secret.Console key gen --help
...
```

Example of the encryption:
```shell
$ ./BasicEC.Secret.Console key gen -n test
[21:10:37 INF] Create store for key test
$ ./BasicEC.Secret.Console encrypt -f file.txt -o encrypted.txt -k test
Progress: 100.0%
```

By default, all keys are stored in `rsa_store` directory next to the application file. You could change this place by setting `BASIC_RSA_STORE` environment variable.

## Build

### Linux

```shell
$ dotnet publish BasicEC.Secret.Console -r linux-x64 -c Release -p:PublishSingleFile=true
```

### Windows
```shell
> dotnet publish BasicEC.Secret.Console -r win-x64 -c Release -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```