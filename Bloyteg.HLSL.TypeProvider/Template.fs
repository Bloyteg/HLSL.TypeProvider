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

type Template = (Text.StringBuilder -> unit)

let apply template = 
    let builder = Text.StringBuilder()
    do template builder
    builder.ToString()

let inline lift (value: 'T) : Template =
    (fun builder -> builder.Append value |> ignore)

let inline (|>>) (lhs: Template) (rhs: Template) : Template = 
    (fun builder -> builder |> lhs |> ignore
                    builder |> rhs  |> ignore)

let inline (>>.) (lhs: Template) (rhs: 'T) : Template = 
    lhs |>> (rhs |> lift)

let inline (.>>) (lhs: 'T) (rhs: Template) : Template = 
    (lhs |> lift) |>> rhs

let inline (.>>.) (lhs: 'S) (rhs: 'T) : Template = 
    (lhs |> lift) |>> (rhs |> lift)

let inline (||>) (template: Template) func = 
    template |> apply |> func

type TemplateBuilder () =
    member __.Yield (value: 'T) : Template = 
        lift value

    member __.YieldFrom template =
        template : Template

    member __.Combine(firstTemplate: Template, secondTemplate: Template) : Template =
        firstTemplate |>> secondTemplate
    
    member __.Delay (template) : Template =
        (fun builder -> (template ()) builder)
    
    member __.Zero () : Template =
        (fun _ -> ())
    
    member __.For (items : 'T seq, template : 'T -> Template) =
                    (fun builder ->
                         let enumerator = items.GetEnumerator ()
                         while enumerator.MoveNext() do
                             (template enumerator.Current) builder)
                            
    member __.While (predicate : unit -> bool, template : Template) =
                    (fun builder -> while predicate () do 
                                        template builder)
                    
let template = new TemplateBuilder ()