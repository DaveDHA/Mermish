namespace Mermish.Interactive

open Mermish
open Microsoft.DotNet.Interactive
open Microsoft.DotNet.Interactive.Formatting
open System
open System.IO
open System.Threading.Tasks


module Formatting = 

    let html = """
<!-- Begin Mermish Content -->
<div style="background-color:white">
    <script type="text/javascript">
        renderMermish_[%mermish_guid%] = () => {
            (require.config({ 'paths': { 'context': 'mermish', 'mermaidUri' : 'https://cdn.jsdelivr.net/npm/mermaid@9.1.3/dist/mermaid.min' }}) || require)(['mermaidUri'], (mermaid) => {
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


    let format chart (writer : TextWriter) =
        html.Replace("[%mermish_guid%]", Guid.NewGuid().ToString().Replace("-",""))
            .Replace("[%mermish_syntax%]", (Mermaid.ToSyntax chart))
        |> writer.Write        


type KernelExtension() =
                        
    interface IKernelExtension with
        
        member this.OnLoadAsync _ =
            
            Formatter.Register<PieChart>(Action<_,_> Formatting.format, "text/html")
            Task.CompletedTask

