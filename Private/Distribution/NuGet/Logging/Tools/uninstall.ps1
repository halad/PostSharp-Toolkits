param($installPath, $toolsPath, $package, $project)

$path = [System.IO.Path]

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

$xml.Project.Using | where { $_.File -like '*PostSharp.Toolkit.Diagnostics.Weaver.dll*' } | foreach {
  $_.ParentNode.RemoveChild($_)
  "Removed the PostSharp Logging weaver"
}

$xml.Save($psprojectFile)