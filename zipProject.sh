mkdir UnitySourceCode
cp  -r Assets/ UnitySourceCode/
cp  -r ProjectSettings/ UnitySourceCode/
cp -r Packages/ UnitySourceCode/
cp Assembly-CSharp.csproj UnitySourceCode/
cp IAJ-Decision Making.sln UnitySourceCode/
cp report/report.pdf ./report.pdf
zip -r  P2Group04 UnitySourceCode/ report.pdf
rm -rf UnitySourceCode
rm report.pdf
