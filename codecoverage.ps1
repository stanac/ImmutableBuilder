cd tests\ImmutableBuilder.Tests

del bin\Debug\netcoreapp2.0\ImmutableBuilder.pdb

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

dotnet tool install coveralls.net --version 1.0.0 --tool-path dotnettools

.\dotnettools\csmacnz.coveralls --opencover -i tests\ImmutableBuilder.Tests\coverage.opencover.xml --repoToken $env:COVERALL_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID
