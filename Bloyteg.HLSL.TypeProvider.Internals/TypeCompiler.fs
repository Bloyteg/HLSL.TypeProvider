// Copyright 2015 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

module Bloyteg.HLSL.TypeProvider.Internals.TypeCompiler

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