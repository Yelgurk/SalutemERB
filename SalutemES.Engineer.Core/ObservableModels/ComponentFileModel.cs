using SalutemES.Engineer.Domain;
using SalutemES.Engineer.SourceGenerator;

namespace SalutemES.Engineer.Core;

[ModelBySourceProperties(typeof(ComponentFileBase))]
public partial class ComponentFileModel
{
    public string FileInfo => $"{this.LocalFilePath.Split("\\").Last()} [{this.LocalFilePath}]";
}
