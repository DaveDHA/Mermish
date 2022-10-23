namespace Mermish

// Just here to make a string implement IMermaidChart

[<StructuredFormatDisplay("{MermaidSyntax}")>]
type ManualChart = internal ManualChart of string
    with
        member this.MermaidSyntax = let (ManualChart str) = this in str

        override this.ToString() = this.MermaidSyntax

        interface IMermaidChart with member this.MermaidSyntax = this.MermaidSyntax