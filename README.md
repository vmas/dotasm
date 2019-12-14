# DotAsm
IL Tools for .NET Core

# Setup
The project was created as a global CLI tool, therefore you can install with a single command:

`dotnet tool install -g DotAsm`

Note that for the command above to work, you need .NET Core SDK 2.1.300 or above installed in your machine.

# Syntax
```
dotnet asm ilasm [options] filename [[options]filename...]
dotnet asm ildasm [options] [PEfilename] [options]
```

# Examples
The following command causes the metadata and disassembled code for the PE file hello.exe to display in the ILDAsm default GUI.
```
dotnet asm ildasm path_to/hello.exe
```

The following command assembles the IL file hello.il and produces the executable hello.exe.
```
dotnet asm ilasm path_to/hello.il
```

# Binary source
Microsoft.NETCore.ILAsm:
https://dotnet.myget.org/feed/dotnet-core/package/nuget/Microsoft.NETCore.ILAsm
Microsoft.NETCore.ILDAsm:
https://dotnet.myget.org/feed/dotnet-core/package/nuget/Microsoft.NETCore.ILDAsm
