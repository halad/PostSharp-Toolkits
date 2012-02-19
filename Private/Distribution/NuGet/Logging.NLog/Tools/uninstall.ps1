param($installPath, $toolsPath, $package, $project)

$path = [System.IO.Path]

# Set the psproj name to be the Project's name, i.e. 'ConsoleApplication1.psproj'
$psprojectName = $project.Name + ".psproj"

# Check if the file previously existed in the project
$psproj = $project.ProjectItems | where { $_.Name -eq $psprojectName }
if (!$psproj)
{
  return
}

$psprojectFile = $psproj.Properties.Item("FullPath").Value
$xml = [xml](Get-Content $psprojectFile)

# Remove the logging backend property
$xml.Project.Property | where { $_.Name -eq 'LoggingBackend' } | foreach {
  $_.ParentNode.RemoveChild($_)
  Write-Output "Removed property LoggingBackend"
}

# Remove the weaver
$xml.Project.Using | where { $_.File -like '*PostSharp.Toolkit.Diagnostics.Weaver.NLog.dll*' } | foreach {
  $_.ParentNode.RemoveChild($_)
  Write-Output "Removed the NLog weaver"
}

$xml.Save($psprojectFile)