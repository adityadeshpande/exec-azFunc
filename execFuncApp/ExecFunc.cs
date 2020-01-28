using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace execFuncApp
{
    public static class ExecFunc
    {
        [FunctionName("ExecFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            // you may have better ways to do this, for demonstration purpose i have chosen to keep things simple and declare varables locally. 
            string WorkingDirectoryInfo = @"D:\home\site\wwwroot\ExecFunc";
            string ExeLocation = @"D:\home\site\wwwroot\helloworld.exe";

            var msg = "";       //Final Message that is passed to function 
            var output = "";    //Intercepted output, Could be anything - String in this case. 

            try
            {
                //msg = $"WorkingDirectoryInfo : {WorkingDirectoryInfo} \n" +
                //      $"ExeLocation : {ExeLocation} \n" +
                //      $"FunctionDirectory: {executionContext.FunctionDirectory} \n" +
                //      $"FunctionAppDirectory: {executionContext.FunctionAppDirectory} ";

                // Values that needs to be set before starting the process. 
                ProcessStartInfo info = new ProcessStartInfo
                {
                    WorkingDirectory = WorkingDirectoryInfo,
                    FileName = ExeLocation,
                    Arguments = "",
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process proc = new Process
                {
                    StartInfo = info
                };

                //  Discard any information about the associated process that has been cached inside the process component.
                proc.Refresh();

                // for the textual output of an application to written to the System.Diagnostics.Process.StandardOutput stream. 
                proc.StartInfo.RedirectStandardOutput = true;

                // Starts the Process, with the above configured values. 
                proc.Start();

                // Scanning the entire stream and reading the output of application process and writing it to a local variable. 
                while (!proc.StandardOutput.EndOfStream)
                {
                    output = proc.StandardOutput.ReadLine();
                }

                // More things that can be done with applications. :) 
                // proc.WaitForInputIdle();
                // proc.WaitForExit();

                msg = $"HelloWorld.exe {DateTime.Now} : HAHAHAH!, Should work! Output: {output}";
            }
            catch (Exception e)
            {
                msg = $"HelloWorld.exe {DateTime.Now} : DAMN-IT!, Failed Somewhere! Output: {e.Message}";
            }

            //Logging Output, you can be more creative than me.
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"{msg}");

            return (ActionResult)new OkObjectResult($"{msg}");
        }
    }
}
