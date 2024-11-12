open System
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open MySql.Data.MySqlClient
open Ply

let connectionString = "Server=localhost;Port=3306;Database=my_database;User=my_user;Password=my_password;"

let insertData (name: string) =
    uply {
        use connection = new MySqlConnection(connectionString)
        do! connection.OpenAsync().AsTask()
        let commandText = "INSERT INTO mytable (name) VALUES (@name)"
        use command = new MySqlCommand(commandText, connection)
        command.Parameters.AddWithValue("@name", name) |> ignore
        do! command.ExecuteNonQueryAsync().AsTask()
    }

let webApp =
    choose [
        POST >=> route "/insert" >=> fun next ctx ->
            task {
                let! body = ctx.ReadBodyAsStringAsync()
                do! insertData body
                return! text "Data inserted successfully." next ctx
            }
    ]

let configureApp (app: IApplicationBuilder) =
    app.UseCors(fun options -> options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() |> ignore)
    app.UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main argv =
    Host.CreateDefaultBuilder(argv)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(configureApp)
                .ConfigureServices(configureServices)
                |> ignore)
        .Build()
        .Run()
    0