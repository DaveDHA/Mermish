namespace Mermish

// UNTESTED

open System.Runtime.CompilerServices

[<Extension>]
type GanttChartExtensions =

    [<Extension>]
    static member inline WithTitle(chart : GanttChart.GanttChart, title) = GanttChart.WithTitle title chart

    // TODO: Fill this out for the C# clients