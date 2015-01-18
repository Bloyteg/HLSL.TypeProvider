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

module Template

open System

type Template = Template of (Text.StringBuilder -> unit)
    with
        static member inline Lift(value: 'T) = 
            Template(fun builder -> builder.Append value |> ignore)

        static member inline (-->) (Template lhs, Template rhs) = 
            Template(fun builder -> builder |> lhs |> ignore
                                    builder |> rhs  |> ignore)

        static member inline (-->) (lhs : Template, rhs : 'T) = 
            lhs --> Template.Lift(rhs)

        static member inline (-->) (lhs : 'T, rhs : Template) = 
            Template.Lift(lhs) --> rhs

        static member inline (|>>) (lhs : Template, func : string -> 'T) : 'T =
            !!lhs |> func

        static member (!!) (Template template) : string =
            let builder = Text.StringBuilder()
            do template builder
            builder.ToString()

let inline (!%) (lhs : 'S) : Template = 
    (lhs |> Template.Lift)

type TemplateBuilder() =
    member __.Yield(value : 'T) : Template = 
        Template.Lift(value)

    member __.YieldFrom(template : Template) = 
        template 

    member __.Combine(firstTemplate : Template, secondTemplate : Template) : Template =
        firstTemplate --> secondTemplate
    
    member __.Delay (template : unit -> Template) : Template =
        Template(fun builder -> 
                     let (Template template) = (template ())
                     template builder)
    
    member __.Zero() : Template =
        Template(fun _ -> ())
    
    member __.For(items : 'T seq, template : 'T -> Template) : Template =
        Template(fun builder ->
                     use enumerator = items.GetEnumerator ()
                     while enumerator.MoveNext() do
                         let (Template template) = template enumerator.Current
                         template builder)
                            
    member __.While(predicate : unit -> bool, Template template) : Template =
        Template(fun builder -> while predicate () do 
                                    template builder)
                    
let template = new TemplateBuilder()