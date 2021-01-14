dotnet tool restore
dotnet format --check
dotnet jb inspectcode kyoseki.Desktop.slnf --output="inspectcodereport.xml" --caches-home="inspectcode" --verbosity=INFO
dotnet nvika parsereport "inspectcodereport.xml" --treatwarningsaserrors
pause