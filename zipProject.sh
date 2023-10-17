mkdir UnitySourceCode
cp  -r Assets/ UnitySourceCode/
cp  -r ProjectSettings/ UnitySourceCode/
cp -r Packages/ UnitySourceCode/
cp report/report.pdf ./report.pdf
zip -r  Group04_IAJ-DecisionMakink UnitySourceCode/ report.pdf
rm -rf UnitySourceCode
rm report.pdf
