param($installPath,
    $toolsPath,
    $package,
    $project)

$toolsPath = "D:\testapp\ConsoleApplication1\packages\PostSharpToolkits.0.1\tools"
$installPath = "D:\testapp\ConsoleApplication1\packages\PostSharpToolkits.0.1"

Set-Project ConsoleApplication1 D:\testapp\ConsoleApplication1\ConsoleApplication1.sln

$project = Get-Project -Name ConsoleApplication1

$psprojectfile = [System.IO.Path]::ChangeExtension($project.FileName, ".psproj")
$psproject = New-Object "System.Xml.XmlDocument"
$psproject.Save($psprojectfile);
