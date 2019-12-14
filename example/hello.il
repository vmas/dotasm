.assembly extern mscorlib {}

.module hello.exe

.method static void Main() cil managed
{
    .maxstack 1
    .entrypoint
    
    ldstr "Hello, World!"
    call void [mscorlib]System.Console::WriteLine (string)
    ret
}