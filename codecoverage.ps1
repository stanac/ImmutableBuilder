cd tests\ImmutableBuilder.Tests

$remFile="bin\Debug\netcoreapp2.0\ImmutableBuilder.pdb"

If (Test-Path $remFile){
    Remove-Item $remFile
}

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

if (-not (Test-Path "dotnettools\csmacnz.Coveralls.exe"))
{
	dotnet tool install coveralls.net --version 1.0.0 --tool-path dotnettools
}

echo "PS location: $PWD"

# Get-ChildItem "*" | ForEach-Object {start-process $_.FullName –Verb Print}

.\dotnettools\csmacnz.coveralls --opencover -i coverage.opencover.xml --repoToken $env:COVERALL_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID

cd ..

cd ..