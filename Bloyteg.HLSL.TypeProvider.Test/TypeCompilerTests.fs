// Copyright 2015 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy otemplate the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS Otemplate ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

[<NUnit.Framework.TestFixture>]
module Bloyteg.HLSL.TypeProvider.TestTypeCompilerTests 

open System
open System.Reflection

open NUnit.Framework
open FsUnit
open Bloyteg.HLSL.TypeProvider.Internals.TypeCompiler

[<Test>]
let ``compileAssembly should return a Success result for valid code`` () =
    let assembly = compileAssembly "Test" [typeof<obj>.Assembly] "public class Test {}"
    let result = match assembly with
                 | Success(_) -> true
                 | Failure(_) -> false

    result |> should equal true

[<Test>]
let ``compileAssembly should return a Failure result for invalid code`` () =
    let assembly = compileAssembly "Test" [typeof<obj>.Assembly] "public Test {}"
    let result = match assembly with
                 | Success(_) -> false
                 | Failure(_) -> true

    result |> should equal true

[<Test>]
let ``The resulting compiled assembly should be valid, executable code`` () =
    let byteCode = match compileAssembly "Test" [typeof<obj>.Assembly] "public class Test { public bool IsSuccess => true; }" with
                   | Success(byteCode) -> byteCode
                   | Failure(_) -> failwith "Failed to compile code."

    let assembly = Assembly.Load(byteCode)
    let testType = assembly.GetType("Test")
    let testObject = Activator.CreateInstance(testType)
    let result = testType.GetProperty("IsSuccess").GetValue(testObject)

    result |> should equal true


