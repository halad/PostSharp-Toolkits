param($installPath, $toolsPath, $package, $project)

# remove the PostSharpDummy.txt file, required to workaround the issue http://nuget.codeplex.com/workitem/595
$item = $project.ProjectItems.Item("PostSharpDummyFile.txt")
if ($item)
{
  $item.Delete()
}