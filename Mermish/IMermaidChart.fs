namespace Mermish

type IMermaidChart =
    abstract member MermaidSyntax : string

type IMermaidChartDebug =
    abstract member Chart : IMermaidChart
