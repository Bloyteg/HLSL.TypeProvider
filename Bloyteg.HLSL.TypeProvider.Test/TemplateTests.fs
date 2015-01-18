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
module Bloyteg.HLSL.TypeProvider.Test.TemplateTests

open NUnit.Framework
open FsUnit
open Bloyteg.HLSL.TypeProvider.Internals.Template

[<Test>]
let ``!% should turn a value into a template`` () =
    !%"Foo" |>> should equal "Foo"

[<Test>]
let ``!! should apply template`` () =
    !!(!%"Foo") |> should equal "Foo"

[<Test>]
let ``--> should combine a template with a value`` () =
    let template = !%"Foo"
    (template --> "Bar") |>> should equal "FooBar"

[<Test>]
let ``--> should combine a value with a template`` () =
    let template = !%"Bar"
    ("Foo" --> template) |>> should equal "FooBar"

[<Test>]
let ``--> should combine two templates`` () =
    let template1 = !%"Foo"
    let template2 = !%"Bar"
    (template1 --> template2) |>> should equal "FooBar"

[<Test>]
let ``&|> should unwrap a template and apply it to a function`` () =
    let template = !%"Foo"
    template |>> Some |> should equal (Some("Foo"))

[<Test>]
let ``yield in template workflow should produce the given value`` () =
    template { yield "Foo" } |>> should equal "Foo"

[<Test>]
let ``multiple yields in template workflow should produce the given value`` () =
    template { yield "Foo"; yield "Bar" } |>> should equal "FooBar"

[<Test>]
let ``yield! in template workflow should produce the given value`` () =
    let foo = template { yield "Foo" }
    template { yield! foo } |>> should equal "Foo"

[<Test>]
let ``for in template workflow should produce the given value`` () =
    template { for i in 0..9 do yield i } |>> should equal "0123456789"