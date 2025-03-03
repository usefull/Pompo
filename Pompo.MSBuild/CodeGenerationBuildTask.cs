using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace Pompo
{
    public class CodeGenerationBuildTask : Task
    {
        [Required]
        public string Src { get; set; }

        [Required]
        public string Dest { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Pompo is here !!!");
            Log.LogMessage(MessageImportance.High, Src);
            Log.LogMessage(MessageImportance.High, Dest);

            File.WriteAllText(
                Dest,
                File.ReadAllText(Src).Replace("/*", string.Empty).Replace("*/", string.Empty)
            );

            return true;
        }
    }
}