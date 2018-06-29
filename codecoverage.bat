cd test\ImmutableBuilder.Tests

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

nuget install coveralls.net -Version 1.0.0 -OutputDir nugettools

.\nugettools\coveralls.net.0.7.0\tools\csmacnz.Coveralls.exe --opencover -i tests\ImmutableBuilder.Tests\coverage.opencover.xml --repoToken $env:COVERALL_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID
