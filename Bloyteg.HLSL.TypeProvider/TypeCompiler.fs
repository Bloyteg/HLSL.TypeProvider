module internal TypeCompiler

open System
open System.IO
open System.Reflection
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp

type AssemblyResult =
| Success of byte[]
| Failure of string seq

let compileAssembly (name: string) (references: Assembly seq) (code: string) : AssemblyResult =
    use outputStream = new MemoryStream()
    let syntaxTree = CSharpSyntaxTree.ParseText(code)
    let compilationResult = CSharpCompilation.Create(name,
                                                     syntaxTrees = [syntaxTree],
                                                     references = (references |> Seq.map MetadataReference.CreateFromAssembly),
                                                     options = CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                                             .Emit(outputStream)
    if compilationResult.Success then
        Success(outputStream.GetBuffer())
    else
        Failure(compilationResult.Diagnostics |> Seq.map Convert.ToString)