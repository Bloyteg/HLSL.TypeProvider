namespace Bloyteg.HLSL.TypeProvider.Test

open NUnit.Framework
open FsUnit

open Template

[<TestFixture>]
module TemplateTests =

    [<Test>]
    let ``&>>> should combine two values`` () =
        let result = !!("Foo" &>>> "Bar")
        result |> should equal "FooBar"

    [<Test>]
    let ``&>> should combine a template with a value`` () =
        let template = Template.Lift("Foo")
        let result = !!(template &>> "Bar")
        result |> should equal "FooBar"

    [<Test>]
    let ``&>> should combine a value with a template`` () =
        let template = Template.Lift("Bar")
        let result = !!("Foo" &>> template)
        result |> should equal "FooBar"

    [<Test>]
    let ``&>> should combine two templates`` () =
        let template1 = Template.Lift("Foo")
        let template2 = Template.Lift("Bar")
        let result = !!(template1 &>> template2)
        result |> should equal "FooBar"

    [<Test>]
    let ``&|> should unwrap a template and apply it to a function`` () =
        let template = Template.Lift("Foo")
        let result = template &|> Some

        result |> should equal (Some("Foo"))

    [<Test>]
    let ``yield in template workflow should produce the given value`` () =
        template { yield "Foo" } &|> should equal "Foo"

    [<Test>]
    let ``multiple yields in template workflow should produce the given value`` () =
        template { yield "Foo"; yield "Bar" } &|> should equal "FooBar"

    [<Test>]
    let ``yield! in template workflow should produce the given value`` () =
        let foo = template { yield "Foo" }
        template { yield! foo } &|> should equal "Foo"

    [<Test>]
    let ``for in template workflow should produce the given value`` () =
        template { for i in 0..9 do yield i } &|> should equal "0123456789"