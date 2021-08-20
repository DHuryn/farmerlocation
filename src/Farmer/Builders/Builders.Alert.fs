﻿[<AutoOpen>]
module Farmer.Builders.Alert

open Farmer
open Farmer.Insights
open Farmer.Arm.InsightsAlerts

type AlertConfig =
    {   Name : Farmer.ResourceName
        Description : string
        Severity : AlertSeverity
        Frequency : DurationInterval
        Window : DurationInterval
        Actions : List<AlertAction>
        LinkedResources : LinkedResource list
        Criteria : MetricAlertCriteria }
    interface IBuilder with
        member this.ResourceId = metricAlert.resourceId this.Name
        member this.BuildResources _ = 
            let a : AlertData = 
                {   Name = this.Name
                    Description = this.Description
                    Severity = this.Severity
                    Frequency = this.Frequency
                    Window = this.Window
                    Actions = this.Actions
                    LinkedResources = this.LinkedResources |> List.map(fun r -> r.ResourceId)
                    Criteria = this.Criteria }
            [ a ]

type AlertBuilder() =
    member __.Yield _ =
        { Name = ResourceName.Empty
          Description = ""
          Severity = AlertSeverity.Alert_Error
          Frequency = DurationInterval.FiveMinutes
          Window = DurationInterval.FifteenMinutes
          Actions = List.empty
          LinkedResources = List.empty
          Criteria = SingleResourceMultipleMetricCriteria [] }

    [<CustomOperation "name">]
    /// Sets the name of the alert.
    member __.Name(state:AlertConfig, name) = { state with Name = ResourceName name }

    [<CustomOperation "description">]
    /// Sets the description of the alert.
    member __.Description(state:AlertConfig, description) = { state with Description = description }

    [<CustomOperation "frequency">]
    /// How often the metric alert is evaluated
    member __.Frequency(state:AlertConfig, frequency) = { state with Frequency = frequency }

    [<CustomOperation "window">]
    /// The period of time that is used to monitor alert activity based on the threshold.
    member __.Window(state:AlertConfig, window) = { state with Window = window }

    [<CustomOperation "severity">]
    /// Alert severity
    member __.Severity(state:AlertConfig, severity) = { state with Severity = severity }

    [<CustomOperation "add_linked_resources">]
    /// Add the target resources on which the alert is created/updated. 
    member __.LinkedResources(state:AlertConfig, linked_resources) = { state with LinkedResources = linked_resources @ state.LinkedResources }

    [<CustomOperation "add_linked_resource">]
    /// Add target resource on which the alert is created/updated. 
    member this.LinkedResource(state:AlertConfig, linked_resource) = { state with LinkedResources = linked_resource :: state.LinkedResources }
    member this.LinkedResource(state:AlertConfig, resource:ResourceId) = this.LinkedResource(state, resource |> Managed)
    member this.LinkedResource(state:AlertConfig, builder:IBuilder) = this.LinkedResource(state, builder.ResourceId |> Managed)

    [<CustomOperation "criteria">]
    /// The rule criteria that defines the conditions of the alert rule.
    member __.Criteria(state:AlertConfig, criteria) = { state with Criteria = criteria }

    [<CustomOperation "add_action">]
    /// Add an action that are performed when the alert rule becomes active.
    member __.Actions(state:AlertConfig, action) = { state with Actions = action :: state.Actions }

let alert = AlertBuilder()