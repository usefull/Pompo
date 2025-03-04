using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace Pompo
{
    public class PostGenerationBuildTask : Task
    {
        [Required]
        public string Src { get; set; }

        [Required]
        public string Dest { get; set; }

        public override bool Execute()
        {
            File.WriteAllText(
                Dest,
                File.ReadAllText(Src).Replace("/*", string.Empty).Replace("*/", string.Empty)
            );

            Log.LogMessage(MessageImportance.High, $"Pompo JS transmitter has uncommented and copied:");
            Log.LogMessage(MessageImportance.High, $"\tfrom {Src}");
            Log.LogMessage(MessageImportance.High, $"\tto {Dest}");

            return true;
        }
    }
}