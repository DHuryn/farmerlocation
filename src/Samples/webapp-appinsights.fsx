#r @"..\Farmer\bin\Debug\netstandard2.0\Farmer.dll"

open Farmer
open Helpers

let template =
    let myWebApp = webApp {
        name "mysuperwebapp"
        service_plan_name "myserverfarm"
        sku WebApp.Sku.F1
        app_insights_name "myappinsights"
    }

    arm {
        location Helpers.Locations.NorthEurope
        resource myWebApp
    }

template
|> Writer.toJson
|> Writer.toFile @"webapp-appinsights.json"
