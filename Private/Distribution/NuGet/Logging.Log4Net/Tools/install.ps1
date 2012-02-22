param($installPath, $toolsPath, $package, $project)

$path = [System.IO.Path]

# remove the PostSharpDummy.txt file, required to workaround the issue http://nuget.codeplex.com/workitem/595
$item = $project.ProjectItems.Item("PostSharpDummyFile.txt")
if ($item)
{
  $item.Delete()
}

# Set the psproj name to be the Project's name, i.e. 'ConsoleApplication1.psproj'
$psprojectName = $project.Name + ".psproj"

# Check if the file previously existed in the project
$psproj = $project.ProjectItems | where { $_.Name -eq $psprojectName }

# If this item already exists, load it
if (!$psproj)
{
  return
} 

$psprojectFile = $psproj.Properties.Item("FullPath").Value
$xml = [xml](Get-Content $psprojectFile)

$defaultUsing = $xml.Project.Using | where { $_.File -eq 'default' }
if (!$defaultUsing)
{
	return
}

$loggingBackendProperty = $xml.Project.Property | where { $_.Name -eq 'LoggingBackend' }
if (!$loggingBackendProperty)
{
  $loggingBackendProperty = $xml.CreateElement("Property", "http://schemas.postsharp.org/1.0/configuration")
	$loggingBackendProperty.SetAttribute("Name", "LoggingBackend")
	$xml.Project.InsertBefore($loggingBackendProperty, $defaultUsing)
}

$loggingBackendProperty.SetAttribute("Value", "log4net")

$weaverFile = $path::Combine($toolsPath, "PostSharp.Toolkit.Diagnostics.Weaver.Log4Net.dll")

# Make the path to the targets file relative.
$projectUri = new-object Uri('file://' + $psprojectFile)
$targetUri = new-object Uri('file://' + $weaverFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)


$toolkitWeaver = $xml.Project.Using | where { $_.File -like '*PostSharp.Toolkit.Diagnostics.Weaver.dll*'}
if ($toolkitWeaver)
{
	$nlogWeaver = $xml.CreateElement("Using", "http://schemas.postsharp.org/1.0/configuration")
	$nlogWeaver.SetAttribute("File", $relativePath) 
	$xml.Project.InsertAfter($nlogWeaver, $toolkitWeaver)
} 

$xml.Save($psprojectFile)
