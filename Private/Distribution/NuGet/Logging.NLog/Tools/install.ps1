param($installPath, $toolsPath, $package, $project)

$path = [System.IO.Path]

# Set the psproj name to be the Project's name, i.e. 'ConsoleApplication1.psproj'
$psprojectName = $project.Name + ".psproj"

# Find the Template file as placed by the NuGet script
$psproj = $project.ProjectItems.Item("Template.psproj");

# Set the new filename to the Template
$psproj.Name = $psprojectName;

$psprojectFile = $path::ChangeExtension($project.FileName, ".psproj")
$weaverFile = $path::Combine($toolsPath, "PostSharp.Toolkit.Instrumentation.Weaver.dll");

# Make the path to the targets file relative.
$projectUri = new-object Uri('file://' + $psprojectFile)
$targetUri = new-object Uri('file://' + $weaverFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

$xml = [xml](Get-Content $psprojectFile)
$element = $xml.CreateElement("Using", "http://schemas.postsharp.org/1.0/configuration");
$element.SetAttribute("File", $relativePath);
$xml.Project.AppendChild($element);
$xml.Save($psprojectFile)