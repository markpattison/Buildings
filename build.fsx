#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"
#load "MonoGameContent.fsx"

open System
open Fake.Core
open Fake.IO.Globbing.Operators
open MonoGameContent

// Directories

let intermediateContentDir = "./intermediateContent/"
let contentDir = "./src/Buildings/"
let buildDir  = "./build/"
let deployDir = "./deploy/"

// Filesets

let appReferences = 
    !! "**/*.fsproj"

let contentFiles =
    !! "**/*.fx"
        ++ "**/*.spritefont"
        ++ "**/*.dds"

// Targets

Target.create "Clean" (fun _ -> 
    Fake.IO.Shell.cleanDirs [buildDir; deployDir]
)

Target.create "BuildContent" (fun _ ->
    contentFiles
        |> MonoGameContent (fun p ->
            { p with
                OutputDir = contentDir;
                IntermediateDir = intermediateContentDir;
            }))

Target.create "BuildApp" (fun _ ->
    appReferences
        |> Fake.DotNet.MSBuild.runRelease id buildDir "Build"
        |> ignore
)

Target.create "RunApp" (fun _ ->
    Fake.Core.Process.fireAndForget (fun info ->
        { info with
            FileName = buildDir + @"Buildings.exe"
            WorkingDirectory = buildDir })
    Fake.Core.Process.setKillCreatedProcesses false)

// Build order

open Fake.Core.TargetOperators

"Clean"
//    ==> "BuildContent"
    ==> "BuildApp"
    ==> "RunApp"

// Start build

Target.runOrDefault "BuildApp"