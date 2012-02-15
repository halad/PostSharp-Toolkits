param($installPath, $toolsPath, $package, $project)

$path = [System.IO.Path]
$xml = [xml] '<?xml version="1.0" encoding="utf-8"?><Project xmlns="http://schemas.postsharp.org/1.0/configuration" ReferenceDirectory="{$ReferenceDirectory}"></Project>'

# Set the psproj name to be the Project's name, i.e. 'ConsoleApplication1.psproj'
$psprojectName = $project.Name + ".psproj"

# Check if the file previously existed in the project
$psproj = $project.ProjectItems | where { $_.Name -eq $psprojectName }

# If this item already exists, load it
if ($psproj)
{
  $psprojectFile = $psproj.Properties.Item("FullPath").Value;
	$xml = [xml](Get-Content $psprojectFile)
} 
else 
{
	# Create a file on disk, write XML, and load it into the project.
	$psprojectFile = $path::ChangeExtension($project.FileName, ".psproj")
	$xml.Save($psprojectFile)
	$project.ProjectItems.AddFromFile($psprojectFile)
}

# Find the default 'Using' node
$defaultUsing = $xml.Project.Using | where { $_.File -eq 'default' }
if (!$defaultUsing)
{
	$defaultUsing = $xml.CreateElement("Using", "http://schemas.postsharp.org/1.0/configuration");
	$defaultUsing.SetAttribute("File", "default");
	$xml.Project.AppendChild($defaultUsing);
}

$weaverFile = $path::Combine($toolsPath, "PostSharp.Toolkit.Diagnostics.Weaver.dll");

# Make the path to the targets file relative.
$projectUri = new-object Uri('file://' + $psprojectFile)
$targetUri = new-object Uri('file://' + $weaverFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

$toolkitWeaver = $xml.Project.Using | where { $_.File -like '*PostSharp.Toolkit.Diagnostics.Weaver.dll*'}
if ($toolkitWeaver)
{
	$toolkitWeaver.SetAttribute("File", $relativePath)
} 
else 
{
	$toolkitWeaver = $xml.CreateElement("Using", "http://schemas.postsharp.org/1.0/configuration");
	$toolkitWeaver.SetAttribute("File", $relativePath);
	$xml.Project.InsertAfter($toolkitWeaver, $defaultUsing)
}

$xml.Save($psprojectFile)
