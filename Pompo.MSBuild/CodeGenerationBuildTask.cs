using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Pompo
{
    public class CodeGenerationBuildTask : Task
    {
        //[Required]
        //public string SrcFolderPath { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Pompo is here !!!");
            return true;
        }
    }
}