module Mermish.InteractiveSupport

open Mermish
open System
open System.IO



[<Literal>]
let private html = """
<!-- Begin Mermish Content -->
<div style="background-color:white">
    <script type="text/javascript">
        renderMermish_[%mermish_guid%] = () => {
            (require.config({ 'paths': { 'context': 'mermish', 'mermaidUri' : 'https://cdn.jsdelivr.net/npm/mermaid@9.1.7/dist/mermaid.min' }}) || require)(['mermaidUri'], (mermaid) => {
                let renderTarget = document.getElementById('mermishTarget_[%mermish_guid%]');
                mermaid.mermaidAPI.render( 
                    'mermishScratch_[%mermish_guid%]', 
                    `[%mermish_syntax%]`, 
                    g => { renderTarget.innerHTML = g });
            },
            (error) => { console.log(error); });
        }

        if ((typeof(require) !==  typeof(Function)) || (typeof(require.config) !== typeof(Function))) {
            let require_script = document.createElement('script');
            require_script.setAttribute('src', 'https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js');
            require_script.setAttribute('type', 'text/javascript');
            require_script.onload = function() { renderMermish_[%mermish_guid%](); };
            document.getElementsByTagName('head')[0].appendChild(require_script);
        }
        else {
            renderMermish_[%mermish_guid%]();
        }
    </script>
    <div id="mermishTarget_[%mermish_guid%]"></div>
</div>
<!-- End Mermish Content -->
"""


let FormatChartForDotNetInteractive (chart : IMermaidChart) (writer : TextWriter) =
    html.Replace("[%mermish_guid%]", Guid.NewGuid().ToString().Replace("-",""))
        .Replace("[%mermish_syntax%]", chart.MermaidSyntax.Trim())
    |> writer.Write
        
