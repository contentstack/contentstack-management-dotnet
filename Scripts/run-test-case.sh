#!/bin/sh

#  run-test-case.sh
#  Contentstack
#
#  Created by Uttam Ukkoji on 12/04/21.
#  Copyright © 2026 Contentstack. All rights reserved.

echo "Removing files"

TEST_TARGETS=('Contentstack.Management.Core.Unit.Tests' 'Contentstack.Management.Core.Tests')

for i in "${TEST_TARGETS[@]}"
do
   rm -rf "$i/TestResults"
done

DATE=$(date +'%d-%b-%Y')

FILE_NAME="Contentstack-DotNet-Test-Case-$DATE"

SDK_VERSION=$(grep -m1 '<Version>' Contentstack.Management.Core/contentstack.management.core.csproj | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/' | tr -d '[:space:]')
export SDK_VERSION="${SDK_VERSION:-unknown}"
export BUILD_NUMBER="${BUILD_NUMBER:-local}"
export COMMIT_SHA=$(git rev-parse --short HEAD 2>/dev/null || echo "unknown")
export TEST_ENV="${TEST_ENV:-integration}"

echo "Running test case..."
dotnet test --logger "trx;LogFileName=Report-$FILE_NAME.trx" --collect:"XPlat code coverage"

echo "Test case Completed..."

echo "Generating code coverage report..."

for i in "${TEST_TARGETS[@]}"
do
    cd "$i"
    reportgenerator "-reports:**/**/coverage.cobertura.xml" "-targetdir:TestResults/Coverage-$FILE_NAME" -reporttypes:HTML
    cd ..
done

echo "Code coverage report generate."

echo "Generating API surface (method coverage)..."
dotnet run --project tools/ApiSurface/ApiSurface.csproj -- --output "tools/ApiSurface/api-surface.json" 2>/dev/null || true

echo "Generating enhanced test report..."
mkdir -p TestResults
dotnet run --project tools/EnhancedTestReport/EnhancedTestReport.csproj -- \
  --trx-dir "Contentstack.Management.Core.Unit.Tests/TestResults" \
  --trx-dir "Contentstack.Management.Core.Tests/TestResults" \
  --cobertura-dir "Contentstack.Management.Core.Unit.Tests/TestResults" \
  --cobertura-dir "Contentstack.Management.Core.Tests/TestResults" \
  --api-surface "tools/ApiSurface/api-surface.json" \
  --js-api "tools/js-cma-api.json" \
  --output "TestResults/EnhancedReport-$FILE_NAME.html"
echo "Enhanced report written to TestResults/EnhancedReport-$FILE_NAME.html"
