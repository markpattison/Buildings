module Program

open Buildings

[<EntryPoint>]
let Main args =
    let game = new Buildings()
    do game.Run()
    0